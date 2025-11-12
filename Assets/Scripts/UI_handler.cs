using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_handler : MonoBehaviour
{
    public static UI_handler instance { get; private set; }

    private VisualElement root;
    private VisualElement defaultZero;
    private VisualElement[] cntIcons = new VisualElement[3];

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        root = uiDocument.rootVisualElement;

        // 기본 배경 (흑백 3개짜리)
        defaultZero = root.Q<VisualElement>("default_zero");

        // 자식 컬러 아이콘 3개
        cntIcons[0] = defaultZero.Q<VisualElement>("cnt1");
        cntIcons[1] = defaultZero.Q<VisualElement>("cnt2");
        cntIcons[2] = defaultZero.Q<VisualElement>("cnt3");
    }

    // ballcontroller가 호출할 메서드
    public void UpdateJumpUI(int currentJump)
    {
        currentJump = Mathf.Clamp(currentJump, 0, 3);

        for (int i = 0; i < cntIcons.Length; i++)
        {
            // currentJump 개수 이하만 표시
            cntIcons[i].style.display = (i < currentJump)
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
    }
}
