using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moving_platform : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float mx_height = 20f;
    Rigidbody2D rigidbody2d;
    Vector2 direction;
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        direction = new Vector2(3f, 2f).normalized;
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
