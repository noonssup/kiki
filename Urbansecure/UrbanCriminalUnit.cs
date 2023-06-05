using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UrbanCriminalUnit : MonoBehaviour
{
    public UrbanSpawnController spawnCtrl;
    Camera cam;
    public float hp = 5f;
    public float maxHp = 5f;
    public Image hpGauge;
    public Canvas canvas;
    public bool isFix = false;
    public GameObject police;
    public Transform wayPointTr;

    public float crimeDelay = 3f;


    private void Start()
    {
        hp = maxHp;
        hpGauge.fillAmount = hp / maxHp;
        cam = Camera.main;
        canvas = this.transform.GetComponentInChildren<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvas.worldCamera = cam;
        police = null;
        wayPointTr = null;
        isFix = false;

        PlaySoundEffect("eff_UrbanCrimeSiren");
    }

    //효과음 재생 (씬 연동 시 사운드매니저 사용)
    void PlaySoundEffect(string _fileName)
    {
        SoundManager.instance.PlayEffectSound(_fileName, 1f);

    }

    private void Update()
    {
        crimeDelay -= Time.deltaTime;
        if (crimeDelay <= 0f)
        {
            crimeDelay = 0f;
            if (isFix) return;
            UrbanGameManager.instance.CrimeIncrease();
        }
    }


    //private void OnMouseDown()
    //{
    //    if (police != null) return;
    //     CallPolice();
    //}

    public void OnClickCallPolice()
    {
        if (police)
        {
            UrbanPatrolUnit _patrol = police.GetComponent<UrbanPatrolUnit>();
            if (_patrol.criminalTargetTr == null || _patrol.criminalTargetTr != this.transform)
            {
                CallPolice();
            }
            return;
        }
        CallPolice();
    }

    public void CallPolice()
    {
        //if (police == null)
        //{
            police = spawnCtrl.CallPolice(this.gameObject);
        //}
        //else if (police != null)
        //{
        //    if (police.GetComponent<UrbanPatrolUnit>().criminalTargetTr != this.transform)
        //    {
        //        Debug.Log("폴리스 있음");
        //        Debug.Log(police.name + "할당");
        //        police = spawnCtrl.CallPolice(this.gameObject);
        //    }
        //}

        if (!police)
        {
            UrbanGameManager.instance.EmptyPatrolAlert();
        }
    }

    public void Damage()
    {
        hp -= Time.deltaTime;
        hpGauge.fillAmount = hp / maxHp;
        isFix = true;
        if(hp <= 0)
        {
            spawnCtrl.DestroyCriminalUnit(this.gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            spawnCtrl.DestroyCriminalUnit(this.gameObject);
        }

        if (collision.CompareTag("LINE") && wayPointTr == null)
        {
            wayPointTr = collision.transform;
        }
        else return;
    }


}
