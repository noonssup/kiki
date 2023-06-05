using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum PatrolState
{
    PATROL = 0,
    CHASE = 1,
    FIX = 2,
    SAFE = 3,
    WAIT=4,
    MOVECHANGE=5,
}


public class UrbanPatrolUnit : MonoBehaviour
{
    public UrbanSpawnController spawnCtrl;
    public UrbanCriminalUnit criminalUnit;
    public PatrolState state;
    public Transform targetTr = null;
    public Transform criminalTargetTr = null;
    public List<Transform> wayPointsTr = new List<Transform>();
    public int nodeCount = 0;

    public float waitTimerLimit = 1f;
    float waitTimer = 0f;

    public float chaseTimerLimit = 10f;
    float chaseTimer = 0f;

    public float safeTimerLimit = 5f;
    float safeTimer = 0f;

    public float patrolMoveSpeed = 1f;
    public float chaseMoveSpeed = 1.75f;

    public GameObject safeRange;
    public float safeRadius = 15f;


    void Start()
    {
        state = PatrolState.WAIT;
        waitTimer = waitTimerLimit;
        chaseTimer = chaseTimerLimit;
        safeTimer = safeTimerLimit;
        safeRange.SetActive(false);
    }

    public void SpawnControllerSetup(UrbanSpawnController _spawnCtrl)  //경찰차 초기화
    {
        spawnCtrl = _spawnCtrl;
        bottomLeft = spawnCtrl.bottomLeft;
        topRight = spawnCtrl.topRight;
        wayPointsTr = spawnCtrl.spawnPointList;
        transform.SetParent(spawnCtrl.transform);
    }

    void Update()
    {
        if (state == PatrolState.WAIT)
        {
            waitTimer -= Time.deltaTime;

            if(waitTimer < 0f) PatrolTargetSetup();

            //AutoChase();  //자동 출동 (테스트용)
        }

        if (state == PatrolState.FIX && UrbanGameManager.instance.isGamePlay)
        {
            FixCrime();
        }
    }

    void AutoChase()
    {
        chaseTimer -= Time.deltaTime;
        if (chaseTimer < 0f)
        {
            int randomCriminal = Random.Range(0, spawnCtrl.criminalUnitList.Count);
            spawnCtrl.criminalUnitList[randomCriminal].GetComponent<UrbanCriminalUnit>().CallPolice();
            chaseTimer = chaseTimerLimit;
        }
    }


    //void SetWayPointsTransform()
    //{
    //    wayPointsTr = new List<Transform>();
    //    //wayPointsTr = UrbanGameManager.instance.spawnPoints;
    //}


    public void SetTarget(Transform _target, PatrolState _state)  //목표 지점 설정
    {
        state = _state;
    }


    public void RemoveTarget()  //대기
    {
        targetTr = null;
        safeRange.SetActive(false);
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
        state = PatrolState.WAIT;
    }


    void PatrolTargetSetup()  //순찰 목표지점 설정
    {
        int index = Random.Range(0,wayPointsTr.Count);
        Transform _tr = wayPointsTr[index];
        PathFinding(_tr, PatrolState.PATROL);
        waitTimer = waitTimerLimit;
    }



    public void Chase(Transform _criminal)  //경찰차 출동 설정
    {
        this.RemoveTarget();

        PlaySoundEffect("eff_UrbanChaseSiren");

        this.state = PatrolState.MOVECHANGE;
        this.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
        criminalTargetTr = _criminal;
        this.PathFinding(_criminal, PatrolState.CHASE);


    }


    void FixedUpdate()
    {
        UnitSetMove();
    }

    void UnitSetMove()  //state 에 따른 동작
    {
        if (!UrbanGameManager.instance.isGamePlay) return;
        switch (state)
        {
            case PatrolState.PATROL: PatrolMove(); break;
            case PatrolState.CHASE: ChaseMove(); break;
            case PatrolState.FIX: if(criminalUnit == null || targetTr == null) RemoveTarget(); break;
            case PatrolState.SAFE: Safe(); break;
            case PatrolState.WAIT: break;
            case PatrolState.MOVECHANGE: break;
        }
    }



    void PatrolMove()  //순찰 이동
    {
        if (FinalNodeList.Count <= 0) return;

        Vector2 nodePos = new Vector2(FinalNodeList[nodeCount].x, FinalNodeList[nodeCount].y);
        Vector2 myPos = new Vector2(this.transform.position.x, this.transform.position.y);
        if (myPos != nodePos)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, nodePos, Time.deltaTime * patrolMoveSpeed);

        }
        else {
            nodeCount++;
            if (nodeCount >= FinalNodeList.Count)
            {
                RemoveTarget();
            }
        }
    }

    void ChaseMove()  //출동 이동
    {
        if (FinalNodeList.Count <= 0) return;
        
        if(criminalTargetTr == null)  //타겟이 없어질 경우 WAIT 상태로 변경
        {
            RemoveTarget();
        }

        Vector2 nodePos = new Vector2(FinalNodeList[nodeCount].x, FinalNodeList[nodeCount].y);
        Vector2 myPos = new Vector2(this.transform.position.x, this.transform.position.y);
        if (myPos != nodePos)
        {
            this.transform.position = Vector2.MoveTowards(this.transform.position, nodePos, Time.deltaTime * chaseMoveSpeed);

        }
        else
        {
            nodeCount++;
            if (nodeCount >= FinalNodeList.Count)
            {
                state = PatrolState.FIX;
            }
        }
    }


    void FixCrime()  //범죄 대응 중
    {
        if (criminalUnit != null)
        {
            criminalUnit.Damage();
        }
    }

    void Safe()
    {
        safeTimer -= Time.deltaTime;
        safeRange.transform.localScale -= Vector3.one * Time.deltaTime;
        if(safeTimer <= 0f)
        {
            state = PatrolState.WAIT;
            safeTimer = safeTimerLimit;
            safeRange.SetActive(false);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("ENEMY"))
        {
            //criminalUnit = other.GetComponent<UrbanCriminalUnit>();
            //if (criminalUnit.police != null && criminalUnit.police != this.gameObject) return;
            criminalUnit = other.GetComponent<UrbanCriminalUnit>();
            if (criminalUnit.isFix) return;

            criminalTargetTr = other.transform;
            criminalUnit.police = this.gameObject;
            this.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;

            PlaySoundEffect("eff_UrbanFixSiren");

            state = PatrolState.FIX;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (criminalUnit == null|| criminalUnit.police != null && criminalUnit.police != this.gameObject) return;

        if (collision.CompareTag("ENEMY"))
        {
            criminalUnit = null;
            criminalTargetTr = null;
            this.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
            chaseTimer = chaseTimerLimit;
            //세이프 모드로 변경
            state = PatrolState.SAFE;
            safeRange.SetActive(true);
            safeRange.transform.localScale = Vector3.one * safeRadius;
        }
    }


    //효과음 재생 (씬 연동 시 사운드매니저 사용)
    void PlaySoundEffect(string _fileName)
    {
        SoundManager.instance.PlayEffectSound(_fileName, 1f);
    }







    #region 길찾기

    [Header("길찾기")]
    public List<Node> FinalNodeList;
    public Vector2Int bottomLeft, topRight, startPos, targetPos;
    public int sizeX, sizeY;
    Node[,] NodeArray;
    public Node startNode, targetNode, curNode;
    public List<Node> openList, closedList;

    public void PathFinding(Transform _goalTr, PatrolState _state)  //이동 경로 찾기
    {
        //Debug.Log(this.gameObject.name + "패쓰파인딩");
        targetTr = _goalTr;
        nodeCount = 0;
        //nodeArray 크기 설정, 경로 중 벽(여기서는 범죄자) 존재 여부 확인
        sizeX = topRight.x - bottomLeft.x + 1;
        sizeY = topRight.y - bottomLeft.y + 1;
        NodeArray = new Node[sizeX, sizeY];

        for (int i = 0; i < sizeX; i++)
        {
            for (int j = 0; j < sizeY; j++)
            {
                bool isWall = false;
                //foreach (Collider2D col in Physics2D.OverlapCircleAll(new Vector2(i + bottomLeft.x, j + bottomLeft.y), 0.4f))
                //{
                //    if (col.gameObject.layer == LayerMask.NameToLayer("ENEMY"))
                //    {
                //        isWall = true;
                //    }
                //}

                NodeArray[i, j] = new Node(isWall, i + bottomLeft.x, j + bottomLeft.y);
            }
        }

        // 출발/도착 노드 설정, 열린리스트 닫힌리스트, 마지막리스트 초기화
        startNode = NodeArray[(int)this.transform.position.x - bottomLeft.x, (int)this.transform.position.y - bottomLeft.y];
        targetNode = NodeArray[(int)targetTr.position.x - bottomLeft.x, (int)targetTr.position.y - bottomLeft.y];

        openList = new List<Node>() { startNode };
        closedList = new List<Node>();
        FinalNodeList = new List<Node>();

        while (openList.Count > 0)
        {
            //열린리스트 중 F가 가장 작거나 같다면 H가 작은 걸 현재노드로 하고 열린리스트에서 닫힌 리스트로 옮기기
            curNode = openList[0];

            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].F <= curNode.F && openList[i].H < curNode.H)
                {
                    curNode = openList[i];
                }
            }

            openList.Remove(curNode);
            closedList.Add(curNode);

            if (curNode == targetNode)
            {
                Node targetCurNode = targetNode;

                while (targetCurNode != startNode)
                {
                    FinalNodeList.Add(targetCurNode);
                    targetCurNode = targetCurNode.ParentNode;
                }
                FinalNodeList.Add(startNode);
                FinalNodeList.Reverse();
                state = _state;
                return;
            }

            //위아래양옆 노드 오픈리스트에 추가
            OpenListAdd(curNode.x, curNode.y + 1);
            OpenListAdd(curNode.x + 1, curNode.y);
            OpenListAdd(curNode.x, curNode.y - 1);
            OpenListAdd(curNode.x - 1, curNode.y);
        }
    }

    void OpenListAdd(int _checkX, int _checkY)
    {
        //상하좌우 범위를 벗어나지 않고 벽이 아니면서 닫힌리스트에 없다면
        if (_checkX >= bottomLeft.x && _checkX < topRight.x + 1 && _checkY >= bottomLeft.y && _checkY < topRight.y + 1  //상하좌우 범위 확인
            && !NodeArray[_checkX - bottomLeft.x, _checkY - bottomLeft.y].isWall   //상하좌우가 벽인지 확인
            && !closedList.Contains(NodeArray[_checkX - bottomLeft.x, _checkY - bottomLeft.y]))  //닫힌리스트에 있는지 확인
        {
            //이웃노드에 넣어주고
            Node neighberNode = NodeArray[_checkX - bottomLeft.x, _checkY - bottomLeft.y];

            //직선은 10, 대각선은 14 비용 적용
            int moveCost = curNode.G + (curNode.x - _checkX == 0 || curNode.y - _checkY == 0 ? 10 : 14);

            //이동비용이 이웃노드G보다 작거나 열린리스트에 이웃노드가 없다면 G,H,ParentNode를 설정 후 열린리스트에 추가
            if (moveCost < neighberNode.G || !openList.Contains(neighberNode))
            {
                neighberNode.G = moveCost;
                neighberNode.H = (Mathf.Abs(neighberNode.x - targetNode.x) + Mathf.Abs(neighberNode.y - targetNode.y)) * 10;
                neighberNode.ParentNode = curNode;

                openList.Add(neighberNode);
            }
        }
    }

    void OnDrawGizmos()  //경로 기즈모 그리기
    {
        Gizmos.color = Color.red;
        if (FinalNodeList.Count != 0)
        {
            for (int i = 0; i < FinalNodeList.Count - 1; i++)
            {
                Gizmos.DrawLine(new Vector2(FinalNodeList[i].x, FinalNodeList[i].y), new Vector2(FinalNodeList[i + 1].x, FinalNodeList[i + 1].y));
            }
        }
    }

    #endregion

}
