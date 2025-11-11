using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class platform_spawner : MonoBehaviour
{
    public GameObject platformPrefab;  // 리젠할 발판 프리팹
    private GameObject currentPlatform;

    public float spawn_time = 5.0f;
    float timer;

    void Start()
    {
        timer = spawn_time;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            SpawnPlatform();
            timer = spawn_time;
        }
    }
    public void SpawnPlatform()
    {
        currentPlatform = Instantiate(
            platformPrefab,
            transform.position,  // 스포너 위치에서 생성
            Quaternion.identity
        );
    }
}
