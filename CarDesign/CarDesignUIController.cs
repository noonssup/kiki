using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class CarDesignUIController : MonoBehaviour
{
    CarDesignGameManager gameCtrl;
    public GameObject gamePanel;
    public GameObject helpPanel;
    public Image fadeImage;

    private void Start()
    {
        gameCtrl = gamePanel.GetComponent<CarDesignGameManager>();

        if (!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);
        if (helpPanel.activeSelf) helpPanel.SetActive(false);
        fadeImage.raycastTarget = true;
        StartCoroutine(OnGamePanelFadeInEffect(0));

    }

    IEnumerator OnGamePanelFadeInEffect(float delay)
    {

        Color _color = fadeImage.color;
        _color.a = 1f;
        fadeImage.color = _color;
        OnGamePanel();
        yield return new WaitForSeconds(delay);
        while (_color.a >0f)
        {
            _color.a -= Time.deltaTime;
            fadeImage.color = _color;
            yield return null;
        }
        fadeImage.raycastTarget = false;
    }

    public void OnGamePanel()  //게임시작 시 (게임시작 화면으로)
    {
        if (!gamePanel.activeSelf) gamePanel.SetActive(true);
    }

    public void RetryGame()
    {
        gameCtrl.pausePanel.SetActive(false);
        if (!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true;
        gameCtrl.RetryGame();
        StartCoroutine(OnGamePanelFadeInEffect(1));
    }

    public void OnClickHelpPanel()
    {
        helpPanel.SetActive(true);
    }


    public void ExitGame()  //게임 종료 시 (메인메뉴로)
    {
        SoundManager.instance.StopAllEffSound();
        SoundManager.instance.StopAllSound();
        Time.timeScale = 1;
        StartCoroutine(ChangeSceneToLobby()); 
    }

    IEnumerator ChangeSceneToLobby()
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>("Utils/ChangeSceneCanvas"));
        yield return gameObject.GetComponent<ChangeSceneManager>().FadeOut(1f);
        GlobalData.nextScene = "LobbyScene";
        SceneManager.LoadScene("LoadScene");
    }
}
