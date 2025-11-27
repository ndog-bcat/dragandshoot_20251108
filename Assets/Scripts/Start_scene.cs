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
        Cursor.visible = false;
        audioSource.PlayOneShot(clickClip);
        StartCoroutine(DelayOnly());
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoFinished;
        if (start_scene_sound != null && start_scene_sound.isPlaying)
        {
            start_scene_sound.Stop();
        }
    }

    public IEnumerator DelayOnly()
    {
        yield return new WaitForSeconds(1f);
    }
    void OnVideoFinished(VideoPlayer vp)
    {
        Cursor.visible = true;
        SceneManager.LoadScene("manual");
    }
}
