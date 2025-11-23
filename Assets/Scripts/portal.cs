using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portal : MonoBehaviour
{
    public Vector2 spawn_point;
    void Start()
    {
        // spawn_point = Vector2.zero;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        ballcontroller player = other.gameObject.GetComponent<ballcontroller>();
        if (player != null)
        {
            player.enterPortal(spawn_point);
        }
    }
}
