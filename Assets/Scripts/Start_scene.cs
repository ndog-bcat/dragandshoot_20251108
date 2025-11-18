using UnityEngine;
using UnityEngine.SceneManagement;

public class Startscreen: MonoBehaviour
{
    void Update()
    {
        // ���콺 Ŭ�� �Ǵ� Ű���� ��ư ���� ����
        if (Input.anyKeyDown)
        {
            // ���� ������ �̵� (MainScene �̸��� ���� ������Ʈ �̸��� �°� �ٲ㵵 OK)
            SceneManager.LoadScene("tutorial");
        }
    }
}
