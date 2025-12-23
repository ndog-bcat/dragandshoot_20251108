using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Record_manager : MonoBehaviour
{
    public static Record_manager Instance;
    public Dictionary<string, float[]> stageRecords = new Dictionary<string, float[]>(){
        {"stage1", new float[] {0f, 0f, 0f}},
        {"stage2", new float[] {0f, 0f, 0f}},
        {"stage3", new float[] {0f, 0f, 0f}}
    }; // float[0]: 기록 float[1]: 차인 횟수 float[2]: 태초 횟수
    private float stageStartTime;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 씬이 로드될 때마다 'OnSceneLoaded' 함수가 실행되도록 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else { Destroy(gameObject); }
    }

    // 씬이 바뀔 때마다 자동으로 실행되는 함수
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 로비나 메뉴가 아닌 '스테이지' 씬일 때만 기록 시작 (씬 이름 등으로 구분)
        if (scene.name.Contains("stage")) 
        {
            stageStartTime = Time.time;
            Debug.Log($"{scene.name} 시작! 기록 개시.");
        }
    }
    public void TimeRecord()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        stageRecords[currentScene][0] = GetElapsedSeconds();
    }

    public void KickedRecord()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        stageRecords[currentScene][1]++;
    }

    public void ToZeroRecord()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        stageRecords[currentScene][2]++;
    }


    public float GetElapsedSeconds()
    {
        return Time.time - stageStartTime;
    }
}
