using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 태양열발전기 스크립트
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

        //부모오브젝트가 튜토리얼패널이면서 튜토리얼팝업창이 비활성화일 경우 전력 생산
        if (this.transform.parent.name == "TutorialGamePanel" && !RenewEnergyGameManager.instance.tutorialPopup.activeSelf)
        {
            CheckGeneratePower();
        }
    }
}
