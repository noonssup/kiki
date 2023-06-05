using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 3D ������ ������ ���ӸŴ��� ��ũ��Ʈ
/// </summary>

public enum PrinterGameState  //���� ���� ����
{
    READY = 0,  //�غ�
    WAIT = 1,   //NPC �ֹ� ���
    WORK = 2,   //������ �۾� ��
    CHECK = 3,  //�۾� �Ϸ� �� �߰� ����
    //PAUSE = 4,  //���� �Ͻ� ����
    RESULT = 5, //���� ���� �� ���
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

    [Header("������Ʈ�ѷ�")]
    public PrinterUIController uiCtrl;

    [Header("���ӵ�����")]
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

    #region ���� ���� ����
    void TimerSet()  //Ÿ�̸� �ؽ�Ʈ ���
    {
        int min, sec;
        min = (int)currentGameTimer / 60;
        sec = (int)currentGameTimer % 60;

        textTimer.text = min.ToString("00") + ":" + sec.ToString("00");
    }

    void DataInit()  //���� ������ �ʱ�ȭ
    {
        textGamePoint.text = gamePoint.ToString("n0");
        currentGameTimer = gameTimer;
        TimerSet();
        gameState = PrinterGameState.WAIT;
    }

    void SetPanelObject()  //���� ���� �� UI�г� ����
    {
        uiCtrl.UIpanelInit(); //������ ����â �ʱ�ȭ
        Invoke("ClientSetup", 2f);
    }

    #endregion


    #region ���� ����
    void ClientSetup()  //npc ����, �ֹ� ���� ���, ������ ����â Ȱ��ȭ
    {
        uiCtrl.ClientSetup();
    }

    public void ReadyGame(bool _value)  //���� ���� ���� ���� Ȯ��
    {
        if (_value)
        {
            
            gameState = PrinterGameState.READY;  //���� ���� ����
        }
        else { gameState = PrinterGameState.WAIT; }  //���� ���� �Ұ�
    }


    //���� ����
    public void OnClickGameStart()
    {
        if (gameState != PrinterGameState.READY) return;
        gameState = PrinterGameState.WORK;
        uiCtrl.OnOffPrinterControlPanel(false);
        uiCtrl.PrinterWorking(0);
        //�ֹ� ���� Ȯ�� (�ֹ� �׸�� ������ �׸��� ��ġ ���� Ȯ��)

    }

    //���� ���� ��ƾ

    void PlayingGame()    //���� �÷��� ���� �� �ð� ����
    {
        if(currentGameTimer > 0)
        {
            currentGameTimer -= Time.deltaTime;
            TimerSet();
        }
        else if(currentGameTimer <= 0)
        {
            //���� ����
            GameSet();
        }

    }


    //�߰� ���
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


    //���� ���
    void GameSet()  //���Ӽ� (���� �ð� ����)
    {
        gameState = PrinterGameState.RESULT;
        uiCtrl.GameSet();
    }

    #endregion


    #region ���� �޴� ��ư ����

    
    public void OnClickPauseButton()  //�Ͻ�������ư
    {
        Time.timeScale = 0f;
        uiCtrl.pauseMenuPanel.SetActive(true);
    }

    
    public void OnClickResumeButton()  //����ϱ��ư (resume)
    {
        Time.timeScale = 1f;
        uiCtrl.pauseMenuPanel.SetActive(false);
    }


    public void OnClickRetryButton()   //�ٽ��ϱ��ư
    {
        DataInit();
        SetPanelObject();
        OnClickResumeButton();
    }


    public void OnClickHelpButton()  //����
    {
        if (!uiCtrl.helpPanel.activeSelf) uiCtrl.helpPanel.SetActive(true);
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

    //�ӽ� �г� ���� ��ư �Ҵ��
    public void OnPrinterInforPanel()
    {
        ClientSetup();
    }

}
