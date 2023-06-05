using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 스마트팜 토양 상태 확인용 스크립트
/// </summary>

public class SmartFarmSoilStatus : MonoBehaviour
{
    SmartFarmGamePanel gameCtrl;

    [Header("오브젝트")]
    public GameObject arrow; // -308~308
    public GameObject[] valueRanges = new GameObject[2];
    public Button[] soilValueButtons;
    public Sprite[] buttonImages;

    [Header("게임데이터")]
    //작물 성장 값
    public float minSoilValue = 0f;
    public float maxSoilValue = 15f;
    public float currentSoilValue;
    public float coolTime = 1f;
    public float adjustValue = 1f;  //가감 수치 (기본 1f, 아이템 사용 시 0f 등으로 변동)
    float currentSoilTimer = 0f;
    [Tooltip("산성도 변화 수치\n산성도 수치 조절 (기본값 0.02f) 식 : 값 * Time.deltaTime")]
    public float soilChangeValue = 0.02f;

    [Header("텍스트")]
    public TextMeshProUGUI textSoilStatus;


    private void Start()
    {
        gameCtrl = GetComponent<SmartFarmGamePanel>();
        SoilDataReset();
    }

    public void SoilDataReset()  //토양 데이터 리셋
    {
        currentSoilValue = Random.Range(4f, 10f);
        soilValueButtons[0].image.sprite = buttonImages[0];
        soilValueButtons[1].image.sprite = buttonImages[1];
    }

    void SoilOptimumRangeSetup()   //토양 적정치 표시
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

    void SoilValueChange()  //토양산성도 변화
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

    void ArrowPosition()  //산성도 화살표 이동
    {
        float _posValue;

        _posValue = ((currentSoilValue / maxSoilValue) - 0.5f) * 490f;
        arrow.transform.localPosition = new Vector3(_posValue, arrow.transform.position.y, arrow.transform.position.z);
        textSoilStatus.text = currentSoilValue.ToString("f1");
    }

    public void SoilValueMinus() //산성도 낮추기
    {
        soilValueButtons[0].interactable = false;

        StartCoroutine(ValueChanges(Random.Range(0.1f, 0.5f), 0));
        StartCoroutine(CoolTime( 0));
    }

    public void SoilValuePlus()  //산성도 올리기
    {
        soilValueButtons[1].interactable = false;
        
        StartCoroutine(ValueChanges(Random.Range(0.1f, 0.5f), 1));
        StartCoroutine(CoolTime(1));
    }

    IEnumerator ValueChanges(float _float, int _int)  //산성도 변화 처리 (가감)
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

    IEnumerator CoolTime(int _num )  //조작 패널 쿨타임
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

    public void UseSoilItem(int value)   //토양 상태 변경 아이템 사용
    {
        StartCoroutine(ItemUse(value));
    }

    IEnumerator ItemUse(int value)
    {
        if (value == 0)  //수치 변화 0
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
        else if (value == 1)  //수치 증가 5
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
        else if (value == 2)  //수치 감소 5
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
