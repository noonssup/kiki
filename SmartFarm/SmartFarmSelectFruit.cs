using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 스마트팜 작물 선택 화면용 스크립트
/// </summary>

public class SmartFarmSelectFruit : MonoBehaviour
{
    [Header("오브젝트")]
    public SmartFarmGamePanel gameCtrl;
    public GameObject fruitSelectPanel;
    public Sprite[] fruitSprites;
    public Image fruitIcon;

    [Header("게임데이터")]
    public Crops crops;
    public int fruitNumber = 0; // 0 = 토마토
    public string fruitName;

    [Header("텍스트")]
    public TextMeshProUGUI textFruitName;

    List<Dictionary<string, object>> data;

    private void Start()
    {
        ParentObjectSet();
        SelectButtonSetup();
    }

    void ParentObjectSet()  //부모오브젝트 설정
    {
        gameCtrl = FindObjectOfType<SmartFarmGamePanel>();
        data = SmartFarmGameManager.instance.data;
        fruitSelectPanel = GameObject.Find("FruitSelectPanel");
    }

    void SelectButtonSetup()  //작물선택 버튼 설정
    {
        for(int i = 0; i < data.Count; i++)
        {
            if(data[i]["ID"].ToString() == fruitNumber.ToString())
            {
                fruitName = data[i]["name_key"].ToString();
            }
        }

        fruitIcon.sprite = fruitSprites[fruitNumber];
        textFruitName.text = fruitName;
    }

    public void SelectFruit()  //작물 선택 버튼을 누르면 실행될 함수...
    {
        SmartFarmGameManager.instance.SelectFruit(crops);
    }

}
