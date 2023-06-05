using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///  크리에이터 게임매니저 스크립트
/// </summary>


public class CreatorGameManager : MonoBehaviour
{
    public static CreatorGameManager instance;
    CreatorContolUIController controlUICtrl;

    [Header("게임오브젝트")]
    public GameObject playerPanel;
    public GameObject onAirPanel;
    public GameObject controlUIPanel;
    public GameObject informationPanel;
    public GameObject pauseMenuPanel;
    public GameObject helpPanel;
    public Image fadeEffectImage;
    public GameObject broadcastButton;
    public GameObject statusPanel;
    public GameObject loadingPanel;
    public Image loadingGaugeBar;
    public GameObject resultPanel;
    public GameObject lastResultPanel;

    [Header("게임데이터")]
    public TextAsset[] textAsset;
    public int gender = 0;
    public string channelName;
    public string[] contentItemsName;
    public int[] contentItemsIndex;
    private int onAirCount = 0;   //방송회차
    public int maxOnAirCount = 6; //최대 방송회차
    public bool isGamePlay = false;
    public bool isReady = false;
    public bool isWait = false;
    public int subscriber=1;
    [Tooltip("흥미도에 따른 조회수 추가 카운트")]
    public int[] viewRate = { 1000, 500, 250, 50 };
    public int viewCount=0;
    public int ViewCount
    {
        get { return viewCount; }
        set { viewCount = value;
            if (viewCount < 10000)
            {
                if (viewCount <= 0)
                {
                    textViewCount.text = 0.ToString() + "뷰";
                }
                else
                {
                    textViewCount.text = viewCount.ToString() + "뷰";
                }
            }
            else if(viewCount >= 10000)
            {
                textViewCount.text = ((float)viewCount * 0.0001f).ToString("f1") + "만뷰";
            }
        }
    }
    public int income=0;
    public int goodRate;
    public int badRate;
    public float interestRate;
    public float popularityRate;
    public float onAirTimer = 30f;    //방송 시간
    public int goodCount = 0;
    public int badCount = 0;
    public List<Dictionary<string, object>> tableData;
    public List<Dictionary<string, object>> rateData;
    public List<Dictionary<string, object>> commentData;

    [Header("텍스트")]
    public TextMeshProUGUI textViewCount;
    public TextMeshProUGUI textSubscriberCount;
    public TextMeshProUGUI textOnAirCount;

    private void Awake()
    {
        if (instance == null) instance = this;
        else
        {
            Destroy(this);
        }
        tableData = CSVReader.Read(textAsset[0]);
        rateData = CSVReader.Read(textAsset[1]);
        commentData = CSVReader.Read(textAsset[2]);
        controlUICtrl = controlUIPanel.GetComponent<CreatorContolUIController>();
    }

    private void Start()
    {
        GameDataInit();
    }    

    public void GameDataInit()  //게임 초기화
    {
        if (!informationPanel.activeSelf) informationPanel.SetActive(true);
        if (controlUIPanel.activeSelf) controlUIPanel.SetActive(false);
        if (playerPanel.activeSelf) playerPanel.SetActive(false);
        if (pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        if (helpPanel.activeSelf) helpPanel.SetActive(false);
        if (onAirPanel.activeSelf) onAirPanel.SetActive(false);
        if(loadingPanel.activeSelf) loadingPanel.SetActive(false);
        if (resultPanel.activeSelf) resultPanel.SetActive(false);
        if (lastResultPanel.activeSelf) lastResultPanel.SetActive(false);

        isGamePlay = false;
        onAirCount = 0; textOnAirCount.text = onAirCount.ToString() + "회";
        controlUICtrl.OffAir(true);
        popularityRate = 0f;
        interestRate = 0f;
        ViewCount= 0;
        subscriber = 1;
        textSubscriberCount.text = "0";
        channelName = string.Empty;
        contentItemsName = new string[3];
        contentItemsIndex = new int[3];

        for (int i = 0; i < contentItemsIndex.Length; i++)
        {
            contentItemsIndex[i] = 0;
            contentItemsName[i] = string.Empty;
        }
        SoundManager.instance.PlayBGMSound("bg_Motor_Exhibition", 1f);
        StartCoroutine(FadeEffect());
    }

    IEnumerator FadeEffect()  //페이드인아웃 효과
    {
        fadeEffectImage.enabled = true;
        fadeEffectImage.raycastTarget = true;
        fadeEffectImage.color = Color.black;
        Color _color = fadeEffectImage.color;
        
        while(fadeEffectImage.color.a > 0f)
        {
            _color.a -= Time.deltaTime * 2f;
            fadeEffectImage.color = _color;
            yield return null;
        }

        fadeEffectImage.raycastTarget = false;
        fadeEffectImage.enabled = false;
    }

    public void GameStart(int genderValue, string ChannelNameValue)  //게임 시작 (채널명+성별 선택 후)
    {
        gender = genderValue;
        channelName= ChannelNameValue;
        informationPanel.SetActive(false);
        controlUIPanel.SetActive(true);
        playerPanel.SetActive(true);
        controlUICtrl.GameSetup();
        controlUICtrl.PopularityValueSetup(0, 0);
    }

    public bool SetContent(int _id)  //컨텐츠 아이템 선택
    {
        if(_id > 100 && _id < 201)
        {
            contentItemsIndex[0] = _id;
        }
        else if (_id > 200 && _id < 301)
        {
            contentItemsIndex[1] = _id;
        }
        else if (_id > 300)
        {
            contentItemsIndex[2] = _id;
        }

        CheckInterest();  //흥미도 체크

        if (contentItemsIndex[0] > 0 && contentItemsIndex[1] > 0 && contentItemsIndex[2] > 0)
        {
            isReady = true;
            return isReady;
        }
        else
        {
            isReady = false;
            return isReady;
        }
    }

    void CheckInterest()  //흥미도 체크
    {
        string styleValue = "";
        string itemValue = "";
        string[] splitText;
        for (int i = 0; i < tableData.Count; i++)
        {
            if (tableData[i]["id"].ToString() == contentItemsIndex[0].ToString())
            {
                splitText = tableData[i]["style100"].ToString().Split('-');
                for (int t = 0; t < splitText.Length; t++)
                {
                    if (splitText[t] == contentItemsIndex[1].ToString())
                    {
                        styleValue = "style100";
                        break;
                    }
                }
                splitText = tableData[i]["style50"].ToString().Split('-');
                for (int t = 0; t < splitText.Length; t++)
                {
                    if (splitText[t] == contentItemsIndex[1].ToString())
                    {
                        styleValue ="style50"; break;
                    }
                }

                splitText = tableData[i]["item100"].ToString().Split('-');
                for (int t = 0; t < splitText.Length; t++)
                {
                    if (splitText[t] == contentItemsIndex[2].ToString())
                    {
                        itemValue = "item100"; break;
                    }
                }

                splitText = tableData[i]["item50"].ToString().Split('-');
                for (int t = 0; t < splitText.Length; t++)
                {
                    if (splitText[t] == contentItemsIndex[2].ToString())
                    {
                        itemValue = "item50"; break;
                    }
                }
                break;
            }
        }

        if (styleValue == "") styleValue = "null";
        if (itemValue == "") itemValue = "null";

        for(int i=0;i<rateData.Count;i++)  //반응 비율
        {
            if (rateData[i]["value1"].ToString() == styleValue && rateData[i]["value2"].ToString() == itemValue)
            {
                goodRate = int.Parse(rateData[i]["good"].ToString());
                badRate = int.Parse(rateData[i]["bad"].ToString());
                interestRate = float.Parse(rateData[i]["interest"].ToString());
                Debug.Log("굿 : " + goodRate+ "배드: " + badRate + " 흥미도: " + interestRate);
                break;
            }
        }
    }

    public void BroadcastStart()  //방송 시작
    {
        isGamePlay = true;
        StartCoroutine(OnAirRoutine());
    }

    IEnumerator OnAirRoutine()   //방송 진행 중
    {
        broadcastButton.SetActive(false);
        statusPanel.SetActive(false);

        //방송 로딩 화면 재생
        loadingPanel.SetActive(true);
        loadingGaugeBar.fillAmount= 0;

        while(loadingGaugeBar.fillAmount < 1)
        {
            loadingGaugeBar.fillAmount += (Time.deltaTime * 0.5f);
            yield return null;
        }

        //방송 로딩 끝
        loadingPanel.SetActive(false);

        goodCount = 0;
        badCount = 0;

        float _timer = onAirTimer;
        float _viewCountTimer = 0f;

        if(interestRate > 80) { _viewCountTimer = 0.2f; }
        else if(interestRate > 60) { _viewCountTimer = 0.4f; }
        else if(interestRate > 40) { _viewCountTimer = 0.6f; }
        else if(interestRate > 20) { _viewCountTimer = 0.8f; }
        else if(interestRate >= 0) { _viewCountTimer = 1f; }

        float _viewerStartTimer = 3f;
        float _vTimer = _viewCountTimer;
        int _subscriberCount = subscriber;

        onAirPanel.SetActive(true);
        controlUIPanel.SetActive(false);
        onAirCount++;
        textOnAirCount.text = onAirCount.ToString() + "회";

        while(_timer > 0)
        {
            _timer -= Time.deltaTime;
            _viewerStartTimer-= Time.deltaTime;
            _vTimer -= Time.deltaTime;
            if(_vTimer < 0 && _viewerStartTimer < 0)
            {
                if(goodRate > 79)
                {
                    ViewCount += viewRate[0];
                }
                else if(goodRate > 39)
                {
                    ViewCount += viewRate[1];
                }
                else if(goodRate > 25)
                {
                    ViewCount += viewRate[2];
                }
                else
                {
                    viewCount+= viewRate[3];
                }

                
                _vTimer = _viewCountTimer;
            }

            //이미지가 2장인 아이템의 경우 남은 시간이 10초 이하일때 이미지 변경 처리
            if(_timer < 10)
            {
                controlUICtrl.SetSecondItemImage();
            }



            yield return null;
        }

        while (isWait)     // CreatorOnAirController.CheckViewerCommentReaction() 댓글 처리 부분에서 isWait 사용
        {
            yield return null;
        }

        OffAir();

        //중간정산 / 최종정산
        if (onAirCount >= maxOnAirCount)  //최대방송횟수를 기준으로 게임 종료 여부 결정
        {
            Debug.Log("게임 종료 및 최종정산");
            controlUICtrl.OffAir(false);
            CheckSubscriberAndPopularity(_subscriberCount, "end");
        }
        else //중간정산
        {
            CheckSubscriberAndPopularity(_subscriberCount, "continue");
        }
    }

    void OffAir()
    {
        controlUIPanel.SetActive(true);
        onAirPanel.SetActive(false);
        broadcastButton.SetActive(true);
        statusPanel.SetActive(true);
        isReady = false;

        for (int i = 0; i < contentItemsIndex.Length; i++)
        {
            contentItemsIndex[i] = 0;
        }
        controlUICtrl.GameSetup();
    }

    void CheckResult(int sCount)  //중간 정산
    {
        resultPanel.SetActive(true);
        CreatorResult ctrl = resultPanel.GetComponent<CreatorResult>();
        ctrl.CheckResult(goodCount, badCount, sCount);
    }

    void LastResult(int sCount)  //최종 결과
    {
        lastResultPanel.SetActive(true);
        CreatorResult ctrl = lastResultPanel.GetComponent<CreatorResult>();
        ctrl.LastResult(sCount);
    }

    void CheckSubscriberAndPopularity(float _subscriberCount, string _value)  //구독자 정산, 인기도/흥미도 정산
    {
        //구독자 정산
        subscriber = (int)((viewCount * 0.25f) * (1 + ((goodCount * 0.1f) - (badCount * 0.1f))));

        //인기도/흥미도 정산
        float _popularityValue = 0;
        if(_subscriberCount <= 0) _subscriberCount= 1;

        if (subscriber <= 0)
        {
            subscriber = 0;
            _popularityValue = ((subscriber + 1) / (_subscriberCount));
        }
        else
        {
            _popularityValue = (subscriber / _subscriberCount);
        }

        textSubscriberCount.text = subscriber.ToString();
        controlUICtrl.PopularityValueSetup((int)interestRate, _popularityValue);

        int subscriberCount = (int)(subscriber - _subscriberCount);
        switch (_value)
        {
            case "end":
                CheckResult(subscriberCount);
                LastResult(subscriber); break;
            case "continue": CheckResult(subscriberCount); break;
        }
    }

    #region 일시정지/재개/종료
    public void OnClickPauseButton()  //게임일시정지
    {
        if (!pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnClickResumeButton()  //게임계속하기
    {
        if (pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        if (resultPanel.activeSelf) resultPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickRetryButton()  //게임 재시작
    {
        StopAllCoroutines();
        GameDataInit();
        OffAir();
        if (pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickHelpButton()  //도움말
    {
        if (!helpPanel.activeSelf) helpPanel.SetActive(true);
    }

    public void OnClickExitButton()  //게임에서 나가기
    {
        Time.timeScale = 1f;
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
    #endregion

}
