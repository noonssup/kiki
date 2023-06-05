using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RescuePad : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public RescueLevel3Ctrl gameCtrl;
    public GameObject padItem;
    public GameObject activePadItem; //생성된 패드 오브젝트
    public GameObject gameWindow;
    public Button btn;
    public GameObject padPoint; //충돌한 패드 포인트를 담을 변수

    private void Start()
    {
        gameCtrl = FindObjectOfType<RescueLevel3Ctrl>();
    }

    private void OnEnable()
    {
    }



    public void OnBeginDrag(PointerEventData eventData)
    {
        padPoint = null;
        GameObject _padPrefab = Instantiate(padItem, this.transform.position, Quaternion.identity);
        _padPrefab.transform.SetParent(gameWindow.transform);
        _padPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        activePadItem = _padPrefab;

        btn = this.transform.GetComponent<Button>();
        ColorBlock _colorBlock = btn.colors;
        _colorBlock.normalColor = new Color(0.549f, 0.549f, 0.549f);
        btn.colors = _colorBlock;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        activePadItem.transform.position = _mousePos;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ColorBlock _colorBlock = btn.colors;
        _colorBlock.normalColor = new Color(1f, 1f, 1f);
        btn.colors = _colorBlock;
        if(padPoint !=null) gameCtrl.CheckCorrect(padPoint);  //패드프리팹에서 전달 받은 오브젝트가 null 이 아니라면,
        Destroy(activePadItem);
        activePadItem = null;
    }

    public void CheckCorrect(GameObject _point)
    {
        //패드 프리팹에서 부착 위치 오브젝트의 게임오브젝트 정보를 받아옴 (드래그가 끝나면 gameCtrl 에 전달)
        padPoint = _point;
    }
}
