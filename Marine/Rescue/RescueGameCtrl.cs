using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Xml.Linq;

public class RescueGameCtrl : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timerText;  //게임시간을 표시할 텍스트
    public float timer;     //게임 플레이 시간
    public int lifeCount;   //라이프카운트
    public Image[] lifeImages; //라이프 이미지
    public int rescueCount;    //구조 카운트, 각 단계마다 카운트
    public int readyCount=3;     //게임시작전 카운트다운
    public bool isGameOver;
    public bool isGameClear;
    public int gameLevel;      //게임의 단계 (0~3단계)

    [Header("이벤트콜라이더")]
    public GameObject[] eventCollider2Ds; //1~3단계 이벤트트리거용 콜라이더 (1단계 1개, 2단계 2개, 3단계 1개 / 총 4개)
    public GameObject[] gameCollider2Ds;  //게임 중 활성/비활성화할 콜라이더

    [Header("결과창")]
    public GameObject resultPanel;
    public GameObject startObject;
    public GameObject failObject;
    public GameObject clearObject;
    public TextMeshProUGUI resultText;   //게임시작 카운트 및 시작, 성공, 실패 문자 출력용
    public TMP_Text readyCountText;
    public float resultText0offsetY;
    public Button[] resultButton;            //결과창 출력 시 활성화될 버튼 (클릭 시 미니게임팝업창 비활성화)
    public Image countRoundImage;        //게임 개시 전 카운트를 할때 움직일 동그라미 이미지
    public Image countLineImage;         //동그라미이미지의테두리
    public GameObject buttonCover;       //아이템슬롯 사용중지를 위한 커버이미지 (아이템 사용 시 활성화하여 이벤트 종료 후 사용할 수 있게 비활성화 처리)
    public GameObject[] gamePopupWindows;  //1~3단계 (총 3개)
    public GameObject alertIamge;
    //결과 성공 이미지
    public GameObject levelSuccessPanel;
    public GameObject[] levelGroups;
    public Image[] successRoundImages;
    public Image[] checkImages;
    public TMP_Text[] levelTexts;
    public Image[] lineImages;

    [Header("1단계 수상응급조치")]
    public Button[] selectPoints;  //익수자의 몸, 선택포인트 버튼
    public Button[] itemButtons;   //아이템버튼 (척추 고정대, 레스큐 튜브, 고정벨트)
    public TMP_Text bottomGuideText;   //게임창 하단 가이드 텍스트
    public GameObject rescueWorker;  //구조대원
    public GameObject helpMan; //익수자 게임오브젝트
    public Image[] helpItemImages;   //구조용품 (고정대, 튜브, 벨트)

    WaitForSeconds wfs1sec = new WaitForSeconds(1f);

    public bool isPlayerMove = false;
    public Sprite[] hearts;

    private int saveLifeCount;

    private void Awake()
    {
        /*bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Marine_bgm");
        bgmPlayer.clip = bgmSound;
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = 0.15f;
        sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Marine_use");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.playOnAwake = false;
        sfxPlayer.loop = false;
        sfxPlayer.volume = 0.5f;*/
    }

    private void Start()
    {
        SoundManager.instance.PlayBGMSound("bg_Jellyfish", 1f);
    }

    private void OnEnable()
    {
        EventColliderSetting(0);  //이벤트 콜라이더 세팅
        TimerSetting();  //타이머 세팅
        ResultPanelSetting(); //결과창 세팅
        GameValueSetting(false);   //게임내 변수값 세팅
        GamePopupWindowsSetting(9); //게임팝업창 세팅
        Level1GameSetting();   //0단계 게임 시작 시 1단계 콜라이더 세팅
        StartCoroutine(GameStart());  //익수자 구조 게임 시작
    }

    void Level1GameSetting()   //0단계 게임 시작 시 1단계 콜라이더 세팅
    {
        isPlayerMove = false;
        for (int i = 0; i < selectPoints.Length; i++)
        {
            selectPoints[i].gameObject.SetActive(true);
        }
        for (int i = 0; i < helpItemImages.Length; i++)
        {
            helpItemImages[i].GetComponent<CircleCollider2D>().enabled = true;
        }
        for (int i = 0; i < itemButtons.Length; i++)
        {
            itemButtons[i].gameObject.SetActive(true);
        }
    }

    public void EventColliderSetting(int _collNumber)  //이벤트 발생용 콜라이더 비활성화 (인자로 받은 콜라이더만 활성화)
    {
        //익수자 방향으로 이동 또는 익수자 구출 이동 시의 이벤트 콜라이더
        for(int i = 0; i < eventCollider2Ds.Length; i++)
        {
            eventCollider2Ds[i].SetActive(false);
        }
        if (_collNumber == 9) return;  //인자를 9로 전달할 경우 모든 콜라이더 끄기
        else eventCollider2Ds[_collNumber].SetActive(false);

        //1단계 익수자 구조 파트의 콜라이더 설정 (재시작 시 꺼져있는 상태의 콜라이더 활성화 시키기)

    }

    void TimerSetting()  //타이머 세팅
    {
        timer = 0f;
        int _min = (int)timer / 60;
        int _sec = (int)timer % 60;
        timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);
    }

    void ResultPanelSetting()  //결과창 시작 설정
    {
        resultPanel.SetActive(true);
        resultText0offsetY = 0.428f;

        startObject.SetActive(false);
        failObject.SetActive(false);
        clearObject.SetActive(false);
        resultText.text = "";
        resultButton[0].gameObject.SetActive(false);
        resultButton[1].gameObject.SetActive(false);
        levelSuccessPanel.SetActive(false);
    }

    void GameValueSetting(bool isSave)  //게임내 변수값 세팅
    {
        rescueCount = 0;
        gameLevel = 0;
        if (isSave) {
            lifeCount = saveLifeCount;
        } else
        {
            lifeCount = 3;
        }
        alertIamge.SetActive(false);
        //helpMan.SetActive(true);
        //rescueWorker.SetActive(true);
        //rescueWorker.GetComponent<RectTransform>().localPosition = new Vector3(260, 135f, 0f);
        for (int i = 0; i < lifeImages.Length; i++)  //라이프 이미지
        {
            lifeImages[i].sprite = hearts[0];
        }
        Color _itemColor = new Color(1f, 1f, 1f, 0f);
        for (int i = 0; i < helpItemImages.Length; i++)  //구조용품 이미지 투명처리
        {
            helpItemImages[i].color = _itemColor;
        }
        rescueCount = 0;    //구조 카운트, 각 단계마다 카운트
        isGameOver = false;
        isGameClear = false;

    }

    public void GamePopupWindowsSetting(int _windowsNumber)  //게임팝업창 세팅
    {
        for (int i = 0; i < gamePopupWindows.Length; i++)
        {
            gamePopupWindows[i].SetActive(false);
        }
        if (_windowsNumber == 9) return;  //인자를 9로 전달할 경우 모든 콜라이더 끄기
        else
        { 
            gamePopupWindows[_windowsNumber].SetActive(true);
            isPlayerMove = false; rescueCount = 0; 
        }
    }


    IEnumerator GameStart()  
        //게임 시작 함수  (게임팝업창은 비활성화, 해변에 구조자와 익수자가 있는 상태 / 구조자 이동으로 1~3단계 게임 실행)
    {
        //float _startCount = 1f;
        //float _roundImageCount = 1f;
        //countRoundImage.fillAmount = 1f;
        //countRoundImage.transform.position = resultText[0].transform.position;


        lineImages[0].enabled = true;
        lineImages[2].enabled = true;
        lineImages[1].enabled = false;
        lineImages[3].enabled = false;

        countLineImage.enabled = false;
        countRoundImage.enabled = false;
        readyCountText.text = "";
        resultButton[0].gameObject.SetActive(false);
        resultButton[1].gameObject.SetActive(false);

        //bgmPlayer.Play();
        //resultText[0].text = "START";
        //게임시작 전 대기 상태 구성
        //while (_startCount > 0)
        //{
        //    //resultText[0].text = Mathf.Ceil(_startCount).ToString("0");   //게임 시작 카운트 텍스트 출력 + 올림 처리
        //    yield return new WaitForSeconds(Time.deltaTime);
        //    //_roundImageCount -= Time.deltaTime / 3f;
        //    //countRoundImage.fillAmount = _roundImageCount;  //동그라미 이미지 radial360 처리
        //    _startCount -= Time.deltaTime;
        //}
        Level1GameStart();
        yield return wfs1sec;
        isPlayerMove = true;
        //resultPanel.SetActive(false);

        //게임시작
        while (!isGameOver && !isGameClear)
        {
            yield return wfs1sec;
            timer += 1f;// Time.deltaTime;
            int _min = (int)timer / 60;
            int _sec = (int)timer % 60;
            timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);

        }

        //종료 사운드재생 후 1초 대기
        yield return wfs1sec;
    }



    public void Level1GameStart()    //플레이어가 익수자와 충돌하면 실행
    {
        gamePopupWindows[0].SetActive(true);
        StartCoroutine(GameReadyFunction(1));
    }

    public IEnumerator GameReadyFunction(int _level)   //단계별 구조활동 시작 준비 (단계별 게임팝업창 활성화, 대기효과연출)
    {
        startObject.SetActive(false);
        failObject.SetActive(false);
        clearObject.SetActive(false);
        resultText.text = "";
        if (!resultPanel.activeSelf) resultPanel.SetActive(true);
        
        float _startCount = readyCount;
        float _roundImageCount = 1f;
        countRoundImage.fillAmount = _roundImageCount;
        countLineImage.enabled = true;
        countRoundImage.enabled = true;

        levelSuccessPanel.SetActive(true);
        levelGroups[0].SetActive(true); levelGroups[1].SetActive(true); levelGroups[2].SetActive(true);

        switch (_level)
        {
            case 1: resultText.text = "수상 응급조치";
                lineImages[0].enabled = true;
                lineImages[2].enabled = true;
                lineImages[1].enabled = false; 
                lineImages[3].enabled = false;
                successRoundImages[0].enabled = false;
                successRoundImages[1].enabled = false;
                successRoundImages[2].enabled = false;
                checkImages[0].enabled = false;
                checkImages[1].enabled = false;
                checkImages[2].enabled = false;
                break;
            case 2: resultText.text = "인공호흡";
                lineImages[0].enabled = true;
                lineImages[2].enabled = true;
                lineImages[1].enabled = false; 
                lineImages[3].enabled = false;
                successRoundImages[0].enabled = true;
                successRoundImages[1].enabled = false;
                successRoundImages[2].enabled = false;
                checkImages[0].enabled = true;
                checkImages[0].GetComponent<Animator>().enabled = false;
                checkImages[1].enabled = false;
                checkImages[2].enabled = false;
                break;
            case 3: resultText.text = "AED 응급조치";
                lineImages[0].enabled = true;
                lineImages[2].enabled = true;
                lineImages[1].enabled = true; 
                lineImages[1].GetComponent<Animator>().enabled = false; 
                lineImages[3].enabled = false;
                successRoundImages[0].enabled = true;
                successRoundImages[1].enabled = true;
                successRoundImages[2].enabled = false;
                checkImages[0].enabled = true;
                checkImages[0].GetComponent<Animator>().enabled = false;
                checkImages[1].enabled = true;
                checkImages[1].GetComponent<Animator>().enabled = false;
                checkImages[2].enabled = false;
                break;
            case 4: resultText.text = "CPR 응급조치";
                lineImages[0].enabled = true;
                lineImages[2].enabled = true;
                lineImages[1].enabled = true;
                lineImages[1].GetComponent<Animator>().enabled = false;
                lineImages[3].enabled = false;
                successRoundImages[0].enabled = true;
                successRoundImages[1].enabled = true;
                successRoundImages[2].enabled = false;
                checkImages[0].enabled = true;
                checkImages[0].GetComponent<Animator>().enabled = false;
                checkImages[1].enabled = true;
                checkImages[1].GetComponent<Animator>().enabled = false;
                checkImages[2].enabled = false;
                break;
        }


        //게임시작 전 대기 상태 구성
        while (_startCount > 0)
        {
            readyCountText.text = Mathf.Ceil(_startCount).ToString("0");   //게임 시작 카운트 텍스트 출력 + 올림 처리
            yield return new WaitForSeconds(Time.deltaTime);
            _roundImageCount -= Time.deltaTime / 1.5f;// / 3f;
            countRoundImage.fillAmount = _roundImageCount;  //동그라미 이미지 radial360 처리
            _startCount -= Time.deltaTime*2f;
        }
        countLineImage.enabled = false;
        readyCountText.text = "";
        isPlayerMove = true;
        startObject.SetActive(true);
        failObject.SetActive(false);
        clearObject.SetActive(false);
        resultText.text = "";
        yield return wfs1sec;
        resultPanel.SetActive(false);
    }

    public IEnumerator RoundSuccessFunction(string _comment, int _clearLevel)  //익수자 구조 단계별 성공 시 실행
    {
        yield return new WaitForFixedUpdate();
        /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_next");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.Play();*/
        SoundManager.instance.PlayEffectSound("eff_Common_next", 1f);
        resultPanel.SetActive(true);
        //resultText0offsetY = 1.14f;
        //resultText[0].transform.position = new Vector3(0f, resultText0offsetY, 0f);
        //Color _textColor0 = new Color32(245, 160, 50, 255);
        //resultText[0].color = _textColor0;

        startObject.SetActive(false);
        failObject.SetActive(false);
        clearObject.SetActive(false);
        resultText.text = _comment;
        resultButton[0].gameObject.SetActive(false);
        resultButton[1].gameObject.SetActive(false);

        levelSuccessPanel.SetActive(true);
        levelGroups[0].SetActive(true); levelGroups[1].SetActive(true); levelGroups[2].SetActive(true); 
        lineImages[0].gameObject.SetActive(true); lineImages[1].gameObject.SetActive(true);


        switch (_clearLevel)   //성공한 단계에 따라 단계 성공 이미지 연출 변경
        {
            case 1: // 수상 응급조치 성공
                lineImages[1].GetComponent<Image>().enabled = false;
                lineImages[3].GetComponent<Image>().enabled = false;
                resultText.text = _comment;
                successRoundImages[0].enabled = true;
                successRoundImages[1].enabled = false;
                successRoundImages[2].enabled = false;
                levelTexts[0].color = new Color(46f, 148f, 45f, 255f);


                checkImages[0].GetComponent<Image>().enabled = true;
                yield return checkImages[0].GetComponent<Image>().fillAmount < 1;
                yield return wfs1sec;


                //yield return wfs1sec;
                break;  
            case 2:   // 인공호흡 조치 성공
                lineImages[1].GetComponent<Image>().enabled = false;
                lineImages[3].GetComponent<Image>().enabled = false;
                successRoundImages[0].enabled = true;
                successRoundImages[1].enabled = true;
                successRoundImages[2].enabled = false;

                checkImages[0].GetComponent<Animator>().enabled = false;
                checkImages[0].fillAmount = 1f;
                checkImages[0].enabled = true;


                levelTexts[0].color = new Color(46f, 148f, 45f, 255f);
                resultText.text = _comment;

                checkImages[1].GetComponent<Image>().enabled = true;
                yield return checkImages[1].GetComponent<Image>().fillAmount < 1;

                lineImages[1].GetComponent<Image>().enabled = true;
                lineImages[3].GetComponent<Image>().enabled = false;
                yield return lineImages[1].GetComponent<Image>().fillAmount < 1;
                yield return wfs1sec;
                break;
            case 3:  // AED 응급 조치 성공
                
                break;
            case 4:  // CPR 응급 조치 성공
                lineImages[1].GetComponent<Animator>().enabled = false;
                lineImages[1].GetComponent<Image>().fillAmount = 1f;
                lineImages[1].GetComponent<Image>().enabled = true;
                lineImages[3].GetComponent<Image>().enabled = false;
                successRoundImages[0].enabled = true;
                successRoundImages[1].enabled = true;
                successRoundImages[2].enabled = true;

                checkImages[0].GetComponent<Animator>().enabled = false;
                checkImages[0].fillAmount = 1f;
                checkImages[0].enabled = true;
                checkImages[1].GetComponent<Animator>().enabled = false;
                checkImages[1].fillAmount = 1f;
                checkImages[1].enabled = true;


                levelTexts[0].color = new Color(46f, 148f, 45f, 255f);
                resultText.text = _comment;
                checkImages[2].GetComponent<Image>().enabled = true;
                yield return checkImages[2].GetComponent<Image>().fillAmount < 1;

                lineImages[3].GetComponent<Image>().enabled = true;
                yield return lineImages[3].GetComponent<Image>().fillAmount < 1;
                yield return wfs1sec;
                break;

        }





        yield return new WaitForSeconds(2f);
        //resultPanel.SetActive(false);

        if (_comment == "CPR 응급 조치 성공")
        {
            StartCoroutine(NextLevel(4));
        }
        else
        {
            //resultPanel.SetActive(false);
            yield break; 
        }


    }


    public void CheckCorrect(bool _isCorrect, string _itemName, Transform _collTr)  
    {
        switch (_isCorrect)
        {
            case true:
                /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Marine_use");
                sfxPlayer.clip = sfxSound;
                sfxPlayer.Play();*/
                SoundManager.instance.PlayEffectSound("eff_Marine_use", 1f);
                rescueCount++;
                switch (_itemName)
                {
                    case "item_001": selectPoints[6].gameObject.SetActive(false); break;
                    case "item_002": selectPoints[0].gameObject.SetActive(false); break;
                    case "item_015": if(_collTr.name == "BeltTopImage") selectPoints[1].gameObject.SetActive(false);
                    else if (_collTr.name == "BeltBottomImage") selectPoints[2].gameObject.SetActive(false);
                        break;
                }
                if(rescueCount == 4)
                {
                    StartCoroutine(NextLevel(2));
                    EventColliderSetting(1);
                }
                break;
            case false:
                /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_wrong");
                sfxPlayer.clip = sfxSound;
                sfxPlayer.Play();*/
                SoundManager.instance.PlayEffectSound("eff_Jellyfish_wrong", 1f);
                lifeCount--;
                lifeImages[lifeCount].sprite = hearts[1];
                CameraShakeEffect();
                if (lifeCount <= 0)
                {
                    isGameClear = false;
                    StartCoroutine(FinishGameFunction());
                }
                break;
        }
    }

    string prevItemName = null;
    public void CheckCorrect(string _itemName)  //벨트 정답 여부
    {
        Debug.Log("버튼 이름 : " + _itemName);
        if (rescueCount ==2)
        {
            if(_itemName == "PointButton2" || _itemName == "PointButton3")
            {
                if (_itemName == "PointButton2") selectPoints[1].gameObject.SetActive(false);
                else if (_itemName == "PointButton3") selectPoints[2].gameObject.SetActive(false);
                CheckCorrect(true, _itemName, null);
                prevItemName = _itemName;
            }
            else CheckCorrect(false, _itemName, null);
        }
        else if(rescueCount == 3)
        {
            if (_itemName != prevItemName)
            {
                if (_itemName == "PointButton2" || _itemName == "PointButton3")
                {
                    CheckCorrect(true, _itemName, null);
                    Color _color = helpItemImages[2].color;
                    _color.a = 1f;
                    helpItemImages[2].color = _color;



                    //인공호흡 미니게임으로 설정
                    //0. 1~3초의 간격을 두고,
                    //1. 게임팝업창 닫기(Level1Panel 비활성화)
                    //2. 2단계 이벤트 콜라이더 켜기
                    //3. 구조자/익수자의 위치는 1단계 이벤트 콜라이더의 위치에서 시작
                    StartCoroutine(NextLevel(2));
                    EventColliderSetting(1);


                }
                else CheckCorrect(false, _itemName, null);
            }
        }
        else CheckCorrect(false, _itemName, null);
    }

    public void GoToNextLevel(int _level, bool _isCorrect)   //2단계에서 게임 완료 시 호출
    {
        if (_isCorrect == true)
        {

           StartCoroutine(NextLevel(_level));
        }
        else
        {
            isGameOver = true;
            isGameClear = false;
            StartCoroutine(FinishGameFunction());
        }
    }



    void ChangeGameLevel(int _gameLevel) //진행할 게임 단계의 변경 (단계별 이벤트콜라이더와 충돌할 때 실행
    {
        gameLevel = _gameLevel;
        rescueCount = 0;
        //EventColliderSetting(9);
        GamePopupWindowsSetting(_gameLevel - 1);
        //rescueWorker.SetActive(false);
       //helpMan.SetActive(false);
    }


    IEnumerator NextLevel(int _level)
    {
        yield return wfs1sec;
        switch (_level)
        {
            case 2:
                //동그라미 버튼 오브젝트 6개 비활성화
                for(int i = 0; i < selectPoints.Length; i++)
                {
                    selectPoints[i].gameObject.SetActive(false);
                }

                yield return new WaitForFixedUpdate();
                yield return StartCoroutine(RoundSuccessFunction("수상 응급조치 성공", 1));
                gamePopupWindows[0].SetActive(false);
                //rescueWorker.SetActive(true);
                //isPlayerMove = true;
                for(int i=0;i<eventCollider2Ds.Length;i++) eventCollider2Ds[i].SetActive(false);
                //이후 플레이어가 2단계 콜라이더에 닿으면 2단계 게임 시작
                saveLifeCount = lifeCount;
                ChangeGameLevel(2);
                break;
            case 3:
                yield return new WaitForFixedUpdate();
                yield return StartCoroutine(RoundSuccessFunction("인공호흡 성공", 2));
                gamePopupWindows[1].SetActive(false);
                //rescueWorker.SetActive(true);
                //isPlayerMove = true;
                for (int i = 0; i < eventCollider2Ds.Length; i++) eventCollider2Ds[i].SetActive(false);
                saveLifeCount = lifeCount;
                ChangeGameLevel(3);
                break;
            case 4:
                isGameClear = true;
                StartCoroutine(FinishGameFunction());
                break;
        }
        yield break;
    }

    IEnumerator FinishGameFunction()   //게임종료 함수 (게임오버 / 게임클리어)
    {

        gamePopupWindows[2].GetComponent<RescueLevel3Ctrl>().isMove = false;
        gamePopupWindows[1].GetComponent<RescueLevel2Ctrl>().isMove = false;
        yield return wfs1sec;

        resultPanel.SetActive(true); //결과창 활성화

        levelSuccessPanel.SetActive(false);
        levelGroups[0].SetActive(false); levelGroups[1].SetActive(false); levelGroups[2].SetActive(false);
        lineImages[0].enabled = false;
        lineImages[2].enabled = false;
        lineImages[1].GetComponent<Animator>().enabled = true;
        lineImages[1].enabled = false;
        lineImages[3].GetComponent<Animator>().enabled = true;
        lineImages[3].enabled = false;
        checkImages[0].GetComponent<Animator>().enabled = true;
        checkImages[0].enabled = false;
        checkImages[1].GetComponent<Animator>().enabled = true;
        checkImages[1].enabled = false;
        checkImages[2].GetComponent<Animator>().enabled = true;
        checkImages[2].enabled = false;
        //lineImages[0].gameObject.SetActive(false); lineImages[1].gameObject.SetActive(false);
        //lineImages[2].gameObject.SetActive(false); lineImages[3].gameObject.SetActive(false);
        resultText.text = "";



        for (int i = 0; i < resultPanel.transform.childCount; i++)
        {
            resultPanel.transform.GetChild(i).gameObject.SetActive(false);
        }
        //lineImages[0].enabled = false;

        yield return new WaitForFixedUpdate();

        if (isGameClear)
        {
            /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_clear");
            sfxPlayer.clip = sfxSound;
            sfxPlayer.Play();*/
            SoundManager.instance.PlayEffectSound("eff_Common_clear", 1f);
            //게임클리어 
            for (int i = 0; i < resultPanel.transform.childCount; i++)
            {
                resultPanel.transform.GetChild(i).gameObject.SetActive(true);
            }
            startObject.SetActive(false);
            failObject.SetActive(false);
            clearObject.SetActive(true);
            //Color _textColor1 = new Color32(255, 234, 89, 255);
            //resultText[1].color = _textColor1;
            //resultText[1].fontSize = 40;
            //resultText[1].transform.localPosition = new Vector3(0f, -25f, 0f);
            //resultText[1].text = "익수자 구조 성공!";
            resultButton[0].gameObject.SetActive(true);
            resultButton[1].gameObject.SetActive(true);
            StartCoroutine(CloseMiniGamePopup("RescueGamePanel"));
        }
        else
        {
            /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_fail");
            sfxPlayer.clip = sfxSound;
            sfxPlayer.Play();*/
            SoundManager.instance.PlayEffectSound("eff_Common_fail", 1f);
            //게임오버
            for (int i = 0; i < resultPanel.transform.childCount; i++)
            {
                resultPanel.transform.GetChild(i).gameObject.SetActive(true);
            }
            startObject.SetActive(false);
            failObject.SetActive(true);
            clearObject.SetActive(false);
            //resultText[1].color = new Color32(255, 112, 112, 255);
            //resultText[1].fontSize = 40;
            //resultText[1].transform.localPosition = new Vector3(0f, -25f, 0f);
            //resultText[1].text = "익수자 구조 실패...";
            resultButton[0].gameObject.SetActive(true);
            resultButton[1].gameObject.SetActive(true);
            StartCoroutine(CloseMiniGamePopup("RescueGamePanel"));
        }
    }

    IEnumerator CloseMiniGamePopup(string _contentName)  //결과창 활성화 후 5초뒤 창닫기
    {
        yield return new WaitForSeconds(5f);
        //sceneManager.CloseMiniGamePopUp(_contentName);
    }


    //오답일 시 화면 흔들리는 연출
    void CameraShakeEffect()
    {
        alertIamge.SetActive(true);
        float shakeTime = 1.2f;
        float shakeAmount = 0.1f;
        //Camera _camera = Camera.main;
        StartCoroutine(ShakeEffectFunction(shakeTime, shakeAmount));
    }

    IEnumerator ShakeEffectFunction(float shakeTime, float shakeAmount)
    {
        GameObject gameWindow = null;
        Vector3 initialPosition = new Vector3(0f,-1f,0f); 
        for(int i=0;i< gamePopupWindows.Length; i++)
        {

            if (gamePopupWindows[i].activeSelf)
            {
                gameWindow = gamePopupWindows[i].transform.GetChild(0).gameObject;
            }
            
        }
        initialPosition = gameWindow.transform.position;

        yield return new WaitForFixedUpdate();
        while (shakeTime > 0)
        {
            yield return new WaitForFixedUpdate();
            gameWindow.transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
            shakeTime -= Time.deltaTime;
        }
        alertIamge.SetActive(false);
        shakeTime = 0f;
        gameWindow.transform.position = initialPosition;
    }


    public void OnExitButtonClick()
    {
        SoundManager.instance.StopBGMSound();
        StartCoroutine(ChangeSceneToLobby());
    }

    IEnumerator ChangeSceneToLobby()
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>("Utils/ChangeSceneCanvas"));
        yield return gameObject.GetComponent<ChangeSceneManager>().FadeOut(1f);
        GlobalData.nextScene = "LobbyScene";
        SceneManager.LoadScene("LoadScene");
    }

    public void OnRetryButtonClick()
    {
        int saveGameLevel = gameLevel;
        EventColliderSetting(0);  //이벤트 콜라이더 세팅
        TimerSetting();  //타이머 세팅
        ResultPanelSetting(); //결과창 세팅
        GameValueSetting(true);   //게임내 변수값 세팅
        GamePopupWindowsSetting(9); //게임팝업창 세팅
        Level1GameSetting();   //0단계 게임 시작 시 1단계 콜라이더 세팅
        //StartCoroutine(GameStart());  //익수자 구조 게임 시작
        ChangeGameLevel(saveGameLevel);
    }
}
