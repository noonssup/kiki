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
                value = "작물 성장률 5% 증가";
                buttonIndex= 0;
                break;
            case "Item2Button (Legacy)":
                value = "토양 수치 5 감소"; buttonIndex =1;
                break;
            case "Item3Button (Legacy)":
                value = "토양 수치 5 증가"; buttonIndex = 2;
                break;
            case "Item4Button (Legacy)":
                value = "수분 수치 10% 증가"; buttonIndex = 3;
                break;
            case "Item5Button (Legacy)":
                value = "온도 수치 10℃ 감소"; buttonIndex = 4;
                break;
            case "Item6Button (Legacy)":
                value = "온도 수치 10℃ 증가"; buttonIndex = 5;
                break;
            case "Item7Button (Legacy)":
                value = "5일간 토양 수치 고정"; buttonIndex = 6;
                break;
            case "Item8Button (Legacy)":
                value = "5일간 수분 수치 고정"; buttonIndex = 7;
                break;
            case "Item9Button (Legacy)":
                value = "5일간 온도 수치 고정"; buttonIndex = 8;
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
                value = "작물 성장률 5% 증가";
                break;
            case "Item2Button (Legacy)":
                value = "토양 수치 5 감소";
                break;
            case "Item3Button (Legacy)":
                value = "토양 수치 5 증가";
                break;
            case "Item4Button (Legacy)":
                value = "수분 수치 10% 증가";
                break;
            case "Item5Button (Legacy)":
                value = "온도 수치 10℃ 감소";
                break;
            case "Item6Button (Legacy)":
                value = "온도 수치 10℃ 증가";
                break;
            case "Item7Button (Legacy)":
                value = "5일간 토양 수치 고정";
                break;
            case "Item8Button (Legacy)":
                value = "5일간 수분 수치 고정";
                break;
            case "Item9Button (Legacy)":
                value = "5일간 온도 수치 고정";
                break;
        }
        popup.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = value;
    }

    private void OnMouseExit()
    {
        popup.SetActive(false);
    }
}
