using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI_handler : MonoBehaviour
{
    public static UI_handler instance { get; private set; }

    private VisualElement root;
    private VisualElement defaultZero;
    private VisualElement[] cntIcons = new VisualElement[3]; // 배열 공간만 있고 내용은 비어있음
    private VisualElement distanceUI;
    private VisualElement background;
    private VisualElement miniball;
    private VisualElement goalPanel;
    private float bgWidth;

    private bool isInitialized = false; // ✨ 초기화 여부 체크용 플래그

    private void Awake()
    {
        // 싱글톤 보호: 이미 있으면 나를 파괴 (안전장치)
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;
    }

    void OnEnable()
    {
        UIDocument uiDocument = GetComponent<UIDocument>();
        if (uiDocument == null) return;

        root = uiDocument.rootVisualElement;
        if (root == null) return; // UI가 로드되지 않았으면 중단

        // 기본 배경 (흑백 3개짜리)
        defaultZero = root.Q<VisualElement>("default_zero");

        // 자식 컬러 아이콘 3개
        if (defaultZero != null)
        {
            cntIcons[0] = defaultZero.Q<VisualElement>("cnt1");
            cntIcons[1] = defaultZero.Q<VisualElement>("cnt2");
            cntIcons[2] = defaultZero.Q<VisualElement>("cnt3");
        }

        distanceUI = root.Q<VisualElement>("distance_UI");
        if (distanceUI != null)
        {
            background = distanceUI.Q<VisualElement>("background");
            miniball = distanceUI.Q<VisualElement>("miniball");

            if (background != null)
            {
                // RegisterCallback은 한 번만 등록되도록 주의하거나, 안전하게 사용
                background.RegisterCallback<GeometryChangedEvent>(evt =>
                {
                    bgWidth = background.resolvedStyle.width;
                });
            }
        }

        goalPanel = root.Q<VisualElement>("goal_panel");

        if (goalPanel != null)
        {
            goalPanel.style.display = DisplayStyle.None;
        }
        
        isInitialized = true; // 준비 완료
    }

    // ballcontroller가 호출할 메서드
    public void UpdateJumpUI(int currentJump)
    {
        // 아직 초기화 안 됐거나 아이콘을 못 찾았으면 실행 중단 (에러 방지)
        if (!isInitialized || cntIcons[0] == null) return; 

        currentJump = Mathf.Clamp(currentJump, 0, 3);

        for (int i = 0; i < cntIcons.Length; i++)
        {
            if (cntIcons[i] == null) continue; // 혹시 모를 null 체크

            // currentJump 개수 이하만 표시
            cntIcons[i].style.display = (i < currentJump)
                ? DisplayStyle.Flex
                : DisplayStyle.None;
        }
    }

    public void UpdateDistanceUI(float start_x, float goal_x, float current_x)
    {
        // UI가 준비되지 않았으면 무시
        if (!isInitialized || miniball == null || background == null) return;

        // bgWidth가 0이면 계산 의미 없음 (아직 레이아웃 잡히기 전)
        if (bgWidth <= 0) return; 

        float ratio = Mathf.InverseLerp(start_x, goal_x, current_x);
        ratio = Mathf.Clamp01(ratio);

        float newX = ratio * bgWidth;
        miniball.style.left = new Length(newX, LengthUnit.Pixel);
    }
    public void ShowGoalUI(bool show)
    {
        if (!isInitialized || goalPanel == null) return;
        
        // Goal UI 표시 상태를 설정합니다.
        goalPanel.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
    }
}