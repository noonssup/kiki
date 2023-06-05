using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 조류발전기 스크립트
/// </summary>

public class RenewEnergyTgenerator : MonoBehaviour
{
    RenewEnergyGameController gameCtrl;
    public RenewEnergyWaveController waveCtrl;

    [Header("오브젝트")]
    private Transform tidalTr;
    private Button button;
    public Transform propellarTr;

    [Header("게임데이터")]
    public int dir = 0;
    private float posX;
    public bool isWave = false;
    public float currentRotSpeed = 0f;
    public float minRotSpeed = 0f;
    public float maxRotSpeed = 100f;


    private void Start()
    {
        gameCtrl = FindObjectOfType<RenewEnergyGameController>();

        tidalTr = GetComponent<Transform>();
        button = this.gameObject.AddComponent<Button>();
        button.onClick.AddListener(ChangeTidalDirection);

        dir = 0;
        posX = tidalTr.localScale.x;
    }


    public void ChangeTidalDirection()  //방향 전환
    {
        ++dir;
        if (dir > 1) dir = 0;
        posX *= -1f;


        tidalTr.localScale = new Vector3(posX, 1f, 1f);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //왼쪽에서 오는 해류
        if (collision.gameObject.name == "WaveLeft")  
        {
            if (this.dir == 0)
            {
                //에너지 상승
                gameCtrl.SaveEnergy(5f);
                isWave = true;
            }
            else { } //무시

        } //오른쪽에서 오는 해류
        else if (collision.gameObject.name == "WaveRight" || collision.gameObject.name == "TutoWaveRight")
        {
            if (this.dir == 1)
            {
                //에너지 상승
                gameCtrl.SaveEnergy(5f);
                isWave = true;
                if (this.gameCtrl.name == "TutorialGamePanel")
                {
                    gameCtrl.tutoWindCount++;
                }
            }
            else { } //무시
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "WaveLeft" || collision.gameObject.name == "WaveRight" || collision.gameObject.name == "TutoWaveRight")
        {
            isWave = false;
        }
    }

    private void Update()
    {
        if (isWave)
        {
            currentRotSpeed += (Time.deltaTime * 2f) * maxRotSpeed;
            if (currentRotSpeed >= maxRotSpeed) currentRotSpeed = maxRotSpeed;
            propellarTr.Rotate(new Vector3(propellarTr.rotation.x, propellarTr.rotation.y, currentRotSpeed * Time.deltaTime));

        }
        else   //흐름이 닿지 않으면 회전하던 프로펠러의 속도가 떨어지며 정지
        {
            currentRotSpeed -= (Time.deltaTime * 2f) * maxRotSpeed;
            if (currentRotSpeed <= minRotSpeed) { currentRotSpeed = minRotSpeed; return; }
            propellarTr.Rotate(new Vector3(propellarTr.rotation.x, propellarTr.rotation.y, currentRotSpeed * Time.deltaTime));
        }
    }
}
