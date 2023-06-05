using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ���� ���� ���� ��¿� ��ũ��Ʈ
/// </summary>


public class SmartFarmGrowthStatus : MonoBehaviour
{
    SmartFarmGamePanel gameCtrl;

    [Header("������Ʈ")]
    public Image cropsImage;
    public Image fillMonthImage;
    public Image fillGrowthRate;
    public GameObject arrow;

    [Header("���ӵ�����")]
    string cropsName;
    float growthRate;
    float growthSpeed;
    string cropsTips;
    public float tipsTimer = 10f;
    float tipsTimerCount = 0f;
    int cropsNumber = 0;   //�۹� ��ȣ 0 = �丶��, 1 = ���....

    [Header("�ؽ�Ʈ")]
    public TextMeshProUGUI textGrowthRate;
    public TextMeshProUGUI textGrowthSpeed;
    public TextMeshProUGUI textCropsTips;
    public TextMeshProUGUI textTipsCropsName;
    public TextMeshProUGUI textGrowthPeriod;

    List<Dictionary<string, object>> data;

    private void Start()
    {
        gameCtrl = GetComponent<SmartFarmGamePanel>();
        data = SmartFarmGameManager.instance.data;
        DataReset();
    }

    public void DataReset()
    {
        growthRate = 0f;
        textGrowthPeriod.text = "";
        textCropsTips.text = "";
        //textCropsName.text = "";
        tipsTimerCount = tipsTimer;
        cropsNumber = gameCtrl.cropsNumber;
        fillMonthImage.fillAmount = 0f;
    }

    public void TipsRenewal()  //�۹� �� ����
    {
        if (!gameCtrl.isGamePlay) return;

        int tipsIndex = Random.Range(0, 5);

        //////���̺� ���� �� �Ʒ� for�� ���
        ///
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i]["ID"].ToString() == cropsNumber.ToString())
            {
                cropsName = data[i]["name_key"].ToString();
                switch (tipsIndex)
                {
                    case 0: cropsTips = data[i]["tip1"].ToString(); break;
                    case 1: cropsTips = data[i]["tip2"].ToString(); break;
                    case 2: cropsTips = data[i]["tip3"].ToString(); break;
                    case 3: cropsTips = data[i]["tip4"].ToString(); break;
                    case 4: cropsTips = data[i]["tip5"].ToString(); break;
                }
                break;
            }
        }

        textCropsTips.text = cropsTips;
        textTipsCropsName.text = cropsName + "�� ���� �丷���";
    }

    public void CropsImageSet(Sprite _cropsSprite)   //�۹� �̹��� ����
    {
        if(this.gameObject.name != "GamePanel") cropsImage.sprite = _cropsSprite;
    }


    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            tipsTimerCount += Time.deltaTime;
            if (tipsTimer <= tipsTimerCount)
            {
                TipsRenewal();
                tipsTimerCount = 0f;
            }
            GrowthCheck();
        }
    }

    void GrowthCheck()  //����� �ݿ�
    {
        growthRate = gameCtrl.growthRate;
        fillGrowthRate.fillAmount = growthRate / 100f;
        if(gameCtrl.isGamePlay) fillMonthImage.fillAmount = gameCtrl.cropsPeriodTime / gameCtrl.cropsMaxTimer;
        textGrowthRate.text = "����� " + growthRate.ToString("F1") + "%";
        growthSpeed = gameCtrl.growthSpeed;
        textGrowthSpeed.text ="����ӵ� : " + (growthSpeed*100f).ToString("f1") + "%";
        ArrowPosition();
    }

    void ArrowPosition()  //����� ȭ��ǥ �̵�
    {
        float _posValue;
        //���� ��ü �������ͽ���
        if (this.gameObject.name == "GamePanel")
        {
            _posValue = ((growthRate / 100f) - 0.5f) * 490f;
            arrow.transform.localPosition = new Vector3(_posValue, arrow.transform.position.y, arrow.transform.position.z);
        }
    }
}
