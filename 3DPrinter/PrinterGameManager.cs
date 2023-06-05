using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 3D 프린터 전문가 게임매니저 스크립트
/// </summary>

public enum PrinterGameState  //게임 진행 상태
{
    READY = 0,  //준비
    WAIT = 1,   //NPC 주문 대기
    WORK = 2,   //프린터 작업 중
    CHECK = 3,  //작업 완료 및 중간 정산
    //PAUSE = 4,  //게임 일시 정지
    RESULT = 5, //게임 종료 및 결과
}

[System.Serializable]
public class PrinterData
{
    public int shapeIndex = 0;
    public string shapeItemName=string.Empty;
    public int materialIndex = 0;
    public string materialItemName = string.Empty;
    public int afterIndex = 0;
    public string afterItemName = string.Empty;
}

public class PrinterGameManager : MonoBehaviour
{
    public static PrinterGameManager instance;

    [Header("게임컨트롤러")]
    public PrinterUIController uiCtrl;

    [Header("게임데이터")]
    public PrinterGameState gameState = PrinterGameState.WAIT;
    //public PrinterData printerData;
    public bool isPlay = false;
    public float gameTimer = 300f;
    float currentGameTimer = 0;
    public int gamePoint = 0;

    [Header("UI")]
    public TextMeshProUGUI textGameState;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textGamePoint;


    public TextAsset textassetItem;
    public TextAsset textassetNpc;
    public List<Dictionary<string, object>> itemData;
    public List<Dictionary<string, object>> npcData;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        itemData = CSVReader.Read(textassetItem);
        npcData = CSVReader.Read(textassetNpc);
    }

    private void Start()
    {
        DataInit();
        SetPanelObject();
    }

    private void Update()
    {
        if (gameState == PrinterGameState.WORK)
        {
            PlayingGame();
        }
    }

    #region 게임 정보 설정
    void TimerSet()  //타이머 텍스트 출력
    {
        int min, sec;
        min = (int)currentGameTimer / 60;
        sec = (int)currentGameTimer % 60;

        textTimer.text = min.ToString("00") + ":" + sec.ToString("00");
    }

    void DataInit()  //게임 데이터 초기화
    {
        textGamePoint.text = gamePoint.ToString("n0");
        currentGameTimer = gameTimer;
        TimerSet();
        gameState = PrinterGameState.WAIT;
    }

    void SetPanelObject()  //게임 시작 시 UI패널 설정
    {
        uiCtrl.UIpanelInit(); //프린터 정보창 초기화
        Invoke("ClientSetup", 2f);
    }

    #endregion


    #region 게임 실행
    void ClientSetup()  //npc 등장, 주문 정보 출력, 프린터 정보창 활성화
    {
        uiCtrl.ClientSetup();
    }

    public void ReadyGame(bool _value)  //게임 시작 가능 여부 확인
    {
        if (_value)
        {
            
            gameState = PrinterGameState.READY;  //게임 시작 가능
        }
        else { gameState = PrinterGameState.WAIT; }  //게임 시작 불가
    }


    //게임 시작
    public void OnClickGameStart()
    {
        if (gameState != PrinterGameState.READY) return;
        gameState = PrinterGameState.WORK;
        uiCtrl.OnOffPrinterControlPanel(false);
        uiCtrl.PrinterWorking(0);
        //주문 내역 확인 (주문 항목과 선택한 항목의 일치 개수 확인)

    }

    //게임 실행 루틴

    void PlayingGame()    //게임 플레이 중일 때 시간 감소
    {
        if(currentGameTimer > 0)
        {
            currentGameTimer -= Time.deltaTime;
            TimerSet();
        }
        else if(currentGameTimer <= 0)
        {
            //게임 종료
            GameSet();
        }

    }


    //중간 결과
    public void CheckResult()
    {
        gameState = PrinterGameState.CHECK;
        uiCtrl.PrinterWorking(1);
    }

    public IEnumerator AddPoint(int _point)
    {
        WaitForSeconds wfs = new WaitForSeconds(3);
        int total = gamePoint + _point;

        yield return wfs;
        while (gamePoint < total)
        {
            gamePoint+=10;
            if (gamePoint >= total) gamePoint = total;
            textGamePoint.text = gamePoint.ToString("n0");
            yield return null;
        }


        yield return wfs;
        yield return wfs;

        SetPanelObject();
    }


    //최종 결과
    void GameSet()  //게임셋 (게임 시간 종료)
    {
        gameState = PrinterGameState.RESULT;
        uiCtrl.GameSet();
    }

    #endregion


    #region 게임 메뉴 버튼 조작

    
    public void OnClickPauseButton()  //일시정지버튼
    {
        Time.timeScale = 0f;
        uiCtrl.pauseMenuPanel.SetActive(true);
    }

    
    public void OnClickResumeButton()  //계속하기버튼 (resume)
    {
        Time.timeScale = 1f;
        uiCtrl.pauseMenuPanel.SetActive(false);
    }


    public void OnClickRetryButton()   //다시하기버튼
    {
        DataInit();
        SetPanelObject();
        OnClickResumeButton();
    }


    public void OnClickHelpButton()  //도움말
    {
        if (!uiCtrl.helpPanel.activeSelf) uiCtrl.helpPanel.SetActive(true);
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

    //임시 패널 열기 버튼 할당용
    public void OnPrinterInforPanel()
    {
        ClientSetup();
    }

}
