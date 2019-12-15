using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Sprite[] HeartSprites;
    
    public Image HeartUI;

    private Player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player").GetComponent<Player>();

    }

    void Update()
    {
        HeartUI.sprite = HeartSprites[player.health]; 
    }

}
