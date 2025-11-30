using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement; 
using System.Collections; // 코루틴 사용을 위해 추가

public class end : MonoBehaviour
{
    // 🎥 인스펙터에 할당할 비디오 플레이어 컴포넌트
    public VideoPlayer videoPlayer; 
    
    // 🎞️ 재생할 비디오 클립 (프로젝트 창의 비디오 파일)
    public VideoClip endingVideoClip;
    
    // 🔊 영상 종료 후 재생할 AudioSource
    public AudioSource end_scene_sound;

    private void Start()
    {
        // 1. 필수 컴포넌트 할당 확인
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

        // 2. 비디오 클립 설정
        videoPlayer.clip = endingVideoClip;

        // 3. 비디오가 끝나면 자동으로 호출될 이벤트 등록
        videoPlayer.loopPointReached += OnVideoEnd;

        // 4. ✨ 준비 및 재생 코루틴 시작
        StartCoroutine(PrepareAndPlay());
        
        // 5. 비디오가 끝나기 전에는 소리가 나지 않도록 AudioSource의 Play On Awake를 꺼두세요.
    }

    public void EXIT()
    {
        Debug.Log("종료 요청");
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
        // Prepare()를 호출하여 비디오 로드 및 디코딩 준비를 시작합니다.
        videoPlayer.Prepare();
        Debug.Log("엔딩 비디오 준비 중...");

        // 비디오 준비(isPrepared)가 true가 될 때까지 매 프레임 대기합니다.
        while (!videoPlayer.isPrepared)
        {
            yield return null; // 다음 프레임까지 대기
        }
        yield return new WaitForSeconds(0.1f);
        // 준비 완료 후 비디오 재생 시작
        videoPlayer.Play();
        Debug.Log("엔딩 비디오 재생 시작.");
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        Debug.Log("엔딩 비디오 재생 종료. 다음 동작 준비 중...");
        
        // 1. 비디오 화면 오브젝트 비활성화 (화면 끄기)
        vp.gameObject.SetActive(false); 
        
        // 2. 영상 종료 후 사운드 재생
        if (end_scene_sound != null)
        {
            end_scene_sound.Play();
        }
    }
}