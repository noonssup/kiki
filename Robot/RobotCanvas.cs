using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RobotCanvas : MonoBehaviour
{
    //David edit 2022-11-11
    //public JobGameSceneManager sceneManager;  //직업체험씬 매니저 스크립트

    [SerializeField] GameObject battlePanel;  //배틀씬 오브젝트 패널
    [SerializeField] Image fadeImage;


    public List<Dictionary<string, object>> stMain;  //csv 읽기
    public List<Dictionary<string, object>> robotData;  //csv 읽기
    public TextAsset textAsset1;
    public TextAsset textAsset2;

    private void Awake()
    {
        //David edit 2022-11-11
        //sceneManager = FindObjectOfType<JobGameSceneManager>();
    }

    private void Start()
    {
        if (battlePanel.activeSelf) battlePanel.SetActive(false);

        stMain = CSVReader.Read(textAsset1);
        robotData = CSVReader.Read(textAsset2);
        SoundSetup();
        StartCoroutine(FadeinEffect());
        BattlePanelSetActive();
        SoundManager.instance.PlayBGMSound("bg_Robot", 1f);
    }

    void SoundSetup()
    {
        /*bgmPlayer = GetComponent<AudioSource>();
        bgmPlayer.loop = true;
        bgmPlayer.volume = 0.5f;
        bgmPlayer.playOnAwake = false;
        sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxPlayer.loop = false;
        sfxPlayer.volume = 1f;
        sfxPlayer.playOnAwake = false;*/
    }

    public void GameButtonClickSoundPlay()
    {
        /*sfxPlayer.clip = gameButtonClickSound;
        sfxPlayer.Play();*/
        SoundManager.instance.PlayEffectSound("eff_World_boutton", 1f);
    }

    public void BattlePanelSetActive()  //배틀씬 패널 활성/비활성화
    {
        if (battlePanel.activeSelf) battlePanel.SetActive(false);
        else battlePanel.SetActive(true);
    }

    //public void MainMenuPanelOnSetActive()  //메인메뉴 패널 활성/비활성화
    //{
    //    if (mainMenuPanel.activeSelf) mainMenuPanel.SetActive(false);
    //    else mainMenuPanel.SetActive(true);
    //}

    //David edit 2022-11-08
    public void ExitRobot()  //메인메뉴 화면으로 이동
    {
        //BgmFadeOut();
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

    public void RestartGame()
    {
        StartCoroutine(WaitEffectTime());
    }

    IEnumerator WaitEffectTime()
    {
        yield return new WaitForFixedUpdate();
        yield return StartCoroutine(FadeoutEffect());
        //MainMenuPanelOnSetActive();
        BattlePanelSetActive();
        yield return new WaitForSeconds(1f);
        BattlePanelSetActive();
        //GameSceneOpen();

        /*yield return */
        StartCoroutine(FadeinEffect());
        
    }

    void GameSceneOpen()
    {

        //MainMenuPanelOnSetActive();
        BattlePanelSetActive();
    }


    /*public void BgmFadeOut()
    {
        if (!bgmPlayer.isPlaying) return;
        StartCoroutine(SoundOff());
    }

    IEnumerator SoundOff()
    {
        while(bgmPlayer.volume > 0f)
        {
            bgmPlayer.volume -= 0.005f;
            yield return new WaitForFixedUpdate();
        }

        bgmPlayer.Stop();
    }*/

    /*public void BGMPlay(int _num)
    {
        if (bgmPlayer.isPlaying) bgmPlayer.Stop();
        bgmPlayer.clip = bgmClips[_num];
        bgmPlayer.loop = true;
        //bgmPlayer.volume = 0.7f;
        StartCoroutine(BgmPlaySetup());
    }

    IEnumerator BgmPlaySetup()
    {
        bgmPlayer.Play();
        while (bgmPlayer.volume < 0.5f)
        {
            bgmPlayer.volume += 0.005f;
            yield return new WaitForFixedUpdate();
        }
    }*/

    IEnumerator FadeoutEffect()  //페이드아웃효과
    {
        if (fadeImage.raycastTarget == false) fadeImage.raycastTarget = true;
        Color _color = fadeImage.color;
        _color.a = 0f;
        fadeImage.color = _color;

        while(fadeImage.color.a < 1f)
        {
            _color.a += 0.02f;
            fadeImage.color = _color;
            if (_color.a >= 1f) break;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator FadeinEffect()  //페이드인효과
    {
        Color _color = fadeImage.color;
        _color.a = 1f;
        fadeImage.color = _color;
        while (fadeImage.color.a > 0f)
        {
            _color.a -= 0.02f;
            fadeImage.color = _color;
            yield return new WaitForFixedUpdate();
        }
        fadeImage.raycastTarget = false;
    }
}
