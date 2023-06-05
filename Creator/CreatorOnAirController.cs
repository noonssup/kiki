using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// ��� �� ȭ��� ��ũ��Ʈ
/// ��� ���� �� ��� ���� ó����
/// </summary>

public class CreatorOnAirController : MonoBehaviour
{
    [Header("���ӿ�����Ʈ")]
    public GameObject reactionCommentPrefab;  //��� ����
    public GameObject viewerCommentPrefab;    //��� ����
    public List<GameObject> listViewerComment = new List<GameObject>();  //����� ��Ƴ��� ����Ʈ
    public List<GameObject> listReactionComment = new List<GameObject>();  //��� ������ ��Ƴ��� ����Ʈ
    public Transform reactionViewTr;
    public Transform viewerViewTr;

    [Header("���ӵ�����")]
    public int maxViewerCommentCount = 20;
    public float viewerCommentTimer = 5;
    private float viewerTimer;
    public float reactionCommentTimer = 6;
    private float reactionTimer;
    private int goodRate;
    private string viewerComment;
    private string reactionComment;
    public int realtimeViewerCount = 0;

    public TextMeshProUGUI textRealtimeViewerCount;

    //��� ����Ʈ
    List<Dictionary<string, object>> commentData;
    List<int> commentId1;
    List<int> commentId2;
    List<int> reactionId1;
    List<int> reactionId2;

    private void Start()
    {
        DataInit();
    }

    void DataInit()  //����Ʈ ������ �ʱ�ȭ
    {
        commentData = CreatorGameManager.instance.commentData;

        commentId1 = new List<int>();
        commentId2 = new List<int>();
        reactionId1 = new List<int>();
        reactionId2 = new List<int>();

        for (int i = 0; i < commentData.Count; i++)
        {
            if (int.Parse(commentData[i]["id"].ToString()) < 200)
            {
                if (int.Parse(commentData[i]["type"].ToString()) == 1)   //���� ��� ����Ʈ
                {
                    commentId1.Add(int.Parse(commentData[i]["id"].ToString()));
                }
                else   //���� ��� ����Ʈ
                {
                    commentId2.Add(int.Parse(commentData[i]["id"].ToString()));
                }

            }
            else
            {
                if (int.Parse(commentData[i]["type"].ToString()) == 1)   //���� ���� ����Ʈ
                {
                    reactionId1.Add(int.Parse(commentData[i]["id"].ToString()));
                }
                else   //���� ���� ����Ʈ
                {
                    reactionId2.Add(int.Parse(commentData[i]["id"].ToString()));
                }

            }
        }
    }

    private void OnEnable()
    {
        viewerTimer = viewerCommentTimer;
        reactionTimer = reactionCommentTimer;
        goodRate = CreatorGameManager.instance.goodRate;
        if (CreatorGameManager.instance.subscriber < 10)
        {
            realtimeViewerCount = 1;
            textRealtimeViewerCount.text = realtimeViewerCount.ToString();
        }
    }


    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            viewerTimer -= Time.deltaTime;
            reactionTimer -= Time.deltaTime;

            if(viewerTimer <= 0)  //��� ����
            {
                int randomRate = Random.Range(0, 100);
                if (randomRate < goodRate) //���� ���
                {
                    ViewerCommentRenewal(1);
                }
                else  //���� ���
                {
                    ViewerCommentRenewal(2);
                }
                viewerTimer = viewerCommentTimer;
                SoundManager.instance.PlayEffectSound("eff_Virus_hit", 1f);
            }

            if(reactionTimer <= 0)  //���� ����
            {
                int randomRate = Random.Range(0, 100);

                if (randomRate < goodRate) //���� ����
                {
                    ReactionViewRenewal(1);
                }
                else  //���� ����
                {
                    ReactionViewRenewal(2);
                }

                //�ǽð� ��û��
                realtimeViewerCount = (int)(CreatorGameManager.instance.viewCount * 0.05f);
                textRealtimeViewerCount.text = realtimeViewerCount.ToString();
                
                reactionTimer = reactionCommentTimer;
                SoundManager.instance.PlayEffectSound("eff_World_touch", 1f);
            }
        }
    }

    void ReactionViewRenewal(int _type)  //��� ���� ����
    {
        GameObject _reactionComment = Instantiate(reactionCommentPrefab);
        _reactionComment.transform.SetParent(reactionViewTr);
        _reactionComment.transform.localScale = Vector3.one;
        listReactionComment.Add(_reactionComment);

        if (_type == 1)  //���� ����
        {
            int randomId = Random.Range(0, reactionId1.Count);
            for (int i = 0; i < commentData.Count; i++)
            {
                if (int.Parse(commentData[i]["type"].ToString()) == _type && int.Parse(commentData[i]["id"].ToString()) == reactionId1[randomId])
                {
                    reactionComment = commentData[i]["desc"].ToString();
                    CreatorGameManager.instance.ViewCount += int.Parse(commentData[i]["people"].ToString());
                }
            }
            CreatorGameManager.instance.goodCount += 1;
        }
        else  //���� ����
        {
            int randomId = Random.Range(0, reactionId2.Count);
            for (int i = 0; i < commentData.Count; i++)
            {
                if (int.Parse(commentData[i]["type"].ToString()) == _type && int.Parse(commentData[i]["id"].ToString()) == reactionId2[randomId])
                {
                    reactionComment = commentData[i]["desc"].ToString();
                    //�������� �� ��ȸ�� ����, ���� ��� (2023.04.03)
                    //CreatorGameManager.instance.ViewCount -= int.Parse(commentData[i]["people"].ToString());
                }
            }
            CreatorGameManager.instance.badCount += 1;
        }

        _reactionComment.GetComponent<CreatorReactionComment>().CommentSetup(_type, reactionComment);

        if(listReactionComment.Count > 10)
        {
            Destroy(listReactionComment[0].gameObject);
            listReactionComment.RemoveAt(0);
        }
    }

    void ViewerCommentRenewal(int _type)   //��� ���� ����
    {
        if (listViewerComment.Count < maxViewerCommentCount)
        {
            GameObject _viewerCommentPrefab = Instantiate(viewerCommentPrefab);
            _viewerCommentPrefab.transform.SetParent(viewerViewTr);
            _viewerCommentPrefab.transform.localScale = Vector3.one;
            _viewerCommentPrefab.transform.localPosition = new Vector3(15f, 0f, 0f);

            if (_type == 1)  //���� ���
            {
                CreatorGameManager.instance.ViewCount += 50;
                int randomId = Random.Range(0, commentId1.Count);
                for(int i = 0; i < commentData.Count; i++)
                {
                    if (int.Parse(commentData[i]["type"].ToString()) == _type && int.Parse(commentData[i]["id"].ToString()) == commentId1[randomId])
                    {
                        viewerComment = commentData[i]["desc"].ToString();
                    }
                }
            }
            else if (_type == 2)  //���� ���
            {
                CreatorGameManager.instance.ViewCount += 5;
                int randomId = Random.Range(0, commentId2.Count);
                for (int i = 0; i < commentData.Count; i++)
                {
                    if (int.Parse(commentData[i]["type"].ToString()) == _type && int.Parse(commentData[i]["id"].ToString()) == commentId2[randomId])
                    {
                        viewerComment = commentData[i]["desc"].ToString();
                    }
                }
            }

            _viewerCommentPrefab.GetComponent<CreatorViewerComment>().ViewerCommentSetup(_type, viewerComment, this);
            listViewerComment.Add(_viewerCommentPrefab);
        }
        else return;  //����� 20���� ��� ����
    }

    public void CheckViewerCommentReaction(GameObject _commentObj)  //��ۿ� ���� �÷��̾� ���� ó��
    {
        StartCoroutine(CommentRemove(_commentObj));
    }

    IEnumerator CommentRemove(GameObject _commentObj)
    {
        CreatorGameManager.instance.isWait = true;
        yield return new WaitForSeconds(0.5f);
        listViewerComment.Remove(_commentObj);
        Destroy(_commentObj);
        CreatorGameManager.instance.isWait = false;
    }
}
