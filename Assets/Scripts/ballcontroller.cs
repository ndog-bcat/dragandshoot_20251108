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
    public float max_distance = 15.0f;
    float current_max_distance;

    bool is_stopped = true;

    bool on_calculate;

    LineRenderer lineRenderer;

    Vector2 start_point = Vector2.zero;
    Vector2 end_point = Vector2.zero;

    float distance;
    Vector2 direction;
    public Vector2 kicked_direction = new Vector2(-2f, 1.3f).normalized;

    public int max_jumpcount = 2;
    int current_jumpcount;
    bool isTouchingPlatform = false;
    float relative_speed = 10f;
    Rigidbody2D platform_rigidbody2d = null;

    public float goal_point_x = 1180f;
    AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip kickedClip;
    public GameObject portal_prefab;
    private GameObject current_portal;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
        UI_handler.instance.UpdateDistanceUI(0, goal_point_x, rigidbody2d.position.x);
        if (isTouchingPlatform)
        {
            if (relative_speed < 2f)
            {
                is_stopped = true;
                current_jumpcount = max_jumpcount; // 점프 횟수 충전
                current_max_distance = max_distance; // 점프 거리 충전
            }
            else
            {
                is_stopped = false;
            }
        }
        else
        {
            if (rigidbody2d.velocity.magnitude < 0.005f)
            {
                is_stopped = true;
                current_jumpcount = max_jumpcount; // 점프 횟수 충전
                current_max_distance = max_distance; // 점프 거리 충전
            }
            else
            {
                is_stopped = false;
            }

        }
        
        if (current_jumpcount <= 0 && !is_stopped)
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
            if (!on_calculate) { return; }
            Vector2 current = Input.mousePosition;
            Vector2 screenVec = current - start_point;
            float screenDist = (current_max_distance*65 < screenVec.magnitude ? current_max_distance*65 : screenVec.magnitude); // 점프 최대 거리와 커서 거리 중 작은 값으로 라인 렌더러 길이 설정
            Vector2 screenDir = screenVec.normalized;

            Vector3 worldDir = (Vector3)(screenDir * screenDist * 0.015f);
            // Vector3 worldDir = (Vector3)(screenDir * screenDist * 1f);

            Vector3 worldStart = transform.position;
            Vector3 worldEnd = worldStart + worldDir;

            lineRenderer.SetPosition(0, worldStart);
            lineRenderer.SetPosition(1, worldEnd);
        }
        else if (Input.GetMouseButtonUp(0)) // 커서 뗄때 끝점 잡기
        {
            if (!on_calculate) { return; }
            // 끝점 계산 및 점프 가능 횟수 최신화
            end_point = Input.mousePosition;
            lineRenderer.enabled = false;
            current_jumpcount -= 1;

            //여기부터 start_point부터 end_point 거리 기반 발사 계산
            direction = (start_point - end_point).normalized;
            distance = Vector2.Distance(start_point, end_point) / 65; // 타일 한칸 길이가 54임
            distance = Mathf.Min(distance, current_max_distance);

            PlaySound(jumpClip);
            rigidbody2d.AddForce(direction * distance * 8f, ForceMode2D.Impulse);
            on_calculate = false;

            start_point = Vector2.zero;
            end_point = Vector2.zero;
            if (current_jumpcount < max_jumpcount)
            {
                current_max_distance = 0.85f * current_max_distance;
            }
        }
        UI_handler.instance.UpdateJumpUI(current_jumpcount);
    }

    public void KickedbyPlayer(int mult)
    {
        current_jumpcount = 0;
        UI_handler.instance.UpdateJumpUI(current_jumpcount);
        rigidbody2d.velocity = Vector2.zero;
        PlaySound(kickedClip);
        rigidbody2d.AddForce(kicked_direction * (max_distance * mult) * 4f, ForceMode2D.Impulse);
        UI_handler.instance.UpdateJumpUI(current_jumpcount);
    }

    IEnumerator SpawnPortalAtPosition(Vector2 spawn_point)
    {
        Vector2 dir = new Vector2(0f, 1f).normalized;
        current_portal = Instantiate(portal_prefab, spawn_point, Quaternion.identity);
        Vector2 start = spawn_point;
        Vector2 upTarget = spawn_point + new Vector2(0, 2.5f); // 위로 2.5 유닛 이동
        float t = 0f;
        float duration = 0.4f;
        rigidbody2d.velocity = Vector2.zero;
        rigidbody2d.position = spawn_point;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            current_portal.transform.position = Vector2.Lerp(start, upTarget, t);
            yield return null;
        }
        rigidbody2d.AddForce(dir * 15f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            current_portal.transform.position = Vector2.Lerp(upTarget, start, t);
            yield return null;
        }
        Destroy(current_portal);
    }

    public void enterPortal(Vector2 spawn_point)
    {
        StartCoroutine(SpawnPortalAtPosition(spawn_point));
    }
    
    // bool IsGrounded()
    // {
    //     CircleCollider2D circle = GetComponent<CircleCollider2D>();
    //     float radius = circle.radius * transform.localScale.y;
    //     float checkDistance = radius + 0.02f; // 살짝 여유 줘야 정확히 닿았을 때 반응함

    //     RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, checkDistance, LayerMask.GetMask("Ground"));
    //     return hit.collider != null;
    // }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform_collide"))
        {
            isTouchingPlatform = true;
            platform_rigidbody2d = collision.rigidbody;
            relative_speed = (rigidbody2d.velocity - platform_rigidbody2d.velocity).magnitude;
            Debug.Log("플랫폼에 닿음");
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform_collide"))
        {
            isTouchingPlatform = true;
            platform_rigidbody2d = collision.rigidbody;
            relative_speed = (rigidbody2d.velocity - platform_rigidbody2d.velocity).magnitude;
            Debug.Log("플랫폼에 닿아있는 중. 속도: " + relative_speed);
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform_collide"))
        {
            isTouchingPlatform = false;
            platform_rigidbody2d = null;
            relative_speed = 10f;
            Debug.Log("플랫폼에서 떨어짐 ❌");
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
