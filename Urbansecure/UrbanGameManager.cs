using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UrbanGameManager : MonoBehaviour
{
    public static UrbanGameManager instance;
    public UrbanSpawnController spawnCtrl;

    [Header("UI패널")]
    public GameObject pauseMenuPanel;
    public GameObject helpPanel;
    public GameObject alertPanel;

    [Header("결과창")]
    public GameObject checkResultPanel;
    public TextMeshProUGUI textCheckDays;
    public TextMeshProUGUI[] textValues = new TextMeshProUGUI[2];
    public GameObject resultPanel;

    [Header("게임데이터")]
    public bool isGamePlay = false;
    float crimeLimit = 100f;
    public float crimeRate = 0f;
    public TextMeshProUGUI textCrimeRate;
    int fixCount = 0;
    public int FixCount
    {
        get { return fixCount; }
        set
        {
            fixCount = value;
            textFixCount.text = fixCount.ToString() + "건";
        }        
    }
    public TextMeshProUGUI textFixCount;

    public float timeLimit = 300f;
    float gameTimer;
    public TextMeshProUGUI textGameTimer;

    //레벨업
    float levelTime = 60f;
    float levelTimer = 0f;



    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        SetInit();
    }

    void SetInit()
    {
        isGamePlay = false;
        FixCount = 0;
        gameTimer = timeLimit+0.5f;
        textGameTimer.text = ((int)gameTimer / 60).ToString("00") + " : " + ((int)gameTimer % 60).ToString("00");
        crimeRate = 0f;
        levelTimer = 1f;
        textCrimeRate.text = crimeRate.ToString("000") + "%";
        if(pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        if (resultPanel.activeSelf) resultPanel.SetActive(false);
        if (alertPanel.activeSelf) alertPanel.SetActive(false);
        if (!helpPanel.activeSelf) helpPanel.SetActive(true);
    }


    public void GameStart()
    {
        if (isGamePlay) return;

        spawnCtrl.GameStart();
        isGamePlay = true;
    }


    private void Update()
    {
        if (isGamePlay)
        {
            GameTimerFlow();   //게임시간 흐름
        }
        CrimeDecrease();  //범죄율 감소
    }

    void GameTimerFlow()  //게임 흐름
    {
        gameTimer -= Time.deltaTime;

        if (gameTimer <= 0f)
        {
            gameTimer = 0f;
            if (crimeRate >= 86)
            {
                GameOver(false);
            }
            else
            {
                GameOver(true);
            }
        }

        if (gameTimer <= 240f)
        {
            levelTimer -= Time.deltaTime;
            if (levelTimer <= 0f)
            {
                spawnCtrl.AddUnit();
                levelTimer = levelTime;
            }
        }

        textGameTimer.text = ((int)gameTimer / 60).ToString("00") + " : " + ((int)gameTimer % 60).ToString("00");
    }


    public void CrimeIncrease()  //범죄율 증가
    {
        if (!isGamePlay) return;

        crimeRate += Time.deltaTime;
        textCrimeRate.text = crimeRate.ToString("000") + "%";
        if(crimeRate >= crimeLimit)
        {
            crimeRate = crimeLimit;
            GameOver(false);
        }
    }


    void CrimeDecrease()  //범죄율 감소
    {
        if (!isGamePlay) return;

        crimeRate -= Time.deltaTime;
        if (crimeRate <= 0f)
        {
            crimeRate = 0f;
        }
        textCrimeRate.text = crimeRate.ToString("000") + "%";
    }

    void GameOver(bool _value)  //게임 종료
    {
        isGamePlay = false;

        resultPanel.SetActive(true);
        string colorCode = string.Empty;
        Color textColor = new Color();
        //if (_value)
        //{
        colorCode = "#F5A032";
        if (ColorUtility.TryParseHtmlString(colorCode, out textColor))
        {

            //textValues[0].text = "Success!!";


            //        SuccessResultSet();
            //    //}
            //    //else
            //    //{
            //    //    colorCode = "#FF0000";
            //    //    if (ColorUtility.TryParseHtmlString(colorCode, out textColor))
            //    //    {
            //    //        textValues[0].color = textColor;
            //    //        textValues[0].text = "Fail...";
            //    //    }
            //    //    textValues[1].text = "범죄로 인해 보안 기능이 파괴되었습니다.";
            //    //}
            //}

            //void SuccessResultSet()  //성공 팝업 결과 텍스트 설정
            //{
            if (crimeRate >= 86)
            {
                colorCode = "#FF0000";
                textValues[0].text = "F grade";
                textValues[1].text = "범죄로 인해 보안 기능이 파괴되었습니다.";
            }
            else if (crimeRate >= 71)
            {
                textValues[0].text = "D grade";
                textValues[1].text = "범죄로 인해 도시에 문제가 생기고 있습니다.";
            }
            else if (crimeRate >= 51)
            {
                textValues[0].text = "C grade";
                textValues[1].text = "범죄로 인해 도시에 문제가 생기고 있습니다.";
            }
            else if (crimeRate >= 31)
            {
                textValues[0].text = "B grade";
                textValues[1].text = "범죄율이 증가하고 있습니다.";
            }
            else if (crimeRate >= 16)
            {
                textValues[0].text = "A grade";
                textValues[1].text = "도시가 안전하게 지켜지고 있습니다.";
            }
            else
            {
                textValues[0].text = "S grade";
                textValues[1].text = "도시의 보안이 철저하게 지켜지고 있습니다.";
            }
            textValues[0].color = textColor;
        }
    }

    void DeleteUnitObject()  //경찰 / 범죄 유닛 제거
    {
        spawnCtrl.DeleteUnitObject();
    }

    public void EmptyPatrolAlert()  //요원 없음 경고창
    {
        if (alertPanel.activeSelf) return;
        StartCoroutine(AlertRoutine());
    }

    IEnumerator AlertRoutine()
    {
        alertPanel.SetActive(true);
        Image _panel = alertPanel.GetComponent<Image>();
        TextMeshProUGUI _text = alertPanel.GetComponentInChildren<TextMeshProUGUI>();
        Color _colorImage = _panel.color;
        Color _colorText = _text.color;
        _colorImage.a = 1f;
        _colorText.a = 1f;
        _panel.color = _colorImage;
        _text.color = _colorText;

        yield return new WaitForSeconds(1.5f);

        while (_panel.color.a > 0)
        {
            _colorImage.a -= Time.deltaTime;
            _colorText.a -= Time.deltaTime;
            _panel.color = _colorImage;
            _text.color = _colorText;
            yield return null;
        }
        alertPanel.SetActive(false);
    }

    #region 게임시작/종료/일시정지/재개

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

        DeleteUnitObject();
        SetInit();

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
