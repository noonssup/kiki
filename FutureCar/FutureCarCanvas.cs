using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;



/// <summary>
/// 321번줄 삭제
/// 62번줄 활성화
/// 192번줄 활성화
/// 221번줄 활성화
/// 238번줄 삭제
/// 332, 357번줄 삭제
/// </summary>

public class FutureCarCanvas : MonoBehaviour
{
    //public JobGameSceneManager sceneManager;

    //public GameObject mainMenuPanel;
    public GameObject mergeGamePanel;
    //public GameObject townMapPanel;
    public GameObject exhibitionPanel;
    public Image fadeImage;

    public List<GameObject> totalItemBlock = new List<GameObject>();

    //[Header("UI")]
    //public Text textJobKindTitle; //전기자동차, 자율주행 자동차 명칭 출력용 텍스트
    //public Text textJobKindDescription; //전기자동차, 자율주행 자동차 설명 출력용 텍스트
    //public Text[] textJobKindButtonTaps;//좌측 직업설명버튼 명칭 출력용 텍스트
    //public Text[] textJobMovieTitle;     //우측 직업소개 영상별 타이틀텍스트
    //public Scrollbar verticalScrollbar;  //스크롤바
    //string[] jobName = { "job00008", "job00009", "job00010", "job00012", "job00013" };
    //string[] jobDescription = { "job90008", "job90009", "job90010", "job90012", "job90013" };
    //string[] jobVideo = { "SUI010401", "SUI010402", "SUI010403", "SUI010525", "SUI010526", "SUI010527" };
    //[SerializeField] Image jobThumbnailImage;
    //[SerializeField] Sprite[] jobThumbnails;

    public List<Dictionary<string, object>> stMain;  //csv 읽기
    public TextAsset textAsset;

    private void Awake()
    {

        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main;
        this.GetComponent<Canvas>().sortingLayerName = "UI";

        //sceneManager = FindObjectOfType<JobGameSceneManager>();

        //stMain = CSVReader.Read("Json/ST_Main_csv");   //StringTable csv 불러오기
        stMain = CSVReader.Read(textAsset);   //StringTable csv 불러오기

        //저장 데이터 로드
        //JobGameDataManager.Instance.LoadGameData("FutureCar");

        ///
        ///씬 연결 시 아래 구문 활성화 시킬 것.
        //DataManager.instance.LoadGameData(GlobalData.Game.FUTURECAR);
        ///

        DataSetup();

        if (SceneManager.GetActiveScene().name.Equals("FutureCar"))
        {
            mergeGamePanel.SetActive(true);
            //if (!exhibitionPanel.activeSelf) exhibitionPanel.SetActive(true);
            StartCoroutine(FadeOutEffect());
        }
        //MainMenuPanelSetup();

        //Invoke("DataSetup", 1f);
        //sfxPlayer = GetComponent<AudioSource>();
    }

    IEnumerator FadeOutEffect(/*Color _color*/)  //페이드아웃효과 + 머지게임 데이터 리셋
    {
        Color _color = fadeImage.color;
        _color.a = 1f;
        fadeImage.color = _color;
        fadeImage.raycastTarget = true;
        yield return new WaitForSeconds(0.2f);
        //mainMenuPanel.SetActive(true);
        if (exhibitionPanel.activeSelf) exhibitionPanel.SetActive(false);
        while (fadeImage.color.a > 0)
        {
            yield return new WaitForFixedUpdate();
            _color.a -= 0.05f;
            fadeImage.color = _color;

            //if(fadeImage.color.a < 0.99f) MainMenuPanelSetup();
        }
        fadeImage.raycastTarget = false;


    }

    private void Start()
    {
    }

    public void TownMapPanelSetup()  //게임에서 나가기
    {
        DataManager.instance.SaveGameData(GlobalData.Game.FUTURECAR);
        SoundManager.instance.StopBGMSound();
        StartCoroutine(ChangeSceneToTownMap());
    }

    IEnumerator ChangeSceneToTownMap()
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>("Utils/ChangeSceneCanvas"));
        yield return gameObject.GetComponent<ChangeSceneManager>().FadeOut(1f);
        GlobalData.nextScene = "TownMap";
        SceneManager.LoadScene("TownMap");
    }

    public void ToMergeGame()  //게임에서 나가기
    {
        DataManager.instance.SaveGameData(GlobalData.Game.FUTURECAR);

        SoundManager.instance.StopBGMSound();
        StartCoroutine(ChangeSceneToMergeGame());
    }

    IEnumerator ChangeSceneToMergeGame()
    {
        GameObject gameObject = Instantiate(Resources.Load<GameObject>("Utils/ChangeSceneCanvas"));
        yield return gameObject.GetComponent<ChangeSceneManager>().FadeOut(1f);
        GlobalData.nextScene = "FutureCar";
        SceneManager.LoadScene("FutureCar");
    }

    public void MergeGamePanelSetup()  //합성화면(전기자동차조립)으로 이동
    {
        //DataManager.instance.SaveGameData(GlobalData.Game.FUTURECAR);

        /*sfxPlayer.clip = gameButtonClickSound;
        sfxPlayer.Play();*/

        StartCoroutine(FadeOutEffect(/*_color*/));
        mergeGamePanel.SetActive(true);
        mergeGamePanel.GetComponent<FutureCarGameCtrl>()._gamePoint = gamePoint;
        //if (townMapPanel.activeSelf) townMapPanel.SetActive(false);
        //if (mainMenuPanel.activeSelf) mainMenuPanel.SetActive(false);
    }

    public void ExhibitionPanel()  //전시판매장 팝업
    {
        exhibitionPanel.SetActive(!exhibitionPanel.activeSelf);
    }

    public void ExitFutureCarGame()  //게임에서 나가기
    {
        DataManager.instance.SaveGameData(GlobalData.Game.FUTURECAR);
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

    ///게임 데이터 저장
    
    public int gamePoint = 0;

    //합성 데이터
    //차고 위치/아이템아이디
    public List<string> garagePosName = new List<string>();
    public List<string> garageItemBlockId = new List<string>();

    //생산 머지보드
    public List<string> devPosName = new List<string>();    //아이템블럭의 위치
    public List<string> devItemBlockId = new List<string>();//아이템블럭의 itemId
    public int devGenerateCount = 30;  //부품 생산 가능 횟수
    public string lastDevTimer;  //부품 생산 횟수 회복용 타이머
    public int devLevel = 0;
    public int devExp = 0;

    //연구 머지보드
    public List<string> labPosName = new List<string>();
    public List<string> labItemBlockId = new List<string>();
    public int labGenerateCount = 30;
    public string lastLabTimer;
    public int labLevel = 0;
    public int labExp = 0;

    //마을 데이터
    public List<string> buildingList = new List<string>();


    void DataSetup()  //저장되어있는 데이터 불러오기
    {
        //JobGameDataManager _dataManager = DataManager.Instance.GetComponent<JobGameDataManager>();
        gamePoint = DataManager.instance.futureCarGameData.gamePoint;
        garagePosName = new List<string>(DataManager.instance.futureCarGameData.garagePosName);
        garageItemBlockId = new List<string>(DataManager.instance.futureCarGameData.garageItemBlockId);
        devPosName = new List<string>(DataManager.instance.futureCarGameData.devPosName);
        devItemBlockId = new List<string>(DataManager.instance.futureCarGameData.devItemBlockId);
        devGenerateCount = DataManager.instance.futureCarGameData.devGenerateCount;
        devLevel = DataManager.instance.futureCarGameData.devLevel;
        devExp = DataManager.instance.futureCarGameData.devExp;
        lastDevTimer = DataManager.instance.futureCarGameData.lastDevTimer;
        labPosName = new List<string>(DataManager.instance.futureCarGameData.labPosName);
        labItemBlockId = new List<string>(DataManager.instance.futureCarGameData.labItemBlockId);
        labGenerateCount = DataManager.instance.futureCarGameData.labGenerateCount;
        labLevel = DataManager.instance.futureCarGameData.labLevel;
        labExp = DataManager.instance.futureCarGameData.labExp;
        lastLabTimer = DataManager.instance.futureCarGameData.lastLabTimer;

        buildingList = new List<string>(DataManager.instance.futureCarGameData.buildingList);
    }

    bool isCheckHelpPanel = false;
    public void MergeDataSave()  //머지 게임 정보 저장
    {
        //JobGameDataManager _dataManager = DataManager.Instance.GetComponent<JobGameDataManager>();

        //머지 게임 정보 저장
        garagePosName = new List<string>();            //차고에 저장된 아이템이 있을 경우, 아이템이 존재하는 타일의 이름 저장
        garageItemBlockId = new List<string>();        //차고에 저장된 아이템의 이름 저장
        devPosName = new List<string>();               //생산 보드 테이블에 아이템에 있을 경우, 아이템이 위치한 타일의 이름 저장
        devItemBlockId = new List<string>();           //생산 보드 테이블에 있는 아이템의 이름 저장
        labPosName = new List<string>();               //연구 보드 테이블에 아이템에 있을 경우, 아이템이 위치한 타일의 이름 저장
        labItemBlockId = new List<string>();           //연구 보드 테이블에 있는 아이템의 이름 저장
        lastDevTimer = System.DateTime.Now.ToString(); //생산 블럭 생성 타이머용, 시간 저장
        lastLabTimer = System.DateTime.Now.ToString(); //연구 블럭 생성 타이머용, 시간 저장
        
        for (int i = 0; i < totalItemBlock.Count; i++)
        {
            if (totalItemBlock[i].transform.parent.transform.parent.name == "Content")
            {
                garagePosName.Add(totalItemBlock[i].transform.parent.name);
                garageItemBlockId.Add(totalItemBlock[i].name);
            }

            if (totalItemBlock[i].transform.parent.transform.parent.name == "DevGameBoard")
            {
                devPosName.Add(totalItemBlock[i].transform.parent.name);

                devItemBlockId.Add(totalItemBlock[i].name);

            }

            if (totalItemBlock[i].transform.parent.transform.parent.name == "LabGameBoard")
            {
                labPosName.Add(totalItemBlock[i].transform.parent.name);
                labItemBlockId.Add(totalItemBlock[i].name);
            }
        }

        DataManager.instance.futureCarGameData.gamePoint = gamePoint;
        DataManager.instance.futureCarGameData.garagePosName = garagePosName;
        DataManager.instance.futureCarGameData.garageItemBlockId = garageItemBlockId;
        DataManager.instance.futureCarGameData.devPosName = devPosName;
        DataManager.instance.futureCarGameData.devItemBlockId = devItemBlockId;
        DataManager.instance.futureCarGameData.devGenerateCount = devGenerateCount;
        DataManager.instance.futureCarGameData.devLevel = devLevel;
        DataManager.instance.futureCarGameData.devExp = devExp;
        DataManager.instance.futureCarGameData.lastDevTimer = lastDevTimer;
        
        DataManager.instance.futureCarGameData.labPosName = labPosName;
        DataManager.instance.futureCarGameData.labItemBlockId = labItemBlockId;
        DataManager.instance.futureCarGameData.labGenerateCount = labGenerateCount;
        DataManager.instance.futureCarGameData.labLevel = labLevel;
        DataManager.instance.futureCarGameData.labExp = labExp;
        DataManager.instance.futureCarGameData.lastLabTimer = lastLabTimer;

        //JobGameDataManager.Instance.SaveGameData("FutureCar");
    }
    public void TownDataSave()  //마을 정보 저장
    {        
        //마을 정보 저장 (건설 완료된 건물의 이름을 buildingList 에 담아 저장)
        //buildingList = new List<string>();
        //buildingList = townMapPanel.GetComponent<FutureCarTownCtrl>().buildingList;

        DataManager.instance.futureCarGameData.gamePoint = gamePoint;
        DataManager.instance.futureCarGameData.buildingList = buildingList;

        //JobGameDataManager.Instance.SaveGameData("FutureCar");
    }


    private void Update()
    {

#if UNITY_EDITOR
        //테스트용 조작 (R 키 조작 시 저장된 데이터 삭제 / 유니티 에디터에서만 동작)
        //if (Input.GetKeyDown(KeyCode.R)) ResetGameData();
#endif
    }

    void ResetGameData()  //저장데이터 초기화
    {
        DataManager.instance.futureCarGameData.garagePosName = new List<string>();
        DataManager.instance.futureCarGameData.garageItemBlockId = new List<string>();
        DataManager.instance.futureCarGameData.devPosName = new List<string>();
        DataManager.instance.futureCarGameData.devItemBlockId = new List<string>();
        DataManager.instance.futureCarGameData.devGenerateCount = 30;
        DataManager.instance.futureCarGameData.devLevel = 0;
        DataManager.instance.futureCarGameData.devExp = 0;
        DataManager.instance.futureCarGameData.lastDevTimer = null;
        DataManager.instance.futureCarGameData.labPosName = new List<string>();
        DataManager.instance.futureCarGameData.labItemBlockId = new List<string>();
        DataManager.instance.futureCarGameData.labGenerateCount = 30;
        DataManager.instance.futureCarGameData.labLevel = 0;
        DataManager.instance.futureCarGameData.labExp = 0;
        DataManager.instance.futureCarGameData.lastLabTimer = null;
        DataManager.instance.futureCarGameData.buildingList = new List<string>();
        DataManager.instance.futureCarGameData.gamePoint = 0;


        DataManager.instance.SaveGameData(GlobalData.Game.FUTURECAR);
    }

}
