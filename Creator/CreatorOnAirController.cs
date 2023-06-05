using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 방송 중 화면용 스크립트
/// 방송 반응 및 댓글 반응 처리용
/// </summary>

public class CreatorOnAirController : MonoBehaviour
{
    [Header("게임오브젝트")]
    public GameObject reactionCommentPrefab;  //방송 반응
    public GameObject viewerCommentPrefab;    //댓글 반응
    public List<GameObject> listViewerComment = new List<GameObject>();  //댓글을 담아놓을 리스트
    public List<GameObject> listReactionComment = new List<GameObject>();  //방송 반응을 담아놓을 리스트
    public Transform reactionViewTr;
    public Transform viewerViewTr;

    [Header("게임데이터")]
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

    //댓글 리스트
    List<Dictionary<string, object>> commentData;
    List<int> commentId1;
    List<int> commentId2;
    List<int> reactionId1;
    List<int> reactionId2;

    private void Start()
    {
        DataInit();
    }

    void DataInit()  //리스트 데이터 초기화
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
                if (int.Parse(commentData[i]["type"].ToString()) == 1)   //긍정 댓글 리스트
                {
                    commentId1.Add(int.Parse(commentData[i]["id"].ToString()));
                }
                else   //부정 댓글 리스트
                {
                    commentId2.Add(int.Parse(commentData[i]["id"].ToString()));
                }

            }
            else
            {
                if (int.Parse(commentData[i]["type"].ToString()) == 1)   //긍정 반응 리스트
                {
                    reactionId1.Add(int.Parse(commentData[i]["id"].ToString()));
                }
                else   //부정 반응 리스트
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

            if(viewerTimer <= 0)  //댓글 생성
            {
                int randomRate = Random.Range(0, 100);
                if (randomRate < goodRate) //긍정 댓글
                {
                    ViewerCommentRenewal(1);
                }
                else  //부정 댓글
                {
                    ViewerCommentRenewal(2);
                }
                viewerTimer = viewerCommentTimer;
                SoundManager.instance.PlayEffectSound("eff_Virus_hit", 1f);
            }

            if(reactionTimer <= 0)  //반응 생성
            {
                int randomRate = Random.Range(0, 100);

                if (randomRate < goodRate) //긍정 반응
                {
                    ReactionViewRenewal(1);
                }
                else  //부정 반응
                {
                    ReactionViewRenewal(2);
                }

                //실시간 시청자
                realtimeViewerCount = (int)(CreatorGameManager.instance.viewCount * 0.05f);
                textRealtimeViewerCount.text = realtimeViewerCount.ToString();
                
                reactionTimer = reactionCommentTimer;
                SoundManager.instance.PlayEffectSound("eff_World_touch", 1f);
            }
        }
    }

    void ReactionViewRenewal(int _type)  //방송 반응 갱신
    {
        GameObject _reactionComment = Instantiate(reactionCommentPrefab);
        _reactionComment.transform.SetParent(reactionViewTr);
        _reactionComment.transform.localScale = Vector3.one;
        listReactionComment.Add(_reactionComment);

        if (_type == 1)  //긍정 반응
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
        else  //부정 반응
        {
            int randomId = Random.Range(0, reactionId2.Count);
            for (int i = 0; i < commentData.Count; i++)
            {
                if (int.Parse(commentData[i]["type"].ToString()) == _type && int.Parse(commentData[i]["id"].ToString()) == reactionId2[randomId])
                {
                    reactionComment = commentData[i]["desc"].ToString();
                    //부정반응 시 조회수 감소, 적용 취소 (2023.04.03)
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

    void ViewerCommentRenewal(int _type)   //댓글 반응 갱신
    {
        if (listViewerComment.Count < maxViewerCommentCount)
        {
            GameObject _viewerCommentPrefab = Instantiate(viewerCommentPrefab);
            _viewerCommentPrefab.transform.SetParent(viewerViewTr);
            _viewerCommentPrefab.transform.localScale = Vector3.one;
            _viewerCommentPrefab.transform.localPosition = new Vector3(15f, 0f, 0f);

            if (_type == 1)  //긍정 댓글
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
            else if (_type == 2)  //부정 댓글
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
        else return;  //댓글이 20개일 경우 무시
    }

    public void CheckViewerCommentReaction(GameObject _commentObj)  //댓글에 대한 플레이어 반응 처리
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
