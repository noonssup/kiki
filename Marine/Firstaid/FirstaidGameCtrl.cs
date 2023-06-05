using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class FirstaidGameCtrl : MonoBehaviour
{
    public int gameLevel;
    
    [Header("해파리응급처치 UI")]
    //왼쪽 상단 UI
    public TMP_Text timerText;
    public TMP_Text levelText;
    public Image[] lifeImages;
    public Image portraitImage;  //초상화
    public float timer;
    public int lifeCount;
    public GameObject alertIamge;

    //하단 아이템UI 패널
    public GameObject[] itemSlots;

    [Header("해파리응급처치 오브젝트")]
    public Image injuryImage;
    public GameObject lv1injuryPart;  //1단계 상처부위 콜라이더
    public GameObject[] lv2InjuryParts; //2단계 상처부위 콜라이더
    public Image[] jellyfishImages;     //2단계 해파리 이미지 오브젝트
    public GameObject lv3injuryPart;  //1단계 상처부위 콜라이더
    public Image lv3CreamImage; //3단계 연고 이미지
    public GameObject healEffect;
    public GameObject healBg;    //힐효과의 부모오브젝트

    [Header("해파리응급처치 변수")]
    public int curePoint;   //상처 치료 카운트
    public int correctCount;//정답 개수
    public bool isGameOver;
    public bool isGameClear;
    public int readyCount = 3;
    WaitForSeconds wait1Sec = new WaitForSeconds(1f);

    [Header("결과창")]
    public GameObject resultPanel;
    public GameObject startObject;
    public GameObject failObject;
    public GameObject clearObject;
    public TextMeshProUGUI resultText;   //게임시작 카운트 및 시작, 성공, 실패 문자 출력용
    public TMP_Text readyCountText;
    public Button[] resultButton;            //결과창 출력 시 활성화될 버튼 (클릭 시 미니게임팝업창 비활성화)
    public Image countRoundImage;        //게임 개시 전 카운트를 할때 움직일 동그라미 이미지
    public Image countLineImage;         //동그라미이미지의테두리
    public GameObject buttonCover;       //아이템슬롯 사용중지를 위한 커버이미지 (아이템 사용 시 활성화하여 이벤트 종료 후 사용할 수 있게 비활성화 처리)
    public GameObject gameWindow;        //게임화면 내부 윈도우 (GamePanel/bg)

    //결과 성공 이미지
    public GameObject levelSuccessPanel;
    public GameObject[] levelGroups;
    public Image[] successRoundImages;
    public Image[] checkImages;
    public TMP_Text[] levelTexts;
    public Image[] lineImages;

    [Header("게임 오브젝트")]
    public Sprite[] objects;
    public Sprite[] characters;
    public Sprite[] hearts;

    private void Awake()
    {
        /*bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_bgm");
        bgmPlayer.clip = bgmSound;
        bgmPlayer.playOnAwake = false;
        bgmPlayer.loop = true;
        bgmPlayer.volume = 0.15f;
        sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_clear");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.playOnAwake = false;
        sfxPlayer.loop = false;
        sfxPlayer.volume = 0.5f;*/
    }

    private void Start()
    {
        SoundManager.instance.PlayBGMSound("bg_Marine", 1f);
    }

    private void OnEnable()
    {
        InitStatus();   //초기 설정

        StartCoroutine(Level1GamePlay());
        //StartCoroutine(Level2GamePlay());
    }

    /*void SfxSoundPlay(string _soundName)
    {
        sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/"+ _soundName);
        sfxPlayer.clip = sfxSound;
        sfxPlayer.Play();
        
    }*/

    void InitStatus() //게임 시작 시 정보 초기화
    {
        alertIamge.SetActive(false);
        //Resources.Load<Sprite>("gMiniGame/Marine/Obj/object_02")
        injuryImage.sprite = objects[0];

        //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_01")
        portraitImage.sprite = characters[0];
        gameLevel = 1;
        lifeCount = 3;
        correctCount = 0;
        buttonCover.SetActive(false);

        //resultPanel 구성
        resultPanel.SetActive(true);
        startObject.SetActive(false);
        failObject.SetActive(false);
        clearObject.SetActive(false);
        levelSuccessPanel.SetActive(true);
        resultButton[0].gameObject.SetActive(false);
        resultButton[1].gameObject.SetActive(false);
        levelGroups[0].SetActive(true); levelGroups[1].SetActive(true); levelGroups[2].SetActive(true);
        lineImages[1].GetComponent<Animator>().enabled = true; 
        lineImages[1].enabled = false;
        lineImages[3].GetComponent<Animator>().enabled = true;
        lineImages[3].enabled = false;

        checkImages[0].GetComponent<Animator>().enabled = true;
        checkImages[1].GetComponent<Animator>().enabled = true;
        checkImages[2].GetComponent<Animator>().enabled = true;

        successRoundImages[0].enabled = false;
        successRoundImages[1].enabled = false;
        successRoundImages[2].enabled = false;
        checkImages[0].enabled = false;
        checkImages[1].enabled = false;
        checkImages[2].enabled = false;
        resultText.text = "응급처치";
        //levelGroups[0].SetActive(false);levelGroups[1].SetActive(false);levelGroups[2].SetActive(false); lineImages[0].gameObject.SetActive(false); ;lineImages[1].gameObject.SetActive(false);

        for (int i = 0; i < itemSlots.Length; i++)  //아이템슬롯 오브젝트에 게임레벨을 1로 설정
        {
            itemSlots[i].SetActive(true);
            itemSlots[i].GetComponent<ItemSlot>().gameLevel = 1;
            itemSlots[i].GetComponent<ItemSlot>().SetItemInfor();
        }

        //상처부위 콜라이더 설정
        lv1injuryPart.SetActive(true);
        for (int i = 0; i < lv2InjuryParts.Length; i++)  //2단계 해파리촉수/독침 오브젝트 비활성화
        {
            lv2InjuryParts[i].SetActive(false);
            jellyfishImages[i].gameObject.SetActive(false);
        }
        lv3injuryPart.SetActive(false);

        Color _color = lv3CreamImage.color;
        _color.a = 0f;
        lv3CreamImage.color = _color;

        //타이머설정
        timer = 0f;
        int _min = (int)timer / 60;
        int _sec = (int)timer % 60;
        timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);

        for (int i = 0; i < lifeCount; i++)  //생명력 이미지
        {
            //Resources.Load<Sprite>("gMiniGame/Marine/UI/img_job_marine_heart_01")
            lifeImages[i].sprite = hearts[0];
        }

        isGameOver = false;
        isGameClear = false;
    }

    IEnumerator Level1GamePlay()
    {
        levelText.text = "1단계";
        lv1injuryPart.SetActive(true);
        float _startCount = readyCount;
        float _roundImageCount = 1f;
        countRoundImage.fillAmount = _roundImageCount;
        //countRoundImage.transform.position = resultText[0].transform.position;
        //countLineImage.transform.position = countRoundImage.transform.position;
        countLineImage.enabled = true;
        //게임시작 전 대기 상태 구성
        while (_startCount > 0)
        {
            readyCountText.text = Mathf.Ceil(_startCount).ToString("0");   //게임 시작 카운트 텍스트 출력 + 올림 처리
            yield return new WaitForSeconds(Time.deltaTime);
            _roundImageCount -= Time.deltaTime / 1.5f;
            countRoundImage.fillAmount = _roundImageCount;  //동그라미 이미지 radial360 처리
            _startCount -= Time.deltaTime * 2f;
        }
        readyCountText.text = "";
        countLineImage.enabled = false;
        resultText.text = "";
        startObject.SetActive(true);
        failObject.SetActive(false);
        clearObject.SetActive(false);

        //bgmPlayer.Play();

        yield return wait1Sec;
        resultPanel.SetActive(false);

        //게임시작
        while (!isGameOver && !isGameClear)
        {
            yield return wait1Sec;
            timer += 1f;// Time.deltaTime;
            int _min = (int)timer / 60;
            int _sec = (int)timer % 60;
            timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);

        }

        //종료 사운드재생 후 1초 대기
        yield return wait1Sec;
        
    }

    public void CorrectFunc(int _level, bool _correct)  //정답 처리
    {
        switch (_level)
        {
            case 1: LevelCorrectFunc(_correct);break;  //1단계 정답 여부 처리
            case 2: LevelCorrectFunc(_correct); break; //2단계 정답 여부 처리
            case 3: LevelCorrectFunc(_correct); break; //3단계 정답 여부 처리
        }
    }

    void LevelCorrectFunc(bool _correct)  //정답 여부 처리
    {
        if (_correct)
        {
            correctCount++;
            StartCoroutine(StatusAction("Correct"));
        }
        else
        {
            lifeCount--;
            if (lifeCount < 0) lifeCount = 0;
            StartCoroutine(StatusAction("Incorrect"));
            for (int i = 3; i > lifeCount; i--)
            {
                //Resources.Load<Sprite>("gMiniGame/Marine/UI/img_job_marine_heart_02")
                lifeImages[i-1].sprite = hearts[1];
            }
        }

    }

    IEnumerator StatusAction(string _correct)  //정답 여부에 따른 연출
    {
        switch (_correct)
        {
            case "Correct":
                SoundManager.instance.PlayEffectSound("eff_Jellyfish_correct", 1f);
                //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_03")
                portraitImage.sprite = characters[2];
                //yield return wait1Sec;
                GameObject _heal = Instantiate(healEffect, new Vector3(230f, 110f, 0f), Quaternion.Euler(healEffect.transform.rotation.x, healEffect.transform.rotation.y, healEffect.transform.rotation.z));
                _heal.transform.parent = healBg.transform;
                _heal.transform.localPosition = new Vector3(230f, 100f, 0f);
                Destroy(_heal, 2f);

                if (correctCount >= 2 && gameLevel == 1)
                {
                    yield return wait1Sec;
                    //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_03")
                    portraitImage.sprite = characters[2];
                    //Resources.Load<Sprite>("gMiniGame/Marine/Obj/object_03")
                    injuryImage.sprite = objects[1];
                    isGameClear = true;
                    yield return wait1Sec;
                    //게임클리어 함수 추가
                    RoundClearResultSet(gameLevel);
                    //GameLevelClear(gameLevel);
                }
                else if (correctCount >= 4 && gameLevel == 2)
                {
                    yield return wait1Sec;
                    //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_03")
                    portraitImage.sprite = characters[0];
                    //Resources.Load<Sprite>("gMiniGame/Marine/Obj/object_05")
                    injuryImage.sprite = objects[2];
                    isGameClear = true;
                    yield return wait1Sec;
                    //게임클리어 함수 추가
                    RoundClearResultSet(gameLevel);
                    //GameLevelClear(gameLevel);
                }
                else if (correctCount >= 3 && gameLevel == 3)
                {
                    //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_03")
                    portraitImage.sprite = characters[2];
                    Color _color = lv3CreamImage.color;
                    while (_color.a < 1)
                    {
                        yield return new WaitForFixedUpdate();
                        _color.a += 0.01f;
                        lv3CreamImage.color = _color;
                    }
                    //portraitImage.sprite = Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_03");
                    //injuryImage.sprite = Resources.Load<Sprite>("gMiniGame/Marine/Obj/object_03");
                    isGameClear = true;
                    yield return wait1Sec;
                    //게임클리어 함수 추가
                    RoundClearResultSet(gameLevel);
                    //GameLevelClear(gameLevel);
                }
                //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_01")
                else { yield return wait1Sec; yield return wait1Sec; portraitImage.sprite = characters[0]; }
                
                break;
            case "Incorrect": //오답
                /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_wrong");
                sfxPlayer.clip = sfxSound; sfxPlayer.Play();*/
                //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_02")
                SoundManager.instance.PlayEffectSound("eff_Jellyfish_wrong", 1f);
                portraitImage.sprite = characters[1];
                alertIamge.SetActive(true);
                CameraShakeEffect();
                yield return new WaitForSeconds(1.3f);
                alertIamge.SetActive(false);
                if (lifeCount <= 0)
                {
                    isGameOver = true;
                    isGameClear = false;
                    yield return wait1Sec;
                    GameLevelClear(gameLevel);
                }
                //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_01")
                else portraitImage.sprite = characters[0];
                break;
        }
    }

    //public float shakeAmount;
    //public float shakeTime;

    void CameraShakeEffect()
    {
        float shakeTime = 1.2f;
        float shakeAmount = 0.1f;
        //Camera _camera = Camera.main;
        StartCoroutine(ShakeEffectFunction(shakeTime, shakeAmount));
    }

    IEnumerator ShakeEffectFunction(float shakeTime, float shakeAmount)
    {
        Vector3 initialPosition = gameWindow.transform.position;
        yield return new WaitForFixedUpdate();
        while (shakeTime > 0)
        {
            yield return new WaitForFixedUpdate();
            gameWindow.transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
            shakeTime -= Time.deltaTime;
        }
        shakeTime = 0f;
        gameWindow.transform.position = initialPosition;
    }

    //IEnumerator Lv3CreamImageChangeColor()
    //{
    //    Color _color = lv3CreamImage.color;
    //    while (_color.a < 1)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        _color.a += 0.1f;
    //        lv3CreamImage.color = _color;
    //    }
    //}

    void GameLevelClear(int _gamelevel)  //단계별 클리어/게임오버
    {
        if (isGameClear)  //단계 클리어시 현재 단계에 따라 다음 단계 진행
        {
            ItemSlotButtonSetFalse(true);

            if (_gamelevel < 3)
            {
                switch (_gamelevel)
                {
                    case 1: StartCoroutine(Level2GamePlay()); lv1injuryPart.SetActive(false); break;
                    case 2: StartCoroutine(Level3GamePlay()); break;
                }
            }
            else if (_gamelevel == 3)
            {
                StartCoroutine(NextStep());
            }
            
        }
        else //게임오버 시에는 게임팝업창 닫기
        {
            ItemSlotButtonSetFalse(false);
            for(int i = 0; i < lv2InjuryParts.Length; i++)  //2단계 해파리 오브젝트 비활성화 시켜주기 (결과창 위에 이미지가 보여짐)
            {
                lv2InjuryParts[i].SetActive(false);
            }
            SoundManager.instance.PlayEffectSound("eff_Common_fail", 1f);
            resultPanel.SetActive(true);
            //resultText0offsetY = 1.14f;
            resultText.text = "";
            startObject.SetActive(false);
            failObject.SetActive(true);
            clearObject.SetActive(false);
            resultButton[0].gameObject.SetActive(true);
            resultButton[1].gameObject.SetActive(true);
            levelSuccessPanel.SetActive(false);
            //levelGroups[0].SetActive(false); levelGroups[1].SetActive(false); levelGroups[2].SetActive(false); lineImages[0].gameObject.SetActive(false); lineImages[1].gameObject.SetActive(false);

            //StartCoroutine(NextStep());
            // 5초 후 팝업창 닫기
            StartCoroutine(CloseMiniGamePopup("FirstaidGamePanel"));
        }
    }

    IEnumerator CloseMiniGamePopup(string _contentName)  //결과창 활성화 후 5초뒤 창닫기
    {
        yield return new WaitForSeconds(5f);
        //sceneManager.CloseMiniGamePopUp(_contentName);
    }

    IEnumerator NextStep()  //3단계 조치 완료 시 실행
    {
        yield return new WaitForSeconds(3f);
        lv3CreamImage.color = new Color(1f, 1f, 1f, 1f);
        resultPanel.SetActive(true);
        //SfxSoundPlay("eff_Common_clear");

        levelSuccessPanel.SetActive(false);
        //resultText0offsetY = 1.14f;
        resultText.text = "";
        startObject.SetActive(false);
        failObject.SetActive(false);
        clearObject.SetActive(true);
        resultButton[0].gameObject.SetActive(true);
        resultButton[1].gameObject.SetActive(true);
        ItemSlotButtonSetFalse(false);


        StartCoroutine(CloseMiniGamePopup("FirstaidGamePanel"));
    }

    public void ItemSlotButtonSetFalse(bool _isBool)  //아이템 사용 시, 결과창 출력 시 버튼 동작 비활성화
    {
        buttonCover.SetActive(_isBool);
    }

    void SetActiveJellyfish()   //해파리 촉수/독침 오브젝트 활성화
    {
        lv1injuryPart.SetActive(false);
        for (int i = 0; i < lv2InjuryParts.Length; i++)
        {
            lv2InjuryParts[i].SetActive(true);
            jellyfishImages[i].gameObject.SetActive(true);
            Color _color = jellyfishImages[i].GetComponent<Image>().color;
            _color.a = 1f;
            jellyfishImages[i].GetComponent<Image>().color = _color;
        }

        //해파리 촉수/독침 일부 비활성화
        int rand1 = Random.Range(0, 3);
        lv2InjuryParts[rand1].SetActive(false); //랜덤으로 촉수 1개 비활성화
        jellyfishImages[rand1].gameObject.SetActive(false);
        int rand2 = Random.Range(3, 7);
        lv2InjuryParts[rand2].SetActive(false); //랜덤으로 독침 1개 비활성화
        jellyfishImages[rand2].gameObject.SetActive(false);
        int rand3 = Random.Range(3, 7);
        if (rand3 == rand2)  //동일한 독침이 나오지 않도록 rand3 숫자 변경
        {
            while (rand2 == rand3)
            {
                rand3 = Random.Range(3, 7);
            }
        }
        lv2InjuryParts[rand3].SetActive(false); //랜덤으로 독침 1개 비활성화
        jellyfishImages[rand3].gameObject.SetActive(false);
    }

    void SetLevel2Window()   //2단계 화면 구성
    {
        levelText.text = "2단계";
        gameLevel = 2;
        correctCount = 0;
        readyCount = 3;
        isGameClear = false;
        for (int i = 0; i < lifeCount; i++) //체력 이미지
        {
            //Resources.Load<Sprite>("gMiniGame/Marine/UI/img_job_marine_heart_01")
            lifeImages[i].sprite = hearts[0];
        }
        //타이머 (1단계에서 계속)
        int _min = (int)timer / 60;
        int _sec = (int)timer % 60;
        timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);

        buttonCover.SetActive(false);
        for (int i = 0; i < itemSlots.Length; i++)  //아이템슬롯 오브젝트에 게임레벨을 2로 설정
        {
            itemSlots[i].SetActive(true);
            itemSlots[i].GetComponent<ItemSlot>().gameLevel = 2;
            if (i > 3)
            {
                itemSlots[i].SetActive(false); //5~7번째 슬롯은 비활성화
            }
            itemSlots[i].GetComponent<ItemSlot>().SetItemInfor();
        }

        //Resources.Load<Sprite>("gMiniGame/Marine/Obj/object_03")
        injuryImage.sprite = objects[1];  //팔 상처 이미지 변경
        //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_01")
        portraitImage.sprite = characters[0];  //NPC초상화 변경

        //결과창 활성화 (게임 시작 카운트 또는 READY/START 텍스트 출력용
        resultPanel.SetActive(true);
        startObject.SetActive(false);
        failObject.SetActive(false);
        clearObject.SetActive(false);
        resultText.text = "촉수제거";
        resultButton[0].gameObject.SetActive(false);
        resultButton[1].gameObject.SetActive(false);
    }

    void RoundClearResultSet(int _gameLevel)  //단계별 클리어 시 성공 코멘트 출력
    {
        SoundManager.instance.PlayEffectSound("eff_Common_next", 1f);
        resultPanel.SetActive(true);

        StartCoroutine(LevelSuccessFunction(_gameLevel));

        resultButton[0].gameObject.SetActive(false);
        resultButton[1].gameObject.SetActive(false);
    }

    IEnumerator LevelSuccessFunction(int _gameLevel) //단계 성공 시 체크 효과 애니메이션 처리
    {
        levelGroups[0].SetActive(true); levelGroups[1].SetActive(true); levelGroups[2].SetActive(true); 
        lineImages[0].gameObject.SetActive(true); lineImages[1].gameObject.SetActive(true);
        startObject.SetActive(false);
        failObject.SetActive(false);
        clearObject.SetActive(false);

        switch (_gameLevel)
        {
            case 1:
                lineImages[1].GetComponent<Image>().enabled = false;
                lineImages[3].GetComponent<Image>().enabled = false;
                resultText.text = "응급처치 성공!";
                successRoundImages[0].enabled = true;
                successRoundImages[1].enabled = false;
                successRoundImages[2].enabled = false;
                levelTexts[0].color = new Color(46f, 148f, 45f, 255f);


                checkImages[0].GetComponent<Image>().enabled = true;

                yield return wait1Sec;

                yield return checkImages[0].GetComponent<Image>().fillAmount < 1;
                yield return wait1Sec;
                break;

            case 2:
                lineImages[1].GetComponent<Image>().enabled = false;
                lineImages[3].GetComponent<Image>().enabled = false;
                successRoundImages[0].enabled = true;
                successRoundImages[1].enabled = true;
                successRoundImages[2].enabled = false;

                checkImages[0].GetComponent<Animator>().enabled = false;
                checkImages[0].fillAmount = 1f;
                checkImages[0].enabled = true;

                resultText.text = "촉수 제거 성공!";

                checkImages[1].GetComponent<Image>().enabled = true;
                yield return checkImages[1].GetComponent<Image>().fillAmount < 1;

                lineImages[1].GetComponent<Image>().enabled = true;
                lineImages[3].GetComponent<Image>().enabled = false;
                yield return lineImages[1].GetComponent<Image>().fillAmount < 1;
                yield return wait1Sec;
                break;

            case 3:
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

                resultText.text = "임시치료 성공!";
                checkImages[2].GetComponent<Image>().enabled = true;
                yield return checkImages[2].GetComponent<Image>().fillAmount < 1;

                lineImages[3].GetComponent<Image>().enabled = true;
                yield return lineImages[3].GetComponent<Image>().fillAmount < 1;
                yield return wait1Sec;

                break;
        }



        GameLevelClear(_gameLevel);
    }

    IEnumerator Level2GamePlay()
    {


        //yield return wait1Sec; yield return wait1Sec; yield return wait1Sec;
        SetLevel2Window();

        float _startCount = (float)readyCount;
        float _roundImageCount = 1f;
        countRoundImage.enabled = true;
        countRoundImage.fillAmount = _roundImageCount;
        //countRoundImage.transform.position = resultText[0].transform.position;
        //countLineImage.transform.position = countRoundImage.transform.position;
        countLineImage.enabled = true;
        //게임시작 전 대기 상태 구성
        while (_startCount > 0)
        {
            readyCountText.text = Mathf.Ceil(_startCount).ToString("0");
            //resultText[0].text = Mathf.Ceil(_startCount).ToString("0");   //게임 시작 카운트 텍스트 출력 + 올림 처리
            yield return new WaitForSeconds(Time.deltaTime);
            _roundImageCount -= Time.deltaTime / 1.5f;
            countRoundImage.fillAmount = _roundImageCount;  //동그라미 이미지 radial360 처리
            _startCount -= Time.deltaTime * 2f;
        }
        readyCountText.text = "";
        countLineImage.enabled = false;
        resultText.text = "";
        startObject.SetActive(true);
        failObject.SetActive(false);
        clearObject.SetActive(false);
        yield return wait1Sec;

        resultPanel.SetActive(false);

        //촉수/독침 오브젝트 활성화
        SetActiveJellyfish();

        //게임시작
        while (!isGameOver && !isGameClear) //게임진행 중, 타이머
        {
            //yield return new WaitForFixedUpdate();
            yield return wait1Sec;// new WaitForSeconds(1f);
            timer += 1f;// Time.deltaTime;
            int _min = (int)timer / 60;
            int _sec = (int)timer % 60;
            timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);
        }

        //제한시간경과에 의한 게임오버 동작
        //종료 사운드재생 후 1초 대기
        yield return wait1Sec;

        //결과창 출력 (결과창은 결과내용 및 나가기 버튼으로 구성)
        switch (isGameClear)
        {
            case true: break;
            case false: break;
        }
    }

    void SetLevel3Window()   //3단계 화면 구성
    {
        levelText.text = "3단계";
        gameLevel = 3;
        correctCount = 0;
        readyCount = 3;
        isGameClear = false;
        for (int i = 0; i < lifeCount; i++) //체력 이미지
        {
            //Resources.Load<Sprite>("gMiniGame/Marine/UI/img_job_marine_heart_01")
            lifeImages[i].sprite = hearts[0];
        }
        //타이머 (1단계에서 계속)
        int _min = (int)timer / 60;
        int _sec = (int)timer % 60;
        timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);

        buttonCover.SetActive(false);
        for (int i = 0; i < itemSlots.Length; i++)  //아이템슬롯 오브젝트에 게임레벨을 3로 설정
        {
            itemSlots[i].SetActive(true);
            itemSlots[i].GetComponent<ItemSlot>().gameLevel = 3;
            if (i > 5)
            {
                itemSlots[i].SetActive(false); //5~7번째 슬롯은 비활성화
            }
            itemSlots[i].GetComponent<ItemSlot>().SetItemInfor();
        }

        lv3CreamImage.color = new Color(1f, 1f, 1f, 0f);
        //Resources.Load<Sprite>("gMiniGame/Marine/Obj/object_05")
        injuryImage.sprite = objects[2];  //팔 상처 이미지 변경
        //Resources.Load<Sprite>("gMiniGame/Marine/Obj/character_mask_01")
        portraitImage.sprite = characters[0];  //NPC초상화 변경

        //결과창 활성화
        resultPanel.SetActive(true);
        startObject.SetActive(false);
        failObject.SetActive(false);
        clearObject.SetActive(false);
        resultText.text = "임시치료";
        resultButton[0].gameObject.SetActive(false);
        resultButton[1].gameObject.SetActive(false);
    }

    IEnumerator Level3GamePlay()
    {
        //yield return wait1Sec; yield return wait1Sec; yield return wait1Sec;
        lv3injuryPart.SetActive(true);
        SetLevel3Window();

        float _startCount = (float)readyCount;
        float _roundImageCount = 1f;
        countRoundImage.fillAmount = _roundImageCount;
        //countRoundImage.transform.position = resultText[0].transform.position;
        //countLineImage.transform.position = countRoundImage.transform.position;
        countLineImage.enabled = true;
        //게임시작 전 대기 상태 구성
        while (_startCount > 0)
        {
            readyCountText.text = Mathf.Ceil(_startCount).ToString("0");
            //resultText[0].text = Mathf.Ceil(_startCount).ToString("0");   //게임 시작 카운트 텍스트 출력 + 올림 처리
            yield return new WaitForSeconds(Time.deltaTime);
            _roundImageCount -= Time.deltaTime / 1.5f;
            countRoundImage.fillAmount = _roundImageCount;  //동그라미 이미지 radial360 처리
            _startCount -= Time.deltaTime * 2f;
        }
        readyCountText.text = "";
        countLineImage.enabled = false;
        resultText.text = "";
        startObject.SetActive(true);
        failObject.SetActive(false);
        clearObject.SetActive(false);
        yield return wait1Sec;

        resultPanel.SetActive(false);

        //게임시작
        while (!isGameOver && !isGameClear) //게임진행 중, 타이머
        {
            //yield return new WaitForFixedUpdate();
            yield return wait1Sec;// new WaitForSeconds(1f);
            timer += 1f;// Time.deltaTime;
            int _min = (int)timer / 60;
            int _sec = (int)timer % 60;
            timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);
        }

        //제한시간경과에 의한 게임오버 동작
        //종료 사운드재생 후 1초 대기
        yield return wait1Sec;

        //결과창 출력 (결과창은 결과내용 및 나가기 버튼으로 구성)
        switch (isGameClear)
        {
            case true: break;
            case false: break;
        }
    }

    public void OnRetryButtonClick()
    {
        InitStatus();
        StartCoroutine(Level1GamePlay());
    }

    public void OnExitButtonClick()
    {
        Debug.Log("OnExitButtonClick()");
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

}
