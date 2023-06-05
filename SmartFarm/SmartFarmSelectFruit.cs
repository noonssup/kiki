using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ����Ʈ�� �۹� ���� ȭ��� ��ũ��Ʈ
/// </summary>

public class SmartFarmSelectFruit : MonoBehaviour
{
    [Header("������Ʈ")]
    public SmartFarmGamePanel gameCtrl;
    public GameObject fruitSelectPanel;
    public Sprite[] fruitSprites;
    public Image fruitIcon;

    [Header("���ӵ�����")]
    public Crops crops;
    public int fruitNumber = 0; // 0 = �丶��
    public string fruitName;

    [Header("�ؽ�Ʈ")]
    public TextMeshProUGUI textFruitName;

    List<Dictionary<string, object>> data;

    private void Start()
    {
        ParentObjectSet();
        SelectButtonSetup();
    }

    void ParentObjectSet()  //�θ������Ʈ ����
    {
        gameCtrl = FindObjectOfType<SmartFarmGamePanel>();
        data = SmartFarmGameManager.instance.data;
        fruitSelectPanel = GameObject.Find("FruitSelectPanel");
    }

    void SelectButtonSetup()  //�۹����� ��ư ����
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

    public void SelectFruit()  //�۹� ���� ��ư�� ������ ����� �Լ�...
    {
        SmartFarmGameManager.instance.SelectFruit(crops);
    }

}
