using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RescueLevel3Ctrl : MonoBehaviour
{
    public RescueGameCtrl gameCtrl;
    public GameObject AEDWindow;   //AED 오브젝트
    public GameObject CPRWindow;   //CPR 오브젝트
    public TMP_Text guideText;

    [Header("AED")]
    public Image[] bodyPoints;  //패드를 부착할 위치 게임오브젝트  (정답 위치의 번호 : 4번, 5번 == [3],[4])
    public Button aedRunButton; //AED 동작 실행 버튼
    public Image arrowImage;    //화살표 이미지
    public Image[] padLinkImages; //신체에 부착된 패드 이미지
    public int padCount;   //패드를 신체에 부착한 수


    [Header("CPR")]
    public GameObject verticalBar;
    public GameObject vTargetTr;
    public GameObject[] vPositions;
    public GameObject[] hPositions;
    public GameObject horizontalBar;
    public GameObject hTargetTr;
    public float barMoveSpeed;
    public bool isMove = false;
    public bool isVon = false;
    public bool isHon = false;
    public GameObject checkRange;  //판정 범위
    public Button spaceBar;  //스페이스바버튼
    public int cprSucceceCount; //CPR 성공 카운트 (max = 10)
    public GameObject[] succeceBar;  //성공게이지

    public Sprite[] objects;
    public GameObject heartHitAnim;  //CPR 시 나타날 효과를 재생할 오브젝트 (하트가 커지며 사라지기)

    /*public AudioSource sfxPlayer;
    public AudioClip sfxSound;*/

    private void Awake()
    {
        /*sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Marine_AEDon");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.playOnAwake = false;
        sfxPlayer.loop = false;
        sfxPlayer.volume = 0.5f;*/
    }

    private void Start()
    {
        gameCtrl = FindObjectOfType<RescueGameCtrl>();
        barMoveSpeed = 4.5f;
    }

    private void OnEnable()
    {
        ObjectSetting();
        StartCoroutine(ReadyFunction());
    }

    void ObjectSetting()
    {
        AEDWindow.SetActive(true);
        CPRWindow.SetActive(false);
        padCount = 0;
        for (int i = 0; i < bodyPoints.Length; i++)
        {
            bodyPoints[i].gameObject.SetActive(true);
        }
        aedRunButton.enabled = false;
        padLinkImages[0].enabled = false;
        padLinkImages[1].enabled = false;
        arrowImage.enabled = false;
        aedRunButton.GetComponent<Image>().sprite = objects[1]; //21 = 실행버튼 이미지
        guideText.text = "패드를 붙이고 작동버튼을 눌러 AED를 작동 시키세요.";
    }

    IEnumerator ReadyFunction()
    {
        yield return new WaitForFixedUpdate();
        yield return StartCoroutine(gameCtrl.GameReadyFunction(3));
        yield break;
    }

    public void CheckCorrect(GameObject _point)  //AED 패드 부착 위치 정답 여부 확인
    {
        if (_point.name == bodyPoints[3].name || _point.name == bodyPoints[4].name)
        {
            /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Marine_AEDon");
            sfxPlayer.clip = sfxSound;
            sfxPlayer.Play();*/
            SoundManager.instance.PlayEffectSound("eff_Marine_AEDon", 1f);
            padCount++;
            if (padCount == 2)
            {
                if (_point.name == bodyPoints[3].name) padLinkImages[0].enabled = true;
                else if (_point.name == bodyPoints[4].name) padLinkImages[1].enabled = true;
                aedRunButton.enabled = true;
                aedRunButton.GetComponent<Image>().sprite = objects[0];
                arrowImage.enabled = true;
                for (int i = 0; i < bodyPoints.Length; i++)
                {
                    bodyPoints[i].gameObject.SetActive(false);
                }
            }
            else
            {
                gameCtrl.CheckCorrect(true, _point.name, null);
                if (_point.name == bodyPoints[3].name) padLinkImages[0].enabled = true;
                else if (_point.name == bodyPoints[4].name) padLinkImages[1].enabled = true;
                _point.gameObject.SetActive(false);
            }
        }
        else
        {
            /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_wrong");
            sfxPlayer.clip = sfxSound;
            sfxPlayer.Play();*/
            SoundManager.instance.PlayEffectSound("eff_Jellyfish_wrong", 1f);
            gameCtrl.CheckCorrect(false, _point.name, null);
        }
    }

    public void RunningAEDMachine()
    {
        /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Marine_AEDact");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.Play();*/
        SoundManager.instance.PlayEffectSound("eff_Marine_AEDact", 1f);
        Debug.Log("자동제세동기 응급처치 완료!!");
        StartCoroutine(NextStep());  //CPR 단계로 이동
    }

    IEnumerator NextStep()  // CPR 단계로 이동
    {
        yield return new WaitForFixedUpdate();
        yield return StartCoroutine(gameCtrl.RoundSuccessFunction("AED 응급 조치 성공", 3));
        AEDWindow.SetActive(false);
        CPRWindow.SetActive(true);
        CPRgameSetting();
        guideText.text = "스페이스바를 누르거나 화면을 터치해 CPR을 시작하세요.";
        yield return StartCoroutine(gameCtrl.GameReadyFunction(4));
        isMove = true;
    }

    void CPRgameSetting()
    {
        cprSucceceCount = 0;
        verticalBar.transform.position = vPositions[0].transform.position;
        horizontalBar.transform.position = hPositions[0].transform.position;
        for (int i = 0; i < succeceBar.Length; i++)
        {
            succeceBar[i].SetActive(false);  //성공게이지 활성화 - 성공카운트에 따라 변동
        }

        for (int i = 0; i < cprSucceceCount; i++)
        {
            succeceBar[i].SetActive(true);  //성공게이지 활성화 - 성공카운트에 따라 변동
        }
        vTargetTr = vPositions[0];
        hTargetTr = hPositions[0];

    }

    void CPRcountCheck()
    {
        for (int i = 0; i < succeceBar.Length; i++)
        {
            succeceBar[i].SetActive(false);  //성공게이지 활성화 - 성공카운트에 따라 변동
        }

        for (int i = 0; i < cprSucceceCount; i++)
        {
            succeceBar[i].SetActive(true);  //성공게이지 활성화 - 성공카운트에 따라 변동
        }
        if(cprSucceceCount == 10)
        {
            StartCoroutine(gameCtrl.RoundSuccessFunction("CPR 응급 조치 성공", 4));
        }
    }
    
    void MoveVbar()
    {
        verticalBar.transform.position = Vector3.MoveTowards(verticalBar.transform.position, vTargetTr.transform.position, barMoveSpeed * Time.deltaTime);
        if (verticalBar.transform.position == vPositions[0].transform.position) vTargetTr = vPositions[1];
        else if (verticalBar.transform.position == vPositions[1].transform.position) vTargetTr = vPositions[0];
    }

    void MoveHbar()
    {
        horizontalBar.transform.position = Vector3.MoveTowards(horizontalBar.transform.position, hTargetTr.transform.position, barMoveSpeed * Time.deltaTime);
        if (horizontalBar.transform.position == hPositions[0].transform.position) hTargetTr = hPositions[1];
        else if (horizontalBar.transform.position == hPositions[1].transform.position) hTargetTr = hPositions[0];
    }


    private void Update()
    {
        if (isMove)
        {
            if (Input.GetKeyDown(KeyCode.Space)) CPRControll();


            if (Input.GetMouseButtonDown(0)) CPRControll();
        }
        else return;
    }

    IEnumerator HeartHitAnimation(GameObject _heart)  //초록포인트를 맞췄을 때 이펙트 애니메이션 오브젝트 생성 후 삭제 처리
    {
        yield return new WaitForSeconds(0.45f);
        Destroy(_heart);
    }

    private void FixedUpdate()
    {
        if (isMove)
        {
            MoveVbar();
            MoveHbar();
        }
        else return;
    }

    public void CheckTrigger(bool _isTrigger, string _name)
    {
        if (_name == "VerticalBar") isVon = _isTrigger;
        else isHon = _isTrigger;
    }

    public void CPRControll()
    {

        if (isVon && isHon)
        {
            /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Marine_heart");
            sfxPlayer.clip = sfxSound;
            sfxPlayer.Play();*/
            SoundManager.instance.PlayEffectSound("eff_Marine_heart", 1f);
            //판정실행
            cprSucceceCount++;
            //판정 효과 (하트가 커지면서 사라지기)
            GameObject _heartAnim = Instantiate(heartHitAnim, checkRange.transform.position, Quaternion.identity);
            _heartAnim.transform.SetParent(GameObject.Find("Level3Panel").transform);
            StartCoroutine(HeartHitAnimation(_heartAnim));
            if (cprSucceceCount > 9)
            {
                cprSucceceCount = 10;
                isMove = false;
            }
            CPRcountCheck();
        }
        else gameCtrl.CheckCorrect(false, "실패", null);
    }

}
