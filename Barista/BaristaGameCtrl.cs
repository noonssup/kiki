using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

//gene 작성
//바리스타 미니게임 로직

public class BaristaGameCtrl : MonoBehaviour
{
    WaitForFixedUpdate waitUpdate = new WaitForFixedUpdate();
    WaitForSeconds wait1sec = new WaitForSeconds(1f);

    [Header("게임데이터")]
    public bool isGamePlaying = false;
    public float timer;               //게임시간
    public TMP_Text timerText;            //게임시간텍스트
    public int score;                 //게임점수
    public TMP_Text scoreText;            //게임점수텍스트
    public bool isFever = false;
    public bool isHelp = false;       //도움말

    [Header("커피종류별스코어")]
    public int scoreEspresso = 100;
    public int scoreAmericano = 150;
    public int scoreIceAmericano = 200;
    public int scoreCaffelatte = 250;
    public int scoreCappuccino = 400;
    public int coffeeMakeStep = 0; //커피제조단계
    public bool isGrind = false; //그라인더 머신 동작 중 또는 템퍼메트에 포터필더가 있을 경우 true
    public bool isEspresso0 = false;
    public bool isEspresso1 = false;
    public TMP_Text[] coffeeMenuText;

    [Header("게임오브젝트")]
    public GameObject helpPanel;  //도움말 창
    int helpCheck = 0;
    public GameObject npcGuestPrefabs;    //손님 게임오브젝트
    public Transform NPCAreaTr;      //NPC 오브젝트의 부모 트랜스폼 (NPCAreaTr 하위로 npc 생성)
    public GameObject[] waitingPos;   //손님이 대기할 위치 (시작지점, 0, 1, 2, 퇴장지점)
    public GameObject[] waitingGuests = new GameObject[3]; //대기 중인 손님을 담을 오브젝트
    public GameObject coffeeTable;    //커피테이블 (아이템오브젝트 생성 시 커피테이블 하위로 생성)
    public GameObject matPorterFilter;      //중앙 메트에 놓여진 필터
    public GameObject[] machinePorterFilter;  //커피머신에 놓여진 필터
    public GameObject tempermatPorterFilter;  //템퍼매트에 놓여진 필터
    public GameObject temper;  //템퍼매트에 놓여진 필터
    public GameObject grinderPorterFilter;  //그라인드에 놓여진 필터
    public GameObject tableEspressoCup;       //테이블 위 에스프레소컵
    public GameObject[] machineEspressoCup;     //커피머신에 놓여진 에스프레소컵
    public BoxCollider2D[] machineTopCollider;     //포터필터를 장착하는 부분
    public GameObject completeCoffee;  //완성된 커피 오브젝트 (커피 종류에 따라 이미지 변경)
    public GameObject iceCup;
    public GameObject hotCup;
    public GameObject alert;
    public GameObject timeOut;
    public GameObject feverTime;
    public GameObject feverTimeTextImage;

    [Header("이미지")]
    public Sprite img_job_barista_shot_on;
    public Sprite img_job_barista_shot_off;
    public Sprite img_job_barista_coffee01_on;
    public Sprite img_job_barista_coffee02_on_01;
    public Sprite img_job_barista_coffee02_on_02;
    public Sprite img_job_barista_coffee02_on_03;
    public Sprite img_job_barista_coffee02_on_05;
    public Sprite img_job_barista_coffee03_on_01;
    public Sprite img_job_barista_coffee03_on_02;
    public Sprite img_job_barista_coffee03_on_03;

    [Header("결과창")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;     //결과창 텍스트
    public Button[] resultButton;

    private void Awake()
    {
        /*sfxPlayer = new AudioSource[7];
        sfxSound = new AudioClip[7];
        bgmPlayer = gameObject.AddComponent<AudioSource>();
        bgmPlayer.clip = bgmSound;
        bgmPlayer.playOnAwake = false;
        bgmPlayer.volume = 0.3f;
        bgmPlayer.loop = true;
        for(int i = 0; i < sfxPlayer.Length; i++)
        {
            if (sfxPlayer[i] == null) 
            { 
                sfxPlayer[i] = gameObject.AddComponent<AudioSource>();
                sfxPlayer[i].playOnAwake = false;
                sfxPlayer[i].volume = 0.5f;
                sfxPlayer[i].loop = false;
            }
        }
        sfxSound[6] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_finish"); 
        sfxPlayer[6].clip = sfxSound[6];*/
    }
    private void OnEnable()
    {
        //CheckHelpGuide();
        GameSetup();
    }

    void CheckHelpGuide()  //도움말 확인 여부
    {
        //isHelp = false;
        if (PlayerPrefs.HasKey("CheckHelpGuide"))
        {
            helpCheck = PlayerPrefs.GetInt("CheckHelpGuide");
        }
        else
        {
            helpCheck = 0;
            OpenHelpPanel();
        }
    }

    public void OpenHelpPanel() //도움말 열기
    {
        helpPanel.SetActive(true);
        //isHelp = true;
        Time.timeScale = 0;
    }

    public void CloseHelpPanel() //도움말 닫기
    {
        //PlayerPrefs.SetInt("CheckHelpGuide", 1);
        helpPanel.SetActive(false);
        isHelp = false;
        Time.timeScale = 1;
    }

    void GameSetup()
    {
        scoreEspresso = 100;
        scoreAmericano = 150;
        scoreIceAmericano = 200;
        scoreCaffelatte = 250;
        scoreCappuccino = 400;

        //if (helpCheck == 0)
        //{
        //    Time.timeScale = 0;
        //}
        coffeeMakeStep = 0;
        alert.gameObject.SetActive(false);
        timeOut.SetActive(false);
        feverTime.SetActive(false);
        feverTimeTextImage.SetActive(false);
        machineTopCollider[0].enabled = true; isEspresso0 = false;
        machineTopCollider[1].enabled = true; isEspresso1 = false;
        isGrind = false;
        isFever = false;

        coffeeMenuText[0].text = "100";
        coffeeMenuText[1].text = "150";
        coffeeMenuText[2].text = "200";
        coffeeMenuText[3].text = "250";
        coffeeMenuText[4].text = "400";
        //coffeeMenuTitleText.text = "메뉴판";
        //coffeeMenuText.text = "에스프레소 : 100\n아메리카노: 150\n아이스아메리카노: 200\n카페라떼: 250\n카푸치노: 400";

        //손님 대기 설정
        //프리팹 준비
        //npcGuestPrefabs[1] = (GameObject)Resources.Load("gMiniGame/Barista/Prefabs/GuestPrefab1");
        //npcGuestPrefabs[2] = (GameObject)Resources.Load("gMiniGame/Barista/Prefabs/GuestPrefab2");
        //npcGuestPrefabs[3] = (GameObject)Resources.Load("gMiniGame/Barista/Prefabs/GuestPrefab3");
        //npcGuestPrefabs[4] = (GameObject)Resources.Load("gMiniGame/Barista/Prefabs/GuestPrefab4");
        NPCAreaTr = GameObject.Find("NPCArea").transform;  //손님이 생성되면 npcareaTr 하위로 생성
        //손님 입장용 변수 설정
        for(int i = 0; i < waitingGuests.Length; i++)
        {
            if(waitingGuests[i] != null)
            {
                Destroy(waitingGuests[i].gameObject);
                waitingGuests[i] = null;

            }
        }

        //게임시간 설정
        timer = 300f;
        int _min = (int)timer / 60;
        int _sec = (int)timer % 60;
        timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);
        //게임스코어 설정
        score = 0;
        scoreText.text = score.ToString("000000");

        //게임오브젝트 설정
        completeCoffee.SetActive(true);  //완성된 음료 비활성화
        completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;   
        machinePorterFilter[0].SetActive(false); //커피머신 위 포터필터 비활성화
        machinePorterFilter[1].SetActive(false); //커피머신 위 포터필터 비활성화
        machineEspressoCup[0].SetActive(false);          //커피머신 위 에스프레소컵 비활성화
        machineEspressoCup[1].SetActive(false);          //커피머신 위 에스프레소컵 비활성화
        tempermatPorterFilter.SetActive(false); //템퍼매트 위 포터필터 비활성화
        grinderPorterFilter.SetActive(false);  //그라인더 위 포터필터 비활성화
        //iceCup.transform.GetChild(0).transform.GetComponent<Image>().enabled = false;  //아이스컵의 에스프레소 이미지 비활성화
        //hotCup.transform.GetChild(0).transform.GetComponent<Image>().enabled = false;  //뜨거운컵의 에스프레소 이미지 비활성화
        for(int i = 0; i < 6; i++)
        {
            completeCoffee.transform.GetChild(i).transform.GetComponent<Image>().enabled = false;  //완성된 커피의 밀크저그 이미지 비활성화
            if(completeCoffee.transform.GetChild(i).transform.GetComponent<Animator>() != null)
            completeCoffee.transform.GetChild(i).transform.GetComponent<Animator>().gameObject.SetActive(false);  //완성된 커피의 밀크저그 애니메이터 비활성화

        }
        //completeCoffee.transform.GetChild(0).transform.GetComponent<Image>().enabled = false;  //완성된 커피의 밀크저그 이미지 비활성화
        //completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;  //완성된 커피의 휘핑크림 이미지 비활성화
        //completeCoffee.transform.GetChild(2).transform.GetComponent<Image>().enabled = false;  //완성된 커피의 시나몬 이미지 비활성화

        //모든 설정이 끝난 후 코루틴으로 게임 시작
        StartCoroutine(BaristaGameStart());

        //사운드설정
        /*bgmSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_bgm");
        bgmPlayer.clip = bgmSound;
        bgmPlayer.pitch = 1f;
        bgmPlayer.Play();
        sfxSound[0] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_clear");*/
        SoundManager.instance.PlayBGMSound("bg_Barista", 1f);
    }
    
    IEnumerator BaristaGameStart()  //게임시작 및 종료
    {
        isGamePlaying = false;
        yield return wait1sec;

        //Color _color = baristaCtrl.fadeinImage.color;
        //_color.a = 1f;

        //while (_color.a > 0)
        //{
        //    yield return waitUpdate;
        //    _color.a -= 0.02f;
        //    baristaCtrl.fadeinImage.color = _color;
        //}

        //게임 가이드 팝업창 활성화 (최초 1회)
        //
        OpenHelpPanel();
        //
        //게임 가이드 팝업창을 닫을 경우 게임시작 카운트 실행

        //baristaCtrl.resultButton[0].gameObject.SetActive(false);
        //baristaCtrl.resultButton[1].gameObject.SetActive(false);
        //baristaCtrl.resultText[1].text = "";
        //baristaCtrl.resultText[0].text = "READY";
        //yield return StartCoroutine(baristaCtrl.GameReadyFunction());  //3~1 START 연출 (게임시작 카운트)
        //yield return waitUpdate;
        isGamePlaying = true;

        while (timer > 0)
        {
            yield return waitUpdate;

            //if(!isHelp) 
                timer -= Time.deltaTime;// 1f;

            int _min = (int)timer / 60;
            int _sec = (int)timer % 60;
            timerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);

            //score++;
            //scoreText.text = score.ToString("0000000");

            if (timer < 0)
            {
                isGamePlaying = false;
            }

            if (isGamePlaying)  //게임이 플레이 중일 때 손님 생성
            {
                CreateGuest();
            }


            if(timer < 46f)
            {
                if (!isFever)
                {
                    //bgmPlayer.pitch = 1.5f;
                    isFever = true;
                    FeverTimeText();
                    StartCoroutine(FeverMode());
                }
            }

        }
        
        
        //게임종료 설정
        yield return wait1sec;
        //타임아웃연출
        
        timeOut.SetActive(true);
        /*sfxSound[0] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_timeout");
        sfxPlayer[0].clip = sfxSound[0];
        sfxPlayer[0].Play();*/
        SoundManager.instance.PlayEffectSound("eff_Common_timeout", 1f);

        yield return wait1sec; yield return wait1sec; yield return wait1sec;
        timeOut.SetActive(false);

        /* 여기 게임 종료 부분? */
        StartCoroutine(GameSetFunction());

        /*sfxSound[0] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_clear");
        sfxPlayer[0].clip = sfxSound[0];
        sfxPlayer[0].Play();*/
        SoundManager.instance.PlayEffectSound("eff_Common_clear", 1f);
    }
    
    IEnumerator FeverMode()
    {
        feverTime.SetActive(true);
        feverTimeTextImage.SetActive(true);
        Image feverColor = feverTime.GetComponent<Image>();
        Color _color = feverColor.color;
        _color.a = 1f;
        feverColor.color = _color;

        while (timer > 0f)
        {
            //float randR = Random.Range(0f, 1f);
            //float randG = Random.Range(0f, 1f);
            //float randB = Random.Range(0f, 1f);
            //_color.r = randR;
            //_color.g = randG;
            //_color.b = randB;
            yield return new WaitForSeconds(0.2f);
            //_color.a = 0.1f;
            //feverColor.color = _color;
            //randR = Random.Range(0f, 1f);
            //randG = Random.Range(0f, 1f);
            //randB = Random.Range(0f, 1f);
            //_color.r = randR;
            //_color.g = randG;
            //_color.b = randB;
            //_color.a = 1f;
            //feverText.color = _color;
        }
        isFever = false;
        feverTime.SetActive(false);
        feverTimeTextImage.SetActive(false);
    }

    void CreateGuest()   //손님 생성 함수
    {
        for (int i = 0; i < waitingGuests.Length; i++)
        {
            int _randomGuestNumber = Random.Range(0, 5);
            if (waitingGuests[i] == null)
            {
                waitingGuests[i] = Instantiate(npcGuestPrefabs/*[_randomGuestNumber]*/, waitingPos[0].transform.position, Quaternion.identity);
                waitingGuests[i].transform.SetParent(NPCAreaTr.transform.GetChild(i+1).transform);  //parent = NPCAreaTr.transform;
                waitingGuests[i].transform.localScale = Vector3.one;
                waitingGuests[i].GetComponent<GuestCtrl>().targetTr = waitingPos[i + 1].transform;
                waitingGuests[i].GetComponent<GuestCtrl>().waitingNumber = i;
                waitingGuests[i].GetComponent<GuestCtrl>().exitTr = waitingPos[waitingPos.Length - 1].transform;
            }
        }
    }

    public void MakeEspresso1()   //에스프레소 원액 제조 1단계
    {
        if (isGrind) return;
        else
        {
            isGrind = true;
            /*sfxSound[1] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_filter");
            sfxPlayer[1].clip = sfxSound[1];
            sfxPlayer[1].Play();*/
            SoundManager.instance.PlayEffectSound("eff_Barista_filter", 1f);
            //그라인더에 있는 포터필터 오브젝트 활성화
            grinderPorterFilter.SetActive(true);
            grinderPorterFilter.transform.GetChild(0).GetComponent<Image>().enabled = true;
            grinderPorterFilter.transform.GetChild(1).GetComponent<Image>().enabled = false;
            //포터필터가 활성화되면 커피분쇄 시작
            //코루틴 함수 작성
            StartCoroutine(MakeEspresso1_1());
        }
    }
    IEnumerator MakeEspresso1_1()
    {
        /*sfxSound[2] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_grind");
        sfxPlayer[2].clip = sfxSound[2];
        sfxPlayer[2].Play();*/
        SoundManager.instance.PlayEffectSound("eff_Barista_grind", 1f);
        yield return new WaitForSeconds(0.8f);
        //0.8초 후 그라인더에 있는 포터필터 오브젝트 비활성화
        grinderPorterFilter.transform.GetChild(0).GetComponent<Image>().enabled = false;
        grinderPorterFilter.transform.GetChild(1).GetComponent<Image>().enabled = true;

        yield return new WaitForSeconds(0.5f);
        grinderPorterFilter.SetActive(false);
        //동시에 템퍼매트에 있는 포터필터 오브젝트 활성화
        tempermatPorterFilter.transform.GetChild(0).gameObject.SetActive(false);
        tempermatPorterFilter.transform.GetChild(1).gameObject.SetActive(true);
        tempermatPorterFilter.transform.GetChild(2).gameObject.SetActive(false);
        tempermatPorterFilter.transform.GetChild(3).gameObject.SetActive(false);
        tempermatPorterFilter.GetComponent<DragItem>().enabled = false;
        tempermatPorterFilter.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        MakeEspresso2();
        //coffeeMakeStep++;
        //yield break;
    }

    void MakeEspresso2()   //에스프레소 원액  제조 2단계 (템핑 / 템퍼를 포터필터에 갖다대기)
    {
        tempermatPorterFilter.transform.GetChild(0).gameObject.SetActive(false);
        tempermatPorterFilter.transform.GetChild(1).gameObject.SetActive(true);
        tempermatPorterFilter.transform.GetChild(2).gameObject.SetActive(false);
        tempermatPorterFilter.transform.GetChild(3).gameObject.SetActive(true);
        temper.SetActive(false);
        //tempermatPorterFilter.SetActive(true);
        StartCoroutine(MakeEspresso2_1());
    }

    IEnumerator MakeEspresso2_1()   //포터필터의 원두를 누른 상태
    {
        yield return new WaitForSeconds(0.8f);
        tempermatPorterFilter.transform.GetChild(0).gameObject.SetActive(false);
        tempermatPorterFilter.transform.GetChild(1).gameObject.SetActive(false);
        tempermatPorterFilter.transform.GetChild(2).gameObject.SetActive(true);
        tempermatPorterFilter.transform.GetChild(3).gameObject.SetActive(false);
        temper.SetActive(true);
        tempermatPorterFilter.GetComponent<DragItem>().enabled = true;
        //coffeeMakeStep++;
    }

    public void MakeEspresso3(string _name)   //에스프레소 원액 제조 3단계1 (템퍼매트의 포터필터를 커피머신으로 옮기기)
    {
        /*sfxSound[1] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_filter");
        sfxPlayer[1].clip = sfxSound[1];
        sfxPlayer[1].Play();*/
        SoundManager.instance.PlayEffectSound("eff_Barista_filter", 1f);
        switch (_name)
        {
            case "TOP0":
                machinePorterFilter[0].SetActive(true);
                tempermatPorterFilter.transform.GetChild(2).gameObject.SetActive(false);
                machineEspressoCup[0].GetComponent<Image>().sprite = img_job_barista_shot_off;
                machineEspressoCup[0].GetComponent<Image>().SetNativeSize();
                machineEspressoCup[0].GetComponent<Image>().enabled = true;
                machineEspressoCup[0].GetComponent<DragItem>().enabled = false;
                machineEspressoCup[0].SetActive(true);
                StartCoroutine(MakeEspresso3_1(0));
                break;
            case "TOP1":
                machinePorterFilter[1].SetActive(true);
                tempermatPorterFilter.transform.GetChild(2).gameObject.SetActive(false);
                machineEspressoCup[1].GetComponent<Image>().sprite = img_job_barista_shot_off;
                machineEspressoCup[1].GetComponent<Image>().SetNativeSize();
                machineEspressoCup[1].GetComponent<Image>().enabled = true;
                machineEspressoCup[1].GetComponent<DragItem>().enabled = false;
                machineEspressoCup[1].SetActive(true);
                StartCoroutine(MakeEspresso3_1(1));
                break;
        }
        isGrind = false;
    }

    IEnumerator MakeEspresso3_1(int _mPoterFilter)  //커피머신의 에스프레소 원액 완성 처리
    {
        //드롭사운드재생
        /*sfxSound[3] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_brew"); sfxPlayer[3].clip = sfxSound[3];
        sfxPlayer[3].Play();*/
        SoundManager.instance.PlayEffectSound("eff_Barista_brew", 1f);

        if (_mPoterFilter == 0) isEspresso0 = true;
        else isEspresso1 = true;
        yield return new WaitForSeconds(1f);
        machinePorterFilter[_mPoterFilter].SetActive(false);
        machineTopCollider[_mPoterFilter].enabled = false;
        machineEspressoCup[_mPoterFilter].GetComponent<Image>().sprite = img_job_barista_shot_on;
        machineEspressoCup[_mPoterFilter].GetComponent<Image>().SetNativeSize();
        machineEspressoCup[_mPoterFilter].GetComponent<DragItem>().enabled = true;
    }

    public void MakeEspresso(DragItem _dragItem)  //에스프레소 제조
    {
        //hotCup.transform.GetChild(0).transform.GetComponent<Image>().enabled = true;
        StartCoroutine(MakeEspresso_1(_dragItem));
    }

    IEnumerator MakeEspresso_1(DragItem _dragItem)
    {
        completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = true;
        completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(true);
        /*sfxSound[4] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_pour"); sfxPlayer[4].clip = sfxSound[4];
        sfxPlayer[4].Play();*/
        SoundManager.instance.PlayEffectSound("eff_Barista_pour", 1f);
        yield return new WaitForSeconds(1f);
        completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = false;
        completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(false);
        StartCoroutine(CompleteCoffeeEffect());  //커피 완성 연출
        completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee01_on;
        switch (_dragItem.gameObject.name)
        {
            case "EspressoCupM0": machineTopCollider[0].enabled = true; isEspresso0 = false; break;
            case "EspressoCupM1": machineTopCollider[1].enabled = true; isEspresso1 = false; break;
        }
    }

    public enum IceAmericano { EMPTY, ICE, ESPRESSO, ICEESPRESSO, ESPRESSOICE }
    public IceAmericano iaState = IceAmericano.EMPTY;

    public void MakeIceAmericano(string _state, DragItem _dragItem)  //아이스아메리카노 제조
    {
        switch (_state)
        {
            case "ICE": iaState = IceAmericano.ICE; break;
            case "ESPRESSO": iaState = IceAmericano.ESPRESSO; break;
            case "ICEESPRESSO": iaState = IceAmericano.ICEESPRESSO; break;
            case "ESPRESSOICE": iaState = IceAmericano.ESPRESSOICE; break;
        }
        StartCoroutine(MakeIceAmericano_1(_dragItem));
    }

    IEnumerator MakeIceAmericano_1(DragItem _dragItem)  //얼음/에스프레소를 넣은 순서에 따라 완성된 아이스아메리카노 활성화 처리
    {
        switch (iaState)
        {
            case IceAmericano.ICE:  //빈 얼음컵에서 얼음이 담긴 얼음컵으로 이미지 변경
                completeCoffee.transform.GetChild(5).transform.GetComponent<Image>().enabled = true;
                completeCoffee.transform.GetChild(5).transform.GetComponent<Animator>().gameObject.SetActive(true);
                /*sfxSound[5] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_ice"); sfxPlayer[5].clip = sfxSound[5];
                sfxPlayer[5].Play();*/
                SoundManager.instance.PlayEffectSound("eff_Barista_ice", 1f);
                yield return new WaitForSeconds(1f);
                completeCoffee.transform.GetChild(5).transform.GetComponent<Image>().enabled = false;
                completeCoffee.transform.GetChild(5).transform.GetComponent<Animator>().gameObject.SetActive(false);
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee03_on_01;
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().enabled = true;
                break;
            case IceAmericano.ESPRESSO:  //빈 얼음컵에서 에스프레소가 담긴 얼음컵으로 이미지 변경
                completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = true;
                completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(true);
                /*sfxSound[4] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_pour"); sfxPlayer[4].clip = sfxSound[4];
                sfxPlayer[4].Play();*/
                SoundManager.instance.PlayEffectSound("eff_Barista_pour", 1f);
                yield return new WaitForSeconds(1f);
                completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = false;
                completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(false);
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee03_on_03;
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().enabled = true;
                break;
            case IceAmericano.ICEESPRESSO:
                completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = true;
                completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(true);
                /*sfxSound[4] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_pour"); sfxPlayer[4].clip = sfxSound[4];
                sfxPlayer[4].Play();*/
                SoundManager.instance.PlayEffectSound("eff_Barista_pour", 1f);
                yield return new WaitForSeconds(1f);
                completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = false;
                completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(false);
                StartCoroutine(CompleteCoffeeEffect());  //커피 완성 연출
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee03_on_02;
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().enabled = true;
                iaState = IceAmericano.EMPTY;
                break;
            case IceAmericano.ESPRESSOICE:
                completeCoffee.transform.GetChild(5).transform.GetComponent<Image>().enabled = true;
                completeCoffee.transform.GetChild(5).transform.GetComponent<Animator>().gameObject.SetActive(true);
                /*sfxSound[5] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_ice"); sfxPlayer[5].clip = sfxSound[5];
                sfxPlayer[5].Play();*/
                SoundManager.instance.PlayEffectSound("eff_Barista_ice", 1f);
                yield return new WaitForSeconds(1f);
                completeCoffee.transform.GetChild(5).transform.GetComponent<Image>().enabled = false;
                completeCoffee.transform.GetChild(5).transform.GetComponent<Animator>().gameObject.SetActive(false);
                StartCoroutine(CompleteCoffeeEffect());  //커피 완성 연출
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee03_on_02;
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().enabled = true;
                iaState = IceAmericano.EMPTY;
                break;
        }

        switch (_dragItem.gameObject.name)
        {
            case "EspressoCupM0": machineTopCollider[0].enabled = true; isEspresso0 = false; break;
            case "EspressoCupM1": machineTopCollider[1].enabled = true; isEspresso1 = false; break;
        }
    }

    public void MakeHotAmericano(DragItem _dragItem)  //뜨거운 아메리카노 제조
    {
        //hotCup.transform.GetChild(0).transform.GetComponent<Image>().enabled = true;
        StartCoroutine(MakeHotAmericano_1(_dragItem));
    }

    IEnumerator MakeHotAmericano_1(DragItem _dragItem)
    {
        //yield return new WaitForSeconds(2f);
        ////hotCup.transform.GetChild(0).transform.GetComponent<Image>().enabled = false;
        //hotCup.GetComponent<Image>().sprite = Resources.Load<Sprite>("gMiniGame/Barista/Images/item/img_job_barista_coffee02_on_01");
        //yield return new WaitForSeconds(1f);
        //hotCup.GetComponent<Image>().sprite = Resources.Load<Sprite>("gMiniGame/Barista/Images/item/img_job_barista_coffee02_off");
        //completeCoffee.GetComponent<Image>().sprite = Resources.Load<Sprite>("gMiniGame/Barista/Images/object/object_01");
        ////completeCoffee.SetActive(true);

        completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = true;
        completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(true);
        /*sfxSound[4] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_pour"); sfxPlayer[4].clip = sfxSound[4];
        sfxPlayer[4].Play();*/
        SoundManager.instance.PlayEffectSound("eff_Barista_pour", 1f);
        yield return new WaitForSeconds(1f);
        completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = false;
        completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(false);
        StartCoroutine(CompleteCoffeeEffect());  //커피 완성 연출
        completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee02_on_01;
        
        switch (_dragItem.gameObject.name)
        {
            case "EspressoCupM0": machineTopCollider[0].enabled = true; isEspresso0 = false; break;
            case "EspressoCupM1": machineTopCollider[1].enabled = true; isEspresso1 = false; break;
        }
    }

    public void MakeCafelatte(string _state, DragItem _dragItem)    //카페라떼 제조 (밀크저그가 어떤 컵에 닿느냐에 따라 연출 변경)
    {

        StartCoroutine(MakeCafelatte_1(_state, _dragItem));
  
    }

    IEnumerator MakeCafelatte_1(string _state, DragItem _dragItem)
    {
        BoxCollider2D _coll = completeCoffee.GetComponent<BoxCollider2D>();
        _coll.enabled = false;
        float animTime = 0f;
        switch (_state)
        {
            case "AMERICANO":  //만들어진 아메리카노에 밀크추가
                completeCoffee.transform.GetChild(2).transform.GetComponent<Image>().enabled = true;
                completeCoffee.transform.GetChild(2).transform.GetComponent<Animator>().gameObject.SetActive(true);
                animTime = completeCoffee.transform.GetChild(2).transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
                /*sfxSound[4] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_pour"); sfxPlayer[4].clip = sfxSound[4];
                sfxPlayer[4].Play();*/
                SoundManager.instance.PlayEffectSound("eff_Barista_pour", 1f);
                yield return new WaitForSeconds(animTime);
                completeCoffee.transform.GetChild(2).transform.GetComponent<Image>().enabled = false;
                completeCoffee.transform.GetChild(2).transform.GetComponent<Animator>().gameObject.SetActive(false);
                StartCoroutine(CompleteCoffeeEffect());  //커피 완성 연출
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee02_on_02;
                break;
            case "HOTCUP":    //빈hotCup에 밀크추가
                completeCoffee.transform.GetChild(2).transform.GetComponent<Image>().enabled = true;
                completeCoffee.transform.GetChild(2).transform.GetComponent<Animator>().gameObject.SetActive(true);
                animTime = completeCoffee.transform.GetChild(2).transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
                /*sfxSound[4] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_pour"); sfxPlayer[4].clip = sfxSound[4];
                sfxPlayer[4].Play();*/
                SoundManager.instance.PlayEffectSound("eff_Barista_pour", 1f);
                yield return new WaitForSeconds(animTime);
                completeCoffee.transform.GetChild(2).transform.GetComponent<Image>().enabled = false;
                completeCoffee.transform.GetChild(2).transform.GetComponent<Animator>().gameObject.SetActive(false);
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee02_on_05;
                break;
            case "MILK":     //밀크가 있는 hotCup에 에스프레소추가
                completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = true;
                completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(true);
                animTime = completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
                /*sfxSound[4] = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_pour"); sfxPlayer[4].clip = sfxSound[4];
                sfxPlayer[4].Play();*/
                SoundManager.instance.PlayEffectSound("eff_Barista_pour", 1f);
                yield return new WaitForSeconds(animTime);
                completeCoffee.transform.GetChild(4).transform.GetComponent<Image>().enabled = false;
                completeCoffee.transform.GetChild(4).transform.GetComponent<Animator>().gameObject.SetActive(false);
                StartCoroutine(CompleteCoffeeEffect());  //커피 완성 연출
                completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee02_on_02;
                break;
        }

        _coll.enabled = true;
        switch (_dragItem.gameObject.name)
        {
            case "EspressoCupM0": machineTopCollider[0].enabled = true; isEspresso0 = false; break;
            case "EspressoCupM1": machineTopCollider[1].enabled = true; isEspresso1 = false; break;
        }
    }

    //public void MakeCappuccino1()  //카푸치노 제조 1
    //{
    //    //completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().enabled = true;
    //    //completeCoffee.transform.GetChild(1).transform.GetComponent<Animator>().enabled = true;
    //    StartCoroutine(MakeCappuccino1_1());
    //}

    //IEnumerator MakeCappuccino1_1()
    //{
    //    //completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().enabled = true;
    //    completeCoffee.transform.GetChild(1).transform.GetComponent<Animator>().gameObject.SetActive(true);
    //    float animTime = completeCoffee.transform.GetChild(1).transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
    //    yield return new WaitForSeconds(animTime);
    //    //completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
    //    completeCoffee.transform.GetChild(1).transform.GetComponent<Animator>().gameObject.SetActive(false);
    //    completeCoffee.GetComponent<Image>().sprite = Resources.Load<Sprite>("gMiniGame/Barista/Images/item/img_job_barista_coffee02_on_03");
    //    completeCoffee.GetComponent<Image>().SetNativeSize();
        
    //    //completeCoffee.SetActive(true);
    //}

    public void MakeCappuccino2()  //카푸치노 제조 2
    {
         StartCoroutine(MakeCappuccino2_1());
    }

    IEnumerator MakeCappuccino2_1()  //시나몬 이미지켜고끄기
    {
        completeCoffee.transform.GetChild(3).transform.GetComponent<Image>().enabled = true;  
        completeCoffee.transform.GetChild(3).transform.GetComponent<Animator>().gameObject.SetActive(true);
        float animTime = completeCoffee.transform.GetChild(3).transform.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animTime);
        completeCoffee.transform.GetChild(3).transform.GetComponent<Image>().enabled = false;
        completeCoffee.transform.GetChild(3).transform.GetComponent<Animator>().gameObject.SetActive(false);
        StartCoroutine(CompleteCoffeeEffect());
        completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee02_on_03;
        completeCoffee.transform.GetChild(1).transform.GetComponent<Image>().SetNativeSize();   
    }

    IEnumerator CompleteCoffeeEffect()  //커피 완성 시 연출
    {
        completeCoffee.GetComponent<BoxCollider2D>().enabled = false;
        completeCoffee.transform.GetChild(0).transform.GetComponent<Image>().enabled = true;  
        completeCoffee.transform.GetChild(0).transform.GetComponent<Animator>().gameObject.SetActive(true);
        //sfxPlayer[6].Play();
        SoundManager.instance.PlayEffectSound("eff_Barista_finish", 1f);
        yield return new WaitForSeconds(1f);// animTime);
        completeCoffee.GetComponent<BoxCollider2D>().enabled = true;
        completeCoffee.transform.GetChild(0).transform.GetComponent<Image>().enabled = false;
        completeCoffee.transform.GetChild(0).transform.GetComponent<Animator>().gameObject.SetActive(false);
    }

    public void ScoreUp(string _coffeeName)
    {
        int _score = 0;
        switch (_coffeeName)
        {
            case "espresso":
                _score = scoreEspresso;
                break;
            case "americano":
                _score = scoreAmericano;
                break;
            case "iceamericano":
                _score = scoreIceAmericano;
                break;
            case "cafelatte":
                _score = scoreCaffelatte;
                break;
            case "cappuccino":
                _score = scoreCappuccino;
                break;

        }
        if (timer < 46f) _score *= 2;
        int _prevScore = score;
        score += _score;
        StartCoroutine(PointCount(_prevScore));
    }

    //스코어 반영 시 점수 텍스트 효과.....
    IEnumerator PointCount(int _prevScore)
    {
        for (int i = _prevScore; i <= score; i++)
        {
            scoreText.text = i.ToString("000000");
            yield return new WaitForEndOfFrame();
        }
    }


    public void AlertFunction()  //오답 경고 효과
    {
        if(isGamePlaying)
        StartCoroutine(AlertEffect());
    }

    IEnumerator AlertEffect()
    {
        alert.SetActive(true);
        yield return wait1sec;
        alert.SetActive(false);
    }

    private void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.KeypadPlus)) timer += 5f;
        else if (Input.GetKeyDown(KeyCode.KeypadMinus)) timer -= 5f;
#endif
    }

    void FeverTimeText()
    {
        //coffeeMenuTitleText.text = "메뉴판 (점수 두배!!!)";
        coffeeMenuText[0].text = "100x2"; //에스프레소
        coffeeMenuText[1].text = "150x2"; //아메리카노
        coffeeMenuText[2].text = "200x2"; //아이스아메리카노
        coffeeMenuText[3].text = "250x2"; //카페라떼
        coffeeMenuText[4].text = "400x2"; //카푸치노




        //"에스프레소 : 100 x 2\n아메리카노: 150 x 2\n아이스아메리카노: 200 x 2\n카페라떼: 250 x 2\n카푸치노: 400 x 2";
    }

    public IEnumerator GameSetFunction()  //게임종료 시 결과창 처리
    {
        yield return new WaitForFixedUpdate();

        resultButton[0].gameObject.SetActive(true);
        //resultButton[1].gameObject.SetActive(true);
        resultPanel.SetActive(true);
        resultText.text = "점수: " + score.ToString();
    }

    public void OnResultOkButtonClick()
    {
        SoundManager.instance.StopBGMSound();
        Time.timeScale = 1f;
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
