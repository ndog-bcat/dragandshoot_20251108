using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Startscreen: MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public AudioClip clickClip;
    public void GoToTuroial()
    {
        audioSource.PlayOneShot(clickClip);
        StartCoroutine(DelayOnly());
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    public IEnumerator DelayOnly()
    {
        yield return new WaitForSeconds(1f);
    }
    void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("tutorial");
    }
}
