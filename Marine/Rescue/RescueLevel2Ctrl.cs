using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RescueLevel2Ctrl : MonoBehaviour
{
    public RescueGameCtrl gameCtrl;
    public float currentHealthBarYPos; //헬스바의 위치 (체력에 따라 이동)
    public int health;  //체력

    public Transform healthBarImage;

    public GameObject gameGaugeBar; //좌우로 움직이는 게이지바
    public float moveSpeed;
    public bool isMove;
    public Transform targetTr;
    public Transform[] movePoints;  //게이지바의 목표지점 (왼쪽/오른쪽)
    public GameObject hitPoint;

    public GameObject[] gaugePoints; //가로 게임게이지바의 초록색 포인트 오브젝트
    public GameObject[] activeGaugePoints = new GameObject[3]; //활성화된 게이지포인트
    public int gaugePointsNumber1; //활성화된 게이지포인트의 번호를 저장할 변수
    public int gaugePointsNumber2; //활성화된 게이지포인트의 번호를 저장할 변수
    public int gaugePointsNumber3; //활성화된 게이지포인트의 번호를 저장할 변수
    public GameObject gaugePointAnim;

    /*public AudioSource sfxPlayer;
    public AudioClip sfxSound;*/

    private void Awake()
    {
        /*sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Marine_air");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.playOnAwake = false;
        sfxPlayer.loop = false;
        sfxPlayer.volume = 0.5f;*/
    }
    private void Start()
    {
        gameCtrl = FindObjectOfType<RescueGameCtrl>();
        moveSpeed = 3.5f;
        isMove = false;
        targetTr = movePoints[0];
    }


    private void OnEnable()
    {
        health = 50;
        HealthBarSetting();

        activeGaugePoints[0] = null;
        activeGaugePoints[1] = null;
        activeGaugePoints[2] = null;
        gaugePointsNumber1 = 13;
        gaugePointsNumber2 = 13;
        gaugePointsNumber3 = 13;
        hitPoint = null;
        PointsMoving();  //게이지바 포인트 생성 (3개)
        StartCoroutine(HealthBarCounting());  //체력바 감소 코루틴
    }

    void HealthBarSetting()   //인공호흡 체력바 초기화
    {
        //게임게이지바 위치 초기화 (게이지바 왼쪽)
        gameGaugeBar.transform.position = movePoints[0].position;

        currentHealthBarYPos = 0f;
        if (health >0)
        {
            if (health > 50) currentHealthBarYPos += (health * 1.2f);
            else currentHealthBarYPos -= ((health-50) * 1.2f);
        }
        else
        {
            currentHealthBarYPos = -120f;
        }
        healthBarImage.localPosition = new Vector3(0f, currentHealthBarYPos, 0f);

        //게임게이지바의 초록 포인트 비활성화
        for(int i = 0; i < gaugePoints.Length; i++)
        {
            gaugePoints[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (isMove)
        {
            GameGaugeBarMove();
            if (Input.GetKeyDown(KeyCode.Space))  HealthUpDown("UP");

            if (Input.GetMouseButtonDown(0))
            {
                HealthUpDown("UP");
            }
        }
    }

    void GameGaugeBarMove()  //좌우로 움직이는 게이지바
    {
        gameGaugeBar.transform.position = Vector3.MoveTowards(gameGaugeBar.transform.position, targetTr.position, moveSpeed * Time.deltaTime);

        if (gameGaugeBar.transform.position == movePoints[0].transform.position) targetTr = movePoints[1];
        else if (gameGaugeBar.transform.position == movePoints[1].transform.position) targetTr = movePoints[0];
    }

    IEnumerator HealthBarCounting()  //인공호흡 체력 카운트
    {
        yield return new WaitForFixedUpdate();
        yield return StartCoroutine(gameCtrl.GameReadyFunction(2));
        isMove = true;
        while (health != 100 && health != 0)
        {
            if(isMove)  HealthUpDown("DOWN");
            yield return new WaitForSeconds(0.2f);
        }


        isMove = false;
        if (health == 0)
        {
            gameCtrl.GoToNextLevel(3, false);

            /////추가부분 (체력이 0 이되면 생명력 1 감소 후 체력 50 으로 재할당
            //gameCtrl.CheckCorrect(false, "hitPoint");
            //if (gameCtrl.lifeCount > 0)
            //{
            //    yield return new WaitForSeconds(1f);
            //    health = 70;
            //    HealthUpDown("UP");
            //    StartCoroutine(HealthBarCounting());
            //}
            /////
        }
        else if (health == 100)
        {
            gameCtrl.GoToNextLevel(3, true); 
        }
        

        yield break;
    }


    public void CheckTriggerGaugeBar(GameObject _gaugePoint)  //게이지바와 초록포인트 오브젝트의 충돌 여부 확인
    {
        if (_gaugePoint != null) hitPoint = _gaugePoint;
        else  hitPoint = null;
    }


    void PointsMoving()  //게이지포인트 생성함수
    {
        if (activeGaugePoints[0] == null)
        {
            int rand = Random.Range(0, 12);

            while (rand == gaugePointsNumber1 || rand == gaugePointsNumber2 || rand == gaugePointsNumber3)  //숫자 중복 여부 확인
            { rand = Random.Range(0, 12); }

            gaugePointsNumber1 = rand;
            gaugePoints[rand].SetActive(true);
            activeGaugePoints[0] = gaugePoints[rand];
        }
        if (activeGaugePoints[1] == null)
        {
            int rand = Random.Range(0, 12);

            while (rand == gaugePointsNumber1 || rand == gaugePointsNumber2 || rand == gaugePointsNumber3)  //숫자 중복 여부 확인
            { rand = Random.Range(0, 12); }

            gaugePointsNumber2 = rand;
            gaugePoints[rand].SetActive(true);
            activeGaugePoints[1] = gaugePoints[rand];
        }
        if (activeGaugePoints[2] == null)
        {
            int rand = Random.Range(0, 12);

            while (rand == gaugePointsNumber1 || rand == gaugePointsNumber2 || rand == gaugePointsNumber3)  //숫자 중복 여부 확인
            { rand = Random.Range(0, 12); }

            gaugePointsNumber3 = rand;
            gaugePoints[rand].SetActive(true);
            activeGaugePoints[2] = gaugePoints[rand];
        }
    }


    public void HealthUpDown(string _value)   //체력바 증감 함수 (인자에 따라 변경)
    {
        if (_value == "UP")   //스페이스바 입력
        {
            if (hitPoint != null)  // hitPoint == 초록포인트의 게임오브젝트
            {
                for (int i = 0; i < activeGaugePoints.Length; i++)
                {
                    if (hitPoint == activeGaugePoints[i])
                    {
                        /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Marine_air");
                        sfxPlayer.clip = sfxSound;
                        sfxPlayer.Play();*/
                        SoundManager.instance.PlayEffectSound("eff_Marine_air", 1f);
                        health += 10;

                        //activeGaugePoints[i].SetActive(false);

                        ///애니메이션 오브젝트 생성
                        GameObject _pointAnim = Instantiate(gaugePointAnim, hitPoint.transform.position, Quaternion.identity);
                        _pointAnim.transform.SetParent(GameObject.Find("GameGaugeBar").transform);
                        StartCoroutine(AirPointHitAction(_pointAnim));
                        ///
                        hitPoint.SetActive(false);
                        activeGaugePoints[i] = null;
                        hitPoint = null;
                        PointsMoving();
                        if (health > 100)
                        {
                            isMove = false;
                            health = 100;
                        }
                        break;
                    }
                }
            }
            else
            {
                ///// 생명력 1 감소 대신, 체력 감소
                //health -= 20;
                //////
                /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_wrong");
                sfxPlayer.clip = sfxSound;
                sfxPlayer.Play();*/
                SoundManager.instance.PlayEffectSound("eff_Jellyfish_wrong", 1f);
                gameCtrl.CheckCorrect(false, "hitPoint",null);
            }
        }
        else
        {
            health--;   //시간경과 (0.2초마다)
            if (health < 0)
            {
                isMove = false;
                health = 0;
            }
        }

        currentHealthBarYPos = 0f;

        if (health > 0)
        {
            if (health > 50) currentHealthBarYPos += ((health - 50) * 2.4f);
            else currentHealthBarYPos -= ((50 - health) * 2.4f);
        }
        else
        {
            health = 0;
            currentHealthBarYPos = -120f;
        }
        healthBarImage.localPosition = new Vector3(0f, currentHealthBarYPos, 0f);
    }

    IEnumerator AirPointHitAction(GameObject _pointAnim)  //초록포인트를 맞췄을 때 이펙트 애니메이션 오브젝트 생성 후 삭제 처리
    {
        yield return  new WaitForSeconds(0.45f);
        Destroy(_pointAnim);
    }

}
