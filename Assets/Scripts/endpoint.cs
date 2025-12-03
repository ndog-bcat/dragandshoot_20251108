using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class endpoint : MonoBehaviour
{
    public string nextSceneName;
    private AudioSource audioSource;
    public AudioClip reachedClip;
    private bool isWaitingForInput = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isWaitingForInput && Input.GetKeyDown(KeyCode.Return)) // Enter 키 감지
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                isWaitingForInput = false;
                if (UI_handler.instance != null)
                {
                    UI_handler.instance.ShowGoalUI(false); 
                }
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider2D>().enabled = false; 

            PlaySound(reachedClip);
            StartCoroutine(ShowGoalUIAndAwaitInput());
        }
    }

    IEnumerator ShowGoalUIAndAwaitInput()
    {
        yield return new WaitForSeconds(0.3f); 
        if (UI_handler.instance != null)
        {
            UI_handler.instance.ShowGoalUI(true); 
        }
        isWaitingForInput = true;
    }

    public void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}