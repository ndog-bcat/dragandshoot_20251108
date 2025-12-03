using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class manual_script : MonoBehaviour
{
    public VideoClip[] tutorialVideos;   // 4개 영상 클립
    public Texture lastImageTexture;     // 마지막에 표시할 이미지 텍스처
    public VideoPlayer videoPlayer;
    public RawImage videoDisplayImage;
    public string startSceneName = "stage1";

    public AudioSource backgroundMusicSource; // 배경 음악 재생할 AudioSource 컴포넌트
    public float transitionDelay = 0.3f;     // 다음 단계로 넘어가기 전 짧은 지연 시간

    private int currentVideoIndex = 0;
    private bool allVideosPlayed = false;   // 모든 영상이 재생 완료되었는지

    void Start()
    {
        Cursor.visible = false;
        
        if (videoPlayer == null || videoDisplayImage == null || tutorialVideos.Length == 0 || lastImageTexture == null)
        {
            Debug.LogError("설정 오류: 필수 컴포넌트/영상/이미지 할당을 확인하세요. (최소 1개의 영상 필요)");
            return;
        }

        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Play();
        }
        
        videoPlayer.gameObject.SetActive(true);
        videoDisplayImage.texture = videoPlayer.targetTexture;
        PlayCurrentVideo();
    }

    void Update() 
    {
        // 1. 모든 영상이 재생되지 않았고 (튜토리얼 중),
        // 2. 아무 키라도 눌렸다면 다음 단계로 이동
        if (!allVideosPlayed && Input.anyKeyDown)
        {
            // 현재 무한 반복 중인 영상 중지
            videoPlayer.Stop();
            videoPlayer.isLooping = false;

            // 다음 단계 코루틴 시작
            StartCoroutine(GoToNextStep());
        }
    } 

    void PlayCurrentVideo()
    {
        if (currentVideoIndex < tutorialVideos.Length)
        {
            videoPlayer.clip = tutorialVideos[currentVideoIndex];
            videoPlayer.isLooping = true; // 현재 영상 무한 반복 설정
            videoPlayer.Play();
        }
    }

    IEnumerator GoToNextStep()
    {
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

    IEnumerator DisplayLastImageAndExit()
    {
        videoPlayer.gameObject.SetActive(false); 
        videoDisplayImage.texture = lastImageTexture;
        
        var rectTransform = videoDisplayImage.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero; 
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero; 
            rectTransform.offsetMax = Vector2.zero;
        }

        yield return new WaitForSeconds(transitionDelay);
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop(); 
        }
        Cursor.visible = true;
        SceneManager.LoadScene(startSceneName);
    }
}