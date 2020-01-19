using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameMaster : MonoBehaviour
{
    public int coins;

    public Text coinsText;

    public static GameMaster instance;

    void OnAwake()
    {
        instance = this;
    }

    void Update()
    {
        coinsText.text = (" " + coins);
    } 

}
