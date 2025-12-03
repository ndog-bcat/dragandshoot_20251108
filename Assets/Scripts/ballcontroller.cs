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
    private int _current_jumpcount;
    public int CurrentJumpCount
    {
        get { return _current_jumpcount; }
        set
        {
            if (_current_jumpcount != value) // 값이 실제 달라졌을 때만 실행
            {
                _current_jumpcount = value;
                // UI 갱신 호출 (UI_handler가 존재하는지 체크)
                if (UI_handler.instance != null)
                    UI_handler.instance.UpdateJumpUI(_current_jumpcount);
            }
        }
    }
    bool isTouchingPlatform = false;
    float relative_speed = 10f;
    Rigidbody2D platform_rigidbody2d = null;

    public float goal_point_x = 1180f;
    AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip kickedClip;
    public AudioClip portalClip;
    public GameObject portal_prefab;
    private GameObject current_portal;
    
    bool is_break;
    bool is_kicked;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rigidbody2d = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        CurrentJumpCount = max_jumpcount;
        current_max_distance = max_distance;
        on_calculate = false;
    }

    void Update()
    {
        if (!is_kicked && Input.GetKeyDown(KeyCode.Space))
        {
            CurrentJumpCount = 0;
            rigidbody2d.velocity = new Vector2(0f, rigidbody2d.velocity.y);
            is_break = true;
        }
        // 공이 멈춰있지 않을 때(움직일 때)만 거리 UI 갱신
        if (!is_stopped || is_break) 
        {
            UI_handler.instance.UpdateDistanceUI(0, goal_point_x, rigidbody2d.position.x);
            if (is_break)
            {
                is_break = false;
            }
        }

        // 정지/움직임 상태 체크 로직
        CheckMovementStatus();

        // 점프 횟수 소진 시 리턴
        if (CurrentJumpCount <= 0 && !is_stopped)
        {
            return;
        }

        HandleInput();
    }

    void CheckMovementStatus()
    {
        bool shouldStop = false;

        if (isTouchingPlatform)
        {
            if (relative_speed < 2f) shouldStop = true;
        }
        else
        {
            if (rigidbody2d.velocity.magnitude < 0.005f) shouldStop = true;
        }

        if (shouldStop)
        {
            is_kicked = false;
            is_stopped = true;
            CurrentJumpCount = max_jumpcount;
            current_max_distance = max_distance;
        }
        else
        {
            is_stopped = false;
        }
    }
    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            start_point = Input.mousePosition;
            lineRenderer.enabled = true;
            on_calculate = true;
        }
        else if (Input.GetMouseButton(0))
        {
            if (!on_calculate) return;
            Vector2 current = Input.mousePosition;
            Vector2 screenVec = current - start_point;
            float screenDist = (current_max_distance * 65 < screenVec.magnitude ? current_max_distance * 65 : screenVec.magnitude);
            Vector2 screenDir = screenVec.normalized;

            Vector3 worldDir = (Vector3)(screenDir * screenDist * 0.015f);
            Vector3 worldStart = transform.position;
            Vector3 worldEnd = worldStart + worldDir;

            lineRenderer.SetPosition(0, worldStart);
            lineRenderer.SetPosition(1, worldEnd);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (!on_calculate) return;
            end_point = Input.mousePosition;
            lineRenderer.enabled = false;

            CurrentJumpCount -= 1;

            direction = (start_point - end_point).normalized;
            distance = Vector2.Distance(start_point, end_point) / 65;
            distance = Mathf.Min(distance, current_max_distance);

            PlaySound(jumpClip);
            rigidbody2d.AddForce(direction * distance * 8f, ForceMode2D.Impulse);
            on_calculate = false;

            start_point = Vector2.zero;
            end_point = Vector2.zero;
            
            if (CurrentJumpCount < max_jumpcount)
            {
                current_max_distance = 0.85f * current_max_distance;
            }
        }
    }
    public void KickedbyPlayer(int mult)
    {
        is_kicked = true;
        CurrentJumpCount = 0;
        rigidbody2d.velocity = Vector2.zero;
        PlaySound(kickedClip);
        rigidbody2d.AddForce(kicked_direction * (max_distance * mult) * 4f, ForceMode2D.Impulse);
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
        PlaySound(portalClip);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform_collide"))
        {
            isTouchingPlatform = true;
            platform_rigidbody2d = collision.rigidbody;
            relative_speed = (rigidbody2d.velocity - platform_rigidbody2d.velocity).magnitude;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform_collide"))
        {
            isTouchingPlatform = true;
            platform_rigidbody2d = collision.rigidbody;
            relative_speed = (rigidbody2d.velocity - platform_rigidbody2d.velocity).magnitude;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform_collide"))
        {
            isTouchingPlatform = false;
            platform_rigidbody2d = null;
            relative_speed = 10f;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
