using UnityEngine;
using UnityEngine.SceneManagement;

public class Startscreen: MonoBehaviour
{
    void Update()
    {
        // 마우스 클릭 또는 키보드 버튼 눌림 감지
        if (Input.anyKeyDown)
        {
            // 다음 씬으로 이동 (MainScene 이름은 본인 프로젝트 이름에 맞게 바꿔도 OK)
            SceneManager.LoadScene("SampleScene");
        }
    }
}
