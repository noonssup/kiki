using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public enum HealthGameState
{
    None=0,
    Ready=1,
    Play=2,
    //Pause=3,
    Check=4,
    Result=5,
}


public enum ProgramState
{
    WALK = 0,
    RUN = 1,
    PUSHUP = 2,
}

public class HealthGameManager : MonoBehaviour
{
    [Header("게임오브젝트")]
    public static HealthGameManager instance;
    public Transform npcSpawnTr;
    public GameObject normalNPC;
    GameObject activeNormalNPC;
    public GameObject exerciseNPC;
    public GameObject programButtonPrefab;


    [Header("UI패널")]
    public GameObject statusPanel;
    public GameObject programSetupPanel;
    public GameObject pauseMenuPanel;
    public GameObject helpPanel;
    public Transform programScrollView;

    [Header("결과창")]
    public GameObject checkResultPanel;
    public TextMeshProUGUI textCheckDays;
    public TextMeshProUGUI[] textValues = new TextMeshProUGUI[2];
    public GameObject resultPanel;



    [Header("게임진행데이터")]
    public HealthGameState gameState = HealthGameState.None;
    public ProgramState programState;
    public float gameTimer = 0f;
    public float delayTimer = 0f;
    public bool isComplete = false;
    public TextMeshProUGUI textTotalDay;


    [Header("게임데이터")]
    public int targetDay = 120;  //목표일자 (기본 120일 / 1초/1일)
    public float programDay = 0;   //프로그램별 진행일자
    public float prevDays = 0;         //중간 결과에 넣을 진행일자 값
    public int targetHealth = 0;
    public float currentHealth = 0;
    public int targetStrength = 0;
    public float currentStrength = 0;
    public float targetMuscle = 0;
    public float currentMuscle = 0;
    public float targetBodyfat = 0;
    public float currentBodyfat = 0;
    public float[] prevStatus = new float[4];
    public string programName;
    public string programDescription;
    public float value1 = 0;
    public float value2 = 0;
    public int value1type = 0;
    public int value2type = 0;
    public int programID = 0;
    public bool[] isExercise = new bool[4];



    [Header("UI/목표")]
    public TextMeshProUGUI textHealth;
    public TextMeshProUGUI textStrength;
    public TextMeshProUGUI textMuscle;
    public TextMeshProUGUI textBodyfat;
    public Color textColor = new Color();
    public string colorCode = "#00BF00";


    [Header("UI/프로그램정보")]
    public TextMeshProUGUI textProgramName;
    public TextMeshProUGUI textProgramDescription;
    public TextMeshProUGUI textDay;
    public TextMeshProUGUI textValue1;
    public TextMeshProUGUI textValue2;
    List<GameObject> btnObjList = new List<GameObject>();
    public Button gameStartButton;


    public TextAsset textassetNPC;
    public TextAsset textassetProgram;
    public List<Dictionary<string, object>> npcData;
    public List<Dictionary<string, object>> programData;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        programState = ProgramState.WALK;
        npcData = CSVReader.Read(textassetNPC);
        programData = CSVReader.Read(textassetProgram);
    }

    private void Start()
    {
        GameSetup();
        StatusDataInit();
        Invoke("CreateNPC", 0.5f);
    }

    public void GameSetup()  //ui 초기화
    {
        ProgramInformationDataInit(string.Empty, string.Empty);
        ProgramButtonSetup();
    }


    public void StatusDataInit()  //상태창 정보창 초기화
    {
        gameState = HealthGameState.None;
        isComplete = false;

        targetDay = 120;
        targetHealth = 0;
        currentHealth = 0;
        targetStrength = 0;
        currentStrength = 0;
        targetMuscle = 0;
        currentMuscle = 0;
        targetBodyfat = 0;
        currentBodyfat = 0;

        textHealth.text = "  -   /   -  ";
        textStrength.text = "  -   /   -  ";
        textMuscle.text = "  -   /   -  ";
        textBodyfat.text = "  -   /   -  ";

        statusPanel.SetActive(false);
        programSetupPanel.SetActive(false);
    }

    public void ProgramInformationDataInit(string _name, string _description)  //프로그램정보창 초기화
    {
        gameStartButton.interactable = false;
        programName = _name;
        programDescription = _description;

        if (programName == string.Empty)
        {
            textProgramName.text = "프로그램을 선택하세요.";
        }
        else
        {
            textProgramName.text = programName;
        }

        if (programDescription == string.Empty)
        {
            textProgramDescription.text = "선택한 프로그램을 설명합니다.";
        }
        else
        {
            textProgramDescription.text = programDescription;
        }
    }

    void CreateNPC()  //npc 등장
    {
        activeNormalNPC = Instantiate(normalNPC);
        activeNormalNPC.transform.SetParent(npcSpawnTr);
        activeNormalNPC.transform.localScale = Vector3.one;
        activeNormalNPC.transform.localPosition = Vector3.zero;
    }


    public void ProgramButtonSetup()  //운동 프로그램 버튼 설정
    {
        if (btnObjList.Count > 0)
        {
            foreach (GameObject btn in btnObjList)
            {
                Destroy(btn);
            }
        }

        btnObjList = new List<GameObject>();

        for (int i = 0; i < programData.Count; i++)
        {
            GameObject btn = Instantiate(programButtonPrefab);
            btn.transform.SetParent(programScrollView);
            btn.transform.localScale = Vector3.one;
            btnObjList.Add(btn);
            HealthProgram btnCtrl = btn.GetComponent<HealthProgram>();
            btnCtrl.ProgramSetup(int.Parse(programData[i]["id"].ToString()));
        }

    }


    public void ProgramInformationSetup(ProgramValue programValue, Image _btnImage)  //운동 프로그램 선택 시 값 설정
    {
        programName = programValue.programName;
        programDescription = programValue.programDescription;
        value1type = programValue.value1Type;
        value2type = programValue.value2Type;
        value1 = programValue.value1Num;
        value2 = programValue.value2Num;
        programID = programValue.programID;
        programDay = (float)Random.Range(10, 16);

        //프로그램창 텍스트 설정
        textProgramName.text = programName;
        textProgramDescription.text = programDescription;
        textDay.text = "운동일정 : " + programDay.ToString()+"일";
        textValue1.text = ExerciseValueSetup(value1type, value1);
        textValue2.text = ExerciseValueSetup(value2type, value2);

        gameStartButton.interactable = true;
        gameState = HealthGameState.Ready;

        //프로그램 리스트의 버튼색상 설정 (선택된 버튼과 선택되지 않은 버튼의 구분을 위함)
        for (int i=0;i< programScrollView.childCount;i++)
        {
            Image _btn = programScrollView.GetChild(i).GetComponent<Button>().transform.GetComponent<Image>();
            if (_btn == _btnImage)
            {
                _btn.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                _btn.color = new Color(0.5f, 0.5f, 0.5f, 1f);
            }
        }
    }

    public string ExerciseValueSetup(int _index, float _value)  //프로그램창의 운동 효과 텍스트 설정
    {
        string text = string.Empty;
        switch (_index)
        {
            case 1: text = "체력 : " + _value.ToString() + "/일"; break;
            case 2: text = "근력 : " + _value.ToString() + "/일"; break;
            case 3: text = "근육량 : " + _value.ToString() + "%/일"; break;
            case 4: text = "체지방율 : " + _value.ToString() + "%/일"; break;
            case 0: text = "효과 : -- /일"; break;
        }

        return text;
    }


    public void TargetSetup(NPCData _data)  //npc 의 목표치 설정
    {
        statusPanel.SetActive(true);
        targetHealth = _data.targetHealth;
        currentHealth = _data.currentHealth;
        targetStrength = _data.targetStrength;
        currentStrength = _data.currentStrength;
        targetMuscle = _data.targetMuscle;
        currentMuscle = _data.currentMuscle;
        targetBodyfat = _data.targetBodyfat;
        currentBodyfat = _data.currentBodyfat;


        textHealth.text = currentHealth + " / " + targetHealth;
        textStrength.text = currentStrength + " / " + targetStrength;
        textMuscle.text = currentMuscle + "% / " + targetMuscle + "%";
        textBodyfat.text = currentBodyfat + "% / " + targetBodyfat + "%";

        programSetupPanel.SetActive(true);
    }





    public void GameStart()  //게임 시작
    {
        if (gameState == HealthGameState.Play || gameState != HealthGameState.Ready) return;

        if (gameTimer <= 0) gameTimer = 0;
        if (btnObjList.Count > 0)
        {
            foreach (GameObject btn in btnObjList)
            {
                Destroy(btn);
            }
        }
        StartCoroutine(GamePlayRoutine());
    }

    IEnumerator GamePlayRoutine()
    {
        WaitForSeconds wfs = new WaitForSeconds(1);

        gameStartButton.interactable = false;
        gameState = HealthGameState.Play;
        SetPrevStatus();
        NPCSetupToExercise(false); //npc비활성화, 운동npc 활성화

        while (programDay > 0)
        {
            yield return null;
            if (gameState == HealthGameState.Play)
            {
                programDay-= Time.deltaTime;
                gameTimer += Time.deltaTime;
                delayTimer += Time.deltaTime;
                if (delayTimer >= 1)
                {
                    AddValue(value1type, value1);
                    AddValue(value2type, value2);
                    delayTimer = 0f;
                }

            }
            textTotalDay.text = ((int)gameTimer).ToString() + "일";
            if (gameTimer >= targetDay || isComplete)
            {
                GameOverFunction();
                yield break;
            }
        }

        //운동 종료
        //운동 종료를 알리는 호루라기 음이 있으면 좋을 듯?
        //SoundManager.instance.PlayEffectSound("eff_Common_timeout", 1f);
        SetCheckResult();
        NPCSetupToExercise(true); //npc 활성화, 운동npc 비활성화

    }

    //npc 운동 스프라이트로 변경
    void NPCSetupToExercise(bool _value)
    {
        activeNormalNPC.SetActive(_value);
        exerciseNPC.SetActive(!_value);
        if (exerciseNPC.activeSelf)
        {
            exerciseNPC.GetComponent<HealthExercise>().SetExercise(programID);
        }
    }

    void SetCheckResult()  //중간결과 확인
    {
        gameState = HealthGameState.Check;
        checkResultPanel.SetActive(true);
        programSetupPanel.SetActive(false);

        textCheckDays.text = "진행 일자 : " + prevDays.ToString("n0") + "일";

        switch (value1type)
        {
            case 1: textValues[0].text = "체력 : +" + (currentHealth - prevStatus[0]).ToString("n0"); break;
            case 2: textValues[0].text = "근력 : +" + (currentStrength - prevStatus[1]).ToString("n0"); break;
            case 3: textValues[0].text = "근육량 : +" + (currentMuscle - prevStatus[2]).ToString("f1") + "%"; break;
            case 4: textValues[0].text = "체지방 : " + (currentBodyfat - prevStatus[3]).ToString("f1") + "%"; break;
        }

        switch (value2type)
        {
            case 1: textValues[1].text = "체력 : +" + (currentHealth - prevStatus[0]).ToString("n0"); break;
            case 2: textValues[1].text = "근력 : +" + (currentStrength - prevStatus[1]).ToString("n0"); break;
            case 3: textValues[1].text = "근육량 : +" + (currentMuscle - prevStatus[2]).ToString("f1") + "%"; break;
            case 4: textValues[1].text = "체지방 : " + (currentBodyfat - prevStatus[3]).ToString("f1") + "%"; break;
        }
    }


    public void NextStep()  //중간정산 후 결과창에서 확인 버튼 누르면 실행
    {
        GameSetup();
        programSetupPanel.SetActive(true);
        textDay.text = "운동일정 : - 일";
        textValue1.text = ExerciseValueSetup(0, 0);
        textValue2.text = ExerciseValueSetup(0, 0);
    }

    void GameOverFunction()  //게임종료
    {
        gameState = HealthGameState.Result;
        resultPanel.SetActive(true);
        TextMeshProUGUI text = resultPanel.transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0).GetChild(2).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI resultText = resultPanel.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>();

        if (isComplete)
        {
            colorCode = "#F5A032";
            if (ColorUtility.TryParseHtmlString(colorCode, out textColor))
            {
                text.color = textColor;
            }
            text.text = "Success!!";
            resultText.text = "고객의 건강 관리에 성공했습니다!";

        }
        else
        {
            colorCode = "#FF0000";
            if (ColorUtility.TryParseHtmlString(colorCode, out textColor))
            {
                text.color = textColor;
            }
            text.text = "Fail...";
            resultText.text = "고객의 건강 관리가 실패했습니다…";
        }
    }

    void SetPrevStatus()  //비교용 이전 데이터 만들기
    {
        prevDays = programDay;
        prevStatus[0] = currentHealth;
        prevStatus[1] = currentStrength;
        prevStatus[2] = currentMuscle;
        prevStatus[3] = currentBodyfat;
    }


    void AddValue(int _type, float _value)  //운동 효과 적용
    {
        switch (_type)
        {
            case 1: if (targetHealth <= currentHealth) break; currentHealth += _value; break;
            case 2: if (targetStrength <= currentStrength) break; currentStrength += _value; break;
            case 3: if (targetMuscle <= currentMuscle) break; currentMuscle += _value; break;
            case 4: if (targetBodyfat >= currentBodyfat) break; currentBodyfat -= _value; break;
        }

        textHealth.text = currentHealth.ToString("f0") + " / " + targetHealth.ToString("f0");
        TextColorSetup(currentHealth, (float)targetHealth, textHealth);
        textStrength.text = currentStrength.ToString("f0") + " / " + targetStrength.ToString("f0");
        TextColorSetup(currentStrength, (float)targetStrength, textStrength);
        textMuscle.text = currentMuscle.ToString("f1") + "% / " + targetMuscle.ToString("f1") + "%";
        TextColorSetup(currentMuscle, targetMuscle, textMuscle);
        textBodyfat.text = currentBodyfat.ToString("f1") + "% / " + targetBodyfat.ToString("f1") + "%";
        TextColorSetup(targetBodyfat, currentBodyfat, textBodyfat);

        int count = 0;
        if (currentHealth >= targetHealth) { isExercise[0] = true; count++; }
        if (currentStrength >= targetStrength) { isExercise[1] = true; count++; }
        if (currentMuscle >= targetMuscle) { isExercise[2] = true; count++; }
        if (currentBodyfat <= targetBodyfat) { isExercise[3] = true; count++; }

        if (count >= 4) isComplete = true;
    }

    void TextColorSetup(float currentValue, float targetValue, TextMeshProUGUI text) //목표치 달성 시 텍스트 컬러 변경
    {
        if (currentValue >= targetValue)
        {
            colorCode = "#00BF00";
            if (ColorUtility.TryParseHtmlString(colorCode, out textColor))
            {
                text.color = textColor;
            }
        }
        else
        {
            colorCode = "#000000";
            if (ColorUtility.TryParseHtmlString(colorCode, out textColor))
            {
                text.color = textColor;
            }
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
        if (transform.childCount > 0)
        {
            Destroy(this.transform.GetChild(0).gameObject);
        }
        GameSetup();
        StatusDataInit();
        Invoke("CreateNPC", 0.5f);
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
