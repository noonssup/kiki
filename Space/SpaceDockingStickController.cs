using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 우주비행사
/// 우주선을 움직이기 위한 가상조이스틱 조작 스크립트
/// </summary>

public class SpaceDockingStickController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("스크립트컨트롤러")]
    public SpaceDockingGameController dockingGameCtrl;

    [Header("게임오브젝트")]
    public RectTransform rectBackground;
    public RectTransform rectJoystick;

    [Header("게임데이터")]
    public float radius;
    public float leverRange = 50f;
    private Vector2 inputVector;
    private bool isInput = false;

    private void Start()
    {
        radius = rectBackground.rect.width * 0.5f;
        leverRange = 50f;
    }

    private void OnEnable()
    {
        rectJoystick.localPosition = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //스틱을 터치 시작
        SetJoystickLeverDirection(eventData);
        isInput= true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //스틱 터치 종료
        rectJoystick.localPosition = Vector2.zero;
        isInput= false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //스틱 터치 중
        SetJoystickLeverDirection(eventData);
    }

    public void SetJoystickLeverDirection(PointerEventData eventData)  //스틱 벡터값 설정
    {
        float screenScale = (float)Screen.width / 1280;
        Vector2 newPos = new Vector2(eventData.position.x / screenScale, eventData.position.y / screenScale);
        var inputDir = newPos - rectBackground.anchoredPosition;
        var clampedDir = inputDir.magnitude < leverRange ? inputDir : inputDir.normalized * leverRange;

        rectJoystick.anchoredPosition = clampedDir;
        inputVector = clampedDir / leverRange;
    }


    private void Update()
    {
        if (isInput)
        {
            InputControlVector();  
        }
    }

    private void InputControlVector() //스틱 벡터값 전달
    {
        dockingGameCtrl.SetShipMoveDirection(inputVector);
        dockingGameCtrl.SetRadarMoveDirection(inputVector);
    }

    
}
