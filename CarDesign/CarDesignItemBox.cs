using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class CarDesignItemBox : MonoBehaviour,IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("������Ʈ")]
    public CarDesignGameManager gameCtrl;
    public CarDesignParts parts;
    public GameObject itemPrefab;
    public GameObject itemObj;
    public Transform moveTr;
    public Image itemImage;

    [Header("���ӵ�����")]
    public string itemName;
    public bool isDrag = false;
    public bool isCorrect = false;

    [Header("�ؽ�Ʈ")]
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
            case "item_CarDesign_01": itemName = "���� �������"; break;
            case "item_CarDesign_02": itemName = "�ķ� �������"; break;
            case "item_CarDesign_03": itemName = "��������"; break;
            case "item_CarDesign_04": itemName = "���͸� ��"; break;
            case "item_CarDesign_05": itemName = "�º�������"; break;
            case "item_CarDesign_06": itemName = "���ӱ�"; break;
            case "item_CarDesign_07": itemName = "���� ��Ʈ"; break;
            case "item_CarDesign_08": itemName = "��Ʈ���� �ǿܱ�"; break;
            case "item_CarDesign_09": itemName = "EPCU"; break;
            case "item_CarDesign_10": itemName = "ȸ��������ġ"; break;
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
    //    if (parts == null)  //���콺��ư�� �� �� parts �� �Ҵ�� ���� ���ٸ�
    //    {
    //        isDrag = false;
    //        itemObj.SetActive(false);
    //        itemImage.enabled = true;
    //        ItemBoxSetup();
    //    }
    //    else   //���콺��ư�� �� �� parts �� �Ҵ�� ���� ���� ���
    //    {
    //        if (isCorrect)  //�����̶��,
    //        {
    //            SoundManager.instance.PlayEffectSound("eff_Robot_equip", 1f);
    //            gameCtrl.currectCount++;
    //            parts.isCorrect = true;
    //            Color _color = new Color(1f,1f,1f,1f);
    //            parts.CorrectSetup(_color);
    //            itemObj.SetActive(false);

    //            ///
    //            //���� ó��
    //            ///
    //            this.gameObject.SetActive(false);
    //        }
    //        else   //������ �ƴ� ���
    //        {
    //            SoundManager.instance.PlayEffectSound("eff_Common_wrong", 1f);
    //            isDrag = false;
    //            itemObj.SetActive(false);
    //            itemImage.enabled = true;
    //            ItemBoxSetup();
    //        }
    //    }
    //}

    public void OnDrag(PointerEventData eventData)  //�巡�� ��
    {
        if (!gameCtrl.isGamePlay) return;
        if (isDrag)
        {
            itemObj.GetComponent<Image>().enabled = true;
            Vector2 _position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            itemObj.transform.position = new Vector3(_position.x, _position.y);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)  //�巡�� ����
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

    public void OnEndDrag(PointerEventData eventData)  //�巡�� ��
    {
        if (parts == null)  //���콺��ư�� �� �� parts �� �Ҵ�� ���� ���ٸ�
        {
            isDrag = false;
            itemObj.SetActive(false);
            itemImage.enabled = true;
            ItemBoxSetup();
        }
        else   //���콺��ư�� �� �� parts �� �Ҵ�� ���� ���� ���
        {
            if (isCorrect)  //�����̶��,
            {
                SoundManager.instance.PlayEffectSound("eff_Robot_equip", 1f);
                gameCtrl.currectCount++;
                parts.isCorrect = true;
                Color _color = new Color(1f, 1f, 1f, 1f);
                parts.CorrectSetup(_color);
                itemObj.SetActive(false);

                ///
                //���� ó��
                ///
                this.gameObject.SetActive(false);
            }
            else   //������ �ƴ� ���
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
