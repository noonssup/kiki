using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// �糭���������� ��Ʈ���г� ��Ʈ�ѿ� ��ũ��Ʈ
/// </summary>

public class DisasterController : MonoBehaviour
{
    [Header("������Ʈ")]
    public DisasterBuilding building;
    public Image[] statusbar;
    Color barColor;
    public Image buildingNullImage;

    [Header("��ư")]
    public Button btnUpgrade;
    public Button btnRepair;
    public Button[] btnPreparations;

    [Header("�ؽ�Ʈ")]
    public TextMeshProUGUI textStatusRate;
    public TextMeshProUGUI textBuildingName;
    public TextMeshProUGUI textBuildingGrade;
    public TextMeshProUGUI textUpgradeButton;
    public TextMeshProUGUI[] textCost;

    private void Start()
    {
        GameSetup();
    }

    public void GameSetup()  //���� �ʱ� �¾�
    {
        building = null;
        textStatusRate.text = 0.ToString() + "%";
        textBuildingName.text = "";
        textBuildingGrade.text = "";
        statusbar[0].fillAmount = 0 / 100f;
        statusbar[1].fillAmount = 0 / 100f;
        statusbar[2].fillAmount = 0 / 100f;
        statusbar[3].fillAmount = 0 / 100f;
        statusbar[4].fillAmount = 0 / 100f;

        btnUpgrade.interactable = false;
        btnRepair.interactable = false;
        for(int i=0;i<btnPreparations.Length;i++)
        {
            btnPreparations[i].interactable= false;
        }
        buildingNullImage.gameObject.SetActive(true);
    }

    private void Update()
    {
        PrintData();
    }

    public void SetData(DisasterBuilding _building)  //��Ʈ���гο� ������ �ǹ��� ������ ����
    {
        building = _building;
    }

    public void ReleaseData()
    {
        building = null;
    }

    public void PrintData()  //��Ʈ��â ������ ���
    {
        if (building == null)
        {
            textBuildingName.text = "";
            textBuildingGrade.text = "";
            buildingNullImage.gameObject.SetActive(true);
            return;
        }
        buildingNullImage.gameObject.SetActive(false);

        if (building.state == BuildingState.MAKE)
        {
            textBuildingName.text = "������";
            textUpgradeButton.text = "";
        }
        else if (building.state == BuildingState.BLANK)
        {
            textBuildingName.text = "����";
            textUpgradeButton.text = "�Ǽ�";
            textCost[0].text = 500.ToString("n0");
        }
        else if (building.state == BuildingState.BROKEN)
        {
            textBuildingName.text = "����";
            textBuildingGrade.text = "�������ǹ�";
            textUpgradeButton.text = "�����";
            textCost[0].text = building.buildingData.rebuilingCost.ToString("n0");
        }
        else
        {
            textCost[0].text = building.buildingData.upgradeCost.ToString("n0");
            textBuildingName.text = building.buildingName;
            textUpgradeButton.text = "�ǹ�����";
        }

        if (building.state > (BuildingState)2) textBuildingGrade.text = building.buildingGrade;

        textStatusRate.text = building.buildingData.health.ToString("n0") + "%";

        textCost[1].text = building.buildingData.repairCost.ToString("n0");
        textCost[2].text = building.buildingData.repairCost.ToString("n0");
        textCost[3].text = building.buildingData.repairCost.ToString("n0");
        textCost[4].text = building.buildingData.repairCost.ToString("n0");
        textCost[5].text = building.buildingData.repairCost.ToString("n0");

        statusbar[0].fillAmount = building.buildingData.health / building.buildingData.maxPreparation;
        statusbar[1].fillAmount = (float)building.buildingData.rainPreparation / 4;
        statusbar[2].fillAmount = (float)building.buildingData.surgePreparation / 4;
        statusbar[3].fillAmount = (float)building.buildingData.typhoonPreparation / 4;
        statusbar[4].fillAmount = (float)building.buildingData.earthquakePreparation / 4;


        for (int i = 1; i < statusbar.Length; i++)
        {
            if (statusbar[i].fillAmount < 0.26f) {
                ColorUtility.TryParseHtmlString("#FFC000FF", out barColor);
                statusbar[i].color = barColor; } 
            else if (statusbar[i].fillAmount < 0.51f)
            {
                //ColorUtility.TryParseHtmlString("#FFC000FF", out barColor);
                statusbar[i].color = Color.yellow;
            }
            else if (statusbar[i].fillAmount < 0.76f)
            {
                ColorUtility.TryParseHtmlString("#C6DB45FF", out barColor);
                statusbar[i].color = barColor;
            }
            else
            {
                statusbar[i].color = Color.green;
            }
        }

        if(DisasterGameManager.instance.goods >= building.buildingData.repairCost)
        {
            if(building.buildingData.health < building.buildingData.maxPreparation) btnRepair.interactable = true;
            else btnRepair.interactable = false;

            for(int i=0;i<btnPreparations.Length;i++)
            {
                btnPreparations[i].interactable = true;
            }
        }
        else
        {
            btnRepair.interactable = false;
            for (int i = 0; i < btnPreparations.Length; i++)
            {
                btnPreparations[i].interactable = false;
            }
        }

        if(DisasterGameManager.instance.goods >= building.buildingData.upgradeCost)
        {
            btnUpgrade.interactable = true;
        }else btnUpgrade.interactable = false;
    }

    public void ButtonDown(int index)  //index �� ���� ���� ����
    {
        if (building == null || building.state < (BuildingState)3) return;

        switch (index)
        {
            case 0://�ǹ����� 
                if (building.buildingData.health >= building.buildingData.maxPreparation) break;
                if (DisasterGameManager.instance.goods <= building.buildingData.repairCost) break;

                DisasterGameManager.instance.goods = -building.buildingData.repairCost;
                building.buildingData.health += building.buildingData.maxPreparation * 0.1f;
                building.HealthGauge();

                if (building.buildingData.health >= building.buildingData.maxPreparation) building.buildingData.health = building.buildingData.maxPreparation;

                if(building.state == BuildingState.BUILDING5 && building.buildingData.health == building.buildingData.maxPreparation) DisasterGameManager.instance.maxLvCount++;
                SoundManager.instance.PlayEffectSound("eff_Common_timer", 1f);
                break;
            case 1: //ȣ�� ���
                if (building.buildingData.rainPreparation >= 4) break;
                if (DisasterGameManager.instance.goods <= building.buildingData.repairCost) break;
                DisasterGameManager.instance.goods = -building.buildingData.repairCost;
                building.buildingData.rainPreparation++;
                if (building.buildingData.rainPreparation >= 4) building.buildingData.rainPreparation = 4;
                SoundManager.instance.PlayEffectSound("eff_Common_timer", 1f);
                break;
            case 2: //���� ���
                if (building.buildingData.surgePreparation >= 4) break;
                if (DisasterGameManager.instance.goods <= building.buildingData.repairCost) break;
                DisasterGameManager.instance.goods = -building.buildingData.repairCost;
                building.buildingData.surgePreparation++;
                if (building.buildingData.surgePreparation >= 4) building.buildingData.surgePreparation = 4;
                SoundManager.instance.PlayEffectSound("eff_Common_timer", 1f);
                break;
            case 3: //��ǳ ���
                if (building.buildingData.typhoonPreparation >= 4) break;
                if (DisasterGameManager.instance.goods <= building.buildingData.repairCost) break;
                DisasterGameManager.instance.goods = -building.buildingData.repairCost;
                building.buildingData.typhoonPreparation++;
                if (building.buildingData.typhoonPreparation >= 4) building.buildingData.typhoonPreparation = 4;
                SoundManager.instance.PlayEffectSound("eff_Common_timer", 1f);
                break;
            case 4: //���� ���
                if (building.buildingData.earthquakePreparation >= 4) break;
                if (DisasterGameManager.instance.goods <= building.buildingData.repairCost) break;
                DisasterGameManager.instance.goods = -building.buildingData.repairCost;
                building.buildingData.earthquakePreparation++;
                if (building.buildingData.earthquakePreparation >= 4) building.buildingData.earthquakePreparation = 4;
                SoundManager.instance.PlayEffectSound("eff_Common_timer", 1f);
                break;
        }
    }


    public void BuildingUpgrade()  //�ǹ� ���� / ��ȭ
    {
        if (building == null) return;
        else if (building.state == BuildingState.BLANK)
        {
            Debug.Log("����, �������");
            SoundManager.instance.PlayEffectSound("eff_Common_timer", 1f);
            building.MakeBuilding(); 
        }
        else if (building.buildingData.health < building.buildingData.maxPreparation && building.state > (BuildingState)2)
        {
            Debug.Log("���� �ǹ��� �������ּ���.");
        }//�ǹ��� �μ��� ��� �����
        else if (building.state == (BuildingState)2)
        {
            if (DisasterGameManager.instance.goods < building.buildingData.upgradeCost)
            {
                Debug.Log("����� ��� ����");
                return;
            }
            DisasterGameManager.instance.goods = -building.buildingData.upgradeCost;
            SoundManager.instance.PlayEffectSound("eff_Common_timer", 1f);
            building.RemakeBuilding();

        }
        else
        {
            if (DisasterGameManager.instance.goods < building.buildingData.upgradeCost)
            {
                Debug.Log("���� ��� ����");
                return;
            }
            DisasterGameManager.instance.goods = -building.buildingData.upgradeCost;
            SoundManager.instance.PlayEffectSound("eff_Common_timer", 1f);
            building.BuildingUpgrade();
        }
    }

}
