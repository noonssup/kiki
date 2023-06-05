using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
///  ũ�������� ���ӸŴ��� ��ũ��Ʈ
/// </summary>


public class CreatorGameManager : MonoBehaviour
{
    public static CreatorGameManager instance;
    CreatorContolUIController controlUICtrl;

    [Header("���ӿ�����Ʈ")]
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

    [Header("���ӵ�����")]
    public TextAsset[] textAsset;
    public int gender = 0;
    public string channelName;
    public string[] contentItemsName;
    public int[] contentItemsIndex;
    private int onAirCount = 0;   //���ȸ��
    public int maxOnAirCount = 6; //�ִ� ���ȸ��
    public bool isGamePlay = false;
    public bool isReady = false;
    public bool isWait = false;
    public int subscriber=1;
    [Tooltip("��̵��� ���� ��ȸ�� �߰� ī��Ʈ")]
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
                    textViewCount.text = 0.ToString() + "��";
                }
                else
                {
                    textViewCount.text = viewCount.ToString() + "��";
                }
            }
            else if(viewCount >= 10000)
            {
                textViewCount.text = ((float)viewCount * 0.0001f).ToString("f1") + "����";
            }
        }
    }
    public int income=0;
    public int goodRate;
    public int badRate;
    public float interestRate;
    public float popularityRate;
    public float onAirTimer = 30f;    //��� �ð�
    public int goodCount = 0;
    public int badCount = 0;
    public List<Dictionary<string, object>> tableData;
    public List<Dictionary<string, object>> rateData;
    public List<Dictionary<string, object>> commentData;

    [Header("�ؽ�Ʈ")]
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

    public void GameDataInit()  //���� �ʱ�ȭ
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
        onAirCount = 0; textOnAirCount.text = onAirCount.ToString() + "ȸ";
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

    IEnumerator FadeEffect()  //���̵��ξƿ� ȿ��
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

    public void GameStart(int genderValue, string ChannelNameValue)  //���� ���� (ä�θ�+���� ���� ��)
    {
        gender = genderValue;
        channelName= ChannelNameValue;
        informationPanel.SetActive(false);
        controlUIPanel.SetActive(true);
        playerPanel.SetActive(true);
        controlUICtrl.GameSetup();
        controlUICtrl.PopularityValueSetup(0, 0);
    }

    public bool SetContent(int _id)  //������ ������ ����
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

        CheckInterest();  //��̵� üũ

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

    void CheckInterest()  //��̵� üũ
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

        for(int i=0;i<rateData.Count;i++)  //���� ����
        {
            if (rateData[i]["value1"].ToString() == styleValue && rateData[i]["value2"].ToString() == itemValue)
            {
                goodRate = int.Parse(rateData[i]["good"].ToString());
                badRate = int.Parse(rateData[i]["bad"].ToString());
                interestRate = float.Parse(rateData[i]["interest"].ToString());
                Debug.Log("�� : " + goodRate+ "���: " + badRate + " ��̵�: " + interestRate);
                break;
            }
        }
    }

    public void BroadcastStart()  //��� ����
    {
        isGamePlay = true;
        StartCoroutine(OnAirRoutine());
    }

    IEnumerator OnAirRoutine()   //��� ���� ��
    {
        broadcastButton.SetActive(false);
        statusPanel.SetActive(false);

        //��� �ε� ȭ�� ���
        loadingPanel.SetActive(true);
        loadingGaugeBar.fillAmount= 0;

        while(loadingGaugeBar.fillAmount < 1)
        {
            loadingGaugeBar.fillAmount += (Time.deltaTime * 0.5f);
            yield return null;
        }

        //��� �ε� ��
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
        textOnAirCount.text = onAirCount.ToString() + "ȸ";

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

            //�̹����� 2���� �������� ��� ���� �ð��� 10�� �����϶� �̹��� ���� ó��
            if(_timer < 10)
            {
                controlUICtrl.SetSecondItemImage();
            }



            yield return null;
        }

        while (isWait)     // CreatorOnAirController.CheckViewerCommentReaction() ��� ó�� �κп��� isWait ���
        {
            yield return null;
        }

        OffAir();

        //�߰����� / ��������
        if (onAirCount >= maxOnAirCount)  //�ִ���Ƚ���� �������� ���� ���� ���� ����
        {
            Debug.Log("���� ���� �� ��������");
            controlUICtrl.OffAir(false);
            CheckSubscriberAndPopularity(_subscriberCount, "end");
        }
        else //�߰�����
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

    void CheckResult(int sCount)  //�߰� ����
    {
        resultPanel.SetActive(true);
        CreatorResult ctrl = resultPanel.GetComponent<CreatorResult>();
        ctrl.CheckResult(goodCount, badCount, sCount);
    }

    void LastResult(int sCount)  //���� ���
    {
        lastResultPanel.SetActive(true);
        CreatorResult ctrl = lastResultPanel.GetComponent<CreatorResult>();
        ctrl.LastResult(sCount);
    }

    void CheckSubscriberAndPopularity(float _subscriberCount, string _value)  //������ ����, �α⵵/��̵� ����
    {
        //������ ����
        subscriber = (int)((viewCount * 0.25f) * (1 + ((goodCount * 0.1f) - (badCount * 0.1f))));

        //�α⵵/��̵� ����
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

    #region �Ͻ�����/�簳/����
    public void OnClickPauseButton()  //�����Ͻ�����
    {
        if (!pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnClickResumeButton()  //���Ӱ���ϱ�
    {
        if (pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        if (resultPanel.activeSelf) resultPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickRetryButton()  //���� �����
    {
        StopAllCoroutines();
        GameDataInit();
        OffAir();
        if (pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickHelpButton()  //����
    {
        if (!helpPanel.activeSelf) helpPanel.SetActive(true);
    }

    public void OnClickExitButton()  //���ӿ��� ������
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
