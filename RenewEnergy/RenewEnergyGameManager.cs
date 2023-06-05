using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 신재생에너지 게임매니저 스크립트
/// 게임준비/시작/종료 담당
/// </summary>

public class RenewEnergyGameManager : MonoBehaviour
{
    public static RenewEnergyGameManager instance;
    public RenewEnergyGameController gameCtrl;

    [Header("오브젝트")]
    public GameObject gamePanel;
    public GameObject gameStartPanel;
    public GameObject tutorialPopup;
    public GameObject tutorialGamePanel;
    public GameObject pausePanel;
    public Image fadeImage;

    [Header("게임데이터")]
    public bool isGamePlaying = false;
    public bool isTutorial = false;
    
    [Header("텍스트")] 
    public TextMeshProUGUI textGameStart;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this)
                Destroy(this);
        }
    }

    private void Start()
    {
        if (!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);
        isTutorial = false;
        tutorialPopup.SetActive(false);
        pausePanel.SetActive(false);
        StartCoroutine(FadeInEffectPlay());
        GameSetup();
    }

    public void GameSetup()  //게임 준비
    {
        if (!isTutorial)
        {
            TutorialSetup();
            gameStartPanel.SetActive(false);
            return;
        }
        
        gamePanel.SetActive(true);
        gameCtrl = gamePanel.GetComponent<RenewEnergyGameController>();
        gameCtrl.GameDataInit();
        tutorialGamePanel.SetActive(false);
        isGamePlaying = false;
        gameStartPanel.SetActive(true);
        //textGameStart.text = "Tap to Start";
        gameCtrl.GameDataInit();
        GameStart();
    }

    #region 튜토리얼

    public void TutorialSetup()
    {
        gamePanel.SetActive(false);
        gameCtrl = tutorialGamePanel.GetComponent<RenewEnergyGameController>();
        tutorialPopup.SetActive(true);
        tutorialPopup.transform.GetChild(0).gameObject.SetActive(true);
        tutorialPopup.transform.GetChild(1).gameObject.SetActive(false);
        tutorialGamePanel.SetActive(true);
        isTutorial = true;
        SoundManager.instance.PlayBGMSound("bg_RenewEnergy", 1f);
    }

    public void TutorialPlay()
    {
        StartCoroutine(gameCtrl.TutorialPlay());
    }

    #endregion



    #region 페이드인아웃 효과

    IEnumerator FadeInEffectPlay()
    {
        fadeImage.raycastTarget = true;
        Color _color = new Color(0f, 0f, 0f, 1f);
        fadeImage.color = _color;
        while (_color.a > 0f)
        {
            _color.a -= Time.deltaTime;
            fadeImage.color = _color;
            yield return null;
        }
        fadeImage.raycastTarget = false;
    }

    IEnumerator FadeOutEffectPlay()
    {
        fadeImage.raycastTarget = true;
        Color _color = new Color(0f, 0f, 0f, 0f);
        fadeImage.color = _color;

        while (_color.a < 1f)
        {
            _color.a += Time.deltaTime;
            fadeImage.color = _color;
            yield return null;
        }
    }

    #endregion

    #region 게임시작/종료 함수
    public void GameStart()  //게임 시작
    {
        SoundManager.instance.PlayEffectSound("eff_Common_popup", 1f);
        StartCoroutine(GameStartCoroutine());
    }

    IEnumerator GameStartCoroutine()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f);
        textGameStart.text = "3";
        yield return wfs; 
        textGameStart.text = "2";
        yield return wfs; 
        textGameStart.text = "1";
        yield return wfs;
        textGameStart.text = "";
        isGamePlaying = true;
        gameStartPanel.SetActive(false);
    }

    public void PauseGame()  //일시정지
    {
        pausePanel.SetActive(true);
        if (gameCtrl.name == "TutorialGamePanel")
        {
            pausePanel.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "튜토리얼 종료";
        }
        else { pausePanel.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "다시하기"; }
        Time.timeScale = 0f;
    }

    public void ResumeGame()  //게임재개 (일시정지 상태에서)
    {
        pausePanel.SetActive(false);
        Time.timeScale= 1f;
    }

    public void RetryGame()  //게임 다시 시작하기
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        gamePanel.SetActive(true);
        gameCtrl = gamePanel.GetComponent<RenewEnergyGameController>();
        tutorialPopup.SetActive(false);

        StopAllCoroutines();
        gameCtrl.guidePopup.SetActive(false);
        if(tutorialPopup.activeSelf) tutorialPopup.SetActive(false);
        isGamePlaying = false;
        gameCtrl.resultPanel.SetActive(false);
        StartCoroutine(GameRenewal());
    }

    IEnumerator GameRenewal()  //게임 재시작 준비
    {
        yield return FadeOutEffectPlay();
        yield return new WaitForSeconds(0.2f);
        GameSetup();
        StartCoroutine(FadeInEffectPlay());
    }

    public void ExitGame()  //게임씬에서 나가기
    {
        pausePanel.SetActive(false);
        SoundManager.instance.StopAllEffSound();
        SoundManager.instance.StopAllSound();
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

    #endregion

}
