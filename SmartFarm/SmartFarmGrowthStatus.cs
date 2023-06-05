using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 성장 탭의 정보 출력용 스크립트
/// </summary>


public class SmartFarmGrowthStatus : MonoBehaviour
{
    SmartFarmGamePanel gameCtrl;

    [Header("오브젝트")]
    public Image cropsImage;
    public Image fillMonthImage;
    public Image fillGrowthRate;
    public GameObject arrow;

    [Header("게임데이터")]
    string cropsName;
    float growthRate;
    float growthSpeed;
    string cropsTips;
    public float tipsTimer = 10f;
    float tipsTimerCount = 0f;
    int cropsNumber = 0;   //작물 번호 0 = 토마토, 1 = 사과....

    [Header("텍스트")]
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

    public void TipsRenewal()  //작물 팁 갱신
    {
        if (!gameCtrl.isGamePlay) return;

        int tipsIndex = Random.Range(0, 5);

        //////테이블 적용 시 아래 for문 사용
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
        textTipsCropsName.text = cropsName + "에 대한 토막상식";
    }

    public void CropsImageSet(Sprite _cropsSprite)   //작물 이미지 설정
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

    void GrowthCheck()  //성장률 반영
    {
        growthRate = gameCtrl.growthRate;
        fillGrowthRate.fillAmount = growthRate / 100f;
        if(gameCtrl.isGamePlay) fillMonthImage.fillAmount = gameCtrl.cropsPeriodTime / gameCtrl.cropsMaxTimer;
        textGrowthRate.text = "성장률 " + growthRate.ToString("F1") + "%";
        growthSpeed = gameCtrl.growthSpeed;
        textGrowthSpeed.text ="성장속도 : " + (growthSpeed*100f).ToString("f1") + "%";
        ArrowPosition();
    }

    void ArrowPosition()  //성장률 화살표 이동
    {
        float _posValue;
        //변경 전체 스테이터스판
        if (this.gameObject.name == "GamePanel")
        {
            _posValue = ((growthRate / 100f) - 0.5f) * 490f;
            arrow.transform.localPosition = new Vector3(_posValue, arrow.transform.position.y, arrow.transform.position.z);
        }
    }
}
