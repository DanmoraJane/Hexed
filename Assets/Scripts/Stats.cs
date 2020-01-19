using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{

    private Image content;
    [SerializeField] private float lerpSpeed;
    private float curFill;
    public float MaxVal { get; set;}
    private float curVal;

    public float MyCurVal
    {
        get 
        {
            return curVal;
        }
        set
        {
            if (value > MaxVal)
            {
                curVal = MaxVal;
            } else if (value < 0)
            {
                curVal = 0;
            } else
            {
                curVal = value;
            }
             curFill = curVal / MaxVal;
        }

    }

    void Start()
    {
        content = GetComponent<Image>();
    }

    void Update()
    {
        if (curFill != content.fillAmount)
        {
            content.fillAmount = Mathf.Lerp(content.fillAmount, curFill, Time.deltaTime * lerpSpeed);
        }
    }

    public void Initialize(float currentVal, float maximumVal)
    {
        MaxVal = maximumVal;
        MyCurVal = currentVal;
    }

}
