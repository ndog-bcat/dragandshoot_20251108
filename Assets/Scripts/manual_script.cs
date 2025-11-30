using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class manual_script : MonoBehaviour
{
    // ⚙️ 설정 값
    public VideoClip[] tutorialVideos;   // 4개의 영상 클립을 할당합니다.
    public Texture lastImageTexture;     // 마지막에 표시할 이미지 텍스처를 할당합니다.
    public VideoPlayer videoPlayer;
    public RawImage videoDisplayImage;
    public string startSceneName = "stage1";

    // 🔊 오디오 설정 값
    public AudioSource backgroundMusicSource; // 배경 음악을 재생할 AudioSource 컴포넌트를 할당합니다.
    public float transitionDelay = 0.3f;     // 다음 단계로 넘어가기 전 짧은 지연 시간

    // 🔄 내부 상태 변수
    private int currentVideoIndex = 0;
    private bool allVideosPlayed = false;   // 모든 영상이 재생 완료되었는지

    void Start()
    {
        Cursor.visible = false;
        
        // 🚨 필수 컴포넌트 및 설정 검사
        if (videoPlayer == null || videoDisplayImage == null || tutorialVideos.Length == 0 || lastImageTexture == null)
        {
            Debug.LogError("설정 오류: 필수 컴포넌트/영상/이미지 할당을 확인하세요. (최소 1개의 영상 필요)");
            return;
        }

        // ❌ videoPlayer.loopPointReached 이벤트는 무한 루프를 위해 제거합니다.

        // 🔊 배경 음악 재생 시작
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Play();
        }
        
        // 🎬 첫 번째 영상 재생 시작
        videoPlayer.gameObject.SetActive(true);
        videoDisplayImage.texture = videoPlayer.targetTexture;
        PlayCurrentVideo();
    }

    /// <summary>
    /// 매 프레임마다 키 입력을 감지합니다.
    /// </summary>
    void Update() 
    {
        // 1. 모든 영상이 재생되지 않았고 (튜토리얼 중),
        // 2. 아무 키라도 눌렸다면 다음 단계로 이동합니다.
        if (!allVideosPlayed && Input.anyKeyDown)
        {
            // 현재 무한 반복 중인 영상 중지
            videoPlayer.Stop();
            videoPlayer.isLooping = false;
            
            Debug.Log($"Input received. Current video {currentVideoIndex + 1} stopped. Transitioning to next step.");

            // 다음 단계 코루틴 시작
            StartCoroutine(GoToNextStep());
        }
    } 

    void PlayCurrentVideo()
    {
        if (currentVideoIndex < tutorialVideos.Length)
        {
            videoPlayer.clip = tutorialVideos[currentVideoIndex];
            videoPlayer.isLooping = true; // 🌟 현재 영상 무한 반복 설정
            videoPlayer.Play();
            Debug.Log($"Playing video: {currentVideoIndex + 1}/{tutorialVideos.Length} (Looping until input)");
        }
    }

    // ⚠️ OnVideoEnd 함수는 더 이상 사용되지 않습니다.

    /// <summary>
    /// 키 입력에 반응하여 다음 단계로 진행합니다.
    /// </summary>
    IEnumerator GoToNextStep()
    {
        // ⏳ 짧은 지연 시간
        yield return new WaitForSeconds(transitionDelay); 

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
            allVideosPlayed = true;
            // 마지막 이미지 표시 및 종료 코루틴으로 전환
            StartCoroutine(DisplayLastImageAndExit());
        }
    }

    /// <summary>
    /// 마지막 이미지를 표시하고 지연 후 시작 화면으로 돌아갑니다.
    /// 이 함수는 GoToNextStep()에서 모든 영상이 끝났을 때만 호출됩니다.
    /// </summary>
    IEnumerator DisplayLastImageAndExit()
    {
        // 🖼️ 이미지 표시 로직
        videoPlayer.gameObject.SetActive(false); 
        videoDisplayImage.texture = lastImageTexture;
        
        // RawImage를 화면에 꽉 차게 설정
        var rectTransform = videoDisplayImage.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero; 
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero; 
            rectTransform.offsetMax = Vector2.zero;
        }

        Debug.Log("Last image displayed. Waiting for final transition.");

        // ⏳ 짧은 지연 후 씬 전환
        yield return new WaitForSeconds(transitionDelay); 
        // stage1 시작
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop(); 
        }
        Cursor.visible = true;
        SceneManager.LoadScene(startSceneName);
    }
}