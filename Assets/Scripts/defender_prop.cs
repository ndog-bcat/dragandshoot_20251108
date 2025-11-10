using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class defender_ver1 : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        ballcontroller player = other.gameObject.GetComponent<ballcontroller>();
        if (player != null)
        {
            player.KickedbyPlayer(4);
        }
    }
}
