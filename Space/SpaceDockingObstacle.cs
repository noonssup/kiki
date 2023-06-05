using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SpaceDockingObstacle : MonoBehaviour
{
    public Transform targetTr;
    public Transform posTr;
    public float rangeXpos = 10;
    public float rangeYpos = 10;
    public Vector2 targetPosition = Vector2.zero;
    public float moveSpeed = 1;

    private SpriteRenderer obstacleImage; 
    public Sprite[] obstacleSprites;
    CircleCollider2D coll;

    public float liveTimer = 10f;

    private void OnEnable()
    {
        coll= GetComponent<CircleCollider2D>();
        coll.enabled = false;
        this.transform.GetChild(0).gameObject.SetActive(true);
        obstacleImage = GetComponentInChildren<SpriteRenderer>();
        obstacleImage.sprite = obstacleSprites[Random.Range(0, obstacleSprites.Length)];
        targetTr = null; posTr = null;
        StartCoroutine(AppearObstacle());
    }

    void SetTargetPosition()  //초기 위치 설정
    {
        if (targetTr == null)
        {
            targetTr = GameObject.FindWithTag("Player").transform;
        }
        posTr = targetTr.GetComponent<SpaceDockingShipController>().spawnTrArray[Random.Range(0, targetTr.GetComponent<SpaceDockingShipController>().spawnTrArray.Length)];
        rangeXpos = Random.Range(-rangeXpos + targetTr.position.x, rangeXpos+ targetTr.position.x);
        rangeYpos = Random.Range(-rangeYpos+ targetTr.position.y, rangeYpos+ targetTr.position.y);
        //this.transform.position = new Vector2(rangeXpos, rangeYpos);
        this.transform.position = posTr.position;
        targetPosition = (targetTr.position - this.transform.position).normalized;
    }

    IEnumerator AppearObstacle()  //장애물 나타나기
    {
        SetTargetPosition();

        Color color = Color.white;
        color.a = 0f;
        obstacleImage.color = color;

        while (color.a < 1f)
        {
            color.a += Time.deltaTime;
            obstacleImage.color = color;
            yield return null;
        }
        coll.enabled = true;
    }

    private void FixedUpdate()
    {
        MoveObstacle();
    }

    void MoveObstacle()  //장애물 이동 동작
    {
        if (targetPosition != Vector2.zero && liveTimer > 0)
        {
            this.transform.position += (Vector3)targetPosition * moveSpeed * Time.deltaTime;
            //Vector2 distance = targetPosition - (Vector2)this.transform.position;
        }

        liveTimer -= Time.deltaTime;
        if (liveTimer < 0f)
        {
            StartCoroutine(DisappearObstacle());
            liveTimer = 10f;
        }
    }

    IEnumerator DisappearObstacle()  //장애물 없애기 (서서히 없어지기)
    {
        Color color = Color.white;
        while(color.a > 0)
        {
            color.a -= Time.deltaTime;
            obstacleImage.color = color;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    //private void OnDisable()
    //{
    //    SpaceObjectPooler.ReturnToPool(gameObject);
    //}

}
