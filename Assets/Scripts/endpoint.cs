using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class endpoint : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  // 공 같은 오브젝트는 Player 태그 달아놔
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
