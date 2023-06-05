using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 우주비행사
/// 랑데부 모드의 우주선 스크립트
/// </summary>

public class SpaceDockingShipController : MonoBehaviour
{
    private SpaceDockingGameController dockingGameCtrl;

    [Header("게임오브젝트")]
    private Rigidbody2D rb;
    public Transform targetTr;
    public Transform[] shipsTr;  //도킹파츠, 우주선본체
    public Transform radarTr;    //레이더
    public GameObject[] effectParticles;
    private ParticleSystem particleSystem;
    public Transform[] spawnTrArray;

    [Header("게임데이터")]
    public float startDelay = 5f;
    public float obstacleTimer = 0.25f;
    float timer = 0.25f;
    private Vector3 dir = Vector3.zero;
    public float moveSpeed = 100f;
    public float rotSpeed = 100f;
    private float controlRotSpeed = 0f;
    public bool isMove = false;
    public bool isDocking = false;
    public float angle;
    public float distance;
    public bool isTryDocking = false;
    int obstacleCount = 0;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dockingGameCtrl = transform.parent.GetComponent<SpaceDockingGameController>();
        obstacleCount = dockingGameCtrl.objPool.obstaclePrefabs.Length;
        particleSystem = effectParticles[0].GetComponent<ParticleSystem>();
        GetAngle();
    }

    private void Update()
    {
        KeyboardMoveControl();  //키보드 조작 (도킹 모드에서의 우주선)

        startDelay -= Time.deltaTime;
        timer -= Time.deltaTime;
        if (timer < 0 && startDelay <0 && !isDocking)
        {
            timer = obstacleTimer;
            GenerateObstacle();
        }
    }


    private void FixedUpdate()
    {
        if (!isDocking)
        {
            if (isMove)
            {
                MoveShip();  //조이스틱 조작에 의한 우주선 이동 동작
            }

            if (controlRotSpeed != 0)
            {
                RotateShip(-controlRotSpeed);  //버튼 조작에 의한 우주선 회전 동작
            }

            isMove = false;
            GetAngle();  //도킹 조건 확인
        }
    }

    #region 우주선 조작

    void KeyboardMoveControl()  //키보드 우주선 이동 조작
    {
        float moveY = Input.GetAxis("Vertical");
        float moveX = Input.GetAxis("Horizontal");

        if (moveY != 0 || moveX != 0)
        {
            isMove = true;
            dir = new Vector3(moveX, moveY, 0).normalized;
        }
    }

    public void GetMoveValue(Vector2 _dir)  //이동 방향값 할당
    {
        isMove = true;
        dir = _dir;
    }


    void MoveShip()  //우주선 이동
    {
        rb.velocity = dir * moveSpeed * Time.deltaTime;
    }

    public void GetRotateValue(float _rotValue)  //좌우 회전값 할당
    {
        controlRotSpeed = _rotValue;
    }

    void RotateShip(float _rotValue)  //좌우회전
    {
        transform.Rotate(new Vector3(0, 0, _rotValue * rotSpeed * Time.deltaTime));
    }

    public void IntoDockingMode()  //도킹 모드 돌입
    {
        isDocking= true;
        SpaceGameManager.instance.lightCtrl.isLight = true;
        rb.velocity = Vector3.zero;
        this.transform.position = Vector3.zero;

        //SpaceGameManager.instance.cameraCtrl.targetTr = null;
        GameObject.Find("Main Camera").GetComponent<SpaceCameraController>().targetTr = null;

        this.transform.position = new Vector3(0f,-2f,0f);
        this.transform.rotation = Quaternion.identity;
        radarTr.gameObject.SetActive(false);
    }

    void GetAngle()   //도킹 조건 확인 - 레이더와 도킹파츠 사이의 각도 구하기
    {
        //도킹 시 거리와 앵글의 범위
        //거리 : 0.5f 이내
        //각도 : 0.3f ~ -0.3f
        //각도와 거리가 위 조건을 충족할 시에 도킹 효과 출력

        float value1 = Mathf.Atan((shipsTr[0].position.y - this.transform.position.y) / (shipsTr[0].position.x - this.transform.position.x));
        float value2 = Mathf.Atan((radarTr.position.y - this.transform.position.y) / (radarTr.position.x - this.transform.position.x));

        angle = value1 - value2;
        //Debug.Log("앵글 : " + angle);
        distance = Vector2.Distance(shipsTr[0].position, radarTr.position);
        //Debug.Log("거리 : " + distance);
    }

    #endregion


    #region 레이더
    private void LateUpdate()
    {
        TargetDirectionUpdate();  //레이더 방향 업데이트
    }

    private void TargetDirectionUpdate()  //레이더 방향 업데이트 (우주정거장의 위치 확인용 레이더)
    {
        Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position, 20f);
        if (colls.Length > 0)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].CompareTag("UNIT"))
                {
                    targetTr = colls[i].transform;
                    break;
                }
            }
        }

        if (targetTr != null)  //방향 레이더 각도/위치 설정
        {
            float angle = Mathf.Atan2(targetTr.position.y - radarTr.position.y, targetTr.position.x - radarTr.position.x) * Mathf.Rad2Deg;
            radarTr.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

            Vector3 radarPos = (targetTr.position - this.transform.position).normalized;
            radarTr.position = radarPos + shipsTr[1].position;
        }
    }
    #endregion


    #region 콜라이더

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDocking) return;

        if (collision.name == "Station" || collision.CompareTag("OBSTACLE")) //실패 연출 (우주선이 우주정거장에 닿을 경우)
        {
            //우주선 이미지 비활성화
            for (int i = 0; i < 3; i++)  
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }

            //폭발 이펙트 재생
            GameObject explosionEffect = Instantiate(effectParticles[1], transform.position, Quaternion.identity);
            explosionEffect.transform.SetParent(shipsTr[1].transform.parent);
            explosionEffect.transform.localScale = Vector3.one;
            explosionEffect.transform.localPosition = Vector3.zero;
            explosionEffect.SetActive(true);
            isMove = false;

            StartCoroutine(RendezvousFail());  //랑데부 실패
        }

        if (collision.name == "DockPos")  //도킹 이펙트 효과 (도킹이 가능한 상태가 되면 도킹 부위에 이펙트 재생)
        {
            if (distance < 0.5f)
            {
                if (angle > -0.3f && angle < 0.3f)
                {
                    dockingGameCtrl.SetToDockingZone(true);
                    effectParticles[0].SetActive(true);
                    if (!particleSystem.isPlaying)
                    {
                        particleSystem.Play();                        
                    }
                    //GameObject hitEffect = Instantiate(effectParticles[0], transform.position, Quaternion.identity);
                    //hitEffect.transform.SetParent(shipsTr[0].transform.parent);
                    //hitEffect.transform.localScale = Vector3.one;
                    //hitEffect.transform.localPosition = new Vector3(0, 0.7f, 0);
                    //hitEffect.SetActive(true);
                }
            }
        }
    }

    IEnumerator RendezvousFail()  //랑데부 실패
    {
        yield return new WaitForSeconds(1);
        SpaceGameManager.instance.FailRendezvousMission();
        Destroy(this.gameObject);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDocking) return;

        if (collision.name == "DockPos")  //도킹 파츠가 서로 닿아있는 상태
        {
            if (distance < 0.5f)
            {
                if (angle > -0.3f && angle < 0.3f)
                {
                    dockingGameCtrl.isTryDocking = true;
                }
            }
            else
            {
                dockingGameCtrl.isTryDocking = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isDocking) return;

        if (collision.name == "DockPos")  //도킹 파츠가 떨어지면,
        {
            dockingGameCtrl.SetToDockingZone(false);
        }
    }

    #endregion



    #region 장애물 생성

    void GenerateObstacle()
    {
        dockingGameCtrl.objPool.GetObstacle(Random.Range(0, obstacleCount), spawnTrArray[Random.Range(0, spawnTrArray.Length)]);
    }

    #endregion


}
