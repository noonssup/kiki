using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ����������� ���ӸŴ��� ��ũ��Ʈ
/// �����غ�/����/���� ���
/// </summary>

public class RenewEnergyGameManager : MonoBehaviour
{
    public static RenewEnergyGameManager instance;
    public RenewEnergyGameController gameCtrl;

    [Header("������Ʈ")]
    public GameObject gamePanel;
    public GameObject gameStartPanel;
    public GameObject tutorialPopup;
    public GameObject tutorialGamePanel;
    public GameObject pausePanel;
    public Image fadeImage;

    [Header("���ӵ�����")]
    public bool isGamePlaying = false;
    public bool isTutorial = false;
    
    [Header("�ؽ�Ʈ")] 
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

    public void GameSetup()  //���� �غ�
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

    #region Ʃ�丮��

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



    #region ���̵��ξƿ� ȿ��

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

    #region ���ӽ���/���� �Լ�
    public void GameStart()  //���� ����
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

    public void PauseGame()  //�Ͻ�����
    {
        pausePanel.SetActive(true);
        if (gameCtrl.name == "TutorialGamePanel")
        {
            pausePanel.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "Ʃ�丮�� ����";
        }
        else { pausePanel.transform.GetChild(0).GetChild(1).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = "�ٽ��ϱ�"; }
        Time.timeScale = 0f;
    }

    public void ResumeGame()  //�����簳 (�Ͻ����� ���¿���)
    {
        pausePanel.SetActive(false);
        Time.timeScale= 1f;
    }

    public void RetryGame()  //���� �ٽ� �����ϱ�
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

    IEnumerator GameRenewal()  //���� ����� �غ�
    {
        yield return FadeOutEffectPlay();
        yield return new WaitForSeconds(0.2f);
        GameSetup();
        StartCoroutine(FadeInEffectPlay());
    }

    public void ExitGame()  //���Ӿ����� ������
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
