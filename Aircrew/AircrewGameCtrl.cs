using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class AircrewGameCtrl :   MonoBehaviour// Singleton<AircrewGameCtrl>
{
    public enum GamePlayState { NORMAL, TUBULENCEAIR, ESCAPE }
    public GamePlayState gameState = GamePlayState.NORMAL;

    public AircrewCanvas airCanvas;
    public Image bg1;
    public GameObject alertImage;
    public GameObject escapeAlertImage;
    public Transform[] npcSpawnPosTr;
    public int npcCount = 18;
    public GameObject npcPrefab;
    public List<GameObject> npcList = new List<GameObject>();
    public GameObject npcMoveSpace;  //npc를 드래그할 때 이미지가 가장 위에 보이도록 트랜스폼을 옮겨준다
    public GameObject gameWindow;
    public float gameTimer;
    public TMP_Text gameTimerText;
    public bool isPassengerEscape;  //승객이 탈출 중인 상태인지 여부 확인
    public bool isGamePlaying;
    public bool isPause;
    public bool isEscapeSuccess;
    public Transform[] escapeTr;  //비행기에서 탈출할 때 npc 가 이동할 방향 (슬라이드를 타고 미끄러지듯 내려가는 연출을 위함)
    public GameObject[] escapeGuide;  //화살표, 슬라이드 오브젝트 (비상탈출 시 승객을 처음 잡으면 활성화)
    public int escapeCount = 0;        //비상탈출 시 승객을 잡으면 가이드가 표시되는 횟수 (2회 이후에는 나오지 않도록)
    public Vector3 initialPosition;
    public Sprite[] emergency;
    public Sprite[] npcSprites;

    [Header("가방떨어뜨리기 제스처")]
    public Image circleImage;
    //public Image square;
    public Image[] squareImages;

    [Header("돌아다니는 진상승객")]
    public GameObject vMove;
    public GameObject hMove;
    public GameObject NPCComplainPopup;
    public Button[] answerButton;
    public GameObject[] moveNPC;
    public GameObject popupNPC;
    public int popupNPCIndex;
    public float npcTimer;
    public int moveNpcCount;

    public float respawnNpcTimer;

    [Header("안내방송용 UI")]
    public GameObject guideUIObj;
    //public Animator guideBoxAnim;
    public TMP_Text speechText;
    public TMP_Text guideText;
    public bool isAlert;

    [Header("결과창 UI")]
    public GameObject resultPanel;
    public GameObject clearObject;
    public GameObject failObject;
    public TextMeshProUGUI resultGuideText;
    public TMP_Text recordText;
    public GameObject redBgObject;
    public GameObject greenBgObject;
    public GameObject npcTimerObject;

    [Header("도움말")]
    public GameObject helpPanel;
    public GameObject[] helpPage;
    public Button[] pageButtons;
    public TMP_Text pageText;
    public int page;

    int _randAnswer;

    private void Awake()
    {
        airCanvas = FindObjectOfType<AircrewCanvas>();
        moveNPC = new GameObject[npcCount];
    }

    private void OnEnable()
    {
        SetPassenger();
        GameSetup();
    }

    public void HelpPanelSet()
    {
        if (helpPanel.activeSelf)
        {
            helpPanel.SetActive(false);
            Time.timeScale = 1;
            VisibleMoveNpcList();
        }
        else
        {
            helpPanel.SetActive(true);
            Time.timeScale = 0;
            page = 1;
            HelpPageSetup();
            HideMoveNpcList();
        }
    }

    void HelpPageSetup()
    {
        switch (page)
        {
            case 1:
                helpPage[0].SetActive(true);
                helpPage[1].SetActive(false);
                pageButtons[0].gameObject.SetActive(false);
                pageButtons[1].gameObject.SetActive(true);
                pageText.text = "1 / 2";
                break;
            case 2:
                helpPage[0].SetActive(false);
                helpPage[1].SetActive(true);
                pageButtons[0].gameObject.SetActive(true);
                pageButtons[1].gameObject.SetActive(false);
                pageText.text = "2 / 2";
                break;
        }
    }

    public void PageChange()
    {
        GameObject _button = EventSystem.current.currentSelectedGameObject;

        switch (_button.name)
        {
            case "PagePrevButton": page--; break;
            case "PageNextButton": page++; break;
        }
        HelpPageSetup();
    }

    public void SetPassenger()   //승객 배치
    {
        npcList = new List<GameObject>();
        for (int i = 0; i < npcCount; i++)
        {
            int randPos = Random.Range(0, npcSpawnPosTr.Length);
            if (npcSpawnPosTr[randPos].childCount == 0)
            {
                GameObject _npc = Instantiate(npcPrefab, npcSpawnPosTr[randPos].position, Quaternion.identity) as GameObject;
                _npc.transform.SetParent(npcSpawnPosTr[randPos].transform);
                _npc.transform.localScale = npcSpawnPosTr[randPos].transform.localScale;
                npcList.Add(_npc);
                npcList[i].GetComponent<AirPassenger>().SetIndex(i);
                if (npcSpawnPosTr[randPos].name == "NPCPos4" || npcSpawnPosTr[randPos].name == "NPCPos5"
                    || npcSpawnPosTr[randPos].name == "NPCPos6" || npcSpawnPosTr[randPos].name == "NPCPos7")
                {
                    //David edit 2022-11-15
                    npcList[i].transform.GetChild(1).transform.rotation = Quaternion.Euler(0f, 0f, 0f);
                    npcList[i].GetComponent<AirPassenger>().resetRot = new Vector3(0f, 0f, 0f);
                    npcList[i].transform.GetChild(1).transform.localScale =
                    new Vector3(npcList[i].transform.GetChild(1).transform.localScale.x * (-1f), npcList[i].transform.GetChild(1).transform.localScale.y, npcList[i].transform.GetChild(1).transform.localScale.z);
                    npcList[i].transform.GetChild(2).transform.rotation = Quaternion.Euler(npcList[i].transform.GetChild(2).transform.rotation.x, npcList[i].transform.GetChild(2).transform.rotation.y + 180f, npcList[i].transform.GetChild(2).transform.rotation.z);
                }
                else
                {
                    //David edit 2022-11-15
                    npcList[i].transform.GetChild(1).transform.rotation = Quaternion.Euler(0f, 180f, 0f);
                    npcList[i].GetComponent<AirPassenger>().resetRot = new Vector3(0f, 0f, 0f);
                    npcList[i].transform.GetChild(1).transform.localScale =
                    new Vector3(npcList[i].transform.GetChild(1).transform.localScale.x/* * (-1f)*/, npcList[i].transform.GetChild(1).transform.localScale.y, npcList[i].transform.GetChild(1).transform.localScale.z);
                }
            }
            else i--;
        }
    }

    private void OnDisable()
    {
        npcList = null;
        Time.timeScale = 1;
    }

    void GameSetup()
    {
        SquareChange(false);
        CircleChange(false);
        initialPosition = gameWindow.transform.position;
        NPCComplainPopup.transform.Find("SpeechImage").GetComponent<Image>().sprite = emergency[0];
        NPCComplainPopup.SetActive(false);
        resultPanel.SetActive(false);
        gameState = GamePlayState.NORMAL;
        escapeAlertImage.SetActive(false);
        escapeCount = 0;
        EscapeGuideSetup(false);
        isPassengerEscape = false;
        isGamePlaying = false;
        isPause = false;
        isEscapeSuccess = false;
        gameTimer = 60f;
        int _min = (int)gameTimer / 60;
        int _sec = (int)gameTimer % 60;
        gameTimerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);
        Color _bgColor = new Color(1f, 1f, 1f, 1f);
        bg1.color = _bgColor;
        isAlert = false;
        alertImage.SetActive(false);
        guideUIObj.SetActive(false);
        helpPanel.SetActive(false);
        //David edit 2022-11-16
        InitMoveNpcList();
        StartCoroutine(GamePlay());
    }
    public void SquareChange(bool _active)
    {

        for (int i = 0; i < squareImages.Length; i++)
        {
            squareImages[i].gameObject.SetActive(_active);
            squareImages[i].fillAmount = 1f;
        }

    }

    public void CircleChange(bool _active)
    {
        circleImage.gameObject.SetActive(_active);
        circleImage.fillAmount = 1f;
    }

    public void EscapeGuideSetup(bool _active)
    {
        if (_active)
        {
            escapeCount++;
            if (escapeCount < 3)
            {
                escapeGuide[1].SetActive(_active);
                escapeGuide[0].SetActive(_active);
            }
            else return;
        }
        else
        {
            escapeGuide[0].SetActive(_active);
            escapeGuide[1].SetActive(_active);
        }

    }

    void SfxSoundPlay(string _soundName)  //효과음 교체 시 사용
    {
        SoundManager.instance.PlayEffectSound(_soundName, 1f);
    }

    void BgmSoundPlay(float volume)
    {
        SoundManager.instance.PlayBGMSound("bg_Airplane", volume);
    }

    IEnumerator GamePlay()
    {
        SfxSoundPlay("eff_Airplane_buzz");

        yield return new WaitForSeconds(3f);

        //기장의 안내멘트 (gene 맘대로 넣음)
        StartCoroutine(CaptainsGuideComment1());

        yield return new WaitForSeconds(6f);

        SfxSoundPlay("eff_Airplane_warning");
        isAlert = true;

        //긴급상황 효과 연출
        alertImage.SetActive(true);
        SoundManager.instance.SetBgmSourceVolume(0.1f);
        yield return new WaitForSeconds(5f);
        yield return StartCoroutine(CaptainsGuideComment2());
        alertImage.SetActive(false);
        SoundManager.instance.StopBGMSound();

        //승객 상태 변경 처리
        int _safetyState = 0;
        int _scaredStste = 0;
        for (_safetyState = 0; _safetyState < npcList.Count;)
        {
            int _randCount = Random.Range(0, npcList.Count);
            if (npcList[_randCount].GetComponent<AirPassenger>().npcState == AirPassenger.NPCState.SIT)
            {
                npcList[_randCount].GetComponent<AirPassenger>().npcState = AirPassenger.NPCState.SAFETY;
                npcList[_randCount].GetComponent<AirPassenger>().SetNPC();
                _safetyState++;
                if (_safetyState == 6) break;
            }
        }

        for (_scaredStste = 0; _scaredStste < npcList.Count;)
        {
            int _randCount = Random.Range(0, npcList.Count);
            if (npcList[_randCount].GetComponent<AirPassenger>().npcState == AirPassenger.NPCState.SIT)
            {
                npcList[_randCount].GetComponent<AirPassenger>().npcState = AirPassenger.NPCState.SCARED;
                npcList[_randCount].GetComponent<AirPassenger>().SetNPC();
                _scaredStste++;
                if (_scaredStste == 12) break;
            }
        }

        //게임 플레이 상태 변경 (시작의 NORMAL 에서 난기류 상태인 TUBULENCEAIR 로 변경)
        //(TUBULENCEAIR 상태에서 승객을 클릭하면 SCARED 상태의 승객을 SAFETY 로 변경 가능)
        gameState = GamePlayState.TUBULENCEAIR;
        isGamePlaying = true;

        //게임안내 팝업
        HelpPanelSet();
        while (Time.timeScale == 0)
        {
            yield return null;
        }

        BgmSoundPlay(0.1f);
        npcTimer = 0;

        List<GameObject> _safetyNpc = new List<GameObject>();
        while (_safetyNpc.Count < npcList.Count)  //안전상태의 승객의 숫자가 npcList 의 카운트보다 낮은 동안 (18명보다 적으면)
        {//승객의 상태 확인
            for (int i = 0; i < npcList.Count; i++)
            {
                if (npcList[i].GetComponent<AirPassenger>().npcState == AirPassenger.NPCState.SAFETY && !_safetyNpc.Contains(npcList[i].gameObject))
                    _safetyNpc.Add(npcList[i].gameObject);
                else if(npcList[i].GetComponent<AirPassenger>().npcState == AirPassenger.NPCState.HMOVE && _safetyNpc.Contains(npcList[i].gameObject) || npcList[i].GetComponent<AirPassenger>().npcState == AirPassenger.NPCState.VMOVE && _safetyNpc.Contains(npcList[i].gameObject))
                {
                    _safetyNpc.Remove(npcList[i].gameObject);
                }
            }
            yield return new WaitForFixedUpdate();
            if (!isPause) { gameTimer -= Time.deltaTime; }   //isPause 가 false 이면 시간 경과, true 일 경우 멈춤
            //int _min = (int)gameTimer / 60;
            //int _sec = (int)gameTimer % 60;
            //gameTimerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);
            TimerText();
            if (gameTimer <= 0f)
            {
                gameTimer = 0f;
                TimerText();
                ResultFuction();
                yield break;
            }
        }

        //상태 변경 3초 후? 
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < _safetyNpc.Count; i++)
        {
            _safetyNpc.RemoveAt(0);
        }

        //화면 쉐이크 연출 (불시착 장면의 연출 효과)
        StartCoroutine(airCanvas.AirplaneDownAnim());
        yield return new WaitForSeconds(3f);
        CameraShakeEffect(5f, 1);
        yield return new WaitForSeconds(3f);
        //AircrewCanvas.Instance.AirplaneDownFunction();

        _safetyNpc = new List<GameObject>();
        //셰이크 연출 중 배경 이미지 변경
        StartCoroutine(ChangeBackgroundImage());
        //yield return StartCoroutine(ChangeBackgroundImage());

        yield return new WaitForSeconds(2.2f);
        isGamePlaying = false;
        //David edit 2022-11-16
        InitMoveNpcList();

        //기장의 탈출 안내 방송
        StartCoroutine(CaptainsGuideComment3());

        //배경 이미지가 변경되면, 다음 상황으로 변경 (+ 승객 상태 변경)
        gameState = GamePlayState.ESCAPE;
        for (int i = 0; i < npcList.Count; i++)  // SIT 상태로 전부 변경하고...
        {
            npcList[i].GetComponent<AirPassenger>().npcState = AirPassenger.NPCState.SIT;
            npcList[i].GetComponent<AirPassenger>().SetNPC();
            //David edit 2022-11-15
            npcList[i].GetComponent<AirPassenger>().SetEnableNpcConfuse();
        }

        for (int _belt = 0; _belt < 6;)
        {
            int _randNum = Random.Range(0, npcList.Count);
            if (npcList[_randNum].GetComponent<AirPassenger>().npcState == AirPassenger.NPCState.SIT && !_safetyNpc.Contains(npcList[_randNum].gameObject))
            {
                npcList[_randNum].GetComponent<AirPassenger>().npcState = AirPassenger.NPCState.BELT;
                _safetyNpc.Add(npcList[_randNum].gameObject); 
                npcList[_randNum].GetComponent<AirPassenger>().SetNPC();
                _belt++;
            }
        }

        for (int _bag = 0; _bag < 5;)
        {
            int _randNum = Random.Range(0, npcList.Count);
            if (npcList[_randNum].GetComponent<AirPassenger>().npcState == AirPassenger.NPCState.SIT && !_safetyNpc.Contains(npcList[_randNum].gameObject))
            {
                npcList[_randNum].GetComponent<AirPassenger>().npcState = AirPassenger.NPCState.PACKING;
                _safetyNpc.Add(npcList[_randNum].gameObject); 
                npcList[_randNum].GetComponent<AirPassenger>().SetNPC();
                _bag++;
            }
        }
        //for(int i = 0; i < _safetyNpc.Count; i++)
        //{
        //    Debug.Log("가방/벨트 승객의 수 " + i + 1.ToString());
        //}

        escapeAlertImage.SetActive(true);

        ////기장의 탈출 안내 방송
        //StartCoroutine(CaptainsGuideComment3());

        //방송이 시작되면 게임타이머가 흐름 (01:30까지, 01:30 이 되면 게임 오버)
        isGamePlaying = true;
        while(/*gameTimer < 90f && */isGamePlaying)
        {
            if (!isPause) { gameTimer -= Time.deltaTime; }   //isPause 가 false 이면 시간 경과, true 일 경우 멈춤
            TimerText();
            if (gameTimer <= 0f)
            {
                gameTimer = 0f;
                TimerText();
                isEscapeSuccess = false;
                isGamePlaying = false;
                break;
            }


            if (npcList.Count <= 0)
            {
                isEscapeSuccess = true;
                isGamePlaying = false;
            }
            yield return new WaitForFixedUpdate();
        }

        yield return new WaitForSeconds(1.5f);
        //게임 결과 확인
        ResultFuction();

    }

    void ResultFuction() //게임 결과 확인
    {
        StartCoroutine(ResultWindowSetActive());
    }

    IEnumerator ResultWindowSetActive()   //게임 결과 확인
    {
        /*while (bgmPlayer.volume > 0)
        {
            yield return new WaitForFixedUpdate();
            bgmPlayer.volume -= Time.deltaTime;
        }*/

        resultPanel.SetActive(true);
        InitMoveNpcList();

        switch (isEscapeSuccess)
        {
            case true:
                SfxSoundPlay("eff_Common_clear");
                clearObject.SetActive(true);
                failObject.SetActive(false);
                resultGuideText.color = new Color32(255, 234, 89, 255);
                resultGuideText.text = "탈출 성공!";
                break;
            case false:
                SfxSoundPlay("eff_Common_fail");
                clearObject.SetActive(false);
                failObject.SetActive(true);
                resultGuideText.color = new Color32(255, 112, 122, 255);
                resultGuideText.text = "탈출 실패...";
                break;
        }
        TimerText();
        /*while (bgmPlayer.volume < 0.5f)
        {
            yield return new WaitForFixedUpdate();
            bgmPlayer.volume += Time.deltaTime*0.1f;
        }*/
        yield return new WaitForSeconds(2f);
        /*if (this.gameObject.activeSelf)
        {
            resultPanel.SetActive(false);
            this.gameObject.SetActive(false);
        }
        else yield break;*/
    }

    void TimerText()
    {
        if (gameTimer < 0f) gameTimer = 0f;
        int _min = (int)gameTimer / 60;
        int _sec = (int)gameTimer % 60;
        recordText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);
        gameTimerText.text = string.Format("{0:D2}:{1:D2}", _min, _sec);
    }

    public void PassengerEscapeSuccess(GameObject _npc)  //승객 탈출 성공 시 실행
    {
        isPassengerEscape = true;
        int _index = npcList.IndexOf(_npc);
        npcList.RemoveAt(_index);
    }
    public void PassengerEscapeFail()  //승객 탈출 실패 시 실행
    {
        CameraShakeEffect(0.5f, 2);
        gameTimer -= 5f;
    }


    void CameraShakeEffect(float _shakeTime, int _type)  //카메라 흔들림 연출
    {
        float shakeTime = _shakeTime;
        float shakeAmount = 0.3f;  ///흔들림 강도
        //Camera _camera = Camera.main;
        switch (_type)
        {
            case 1: StartCoroutine(ShakeEffectFunction(shakeTime, shakeAmount)); break;
            case 2: StartCoroutine(EscapeFailShakeEffectFunction(shakeTime, shakeAmount)); break;
        }

    }

    IEnumerator ShakeEffectFunction(float shakeTime, float shakeAmount) //비상착륙 시 화면 흔들림효과
    {
        SfxSoundPlay("eff_Airplane_shake");
        gameWindow.transform.position = initialPosition;
        float _currentShakeTime = shakeTime;
        yield return new WaitForFixedUpdate();
        while (shakeTime > 0)
        {
            yield return new WaitForFixedUpdate();
            gameWindow.transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
            shakeTime -= Time.deltaTime;
            if (_currentShakeTime * 0.5f >= shakeTime)
            {
                //sfxPlayer.volume -= Time.deltaTime;
            }
        }
        shakeTime = 0f;
        gameWindow.transform.position = initialPosition;
        /*sfxPlayer.Stop();
        sfxPlayer.volume = 1f;*/
        SoundManager.instance.StopAllEffSound();
    }
    IEnumerator EscapeFailShakeEffectFunction(float shakeTime, float shakeAmount) //npc탈출 실패 시 화면 흔들림효과 (가방을 든 npc 탈출 시도시)
    {
        isPassengerEscape = true;
        Vector3 initialPosition = gameWindow.transform.position;
        yield return new WaitForFixedUpdate();
        while (shakeTime > 0)
        {
            yield return new WaitForFixedUpdate();
            gameWindow.transform.position = Random.insideUnitSphere * shakeAmount + initialPosition;
            shakeTime -= Time.deltaTime;
        }
        shakeTime = 0f;
        gameWindow.transform.position = initialPosition;
        isPassengerEscape = false;
    }

    IEnumerator ChangeBackgroundImage()  //비상착륙 시 비행기내부 이미지 변경 효과
    {
        yield return new WaitForFixedUpdate();
        Color _bg1Color = bg1.color;

        while(bg1.color.a > 0f)
        {
            _bg1Color.a -= Time.deltaTime * 0.5f;
            bg1.color = _bg1Color;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator CaptainsGuideComment1()  //기장의 안내말씀 (처음 인사)
    {

        WaitForSeconds wfs15 = new WaitForSeconds(1.5f);
        WaitForSeconds wfs02 = new WaitForSeconds(0.02f);
        
        int _commentCount = 0;
        string _commentText;
        while (!isAlert)
        {
            switch (_commentCount)
            {
                case 0:
                    speechText.text = "";
                    guideText.text = "";
                    //guideBoxAnim.SetBool("guidebox", false);
                    //guideBoxAnim.gameObject.SetActive(false);
                    guideUIObj.SetActive(true);
                    //guideBoxAnim.gameObject.SetActive(true);
                    //guideBoxAnim.SetBool("guidebox", true);
                    break;
                case 1: 
                    _commentText = "포키항공을 이용해주신 여러분께";
                    for(int i = 0; i < _commentText.Length; i++)
                    {
                        speechText.text = _commentText.Substring(0, i + 1);
                        yield return wfs02;
                    }
                    break;
                case 2: 
                    _commentText = "감사의 말씀 드립니다.";
                    for (int i = 0; i < _commentText.Length; i++)
                    {
                        speechText.text = _commentText.Substring(0, i + 1);
                        yield return wfs02;
                    }
                    break;
                case 3: 
                    _commentText = "목적지까지 편안하게 모시겠습니다.";
                    for (int i = 0; i < _commentText.Length; i++)
                    {
                        speechText.text = _commentText.Substring(0, i + 1);
                        yield return wfs02;
                    }
                    break;
                case 4:
                    _commentText = "감사합니다.";
                    for (int i = 0; i < _commentText.Length; i++)
                    {
                        speechText.text = _commentText.Substring(0, i + 1);
                        yield return wfs02;
                    }
                    break;
            }
            _commentCount++;
            yield return wfs15;
        }
        //guideBoxAnim.SetBool("guidebox", false);
        //guideBoxAnim.gameObject.SetActive(false);
        guideUIObj.SetActive(false);
    }

    IEnumerator CaptainsGuideComment2()  //기장의 난기류 방송
    {
        WaitForSeconds wfs15 = new WaitForSeconds(1.5f);
        WaitForSeconds wfs02 = new WaitForSeconds(0.02f);
        string _commentText;
        int _commentCount = 0;

        while (_commentCount < 4)
        {
            switch (_commentCount)
            {
                case 0:
                    speechText.text = "";
                    guideText.text = "";
                    //guideBoxAnim.SetBool("guidebox", false);
                    //guideBoxAnim.gameObject.SetActive(false);
                    guideUIObj.SetActive(true);
                    //guideBoxAnim.gameObject.SetActive(true);
                    //guideBoxAnim.SetBool("guidebox", true);
                    break;
                case 1: 
                    guideText.text = "(겁먹은 승객을 클릭해주세요.)";   //"(사람들이 충격에 대비할 수 있게 해주세요.)" "(겁먹은 승객을 클릭해주세요.)"
                    _commentText = "긴급상황 발생";
                    for (int i = 0; i < _commentText.Length; i++)
                    {
                        speechText.text = _commentText.Substring(0, i + 1);
                        yield return wfs02;
                    }
                    break;
                case 2: 
                    _commentText = "비행기가 난기류를 만났습니다.";
                    for (int i = 0; i < _commentText.Length; i++)
                    {
                        speechText.text = _commentText.Substring(0, i + 1);
                        yield return wfs02;
                    }
                    break;
                case 3: 
                    _commentText = "충격에 대비해주시기 바랍니다.";
                    for (int i = 0; i < _commentText.Length; i++)
                    {
                        speechText.text = _commentText.Substring(0, i + 1);
                        yield return wfs02;
                    }
                    break;
            }
            _commentCount++;
            yield return wfs15;
        }
        //guideBoxAnim.gameObject.SetActive(false);
        guideUIObj.SetActive(false);
    }

    IEnumerator CaptainsGuideComment3()  //기장의 비상탈출 안내방송
    {
        yield return new WaitForSeconds(1.5f);
        WaitForSeconds wfs02 = new WaitForSeconds(0.02f);
        string _commentText;
        speechText.text = "";
        guideText.text = "";
       // guideBoxAnim.SetBool("guidebox", false);
        //guideBoxAnim.gameObject.SetActive(false);
        guideUIObj.SetActive(true);
        //guideBoxAnim.gameObject.SetActive(true);
        //guideBoxAnim.SetBool("guidebox", true);

        _commentText = "기장입니다. 탈출하십시오!";
        guideText.text = "(승객을 드래그해서 비상탈출구로 옮겨주세요.)";      //"(사람들을 드래그해서 비상탈출구로 옮겨주세요.)"  "(사람들이 탈출할 수 있게 옮겨주세요.)"
        for (int i = 0; i < _commentText.Length; i++)
        {
            speechText.text = _commentText.Substring(0, i + 1);
            yield return wfs02;
        }

        yield return new WaitForSeconds(5f);
        //guideBoxAnim.gameObject.SetActive(false);
        guideUIObj.SetActive(false);
    }

    public void SetComplainPopup(bool _active, GameObject _npc)  //진상승객 팝업 활성화
    {
        if(_active == true)
        {
            popupNPC = _npc;
            GameObject npcObject = NPCComplainPopup.transform.Find("SpeechImage").gameObject;
            npcObject.GetComponent<Image>().sprite = emergency[0];
            npcObject.GetComponent<Image>().SetNativeSize();
            NPCComplainPopup.SetActive(true);
            StartCoroutine(ComplainFunction());
        }
        else if(_active == false)
        {
            popupNPC = null;
            NPCComplainPopup.SetActive(false);
        }
    }
    public  int _correctAnswer = 0;  //정답 번호

    IEnumerator ComplainFunction() //진상승객의 진상행동 상황 진행
    {
        isPause = true;
        HideMoveNpcList();
        redBgObject.SetActive(true);
        greenBgObject.SetActive(false);
        npcTimerObject.SetActive(false);
        ColorBlock _color = answerButton[0].colors;
        _color.normalColor = new Color(1f, 1f, 1f, 1f);
        for (int i = 0; i < answerButton.Length; i++)
        {
            answerButton[i].colors = _color;
            answerButton[i].gameObject.SetActive(false);
        }
        GameObject npcImage = NPCComplainPopup.transform.Find("NPC").gameObject;
        npcImage.transform.GetChild(0).GetComponent<Image>().sprite = popupNPC.transform.GetChild(1).GetComponent<Image>().sprite;
        npcImage.transform.GetChild(0).transform.localScale = new Vector3(2f, 2f, 2f);

        GameObject speechImage = NPCComplainPopup.transform.Find("SpeechImage").gameObject;
        speechImage.gameObject.SetActive(false);
        speechImage.transform.GetChild(0).localPosition = new Vector3(0, 0, 0);
        _randAnswer = Random.Range(0, 4);
        switch(_randAnswer)
        {
            case 0:
                speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "무...무슨 상황이야! 빨리 뭐라도 좀 해봐!";
                break;
            case 1:
                speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "뭐야...무슨 상황인거야...빨리 나가고싶어요";
                break;
            case 2:
                speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "왜이렇게 흔들려요? 무슨 상황이에요?";
                break;
            case 3:
                speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "빨리 탈출시켜줘요!!";
                break;
        }
        yield return new WaitForSeconds(0.5f);
        speechImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < answerButton.Length; i++)
        {
            answerButton[i].gameObject.SetActive(true);
        }

        switch (_randAnswer)
        {
            case 0:
                _correctAnswer = 0;
                answerButton[0].transform.GetChild(1).GetComponent<TMP_Text>().text = "위급상황입니다!! 자리에 앉으세요!!";
                answerButton[1].transform.GetChild(1).GetComponent<TMP_Text>().text = "지...진정하세요...";
                answerButton[2].transform.GetChild(1).GetComponent<TMP_Text>().text = "비상상황입니다~ 진정해주세요~";
                break;
            case 1:
                _correctAnswer = 0;
                answerButton[0].transform.GetChild(1).GetComponent<TMP_Text>().text = "Clam Down!! 자리에 앉아!!";
                answerButton[1].transform.GetChild(1).GetComponent<TMP_Text>().text = "자...자리에 앉아주세요...";
                answerButton[2].transform.GetChild(1).GetComponent<TMP_Text>().text = "지...진정하시오 앉아주세요....";
                break;
            case 2:
                _correctAnswer = 0;
                answerButton[0].transform.GetChild(1).GetComponent<TMP_Text>().text = "진정하십시오!! 자리에 앉아주세요!!";
                answerButton[1].transform.GetChild(1).GetComponent<TMP_Text>().text = "단순한 난기류 상황입니다. 괜찮아요~";
                answerButton[2].transform.GetChild(1).GetComponent<TMP_Text>().text = "걱정마세요~ 잠시 흔들리는거에요~";
                break;
            case 3:
                _correctAnswer = 0;
                answerButton[0].transform.GetChild(1).GetComponent<TMP_Text>().text = "위험합니다!! 자리에 앉아!!";
                answerButton[1].transform.GetChild(1).GetComponent<TMP_Text>().text = "지...진정하세요...";
                answerButton[2].transform.GetChild(1).GetComponent<TMP_Text>().text = "걱정마세요~ 잠시 흔들리는거에요~";
                break;
        }

        while (NPCComplainPopup.activeSelf)
        {
            yield return new WaitForFixedUpdate();
            if (!isPause)
            {
                yield break;
            }
        }

    }

    public void ClickButton(int _buttonNum)  //대답 버튼 클릭
    {
        ColorBlock _color = answerButton[0].colors;
        _color.normalColor = new Color(0.65f, 0.65f, 0.65f, 1f);
        for (int i = 0; i < answerButton.Length; i++)
        {
            answerButton[i].colors = _color;
        }
        StartCoroutine(SelectAnswerFunction(_buttonNum));
    }

    IEnumerator SelectAnswerFunction(int _buttonNum)  //대답에 따른 동작 수행
    {
        yield return new WaitForSeconds(1f);
        if(_buttonNum == _correctAnswer)
        {
            int npcIndex = 0;
            for(int i = 0; i < npcSprites.Length; i++)
            {
                if (npcSprites[i].name.Equals(popupNPC.transform.GetChild(1).GetComponent<Image>().sprite.name))
                {
                    npcIndex = i - 1;
                    break;
                }
            }
            GameObject npcImage = NPCComplainPopup.transform.Find("NPC").gameObject;
            npcImage.transform.GetChild(0).GetComponent<Image>().sprite = npcSprites[npcIndex];
            npcImage.transform.GetChild(0).transform.localScale = new Vector3(2f, 2f, 2f);

            GameObject speechImage = NPCComplainPopup.transform.Find("SpeechImage").gameObject;
            speechImage.GetComponent<Image>().sprite = emergency[1];
            speechImage.GetComponent<Image>().SetNativeSize();
            speechImage.transform.GetChild(0).localPosition = new Vector3(-30, 10, 0);

            switch (_randAnswer)
            {
                case 0:
                    speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "아...알겠어요...";
                    break;
                case 1:
                    speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "지시에 따르자 그게 제일 빠를거야...";
                    break;
                case 2:
                    speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "비상상황인가봐...지시에 따르자";
                    break;
                case 3:
                    speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "아...알겠어요...";
                    break;
            }
            
            SfxSoundPlay("eff_Common_next");
            redBgObject.SetActive(false);
            greenBgObject.SetActive(true);
            npcTimerObject.SetActive(false);
            yield return new WaitForSeconds(2f);
            ComplainNPCReset();
        }
        else
        {
            GameObject speechImage = NPCComplainPopup.transform.Find("SpeechImage").gameObject;
            switch (_randAnswer)
            {
                case 0:
                    speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "이런 상황에 어떻게 진정하란거야!!";
                    break;
                case 1:
                    speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "제대로 말해요! 안들리잖아요!!";
                    break;
                case 2:
                    speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "엔진에 불 난게 보이는데 무슨 말이야!!";
                    break;
                case 3:
                    speechImage.transform.GetChild(0).GetComponent<TMP_Text>().text = "뭐라는거야!! 엔진에 불 난게 보이는데!!";
                    break;
            }
            SfxSoundPlay("eff_Common_fail");
            npcTimerObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            ComplainNPCReset();
            //isPause = false;
            gameTimer -= 10f;
        }
    }

    void ComplainNPCReset()  //진상승객을 원래의 위치로 이동, 상태 SIT 로 변경
    {
        popupNPC.GetComponent<AirPassenger>().npcState = AirPassenger.NPCState.SAFETY;
        popupNPC.GetComponent<AirPassenger>().SetNPC();
        NPCnull(popupNPC.GetComponent<AirPassenger>().npcIndex);
        SetComplainPopup(false, null);
        VisibleMoveNpcList();
        isPause = false;
    }

    void NPCnull(int index)  //저장한 hNPC, vNPC 삭제
    {
        Destroy(moveNPC[index]);
        moveNPC[index] = null;
        moveNpcCount--;
    }

    private void Update()
    {
        if (gameState == GamePlayState.TUBULENCEAIR && isGamePlaying == true)
        {
            if (npcTimer == -1)
            {
                return;
            } else if (npcTimer >= 0 && isPause == false)
            {
                npcTimer -= Time.deltaTime;
            }
        }

#if UNITY_EDITOR
        if (isGamePlaying)
        {
            if (Input.GetKeyDown(KeyCode.KeypadPlus)) gameTimer += 5f;
            else if (Input.GetKeyDown(KeyCode.KeypadMinus)) gameTimer -= 5f;
        }
#endif
    }

    public void InitMoveNpcList()
    {
        for(int i = 0; i < npcCount; i++)
        {
            if (moveNPC[i] != null)
            {
                Destroy(moveNPC[i]);
                moveNPC[i] = null;
            }
        }
        npcTimer = -1;
        moveNpcCount = 0;
    }

    public void HideMoveNpcList()
    {
        for (int i = 0; i < npcCount; i++)
        {
            if (moveNPC[i] != null)
            {
                moveNPC[i].SetActive(false);
            }
        }
    }

    public void VisibleMoveNpcList()
    {
        for (int i = 0; i < npcCount; i++)
        {
            if (moveNPC[i] != null)
            {
                moveNPC[i].SetActive(true);
            }
        }
    }
}
