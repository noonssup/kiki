using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType  //��ư Ÿ�� (��, �����۹�ư3��)
{
    TAB = 0,
    SHAPEBUTTON = 1,
    MATERIALBUTTON = 2,
    AFTERBUTTON = 3,
}

[System.Serializable]
public class ItemButtonInformation  //������ ��ư�� ����
{
    public int id = 0;
    public string itemName = string.Empty;
}


public class PrinterItemButton : MonoBehaviour
{
    public PrinterUIController uiCtrl;
    public ButtonType btnType = ButtonType.TAB;
    public ItemButtonInformation itemInfor;
    Sprite[] sprites;
    TextMeshProUGUI text;
    Image itemImage;

    public void ButtonTypeSetup(int _type)
    {
        this.btnType = (ButtonType)_type;
        text = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
    }

    public void ButtonInformationSetup(int _id, string _itemName, PrinterUIController _ctrl)
    {
        itemImage = transform.GetChild(0).GetComponent<Image>();
        uiCtrl = _ctrl;
        sprites = uiCtrl.itemButtonSprites;
        itemInfor.id = _id;
        itemInfor.itemName = _itemName;
        text.text = itemInfor.itemName;

        //�̹��� �˻�
        foreach (Sprite sprite in sprites)
        {
            if("item_"+_id == sprite.name)
            {
                itemImage.sprite = sprite;
                break;
            }
        }
    }

    public void OnClickButtonItemSetup()
    {
        if (uiCtrl == null || this.btnType < (ButtonType)1) return;

        uiCtrl.SetTabsInfor((int)btnType-1, itemInfor.id, itemInfor.itemName);
    }                       
}
