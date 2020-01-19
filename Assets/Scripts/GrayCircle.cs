using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]

public enum MaskType {OFF, GONE, GRAY, FADE};

[ExecuteInEditMode]
public class GrayCircle : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private TilemapRenderer tileRenderer;
    public MaskType type = MaskType.OFF;
    public float distance = 0;
    public static GameObject target;
    public bool rageOn = false;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null) tileRenderer = GetComponent<TilemapRenderer>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) target = GameObject.FindGameObjectWithTag("Player");
        updateShader();
        toggleShader();
    }

    private int typeToInt()
    {
        if (distance <= 0 || type == MaskType.OFF) return 0;
        switch(type)
        {
        case MaskType.GONE: return 1;
        case MaskType.GRAY: return 2;
        case MaskType.FADE: return 3;
        }
      return 0;
    }

    private void toggleShader()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            rageOn = !rageOn;
        }
        
        if (rageOn)
        {
            type = MaskType.GRAY;
            distance = Mathf.Clamp(distance + (Time.deltaTime * 3), 0, 10);
        }
        else if (!rageOn)
        {
            distance = Mathf.Clamp(distance - (Time.deltaTime * 3), 0, 10);
            if (distance <= 0)
            {
                type = MaskType.OFF;
            }
            
        }
    }

    private void updateShader()
    {
        if (spriteRenderer == null && tileRenderer == null || target == null) return;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        if(spriteRenderer != null) spriteRenderer.GetPropertyBlock(mpb);
        if (tileRenderer !=null) tileRenderer.GetPropertyBlock(mpb);

        mpb.SetFloat ("_RenderDistance", distance);
        mpb.SetFloat ("_MaskTargetX", target.transform.position.x);
        mpb.SetFloat ("_MaskTargetY", target.transform.position.y);
        mpb.SetFloat ("_MaskType", typeToInt());

        if (spriteRenderer != null) spriteRenderer.SetPropertyBlock(mpb);
        if (tileRenderer !=null) tileRenderer.SetPropertyBlock(mpb);
    }

}
