using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; 
using System.Collections;
using TMPro;

public class end : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public VideoClip endingVideoClip;
    public AudioSource end_scene_sound;
    public GameObject resultUI;
    public TMP_Text recordText;

    private void Start()
    {
        if (videoPlayer == null)
        {
            Debug.LogError("VideoPlayer 컴포넌트가 할당되지 않았습니다. 인스펙터에서 할당해주세요.");
            return;
        }
        if (endingVideoClip == null)
        {
            Debug.LogError("Ending Video Clip이 할당되지 않았습니다.");
            return;
        }
        videoPlayer.clip = endingVideoClip;
        videoPlayer.loopPointReached += OnVideoEnd;
        StartCoroutine(PrepareAndPlay());
    }

    public void EXIT()
    {
        Application.Quit();
        #if UNITY_EDITOR
        // 에디터에서만 실행되는 코드 (테스트용)
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("stage1");
    }

    IEnumerator PrepareAndPlay()
    {
        // Prepare() 호출, 비디오 로드 및 디코딩 준비
        videoPlayer.Prepare();

        // 비디오 준비(isPrepared)가 true가 될 때까지 대기
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        // 준비 완료 후 비디오 재생 시작
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        videoPlayer.Play();
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        // 1. 비디오 화면 오브젝트 비활성화 (화면 끄기)
        vp.gameObject.SetActive(false); 
        
        // 2. 영상 종료 후 사운드 재생
        if (end_scene_sound != null)
        {
            end_scene_sound.Play();
        }
        ShowRecords();
        if (resultUI != null)
        {
            resultUI.SetActive(true);
        }
    }

    void ShowRecords()
    {
        if (Record_manager.Instance == null || recordText == null)
        {
            Debug.LogError("할당안됐어");
            return;
        }

        // 문자열을 빌드할 때 가독성을 위해 상단에 타이틀 추가
        string finalReport = "<size=120%><b>[ STAGE RECORDS ]</b></size>\n\n";
        
        foreach (var record in Record_manager.Instance.stageRecords)
        {
            // 실제 데이터가 있는(플레이한) 스테이지 기록만 표시하거나 모두 표시
            if (record.Value[0] <= 0 && record.Value[1] <= 0) continue; 

            string stageName = record.Key;
            float time = record.Value[0];
            int kicked = (int)record.Value[1];
            int toZero = (int)record.Value[2];

            string timeStr = string.Format("{0:00}:{1:00}", (int)time / 60, (int)time % 60);
            
            // 축구 컨셉에 맞춘 텍스트 구성
            finalReport += $"<b>{stageName}</b> : {timeStr} | Kicked: {kicked} | Restart: {toZero}\n";
        }

        recordText.text = finalReport;
    }
}