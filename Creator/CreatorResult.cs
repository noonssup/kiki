using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 크리에이터 결과창용 스크립트
/// </summary>


public class CreatorResult : MonoBehaviour
{
    public Image resultImage;
    public Sprite[] resultSprites;
    public TextMeshProUGUI subscriberText;
    public TextMeshProUGUI goodCountText;
    public TextMeshProUGUI badCountText;


    //중간 정산 결과창으로 실행
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

        subscriberText.text = "구독자 : " + subscriber.ToString("n0");
        goodCountText.text = "긍정 반응 : " + _goodCount.ToString();
        badCountText.text = "부정 반응 : " + _badCount.ToString();
    }


    //최종 결과창으로 실행
    public void LastResult(int subscriber)
    {
        Color _color;
        if (subscriber >= 100000)
        {
            subscriberText.text = "10만 구독자 달성에 성공했습니다!";
            resultImage.sprite = resultSprites[0];
            ColorUtility.TryParseHtmlString("#F5A032FF", out _color);
            this.goodCountText.text = "Success!";
            this.goodCountText.color = _color;
        }
        else
        {
            subscriberText.text = "10만 구독자 달성에 실패했습니다...";
            resultImage.sprite = resultSprites[1];
            ColorUtility.TryParseHtmlString("#FF0000FF", out _color);
            //this.goodCountText.text = "fail...";
            this.goodCountText.text = "<color=#FF0000>fail...</color>";
            //this.goodCountText.color = _color;
        }
    }
}
