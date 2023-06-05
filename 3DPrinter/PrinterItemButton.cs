using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ButtonType  //버튼 타입 (탭, 아이템버튼3종)
{
    TAB = 0,
    SHAPEBUTTON = 1,
    MATERIALBUTTON = 2,
    AFTERBUTTON = 3,
}

[System.Serializable]
public class ItemButtonInformation  //아이템 버튼의 정보
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

        //이미지 검색
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
