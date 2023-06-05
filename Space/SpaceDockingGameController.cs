using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 우주비행사
/// 도킹 파트 게임컨트롤 스크립트
/// </summary>

public class SpaceDockingGameController : MonoBehaviour
{
    private SpaceDockingShipController shipCtrl;
    private SpaceDockingStationController stationCtrl;
    private SpaceDockingRadar radarCtrl;
    public SpaceObjectPool objPool;

    [Header("게임오브젝트")]
    public GameObject dockingShip;
    public GameObject dockingStation;
    public GameObject obstacle;
    public GameObject[] currentShipStation = new GameObject[2];
    public GameObject dockingRadar;
    public Button[] rotateButtons;
    public Button dockingButton;

    [Header("게임데이터")]
    public bool isDocking = false;
    public bool isTryDocking = false;
    public bool isDockingMode = false;
    float obstacleStartTimer = 6;
    public float obstacleDelayTimer = 0.25f;
    float obstacleTimer = 0;
    public int obstacleGenerateCount = 2;
    List<GameObject> obstacles = new List<GameObject>();

    [Header("텍스트UI")]
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textDistance;


    public void RendezvousDockingModeSetup()  //초기 게임셋업
    {
        textTimer.text = "";
        textDistance.text = "";

        isDocking = false;
        isDockingMode = false;
        isTryDocking = false;
        dockingRadar.SetActive(false);
        dockingButton.interactable = false;

        //재시작 시 기존에 존재하던 우주선/정거장 삭제 후 재생성
        for (int i = 0; i < currentShipStation.Length; i++)
        {
            if (currentShipStation[i] != null)
            {
                Destroy(currentShipStation[i].gameObject);
                currentShipStation[i] = null;
            }
        }

        currentShipStation[0] = Instantiate(dockingShip, transform.position, Quaternion.identity);
        currentShipStation[1] = Instantiate(dockingStation, dockingStation.transform.position, Quaternion.identity);
        for (int i = 0; i < currentShipStation.Length; i++)
        {
            currentShipStation[i].transform.SetParent(transform, false);
        }
        shipCtrl = currentShipStation[0].GetComponent<SpaceDockingShipController>();
        stationCtrl = currentShipStation[1].GetComponent<SpaceDockingStationController>();

        //기존 생성된 장애물 오브젝트가 있다면 삭제
        if (objPool.transform.childCount > 0)
        {
            List<Transform> list = new List<Transform>();
            list.AddRange(objPool.transform.GetComponentsInChildren<Transform>());
            list.RemoveAt(0);
            foreach(Transform obj in list)
            {
              if(obj.gameObject.activeSelf) obj.gameObject.SetActive(false);
            }
        }

        //foreach (GameObject go in obstacles)
        //{
        //    if (go != null)
        //    {
        //        Destroy(go);
        //    }
        //    else continue;
        //}
        //obstacles.Clear();

        //장애물 오브젝트풀링 셋업
        //SpaceObjectPooler.inst.SetupObstacle();
        obstacleTimer = obstacleStartTimer;

        //카메라 설정
        GameObject.Find("Main Camera").GetComponent<SpaceCameraController>().GetGameMode(GameMode.DockingMode, currentShipStation[0].transform);
    }

    #region 우주선 / 도킹레이더 조작

    public void SetShipMoveDirection(Vector2 _dir)    //우주선 이동 방향값 전달 
    {
        shipCtrl.GetMoveValue(_dir);
    }

    public void OnClickRotate(float _value)  //우주선 회전 (버튼 이벤트트리거에 좌우각각 -1, 1 할당)
    {
        shipCtrl.GetRotateValue(_value);

        //회전 사운드 재생 (반복/중지)
        if (_value != 0)
        {
            SoundManager.instance.PlayEffectSoundLoop("eff_Space_rotate", 0.7f);
        }
        else
        {
            SoundManager.instance.StopEffSoundLoop();
        }
    }


    public void SetRadarMoveDirection(Vector2 _dir)  //도킹 레이더 이동 방향값 전달
    {
        if (isDockingMode)
        {
            radarCtrl.GetMoveDirection(_dir);
        }
    }

    #endregion

    #region 랑데부 모드

    //private void Update()
    //{
    //    if (currentShipStation[0] && !isDocking)
    //    {
    //        GenerateObstacle();
    //    }
    //}

    //void GenerateObstacle()  //장애물 생성
    //{
    //    obstacleTimer -= Time.deltaTime;
    //    if (obstacleTimer < 0)
    //    {
    //        if (currentShipStation[0] != null)
    //        {
    //            SpaceObjectPooler.SpawnFromPool<SpaceDockingObstacle>("DockingObstacle", Vector3.zero).targetTr = currentShipStation[0].transform;
    //        }
    //        else
    //        {
    //            SpaceObjectPooler.SpawnFromPool<SpaceDockingObstacle>("DockingObstacle", Vector3.zero).targetTr = currentShipStation[1].transform;
    //        }

    //        obstacleTimer = obstacleDelayTimer;
    //    }
    //}

    #endregion


    #region 도킹 모드

    public void IntoDockingMode()  //도킹 모드 돌입
    {
        System.GC.Collect();
        SpaceGameManager.instance.resultPanel.SetActive(false);
        SpaceGameManager.instance.cameraCtrl.transform.position = new Vector3(0f, 0f, SpaceGameManager.instance.cameraCtrl.transform.position.z);
        isDocking = true;

        //foreach (GameObject go in obstacles)
        //{
        //    if (go != null)
        //    {
        //        Destroy(go);
        //    }
        //    else continue;
        //}
        //obstacles.Clear();

        shipCtrl.IntoDockingMode();
        stationCtrl.IntoDockingMode();
        SpaceGameManager.instance.FadeEffect();
        StartCoroutine(DockingModeOn());
    }

    IEnumerator DockingModeOn()  //도킹 모드 돌입 딜레이, 화면 정리
    {
        dockingRadar.SetActive(true);
        yield return new WaitForSeconds(2);
        isDockingMode = true;
        radarCtrl = dockingRadar.GetComponent<SpaceDockingRadar>();
    }

    public void SetToDockingZone(bool _value)
    {
        isTryDocking = _value;
        dockingButton.interactable = _value;
    }

    public void OnClickDockingButton()  //도킹 버튼 동작
    {
        if (isTryDocking)
        {
            IntoDockingMode();
            SoundManager.instance.PlayEffectSound("RobotAsseySound", 1f);
        }
    }

    #endregion

}
