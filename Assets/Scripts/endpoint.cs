using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class endpoint : MonoBehaviour
{
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(loadnextscene(nextSceneName));
        }
    }

    IEnumerator loadnextscene(string nextSceneName)
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(nextSceneName);
    }
}
