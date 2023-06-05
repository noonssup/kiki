using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 신재생에너지 게임 로직 컨트롤 스크립트
/// 상단 = 본게임 로직 / 하단 = 튜토리얼 게임 로직
/// </summary>

public class RenewEnergyGameController : MonoBehaviour
{
    public GameObject resultPanel;

    [Header("오브젝트")]
    public RectTransform wingGenerator;
    public RectTransform solarGenerator;
    public RectTransform tidalGenerator;
    public GameObject windPrefab;
    public GameObject[] cloudPrefab;
    public GameObject wavePrefab;
    public GameObject wavePrefabL;
    public GameObject wavePrefabR;
    public Image gauge;

    [Header("게임데이터")]
    public int cloudCount = 0;
    public float cloudTimer = 0f;
    public float currentEnergy;
    public float gameMaxTimer = 0f;
    public float maxEnergy = 700f;
    public bool isRain = false;
    private float windTimer = 0f;
    private float gameTimer = 0f;
    private float waveTimer = 0f;

    [Header("텍스트")]
    public TextMeshProUGUI textResultEnergy;
    public TextMeshProUGUI textCurrentEnergy;
    public TextMeshProUGUI textGameTimer;

    private void Start()
    {
        GameDataInit();
    }


    public void GameDataInit()  //게임 시작 시 정보 초기화
    {
        resultPanel.SetActive(false);

        windTimer = 6f;  //바람 생성 주기 
        cloudTimer = 3f; //구름 생성 주기 
        waveTimer = 6f;  //조류 생성 주기 
        currentEnergy = 0f;
        textCurrentEnergy.text = currentEnergy.ToString("n0") + "MW";
        gauge.fillAmount = currentEnergy / maxEnergy;
        gameTimer = gameMaxTimer;
        textGameTimer.text = ((int)gameTimer / 60).ToString("00") + ":" + (gameTimer % 60).ToString("00");
    }

    private void Update()
    {
        if (RenewEnergyGameManager.instance.isGamePlaying)
        {
            if (gameTimer > 5f)
            {
                WindMaker();
                if (cloudCount < 5) CloudMaker();
                WaveMaker();
            }


            gameTimer -= Time.deltaTime;
            if (gameTimer <= 0f)
            {
                gameTimer = 0f;
                RenewEnergyGameManager.instance.isGamePlaying = false;
                StartCoroutine(GameResult());
            }

            textGameTimer.text = ((int)gameTimer / 60).ToString("00") + ":" + (gameTimer % 60).ToString("00");
        }
    }

    IEnumerator GameResult()
    {
        yield return new WaitForSeconds(1f);

        resultPanel.SetActive(true);
        textResultEnergy.text = "전력 생산량 : " + currentEnergy.ToString("n0") + "MW";
    }

    #region 전력 생산
    public void SaveEnergy(float value)
    {
        if (gameTimer <= 0f) return;

        currentEnergy += value * Time.deltaTime;

        textCurrentEnergy.text = currentEnergy.ToString("n0") + "MW";
        gauge.fillAmount = currentEnergy / maxEnergy;
    }
    #endregion

    #region 바람/구름/조류 생성기
    void WindMaker()  //바람 생성기
    {
        if (RenewEnergyGameManager.instance.isGamePlaying && windTimer <= 0f)
        {
            GameObject _wind = Instantiate(windPrefab, new Vector3(15f, Random.Range(-1f, 3f), 0f), Quaternion.identity);
            _wind.transform.SetParent(this.transform);
            _wind.transform.localScale = Vector3.one;
            windTimer = Random.Range(2f, 5f);
        }

        if (windTimer > 0f)
        {
            windTimer -= Time.deltaTime;
        }
    }

    void CloudMaker()  //구름 생성기
    {
        if (RenewEnergyGameManager.instance.isGamePlaying && cloudTimer <= 0f)
        {
            int index = Random.Range(0, cloudPrefab.Length);

            GameObject _cloud = Instantiate(cloudPrefab[index], new Vector3(Random.Range(-11f, 11f), 4f, 0f), Quaternion.identity);
            if(_cloud.name != "TutoCloud(Clone)") _cloud.GetComponent<RenewEnergyCloudMove>().gameCtrl = this;
            _cloud.transform.SetParent(this.transform);
            _cloud.transform.localScale = Vector3.one;
            cloudTimer = Random.Range(2f, 5f);
            int randIndex = Random.Range(0, 2);
            if (!isRain && randIndex==0)
            {
                isRain = true;
                _cloud.GetComponent<RenewEnergyCloudMove>().isThunder= true;
                StartCoroutine(RainSoundPlay(_cloud));
            }
        }

        if (cloudTimer > 0f)
        {
            cloudTimer -= Time.deltaTime;
        }
    }

    IEnumerator RainSoundPlay(GameObject _cloud)
    {
        yield return new WaitForSeconds(0.5f);
        if (_cloud == null) yield break;
        SoundManager.instance.PlayEffectSound("eff_SmartFarm_rain", 1f);
    }

    void WaveMaker()  //해류 생성기
    {
        if (RenewEnergyGameManager.instance.isGamePlaying && waveTimer <= 0f)
        {
            int index = Random.Range(0, 2);
            if (index == 0)
            {
                GameObject _wave = Instantiate(wavePrefabL, new Vector3(-0.6f, -4.3f, 0f), Quaternion.Euler(0f, 180f, 90f));
                _wave.transform.SetParent(this.transform);
                _wave.transform.localScale = new Vector3(150f, 150f, 150f);
                _wave.gameObject.name = "WaveLeft";
            }
            else if (index == 1)
            {
                GameObject _wave = Instantiate(wavePrefabR, new Vector3(8.4f, -4.3f, 0f), Quaternion.Euler(0f, 0f, 90f));
                _wave.transform.SetParent(this.transform);
                _wave.transform.localScale = new Vector3(150f, 150f, 150f);
                _wave.gameObject.name = "WaveRight";
            }
            waveTimer = Random.Range(5f, 7f);
        }

        if (waveTimer > 0f)
        {
            waveTimer -= Time.deltaTime;
        }
    }
    #endregion


    #region 튜토리얼 로직

    [Header("튜토리얼용")]
    public GameObject tutoCloudPrefab;
    public GameObject tutoWindPrefab;
    public GameObject tutoWavePrefab;
    public GameObject tutoCloudControlIcon;
    public GameObject guidePopup;
    public int tutoCloudCount = 0;
    public int tutoWindCount = 0;

    public IEnumerator TutorialPlay()
    {
        guidePopup.SetActive(false);
        currentEnergy = 0f;
        textCurrentEnergy.text = currentEnergy.ToString("n0") + "MW";
        gauge.fillAmount = currentEnergy / maxEnergy;

        WaitForSeconds _wait = new WaitForSeconds(0.5f);
        if (this.gameObject.name != "TutorialGamePanel") yield break;

        RenewEnergyGameManager.instance.tutorialPopup.SetActive(false);
        yield return null;

        guidePopup.SetActive(true);
        guidePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "태양광 발전은 태양의 빛 에너지를 전기로 변환합니다.\n좌우로 움직여서 구름에서 벗어나세요.";

        //구름 생성
        GameObject _cloud = Instantiate(tutoCloudPrefab, new Vector3(0f, 4f, 0f), Quaternion.identity);
        _cloud.transform.SetParent(this.transform);
        _cloud.transform.localScale = Vector3.one;

        yield return _wait;

        GameObject _icon = Instantiate(tutoCloudControlIcon);
        _icon.transform.SetParent(this.transform);
        _icon.transform.localScale = Vector3.one;
        _icon.GetComponent<RectTransform>().localPosition = new Vector3(34f, -23f, 0f);
        _icon.GetComponent<Animator>().SetTrigger("cloud");


        while (tutoCloudCount < 7)  //태양열발전기를 움직일 때까지 대기
        {
            yield return null;
        }

        guidePopup.SetActive(false);
        Destroy(_icon.gameObject);
        _icon = null;

        yield return _wait;

        GameObject _wind;
        guidePopup.SetActive(true);
        guidePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "풍력 발전은 바람이 날개를 회전시켜 전기를 생산합니다.\n위아래로 움직여 바람이 부는 곳으로 이동시키세요.";
        
        while (tutoWindCount < 1)
        {
            //바람 생성
            _wind = Instantiate(tutoWindPrefab, new Vector3(5f, 0f, 0f), Quaternion.identity);
            _wind.transform.SetParent(this.transform);
            _wind.transform.localScale = Vector3.one;
            if (_icon == null)
            {
                _icon = Instantiate(tutoCloudControlIcon, new Vector3(-386f, 162f, 0f), Quaternion.identity);
                _icon.transform.SetParent(this.transform);
                _icon.transform.localScale = Vector3.one;
                _icon.GetComponent<RectTransform>().localPosition = new Vector3(-695f, 162f, 0f);
                _icon.GetComponent<Animator>().SetTrigger("wind");
            }


            while (_wind.gameObject != null)
            {
                yield return null;
            }

            yield return null;
        }

        guidePopup.SetActive(false);
        Destroy(_icon.gameObject);
        _icon = null;
        _wind= null;
        tutoWindCount = 0;

        yield return _wait;

        guidePopup.SetActive(true);
        guidePopup.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "해류 발전은 해류의 흐름이 날개를 회전시켜 전기를 생산합니다.\n발전기를 터치하여 방향을 바꿔보세요.";
        
        while (tutoWindCount < 1)
        {
            //해류 생성
            GameObject _wave = Instantiate(wavePrefabR, new Vector3(8.4f, -4.3f, 0f), Quaternion.Euler(0f, 0f, 90f));
            _wave.transform.SetParent(this.transform);
            _wave.transform.localScale = new Vector3(150f, 150f, 150f);
            _wave.gameObject.name = "TutoWaveRight";

            if (_icon == null)
            {
                _icon = Instantiate(tutoCloudControlIcon, new Vector3(210f, -210f, 0f), Quaternion.identity);
                _icon.transform.SetParent(this.transform);
                _icon.transform.localScale = Vector3.one;
                _icon.GetComponent<RectTransform>().localPosition = new Vector3(265, -227f, 0f);
                _icon.GetComponent<Animator>().SetTrigger("wave");
            }

            while (_wave.gameObject != null)
            {
                yield return null;
            }

            yield return null;
        }
        yield return _wait;

        guidePopup.SetActive(true);
        _icon.SetActive(false);
        guidePopup.SetActive(false);

        RenewEnergyGameManager.instance.tutorialPopup.SetActive(true);
        RenewEnergyGameManager.instance.tutorialPopup.transform.GetChild(1).gameObject.SetActive(true);
        RenewEnergyGameManager.instance.tutorialPopup.transform.GetChild(0).gameObject.SetActive(false);

        yield return _wait; yield return _wait; yield return _wait; yield return _wait; yield return _wait; yield return _wait;

        RenewEnergyGameManager.instance.RetryGame();
    }

    #endregion

}
