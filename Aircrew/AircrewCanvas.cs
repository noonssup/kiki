using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AircrewCanvas : MonoBehaviour
{
    [Header("게임오브젝트")]
    public GameObject aircrewGamePanel;      //항공승무원 미니게임 팝업창
    public GameObject aircrewGameLoadPanel;  //항공승무원 미니게임 로딩 연출 팝업창
    public GameObject aircrewToonPanel;  //항공승무원 최초 로딩 툰

    public Image fadeinImage;           //페이드인 효과용 이미지
    public TMP_Text flyingText;             //비행기 이륙하는 중...
    public GameObject loadingAirplane;  //로딩화면의 비행기
    public Animator aircrewAnim;        //항공승무원용 애니메이터

    private void Start()
    {
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main;
        this.GetComponent<Canvas>().sortingLayerName = "UI";  //캔버스의 sorting layer 를 UI 로 지정

        Color _color = new Color(0f, 0f, 0f, 0f);
        fadeinImage.color = _color;
        aircrewGamePanel.SetActive(false);
        aircrewToonPanel.SetActive(false);
        aircrewGameLoadPanel.SetActive(false);
        aircrewAnim = loadingAirplane.GetComponent<Animator>();

        StartCoroutine(AircrewToonProcess());
    }

    public void CloseGamePanel()  //미니게임창 닫기
    {
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
    public void RetryGame()  //미니게임 재시작
    {
        SoundManager.instance.PlayBGMSound("bg_Airplane", 1f);
        if (aircrewGamePanel.activeSelf) aircrewGamePanel.SetActive(false);
        StartCoroutine(AircrewToonProcess());
    }
    public void AircrewGameSetup()   //기내비상탈출 게임 로딩 연출 (연출 이후 게임 시작)
    {
        //컨텐츠 선택 버튼 비활성화
        StartCoroutine(AircrewGameLoad());
    }

    IEnumerator AircrewGameLoad()
    {
        flyingText.text = "비행기 이륙하는 중...";
        aircrewGameLoadPanel.SetActive(true);

        yield return new WaitForFixedUpdate();     //비행기가 이동하는 애니메이션 재생
        aircrewAnim.SetTrigger("AirplaneMove");

        yield return new WaitForSeconds(0.5f);

        if (!fadeinImage.gameObject.activeSelf) fadeinImage.gameObject.SetActive(true);
        fadeinImage.enabled = true;
        Color _color = new Color(0f, 0f, 0f, 0f);
        while (fadeinImage.color.a < 1)
        {
            _color.a += Time.deltaTime * 0.5f;
            fadeinImage.color = _color;
            yield return new WaitForFixedUpdate();
        }
        
        aircrewGameLoadPanel.SetActive(false);
        aircrewGamePanel.SetActive(true);
        _color = new Color(0f, 0f, 0f, 1f);  
        fadeinImage.color = _color;
        while(fadeinImage.color.a > 0)
        {
            _color.a -= Time.deltaTime * 0.3f;
            fadeinImage.color = _color;
            yield return new WaitForFixedUpdate();
        }
        SoundManager.instance.PlayBGMSound("bg_Airplane", 1f);
    }

    public void AirplaneDownFunction()
    {
        aircrewGameLoadPanel.SetActive(true);
        StartCoroutine(AirplaneDownAnim());
    }
    public IEnumerator AirplaneDownAnim()
    {
        yield return new WaitForFixedUpdate();
        if (!fadeinImage.gameObject.activeSelf) fadeinImage.gameObject.SetActive(true);
        fadeinImage.enabled = true;
        Color _color = new Color(0f, 0f, 0f, 0f);
        while (fadeinImage.color.a < 1)
        {
            _color.a += Time.deltaTime * 0.75f;
            fadeinImage.color = _color;
            yield return new WaitForFixedUpdate();
        }
        _color.a = 0f;
        fadeinImage.color = _color;
        flyingText.text = "비상선언";
        flyingText.color = new Color(1, 0.4392157f, 0.4392157f, 1f);
        aircrewGameLoadPanel.SetActive(true);

        aircrewAnim.SetTrigger("AirplaneDown");
        float currentAnimTime = aircrewAnim.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSeconds(2.5f);
        _color.a = 0f;
        while (fadeinImage.color.a < 1)
        {
            _color.a += Time.deltaTime * 0.75f;
            fadeinImage.color = _color;
            yield return new WaitForFixedUpdate();
        }
        aircrewGameLoadPanel.SetActive(false);
        _color = new Color(0f, 0f, 0f, 1f);
        fadeinImage.color = _color;
        while (fadeinImage.color.a > 0)
        {
            _color.a -= Time.deltaTime * 0.75f;
            fadeinImage.color = _color;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator AircrewToonProcess()
    {
        aircrewToonPanel.SetActive(true);
        yield return new WaitForSeconds(5f);
        aircrewToonPanel.SetActive(false);
        AircrewGameSetup();
    }
}