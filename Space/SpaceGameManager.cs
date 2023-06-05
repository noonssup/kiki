using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 우주비행사
/// 게임매니저 스크립트
/// </summary>

public enum GameMode
{
    NONE=0,
    LauncherMode,
    DockingMode,
}


public class SpaceGameManager : MonoBehaviour
{
    public static SpaceGameManager instance;

    [Header("스크립트컨트롤러")]
    private SpaceLauncherGameController launcherCtrl;
    private SpaceDockingGameController dockingCtrl;
    public SpaceCameraController cameraCtrl;
    public SpaceLight lightCtrl;

    [Header("게임오브젝트")]
    public GameMode gameMode = 0;
    public GameObject[] launcherGroups;
    public GameObject[] dockingGroups;
    public GameObject pauseMenuPanel;
    public GameObject helpPanel;
    public GameObject resultPanel;
    public GameObject startPanel;
    public GameObject dockingShip;
    public GameObject dockingStationObject;
    public Image fadeImage;
    public Transform bgTr;
    public GameObject spaceBg;
    public List<GameObject> spaceBgList = new List<GameObject>();

    [Header("게임데이터")]
    public bool isLauncherReady = false;
    public bool isDockingReady = false;
    public bool isGamePlay = false;
    public int gameModeCount = 0;


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
        
        gameMode = GameMode.NONE;
    }

    private void Start()
    {
        GameSetup();
    }

    #region 게임 모드 셋업

    void GameSetup() //우주비행사 첫 실행 시
    {
        gameModeCount = 0;
        LauncherGameSetup();
        //RendezvousDockingGameSetup();
        FadeEffect();
    }

    public void LauncherGameSetup()  //우주선 발사 모드 셋업
    {
        for (int i = 0; i < dockingGroups.Length; i++)
        {
            dockingGroups[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < launcherGroups.Length; i++)
        {
            launcherGroups[i].gameObject.SetActive(true);
        }
        launcherCtrl = launcherGroups[0].GetComponent<SpaceLauncherGameController>();
        gameMode = GameMode.LauncherMode;
        cameraCtrl.GetGameMode(gameMode, null);
        SoundManager.instance.PlayBGMSound("bg_Space_launch", 1f);
    }


    public void RendezvousDockingGameSetup()  //랑데부/도킹 모드 셋업
    {
        //System.GC.Collect();
        FadeEffect();
        for (int i=0;i<dockingGroups.Length;i++)
        {
            dockingGroups[i].gameObject.SetActive(true);
        }

        for (int i = 0; i < launcherGroups.Length; i++)
        {
            launcherGroups[i].gameObject.SetActive(false);
        }

        dockingCtrl = dockingGroups[0].GetComponent<SpaceDockingGameController>();
        gameMode = GameMode.DockingMode;
        cameraCtrl.transform.position = new Vector3(0f, 0f, cameraCtrl.transform.position.z);
        dockingCtrl.RendezvousDockingModeSetup();
        StartCoroutine(RendezvouzModeStartCount());  //시작카운트

        //우주정거장 컬러값/스케일 초기화
        Color _color = Color.white;
        Image _image = dockingStationObject.GetComponent<Image>();
        _image.transform.localScale = Vector3.one;
        _image.color = _color;
        SoundManager.instance.PlayBGMSound("bg_Space_docking", 1f);


        //우주 배경 셋업 ( x=9ea, y = 7ea )

        if (spaceBgList.Count > 0) return;
        for(int x = -4; x < 5; ++x)
        {
            for(int y = -3; y < 4; ++y)
            {
                GameObject _bg = Instantiate(spaceBg, new Vector3((x * 8), (y * 8), 0f), Quaternion.identity);
                _bg.transform.SetParent(bgTr);
                spaceBgList.Add(_bg);
            }
        }
    }

    IEnumerator RendezvouzModeStartCount()  //랑데부 모드 시작 카운트
    {
        WaitForSeconds wfs = new WaitForSeconds(1);
        if (!startPanel.activeSelf) startPanel.SetActive(true);
        startPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        TextMeshProUGUI text = startPanel.GetComponentInChildren<TextMeshProUGUI>();
        text.transform.localPosition = new Vector3(0f, -60f, 0f);

        yield return wfs;
        for (int i = 3; i > -1; i--)
        {
            text.text = i.ToString();
            if (i == 0) text.text = "";
            yield return wfs;
        }

        startPanel.SetActive(false);
    }

    #endregion

    #region 페이드인아웃 효과
    public void FadeEffect()  //페이드인아웃 효과
    {
        Color color = fadeImage.color;
        if (!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;
        color.a = 1;
        fadeImage.color = color;
        StartCoroutine(FadeInoutEffectPlay(color));
    }

    IEnumerator FadeInoutEffectPlay(Color _color)
    {
        while(fadeImage.color.a < 1f)
        {
            _color.a += Time.deltaTime;
            fadeImage.color = _color;
            yield return null;
        }

        yield return new WaitForSeconds(1);

        while (fadeImage.color.a > 0f)
        {
            _color.a -= Time.deltaTime;
            fadeImage.color = _color;
            yield return null;
        }
        fadeImage.raycastTarget = false;
        fadeImage.gameObject.SetActive(false);

    }


    #endregion



    #region 미션 시작 / 성공 / 실패
    public TextMeshProUGUI OnStartPanel()  //발사 모드 스타트 패널 켜기
    {
        if(!startPanel.activeSelf) startPanel.SetActive(true);
        startPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        TextMeshProUGUI text = startPanel.GetComponentInChildren<TextMeshProUGUI>();
        return text;
    }

    public void ClearLauncherMission(float _record)  //우주선 발사 미션 클리어
    {
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true); //텍스트
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); //성공 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);  //실패 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);  //다시하기 버튼
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);  //나가기 버튼
        resultPanel.GetComponentInChildren<TextMeshProUGUI>().text ="도달 거리 : "+ _record.ToString("f1") + "km";
        SoundManager.instance.PlayEffectSound("eff_Common_result", 1f);
    }

    public void FailLauncherMission()  //우주선 발사 미션 실패
    {
        gameModeCount = 0;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); //텍스트
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); //성공 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);  //실패 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);  //다시하기 버튼
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);  //나가기 버튼
        SoundManager.instance.PlayEffectSound("eff_Common_fail", 1f);
    }

    public void ClearRendezvousMission()  //랑데부 미션 성공
    {
        gameModeCount = 0;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(true); //텍스트
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); //성공 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);  //실패 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(false);  //다시하기 버튼
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);  //나가기 버튼
        resultPanel.GetComponentInChildren<TextMeshProUGUI>().text = "우주선 도킹을 시작합니다.";
    }
        
    public void FailRendezvousMission()  //랑데부 미션 실패
    {
        gameModeCount = 1;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); //텍스트
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); //성공 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);  //실패 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);  //다시하기 버튼
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);  //나가기 버튼
    }

    public void ClearDockingMission()  //도킹 미션 클리어
    {
        SoundManager.instance.PlayEffectSound("RobotAsseySound", 1f);
        StartCoroutine(DockingStationZoomin());
    }

    IEnumerator DockingStationZoomin()  //도킹 미션 클리어 (도킹스테이션 이미지 확대)
    {
        yield return new WaitForSeconds(1);
        Image _image = dockingStationObject.GetComponent<Image>();
        Color _color = _image.color;

        while(dockingStationObject.transform.localScale.x < 3f)
        {
            dockingStationObject.transform.localScale += Vector3.one * Time.deltaTime * 0.5f;
            yield return null;
        }

        while(_image.color.r > 0f)
        {
            _color.r -= Time.deltaTime;
            _color.g -= Time.deltaTime;
            _color.b -= Time.deltaTime;
            _image.color = _color;
            yield return null;
        }

        lightCtrl.isLight = false;  //도킹스테이션 라이트 끄기

        //성공 결과 패널 활성화
        gameModeCount = 0;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); //텍스트
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(true); //성공 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);  //실패 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);  //다시하기 버튼
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);  //나가기 버튼
        //resultPanel.GetComponentInChildren<TextMeshProUGUI>().text = "우주정거장 도착!";
    }

    public void FailDockingMission()  //도킹 미션 실패
    {
        gameModeCount = 2;
        resultPanel.SetActive(true);
        resultPanel.transform.GetChild(0).GetChild(0).gameObject.SetActive(false); //텍스트
        resultPanel.transform.GetChild(0).GetChild(1).gameObject.SetActive(false); //성공 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);  //실패 텍스트 이미지
        resultPanel.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);  //다시하기 버튼
        resultPanel.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);  //나가기 버튼
    }    

    #endregion


    #region 일시정지/재개/종료
    public void OnClickPauseButton()  //게임일시정지
    {
        if (!pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(true);
        gameModeCount = 0;
        Time.timeScale = 0f;
    }

    public void OnClickResumeButton()  //게임계속하기
    {
        if (pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OnClickRetryButton()  //게임 재시작
    {
        StopAllCoroutines();
        if (pauseMenuPanel.activeSelf) pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        switch (gameModeCount)
        {
            case 0:
                GameSetup();
                launcherCtrl.RetryGame();
                break;
            case 1:
                RendezvousDockingGameSetup();
                break;
            case 2:
                dockingCtrl.IntoDockingMode();
                break;
        }
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
