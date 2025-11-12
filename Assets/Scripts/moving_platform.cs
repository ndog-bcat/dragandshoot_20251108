using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_platform : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float mx_height = 30f;
    Rigidbody2D rigidbody2d;
    Vector2 direction = Vector2.zero;
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        direction = Vector2.up;
    }

    void Update()
    {        
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position = position + moveSpeed * direction * Time.deltaTime;
        rigidbody2d.MovePosition(position);
        if (position.y >= mx_height)
        {
            Destroy(gameObject);
        }
    }
}
