using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 스마트팜 게임 로직 컨트롤 스크립트
/// </summary>

public class SmartFarmGamePanel : MonoBehaviour
{
    [Header("게임오브젝트")]
    SmartFarmTapButton tapCtrl;
    SmartFarmGrowthStatus growthCtrl;
    SmartFarmSoilStatus soilCtrl;
    SmartFarmMoistureStatus moistureCtrl;
    SmartFarmTempStatus temperatureCtrl;
    public GameObject cropsObj;  //화면 좌측 상단에 표시될 작물의 이미지오브젝트
    public GameObject startPanel; //게임시작 팝업
    public GameObject resultPanel;
    public GameObject arrow;
    public Transform cropsPosTr;
    public Transform cropsParentTr;  //작물이 위치할 부모오브젝트 트랜스폼
    public SmartFarmTapButton gameStatusPanel; //스테이터스 패널
    public Sprite[] cropSprites;

    [Header("게임데이터")]
    public int gameMoney = 0;
    public int itemUseCount = 3;
    [Tooltip("한달 기준 시간 (기본값 60f)")] public float monthTime = 60f;
    float weekTime;
    public float worldTimer;  //한달은 60초
    public float cropsPeriodTime;  //작물이 심어진 이후의 시간
    public float cropsMaxTimer; //작물별 성장 시간
    int growthMonth = 0;      //실제 작물의 재배 기간
    float growthDate = 0f;   //작물 재배 시작 이후의 시간
    public float growthSpeed = 0f;   //성장속도
    public float growthRate;         //성장률
    public int cropsNumber = 0; //작물 번호
    string cropsName;  //작물 이름
    public bool isGamePlay = false;
    public bool isGrowthCheck = false;
    //재배 시작 시점
    float growthStartTime;
    float growthEndTime;
    //작물 환경에 따른 성장률
    float soilGrowthRate = 1f;
    float moistureGrowthRate = 1f;
    float temperatureGrowthRate = 1f;
    [Header("작물 환경수치 min / max")]
    //작물 환경 수치 (min, max)
    public float[] soilValue = new float[2];
    public float[] moistureValue = new float[2];
    public float[] temperatureValue = new float[2];

    [Header("텍스트")]
    public TextMeshProUGUI textItemUseCount;
    public TextMeshProUGUI textWorldTimer;
    public TextMeshProUGUI textCropPeriodTime;
    public TextMeshProUGUI textResultGrowth;
    public TextMeshProUGUI textResultComment;

    [Header("날씨효과")]
    public bool isRain = false;
    public bool isSnow = false;
    public bool isFlower = false;
    public bool isSun = false;
    public bool isNormal = false;
    public bool isWeather = false;
    public GameObject rain;
    public GameObject snow;
    public GameObject flower;
    public GameObject sunny;
    public GameObject normal;

    List<Dictionary<string, object>> data;

    private void Start()
    {
        growthCtrl = GetComponent<SmartFarmGrowthStatus>();
        soilCtrl = GetComponent<SmartFarmSoilStatus>();
        moistureCtrl = GetComponent<SmartFarmMoistureStatus>();
        temperatureCtrl = GetComponent<SmartFarmTempStatus>();
        tapCtrl = FindObjectOfType<SmartFarmTapButton>();
        data = SmartFarmGameManager.instance.data;
        cropSprites = SmartFarmGameManager.instance.cropSprites;
    }

    private void OnEnable()
    {
        if (PlayerPrefs.HasKey("money"))
        {
            gameMoney = PlayerPrefs.GetInt("money");
        }
        else { gameMoney = 0; }

        itemUseCount = 3;
        textItemUseCount.text = itemUseCount.ToString();
        if (startPanel.activeSelf) startPanel.SetActive(false);
        if (cropsPosTr.childCount > 0) Destroy(cropsPosTr.GetChild(0).gameObject);
        if (resultPanel.activeSelf) resultPanel.SetActive(false);
        gameStatusPanel.SelectTap(0);
        ParticleObjectSetup();
    }

    void ParticleObjectSetup() //날씨 효과 끄기
    {
        isWeather = false;
        isRain = false;
        isSnow = false;
        isFlower = false;
        isSun = false;
        isNormal = false;
        rain.SetActive(false);
        snow.SetActive(false);
        flower.SetActive(false);
        sunny.SetActive(false);
        normal.SetActive(false);
    }

    public void GameStatusSetup(int _fruitCount)  //작물 정보 할당
    {
        cropsPeriodTime = 0f;
        growthRate = 0f;
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i]["ID"].ToString() == _fruitCount.ToString())
            {
                cropsName = data[i]["name_key"].ToString();
                growthMonth = int.Parse(data[i]["end_month"].ToString());
            }
        }

        worldTimer = Random.Range(0f, (monthTime * 12f));
        cropsMaxTimer = monthTime * (float)growthMonth;
        cropsNumber = _fruitCount;
        growthStartTime = worldTimer;
        growthEndTime = growthStartTime + cropsMaxTimer;
        weekTime = monthTime / 4;

        textWorldTimer.text = (Mathf.FloorToInt(worldTimer / (float)monthTime) + 1).ToString("00") + "월 ";
        textCropPeriodTime.text = ((cropsPeriodTime / weekTime) + 1).ToString("00") + "주";

        StatusDataReset();
        CropsStatusSetup();

        if (!startPanel.activeSelf) startPanel.SetActive(true);
        StartCoroutine(GrowthStart());
    }


    void StatusDataReset()  //상태 정보 리셋 (성장, 토양, 수분, 온도)
    {
        growthCtrl.DataReset();
        temperatureCtrl.TemperatureReset();
        moistureCtrl.MoistureDataReset();
        soilCtrl.SoilDataReset();

        if (cropsNumber == 0)  //토마토 재배일 경우, 토양탭만 활성화
        {
            tapCtrl.tapBtns[1].interactable = false;
            tapCtrl.tapBtns[2].interactable = false;
            tapCtrl.tapBtns[3].interactable = false;
        }
        else if (cropsNumber == 1)  //수박 재배일 경우, 토양/수분탭만 활성화
        {
            tapCtrl.tapBtns[1].interactable = true;
            tapCtrl.tapBtns[2].interactable = false;
            tapCtrl.tapBtns[3].interactable = false;
        }
        else  //이외 모든 탭 활성화
        {
            for (int i = 0; i < tapCtrl.tapBtns.Length; i++) tapCtrl.tapBtns[i].interactable = true;
        }
    }

    void CropsStatusSetup()  //작물 재배 조건 초기화
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (cropsNumber == int.Parse(data[i]["ID"].ToString()))
            {
                soilValue[0] = float.Parse(data[i]["soil_value1_min"].ToString());
                soilValue[1] = float.Parse(data[i]["soil_value1_max"].ToString());
                moistureValue[0] = float.Parse(data[i]["moist_value1_min"].ToString());
                moistureValue[1] = float.Parse(data[i]["moist_value1_max"].ToString());
                temperatureValue[0] = float.Parse(data[i]["temp_value1_min"].ToString());
                temperatureValue[1] = float.Parse(data[i]["temp_value1_max"].ToString());
                break;
            }
        }
    }

    //public void GameStart(GameObject _startPanel)  //게임스타트
    //{
    //    _startPanel.SetActive(false);
    //    StartCoroutine(GrowthStart());

    //}

    IEnumerator GrowthStart()   //작물 재배 시작
    {
        //튜토리얼을 위한 수분,온도 컨트롤러 스크립트에 작물명 전달 (토마토, 수박 한정)
        temperatureCtrl.crops = (Crops)cropsNumber;
        moistureCtrl.crops = (Crops)cropsNumber;

        while (startPanel.activeSelf)
        {
            yield return null;
        }

        isGamePlay = true;

        growthCtrl.TipsRenewal();

        if (cropsParentTr.childCount > 0) Destroy(cropsParentTr.GetChild(0).gameObject);
        var _cropsObj = Instantiate(cropsObj.gameObject).GetComponent<SmartFarmCrops>();
        _cropsObj.transform.SetParent(cropsParentTr);
        _cropsObj.transform.localScale = Vector3.one;
        _cropsObj.transform.localPosition = Vector3.zero;
        _cropsObj.fruit = cropsNumber;
        _cropsObj.transform.Find("ResultFlag").gameObject.SetActive(false);

        //배경음 출력


        while (cropsPeriodTime < cropsMaxTimer)
        {
            cropsPeriodTime += Time.deltaTime;
            worldTimer += Time.deltaTime;
            WorldTimerSet();


            CropsGrowth(_cropsObj);

            if (growthRate >= 100f)
            {
                break;
            }

            yield return null;
        }

        isGamePlay = false;

        //결과창 활성화
        resultPanel.SetActive(true);
        var _resultCrops = Instantiate(_cropsObj.gameObject).GetComponent<SmartFarmCrops>();
        _resultCrops.transform.SetParent(cropsPosTr);
        _resultCrops.transform.localScale = Vector3.one;
        _resultCrops.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(_resultCrops.GrowthRateCheck());

        _resultCrops.transform.Find("ResultFlag").gameObject.SetActive(true);
        TextMeshProUGUI resultText = _resultCrops.transform.Find("ResultFlag").transform.GetComponentInChildren<TextMeshProUGUI>();
        textResultGrowth.text = "성장률 : " + growthRate.ToString("F1") + "%";
        Color _color = new Color();
        if (growthRate >= 80f)
        {
            SoundManager.instance.PlayEffectSound("eff_Common_clear", 1f);
            textResultComment.text = cropsName + " 재배 성공";
            resultText.text = "Success!";
            ColorUtility.TryParseHtmlString("#f5a032ff", out _color);
            resultText.color = _color;
        }
        else if (growthRate < 80f)
        {
            SoundManager.instance.PlayEffectSound("eff_Barista_wrong", 1f);
            textResultComment.text = cropsName + " 재배 실패";
            resultText.text = "Fail...";
            ColorUtility.TryParseHtmlString("#ff0000ff", out _color);
            resultText.color = _color;
        }

        //익은 열매일 경우 작물카운트 추가 (다음 작물 추가 / 추가 작물 2가지 2023.01.27 기준)
        if (growthRate > 80f && cropsNumber == 0)
        {
            if (SmartFarmGameManager.instance.fruitCount >= 2) yield break;
            SmartFarmGameManager.instance.fruitCount = 2; //초기 작물이 토마토일 경우 fruitCount 를 2로 변경하여 수박 추가
        }
        else if (growthRate > 80f && cropsNumber == 1) SmartFarmGameManager.instance.fruitCount = 3;  //수박 재배를 완료할 경우 fruitCount 를 3으로 변경하여 참외 추가

    }

    //int ResultMoney(SmartFarmCrops _crop) //재화 획득이 있다면...
    //{
    //    float _money = 0;
    //    if (growthRate > 90f) _money = _crop.cropsMoney;
    //    else if (growthRate > 80f) _money = _crop.cropsMoney * 0.75f;
    //    else if (growthRate > 70f) _money = _crop.cropsMoney * 0.5f;
    //    else if (growthRate > 60f) _money = _crop.cropsMoney * 0.25f;

    //    return (int)_money;
    //}

    void WorldTimerSet()  //날짜의 흐름
    {
        if (worldTimer > monthTime * 12) worldTimer = 0f;
        textWorldTimer.text = (Mathf.FloorToInt(worldTimer / (float)monthTime) + 1).ToString("00") + "월 ";
        textCropPeriodTime.text = ((cropsPeriodTime / weekTime) + 1).ToString("00") + "주 / " + ((cropsMaxTimer / weekTime) + 1).ToString("00") + "주";

        if (isWeather) return; //계절 효과 적용 중이면 무시

        //계절에 따른 날씨 효과
        if ((((int)worldTimer / monthTime) + 1) >= 3 && (((int)worldTimer / monthTime) + 1) < 12 && !isWeather) //봄여름가을
        {
            int index = Random.Range(0, 5);
            switch (index)
            {
                case 0:
                    isWeather = true; isRain = true; StartCoroutine(ParticleEffectPlay(rain, isWeather));
                    SoundManager.instance.PlayEffectSound("eff_SmartFarm_rain", 1f); break;
                case 1:
                    isWeather = true; isFlower = true; StartCoroutine(ParticleEffectPlay(flower, isWeather));
                    SoundManager.instance.PlayEffectSound("eff_SmartFarm_bird", 1f); ; break;
                case 2:
                    isWeather = true; isSun = true; StartCoroutine(ParticleEffectPlay(sunny, isWeather));
                    SoundManager.instance.PlayEffectSound("eff_SmartFarm_sun", 1f); break;
                //case 3: isSnow = true; StartCoroutine(ParticleEffectPlay(snow, isSnow)); break;
                default: isWeather = true; isNormal = true; StartCoroutine(ParticleEffectPlay(normal, isWeather)); break;
            }
        }
        else if ((((int)worldTimer / monthTime) + 1) >= 12 || (((int)worldTimer / monthTime) + 1) < 3 && !isWeather) //겨울
        {
            int index = Random.Range(0, 5);
            switch (index)
            {
                case 0:
                    isWeather = true; isRain = true; StartCoroutine(ParticleEffectPlay(rain, isWeather));
                    SoundManager.instance.PlayEffectSound("eff_SmartFarm_rain", 1f); break;
                //case 1: isFlower = true; StartCoroutine(ParticleEffectPlay(flower, isFlower)); break;
                case 2:
                    isWeather = true; isSun = true; StartCoroutine(ParticleEffectPlay(sunny, isWeather));
                    SoundManager.instance.PlayEffectSound("eff_SmartFarm_sun", 1f); break;
                case 3:
                    isWeather = true; isSnow = true; StartCoroutine(ParticleEffectPlay(snow, isWeather));
                    SoundManager.instance.PlayEffectSound("eff_Common_wind", 1f); break;
                default: isWeather = true; isNormal = true; StartCoroutine(ParticleEffectPlay(normal, isWeather)); break;
            }
        }
    }

    IEnumerator ParticleEffectPlay(GameObject _effect, bool _bool) //날씨 효과 적용
    {
        float particleTimer = 0f;
        int particleLimit = Random.Range(5, 15);

        _effect.SetActive(_bool);

        while (particleTimer < (float)particleLimit)
        {
            particleTimer += Time.deltaTime;
            yield return null;
        }

        _effect.SetActive(false);

        isRain = false;
        isSnow = false;
        isFlower = false;
        isSun = false;
        isNormal = false;
        isWeather = false;
    }

    void CropsGrowth(SmartFarmCrops _crops)  //성장률 확인
    {
        //성장률 계산 처리
        if (!isGamePlay) return;
        #region 토양
        if (soilCtrl.currentSoilValue >= soilValue[0] && soilCtrl.currentSoilValue <= soilValue[1])  //정상 수치
        {
            soilGrowthRate = 1f;
        }
        else if (soilCtrl.currentSoilValue < soilValue[0])
        {

            soilGrowthRate = 1 - ((soilValue[0] - soilCtrl.currentSoilValue) / soilValue[0]);
        }
        else if (soilCtrl.currentSoilValue > soilValue[1])
        {
            soilGrowthRate = (soilCtrl.maxSoilValue - soilCtrl.currentSoilValue) / (soilCtrl.maxSoilValue - soilValue[1]);
        }

        if (soilGrowthRate <= 0.1f) soilGrowthRate = 0.1f;
        //textSoilStatus.text = soilCtrl.currentSoilValue.ToString("F1");
        #endregion

        #region 수분
        if (moistureCtrl.currentMoistureValue >= moistureValue[0] && moistureCtrl.currentMoistureValue <= moistureValue[1])  //정상 수치
        {
            moistureGrowthRate = 1f;
        }
        else if (moistureCtrl.currentMoistureValue < moistureValue[0])
        {

            moistureGrowthRate = 1 - ((moistureValue[0] - moistureCtrl.currentMoistureValue) / moistureValue[0]);
        }
        else if (moistureCtrl.currentMoistureValue > moistureValue[1])
        {
            moistureGrowthRate = (moistureCtrl.maxMoistureValue - moistureCtrl.currentMoistureValue) / (moistureCtrl.maxMoistureValue - moistureValue[1]);
        }

        if (moistureGrowthRate <= 0.1f) moistureGrowthRate = 0.1f;

        //작물이 토마이토일 경우 수분에 의한 성장률은 1f로 고정
        //if (cropsNumber == 0)
        //{
        //    moistureGrowthRate = 1f;
        //    textMoistureStatus.text = ((moistureValue[0] + moistureValue[1]) * 0.5f).ToString("N0") + "%";
        //} else
        //    textMoistureStatus.text = moistureCtrl.currentMoistureValue.ToString("N0") + "%";
        #endregion

        #region 온도
        if (temperatureCtrl.currentTempValue >= temperatureValue[0] && temperatureCtrl.currentTempValue <= temperatureValue[1])  //정상 수치
        {
            temperatureGrowthRate = 1f;
        }
        else if (temperatureCtrl.currentTempValue < temperatureValue[0])
        {

            temperatureGrowthRate = 1 - ((temperatureValue[0] - temperatureCtrl.currentTempValue) / temperatureValue[0]);
        }
        else if (temperatureCtrl.currentTempValue > temperatureValue[1])
        {
            temperatureGrowthRate = (temperatureCtrl.maxTemperatureValue - temperatureCtrl.currentTempValue) / (temperatureCtrl.maxTemperatureValue - temperatureValue[1]);
        }

        if (temperatureGrowthRate <= 0.1f) temperatureGrowthRate = 0.1f;

        //작물이 토마토, 수박일 경우 온도에 의한 성장률은 1f로 고정
        //if (cropsNumber == 0 || cropsNumber == 1)
        //{
        //    temperatureGrowthRate = 1f;
        //    textTemperatureStatus.text = ((temperatureValue[0] + temperatureValue[1]) * 0.5f).ToString("f1") + "℃";
        //} else textTemperatureStatus.text = temperatureCtrl.currentTempValue.ToString("f1") + "℃";
        #endregion

        #region 성장률/속도
        float _totalGrowthRate = (100f / cropsMaxTimer) * ((soilGrowthRate + moistureGrowthRate + temperatureGrowthRate) / 3);
        growthRate += (_totalGrowthRate * 1f) * Time.deltaTime; //성장률
        growthSpeed = _totalGrowthRate / (100f / cropsMaxTimer);  //성장속도
        #endregion

        #region 병충해
        //if (growthSpeed < 1f)
        //{
        //    poorRate += (1f - growthSpeed) * Time.deltaTime;
        //}

        //if (poorRate > 1f) poorRate = 1f;

        //if (poorRate < 0.2f)
        //{
        //    textPoorRate.text = "병충해 상태\n보통";
        //}
        //else if (poorRate > 0.7f)
        //{
        //    textPoorRate.text = "병충해 상태\n매우 나쁨";
        //}
        //else if (poorRate > 0.5f)
        //{
        //    textPoorRate.text = "병충해 상태\n나쁨";
        //}
        //else if (poorRate > 0.2f)
        //{
        //    textPoorRate.text = "병충해 상태\n조금 나쁨";
        //}

        //poorGaugeImage.fillAmount = poorRate / 1f;
        //float _posValue = ((poorRate / 1) - 0.5f) * 196f; // 화살표 포지션 <가로-308~308> <세로-98~98>
        //arrow.transform.localPosition = new Vector3(arrow.transform.position.x + 30f, _posValue, arrow.transform.position.z);
        #endregion

        //성장률 계산 처리 후 작물에 성장률 수치 전달
        if (growthRate >= 100f) { growthRate = 100f; }
        _crops.growthRate = growthRate;
    }



    public void UseItems(int itemIndex)
    {
        if(itemUseCount <= 0) return;
        itemUseCount--;
        if(itemUseCount <=0) itemUseCount= 0;
        textItemUseCount.text = itemUseCount.ToString();

        switch (itemIndex)
        {
            case 0: StartCoroutine(ItemUse(5f)); break;
            case 1: soilCtrl.UseSoilItem(2); break;
            case 2: soilCtrl.UseSoilItem(1); break;
            case 3: moistureCtrl.UseMoistureItem(1); break;
            case 4: temperatureCtrl.UseTempItem(2); break;
            case 5: temperatureCtrl.UseTempItem(1); break;
            case 6: soilCtrl.UseSoilItem(0); break;
            case 7: moistureCtrl.UseMoistureItem(0); break;
            case 8: temperatureCtrl.UseTempItem(0); break;
        }
        SoundManager.instance.PlayEffectSound("eff_Motor_appear", 1f);
    }

    IEnumerator ItemUse(float value)  //성장률 증가 (5f)
    {
        while(value> 0f)
        {
            value -= Time.deltaTime;
            growthRate+= Time.deltaTime;
            yield return null;
        }
        
    }
}
