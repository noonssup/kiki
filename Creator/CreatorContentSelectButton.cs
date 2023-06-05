using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 컨텐츠 선택 버튼 스크립트
/// 영상컨텐츠, 영상스타일, 영상아이템
/// </summary>


public class CreatorContentSelectButton : MonoBehaviour
{
    public CreatorContolUIController controlUICtrl;
    public int id = 0;
    public string nameValue;
    public TextMeshProUGUI textNameValue;

    private void Start()
    {
        textNameValue = GetComponentInChildren<TextMeshProUGUI>();
        textNameValue.text = nameValue;
    }

    public void OnClickContentValueButton()
    {
        SoundManager.instance.PlayEffectSound("eff_Motor_button", 1f);
        controlUICtrl.SetCreatorContent(id, nameValue);
    }

}
