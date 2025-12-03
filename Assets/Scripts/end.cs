using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; 
using System.Collections;

public class end : MonoBehaviour
{
    public VideoPlayer videoPlayer; 
    public VideoClip endingVideoClip;
    public AudioSource end_scene_sound;

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
    }
}