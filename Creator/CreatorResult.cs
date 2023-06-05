using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ũ�������� ���â�� ��ũ��Ʈ
/// </summary>


public class CreatorResult : MonoBehaviour
{
    public Image resultImage;
    public Sprite[] resultSprites;
    public TextMeshProUGUI subscriberText;
    public TextMeshProUGUI goodCountText;
    public TextMeshProUGUI badCountText;


    //�߰� ���� ���â���� ����
    public void CheckResult(int _goodCount, int _badCount, int subscriber)
    {
        if(_goodCount > _badCount)
        {
            resultImage.sprite = resultSprites[0];
        }
        else
        {
            resultImage.sprite = resultSprites[1];
        }

        subscriberText.text = "������ : " + subscriber.ToString("n0");
        goodCountText.text = "���� ���� : " + _goodCount.ToString();
        badCountText.text = "���� ���� : " + _badCount.ToString();
    }


    //���� ���â���� ����
    public void LastResult(int subscriber)
    {
        Color _color;
        if (subscriber >= 100000)
        {
            subscriberText.text = "10�� ������ �޼��� �����߽��ϴ�!";
            resultImage.sprite = resultSprites[0];
            ColorUtility.TryParseHtmlString("#F5A032FF", out _color);
            this.goodCountText.text = "Success!";
            this.goodCountText.color = _color;
        }
        else
        {
            subscriberText.text = "10�� ������ �޼��� �����߽��ϴ�...";
            resultImage.sprite = resultSprites[1];
            ColorUtility.TryParseHtmlString("#FF0000FF", out _color);
            //this.goodCountText.text = "fail...";
            this.goodCountText.text = "<color=#FF0000>fail...</color>";
            //this.goodCountText.color = _color;
        }
    }
}
