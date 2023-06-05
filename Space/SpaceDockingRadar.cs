using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 우주비행사
/// 도킹 모드의 도킹 레이더 스크립트
/// 우주선과 정거장의 랑데뷰 후 도킹 버튼을 누르면 레이더가 활성화되어
/// 도킹을 위한 동작 수행
/// </summary>

public class SpaceDockingRadar : MonoBehaviour
{
    [Header("스크립트컨트롤러")]
    public SpaceDockingMoveRadar moveRadar;

    [Header("게임오브젝트")]
    public Rigidbody2D radarRb;
    public Image dockingGaugeBar;

    [Header("게임데이터")]
    public float moveSpeed = 2f;
    public float controlPower = 60f;
    public float moveXRange = 2.5f;
    public float moveYRange = 2f;
    private Vector2[] movePosition= new Vector2[6];
    public Vector2 targetPos= Vector2.zero;
    public Vector2 controlMoveDirection = Vector2.zero;
    private float coolTime = 3f;
    public float radarMoveCoolTime = 5f;


    private void Start()
    {
        movePosition[0] = new Vector2(moveXRange, (-moveYRange+1));
        movePosition[1] = new Vector2(-moveXRange, moveYRange);
        movePosition[2] = new Vector2(moveXRange, moveYRange);
        movePosition[3] = new Vector2(-moveXRange, (-moveYRange+1));
        movePosition[4] = new Vector2(-moveXRange, 0f);
        movePosition[5] = new Vector2(moveXRange, 0f);
        radarRb.drag = 1;
        TargetPositionSetup();
    }

    void TargetPositionSetup()   //타겟 포지션 설정
    {
        int randomIndex = Random.Range(0, 6);
        targetPos = movePosition[randomIndex];
    }

    private void FixedUpdate()
    {
        if (moveRadar.isMove)  //타겟포지션이 정해진 상태에서 레이더의 isMove = true 일 경우
        {
            if ((Vector2)moveRadar.transform.position != targetPos)
            {
                moveRadar.transform.position = Vector2.MoveTowards(moveRadar.transform.position, targetPos, moveSpeed * Time.deltaTime);
            }
            else
            {
                TargetPositionSetup();
            }

            if (((Vector2)moveRadar.transform.position - targetPos).magnitude <= 0.1f)
            {
                TargetPositionSetup();
            }

            MoveRadar();  //조이스틱으로 레이더 조작
        }

        //타겟포지션을 벗어날 경우 방향값 속도값 초기화
        if(moveRadar.transform.position.x > moveXRange || moveRadar.transform.position.x < -moveXRange)
        {
            controlMoveDirection = Vector2.zero;
            radarRb.velocity = Vector2.zero;
            TargetPositionSetup();
        }
        if (moveRadar.transform.position.y > moveYRange || moveRadar.transform.position.y < -moveYRange)
        {
            controlMoveDirection = Vector2.zero;
            radarRb.velocity = Vector2.zero;
            TargetPositionSetup();
        }
    }

    public void GetMoveDirection(Vector2 _dir)  //레이더 조작 이동 방향
    {
        //게임종료가 되면 레이더 움직이지 않도록
        if (!moveRadar.isMove)
        {
            controlMoveDirection = Vector2.zero;
            radarRb.velocity = Vector2.zero; 
            return;
        }

        //스틱 조작에 따른 위치 이동
        controlMoveDirection = _dir;
        coolTime -= Time.deltaTime;
        if (coolTime < 0)
        {
            TargetPositionSetup();
            coolTime = radarMoveCoolTime;
        }
    }


    void MoveRadar()  //레이더 이동
    {
        radarRb.velocity = controlMoveDirection * (moveSpeed* controlPower) * Time.deltaTime;
    }


    public void SetDockingGauge(float _currentValue, float _maxValue)  //도킹게이지 설정
    {
        dockingGaugeBar.fillAmount = _currentValue / _maxValue;
    }

}
