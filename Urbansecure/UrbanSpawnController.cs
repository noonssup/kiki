using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class Node
{
    public Node(bool _isWall, int _x, int _y)
    {
        isWall = _isWall;
        x = _x;
        y = _y;
    }

    public bool isWall = false;
    public Node ParentNode;

    public int x, y, G, H;
    public int F { get { return G + H; } }
}



public class UrbanSpawnController : MonoBehaviour
{

    public List<Transform> spawnPointList = new List<Transform>();
    public Transform grid;

    [Header("경찰")]
    public Transform patrolGroupTr;
    public List<GameObject> patrolUnitList;// = new List<GameObject>();
    public int maxPatrolUnit = 4;
    public int patrolUnitCount = 2;
    public GameObject patrolUnitPrefab;
    public GameObject callPatrolUnit;


    [Header("범죄")]
    public Transform criminalGroupTr;
    public List<GameObject> criminalUnitList = new List<GameObject>();
    public int maxCriminalUnit = 5;
    public int criminalUnitCount = 3;
    public GameObject criminalUnitPrefab;
    public float criminalSpawnTime = 5f;
    float criminalSpawntimer = 0f;

    [Header("길찾기")]
    public Vector2Int bottomLeft, topRight, startPos, targetPos;


    private void Update()
    {
        if (UrbanGameManager.instance.isGamePlay && criminalUnitList.Count < criminalUnitCount)
        {
            criminalSpawntimer -= Time.deltaTime;
            if(criminalSpawntimer <= 0) CreateCriminalUnit();
        }
    }

    void SetWayPoints()  //웨이포인트 설정
    {
        var group = grid;
        if (group != null)
        {
            group.GetComponentsInChildren<Transform>(spawnPointList);
            spawnPointList.RemoveAt(0);
        }
    }

    void GameDataInit() //게임데이터 설정
    {
        patrolUnitList = new List<GameObject>();
        criminalSpawnTime = 5f;
        criminalSpawntimer = 1f;  //게임시작 1초 후 범죄 발생
        //초기 범죄, 요원 수 설정 (남은 시간이 2분, 1분이 되면 각각 1 증가)
        criminalUnitCount = 3;
        patrolUnitCount = 2;
        CreatePatrolUnit();
    }

    public void GameStart()  //게임매니저에서 게임 시작 함수를 실행하면...
    {
        SetWayPoints();
        GameDataInit();
        UrbanGameManager.instance.isGamePlay = true;
    }

    void CreatePatrolUnit()  //경찰 생성
    {
        int _count = patrolUnitList.Count;
        for (int i = _count; i < patrolUnitCount; i++)
        {
            int randomIndex = Random.Range(0, spawnPointList.Count);
            GameObject _unit = Instantiate(patrolUnitPrefab, spawnPointList[randomIndex].position, Quaternion.identity);
            _unit.GetComponent<UrbanPatrolUnit>().SpawnControllerSetup(this);
            _unit.gameObject.name = "Patrol" + (i+1).ToString("00");
            //_unit.transform.SetParent(patrolGroupTr);
            patrolUnitList.Add(_unit);
        }
    }

    void CreateCriminalUnit()  //범죄 생성
    {
        int count = 0;
        while (count < 1)
        {
            int randomIndex = Random.Range(0, spawnPointList.Count);
            UrbanWayPoint point = spawnPointList[randomIndex].GetComponent<UrbanWayPoint>();
            if(point.state == UrbanWayPointState.NORMAL)
            {
                GameObject _unit = Instantiate(criminalUnitPrefab, spawnPointList[randomIndex].position, Quaternion.identity);
                _unit.transform.SetParent(criminalGroupTr);
                _unit.GetComponent<UrbanCriminalUnit>().spawnCtrl = this;
                criminalUnitList.Add(_unit);
                point.state = UrbanWayPointState.DANGER;
                count++;
            }
        }
        criminalSpawntimer = criminalSpawnTime;
    }

    public void AddUnit()  //시간 경과에 따른 유닛 추가
    {
        criminalUnitCount++;
        if (criminalUnitCount >= maxCriminalUnit) criminalUnitCount = maxCriminalUnit;
        criminalSpawnTime--;
        if (criminalSpawnTime <= 2f) criminalSpawnTime = 2.5f;
        patrolUnitCount++;
        if (patrolUnitCount >= maxPatrolUnit) patrolUnitCount = maxPatrolUnit;
        CreatePatrolUnit();
        CreateCriminalUnit();
    }

    public void DestroyCriminalUnit(GameObject _unit)  //범죄 삭제
    {
        UrbanGameManager.instance.FixCount += 1;

        criminalUnitList.Remove(_unit);
        Destroy(_unit);
    }


    public GameObject CallPolice(GameObject _criminal)  //신고
    {
        //순찰 중인 경찰 찾기
        foreach (GameObject go in patrolUnitList)
        {
            UrbanPatrolUnit _unit = go.GetComponent<UrbanPatrolUnit>();
            if (_unit.state == PatrolState.PATROL || _unit.state == PatrolState.WAIT)
            {
                _unit.transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = true;
                _unit.Chase(_criminal.transform);

                return _unit.gameObject;
            }
        }
        return null;
    }

    public void DeleteUnitObject()  //경찰 / 범죄 유닛 제거
    {
        GoListInit(ref patrolUnitList);
        GoListInit(ref criminalUnitList);
    }

    void GoListInit(ref List<GameObject> _list)
    {
        if (_list.Count > 0)
        {
            foreach (GameObject go in _list)
            {
                Destroy(go);
            }
            _list = new List<GameObject>();
        }
    }

}
