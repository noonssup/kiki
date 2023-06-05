using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 우주비행사
/// 우주선 발사 모드 게임컨트롤 스크립트
/// </summary>

public class SpaceLauncherGameController : MonoBehaviour
{
    [Header("배경오브젝트/데이터")]//배경
    public GameObject groundBg;
    public GameObject[] skyBgs;
    public Vector3[] bgResetPos = new Vector3[3];
    public bool isBgMove = false;
    public float bottomYposition = -10f;
    public float topYposition = 10f;
    public float moveSpeed = 0f;
    public float maxMoveSpeed = 20f;
    public float gameTimer = 0f;
    public float flyDistance = 0f;
    public TextMeshProUGUI textGameTimer;
    public TextMeshProUGUI textFlyDistance;

    [Header("버튼UI")]//UI
    public Button[] separateButtons;
    public Button launcherButton;
    //public Button readyButton;

    [Header("우주선오브젝트/데이터")] //우주선
    public SpaceLauncherShipController shipCtrl;
    public GameObject spaceShipPrefab;//우주선프리팹
    public GameObject spaceShipObj;  //화면에 출력되는 우주선
    public float[] fuelLevel = new float[3];
    public float maxFuelLevel = 0f;
    public Image[] fuelGaugeBar;

    [Header("발사대오브젝트/데이터")]//발사대
    public Image powerGauge;
    public float maxPower = 2.5f;
    public float minPower = 0.1f;
    public float changePower = 5f;
    public float currentPower = 0f;
    public float fixPower = 0f;
    public float launchTimeCount = 0f;
    public bool isMax = false;
    public bool isMin = false;
    public bool isReady = false;



    private void Start()
    {
        SetBgPosition();
        LauncherGameStart();
    }

    private void Update()
    {
        FirePowerGaugeControl();  //파워게이지 동작
        BgMove();  //배경 움직이기
        UseFuel();  //우주선 연료 소모
    }

    #region 시작/종료 함수

    void SetBgPosition()  //배경 오브젝트 초기 위치 설정
    {
        bgResetPos[0] = groundBg.transform.localPosition;
        bgResetPos[1] = skyBgs[0].transform.localPosition;
        bgResetPos[2] = skyBgs[1].transform.localPosition;
    }

    public void RetryGame()  //배경 원위치, 기존 우주선이 있을 경우 삭제
    {
        StopAllCoroutines();
        SoundManager.instance.StopAllEffSound();
        System.GC.Collect();

        isReady = false;
        isBgMove = false;

        //게임시간 + 발사 후 이동거리
        gameTimer = 0f;
        textGameTimer.text = "00:00";
        flyDistance = 0f;
        textFlyDistance.text = "0Km";

        //배경 원위치
        if (!groundBg.activeSelf) groundBg.SetActive(true);
        groundBg.transform.localPosition = bgResetPos[0];
        skyBgs[0].transform.localPosition = bgResetPos[1];
        skyBgs[1].transform.localPosition = bgResetPos[2];
        for (int i = 3; i < skyBgs.Length; i++)
        {
            skyBgs[i].transform.position = new Vector3(Random.Range(-7f, 6), Random.Range(10f, 15f), skyBgs[i].transform.position.z);
        }

        //우주선
        if (spaceShipObj != null)
        {
            Destroy(spaceShipObj.gameObject);
            spaceShipObj = null;
        }
        //UI
        powerGauge.transform.parent.gameObject.SetActive(false);
        separateButtons[0].interactable = true;
        separateButtons[1].interactable = true;
        launcherButton.interactable = true;
        //readyButton.interactable = true;

        for (int i = 0; i < fuelLevel.Length; i++)
        {
            fuelLevel[i] = 0f;
            fuelGaugeBar[i].fillAmount = fuelLevel[i] / maxFuelLevel;
        }

        LauncherGameStart();
    }


    public void LauncherGameStart()  //발사대게임 시작
    {
        if (isReady) return;
        isReady = true;

        //연료량 초기화 = 0
        maxFuelLevel = 1f;
        for (int i=0;i< fuelLevel.Length;i++)
        {
            fuelLevel[i] = 0f;
            fuelGaugeBar[i].fillAmount = fuelLevel[i] / maxFuelLevel;
        }

        //연료량을 정할 파워게이지 활성화
        powerGauge.transform.parent.gameObject.SetActive(true);
        currentPower= minPower;
        isMin = true;

        //우주선 활성화
        SpaceShipSetup();
        //readyButton.interactable = false;
    }


    IEnumerator FinishFunction()  //게임 종료
    {
        //결과창 생성
        SoundManager.instance.StopEffSoundLoop();
        SpaceGameManager.instance.ClearLauncherMission(flyDistance);
        yield return new WaitForSeconds(5);
        //결과창 비활성화 후 우주선 이동 (우주선 이동과 동시에 페이드아웃 효과)
        SpaceGameManager.instance.resultPanel.SetActive(false);
        shipCtrl.FinishMove();
    }

    #endregion


    #region 우주선
    void SpaceShipSetup()  //우주선 셋업
    {
        spaceShipObj = Instantiate(spaceShipPrefab);
        spaceShipObj.transform.SetParent(this.transform);
        spaceShipObj.transform.localPosition = new Vector3(0f, 0.7f, 0f);
        shipCtrl = spaceShipObj.GetComponent<SpaceLauncherShipController>();
        separateButtons[0].onClick.AddListener(shipCtrl.SeparatePart1);
        separateButtons[1].onClick.AddListener(shipCtrl.SeparatePart2);
    }

    void UseFuel()   //연료 소모
    {
        if (!isBgMove) return;

        if(shipCtrl != null)
        {
            if (shipCtrl.isLaunchLevel[0] && !shipCtrl.isLaunchLevel[1])  //파츠3 연료 소모
            {
                fuelLevel[2] -= Time.deltaTime * (moveSpeed * 0.1f);
                fuelGaugeBar[2].fillAmount = fuelLevel[2] / maxFuelLevel;
            }

            if (shipCtrl.isLaunchLevel[1] && !shipCtrl.isLaunchLevel[2])  //파츠2 연료 소모
            {
                fuelLevel[1] -= Time.deltaTime * (moveSpeed * 0.1f);
                fuelGaugeBar[1].fillAmount = fuelLevel[1] / maxFuelLevel;
            }

            if (shipCtrl.isLaunchLevel[2])  //파츠1 연료 소모
            {
                fuelLevel[0] -= Time.deltaTime * (moveSpeed * 0.1f);
                fuelGaugeBar[0].fillAmount = fuelLevel[0] / maxFuelLevel;
            }
        }


        if (fuelLevel[0] < 0)
        {
            isBgMove = false;
            StartCoroutine(FinishFunction());
        }
        else if(!shipCtrl.isLaunchLevel[1] && fuelLevel[2] < 0f)  //파츠3
        {
            isBgMove = false;
            separateButtons[0].interactable= false;
            spaceShipObj.GetComponent<SpaceLauncherShipController>().FailSeparateParts();
        }
        else if(!shipCtrl.isLaunchLevel[2] && fuelLevel[1] < 0f)  //파츠2
        {
            isBgMove = false;
            separateButtons[1].interactable = false;
            spaceShipObj.GetComponent<SpaceLauncherShipController>().FailSeparateParts();
        }
    }

    #endregion


    #region 런처 

    void FirePowerGaugeControl()  //파워게이지 변경 효과
    {
        if (isMax && powerGauge.gameObject.activeSelf)
        {
            currentPower -= changePower * Time.deltaTime;
            if(currentPower < minPower )
            {
                currentPower= minPower;
                isMax = false;
                isMin= true;
            }
        }
        else if(isMin&& powerGauge.gameObject.activeSelf)
        {
            currentPower += changePower * Time.deltaTime;
            if (currentPower > maxPower)
            {
                currentPower= maxPower;
                isMin= false;
                isMax= true;
            }
        }
        powerGauge.fillAmount= currentPower / maxPower;
    }

    public void OnClickLauncherButton()  //발사 버튼 (발사)
    {
        if (!isMax && !isMin) return;

        isMin= false;
        isMax= false;
        fixPower = currentPower;
        moveSpeed = maxPower;
        maxFuelLevel = fixPower * 30f;

        //연료량 설정
        for (int i = 0; i < fuelLevel.Length; i++)
        {
            fuelLevel[i] = maxFuelLevel;
            fuelGaugeBar[i].fillAmount = fuelLevel[i] / maxFuelLevel;
        }

        launcherButton.interactable = false;
        StartCoroutine(LauncherSpaceShip());
    }

    IEnumerator LauncherSpaceShip()
    {
        //카운트 다운
        WaitForSeconds wfs = new WaitForSeconds(0.5f);
        TextMeshProUGUI text = SpaceGameManager.instance.OnStartPanel();
        text.transform.localPosition = Vector3.zero;
        text.text = "T-Minus";
        yield return wfs; 
        yield return wfs;
        SoundManager.instance.PlayEffectSound("eff_Space_count", 1f);
        for(int i = 3; i > -1; i--)
        {
            text.text = i.ToString();

            yield return wfs; //딜레이
            yield return wfs; //딜레이
        }
        text.text = "LIFTOFF";

        yield return wfs; //딜레이
        
        shipCtrl.LaunchAction();
        powerGauge.transform.parent.gameObject.SetActive(false);  //파워게이지 비활성화
        yield return wfs; //딜레이
        SpaceGameManager.instance.startPanel.SetActive(false);
    }

    #endregion

    #region 배경
    void BgMove()
    {
        if (isBgMove)
        {
            gameTimer += Time.deltaTime;
            textGameTimer.text = ((int)gameTimer / 60).ToString("00") + ":" + ((int)(gameTimer % 60)).ToString("00");

            flyDistance += fixPower * Time.deltaTime * (moveSpeed * 0.1f);
            textFlyDistance.text = flyDistance.ToString("f1") + "Km";

            //땅 배경은 오브젝트가 활성화되어 있는 경우만 스크롤 처리
            if (groundBg.activeSelf)
            {
                groundBg.transform.position += new Vector3(0, (-moveSpeed) * Time.deltaTime, 0);
            }
            else
            {
                if (moveSpeed < maxMoveSpeed)
                {
                    launchTimeCount += Time.deltaTime;
                    if (launchTimeCount > 0.1f)
                    {
                        moveSpeed += 2f;
                        launchTimeCount = 0f;
                    }
                }
            }

            if (groundBg.transform.position.y < -15f)
            {
                groundBg.SetActive(false);
            }

            //완성 이미지 배경 적용 (2023.03.24)
            //하늘 배경은 제자리 그대로 (나중에 구름 데코 같은 게 나오면 그걸로 속도감 표현 처리)
            //구름으로 속도감을 살리고,
            //연료가 모두 소진된 후 우주로 날아가는 장면에서 우주와 하늘배경을 겹쳐서 우주로 날아가는 듯한 연출 표현

            if (gameTimer > 20)
            {
                skyBgs[0].transform.position += new Vector3(0, (-2f) * Time.deltaTime, 0);
                skyBgs[1].transform.position += new Vector3(0, (-2f) * Time.deltaTime, 0);
                skyBgs[2].transform.position += new Vector3(0, (-2f) * Time.deltaTime, 0);
            }


            //구름 이동
            skyBgs[3].transform.position += new Vector3(0, (-moveSpeed * 0.5f) * Time.deltaTime, 0);
            skyBgs[4].transform.position += new Vector3(0, (-moveSpeed * 0.5f) * Time.deltaTime, 0);
            skyBgs[5].transform.position += new Vector3(0, (-moveSpeed * 0.5f) * Time.deltaTime, 0);

            for (int i = 3; i < skyBgs.Length; i++)
            {
                if (skyBgs[i].transform.position.y < -8f)
                {
                    skyBgs[i].transform.position = new Vector3(Random.Range(-7f, 6), Random.Range(10f, 15f), skyBgs[i].transform.position.z);
                }
            }


            // 아래는 이미지 작업 전 임시 이미지로 작업한 내용

            ////하늘/우주 배경은 무한 스크롤 (2023.03.09 기준 임시 이미지로 추후 변경 필요)
            //skyBgs[0].transform.position += new Vector3(0, (-moveSpeed) * Time.deltaTime, 0);
            //skyBgs[1].transform.position += new Vector3(0, (-moveSpeed) * Time.deltaTime, 0);

            //if (skyBgs[0].transform.position.y < bottomYposition)
            //{
            //    skyBgs[0].transform.position = new Vector3(0f, topYposition, 0f);
            //}

            //if (skyBgs[1].transform.position.y < bottomYposition)
            //{
            //    skyBgs[1].transform.position = new Vector3(0f, topYposition, 0f);
            //}
        }
    }

    #endregion

}
