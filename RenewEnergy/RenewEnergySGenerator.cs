using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �¾翭������ ��ũ��Ʈ
/// </summary>


public class RenewEnergySGenerator : MonoBehaviour
{
    RenewEnergyGameController gameCtrl;
    public RenewEnergySolarLed[] generateLedsCtrl;
    float[] generateRate;
    float generatePower = 0f;

    private void Start()
    {
        gameCtrl = FindObjectOfType<RenewEnergyGameController>();
        generateRate = new float[generateLedsCtrl.Length];
        for (int i = 0; i < generateLedsCtrl.Length; i++)
        {
            generateRate[i] = generateLedsCtrl[i].generatePower;
        }

        CheckGeneratePower();
    }

    void CheckGeneratePower()
    {
        for (int i = 0; i < generateLedsCtrl.Length; i++)
        {
            generatePower += generateLedsCtrl[i].generatePower;
        }
        generatePower = generatePower / (generateLedsCtrl.Length+1);
        generatePower = generatePower * 3f;
        gameCtrl.SaveEnergy(generatePower);

    }

    private void Update()
    {
        if (RenewEnergyGameManager.instance.isGamePlaying)
        {
            CheckGeneratePower();
        }

        //�θ������Ʈ�� Ʃ�丮���г��̸鼭 Ʃ�丮���˾�â�� ��Ȱ��ȭ�� ��� ���� ����
        if (this.transform.parent.name == "TutorialGamePanel" && !RenewEnergyGameManager.instance.tutorialPopup.activeSelf)
        {
            CheckGeneratePower();
        }
    }
}
