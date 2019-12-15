using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{

private Player player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("player").GetComponent<Player>();

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("player"))
        {
            player.Damage(1);

            //because it's an IENumerator function
            StartCoroutine(player.Knockback(0.02f, 100, player.transform.position));
        }

    }
}
