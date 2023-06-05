using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 재난관리전문가 게임 로직 컨트롤 스크립트
/// </summary>

public class DisasterGameManager : MonoBehaviour
{ 
    [Header("오브젝트")]
    public static DisasterGameManager instance;
    DisasterController disasterCtrl;
    DisasterManager disasterManager;
    public GameObject mapPanel;
    public GameObject buildingButton;
    public GameObject startPanel;
    public GameObject pausePanel;
    public GameObject helpPanel;
    public GameObject missionResultPanel;
    public Sprite[] resultSprite;
    public List<GameObject> buildingList;
    public Image fadeImage;
    public GameObject normalNews;
    public GameObject alertNews;

    [Header("게임데이터")]
    public Vector3 mapPosition;
    public int maxLvCount = 0;  //레벨이 MAX 인 빌딩의 수
    public bool isGamePlay = false;
    public bool isFailed = false;
    public bool isGameClear = false;
    int touchGoodsValue = 1;
    int Goods = 0;
    public int goods
    {
        get { return Goods; }
        set
        {
            Goods += value;
            textGoodsCount.text = goods.ToString("n0");
        }
    }
    public int buildingCount = 0;
    [Tooltip("게임타이머 / 설정한 시간 내 재난 관리 성공 시 계속 플레이")]
    public float gameTimer = 300f;
    [Tooltip("게임 플레이 시간")]
    public float playTime = 0f;
    int min = 0;
    bool isAlert = false;
    int guideIndex = 0;
    string guide = null;
    public List<Dictionary<string, object>> data;
    public TextAsset textAsset;

    [Header("텍스트")]
    public TextMeshProUGUI textPlayTime;
    public TextMeshProUGUI textGoodsCount;
    public TextMeshProUGUI textNormalGuide;
    public TextMeshProUGUI textAlertGuide;

    Coroutine guideCommentCoroutine;
    Coroutine missionPlayCoroutine;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if(instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        disasterCtrl = FindObjectOfType<DisasterController>();
        disasterManager = FindObjectOfType<DisasterManager>();
        data = CSVReader.Read(textAsset);
    }

    private void Start()
    {
        mapPosition = mapPanel.transform.position;
        buildingList = new List<GameObject>();
        GameSetup();
    }
    public void GameSetup()  //게임 초기 셋업
    {
        StopAllCoroutines();
        SoundManager.instance.StopBGMSound();
        Time.timeScale = 1f;
        isGamePlay = false;
        isGameClear= false;
        isFailed = false;
        pausePanel.SetActive(false);
        helpPanel.SetActive(false);
        missionResultPanel.SetActive(false);
        if (buildingList.Count > 0 )
        {
            foreach(GameObject go in buildingList )
            {
                Destroy(go);
            }
           for(int i= buildingList.Count-1; i>-1;i--)
            {
                buildingList.RemoveAt(i);
            }
        }

        buildingCount = 0;
        maxLvCount = 0;
        Goods = 6000;
        goods = 0;
        playTime = 0;
        min = 0;
        textPlayTime.text = min.ToString("00") + ":" + ((int)playTime).ToString("00");
        BuildingPositionSetup();
        disasterCtrl.GameSetup();
        disasterManager.RetryGame();
        mapPanel.transform.position = mapPosition;
        startPanel.gameObject.SetActive(true);
        StartCoroutine(FadeInEffect());
    }

    IEnumerator FadeInEffect()
    {
        if(!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;
        fadeImage.raycastTarget = true;

        yield return new WaitForSeconds(0.5f);

        while (fadeImage.color.a > 0)
        {
            color.a -= Time.deltaTime;
            fadeImage.color = color;
            yield return null;
        }

        fadeImage.raycastTarget = false;
        missionPlayCoroutine = StartCoroutine(MissionPlay());
    }

    IEnumerator MissionPlay()   //미션플레이 (초기 5분간 재난관리 연습)
    {
        while (startPanel.gameObject.activeSelf) yield return null;

        SoundManager.instance.PlayBGMSound("bg_Disaster", 1f);
        guideIndex = 0;
        GuideComment();

        while (buildingCount <= 0)
        {
            yield return null;
        }
        isGamePlay = true;
        isFailed = false;
        touchGoodsValue = 1;

        float timer = gameTimer;

        while(timer > 0f && !isFailed && isGamePlay)
        {
            timer -= Time.deltaTime;
            if (!isGamePlay) yield break;
            yield return null;
        }

        yield return new WaitForSeconds(2f);
        
        if(timer <= 0f || isFailed) MissionOver();

    }

    void BuildingPositionSetup() //빌딩 위치 설정
    {
        // -300/130, -300/-70  ~ 300/130, 300/-70
        for(int i = 0; i < 10; i++)
        {
            float posX = 0f, posY = 0f;
            switch (i)
            {
                case 0: posX = 333f; posY = 142f;break;
                case 1: posX = 138f; posY = 142f;break;
                case 2: posX = -55f; posY = 142f;break;
                case 3: posX = -250f; posY = 142f;break;
                case 4: posX = -350f; posY = -4f;break;
                case 5: posX = -153f; posY = -4f;break;
                case 6: posX = 42f; posY = -4f;break;
                case 7: posX = 235f; posY = -4f;break;
                case 8: posX = -251f; posY = -151f;break;
                case 9: posX = -56f; posY = -151f;break;
            }
            GameObject _building = Instantiate(buildingButton, new Vector2(posX, posY), Quaternion.identity);
            _building.gameObject.name = "(" + (int)posX + "," + (int)posY + ")";
            _building.transform.SetParent(mapPanel.transform, false);
            _building.transform.localScale = Vector3.one;
            buildingList.Add(_building);
        }
    }

    public void GuideComment()   //가이드 멘트 설정 (화면 하단)
    {
        isAlert = false;
        guideCommentCoroutine = StartCoroutine(GuideCommentSet());
    }

    IEnumerator GuideCommentSet()
    {
        Color color = Color.white;
        int size = 25;

        while (!isAlert)
        {
            
            switch (guideIndex)
            {
                case 0:
                    if (playTime < 10f) guide = "재난관리 시스템을 통해 다가올 재난에 대비해주세요.";
                    else guide = "건물내구도가 떨어지면 획득 재화의 숫자도 줄어듭니다.\n획득 재화를 늘리려면 건물을 수리하여 건물내구도를 올리세요."; break;
                case 1: guide = "화면을 터치하거나 마우스 왼쪽 버튼을 클릭하면 재화를 획득합니다.\n게임을 진행할수록 획득할 수 있는 재화가 늘어납니다."; break;
                case 2: guide = "재난관리 시스템을 통해 다가올 재난에 대비해주세요."; break;
                case 3: guide = "게임 시작 5분동안 재난으로부터 모든 건물을 지켜내면 성공입니다."; break;
            }
            DisasterGuideComment(guide, color, size, TextAlignmentOptions.MidlineLeft, false);
            yield return new WaitForSeconds(15f);
            guideIndex = Random.Range(0, 4);
        }
    }

    //재난발생상황, 일반상황에서의 가이드멘트 설정
    public void DisasterGuideComment(string comment, Color fontcolor, int fontsize, TextAlignmentOptions anchor, bool _isAlert)
    {
        isAlert = _isAlert;
        if (isAlert)
        {
            StopCoroutine(guideCommentCoroutine);
            normalNews.gameObject.SetActive(false);
            alertNews.gameObject.SetActive(true);
            textAlertGuide.text = comment;
            textAlertGuide.color = fontcolor;
            textAlertGuide.fontSize = fontsize;
            textAlertGuide.alignment = anchor;
        }
        else
        {
            alertNews.gameObject.SetActive(false);
            normalNews.gameObject.SetActive(true);
            textNormalGuide.text = comment;
            textNormalGuide.color = fontcolor;
            textNormalGuide.fontSize = fontsize;
            textNormalGuide.alignment = anchor;
        }
    }

    public void MissionOver() //초기 5분 플레이 종료
    {
        //resultPanel.SetActive(true);
        Color _color = Color.white;
        // 파괴된 건물이 있을 경우 실패
        int count = 0;
        foreach (GameObject building in buildingList)
        {
            if (building.GetComponent<DisasterBuilding>().state == BuildingState.BROKEN) count++;
            else continue;
        }

        if (buildingCount <= 0 || count > 0) isFailed = true;

        if (isFailed)
        {
            missionResultPanel.SetActive(true);
            isGamePlay = false;
            missionResultPanel.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text
                = "재난관리 실패\n재난에 의한 피해를 입은 건물이 발생했습니다.";

            ColorUtility.TryParseHtmlString("#FF0000", out _color);
            missionResultPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().color = _color;
            missionResultPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text
                = "Fail...";
            missionResultPanel.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = resultSprite[0];
            missionResultPanel.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
            SoundManager.instance.PlayEffectSound("eff_Barista_wrong", 1f);
        }
        //else if(maxLvCount >= 10)
        //{
        //    resultPanel.SetActive(true);
        //    isGameClear = true;
        //    float _min = min;
        //    float _sec = (playTime % 60);
        //    resultPanel.transform.GetChild(0).GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>().text
        //        = "모든 건물 강화 완료!!\n완료시간 : " + _min.ToString("00") + ":" + _sec.ToString("00");
        //    resultPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<Button>().interactable = true;
        //    SoundManager.instance.PlayEffectSound("eff_Common_clear", 1f);
        //}
        else if (maxLvCount >= 10)
        {
            isGameClear = true;
            float _min = min; missionResultPanel.SetActive(true);
            float _sec = (playTime % 60); ColorUtility.TryParseHtmlString("#f5a032", out _color);
            missionResultPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().color
                = _color;
            missionResultPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text
                = "Success!";
            missionResultPanel.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text
                = "모든 건물 강화 완료!!\n완료시간 : " + _min.ToString("00") + ":" + _sec.ToString("00");
            missionResultPanel.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = resultSprite[1];
            missionResultPanel.transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
            SoundManager.instance.PlayEffectSound("eff_Common_clear", 1f);
        }
        else
        {
            missionResultPanel.SetActive(true);
            ColorUtility.TryParseHtmlString("#f5a032", out _color);
            missionResultPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().color
                = _color;
            missionResultPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetComponentInChildren<TextMeshProUGUI>().text
                = "Success!";
            missionResultPanel.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text
                = "재난관리 성공\n계속해서 건물을 관리해주세요.";
            missionResultPanel.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite = resultSprite[1];
            missionResultPanel.transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
            SoundManager.instance.PlayEffectSound("eff_Common_clear", 1f);
        }

        Time.timeScale = 0f;
    }

    public void SelectBuildingCursor(GameObject building)  //건물을 클릭(터치)하면, 선택된 건물 외 나머지 건물의 커서 비활성화
    {
        foreach(GameObject db in buildingList)
        {
            if (db == building) continue;
            db.GetComponent<DisasterBuilding>().CurSorSelect();
        }
    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        else
        {
            TouchAction();
            MouseAction();
        }

        if (isGamePlay)
        {
            playTime += Time.deltaTime;
            textPlayTime.text = min.ToString("00") + ":" + ((int)playTime).ToString("00");
            if ((int)playTime > 59) { min++; playTime = 0f; touchGoodsValue++; }

            if(maxLvCount >= 10 && !isGameClear)
            {
                MissionOver();
            }
        }
    }

    public void ButtonClickSoundPlay()
    {
        SoundManager.instance.PlayEffectSound("eff_Common_popup", 1f);
    }

    #region 재화획득
    void TouchAction()  //터치 디바이스에서의 재화 획득 컨트롤
    {
        if (isGamePlay)
        {
            int count = Input.touchCount;
            if (count == 0) return;
            for(int i=0;i<count; i++)
            {
                Touch touch = Input.GetTouch(i);
                if(touch.phase== TouchPhase.Began)
                {
                    goods = touchGoodsValue;
                }
            }
        }
    }

    void MouseAction()  //PC에서의 재화 획득 컨트롤
    {
        if (isGamePlay)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.M))
            {
                goods = touchGoodsValue;
            }
        }
    }
    #endregion

    #region 일시정지/재개/종료
    public void PauseGame()  //게임일시정지
    {
        Time.timeScale = 0f;
    }

    public void PlayContinue()  //게임계속하기
    {
        Time.timeScale = 1f;
    }

    public void OnClickHelpButton()
    {
        helpPanel.SetActive(true);
    }

    public void ExitGame()  //게임에서 나가기
    {
        Time.timeScale = 1f;
        //resultPanel.SetActive(false);

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
