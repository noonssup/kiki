using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarDesignGameManager : MonoBehaviour
{
    [Header("������Ʈ")]
    CarDesignUIController uiCtrl;
    public CarDesignParts[] partsCtrl;
    public GameObject[] partsArry; //����������Ʈ �迭
    public GameObject itemBoxPrefab;  //ȭ�� �ϴܿ� ������ �����۹ڽ� ������
    public GameObject moveItemObj;
    public GameObject moveItemPrefab;
    public GameObject storyPanel;
    public GameObject guidePanel;
    public GameObject startPanel;
    public GameObject levelPanel;
    public GameObject resultPanel;
    public GameObject timeoutPanel;
    public GameObject pausePanel;
    public List<GameObject> itemList;
    public Transform itemPanelTr;
    public Transform carFrameObj;
    public Image timerGauge;
    public Sprite[] itemImages;     //ȭ�� �ϴܿ� ������ �����۹ڽ��� �̹���
    public BoxCollider2D[] partsColls;

    [Header("���ӵ�����")]
    public int isCheckFirstPlay = 0;  //������ ó�� �����ϴ��� ���� (�ϴ� ��������, ��⺰��...)
    public bool isGamePlay = false;
    int carCount = 0;
    public int CarCount
    {
        get { return carCount; }
        set
        {
            carCount = value;
            //LevelUpEffect();
            textCount.text = carCount.ToString("00");
        }
    }
    public float maxTimer = 90f; 
    float timer = 0f;
    public int currectCount = 0;
    public int gamePoint = 0;
    public int getPoint = 5000;
    public float roundTimer = 0f;
    public float roundLimitTimer = 0f;
    IEnumerator gameStartCoroutine;
    string[] soundName = { "eff_Common_timeout","eff_Robot_move" };

    [Header("�ؽ�Ʈ")]
    public TextMeshProUGUI textLevelPanel;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textCount;
    public TextMeshProUGUI textResultScore;
    public TextMeshProUGUI textGamePoint;

    //[SerializeField] Image effectImage;
    //[SerializeField] bool isEffectPlay = false;

    //void LevelUpEffect()
    //{
    //    if (GameLevel == 0) return;
    //    //����Ʈ ����

    //    //�Ʒ��� �ӽ� ����Ʈ
    //    //StartCoroutine(EffectPlay());
    //}

    //IEnumerator EffectPlay()
    //{
    //    if (isEffectPlay) yield break;
    //    isEffectPlay = true;
    //    effectImage.transform.localScale = Vector3.zero;

    //    while (effectImage.transform.localScale.x < 1f)
    //    {
    //        Color _color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
    //        effectImage.color = _color;
    //        effectImage.transform.localScale += Vector3.one * Time.deltaTime;
    //        yield return null;
    //    }

    //    effectImage.transform.localScale = Vector3.zero;
    //    isEffectPlay = false;
    //    yield break;
    //}

    private void Start()
    {
        Time.timeScale = 1;
        uiCtrl = this.transform.parent.GetComponent<CarDesignUIController>();
        gameStartCoroutine = PlayGame();
    }

    private void OnEnable()
    {
        moveItemObj = Instantiate(moveItemPrefab);
        moveItemObj.transform.SetParent(this.transform);
        if (moveItemObj.activeSelf) moveItemObj.SetActive(false);
        CarCount = 0;
        CheckFirstPlay();
        GamePanelSetup();
        GameSetup();
    }

    void CheckFirstPlay() //������ ó���ϴ���, �̹� �غô��� Ȯ�� (�ϴ� Ȯ�� ���� ������ ���ÿ���)
    {
        if (PlayerPrefs.HasKey("firsttime"))
        {
            isCheckFirstPlay = PlayerPrefs.GetInt("firsttime");
            GuidePanelOnoff(false);
        }
        else
        {
            GuidePanelOnoff(true);
        }
        isCheckFirstPlay = 1;
        PlayerPrefs.SetInt("firsttime", isCheckFirstPlay);
    }

    void GamePanelSetup()  //�г� ����
    {
        storyPanel.SetActive(false);
        guidePanel.SetActive(false);
        resultPanel.SetActive(false);
        timeoutPanel.SetActive(false);
        pausePanel.SetActive(false);
        levelPanel.SetActive(false);
        StartCoroutine(StartPanelActiveFalse());
    }

    IEnumerator StartPanelActiveFalse()
    {
        SoundManager.instance.StopBGMSound();
        yield return new WaitForSeconds(1);
        startPanel.SetActive(true);
        while (startPanel.activeSelf) yield return null;
        SoundManager.instance.PlayBGMSound("bg_ElectricCar", 1f);
        GameStart();
    }

    void GameSetup()  //���� ����
    {
        isGamePlay = false;
        timer = maxTimer;
        textTimer.text = ((int)timer/60).ToString("00") + ":" + ((int)timer%60).ToString("00");
        timerGauge.fillAmount = 00f;
        gamePoint = 0;
        textGamePoint.text = gamePoint.ToString();
        ItemBoxImageSetup();
        AsseyPartsSetup();
    }

    void AsseyPartsSetup()   //���� �������� ��ǰ ����
    {
        partsColls = new BoxCollider2D[partsCtrl.Length];
        for (int i = 0; i < partsCtrl.Length; i++)
        {
            partsColls[i] = partsCtrl[i].GetComponent<BoxCollider2D>();
            partsColls[i].enabled = true;
            Color _color = partsCtrl[i].transform.GetChild(0).GetComponent<Image>().color;
            partsCtrl[i].GetComponent<Image>().enabled = true;
            _color.a = 1f;
            partsCtrl[i].transform.GetChild(0).GetComponent<Image>().color = _color;
            _color.a = 0f;
            partsCtrl[i].transform.GetComponent<Image>().color = _color;
        }
    }

    void ItemBoxImageSetup()  //�ϴ� �����۹ڽ� ����
    {
        if(itemList.Count > 0)
        {
            for (int i = itemList.Count-1; i > -1; i--) Destroy(itemList[i].gameObject);
        }
        
        itemList = new List<GameObject>();

        int maxCount = itemImages.Length;
        int[] defaultCount = new int[maxCount];
        int[] resultNumber = new int[maxCount];

        for(int i = 0; i < maxCount; i++) { defaultCount[i] = i; }

        for(int i = 0; i < itemImages.Length; i++)
        {
            int index = Random.Range(0, maxCount);

            resultNumber[i] = defaultCount[index];
            defaultCount[index] = defaultCount[maxCount - 1];

            maxCount--;
            var _itemBox = Instantiate(itemBoxPrefab.GetComponent<CarDesignItemBox>());
            _itemBox.itemImage.sprite = itemImages[resultNumber[i]];
            _itemBox.transform.SetParent(itemPanelTr);
            
            itemList.Add(_itemBox.gameObject);
            _itemBox.transform.localScale = Vector3.one;
            _itemBox.GetComponent<BoxCollider2D>().enabled = false;
            _itemBox.GetComponent<BoxCollider2D>().enabled = true;
            _itemBox.itemObj = moveItemObj;
        }
    }

    public void GameStart()   //���ӽ���
    {
        partsColls = new BoxCollider2D[partsCtrl.Length];
        for (int i = 0; i < partsCtrl.Length; i++)
        {
            partsColls[i] = partsCtrl[i].GetComponent<BoxCollider2D>();
            partsColls[i].enabled = false;
            Color _color = partsCtrl[i].transform.GetChild(0).GetComponent<Image>().color;
            _color.a = 1f;
            partsCtrl[i].transform.GetChild(0).GetComponent<Image>().color = _color;

            _color = partsCtrl[i].GetComponent<Image>().color;
            _color.a = 0f;
            partsCtrl[i].GetComponent<Image>().color = _color;
        }

        Time.timeScale = 1;
        textTimer.text = ((int)timer / 60).ToString("00") + ":" + ((int)timer % 60).ToString("00");
        timerGauge.fillAmount = timer / maxTimer;
        //if (startPanel.activeSelf) startPanel.SetActive(false);
        if (resultPanel.activeSelf) resultPanel.SetActive(false);

        StartCoroutine(PlayGame());
    }

    IEnumerator PlayGame()   //���� �÷���
    {
        isGamePlay = true;
        int _maxCount = partsArry.Length;
        int[] _defaultCount = new int[partsArry.Length];
        int[] _resultCount = new int[partsArry.Length];
        int randomPartsCount = 0;

        if(randomPartsCount == 0) randomPartsCount = Random.Range(1, 2);

        if (CarCount < 2) randomPartsCount = Random.Range(1, 2);
        else if (CarCount > 30) randomPartsCount = Random.Range(7, 11);
        else if (CarCount >= 25) randomPartsCount = Random.Range(6, 10);
        else if (CarCount >= 20) randomPartsCount = Random.Range(5, 9);
        else if (CarCount >= 15) randomPartsCount = Random.Range(4, 8);
        else if (CarCount >= 10) randomPartsCount = Random.Range(4, 6);
        else if (CarCount >= 5) randomPartsCount = Random.Range(3, 5);
        else if (CarCount >= 2) randomPartsCount = Random.Range(2, 4);

        roundLimitTimer = randomPartsCount * 5f;
        roundTimer = 0f;

        for (int i = 0; i < _maxCount; i++) { _defaultCount[i] = i; }

        for (int i=0; i < randomPartsCount; i++)   //��ǰ �̹��� ��Ȱ��ȭ(����ȭ)
        {
            int randomNum = Random.Range(0, _maxCount);

            _resultCount[i] = _defaultCount[randomNum];
            _defaultCount[randomNum] = _defaultCount[_maxCount - 1];

            Color _color = partsArry[_resultCount[i]].GetComponent<Image>().color;
            _color.a = 1f;
            partsArry[_resultCount[i]].GetComponent<Image>().color = _color;
            partsArry[_resultCount[i]].GetComponent<BoxCollider2D>().enabled = true;
            partsArry[_resultCount[i]].GetComponent<CarDesignParts>().isCorrect = false;
            _color.a = 0f;
            partsArry[_resultCount[i]].transform.GetChild(0).GetComponent<Image>().color = _color;
            _maxCount--;   
        }

        while (timer > 0f)
        {
            roundTimer += Time.deltaTime;
            timer -= Time.deltaTime;
            textTimer.text = ((int)timer / 60).ToString("00") + ":" + ((int)timer % 60).ToString("00");
            timerGauge.fillAmount = timer / maxTimer;
            Color _color;
            if (timerGauge.fillAmount < 0.25f) 
            { 
                ColorUtility.TryParseHtmlString("#ff0000", out _color);
                timerGauge.color = _color;
            }
            else if (timerGauge.fillAmount < 0.6f)
            {
                ColorUtility.TryParseHtmlString("#fff100", out _color);
                timerGauge.color = _color;
            }
            else if (timerGauge.fillAmount >= 0.6f)
            {
                ColorUtility.TryParseHtmlString("#0cff00", out _color);
                timerGauge.color = _color;
            }

            if (currectCount == randomPartsCount)  //�ڵ����ϼ� �� ���� ȹ�� �� ���� �ܰ��...
            {
                //���� Ŭ����
                isGamePlay = false;
                yield return new WaitForSeconds(0.5f);

                //���� ȹ�� (0.5 > 300, 0.75 > 200, 1 > 150, 1 < 100) 
                if (roundTimer <= (roundLimitTimer * 0.3f)) StartCoroutine(GamePointCount(getPoint * 6));
                else if (roundTimer > (roundLimitTimer * 0.3f) && roundTimer <= (roundLimitTimer * 0.5f))
                    StartCoroutine(GamePointCount(getPoint * 5));
                else if (roundTimer > (roundLimitTimer * 0.5f) && roundTimer <= (roundLimitTimer * 0.75f))
                    StartCoroutine(GamePointCount(getPoint * 4));
                else if (roundTimer > (roundLimitTimer * 0.75f) && roundTimer < (roundLimitTimer * 1f))
                    StartCoroutine(GamePointCount(getPoint * 3));
                else StartCoroutine(GamePointCount(getPoint * 2));

                NextLevel();
                yield break;
            }
            yield return null;
        }

        isGamePlay = false;
        timer = 0f;
        timerGauge.fillAmount = 0f;
        timerGauge.color = Color.green;

        timeoutPanel.SetActive(true);
        SoundManager.instance.PlayEffectSound(soundName[0], 1f);
        yield return new WaitForSeconds(2f);
        timeoutPanel.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        resultPanel.SetActive(true);
        Button[] retryButton = resultPanel.GetComponentsInChildren<Button>();
        retryButton[0].interactable = false;
        retryButton[1].interactable = false;
        textLevelPanel.text = $"�� ���귮 : {CarCount + 1}��";
        yield return StartCoroutine(GamePointCount(CarCount * 10000));

        retryButton[0].interactable = true;
        retryButton[1].interactable = true;

        yield break;
    }

    IEnumerator GamePointCount(int _point)  //���� ����Ʈ ȹ�� ����
    {
        int totalPoint = gamePoint + _point;
        while(gamePoint < totalPoint)
        {

            gamePoint += 1000;
            textGamePoint.text = gamePoint.ToString("n0");
            textResultScore.text = "�������� : " + gamePoint.ToString("n0");
            //yield return null;
            yield return new WaitForFixedUpdate();
        }

        if (_point <= 0)
        {
            textGamePoint.text = gamePoint.ToString("n0");
            textResultScore.text = "�������� : " + gamePoint.ToString("n0");
        }
    }

    void NextLevel()  //���� ��������...
    {
        SoundManager.instance.PlayEffectSound(soundName[1], 1f); 
        if (!levelPanel.activeSelf) levelPanel.SetActive(true);
        textLevelPanel.text = "���� ���谡 �Ϸ�Ǿ����ϴ�.\n���� ������ �����մϴ�.";

        //���� ���� ī��Ʈ ��!!
        CarCount++;
        currectCount = 0;

        StartCoroutine(NextLevelStart());
        StopCoroutine(gameStartCoroutine);
    }

    IEnumerator NextLevelStart()   //���� ��������...
    {
        //���� �̹��� �������� �̵� (��, ���ʿ��� �߾����� �̵�)
        while (carFrameObj.position.x < 15f)
        {
            carFrameObj.position += Vector3.right * Time.deltaTime * 10f;
            yield return null;
        }

        carFrameObj.localPosition = new Vector3(-1000f, 25f, 0f);
        AsseyPartsSetup();

        while (carFrameObj.position.x < 0f)
        {
            carFrameObj.position += Vector3.right * Time.deltaTime * 10f;
            yield return null;
        }

        carFrameObj.localPosition = new Vector3(0f, 25f,0f);

        ItemBoxImageSetup();

        GameStart();

        if (levelPanel.activeSelf) levelPanel.SetActive(false);
        yield break;
    }

    //IEnumerator AllLevelClear()
    //{
    //    yield return new WaitForSeconds(5f);
    //    if (levelPanel.activeSelf) levelPanel.SetActive(false);

    //    //Ŭ���� �� �޴� ȭ������ �̵� (�ӽ÷� ����� ���� ó��)
    //    RetryGame();
    //    yield break;
    //}

    public void GuidePanelOnoff(bool _bool)
    {
        guidePanel.SetActive(_bool);
        Time.timeScale = guidePanel.activeSelf ? 0f : 1f;
    }

    public void ResumeGame()
    {
        pausePanel.SetActive(pausePanel.activeSelf ? false : true);
        Time.timeScale = pausePanel.activeSelf ? 0f : 1f;
    }

    public void RetryGame()  //���� �����
    {
        StopAllCoroutines();
        Time.timeScale = 1;
        currectCount = 0;
        CarCount = 0;
        if (moveItemObj.activeSelf) moveItemObj.SetActive(false);
        GamePanelSetup();
        GameSetup();
    }
}
