using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 스마트팜 수분 상태 확인용 스크립트
/// </summary>

public class SmartFarmMoistureStatus : MonoBehaviour
{
    SmartFarmGamePanel gameCtrl;

    [Header("오브젝트")]
    public GameObject[] valueRanges = new GameObject[2];
    public GameObject arrow;
    public Button[] moistureValueButtons;   //살수 버튼
    public Sprite[] buttonImages;
    public Image moistureGauge;

    [Header("게임데이터")]
    public Crops crops;
    public float minMoistureValue = 0f;
    public float maxMoistureValue = 100f;
    public float currentMoistureValue = 0f;
    public float coolTime = 1f;
    public float adjustValue = 1f;  //가감 수치 (기본 1f, 아이템 사용 시 0f 등으로 변동)
    public float fillSpeed = 5f;
    [Tooltip("수분 감소 수치 (기본값 1f)  식 : 값 * Time.deltaTime")]
    public float moistureChangeValue = 1f;

    [Header("텍스트")]
    public TextMeshProUGUI textMoistureStatus;


    private void Start()
    {
        gameCtrl = GetComponent<SmartFarmGamePanel>();
        crops = SmartFarmGameManager.instance.crops;
        MoistureDataReset();
    }

    public void MoistureDataReset()   //수분 데이터 설정
    {
        currentMoistureValue = Random.Range(40f, 60f);
        moistureValueButtons[0].image.sprite = buttonImages[0];
        moistureValueButtons[1].image.sprite = buttonImages[1];
        moistureValueButtons[2].image.sprite = buttonImages[2];
    }

    void MoistureOptimumRangeSetup()   //적정 수분 데이터 설정
    {
        float valueMinPos = ((gameCtrl.moistureValue[0] / maxMoistureValue) - 0.5f) * 490f;
        float valueMaxPos = ((gameCtrl.moistureValue[1] / maxMoistureValue) - 0.5f) * 490f;
        valueRanges[0].transform.localPosition = new Vector3(valueMinPos, 0f, valueRanges[0].transform.position.z);
        valueRanges[1].transform.localPosition = new Vector3(valueMaxPos, 0f, valueRanges[0].transform.position.z);
    }

    private void Update()
    {
        if (gameCtrl.isGamePlay)
        {
            MoistureValueChange();
        }
    }

    void MoistureValueChange()  //수분 변화
    {
        float rainny = 1f;

        if (gameCtrl.isRain) rainny = -0.5f;
        else if (gameCtrl.isSun) rainny = 1.2f;
        else rainny = 1f;

        currentMoistureValue -= moistureChangeValue * rainny * Time.deltaTime * adjustValue;
        if(gameCtrl.isSun) currentMoistureValue -= moistureChangeValue * 1.5f * Time.deltaTime * adjustValue;

        if (currentMoistureValue <= 0f) currentMoistureValue = minMoistureValue;
        if (currentMoistureValue >= 100f) currentMoistureValue = maxMoistureValue;

        if (crops == (Crops)0)
        {
            currentMoistureValue = ((gameCtrl.moistureValue[0] + gameCtrl.moistureValue[1]) * 0.5f);
        }

        moistureGauge.fillAmount = currentMoistureValue / maxMoistureValue;
        MoistureOptimumRangeSetup();
        ArrowPosition();
    }
    void ArrowPosition()  //수분 화살표 이동
    {
        float _posValue;
        //변경 전체 스테이터스판
        if (this.gameObject.name == "GamePanel")
        {
            _posValue = ((currentMoistureValue / maxMoistureValue) - 0.5f) * 490f;
            arrow.transform.localPosition = new Vector3(_posValue, arrow.transform.position.y, arrow.transform.position.z);
            textMoistureStatus.text = currentMoistureValue.ToString("n0") + "%";
        }
    }


    public void MoistureValuePlus(int _num)  //수분 공급
    {
        moistureValueButtons[_num].interactable = false;

        switch (_num)
        {
            case 0:
                StartCoroutine(FillMoisture(Random.Range(5, 11),_num));
                break;
            case 1:
                StartCoroutine(FillMoisture(Random.Range(11,21),_num));
                break;
            case 2:
                StartCoroutine(FillMoisture(Random.Range(21,31), _num));
                break;
            default: return;
        }
    }

    IEnumerator FillMoisture(float _value, int _num)  //수분 공급 (서서히 수분 채우기)
    {
        float _moisture = _value;
        moistureValueButtons[_num].image.sprite = buttonImages[_num + 3];
        while (_moisture > 0f)
        {
            _moisture -= Time.deltaTime * fillSpeed;
            currentMoistureValue += Time.deltaTime * fillSpeed;

            yield return null;
        }
        moistureValueButtons[_num].image.sprite = buttonImages[_num];
        moistureValueButtons[_num].interactable = true;
    }

    public void UseMoistureItem(int value)  //수분 공급용 아이템 사용
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
        else if (value == 1)  //수치 증가 10
        {
            float plusValue = 10f;
            while (plusValue > 0f)
            {
                plusValue -= Time.deltaTime;
                currentMoistureValue += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }


}
