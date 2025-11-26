using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections; // 👈 코루틴 사용을 위해 추가

public class manual_script : MonoBehaviour
{
    // ⚙️ 설정 값
    public VideoClip[] tutorialVideos;   // 4개의 영상 클립을 할당합니다.
    public Texture lastImageTexture;     // 마지막에 표시할 이미지 텍스처를 할당합니다.
    public VideoPlayer videoPlayer;
    public RawImage videoDisplayImage;
    public string startSceneName = "StartScene";

    // 🔊 오디오 설정 값 👈 새로 추가
    public AudioSource backgroundMusicSource; // 배경 음악을 재생할 AudioSource 컴포넌트를 할당합니다.
    public float delayDuration = 1.5f;       // 지연 시간 (3초)

    // 🔄 내부 상태 변수
    private int currentVideoIndex = 0;
    private bool isProcessing = false; // 현재 지연 처리 중인지 확인 (중복 실행 방지)

    void Start()
    {
        Cursor.visible = false;
        // 🚨 필수 컴포넌트 및 설정 검사
        if (videoPlayer == null || videoDisplayImage == null || tutorialVideos.Length != 4 || lastImageTexture == null)
        {
            Debug.LogError("설정 오류: 필수 컴포넌트/영상/이미지 할당을 확인하세요.");
            return;
        }

        // 🎥 영상 재생이 끝날 때마다 호출될 이벤트 핸들러 등록
        videoPlayer.loopPointReached += OnVideoEnd;

        // 🔊 배경 음악 재생 시작 👈 새로 추가
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Play();
        }
        
        // 🎬 첫 번째 영상 재생 시작
        videoPlayer.gameObject.SetActive(true);
        videoDisplayImage.texture = videoPlayer.targetTexture;
        PlayCurrentVideo();
    }

    // ⌨️ Update() 함수 제거 또는 빈 상태 유지 (더 이상 입력 감지 불필요)
    // void Update() {} 

    void PlayCurrentVideo()
    {
        if (currentVideoIndex < tutorialVideos.Length)
        {
            videoPlayer.clip = tutorialVideos[currentVideoIndex];
            videoPlayer.Play();
            isProcessing = false; // 영상 재생 중이므로 처리 중 상태 해제
            Debug.Log($"Playing video: {currentVideoIndex + 1}/{tutorialVideos.Length}");
        }
    }

    /// <summary>
    /// 영상이 끝났을 때 VideoPlayer 이벤트로 호출됩니다.
    /// </summary>
    void OnVideoEnd(VideoPlayer vp)
    {
        // 중복 처리 방지
        if (isProcessing) return; 

        Debug.Log($"Video {currentVideoIndex + 1} has ended. Starting {delayDuration} second delay.");
        
        // 3초 지연 후 다음 단계로 이동하는 코루틴 시작
        StartCoroutine(GoToNextStepWithDelay());
    }

    /// <summary>
    /// 강제 지연 시간을 포함하여 다음 단계로 진행합니다.
    /// </summary>
    IEnumerator GoToNextStepWithDelay()
    {
        isProcessing = true; // 처리 시작
        
        // ⏳ 3초 지연
        yield return new WaitForSeconds(delayDuration); 

        // 1. 다음 영상으로 인덱스 증가
        currentVideoIndex++; 

        // 2. 다음 영상이 남아 있다면 재생
        if (currentVideoIndex < tutorialVideos.Length)
        {
            PlayCurrentVideo();
        }
        // 3. 모든 영상이 끝났다면 -> 마지막 이미지 표시 및 씬 전환 준비
        else 
        {
            StartCoroutine(DisplayLastImageAndExit());
        }
    }

    /// <summary>
    /// 마지막 이미지를 표시하고 지연 후 시작 화면으로 돌아갑니다.
    /// </summary>
    IEnumerator DisplayLastImageAndExit()
    {
        // 🖼️ 이미지 표시 로직
        videoPlayer.Stop();
        videoPlayer.gameObject.SetActive(false); 

        videoDisplayImage.texture = lastImageTexture;
        
        // RawImage를 화면에 꽉 차게 설정 (RectTransform 설정은 이미 인스펙터에서 완료되어야 함)
        var rectTransform = videoDisplayImage.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero; 
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero; 
            rectTransform.offsetMax = Vector2.zero;
        }

        Debug.Log("Last image displayed. Waiting another 3 seconds before returning to Start Scene.");

        // ⏳ 3초 지연
        yield return new WaitForSeconds(delayDuration); 

        // 🎬 시작 화면으로 돌아가기
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop(); // 음악 정지 (선택 사항)
        }
        Cursor.visible = true;
        SceneManager.LoadScene(startSceneName);
    }
}