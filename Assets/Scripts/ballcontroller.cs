using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ballcontroller : MonoBehaviour
{
    public InputAction MoveAction;
    Rigidbody2D rigidbody2d;
    public float max_distance = 4.0f;
    float current_max_distance;

    bool is_stopped = true;

    bool on_calculate;

    LineRenderer lineRenderer;

    Vector2 start_point = Vector2.zero;
    Vector2 end_point = Vector2.zero;

    float distance;
    Vector2 direction;
    public Vector2 kicked_direction = new Vector2(-1f, 1.3f).normalized;

    public int max_jumpcount = 2;
    int current_jumpcount;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        current_jumpcount = max_jumpcount;
        current_max_distance = max_distance;
        on_calculate = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (rigidbody2d.velocity.magnitude < 0.005f && IsGrounded())
        {
            is_stopped = true;
            current_jumpcount = max_jumpcount; // 점프 횟수 충전
            current_max_distance = max_distance; // 점프 거리 충전
        }
        else
        {
            is_stopped = false;
        }

        if (current_jumpcount <=0 && !is_stopped)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0)) // 처음 눌렸을때 시작점 잡기
        {
            start_point = Input.mousePosition;
            lineRenderer.enabled = true;
            on_calculate = true;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!on_calculate){ return; }
            Vector2 current = Input.mousePosition;
            Vector2 screenVec = current - start_point;
            float screenDist = screenVec.magnitude;
            Vector2 screenDir = screenVec.normalized;

            Vector3 worldDir = (Vector3)(screenDir * screenDist * 0.015f);

            Vector3 worldStart = transform.position;
            Vector3 worldEnd = worldStart + worldDir;

            lineRenderer.SetPosition(0, worldStart);
            lineRenderer.SetPosition(1, worldEnd);
        }
        else if (Input.GetMouseButtonUp(0)) // 커서 뗄때 끝점 잡기
        {
            if (!on_calculate){ return; }
            // 끝점 계산 및 점프 가능 횟수 최신화
            end_point = Input.mousePosition;
            lineRenderer.enabled = false;
            current_jumpcount -= 1;

            //여기부터 start_point부터 end_point 거리 기반 발사 계산
            direction = (start_point - end_point).normalized;
            distance = Vector2.Distance(start_point, end_point)/65; // 타일 한칸 길이가 54임
            distance = Mathf.Min(distance, current_max_distance);

            rigidbody2d.AddForce(direction * distance * 4f, ForceMode2D.Impulse);
            on_calculate = false;

            start_point = Vector2.zero;
            end_point = Vector2.zero;
            if (current_jumpcount < max_jumpcount)
            {
                current_max_distance = 0.85f * current_max_distance;
            }
        }
    }

    void FixedUpdate()
    {   // 업데이트는 프레임단위 계산 fixedupdate는 일정시간 단위 계산이기 때문에 거리계산 분리하면 뚝뚝 끊기는 움직임 발생
        // if (end_point != Vector2.zero && start_point != Vector2.zero)
        // {
        //     direction = (start_point - end_point).normalized;
        //     distance = Vector2.Distance(start_point, end_point);
        //     distance = Mathf.Min(distance, current_max_distance);

        //     rigidbody2d.AddForce(direction * distance * 4f, ForceMode2D.Impulse);

        //     start_point = Vector2.zero;
        //     end_point = Vector2.zero;
        //     if (current_jumpcount < max_jumpcount)
        //     {
        //         current_max_distance = 0.85f * current_max_distance;
        //     }
        // }
    }

    public void KickedbyPlayer(int mult)
    {
        rigidbody2d.velocity = Vector2.zero;
        rigidbody2d.AddForce(kicked_direction * (max_distance * mult) * 4f, ForceMode2D.Impulse);
    }

    public void enterPortal()
    {
        rigidbody2d.position = Vector2.zero;
    }

    bool IsGrounded()
    {
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        float radius = circle.radius * transform.localScale.y;
        float checkDistance = radius + 0.02f; // 살짝 여유 줘야 정확히 닿았을 때 반응함

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, checkDistance, LayerMask.GetMask("Ground"));
        return hit.collider != null;
    }

}
