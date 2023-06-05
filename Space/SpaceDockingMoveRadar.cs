using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 우주비행사
/// 도킹 모드에서의 도킹용 레이더 컨트롤스크립트
/// </summary>

public class SpaceDockingMoveRadar : MonoBehaviour
{
    [Header("게임오브젝트")]
    private SpaceDockingRadar radarCtrl;
    private CircleCollider2D coll;
    public Transform dockingTr;
    public GameObject joystick;
    public GameObject dockingGauge;

    [Header("게임데이터")]
    public bool isMatch = false;
    public bool isMove = true;
    public float maxMatchCount = 5f;
    public float currentMatchCount = 0f;
    private float delay = 1f;
    private float soundDelay = 0f;

    private void Start()
    {
        radarCtrl = transform.parent.parent.GetComponent<SpaceDockingRadar>();
    }

    private void OnEnable()
    {
        joystick.SetActive(true);
        dockingGauge.SetActive(true);
        isMove = true;
        coll = GetComponent<CircleCollider2D>();
        coll.enabled = true;
        maxMatchCount = 5f;
        currentMatchCount = 0f;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.name == "DockingStation")  //레이더가 도킹 자리에 위치하면 도킹게이지 증가
        {
            currentMatchCount += Time.deltaTime;
            radarCtrl.SetDockingGauge(currentMatchCount, maxMatchCount);

            soundDelay -= Time.deltaTime;
            if(soundDelay < 0f)
            {
                SoundManager.instance.PlayEffectSound("eff_Robot_matching", 1f);
                soundDelay = 1f;
            }

            if (currentMatchCount > maxMatchCount)
            {
                isMove = false;
                coll.enabled = false;
                joystick.SetActive(false);
                //dockingGauge.SetActive(false);
                SpaceGameManager.instance.ClearDockingMission();
                this.gameObject.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (delay > 0f)
        {
            delay -= Time.deltaTime;
        }
        
        if (currentMatchCount > 0 && delay < 0 && isMove)  //도킹게이지 감소
        {
            currentMatchCount -= Time.deltaTime * 0.5f;
            radarCtrl.SetDockingGauge(currentMatchCount, maxMatchCount);
        }

        if (!isMove)  //도킹 완료 후 레이더 위치 도킹 위치로 이동
        {
            this.transform.position
                = new Vector3(dockingTr.position.x, dockingTr.position.y, this.transform.position.z);
        }
    }
}
