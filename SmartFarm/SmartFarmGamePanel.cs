using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����Ʈ�� ���� ���� ��Ʈ�� ��ũ��Ʈ
/// </summary>

public class SmartFarmGamePanel : MonoBehaviour
{
    [Header("���ӿ�����Ʈ")]
    SmartFarmTapButton tapCtrl;
    SmartFarmGrowthStatus growthCtrl;
    SmartFarmSoilStatus soilCtrl;
    SmartFarmMoistureStatus moistureCtrl;
    SmartFarmTempStatus temperatureCtrl;
    public GameObject cropsObj;  //ȭ�� ���� ��ܿ� ǥ�õ� �۹��� �̹���������Ʈ
    public GameObject startPanel; //���ӽ��� �˾�
    public GameObject resultPanel;
    public GameObject arrow;
    public Transform cropsPosTr;
    public Transform cropsParentTr;  //�۹��� ��ġ�� �θ������Ʈ Ʈ������
    public SmartFarmTapButton gameStatusPanel; //�������ͽ� �г�
    public Sprite[] cropSprites;

    [Header("���ӵ�����")]
    public int gameMoney = 0;
    public int itemUseCount = 3;
    [Tooltip("�Ѵ� ���� �ð� (�⺻�� 60f)")] public float monthTime = 60f;
    float weekTime;
    public float worldTimer;  //�Ѵ��� 60��
    public float cropsPeriodTime;  //�۹��� �ɾ��� ������ �ð�
    public float cropsMaxTimer; //�۹��� ���� �ð�
    int growthMonth = 0;      //���� �۹��� ��� �Ⱓ
    float growthDate = 0f;   //�۹� ��� ���� ������ �ð�
    public float growthSpeed = 0f;   //����ӵ�
    public float growthRate;         //�����
    public int cropsNumber = 0; //�۹� ��ȣ
    string cropsName;  //�۹� �̸�
    public bool isGamePlay = false;
    public bool isGrowthCheck = false;
    //��� ���� ����
    float growthStartTime;
    float growthEndTime;
    //�۹� ȯ�濡 ���� �����
    float soilGrowthRate = 1f;
    float moistureGrowthRate = 1f;
    float temperatureGrowthRate = 1f;
    [Header("�۹� ȯ���ġ min / max")]
    //�۹� ȯ�� ��ġ (min, max)
    public float[] soilValue = new float[2];
    public float[] moistureValue = new float[2];
    public float[] temperatureValue = new float[2];

    [Header("�ؽ�Ʈ")]
    public TextMeshProUGUI textItemUseCount;
    public TextMeshProUGUI textWorldTimer;
    public TextMeshProUGUI textCropPeriodTime;
    public TextMeshProUGUI textResultGrowth;
    public TextMeshProUGUI textResultComment;

    [Header("����ȿ��")]
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

    void ParticleObjectSetup() //���� ȿ�� ����
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

    public void GameStatusSetup(int _fruitCount)  //�۹� ���� �Ҵ�
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

        textWorldTimer.text = (Mathf.FloorToInt(worldTimer / (float)monthTime) + 1).ToString("00") + "�� ";
        textCropPeriodTime.text = ((cropsPeriodTime / weekTime) + 1).ToString("00") + "��";

        StatusDataReset();
        CropsStatusSetup();

        if (!startPanel.activeSelf) startPanel.SetActive(true);
        StartCoroutine(GrowthStart());
    }


    void StatusDataReset()  //���� ���� ���� (����, ���, ����, �µ�)
    {
        growthCtrl.DataReset();
        temperatureCtrl.TemperatureReset();
        moistureCtrl.MoistureDataReset();
        soilCtrl.SoilDataReset();

        if (cropsNumber == 0)  //�丶�� ����� ���, ����Ǹ� Ȱ��ȭ
        {
            tapCtrl.tapBtns[1].interactable = false;
            tapCtrl.tapBtns[2].interactable = false;
            tapCtrl.tapBtns[3].interactable = false;
        }
        else if (cropsNumber == 1)  //���� ����� ���, ���/�����Ǹ� Ȱ��ȭ
        {
            tapCtrl.tapBtns[1].interactable = true;
            tapCtrl.tapBtns[2].interactable = false;
            tapCtrl.tapBtns[3].interactable = false;
        }
        else  //�̿� ��� �� Ȱ��ȭ
        {
            for (int i = 0; i < tapCtrl.tapBtns.Length; i++) tapCtrl.tapBtns[i].interactable = true;
        }
    }

    void CropsStatusSetup()  //�۹� ��� ���� �ʱ�ȭ
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

    //public void GameStart(GameObject _startPanel)  //���ӽ�ŸƮ
    //{
    //    _startPanel.SetActive(false);
    //    StartCoroutine(GrowthStart());

    //}

    IEnumerator GrowthStart()   //�۹� ��� ����
    {
        //Ʃ�丮���� ���� ����,�µ� ��Ʈ�ѷ� ��ũ��Ʈ�� �۹��� ���� (�丶��, ���� ����)
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

        //����� ���


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

        //���â Ȱ��ȭ
        resultPanel.SetActive(true);
        var _resultCrops = Instantiate(_cropsObj.gameObject).GetComponent<SmartFarmCrops>();
        _resultCrops.transform.SetParent(cropsPosTr);
        _resultCrops.transform.localScale = Vector3.one;
        _resultCrops.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(_resultCrops.GrowthRateCheck());

        _resultCrops.transform.Find("ResultFlag").gameObject.SetActive(true);
        TextMeshProUGUI resultText = _resultCrops.transform.Find("ResultFlag").transform.GetComponentInChildren<TextMeshProUGUI>();
        textResultGrowth.text = "����� : " + growthRate.ToString("F1") + "%";
        Color _color = new Color();
        if (growthRate >= 80f)
        {
            SoundManager.instance.PlayEffectSound("eff_Common_clear", 1f);
            textResultComment.text = cropsName + " ��� ����";
            resultText.text = "Success!";
            ColorUtility.TryParseHtmlString("#f5a032ff", out _color);
            resultText.color = _color;
        }
        else if (growthRate < 80f)
        {
            SoundManager.instance.PlayEffectSound("eff_Barista_wrong", 1f);
            textResultComment.text = cropsName + " ��� ����";
            resultText.text = "Fail...";
            ColorUtility.TryParseHtmlString("#ff0000ff", out _color);
            resultText.color = _color;
        }

        //���� ������ ��� �۹�ī��Ʈ �߰� (���� �۹� �߰� / �߰� �۹� 2���� 2023.01.27 ����)
        if (growthRate > 80f && cropsNumber == 0)
        {
            if (SmartFarmGameManager.instance.fruitCount >= 2) yield break;
            SmartFarmGameManager.instance.fruitCount = 2; //�ʱ� �۹��� �丶���� ��� fruitCount �� 2�� �����Ͽ� ���� �߰�
        }
        else if (growthRate > 80f && cropsNumber == 1) SmartFarmGameManager.instance.fruitCount = 3;  //���� ��踦 �Ϸ��� ��� fruitCount �� 3���� �����Ͽ� ���� �߰�

    }

    //int ResultMoney(SmartFarmCrops _crop) //��ȭ ȹ���� �ִٸ�...
    //{
    //    float _money = 0;
    //    if (growthRate > 90f) _money = _crop.cropsMoney;
    //    else if (growthRate > 80f) _money = _crop.cropsMoney * 0.75f;
    //    else if (growthRate > 70f) _money = _crop.cropsMoney * 0.5f;
    //    else if (growthRate > 60f) _money = _crop.cropsMoney * 0.25f;

    //    return (int)_money;
    //}

    void WorldTimerSet()  //��¥�� �帧
    {
        if (worldTimer > monthTime * 12) worldTimer = 0f;
        textWorldTimer.text = (Mathf.FloorToInt(worldTimer / (float)monthTime) + 1).ToString("00") + "�� ";
        textCropPeriodTime.text = ((cropsPeriodTime / weekTime) + 1).ToString("00") + "�� / " + ((cropsMaxTimer / weekTime) + 1).ToString("00") + "��";

        if (isWeather) return; //���� ȿ�� ���� ���̸� ����

        //������ ���� ���� ȿ��
        if ((((int)worldTimer / monthTime) + 1) >= 3 && (((int)worldTimer / monthTime) + 1) < 12 && !isWeather) //����������
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
        else if ((((int)worldTimer / monthTime) + 1) >= 12 || (((int)worldTimer / monthTime) + 1) < 3 && !isWeather) //�ܿ�
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

    IEnumerator ParticleEffectPlay(GameObject _effect, bool _bool) //���� ȿ�� ����
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

    void CropsGrowth(SmartFarmCrops _crops)  //����� Ȯ��
    {
        //����� ��� ó��
        if (!isGamePlay) return;
        #region ���
        if (soilCtrl.currentSoilValue >= soilValue[0] && soilCtrl.currentSoilValue <= soilValue[1])  //���� ��ġ
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

        #region ����
        if (moistureCtrl.currentMoistureValue >= moistureValue[0] && moistureCtrl.currentMoistureValue <= moistureValue[1])  //���� ��ġ
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

        //�۹��� �丶������ ��� ���п� ���� ������� 1f�� ����
        //if (cropsNumber == 0)
        //{
        //    moistureGrowthRate = 1f;
        //    textMoistureStatus.text = ((moistureValue[0] + moistureValue[1]) * 0.5f).ToString("N0") + "%";
        //} else
        //    textMoistureStatus.text = moistureCtrl.currentMoistureValue.ToString("N0") + "%";
        #endregion

        #region �µ�
        if (temperatureCtrl.currentTempValue >= temperatureValue[0] && temperatureCtrl.currentTempValue <= temperatureValue[1])  //���� ��ġ
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

        //�۹��� �丶��, ������ ��� �µ��� ���� ������� 1f�� ����
        //if (cropsNumber == 0 || cropsNumber == 1)
        //{
        //    temperatureGrowthRate = 1f;
        //    textTemperatureStatus.text = ((temperatureValue[0] + temperatureValue[1]) * 0.5f).ToString("f1") + "��";
        //} else textTemperatureStatus.text = temperatureCtrl.currentTempValue.ToString("f1") + "��";
        #endregion

        #region �����/�ӵ�
        float _totalGrowthRate = (100f / cropsMaxTimer) * ((soilGrowthRate + moistureGrowthRate + temperatureGrowthRate) / 3);
        growthRate += (_totalGrowthRate * 1f) * Time.deltaTime; //�����
        growthSpeed = _totalGrowthRate / (100f / cropsMaxTimer);  //����ӵ�
        #endregion

        #region ������
        //if (growthSpeed < 1f)
        //{
        //    poorRate += (1f - growthSpeed) * Time.deltaTime;
        //}

        //if (poorRate > 1f) poorRate = 1f;

        //if (poorRate < 0.2f)
        //{
        //    textPoorRate.text = "������ ����\n����";
        //}
        //else if (poorRate > 0.7f)
        //{
        //    textPoorRate.text = "������ ����\n�ſ� ����";
        //}
        //else if (poorRate > 0.5f)
        //{
        //    textPoorRate.text = "������ ����\n����";
        //}
        //else if (poorRate > 0.2f)
        //{
        //    textPoorRate.text = "������ ����\n���� ����";
        //}

        //poorGaugeImage.fillAmount = poorRate / 1f;
        //float _posValue = ((poorRate / 1) - 0.5f) * 196f; // ȭ��ǥ ������ <����-308~308> <����-98~98>
        //arrow.transform.localPosition = new Vector3(arrow.transform.position.x + 30f, _posValue, arrow.transform.position.z);
        #endregion

        //����� ��� ó�� �� �۹��� ����� ��ġ ����
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

    IEnumerator ItemUse(float value)  //����� ���� (5f)
    {
        while(value> 0f)
        {
            value -= Time.deltaTime;
            growthRate+= Time.deltaTime;
            yield return null;
        }
        
    }
}
