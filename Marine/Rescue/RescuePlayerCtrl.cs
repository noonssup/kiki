using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescuePlayerCtrl : MonoBehaviour
{
    public RescueGameCtrl gameCtrl;
    public Transform playerImage;
    public float moveSpeed = 1.7f;
    public float mouseMoveSpeed = 100f;
    public bool isMove = false;
    public Rigidbody2D rB;




    private void Start()
    {
        gameCtrl = FindObjectOfType<RescueGameCtrl>();
        rB = this.transform.GetComponent<Rigidbody2D>();
        playerImage = this.transform.GetChild(0);
    }
    private void OnEnable()
    {
        moveSpeed = 2f;
        mouseMoveSpeed = 100f;
        StartCoroutine(MoveControl());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.name)
        {
            case "Level1Part":
                ChangeGameLevel(1);
                gameCtrl.Level1GameStart();
                break;
            case "Level2Part":
                ChangeGameLevel(2);
                break;
            case "Level3Part":
                ChangeGameLevel(3);
                break;
        }
    }

    void ChangeGameLevel(int _gameLevel) //진행할 게임 단계의 변경 (단계별 이벤트콜라이더와 충돌할 때 실행
    {
        gameCtrl.gameLevel = _gameLevel;
        gameCtrl.rescueCount = 0;
        gameCtrl.EventColliderSetting(9);
        gameCtrl.GamePopupWindowsSetting(_gameLevel - 1);
        gameCtrl.rescueWorker.SetActive(false);
        gameCtrl.helpMan.SetActive(false);
    }

    IEnumerator MoveControl()
    {

        while (this.gameObject.activeSelf)
        {
            yield return new WaitForFixedUpdate();
            if (gameCtrl.isPlayerMove)
            {
                Vector3 moveVelocity = Vector3.zero;
                moveVelocity.x = Input.GetAxisRaw("Horizontal");
                moveVelocity.y = Input.GetAxisRaw("Vertical");
                moveVelocity.Normalize();

                if (moveVelocity.x < 0) playerImage.rotation = Quaternion.Euler(0f, 180f, 0f);
                else if (moveVelocity.x > 0) playerImage.rotation = Quaternion.Euler(0f, 0f, 0f);
                this.transform.position += moveVelocity * moveSpeed * Time.deltaTime;
            }

            //if (Input.GetMouseButtonDown(0))
            //{
            //    CalTargetPos();
            //}

        }
    }

    private void Update()
    {
        ScreenInteraction();
    }

    void ScreenInteraction()
    {
        Vector2 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 클릭한 좌표를 가져 옴
        Ray2D ray = new Ray2D(wp, Vector2.zero); // 원점에서 터치한 좌표 방향으로 Ray를 쏨
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, float.MaxValue/*, layerMask*/);


        if (Input.GetMouseButtonDown(0))
        {
            isMove = true;
        }

        if (Input.GetMouseButton(0) && gameCtrl.isPlayerMove)
        {

            if (isMove) { Move(wp); }

        }
        else if (Input.GetMouseButtonUp(0))
        {
            isMove = false;
            rB.velocity = Vector2.zero;
        }
    }

    public void Move(Vector2 wp)
    {
        Vector2 direction = (wp - (Vector2)transform.position).normalized;
        
        //이동방향에 따라 이미지 반전
        if (direction.x < 0) playerImage.rotation = Quaternion.Euler(0f, 180f, 0f);
        else if (direction.x > 0) playerImage.rotation = Quaternion.Euler(0f, 0f, 0f);

        //리지드 바디 가속도로 이동 구현
        Vector2 avatarVelocity = direction * (mouseMoveSpeed * Time.fixedDeltaTime);
        rB.velocity = avatarVelocity;
    }
}
