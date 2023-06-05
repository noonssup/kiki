using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FutureCarGameCtrl : MonoBehaviour
{
    public FutureCarCanvas sceneCtrl;
    public GameObject itemBlockPrefab;

    [Header("생산 보드")]
    public GameObject devPanel;   //생산 게임보드
    public List<Transform> devBlockTile; //생산 보드 타일 트랜스폼
    /*public*/ int maxDevGenerateCount;
    /*public*/ int devGenerateCount = 60;
    public TextMeshProUGUI textDevGenerateCount;
    public Image devGauge;
    public float devGenerateTimer = 30f;
    public string lastDevTime;
    public TextMeshProUGUI textDevProductName;
    public TextMeshProUGUI textDevDescription;
    public Image devProductIcon;
    public TextMeshProUGUI textDevLevel;
    public TextMeshProUGUI textDevExp;
    public Image devExpGauge;
    public Image devFactoryImage;

    [Header("연구 보드")]
    public GameObject labPanel;   //연구 게임보드
    public List<Transform> labBlockTile; //연구 보드 타일 트랜스폼
    /*public*/ int maxLabGenerateCount;
    /*public*/ int labGenerateCount = 60;
    public TextMeshProUGUI textLabGenerateCount;
    public Image labGauge;
    public float labGenerateTimer = 30f;
    public string lastLabTime;
    public TextMeshProUGUI textLabProductName;
    public TextMeshProUGUI textLabDescription;
    public Image labProductIcon;
    public TextMeshProUGUI textLabLevel;
    public TextMeshProUGUI textLabExp;
    public Image labExpGauge;
    public Image labFactoryImage;

    [Header("차고")]
    public GameObject garagePanel;//차고 패널
    public GameObject garageBox;  //차고 오브젝트
    public List<Transform> _stockTileList;
    public List<Transform> stockBlock;

    [Header("도움말팝업")]
    [SerializeField] GameObject helpPanel;
    [SerializeField] GameObject[] helpPage;
    [SerializeField] GameObject techTreePanel;
    [SerializeField] Button[] pageButton;
    public List<Sprite> itemSprites; //퍼즐아이템이미지

    //데이터 항목에는 활성화된 아이템이 위치한 자리의 이름과 아이템명을 저장
    //씬 로드 시 위치명과 아이템명을 확인하여 마지막에 저장된 위치에 저장된 아이템을 배치하도록 한다
    [Header("데이터")]
    public int gamePoint = 0;         //게임 포인트
    public int _gamePoint
    {
        get { return gamePoint; }
        set { gamePoint = sceneCtrl.gamePoint;
            textGamePoint.text = gamePoint.ToString("#,##0");
        }
    }
    public int devExp;
    public int _devExp
    {
        get { return devExp; }
        set
        {
            if (devLevel >= 50) return;
            devExp += value;
            textDevExp.text = "Exp : " + value.ToString();
            StartCoroutine(ExpEffect(textDevExp));
            if (devExp >= devNextExp)
            {
                devLevel++;
                if (devLevel >= 50) textDevLevel.text = "Lv. MAX";
                else textDevLevel.text = "Lv. " + devLevel.ToString();

                devExp = 0;
                for(int i = 0; i < levelData.Count; i++)
                {
                    if(levelData[i]["Lv"].ToString() == devLevel.ToString())
                    {
                        devNextExp = int.Parse(levelData[i]["Next_exp"].ToString());
                        maxDevGenerateCount = int.Parse(levelData[i]["Max_Prod"].ToString());
                        devGenerateCount = maxDevGenerateCount;
                        
                        break;
                    }
                }
            }
            textDevGenerateCount.text = devGenerateCount.ToString() + "/" + maxDevGenerateCount.ToString();
            devExpGauge.fillAmount = ((float)devExp / (float)devNextExp)*0.77f;
            BlockListSave();            
        }
    }
    public int devNextExp;
    public int devLevel = 1;
    public int labExp;
    public int _labExp
    {
        get { return labExp; }
        set
        {
            if (labLevel >= 50) return;
            labExp += value;
            textLabExp.text = "Exp : " + value.ToString();
            StartCoroutine(ExpEffect(textLabExp));
            if (labExp >= labNextExp)
            {
                labLevel++;
                if (labLevel >= 50) textLabLevel.text = "Lv. MAX";
                else textLabLevel.text = "Lv. " + labLevel.ToString();

                labExp = 0;
                for (int i = 0; i < levelData.Count; i++)
                {
                    if (levelData[i]["Lv"].ToString() == labLevel.ToString())
                    {
                        labNextExp = int.Parse(levelData[i]["Next_exp"].ToString());
                        maxLabGenerateCount = int.Parse(levelData[i]["Max_Prod"].ToString());
                        labGenerateCount = maxLabGenerateCount;
                        break;
                    }
                }
            }
            textLabGenerateCount.text = labGenerateCount.ToString() + "/" + maxLabGenerateCount.ToString();
            labExpGauge.fillAmount = ((float)labExp / (float)labNextExp)*0.77f;
            BlockListSave();
        }
    }
    public int labNextExp;
    public int labLevel = 1;
    [SerializeField] List<string> devPosName = new List<string>();
    [SerializeField] List<string> labPosName = new List<string>();
    [SerializeField] List<string> devItemId = new List<string>();
    [SerializeField] List<string> labItemId = new List<string>();
    [SerializeField] List<string> garagePosName = new List<string>();
    [SerializeField] List<string> garageItemId = new List<string>();
    [SerializeField] List<Dictionary<string, object>> levelData;  //csv 읽기
    public TextAsset textAsset;
    string soundName = "eff_Common_next";

    [Header("텍스트")]
    [SerializeField] TextMeshProUGUI textHelpPopupPage;
    public TextMeshProUGUI textGamePoint;    //게임 포인트를 표시할 텍스트


    public void PlaySoundEffect(string _sound)
    {
        /*switch (_sound)
        {
            case "Factory": if(!sfxPlayer.isPlaying) sfxPlayer.PlayOneShot(factorySound); break;
        }*/
        SoundManager.instance.PlayEffectSound(_sound, 1f);
    }

    IEnumerator ExpEffect(TextMeshProUGUI _text)   //생산/연구 경험치 획득 연출
    {
        _text.enabled = true;
        Vector2 _resetPos = _text.transform.localPosition;
        Vector2 _targetPos = new Vector2(_text.transform.localPosition.x, _text.transform.localPosition.y + 150f);
        Color _color = _text.color;
        _color.a = 1f;
        _text.color = _color;
        yield return new WaitForFixedUpdate();

        while(_text.color.a > 0)
        {
            yield return new WaitForFixedUpdate();
            _color.a -= 0.05f;
            _text.transform.localPosition = Vector2.MoveTowards(_text.transform.localPosition, _targetPos, 50f * Time.deltaTime);
            _text.color = _color;
        }

        _text.transform.localPosition = _resetPos;
        _text.enabled = false;
    }

    private void Awake()
    {
        sceneCtrl = FindObjectOfType<FutureCarCanvas>();
        stockBlock = new List<Transform>();
        //levelData = CSVReader.Read("gMiniGame/FutureCar/Merge_facility_lv_csv");
        levelData = CSVReader.Read(textAsset);
        AudioSourceSetup();
        GenerateTimer();
    }

    void AudioSourceSetup()
    {
        SoundManager.instance.PlayBGMSound("bg_FutureCar", 1f);
    }

    private void Start()
    {
        BlockTileSetup();
        FactoryLevelSetup();
    }

    void FactoryLevelSetup()  //시설 레벨 셋업
    {
        if (sceneCtrl.devLevel == 0)
        {
            devLevel = 1;
            for (int i = 0; i < levelData.Count; i++)
            {
                if (levelData[i]["Lv"].ToString() == devLevel.ToString())
                {
                    devNextExp = int.Parse(levelData[i]["Next_exp"].ToString());
                    devExp = sceneCtrl.devExp;
                    devExpGauge.fillAmount = (devExp / devNextExp)*0.77f;

                    maxDevGenerateCount = int.Parse(levelData[i]["Max_Prod"].ToString());
                    devGenerateCount = maxDevGenerateCount;
                    break;
                }
            }
        }
        else
        {
            devLevel = sceneCtrl.devLevel;
            for (int i = 0; i < levelData.Count; i++)
            {
                if (levelData[i]["Lv"].ToString() == devLevel.ToString())
                {
                    devNextExp = int.Parse(levelData[i]["Next_exp"].ToString());
                    devExp = sceneCtrl.devExp;
                    devExpGauge.fillAmount = (devExp / devNextExp)*0.77f;

                    maxDevGenerateCount = int.Parse(levelData[i]["Max_Prod"].ToString());
                    devGenerateCount = maxDevGenerateCount;
                    break;
                }
            }
            devExp = sceneCtrl.devExp;
            devGenerateCount = sceneCtrl.devGenerateCount;
        }
        if (sceneCtrl.labLevel == 0)
        {
            labLevel = 1;
            for (int i = 0; i < levelData.Count; i++)
            {
                if (levelData[i]["Lv"].ToString() == devLevel.ToString())
                {
                    labNextExp = int.Parse(levelData[i]["Next_exp"].ToString());
                    labExp = sceneCtrl.labExp;
                    labExpGauge.fillAmount = (labExp / labNextExp)*0.77f;

                    maxLabGenerateCount = int.Parse(levelData[i]["Max_Prod"].ToString());
                    labGenerateCount = maxLabGenerateCount;
                    break;
                }
            }

        }
        else
        {
            labLevel = sceneCtrl.labLevel;
            for (int i = 0; i < levelData.Count; i++)
            {
                if (levelData[i]["Lv"].ToString() == labLevel.ToString())
                {
                    labNextExp = int.Parse(levelData[i]["Next_exp"].ToString());
                    labExp = sceneCtrl.labExp;
                    labExpGauge.fillAmount = (labExp / labNextExp)*0.77f;

                    maxLabGenerateCount = int.Parse(levelData[i]["Max_Prod"].ToString());
                    labGenerateCount = maxLabGenerateCount;
                    break;
                }
            }
            labExp = sceneCtrl.labExp;
            labGenerateCount = sceneCtrl.labGenerateCount;

        }

        textDevLevel.text = "Lv. " + devLevel.ToString();
        textLabLevel.text = "Lv. " + labLevel.ToString();
        labExpGauge.fillAmount = ((float)labExp / (float)labNextExp)*0.77f;
        devExpGauge.fillAmount = ((float)devExp / (float)devNextExp)*0.77f;
        GameSetup();
    }

    private void OnEnable()
    {
        //gamePoint = sceneCtrl.transform.GetComponent<JobGameData>().gamePoint;

        GameSetup();

        ////BlockTileSetup();
    }

    int pageNo = 0;
    public void HelpPopup()  //도움말
    {
        GameObject _button = EventSystem.current.currentSelectedGameObject;
        SoundManager.instance.PlayEffectSound(soundName, 1f);
        switch (_button.name)
        {
            case "TechTreeButton":
                if (!techTreePanel.activeSelf) techTreePanel.SetActive(true);  break;
            case "HelpButton":
                if (!helpPanel.activeSelf) helpPanel.SetActive(true); pageNo = 1; textHelpPopupPage.text = "1 / 4"; pageButton[0].gameObject.SetActive(false);pageButton[1].gameObject.SetActive(true);
                foreach (GameObject popupPage in helpPage)
                {
                    if ("HelpImage" + (pageNo).ToString() == popupPage.name)
                    {
                        popupPage.SetActive(true);
                    }
                    else popupPage.SetActive(false);
                }
                break;
            default: if(helpPanel.activeSelf) helpPanel.SetActive(false);
                if (techTreePanel.activeSelf) techTreePanel.SetActive(false);
                break;
        }
    }

    public void HelpPopupChangePage()  //도움말 페이지 변경
    {
        GameObject _button = EventSystem.current.currentSelectedGameObject;
        SoundManager.instance.PlayEffectSound(soundName, 1f);
        switch (_button.name)
        {
            case "PageLeftButton": pageNo--; break;
            case "PageRightButton": pageNo++; break;
        }

        if (pageNo <= 1)
        {
            pageNo = 1;
            textHelpPopupPage.text = "1 / 4";
            pageButton[0].gameObject.SetActive(false);
            pageButton[1].gameObject.SetActive(true);
        }
        if (pageNo == 2 /*|| pageNo == 3*/)
        {
           if(pageNo ==2) textHelpPopupPage.text = "2 / 4";
           //if(pageNo ==3) textHelpPopupPage.text = "3 / 3";
            pageButton[0].gameObject.SetActive(true);
            pageButton[1].gameObject.SetActive(true);
        }
        if (pageNo == 3)
        {
            //pageNo = 3;
            textHelpPopupPage.text = "3 / 4";
            pageButton[0].gameObject.SetActive(true);
            pageButton[1].gameObject.SetActive(true);
        }
        if (pageNo >= 4)
        {
            pageNo = 4;
            textHelpPopupPage.text = "4 / 4";
            pageButton[0].gameObject.SetActive(true);
            pageButton[1].gameObject.SetActive(false);
        }

        foreach (GameObject popupPage in helpPage)
        {
            if("HelpImage"+(pageNo).ToString() == popupPage.name)
            {
                popupPage.SetActive(true);
            } else popupPage.SetActive(false);
        }
    }

    int isCheckHelpPanel = 0;

    void GameSetup()  //게임셋업 (게임포인트, 부품생성카운트)
    {
        if (PlayerPrefs.HasKey("helpPanelCheck")) isCheckHelpPanel = PlayerPrefs.GetInt("helpPanelCheck");
        else isCheckHelpPanel =0;
        
        if (isCheckHelpPanel == 0)
        {
            helpPanel.SetActive(true);
            pageNo = 1; textHelpPopupPage.text = "1 / 3"; pageButton[0].gameObject.SetActive(false);
            isCheckHelpPanel = 1;
            foreach (GameObject popupPage in helpPage)
            {
                if ("HelpImage" + (pageNo).ToString() == popupPage.name)
                {
                    popupPage.SetActive(true);
                }
                else popupPage.SetActive(false);
            }
            PlayerPrefs.SetInt("helpPanelCheck", isCheckHelpPanel);
        }else helpPanel.SetActive(false);
        techTreePanel.SetActive(false);

        textDevExp.enabled = false;
        textLabExp.enabled = false;

        gamePoint = sceneCtrl.gamePoint;
        textGamePoint.text = gamePoint.ToString("#,##0");

        //연구 부품 생성 카운트 셋업
        if (sceneCtrl.labGenerateCount >= maxLabGenerateCount)
        {
            labGenerateCount = maxLabGenerateCount; labGenerateTimer = 30f;
        }
        else
        {
            labGenerateCount = sceneCtrl.labGenerateCount;
            if (sceneCtrl.lastLabTimer != "")// null)
            {
                //Debug.Log("라스트랩타이머 : " + sceneCtrl.lastLabTimer);
                string lastLabTime = sceneCtrl.lastLabTimer;
                System.DateTime _lastLabDateTime = System.DateTime.Parse(lastLabTime);
                System.TimeSpan _labConpareTime = System.DateTime.Now - _lastLabDateTime;

                float _labThroughTime = (float)_labConpareTime.TotalSeconds;
                while (_labThroughTime >= 30f)
                {
                    _labThroughTime -= 30f;
                    labGenerateCount++;
                    if (labGenerateCount >= maxLabGenerateCount)
                    {
                        labGenerateCount = maxLabGenerateCount;
                        _labThroughTime = _labThroughTime % 30;
                        break;
                    }
                }
                if (_labThroughTime != 0) labGenerateTimer = _labThroughTime;
                else labGenerateTimer = 30f;
            }

        }


        //생산 부품 생성 카운트 셋업
        if (sceneCtrl.devGenerateCount >= maxDevGenerateCount)
        {
            devGenerateCount = maxDevGenerateCount; devGenerateTimer = 30f;
        }
        else
        {
            devGenerateCount = sceneCtrl.devGenerateCount;
            if (sceneCtrl.lastDevTimer !="")// null)
            {
                //Debug.Log("라스트데브타이머 : " + sceneCtrl.lastDevTimer);
                string lastDevTime = sceneCtrl.lastDevTimer;
                System.DateTime _lastDevDateTime = System.DateTime.Parse(lastDevTime);
                System.TimeSpan _devConpareTime = System.DateTime.Now - _lastDevDateTime;

                float _devThroughTime = (float)_devConpareTime.TotalSeconds;
                while (_devThroughTime >= 30f)
                {
                    _devThroughTime -= 30f;

                    devGenerateCount++;
                    if (devGenerateCount >= maxDevGenerateCount)
                    {
                        devGenerateCount = maxDevGenerateCount;
                        _devThroughTime = _devThroughTime % 30;
                        break;
                    }
                }
                if (_devThroughTime != 0) devGenerateTimer = _devThroughTime;
                else devGenerateTimer = 30f;
            }
        }

        textDevGenerateCount.text = devGenerateCount.ToString() + "/" + maxDevGenerateCount.ToString();
        textLabGenerateCount.text = labGenerateCount.ToString() + "/" + maxLabGenerateCount.ToString();
    }


    private void Update()
    {
        GenerateTimer();


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.R))
        {
            if(devGenerateCount < maxDevGenerateCount)  devGenerateCount = maxDevGenerateCount;
            if(labGenerateCount < maxLabGenerateCount) labGenerateCount = maxLabGenerateCount;
            devGenerateTimer = 30f;
            devGauge.fillAmount = devGenerateTimer / 30;
            labGenerateTimer = 30f;
            labGauge.fillAmount = labGenerateTimer / 30;
        }


        if (Input.GetKeyDown(KeyCode.D))  //부품 블럭 생성
        {
            BlockGenerateTest("dev");
        }

        if (Input.GetKeyDown(KeyCode.L))  //연구 블럭 생성
        {
            BlockGenerateTest("lab");
        }

#endif
    }

    #region 테스트용 블럭 자동 생성 함수
    //바로 카운트만큼 블럭을 생성 시키면 앱이 멈추는 현상이 발생하였다
    //블럭 생성 사이사이 간격을 둬야 문제없을 듯... (코루틴으로 테스트)... 근데 계속 자동생성된다... 왜 멈추지를 않는것이냐....
    void BlockGenerateTest(string _str)
    {
        int _count = 0;
        switch (_str)
        {
            case "lab": _count = labGenerateCount; break;
            case "dev": _count = devGenerateCount; break;
        }
        StartCoroutine(GenerateDelay(_count, _str));
    }

    IEnumerator GenerateDelay(int _count, string _str)
    {
        while (_count > 0)
        {
            GenerateItemBlock(_str);
            _count--;
            yield return new WaitForSeconds(0.2f);
        }

    }

    #endregion

    void GenerateTimer()  //블럭 생성 타이머 설정
    {
        if (maxDevGenerateCount > devGenerateCount)
        {
            if (devGenerateTimer > 0)
            {
                devGenerateTimer -= Time.deltaTime;
                devGauge.fillAmount = devGenerateTimer / 30;
            }
            else
            {
                devGenerateCount++;
                textDevGenerateCount.text = devGenerateCount.ToString() + "/" + maxDevGenerateCount.ToString();
                devGenerateTimer = 30f;
                devGauge.fillAmount = devGenerateTimer / 30;
            }

        }
        else
        {
            textDevGenerateCount.text = devGenerateCount.ToString() + "/" + maxDevGenerateCount.ToString();
            devGenerateTimer = 30f;
            devGauge.fillAmount = devGenerateTimer / 30;
        }

        if (maxLabGenerateCount > labGenerateCount)
        {
            if (labGenerateTimer > 0)
            {
                labGenerateTimer -= Time.deltaTime;
                labGauge.fillAmount = labGenerateTimer / 30;
            }
            else
            {
                labGenerateCount++;
                textLabGenerateCount.text = labGenerateCount.ToString() + "/" + maxLabGenerateCount.ToString();
                labGenerateTimer = 30f;
                labGauge.fillAmount = labGenerateTimer / 30;
            }
        }
        else
        {
            textLabGenerateCount.text = labGenerateCount.ToString() + "/" + maxLabGenerateCount.ToString();
            labGenerateTimer = 30f;
            labGauge.fillAmount = labGenerateTimer / 30;
        }
    }

    void BlockTileSetup()  //블럭타일 정보 셋업
    {        
        devPosName = new List<string>(); 
        labPosName = new List<string>();
        devItemId = new List<string>();
        labItemId = new List<string>();
        garagePosName = new List<string>();
        garageItemId = new List<string>();

        devPosName = sceneCtrl.devPosName;
        labPosName = sceneCtrl.labPosName;
        devItemId = sceneCtrl.devItemBlockId;
        labItemId = sceneCtrl.labItemBlockId;
        garagePosName = sceneCtrl.garagePosName;
        garageItemId = sceneCtrl.garageItemBlockId;


        //게임보드 하위 오브젝트 54개 blockTile 리스트에 저장
        //게임 종료 및 변동사항 발생 시 각 타일의 번호와 자식오브젝트의 item_id 저장을 위해 설정

        //생산 보드 셋업
        var _gameBoard = devPanel.transform.Find("DevGameBoard").gameObject;
        if (_gameBoard != null)
        {
            _gameBoard.transform.GetComponentsInChildren<Transform>(devBlockTile);
            devBlockTile.RemoveAt(0);
            for (int i = 0; i < devBlockTile.Count; i++)
            {
                devBlockTile[i].name = i.ToString("000");
                devBlockTile[i].GetComponent<Image>().enabled = false;
                if (devPosName.Contains(devBlockTile[i].name))
                {
                    GameObject _itemBlock = Instantiate(itemBlockPrefab, transform.position, Quaternion.identity);
                    _itemBlock.transform.SetParent(devBlockTile[i].transform);
                    _itemBlock.GetComponent<FutureCarItemBlock>().tempItemId = devItemId[devPosName.IndexOf(devBlockTile[i].name)];
                    _itemBlock.transform.localPosition = Vector3.zero;
                    _itemBlock.transform.localScale = Vector3.one;
                }
            }
        }


        //연구 보드 셋업
        _gameBoard = labPanel.transform.Find("LabGameBoard").gameObject;
        if (_gameBoard != null)
        {
            _gameBoard.transform.GetComponentsInChildren<Transform>(labBlockTile);
            labBlockTile.RemoveAt(0);
            for (int i = 0; i < devBlockTile.Count; i++)
            {
                labBlockTile[i].name =  i.ToString("000");
                labBlockTile[i].GetComponent<Image>().enabled = false;
                if (labPosName.Contains(labBlockTile[i].name))
                {
                    GameObject _itemBlock = Instantiate(itemBlockPrefab, transform.position, Quaternion.identity);
                    _itemBlock.transform.SetParent(labBlockTile[i].transform);
                    _itemBlock.GetComponent<FutureCarItemBlock>().tempItemId = labItemId[labPosName.IndexOf(labBlockTile[i].name)];
                    _itemBlock.transform.localPosition = Vector3.zero;
                    _itemBlock.transform.localScale = Vector3.one;
                }
            }
        }

        //차고 셋업
        _stockTileList = new List<Transform>();
        garageBox.transform.GetComponentsInChildren<Transform>(_stockTileList);
        _stockTileList.RemoveAt(0);
        for (int i = 0; i < _stockTileList.Count; i++)
        {
            _stockTileList[i].name = i.ToString("000");
        }
        int _idx = 0;
        for (int i = 0; i < _stockTileList.Count; i++)
        {
            if (garagePosName.Contains(_stockTileList[i].name))
            {
                GameObject _itemBlock = Instantiate(itemBlockPrefab, transform.position, Quaternion.identity);
                _itemBlock.transform.SetParent(_stockTileList[i].transform);
                
                _itemBlock.GetComponent<FutureCarItemBlock>().tempItemId = garageItemId[_idx];_idx++;
                //_itemBlock.name = garageItemId[i];
                _itemBlock.transform.localPosition = Vector3.zero;
                _itemBlock.transform.localScale = Vector3.one;

                // sceneCtrl.totalItemBlock.Add(_itemBlock);

                stockBlock.Add(_itemBlock.transform);
            }
        }
    }

    public void BlockListSave()  //보드의 블록 정보 저장
    {
        devPosName = new List<string>();
        devItemId = new List<string>();
        foreach(Transform _tile in devBlockTile)
        {
            if(_tile.childCount != 0)
            {
                devPosName.Add(_tile.name);
                devItemId.Add(_tile.GetChild(0).name);
            }
        }
        labPosName = new List<string>();
        labItemId = new List<string>();
        foreach (Transform _tile in labBlockTile)
        {
            if (_tile.childCount != 0)
            {
                labPosName.Add(_tile.name);
                labItemId.Add(_tile.GetChild(0).name);
            }
        }
        garagePosName = new List<string>();
        garageItemId = new List<string>();
        foreach (Transform _tile in _stockTileList)
        {
            if (_tile.childCount != 0)
            {
                garagePosName.Add(_tile.name);
                garageItemId.Add(_tile.GetChild(0).name);
            }
        }
        sceneCtrl.devPosName = new List<string>();
        sceneCtrl.labPosName = new List<string>();
        sceneCtrl.devItemBlockId = new List<string>();
        sceneCtrl.labItemBlockId = new List<string>();
        sceneCtrl.garagePosName = new List<string>();
        sceneCtrl.garageItemBlockId = new List<string>();

        sceneCtrl.devPosName = devPosName;
        sceneCtrl.labPosName = labPosName;
        sceneCtrl.devItemBlockId = devItemId;
        sceneCtrl.labItemBlockId = labItemId;
        sceneCtrl.garagePosName = garagePosName;
        sceneCtrl.garageItemBlockId = garageItemId;

        sceneCtrl.devGenerateCount = devGenerateCount;
        sceneCtrl.labGenerateCount = labGenerateCount;
        lastDevTime = System.DateTime.Now.ToString();
        lastLabTime = System.DateTime.Now.ToString();
        sceneCtrl.lastDevTimer = lastDevTime;
        sceneCtrl.lastLabTimer = lastLabTime;

        sceneCtrl.devLevel = devLevel;
        sceneCtrl.labLevel = labLevel;
        sceneCtrl.devExp = devExp;
        sceneCtrl.labExp = labExp;

        sceneCtrl.MergeDataSave();
    }

    public void CreateItemBlock()   //부품 생성 버튼
    {
        GameObject clickButton = EventSystem.current.currentSelectedGameObject;
        switch (clickButton.name)
        {
            case "DevItemGenerateButton": GenerateItemBlock("dev"); break;
            case "LabItemGenerateButton": GenerateItemBlock("lab"); break;
        }
    }

    void GenerateItemBlock(string _part)  //부품 생성
    {
        List<int> emptyPosNo = new List<int>();
        switch (_part)
        {
            case "dev":
                if (devGenerateCount <= 0) return;
                for (int i = 0; i < devBlockTile.Count; i++)
                {
                    if (devBlockTile[i].childCount != 0) continue;
                    else emptyPosNo.Add(i);
                }
                if (emptyPosNo.Count > 0)
                {
                    int rand = Random.Range(emptyPosNo.Count - emptyPosNo.Count, emptyPosNo.Count);

                    devGenerateCount--;
                    textDevGenerateCount.text = devGenerateCount.ToString() + "/" + maxDevGenerateCount.ToString();

                    int randomId = Random.Range(1, 5);
                    GameObject _block = Instantiate(itemBlockPrefab, devBlockTile[emptyPosNo[rand]].position, Quaternion.identity);
                    _block.transform.SetParent(devBlockTile[emptyPosNo[rand]]);
                    _block.transform.localScale = Vector3.one;
                    _block.transform.GetComponent<FutureCarItemBlock>().tempItemId = "0";
                    _block.transform.GetComponent<FutureCarItemBlock>().productPart = randomId;


                    //sceneCtrl.totalItemBlock.Add(_block);

                }
                //사운드 재생
                //sfxPlayer.PlayOneShot(productGenerateSound);
                SoundManager.instance.PlayEffectSound("eff_Future_create", 1f);
                break;
            case "lab":
                if (labGenerateCount <= 0) return;
                for (int i = 0; i < labBlockTile.Count; i++)
                {
                    if (labBlockTile[i].childCount != 0) continue;
                    else emptyPosNo.Add(i);
                }
                if (emptyPosNo.Count > 0)
                {
                    int rand = Random.Range(emptyPosNo.Count - emptyPosNo.Count, emptyPosNo.Count);

                    labGenerateCount--;
                    textLabGenerateCount.text = labGenerateCount.ToString() + "/" + maxLabGenerateCount.ToString();

                    //81 부품이 6개일 경우, 81이 나오지 않도록 처리
                    int _count81 = 0;
                    foreach(GameObject _blockName in sceneCtrl.totalItemBlock)
                    {
                        if(_blockName.name == "81")
                        {
                            _count81++;
                        }
                        if (_blockName.name == null) Debug.Log("무언가 미싱됨");
                    }

                    int randomId = 0;
                    if(_count81 >= 7)  randomId = Random.Range(5, 8);
                    else randomId =  Random.Range(5, 9);

                    GameObject _block = Instantiate(itemBlockPrefab, labBlockTile[emptyPosNo[rand]].position, Quaternion.identity);
                    _block.transform.SetParent(labBlockTile[emptyPosNo[rand]]);
                    _block.transform.localScale = Vector3.one;
                    _block.transform.GetComponent<FutureCarItemBlock>().tempItemId = "0";
                    _block.transform.GetComponent<FutureCarItemBlock>().productPart = randomId;


                    //rsceneCtrl.totalItemBlock.Add(_block);
                }
                //사운드 재생
                //sfxPlayer.PlayOneShot(productGenerateSound);
                SoundManager.instance.PlayEffectSound("eff_Future_create", 1f);
                break;
        }
        ////사운드 재생
        //sfxPlayer.PlayOneShot(productGenerateSound);
    }


    public void RemoveStockBlock(Transform _blockTr)
    {
        for (int i = 0; i < stockBlock.Count; i++)
        {
            if (stockBlock[i] == _blockTr)
            {
                stockBlock.RemoveAt(i);
                return;
            }
        }
    }




    
}
