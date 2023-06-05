using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatorViewerComment : MonoBehaviour
{
    CreatorOnAirController onAirCtrl;

    [Header("게임오브젝트")]
    public GameObject[] btnObjects;
    public Image likeIcon;
    public Sprite[] iconSprites;

    [Header("게임데이터")]
    public bool isReaction = false;
    public int type = 1;
    public Color colorRed = new Color(0.9019f,0f,0.0705f);
    public Color colorGreen = new Color(0f, 0.549f, 0.862f);

    [Header("텍스트")]
    public TextMeshProUGUI textComment;

    private void Start()
    {
        btnObjects[0].gameObject.SetActive(true);
        btnObjects[1].gameObject.SetActive(true);
        likeIcon.enabled = false;
    }

    public void ViewerCommentSetup(int _type, string _comment, CreatorOnAirController _ctrl)
    {
        type = _type;
        onAirCtrl = _ctrl;

        if (type == 1)
        {
            textComment.color = colorGreen;
            textComment.text = _comment;
        }
        else if (type == 2)
        {
            textComment.color = colorRed;
            textComment.text = _comment;
        }
    }

    public void OnClickReactionButton(int _value)  //댓글에 대한 반응
    {

        if(type == 1)
        {
            if(_value == 1)   //좋아요 / 좋아요
            {
                likeIcon.enabled = true;
                likeIcon.sprite = iconSprites[0];
                likeIcon.transform.localScale = Vector3.one;
                CreatorGameManager.instance.ViewCount += 100;
            }
            else        //좋아요 / 싫어요
            {
                likeIcon.enabled = true;
                likeIcon.sprite = iconSprites[1];
                likeIcon.transform.localScale = Vector3.one;
                CreatorGameManager.instance.ViewCount += 50;
            }
        }
        else if(type == 2)
        {
            if (_value == 2)        //싫어요 / 싫어요
            {
                likeIcon.enabled = true;
                likeIcon.sprite = iconSprites[0];
                likeIcon.transform.localScale = Vector3.one;
                CreatorGameManager.instance.ViewCount += 50;
            }
            else        //싫어요 / 좋아요
            {
                likeIcon.enabled = true;
                likeIcon.sprite = iconSprites[1];
                likeIcon.transform.localScale = Vector3.one;
                CreatorGameManager.instance.ViewCount += 25;
            }
        }

        btnObjects[0].gameObject.SetActive(false);
        btnObjects[1].gameObject.SetActive(false);
        onAirCtrl.CheckViewerCommentReaction(this.gameObject);
    }

}
