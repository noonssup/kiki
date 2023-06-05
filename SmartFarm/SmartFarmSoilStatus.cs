using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ����Ʈ�� ��� ���� Ȯ�ο� ��ũ��Ʈ
/// </summary>

public class SmartFarmSoilStatus : MonoBehaviour
{
    SmartFarmGamePanel gameCtrl;

    [Header("������Ʈ")]
    public GameObject arrow; // -308~308
    public GameObject[] valueRanges = new GameObject[2];
    public Button[] soilValueButtons;
    public Sprite[] buttonImages;

    [Header("���ӵ�����")]
    //�۹� ���� ��
    public float minSoilValue = 0f;
    public float maxSoilValue = 15f;
    public float currentSoilValue;
    public float coolTime = 1f;
    public float adjustValue = 1f;  //���� ��ġ (�⺻ 1f, ������ ��� �� 0f ������ ����)
    float currentSoilTimer = 0f;
    [Tooltip("�꼺�� ��ȭ ��ġ\n�꼺�� ��ġ ���� (�⺻�� 0.02f) �� : �� * Time.deltaTime")]
    public float soilChangeValue = 0.02f;

    [Header("�ؽ�Ʈ")]
    public TextMeshProUGUI textSoilStatus;


    private void Start()
    {
        gameCtrl = GetComponent<SmartFarmGamePanel>();
        SoilDataReset();
    }

    public void SoilDataReset()  //��� ������ ����
    {
        currentSoilValue = Random.Range(4f, 10f);
        soilValueButtons[0].image.sprite = buttonImages[0];
        soilValueButtons[1].image.sprite = buttonImages[1];
    }

    void SoilOptimumRangeSetup()   //��� ����ġ ǥ��
    {
        float valueMinPos = ((gameCtrl.soilValue[0]/maxSoilValue) - 0.5f) * 490f;
        float valueMaxPos = ((gameCtrl.soilValue[1] / maxSoilValue) - 0.5f) * 490f;
        valueRanges[0].transform.localPosition = new Vector3(valueMinPos, valueRanges[0].transform.position.y, valueRanges[0].transform.position.z);
        valueRanges[1].transform.localPosition = new Vector3(valueMaxPos, valueRanges[0].transform.position.y, valueRanges[0].transform.position.z);
    }

    private void Update()
    {
        if (gameCtrl.isGamePlay)
        {
            SoilValueChange();
        }
    }

    void SoilValueChange()  //���꼺�� ��ȭ
    {
        currentSoilTimer += Time.deltaTime;

        float flower = 1f;
        if (gameCtrl.isFlower) flower = -1.2f;
        else flower = 1f;

        if (currentSoilValue <= 7f)
        {
            currentSoilValue -= soilChangeValue * flower * Time.deltaTime * adjustValue;
            if (currentSoilValue <= minSoilValue) currentSoilValue = minSoilValue;
        }
        else if (currentSoilValue > 7f)
        {
            currentSoilValue += soilChangeValue * flower * Time.deltaTime * adjustValue;
            if (currentSoilValue >= maxSoilValue) currentSoilValue = maxSoilValue;
        }

        currentSoilTimer = 0f;
        SoilOptimumRangeSetup();
        ArrowPosition();
    }

    void ArrowPosition()  //�꼺�� ȭ��ǥ �̵�
    {
        float _posValue;

        _posValue = ((currentSoilValue / maxSoilValue) - 0.5f) * 490f;
        arrow.transform.localPosition = new Vector3(_posValue, arrow.transform.position.y, arrow.transform.position.z);
        textSoilStatus.text = currentSoilValue.ToString("f1");
    }

    public void SoilValueMinus() //�꼺�� ���߱�
    {
        soilValueButtons[0].interactable = false;

        StartCoroutine(ValueChanges(Random.Range(0.1f, 0.5f), 0));
        StartCoroutine(CoolTime( 0));
    }

    public void SoilValuePlus()  //�꼺�� �ø���
    {
        soilValueButtons[1].interactable = false;
        
        StartCoroutine(ValueChanges(Random.Range(0.1f, 0.5f), 1));
        StartCoroutine(CoolTime(1));
    }

    IEnumerator ValueChanges(float _float, int _int)  //�꼺�� ��ȭ ó�� (����)
    {
        float _value = 0;
        while (_value < _float)
        {
            _value += Time.deltaTime;
            if (_int == 0)
            {
                currentSoilValue -= Time.deltaTime * 0.5f * adjustValue;
                if (currentSoilValue <= minSoilValue) currentSoilValue = minSoilValue;
            }
            else if (_int == 1)
            {
                currentSoilValue += Time.deltaTime * 0.5f * adjustValue;
                if (currentSoilValue >= maxSoilValue) currentSoilValue = maxSoilValue;
            }
            yield return null;
        }
    }

    IEnumerator CoolTime(int _num )  //���� �г� ��Ÿ��
    {
        soilValueButtons[_num].interactable = false;
        soilValueButtons[_num].image.sprite = buttonImages[_num+2];
        float _coolTime = coolTime;
        while (_coolTime > 0f)
        {
            _coolTime -= Time.deltaTime;
            yield return null;
        }
        soilValueButtons[_num].image.sprite = buttonImages[_num];
        soilValueButtons[_num].interactable = true;
    }

    public void UseSoilItem(int value)   //��� ���� ���� ������ ���
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
        else if (value == 1)  //��ġ ���� 5
        {
            float plusValue = 5f;
            while (plusValue > 0f)
            {
                plusValue -= Time.deltaTime;
                currentSoilValue += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
        else if (value == 2)  //��ġ ���� 5
        {
            float minusValue = 5f;
            while (minusValue > 0f)
            {
                minusValue -= Time.deltaTime;
                currentSoilValue -= Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }
}
