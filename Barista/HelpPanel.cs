using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HelpPanel : MonoBehaviour
{
    [Header("공용")]
    public BaristaGameCtrl gameCtrl;
    public GameObject[] helpPageObj;  //도움말 페이지 오브젝트
    public Button[] pageButtons;  //페이지 넘기기 버튼튼
    public TMP_Text pageText;         //현재 페이지를 보여줄 텍스트
    public int page;              //현재 페이지 번호

    [Header("도움말 1페이지")]
    public TMP_Text guideText;        //도움말 가이드 텍스트
    public TMP_Text[] objText;        //오브젝트이름을 표시할 텍스트


    private void Start()
    {
        gameCtrl = FindObjectOfType<BaristaGameCtrl>();
    }

    private void OnEnable()
    {
        page = 1;
        SetPage();
    }

    void SetPage()
    {
        switch (page)
        {
            case 1:
                helpPageObj[0].SetActive(true);
                helpPageObj[1].SetActive(false);
                helpPageObj[2].SetActive(false);
                pageButtons[0].gameObject.SetActive(false);
                pageButtons[1].gameObject.SetActive(true);
                pageText.text = "1 / 3";
                break;
            case 2:
                helpPageObj[0].SetActive(false);
                helpPageObj[1].SetActive(true);
                helpPageObj[2].SetActive(false);
                pageButtons[0].gameObject.SetActive(true);
                pageButtons[1].gameObject.SetActive(true);
                pageText.text = "2 / 3";
                break;
            case 3:
                helpPageObj[0].SetActive(false);
                helpPageObj[1].SetActive(false);
                helpPageObj[2].SetActive(true);
                pageButtons[0].gameObject.SetActive(true);
                pageButtons[1].gameObject.SetActive(false);
                pageText.text = "3 / 3";
                break;
        }
    }

    public void NextPage()  //다음 페이지
    {
        page++;
        if (page > 3) page = 2;
        SetPage();
    }

    public void PrevPage()  //이전 페이지
    {
        page--;
        if (page < 1 ) page = 1;
        SetPage();
    }        

    public void CloseHelpPanel()  //도움말 패널 닫기
    {
        gameCtrl.CloseHelpPanel();
    }
}
