using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 스마트팜 게임매니저 스크립트
/// 작물 데이터 설정, 게임시작/종료 설정
/// </summary>

public enum Crops //작물
{
    tomato = 0,
    watermelon,
    koreanmelon,
    melon,
    banana,
    strawberry,
    applemango,
    empty
}

public class SmartFarmGameManager : MonoBehaviour
{
    [Header("오브젝트")]
    public static SmartFarmGameManager instance;
    public SmartFarmGamePanel gameCtrl;
    public GameObject fruitSelectButtonPrefab;
    public GameObject fruitSelectPanel;
    public GameObject mainMenuPanel;
    public GameObject gamePanel;
    public GameObject pausePanel;
    public GameObject helpGuidePanel;
    public Transform cropSelectContentTr;
    public Sprite[] cropSprites;
    public Image fadeImage;

    [Header("게임데이터")]
    public Crops crops = Crops.empty;
    [Tooltip("선택 가능한 작물의 수")]
    public int fruitCount = 1;
    public List<Dictionary<string, object>> data;
    public TextAsset textAsset;

    [Header("텍스트")]
    public TextMeshProUGUI textCropName;  //작물명
    public TextMeshProUGUI textDescriptionCropInfo;  //작물 설명 텍스트
    public TextMeshProUGUI textButtonCropName;   //재배시작 버튼의 작물명 텍스트

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            if (instance != this) Destroy(this.gameObject);
        }
        data = CSVReader.Read(textAsset);
    }


    private void Start()
    {
        gameCtrl = gamePanel.GetComponent<SmartFarmGamePanel>();

        SoundManager.instance.PlayBGMSound("bg_SmartFarm", 1f);

        pausePanel.SetActive(false);
        if (helpGuidePanel.activeSelf) helpGuidePanel.SetActive(false);
        if (!fadeImage.gameObject.activeSelf) fadeImage.gameObject.SetActive(true);
        crops = Crops.empty;
        Color _color = fadeImage.color;
        _color.a = 0f;
        fadeImage.color = _color;
        fadeImage.raycastTarget = false;
        OnGamePanel();
    }

    public void CropMenuPanel()  //작물선택 화면으로 이동
    {
        SoundManager.instance.PlayEffectSound("eff_Common_next", 1f);
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
        crops = Crops.empty;
        gameCtrl.isGamePlay = false;
        StartCoroutine(RestartGame());
    }

    IEnumerator RestartGame()
    {
        Color _color = fadeImage.color;
        fadeImage.raycastTarget = true;
        while (_color.a < 1f)
        {
            _color.a += Time.deltaTime;
            fadeImage.color = _color;
            yield return null;
        }
        fruitSelectPanel.SetActive(false);
        gamePanel.SetActive(false);

        yield return null;

        OnGamePanel();
        while (_color.a > 0f)
        {
            _color.a -= Time.deltaTime;
            fadeImage.color = _color;
            yield return null;
        }
        fadeImage.raycastTarget = false;
    }

    public void OnGamePanel()  //게임화면으로 이동
    {
        gamePanel.SetActive(true);
        if(crops == Crops.empty)
        {
            OnSelectFruitPanel();
        }
    }

    public void OnSelectFruitPanel()  //작물 선택 팝업 활성화
    {
        fruitSelectPanel.SetActive(true);
        textCropName.text = "";
        textDescriptionCropInfo.text = "";
        textButtonCropName.text = "작물을\n선택하세요";
        textButtonCropName.transform.parent.GetComponent<Button>().interactable = false;

        for (int i = 0; i < fruitCount; i++)
        {
            cropSelectContentTr.GetChild(i).GetComponent<Button>().interactable = true;
            cropSelectContentTr.GetChild(i).GetChild(2).gameObject.SetActive(false);
        }


        //if(cropSelectContentTr.childCount > 0)
        //{
        //    for(int i= cropSelectContentTr.childCount - 1; i > -1; i--)
        //    {
        //        Destroy(cropSelectContentTr.GetChild(i).gameObject);
        //    }
        //}

            //for (int i = 0; i < fruitCount; i++)
            //{
            //    var _cropsButton = Instantiate(fruitSelectButtonPrefab).GetComponent<SmartFarmSelectFruit>();
            //    _cropsButton.transform.SetParent(cropSelectContentTr);
            //    _cropsButton.transform.localScale = Vector3.one;
            //    _cropsButton.fruitNumber = i;

            //    _cropsButton.crops = (Crops)i;
            //}
    }

    public void SelectFruit(Crops _crops)  //작물 선택 버튼 클릭 시
    {
        SoundManager.instance.PlayEffectSound("eff_Common_next", 1f);
        crops = _crops;

        string _desc = null;
        string _name = null;
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i]["ID"].ToString() == ((int)crops).ToString())
            {
                _desc = data[i]["desc"].ToString();
                _name = data[i]["name_key"].ToString();
            }
        }

        textCropName.text = _name;
        textDescriptionCropInfo.text = _desc;
        textButtonCropName.text = "재배 시작";
        textButtonCropName.transform.parent.GetComponent<Button>().interactable = true;
    }

    public void StartCultivate() //재배시작
    {
        if (crops == Crops.empty) return;
        SoundManager.instance.PlayEffectSound("eff_Common_next", 1f);
        fruitSelectPanel.SetActive(false);
        gameCtrl.GameStatusSetup((int)crops);
    }

    public void PauseGame()  //일시정지 버튼
    {
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()    //게임재개 버튼
    {
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void OnClickHelpGuideButton()
    {
        helpGuidePanel.SetActive(true);
    }

    public void ExitSmartFarmGame()  //스마트팜 게임에서 나가기
    {
        Time.timeScale = 1f;
        SoundManager.instance.PlayEffectSound("eff_Common_next", 1f);
        SoundManager.instance.StopAllEffSound();
        SoundManager.instance.StopAllSound();
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
