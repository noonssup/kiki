using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmartFarmItemDescription : MonoBehaviour
{
    SmartFarmGamePanel gameCtrl;
    public GameObject popup;
    int clickCount = 0;
    int buttonIndex = 10;

    private void Start()
    {
        gameCtrl= FindObjectOfType<SmartFarmGamePanel>();

    }

    private void OnMouseDown()
    {
        clickCount++;
        popup.SetActive(true);
        string value = null;
        switch (this.gameObject.name)
        {
            case "Item1Button (Legacy)":
                value = "�۹� ����� 5% ����";
                buttonIndex= 0;
                break;
            case "Item2Button (Legacy)":
                value = "��� ��ġ 5 ����"; buttonIndex =1;
                break;
            case "Item3Button (Legacy)":
                value = "��� ��ġ 5 ����"; buttonIndex = 2;
                break;
            case "Item4Button (Legacy)":
                value = "���� ��ġ 10% ����"; buttonIndex = 3;
                break;
            case "Item5Button (Legacy)":
                value = "�µ� ��ġ 10�� ����"; buttonIndex = 4;
                break;
            case "Item6Button (Legacy)":
                value = "�µ� ��ġ 10�� ����"; buttonIndex = 5;
                break;
            case "Item7Button (Legacy)":
                value = "5�ϰ� ��� ��ġ ����"; buttonIndex = 6;
                break;
            case "Item8Button (Legacy)":
                value = "5�ϰ� ���� ��ġ ����"; buttonIndex = 7;
                break;
            case "Item9Button (Legacy)":
                value = "5�ϰ� �µ� ��ġ ����"; buttonIndex = 8;
                break;
        }
        popup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = value;
        StartCoroutine(UseItem());
        if(clickCount > 1)
        {
            gameCtrl.UseItems(buttonIndex);
        }
    }

    IEnumerator UseItem()
    {
        yield return new WaitForSeconds(0.3f);
        clickCount = 0;
    }



    public void OnMouseOver()
    {
        popup.SetActive(true);
        string value = null;
        switch (this.gameObject.name)
        {
            case "Item1Button (Legacy)":
                value = "�۹� ����� 5% ����";
                break;
            case "Item2Button (Legacy)":
                value = "��� ��ġ 5 ����";
                break;
            case "Item3Button (Legacy)":
                value = "��� ��ġ 5 ����";
                break;
            case "Item4Button (Legacy)":
                value = "���� ��ġ 10% ����";
                break;
            case "Item5Button (Legacy)":
                value = "�µ� ��ġ 10�� ����";
                break;
            case "Item6Button (Legacy)":
                value = "�µ� ��ġ 10�� ����";
                break;
            case "Item7Button (Legacy)":
                value = "5�ϰ� ��� ��ġ ����";
                break;
            case "Item8Button (Legacy)":
                value = "5�ϰ� ���� ��ġ ����";
                break;
            case "Item9Button (Legacy)":
                value = "5�ϰ� �µ� ��ġ ����";
                break;
        }
        popup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = value;
    }

    private void OnMouseExit()
    {
        popup.SetActive(false);
    }
}
