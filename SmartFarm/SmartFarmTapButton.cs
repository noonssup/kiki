using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// ����Ʈ��
/// ����â �� ��ư�� ��ũ��Ʈ
/// </summary>

public class SmartFarmTapButton : MonoBehaviour
{
    public Button[] tapBtns;
    public GameObject[] statusPanels;

    public void SelectTap(int _num)
    {

        ColorBlock _colorBlock;
        Color _color;
        for (int i = 0; i < tapBtns.Length; i++)
        {
            _colorBlock = tapBtns[i].colors;
            ColorUtility.TryParseHtmlString("#5c6686", out _color);
            _colorBlock.normalColor = _color;
            tapBtns[i].colors = _colorBlock;
            ColorUtility.TryParseHtmlString("#ffffff", out _color);
            tapBtns[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = _color;
            statusPanels[i].SetActive(false);
        }
        _colorBlock = tapBtns[_num].colors;
        _colorBlock.normalColor = new Color(1f, 1f, 1f, 1f);
        ColorUtility.TryParseHtmlString("#5c6686", out _color);
        tapBtns[_num].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = _color;
        tapBtns[_num].colors = _colorBlock;
        statusPanels[_num].SetActive(true);
        if(_num == 3)
        {
            this.transform.parent.Find("DesPopup").gameObject.SetActive(false);
        }
        SoundManager.instance.PlayEffectSound("eff_Common_popup", 1f);
    }

    public void TempControllerButtonSound()  //�µ� �г��� ���� ��ư�� ���
    {
        SoundManager.instance.PlayEffectSound("eff_SmartFarm_temp", 1f);
    }

    public void SoilMoisControllerButtonSound()  //���,���� �г��� ���� ��ư�� ���
    {
        SoundManager.instance.PlayEffectSound("eff_SmartFarm_button", 1f);
    }
}

