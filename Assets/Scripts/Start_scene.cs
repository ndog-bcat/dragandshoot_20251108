using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Startscreen: MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public AudioSource audioSource;
    public AudioSource start_scene_sound;
    public AudioClip clickClip;
    public void GoToTuroial()
    {
        audioSource.PlayOneShot(clickClip);
        StartCoroutine(DelayOnly());
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoFinished;
        if (start_scene_sound != null && start_scene_sound.isPlaying)
        {
            start_scene_sound.Stop();
        }
    }

    public void GoToManual()
    {
        audioSource.PlayOneShot(clickClip);
        StartCoroutine(DelayOnly());
        SceneManager.LoadScene("manual");
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
