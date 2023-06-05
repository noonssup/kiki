using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MoveDirType
{
    STOP=0,
    RIGHT=1,
    LEFT=2,
    UP=3,
}


public class PrinterMachineController : MonoBehaviour
{
    public PrinterUIController uiCtrl;
    MoveDirType moveDirType;

    [Header("노즐")]
    public RectTransform nozzleTr;
    public GameObject nozzle;
    public Vector3 moveDir = Vector3.right;
    public float minX = -150f;
    public float maxX = 150f;
    public float minY = -80f;
    public float maxY = 150f;
    public float yPlus = 20f;
    public float prevYpos = 0f;
    public bool isMoveRight = true;
    public float normalSpeed = 0.5f;
    public float highSpeed = 1.5f;
    public float currentSpeed = 1f;

    [Header("부스터")]
    public Image imgBoosterGauge;
    public bool isBooster = false;
    public bool isOverHeat = false;
    public float boosterGauge = 0f;
    public float boosterLimit = 100f;
    public int overHeatCount = 0;
    public float heatValue = 5f;
    public float coolValue = 3f;

    [Header("작업물")]
    public Image productImage;
    public int productProgress = 0;

    private void Update()
    {
        if (PrinterGameManager.instance.gameState == PrinterGameState.WORK)
        {
            MoveDirectionChange();
            NozzleMove();
            BoosterOn();
        }
    }

    #region 프린터 정보
    public void MachineSetup()  //프린터 초기 설정
    {
        moveDirType = MoveDirType.RIGHT;
        isMoveRight = true;
        productProgress = 0;
        overHeatCount = 0;
        currentSpeed = normalSpeed;
        productImage.gameObject.transform.localPosition = new Vector3(0f, 1f, 0f);
        nozzleTr.localPosition = new Vector3(-145f, -80f, 0f);
        prevYpos = nozzleTr.localPosition.y;
        productImage.fillAmount = 0f;
        boosterGauge = 0f;
        imgBoosterGauge.fillAmount = boosterGauge * 0.01f;
    }

    void ProductProgressSet()  //제품 작업 진행률
    {
        productProgress += 10;
        productImage.fillAmount = ((float)productProgress * 0.01f);
        uiCtrl.ProgressCheck(productProgress);
    }
    #endregion


    #region 노즐 동작
    void NozzleMove()  //노즐 이동
    {
        switch (moveDirType)
        {
            case (MoveDirType)1: 
                moveDir = Vector3.right;
                isMoveRight = true; 
                break;
            case (MoveDirType)2: 
                moveDir = Vector3.left;
                isMoveRight = false;
                break;
            case (MoveDirType)3: 
                moveDir = Vector3.up; 
                break;
            default: 
                moveDir = Vector3.zero; 
                break;
        }
        nozzle.transform.localPosition += moveDir * Time.deltaTime * currentSpeed;
    }

    void MoveDirectionChange()  //노즐 이동 방향 변경
    {
        if (nozzleTr.localPosition.x >= maxX && nozzleTr.localPosition.y < prevYpos + yPlus)
        {
            nozzleTr.localPosition = new Vector3(maxX, nozzleTr.localPosition.y, nozzleTr.localPosition.z);
            moveDirType = MoveDirType.UP;
        }
        else if (nozzleTr.localPosition.x <= minX && nozzleTr.localPosition.y < prevYpos + yPlus)
        {
            nozzleTr.localPosition = new Vector3(minX, nozzleTr.localPosition.y, nozzleTr.localPosition.z);
            moveDirType = MoveDirType.UP;

        }

        if (nozzleTr.localPosition.y >= prevYpos + yPlus)
        {
            if (isMoveRight)
            {
                nozzleTr.localPosition = new Vector3(nozzleTr.localPosition.x, prevYpos + yPlus, nozzleTr.localPosition.z);
                prevYpos = nozzleTr.localPosition.y;
                moveDirType = MoveDirType.LEFT;
                ProductProgressSet();
            }
            else
            {
                nozzleTr.localPosition = new Vector3(nozzleTr.localPosition.x, prevYpos + yPlus, nozzleTr.localPosition.z);
                prevYpos = nozzleTr.localPosition.y;
                moveDirType = MoveDirType.RIGHT;
                ProductProgressSet();
            }
        }

        if (nozzleTr.localPosition.y >= maxY)
        {
            moveDirType = MoveDirType.STOP;
            PrinterGameManager.instance.CheckResult();  //작업 정지하고 중간 결과 처리
        }
    }

    private void OnMouseDown()  //프린터 동작 속도 증가 효과 켜기
    {
        if (isOverHeat) return;

        currentSpeed = highSpeed;
        isBooster = true;
    }

    private void OnMouseUp()  //프린터 동작 속도 증가 효과 끄기
    {
        if (isOverHeat) return;
        currentSpeed = normalSpeed;
        isBooster = false;
    }

    void BoosterOn()  //프린터 동작 속도 증가
    {
        if (isOverHeat)
        {
            currentSpeed = 0;
            if (boosterGauge <= 0)
            {
                isOverHeat = false;
                currentSpeed = normalSpeed;
            }
        }

        if(isBooster)
        {
            boosterGauge += Time.deltaTime * heatValue;
            if(boosterGauge >= boosterLimit)
            {
                isOverHeat = true;
                boosterGauge = boosterLimit;
                overHeatCount++;
                currentSpeed = 0;
                isBooster = false;
            }
        }
        else
        {
            boosterGauge -= Time.deltaTime * coolValue;
            if(boosterGauge <= 0) boosterGauge = 0;
        }
        imgBoosterGauge.fillAmount = boosterGauge * 0.01f;
    }

    #endregion


}
