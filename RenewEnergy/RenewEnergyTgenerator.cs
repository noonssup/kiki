using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// ���������� ��ũ��Ʈ
/// </summary>

public class RenewEnergyTgenerator : MonoBehaviour
{
    RenewEnergyGameController gameCtrl;
    public RenewEnergyWaveController waveCtrl;

    [Header("������Ʈ")]
    private Transform tidalTr;
    private Button button;
    public Transform propellarTr;

    [Header("���ӵ�����")]
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


    public void ChangeTidalDirection()  //���� ��ȯ
    {
        ++dir;
        if (dir > 1) dir = 0;
        posX *= -1f;


        tidalTr.localScale = new Vector3(posX, 1f, 1f);

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //���ʿ��� ���� �ط�
        if (collision.gameObject.name == "WaveLeft")  
        {
            if (this.dir == 0)
            {
                //������ ���
                gameCtrl.SaveEnergy(5f);
                isWave = true;
            }
            else { } //����

        } //�����ʿ��� ���� �ط�
        else if (collision.gameObject.name == "WaveRight" || collision.gameObject.name == "TutoWaveRight")
        {
            if (this.dir == 1)
            {
                //������ ���
                gameCtrl.SaveEnergy(5f);
                isWave = true;
                if (this.gameCtrl.name == "TutorialGamePanel")
                {
                    gameCtrl.tutoWindCount++;
                }
            }
            else { } //����
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
        else   //�帧�� ���� ������ ȸ���ϴ� �����緯�� �ӵ��� �������� ����
        {
            currentRotSpeed -= (Time.deltaTime * 2f) * maxRotSpeed;
            if (currentRotSpeed <= minRotSpeed) { currentRotSpeed = minRotSpeed; return; }
            propellarTr.Rotate(new Vector3(propellarTr.rotation.x, propellarTr.rotation.y, currentRotSpeed * Time.deltaTime));
        }
    }
}
