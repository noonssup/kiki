using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotBattleSceneController : MonoBehaviour
{
    [Header("UI패널 / 컨트롤러 스크립트")]
    [SerializeField] RobotCanvas robotCanvasCtrl;
    public GameObject inventoryPanel;
    public RobotUICanvasController battleUiCtrl;
    public GameObject searchingPanel;
    public GameObject resultPanel;
    public Button cancelButton;
    public Image[] resultComments; //0 start, 1 victory, 2 lose

    [Header("게임오브젝트")]
    public List<Transform> itemPosTr;    //아이템슬롯을 클릭하여 생성할 아이템이 위치할 자리의 트랜스폼
    public List<Transform> itemListTr;   //아이템이 생성될 슬롯을 관리할 리스트
    public GameObject[] itemSlots;       //아이템슬롯 배열
    public GameObject[] itemPrefabs;     //부품아이템 프리팹
    public GameObject playerFramePrefab;       //플레이어 프레임 부품 프리팹
    public GameObject unitParent;              //플레이어 프레임 부품이 생성될 부모오브젝트
    public GameObject activePlayerGo;          //활성화된 플레이어 프레임 게임오브젝트
    public GameObject activeEnemyGo;          //활성화된 플레이어 프레임 게임오브젝트
    public GameObject[] backgroundImages;     //배경이미지
    [SerializeField] GameObject[] enemyPrefabs;
    [SerializeField] int enemyNumber = 0;  //프리팹이 여러개일 경우, 랜덤으로 적 프리팹을 생성하기 위한 변수
    private GameObject _enemy;

    [Header("플레이어유닛정보")]
    [SerializeField] TMP_Text textArmorCount;
    [SerializeField] TMP_Text textArmor;
    public int armorCount = 0;
    public int _armorCount
    {
        get { return armorCount; }
        set
        {
            armorCount += value;
            if (armorCount == maxArmorCount)
            {
                Color _color;
                ColorUtility.TryParseHtmlString("#FFCC5D", out _color);
                textArmorCount.color = _color;
                textArmor.color = _color;
            }
            else
            {
                Color _color;
                ColorUtility.TryParseHtmlString("#FFFFFF", out _color);
                textArmorCount.color = _color;
                textArmor.color = _color;
            }
            textArmorCount.text = armorCount.ToString() + "/" + maxArmorCount.ToString();
            
        }
    }
    public int maxArmorCount = 5;
    [SerializeField] TMP_Text textWeaponCount;
    [SerializeField] TMP_Text textWeapon;
    public int weaponCount = 0;
    public int _weaponCount
    {
        get { return weaponCount; }
        set
        {
            weaponCount += value;
            if (weaponCount == maxWeaponCount)
            {
                Color _color;
                ColorUtility.TryParseHtmlString("#FFCC5D", out _color);
                textWeaponCount.color = _color;
                textWeapon.color = _color;
            }
            else
            {
                Color _color;
                ColorUtility.TryParseHtmlString("#FFFFFF", out _color);
                textWeaponCount.color = _color;
                textWeapon.color = _color;
            }
            textWeaponCount.text = weaponCount.ToString() + "/" + maxWeaponCount.ToString();
        }
    }

    public int maxWeaponCount = 2;
    [SerializeField] TMP_Text textWheelCount;
    [SerializeField] TMP_Text textWheel;
    public int wheelCount = 0;
    public int _wheelCount
    {
        get { return wheelCount; }
        set
        {
            wheelCount += value;
            if (wheelCount == maxWheelCount)
            {
                Color _color;
                ColorUtility.TryParseHtmlString("#FFCC5D", out _color);
                textWheelCount.color = _color;
                textWheel.color = _color;
            }
            else
            {
                Color _color;
                ColorUtility.TryParseHtmlString("#FFFFFF", out _color);
                textWheelCount.color = _color;
                textWheel.color = _color;
            }
            textWheelCount.text = wheelCount.ToString() + "/" + maxWheelCount.ToString();
        }
    }
    public int maxWheelCount = 4;


    [Header("게임실행")]
    [SerializeField] TMP_Text textTimer;
    [SerializeField] float timeCount = 60f;
    public List<Transform> childPartsList;  //배틀씬의 모든 트랜스폼 오브젝트를 담을 리스트변수
    public bool isClick = false;            //부품 오브젝트 드래그 여부 확인용 (드래그 중일 때 포인트 이미지 활성화)
    public bool isGamePlay = false;       //게임 진행 여부
    public bool isGameOver = false;       //게임 진행 여부
    public bool isDontMove = false;       //게임 시작 시 로봇오브젝트를 클릭할 수 없도록...
    [SerializeField] GameObject playerGo;
    [SerializeField] GameObject enemyGo;

    [Header("공격/체력UI")]
    public float ratioPlayerHp;  //플레이어 / Ai 의 최대체력과 현재 체력의 비율
    public float ratioEnemyHp;
    //공격력 UI 표시
    [SerializeField] TMP_Text textAttackPoint;
    [SerializeField] int attackPoint;
    public int _attackPoint
    {
        get
        {
            
            return attackPoint; 
        }
        set
        {
            //_attackPoint = value;
            attackPoint += value;// _attackPoint;
            //Debug.Log("입력한 어택포인트 : " + _attackPoint + " 입력받은 포인트 : " + attackPoint);
            textAttackPoint.text = attackPoint.ToString();
        }
    }

    //체력 UI 표시
    [SerializeField] TMP_Text textHealthPoint;
    public int healthPoint;
    public int _healthPoint
    {
        get 
        {
            return healthPoint; 
        }
        set
        {
            //_healthPoint = value;
            healthPoint += value;// _healthPoint;
            //Debug.Log("입력한 헬스포인트 : " + _healthPoint + " 입력받은 포인트 : " + healthPoint);
            textHealthPoint.text = healthPoint.ToString();
        }
    }

    string[] soundName = { "eff_Common_fail", "eff_Common_clear", "eff_Robot_matching", "eff_Robot_battleStart" };

    private void Start()
    {
        GameObject.Find("ItemPosition").GetComponentsInChildren<Transform>(itemPosTr);
        //battleUiCtrl = this.transform.GetComponentInChildren<RobotUICanvasController>();

        itemPosTr.RemoveAt(0);
        //Debug.Log("배틀씬 어웨이크 발동");
    }

    private void OnEnable()
    {
        BattleSceneSetup();
        StatusSetup();
    }

    void StatusSetup()
    {
        attackPoint = 0;
        textAttackPoint.text = attackPoint.ToString();
        healthPoint = 0;
        textHealthPoint.text = healthPoint.ToString();
    }

    void BattleSceneSetup()
    {
        enemyNumber = Random.Range(0, enemyPrefabs.Length);  //활성화할 적 프리팹 선정

        backgroundImages[0].SetActive(false);
        backgroundImages[1].SetActive(true);

        isGamePlay = false;
        isGameOver = false;
        isDontMove = false;
        itemListTr = new List<Transform>();
        cancelButton.interactable = true;
        battleUiCtrl.gameObject.SetActive(false);
        ParteCountReset();
        if (!inventoryPanel.activeSelf) inventoryPanel.SetActive(true);

        GameObject _playerFrame = Instantiate(playerFramePrefab);
        _playerFrame.transform.SetParent(unitParent.transform);
        _playerFrame.transform.localPosition = new Vector3(-3.75f, -1.25f, 0f);
        activePlayerGo = _playerFrame;

        GravityController[] _allGc = FindObjectsOfType<GravityController>();
        foreach(GravityController _tr in _allGc) _tr.GetComponent<Rigidbody2D>().gravityScale = 0f;

        timeCount = 60f;
        textTimer.text = timeCount.ToString("00");

        armorCount = 0;
        maxArmorCount = 5;
        weaponCount = 0;
        maxWeaponCount = 2;
        wheelCount = 0;
        maxWheelCount = 4;
        textArmorCount.text = armorCount.ToString() + "/" + maxArmorCount.ToString();
        textWeaponCount.text = weaponCount.ToString() + "/" + maxWeaponCount.ToString();
        textWheelCount.text = wheelCount.ToString() + "/" + maxWheelCount.ToString();

    }

    private void OnDisable()
    {
        isGamePlay = false;
        if (activePlayerGo != null) Destroy(activePlayerGo);

        if(unitParent.transform.childCount > 0)
        {
            List<Transform> _childTr = new List<Transform>();
            unitParent.transform.GetComponentsInChildren<Transform>(_childTr);
            _childTr.RemoveAt(0);
            foreach(Transform t in _childTr)
            {
                if (t != null) Destroy(t.gameObject);
            }
        }

        ResetAssemblyParts();
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) if(!isGamePlay && !isGameOver) GameStart();
        CheckResult();
        GameTimer();
    }

    void GameTimer()
    {
        if (isGamePlay) timeCount -= Time.deltaTime;
        textTimer.text = timeCount.ToString("00");
        if (timeCount <= 0f)
        {
            timeCount = 0f;
            textTimer.text = timeCount.ToString("00");
            //승부 결과가 나지 않은 경우...


            int _maxPlayerHp = RobotUICanvasController.uiCtrl.maxPlayerHp;
            int _maxEnemyHp = RobotUICanvasController.uiCtrl.maxEnemyHp;
            int _currentPlayerHp = RobotUICanvasController.uiCtrl.currentPlayerHp;
            int _currentEnemyHp = RobotUICanvasController.uiCtrl.currentEnemyHp;

            ratioPlayerHp = (float)_currentPlayerHp / (float)_maxPlayerHp;
            ratioEnemyHp = (float)_currentEnemyHp / (float)_maxEnemyHp;

            CheckResult();
        }
    }

    void CheckResult()   //게임 결과 확인
    {
        if (isGamePlay && playerGo == null || timeCount <= 0f && ratioPlayerHp < ratioEnemyHp)     //플레이어 패배
        {
            isGamePlay = false;  
            isGameOver = true;
            resultPanel.SetActive(true);
            SoundManager.instance.PlayEffectSound(soundName[0], 1f);

            for(int i = 0; i < resultComments.Length; i++)
            {
                if (i == 2) resultComments[i].enabled = true;
                else resultComments[i].enabled = false;
            }
            StartCoroutine(GameOverSetup());
        }
        else if (isGamePlay && enemyGo == null || timeCount <= 0f && ratioPlayerHp > ratioEnemyHp) //플레이어 승리
        {
            isGamePlay = false;
            isGameOver = true; 
            resultPanel.SetActive(true);
            /*sfxPlayer.clip = victorySound;
            sfxPlayer.Play();*/
            SoundManager.instance.PlayEffectSound(soundName[1], 1f);

            for (int i = 0; i < resultComments.Length; i++)
            {
                if (i == 1) resultComments[i].enabled = true;
                else resultComments[i].enabled = false;
            }
            StartCoroutine(GameOverSetup());
        }

        
    }

    IEnumerator GameOverSetup()
    {
        yield return new WaitForSeconds(5f);

        robotCanvasCtrl.RestartGame();
    }

    public void GameStart()   //게임 스타트 버튼을 누르면 실행될 함수
    {
        isDontMove = true;
        backgroundImages[1].SetActive(false);
        backgroundImages[0].SetActive(true);

        //isGamePlay = true;

        battleUiCtrl.gameObject.SetActive(true);  //전투화면의 UI 활성화
        resultPanel.SetActive(false);
        searchingPanel.SetActive(true);  //상대 검색 중 화면 띄우기

        _enemy = Instantiate(enemyPrefabs[enemyNumber]);
        _enemy.transform.SetParent(unitParent.transform);
        _enemy.transform.localPosition = new Vector3(5f, -2f, 0f);

        //게임오브젝트 (player, enemy 유닛 오브젝트) 의 중력 적용
        GravityController[] allGc = FindObjectsOfType<GravityController>();
        foreach(GravityController _gc in allGc)
        {
            _gc.rb.gravityScale = 1f;
        }

        if (inventoryPanel.activeSelf) inventoryPanel.SetActive(false);

        SoundManager.instance.PlayEffectSound(soundName[2], 1f);

        //게임 시작 시 실행될 내용 작성

        // 1. 무기 사용의 제한 = 게임스타트 bool 변수를 적용하여 시작 직후에는 공격 및 조작을 할 수 없도록 조치
        // 2. 시작 직후부터 조작이 가능한 시간까지의 delay 설정
        //Invoke("UnitHealthSetup", 0.1f);


        //battleUiCtrl.gameObject.SetActive(true);  //전투화면의 UI 활성화
        //searchingPanel.SetActive(true);  //상대 검색 중 화면 띄우기
        //Invoke("HealthBarSetup", 0.2f);

        StartCoroutine(GameStartSetup());
    }

    IEnumerator GameStartSetup()
    {
        /*RobotCanvas.robotInstance.BgmFadeOut();

        while (RobotCanvas.robotInstance.bgmPlayer.isPlaying)
        {
            yield return null;
        }*/
        cancelButton.interactable = false;

        int rand = Random.Range(2, 6);
        //RobotCanvas.robotInstance.BGMPlay(rand);
        yield return new WaitForSeconds(0.1f);
        UnitHealthSetup();


        //battleUiCtrl.gameObject.SetActive(true);  //전투화면의 UI 활성화
        //searchingPanel.SetActive(true);  //상대 검색 중 화면 띄우기

        yield return new WaitForSeconds(0.1f);
        HealthBarSetup();

        yield return new WaitForSeconds(1.5f);
        searchingPanel.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        resultPanel.SetActive(true);
        for (int i = 0; i < resultComments.Length; i++)
        {
            if (i == 0) resultComments[i].enabled = true;
            else resultComments[i].enabled = false;
        }

        yield return new WaitForSeconds(1.5f);
        
        resultPanel.SetActive(false);
        /*sfxPlayer.clip = gameStartSound;
        sfxPlayer.Play();*/
        SoundManager.instance.PlayEffectSound(soundName[3], 1f);

        yield return new WaitForSeconds(0.5f);
        
        isGamePlay = true;
    }


    void UnitHealthSetup()
    {
        RobotBodyHealthCtrl[] bodyCtrl = FindObjectsOfType<RobotBodyHealthCtrl>();
        foreach (RobotBodyHealthCtrl bh in bodyCtrl)
        {
            bh.HealthPointSetup();
        }

        RobotPlayerMoveController _playerMoveCtrl = FindObjectOfType<RobotPlayerMoveController>();
        _playerMoveCtrl.WheelControllerSetup();
    }

    void HealthBarSetup()
    {
        battleUiCtrl.HealthBarSetup();
        battleUiCtrl.AttackButtonSetup();

        playerGo = FindObjectOfType<RobotPlayerMoveController>().gameObject;
        enemyGo = _enemy;
      //  isGamePlay = true;
    }

    public void ResetAssemblyParts()   //조립부품 분해
    {
        Transform[] _allTr = FindObjectsOfType<Transform>();
        foreach(Transform _point in _allTr)
        {
            if(_point.gameObject.layer == LayerMask.NameToLayer("POINT"))
            {
                if(_point.GetComponent<CircleCollider2D>() != null)
                _point.GetComponent<CircleCollider2D>().enabled = true;
            }
        }

        foreach(Transform _tr in itemPosTr)
        {
            if (_tr.childCount > 0) Destroy(_tr.GetChild(0).gameObject);
        }

        
        if (itemListTr.Count > 0 && itemListTr[0] != null)
        {
            for (int i = 0; i < itemListTr.Count;i++)   //활성화된 부품의 트랜스폼을 갖고 있는 리스트를 활용, 활성화된 오브젝트 삭제, 리스트 비우기
            {
                if (itemListTr.Count > 0)
                {
                    if (itemListTr[itemListTr.Count - (i + 1)].gameObject == null) continue;
                    else if (itemListTr[itemListTr.Count - (i + 1)].gameObject != null) Destroy(itemListTr[itemListTr.Count - (i + 1)].gameObject);
                }
                else break;
            }   
        }



        for (int i = 0; i < itemSlots.Length; i++)   //오브젝트 삭제 후 아이템슬롯 활성화 처리
        {
            if (!itemSlots[i].gameObject.activeSelf) itemSlots[i].SetActive(true);
        }

        ParteCountReset();

    }

    void ParteCountReset()   //파츠 생성 가능 카운트 리셋
    {
        itemListTr = new List<Transform>();
        _armorCount = -armorCount;
        _weaponCount = -weaponCount;
        _wheelCount = -wheelCount;
        StatusSetup();

        _healthPoint = 1500;
    }


    public void CheckChildTransform()  //배틀씬 오브젝트 하위의 모든 트랜스폼을 리스트에 넣기
    {
        this.transform.GetComponentsInChildren<Transform>(childPartsList);

        for(int i = 0; i < childPartsList.Count;)  //태그가 Weapon 이 아닌 경우 리스트에서 삭제
        {
            if (childPartsList[i].tag != "Weapon")
            {
                childPartsList.Remove(childPartsList[i]);
            }
            else i++;
        }
    }

    public void PointImageCheck(bool _isClick)
    {
        isClick = _isClick;
        RobotPosSetupController[] _points = FindObjectsOfType<RobotPosSetupController>();
        if (isClick)
        {
            foreach (RobotPosSetupController p in _points)
            {
                p.PointImageOn();
            }
        }
        else if (!isClick)
        {
            foreach (RobotPosSetupController p in _points)
            {
                p.PointImageOff();
            }
        }
    }

}
