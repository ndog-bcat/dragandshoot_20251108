using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class opposite_player : MonoBehaviour
{
    Animator animator;
    public float speed;
    public float changeTime = 3.0f;
    Rigidbody2D rigidbody2d;
    float timer;
    int direction;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        direction = Random.value < 0.5f ? -1 : 1;
    }
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * direction * Time.deltaTime;
        // animator.SetFloat("Move X", direction);
        // animator.SetFloat("Move Y", 0);
        rigidbody2d.MovePosition(position);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        ballcontroller player = other.gameObject.GetComponent<ballcontroller>();
        if (player != null)
        {
            player.KickedbyPlayer(2);
        }
        if (other.CompareTag("Tile") || other.CompareTag("Platform"))
        {
            direction = -direction;
            timer = changeTime;
        }
        if (other.CompareTag("TurnTrigger"))
        {
            direction = -direction;
            timer = changeTime;
        }
    }
}
