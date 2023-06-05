using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 풍력발전기 스크립트
/// </summary>

public class RenewEnergyWGenerator : MonoBehaviour
{
    RenewEnergyGameController gameCtrl;

    [Header("윙 회전")]
    public float currentRotSpeed = 0f;
    public float maxRotSpeed = 100f;
    public float minRotSpeed = 0f;
    bool isWind = false;
    public float maxWGeneratePower = 4f;
    float generatePower;

    private void Start()
    {
        gameCtrl = FindObjectOfType<RenewEnergyGameController>();
        generatePower = 0f;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.name == "Wind(Clone)" || collision.name == "TutoWind(Clone)")
        {
            isWind = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Wind(Clone)" || collision.name == "TutoWind(Clone)")
        {
            isWind = false;

            if(this.gameCtrl.name == "TutorialGamePanel")
            {
                gameCtrl.tutoWindCount++;
            }
        }
    }

    private void Update()
    {
        if (isWind)  //바람에 닿고 있을 때 프로펠러 회전
        {
            currentRotSpeed += (Time.deltaTime * 2f) * maxRotSpeed;
            if (currentRotSpeed >= maxRotSpeed) currentRotSpeed = maxRotSpeed;
            generatePower += currentRotSpeed * 0.05f * Time.deltaTime;
            if (generatePower >= maxWGeneratePower) generatePower = maxWGeneratePower;
            transform.Rotate(new Vector3(this.transform.rotation.x, this.transform.rotation.y, currentRotSpeed * Time.deltaTime));
            
            //에너지 충전!!
            gameCtrl.SaveEnergy(generatePower); 
        }
        else   //바람이 닿지 않으면 회전하던 프로펠러의 속도가 떨어지며 정지
        {
            currentRotSpeed -= (Time.deltaTime * 2f) * maxRotSpeed;
            if (currentRotSpeed <= minRotSpeed) currentRotSpeed = minRotSpeed;
            //generatePower -= currentRotSpeed * 0.1f * Time.deltaTime;
            //if(generatePower <= 0f)
                generatePower = 0f;
            transform.Rotate(new Vector3(this.transform.rotation.x, this.transform.rotation.y, currentRotSpeed * Time.deltaTime));
        }

    }
}
