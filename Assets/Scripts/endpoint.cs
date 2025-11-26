using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class endpoint : MonoBehaviour
{
    public string nextSceneName;
    AudioSource audioSource;
    public AudioClip reachedClip;

    // public GameObject goalUIprefab;
    // private GameObject currentGoalprefab;
    // private bool isWaitingForInput = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // private void Update()
    // {
    //     if (isWaitingForInput && Input.GetKeyDown(KeyCode.Return)) // Enter 키 감지
    //     {
    //         // Enter 키가 눌렸고, 다음 씬 이름이 유효한 경우
    //         if (!string.IsNullOrEmpty(nextSceneName))
    //         {
    //             isWaitingForInput = false; // 중복 실행 방지
    //             SceneManager.LoadScene(nextSceneName);
    //         }
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlaySound(reachedClip);
            StartCoroutine(loadnextscene(nextSceneName));
        }
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         // 플레이어가 충돌했음을 알리기 위해 충돌체 비활성화
    //         GetComponent<Collider2D>().enabled = false; 

    //         PlaySound(reachedClip);
    //         StartCoroutine(ShowGoalUIAndAwaitInput());
    //     }
    // }

    IEnumerator loadnextscene(string nextSceneName)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextSceneName);
    }

    // IEnumerator ShowGoalUIAndAwaitInput()
    // {
    //     yield return new WaitForSeconds(0.5f);

    //     if (goalUIPrefab != null)
    //     {
    //         currentGoalUI = Instantiate(goalUIPrefab);
            
    //         Canvas canvas = FindObjectOfType<Canvas>();
    //         if (canvas != null)
    //         {
    //             currentGoalUI.transform.SetParent(canvas.transform, false);
    //         }
    //     }
    //     isWaitingForInput = true;
    // }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
