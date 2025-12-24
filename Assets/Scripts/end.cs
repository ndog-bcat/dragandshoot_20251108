using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; 
using System.Collections;
using TMPro; // 텍스트 출력을 위해 추가

public class end : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public VideoClip endingVideoClip;
    public AudioSource end_scene_sound;

    [Header("UI 연동")]
    public GameObject recordPanel;   // 기록창 판 (배경+텍스트 포함)
    public TMP_Text recordText;      // 기록이 써질 TMP 텍스트
    public GameObject finalButtons;  // 마지막에 뜰 버튼들 (Tutorial, EXIT)

    private bool record_showing = false;

    private void Start()
    {
        // 초기 상태 설정
        if (recordPanel != null) recordPanel.SetActive(false);
        if (finalButtons != null) finalButtons.SetActive(false);

        if (videoPlayer == null || endingVideoClip == null)
        {
            Debug.LogError("비디오 설정이 누락되었습니다.");
            return;
        }

        videoPlayer.clip = endingVideoClip;
        videoPlayer.loopPointReached += OnVideoEnd;
        StartCoroutine(PrepareAndPlay());
    }

    void Update()
    {
        // 기록이 보여지고 있을 때 아무 키나 누르면
        if (record_showing && Input.anyKeyDown)
        {
            record_showing = false; // 중복 실행 방지
            
            if (recordPanel != null) recordPanel.SetActive(false); // 기록창 끄기
            if (finalButtons != null) finalButtons.SetActive(true); // 버튼들 켜기
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        vp.gameObject.SetActive(false); 
        
        if (end_scene_sound != null) end_scene_sound.Play();

        // 영상 종료 후 기록 데이터 생성 및 표시
        ShowRecords();
    }

    void ShowRecords()
    {
        if (Record_manager.Instance == null || recordText == null) return;

        string finalReport = "<size=130%><b>[ STAGE RECORDS ]</b></size>\n\n\n";
        
        foreach (var record in Record_manager.Instance.stageRecords)
        {
            // 실제 플레이 기록이 있는 것만 표시
            if (record.Value[0] <= 0 && record.Value[1] <= 0) continue; 

            string timeStr = string.Format("{0:00}:{1:00}", (int)record.Value[0] / 60, (int)record.Value[0] % 60);
            finalReport += $"<b>{record.Key}</b> : {timeStr} | Kicks: {record.Value[1]} | Restart: {record.Value[2]}\n\n";
        }

        recordText.text = finalReport;

        // 기록창 활성화 및 입력 대기 상태 돌입
        if (recordPanel != null) recordPanel.SetActive(true);
        record_showing = true;
    }

    // --- 버튼 이벤트들 ---
    public void EXIT()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("stage1");
    }

    IEnumerator PrepareAndPlay()
    {
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared) yield return null;
        yield return new WaitForSeconds(0.1f);
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        videoPlayer.Play();
    }
}