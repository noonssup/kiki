using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CarDesignItemBox : MonoBehaviour,IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("오브젝트")]
    public CarDesignGameManager gameCtrl;
    public CarDesignParts parts;
    public GameObject itemPrefab;
    public GameObject itemObj;
    public Transform moveTr;
    public Image itemImage;

    [Header("게임데이터")]
    public string itemName;
    public bool isDrag = false;
    public bool isCorrect = false;

    [Header("텍스트")]
    public TextMeshProUGUI textItemName;

    private void Start()
    {
        ItemBoxSetup();
        moveTr = GameObject.Find("MoveTrPanel").transform;
        gameCtrl = FindObjectOfType<CarDesignGameManager>();
    }

    void ItemBoxSetup()
    {
        itemObj.SetActive(false);
        switch (itemImage.sprite.name)
        {
            case "item_CarDesign_01": itemName = "전륜 서스펜션"; break;
            case "item_CarDesign_02": itemName = "후륜 서스펜션"; break;
            case "item_CarDesign_03": itemName = "구동모터"; break;
            case "item_CarDesign_04": itemName = "배터리 팩"; break;
            case "item_CarDesign_05": itemName = "온보드차저"; break;
            case "item_CarDesign_06": itemName = "감속기"; break;
            case "item_CarDesign_07": itemName = "충전 포트"; break;
            case "item_CarDesign_08": itemName = "히트펌프 실외기"; break;
            case "item_CarDesign_09": itemName = "EPCU"; break;
            case "item_CarDesign_10": itemName = "회생제동장치"; break;
        }
        textItemName.text = itemName;
    }

    //private void OnMouseDown()
    //{
    //    if (!gameCtrl.isGamePlay) return;
    //    isDrag = true;
    //    itemObj.SetActive(true);
    //    itemObj.transform.SetParent(moveTr);
    //    itemObj.transform.localScale = Vector3.one;
    //    itemObj.GetComponent<Image>().enabled= false;
    //    itemObj.GetComponent<Image>().sprite = this.itemImage.sprite;
    //    itemObj.GetComponent<CarDesignMoveItem>().itemCtrl = this;
    //    itemImage.enabled = false;
    //}

    //private void OnMouseDrag()
    //{
    //    if (!gameCtrl.isGamePlay) return;
    //    if (isDrag)
    //    {
    //        itemObj.GetComponent<Image>().enabled = true;
    //        Vector2 _position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        itemObj.transform.position = new Vector3(_position.x, _position.y);
    //    }
    //}

    //private void OnMouseUp()
    //{
    //    if (parts == null)  //마우스버튼을 땔 때 parts 에 할당된 값이 없다면
    //    {
    //        isDrag = false;
    //        itemObj.SetActive(false);
    //        itemImage.enabled = true;
    //        ItemBoxSetup();
    //    }
    //    else   //마우스버튼을 땔 때 parts 에 할당된 값이 있을 경우
    //    {
    //        if (isCorrect)  //정답이라면,
    //        {
    //            SoundManager.instance.PlayEffectSound("eff_Robot_equip", 1f);
    //            gameCtrl.currectCount++;
    //            parts.isCorrect = true;
    //            Color _color = new Color(1f,1f,1f,1f);
    //            parts.CorrectSetup(_color);
    //            itemObj.SetActive(false);

    //            ///
    //            //점수 처리
    //            ///
    //            this.gameObject.SetActive(false);
    //        }
    //        else   //정답이 아닐 경우
    //        {
    //            SoundManager.instance.PlayEffectSound("eff_Common_wrong", 1f);
    //            isDrag = false;
    //            itemObj.SetActive(false);
    //            itemImage.enabled = true;
    //            ItemBoxSetup();
    //        }
    //    }
    //}

    public void OnDrag(PointerEventData eventData)  //드래그 중
    {
        if (!gameCtrl.isGamePlay) return;
        if (isDrag)
        {
            itemObj.GetComponent<Image>().enabled = true;
            Vector2 _position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            itemObj.transform.position = new Vector3(_position.x, _position.y);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)  //드래그 시작
    {
        if (!gameCtrl.isGamePlay) return;
        isDrag = true;
        itemObj.SetActive(true);
        itemObj.transform.SetParent(moveTr);
        itemObj.transform.localScale = Vector3.one;
        itemObj.GetComponent<Image>().enabled = false;
        itemObj.GetComponent<Image>().sprite = this.itemImage.sprite;
        itemObj.GetComponent<CarDesignMoveItem>().itemCtrl = this;
        itemImage.enabled = false;
    }

    public void OnEndDrag(PointerEventData eventData)  //드래그 끝
    {
        if (parts == null)  //마우스버튼을 땔 때 parts 에 할당된 값이 없다면
        {
            isDrag = false;
            itemObj.SetActive(false);
            itemImage.enabled = true;
            ItemBoxSetup();
        }
        else   //마우스버튼을 땔 때 parts 에 할당된 값이 있을 경우
        {
            if (isCorrect)  //정답이라면,
            {
                SoundManager.instance.PlayEffectSound("eff_Robot_equip", 1f);
                gameCtrl.currectCount++;
                parts.isCorrect = true;
                Color _color = new Color(1f, 1f, 1f, 1f);
                parts.CorrectSetup(_color);
                itemObj.SetActive(false);

                ///
                //점수 처리
                ///
                this.gameObject.SetActive(false);
            }
            else   //정답이 아닐 경우
            {
                SoundManager.instance.PlayEffectSound("eff_Common_wrong", 1f);
                isDrag = false;
                itemObj.SetActive(false);
                itemImage.enabled = true;
                ItemBoxSetup();
            }
        }
    }
}
