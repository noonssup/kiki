using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ����Ʈ�� �µ� ���� Ȯ�ο� ��ũ��Ʈ
/// </summary>

public class SmartFarmTempStatus : MonoBehaviour
{
    SmartFarmGamePanel gameCtrl;

    [Header("������Ʈ")]
    public GameObject[] valueRanges = new GameObject[2];
    public GameObject arrow;
    public Image tempGaugeImage;
    public Image[] tempControlOnOffLEDs;
    public Button[] tempControlButtons;

    [Header("���ӵ�����")]
    public Crops crops;
    public float minTemperatureValue = -30f;
    public float maxTemperatureValue = 50f;
    public float currentTempValue = 0f;
    public float adjustValue = 1f;  //���� ��ġ (�⺻ 1f, ������ ��� �� 0f ������ ����)
    public bool[] isController = new bool[6];
    [Tooltip("�µ� ��ȭ ��ġ (�⺻�� 0.5f) �� : �� * Time.deltaTime")]
    public float temperatureChangeValue = 0.5f;

    [Header("�ؽ�Ʈ")]
    public TextMeshProUGUI textTempStatus;

    private void Start()
    {
        gameCtrl = GetComponent<SmartFarmGamePanel>();
        crops = SmartFarmGameManager.instance.crops;
        TemperatureReset();
    }

    public void TemperatureReset()   //�µ� ������ ����
    {
        currentTempValue = Random.Range(10, 30);
        for (int i = 0; i < tempControlOnOffLEDs.Length; i++)
        {
            tempControlOnOffLEDs[i].enabled = false;
            isController[i] = false;
        }
    }

    void TemperatureOptimumRangeSetup()   //���� �µ� ����
    {
        float valueMinPos = (((gameCtrl.temperatureValue[0] - minTemperatureValue) / (maxTemperatureValue - minTemperatureValue)) - 0.5f) * 490f;
        float valueMaxPos = (((gameCtrl.temperatureValue[1] - minTemperatureValue) / (maxTemperatureValue - minTemperatureValue)) - 0.5f) * 490f;

        valueRanges[0].transform.localPosition = new Vector3(valueMinPos, 0f, valueRanges[0].transform.position.z);
        valueRanges[1].transform.localPosition = new Vector3(valueMaxPos, 0f, valueRanges[0].transform.position.z);
    }

    private void Update()
    {
        if (gameCtrl.isGamePlay)
        {
            TemperatureValueChange();
            TemperatureController();
        }
    }

    void TemperatureController()  //�µ���ȯ�� �۵�
    {
        if (isController[0]) currentTempValue -= 0.3f * Time.deltaTime * adjustValue; 
        if (isController[1]) currentTempValue -= 0.6f * Time.deltaTime * adjustValue; 
        if (isController[2]) currentTempValue -= 1f * Time.deltaTime * adjustValue; 
        if (isController[3]) currentTempValue += 0.3f * Time.deltaTime * adjustValue; 
        if (isController[4]) currentTempValue += 0.6f * Time.deltaTime * adjustValue; 
        if (isController[5]) currentTempValue += 1f * Time.deltaTime * adjustValue; 
    }

    void TemperatureValueChange()  //�µ���ȭ
    {
        float _month = gameCtrl.worldTimer;
        float _monthTime = gameCtrl.monthTime;
        _month = (_month / _monthTime) + 1;

        float _rate = 0f;

        switch ((int)_month)
        {
            case 1: _rate = -2f;break;
            case 2: _rate = -1.5f;break;
            case 3: _rate = -0.5f;break;
            case 4: _rate = 0.1f;break;
            case 5: _rate = 1f;break;
            case 6: _rate = 1.2f;break;
            case 7: _rate = 1.5f;break;
            case 8: _rate = 2f;break;
            case 9: _rate = 1.2f;break;
            case 10: _rate = 0.1f;break;
            case 11: _rate = -0.5f;break;
            case 12: _rate = -1.5f;break;
        }

        if (!gameCtrl.isSun && !gameCtrl.isSnow) currentTempValue += (temperatureChangeValue * _rate) * Time.deltaTime * adjustValue;
        else if (gameCtrl.isSun) currentTempValue += (temperatureChangeValue * (_rate + 0.5f)) * Time.deltaTime * adjustValue;
        else if (gameCtrl.isSnow) currentTempValue += (temperatureChangeValue * (_rate - 0.25f)) * Time.deltaTime * adjustValue;

        if (currentTempValue <= minTemperatureValue) currentTempValue = minTemperatureValue;
        else if (currentTempValue >= maxTemperatureValue) currentTempValue = maxTemperatureValue;

        if(crops == (Crops)0 || crops == (Crops)1)
        {
            currentTempValue = ((gameCtrl.temperatureValue[0] + gameCtrl.temperatureValue[1]) * 0.5f);
        }

        TemperatureOptimumRangeSetup();
        ArrowPosition();
    }

    void ArrowPosition()  //�µ� ȭ��ǥ �̵�
    {
        float _posValue;
        //���� ��ü �������ͽ���
        if (this.gameObject.name == "GamePanel")
        {
            _posValue = (((currentTempValue- minTemperatureValue) / (maxTemperatureValue- minTemperatureValue)) - 0.5f) * 490f;
            arrow.transform.localPosition = new Vector3(_posValue, arrow.transform.position.y, arrow.transform.position.z);
            textTempStatus.text = currentTempValue.ToString("f1") + "��";
            tempGaugeImage.fillAmount = (currentTempValue - minTemperatureValue) / (maxTemperatureValue - minTemperatureValue);
        }
    }

    public void TemperatureChange(int _num)   //�µ� ��ȯ�� ���
    {
        isController[_num] = !isController[_num];
        tempControlOnOffLEDs[_num].enabled = isController[_num];
    }


    public void UseTempItem(int value)   //�µ� ���� ������ ���
    {
        StartCoroutine(ItemUse(value));
    }

    IEnumerator ItemUse(int value)
    {
        if (value == 0)  //��ġ ��ȭ 0
        {
            float coolTime = 15f;
            while (coolTime > 0f)
            {
                coolTime -= Time.deltaTime;
                adjustValue = 0f;
                yield return null;
            }

            adjustValue = 1f;
            yield break;
        }
        else if (value == 1)  //��ġ ���� 10
        {
            float plusValue = 10f;
            while (plusValue > 0f)
            {
                plusValue -= Time.deltaTime;
                currentTempValue += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
        else if (value == 2)  //��ġ ���� 10
        {
            float minusValue = 10f;
            while (minusValue > 0f)
            {
                minusValue -= Time.deltaTime;
                currentTempValue -= Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}
