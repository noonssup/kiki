using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AirPassenger : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public enum NPCState { SIT, SCARED, SAFETY, BELT, PACKING, HMOVE, VMOVE}  //승객의 상태
    public NPCState npcState = NPCState.SIT;
    public enum MouseState { DRAGON, DRAGOFF }    //마우스 클릭여부
    public MouseState mouseState = MouseState.DRAGOFF;

    public enum EscapeState { IDLE, ESCAPE }  //탈출 가능 여부 (탈출 장소로 가면 ESCAPE 로 상태 변경)
    public EscapeState escapeState = EscapeState.IDLE;

    public float stateTimer = 0f;  //비상탈출 상황 시 스테이트 변경 시간
    public int clickCount = 0;     //마우스클릭 카운트

    public GameObject aircrewAvatarImage;
    public Image[] npcImage;    //승객 이미지[0], 가방, 벨트, 불안상태 이미지[1]
    public int npcImageIndex; //승객 이미지의 이름
    public string npcImageInfo; //승객 이미지의 파츠 정보값
    public Sprite[] npcSprites;
    public Sprite aircrewBelt;
    public GameObject bagPrefab;
    public Sprite[] aircrewBag;
    public Vector3 resetPos;    //승객이 앉아있는 자리 위치정보
    public Vector3 resetScale;  //승객 오브젝트의 스케일 정보
    public Vector3 resetRot;  //승객 오브젝트의 회전값 정보
    public Transform resetTr;   //승객 오브젝트의 부모 트랜스폼
    public Animator anim;
    public int randBag;

    public Transform[] escapeTr = new Transform[2];  //탈출 시 npc가 미끄러질 이동 경로

    AircrewCanvas aircrewCanvas;
    AircrewGameCtrl gameCtrl;

    [Header("가방떨어뜨리기 제스처")]
    public Vector2 v2;
    public bool isUp = false;
    public bool isDown = false;
    public bool isLeft = false;
    public bool isRight = false;
    public bool isCircle = false;

    public float upValue = 1f;
    public float downValue = 1f;
    public float leftValue = 1f;
    public float rightValue = 1f;
    public float circleValue = 1f;
    public float beltCount;
    public Vector2 beltPos;
    float _randTimer = 0f;
    public GameObject hMove;
    public GameObject vMove;
    public Transform[] moveTr;// = new Transform[2];
    public Transform targetTr;

    public int npcIndex;
    private float moveSpeed;

    private void Awake()
    {
        aircrewCanvas = FindObjectOfType<AircrewCanvas>();
        gameCtrl = FindObjectOfType<AircrewGameCtrl>();
        hMove = GameObject.Find("NPCHMove");
        vMove = GameObject.Find("NPCVMove");
        moveTr = new Transform[2];
    }

    private void Start()
    {
        //npcImage[1].enabled = false;
        _randTimer = Random.Range(3f, 7f);
        randBag = Random.Range(0, 2);
        complainCount = 0;
        anim = npcImage[1].GetComponent<Animator>();
        int rand = Random.Range(0, 5);
        npcImageIndex = rand;
        switch(rand)
        {
            case 0:
                npcImageInfo = "b_1,e_12,p_12,t_12,h_12,a_15";
                break;
            case 1:
                npcImageInfo = "b_1,e_11,p_11,t_11,h_11,a_14";
                break;
            case 2:
                npcImageInfo = "b_1,e_3,p_9,t_9,h_9,a_12";
                break;
            case 3:
                npcImageInfo = "b_1,e_10,p_10,t_10,h_10,a_13";
                break;
            case 4:
                npcImageInfo = "b_1,e_13,p_13,t_14,h_14,a_1";
                break;
        }
        npcState = NPCState.SIT;
        //resetRot = new Vector3(npcImage[0].transform.rotation.x, npcImage[0].transform.rotation.y, npcImage[0].transform.rotation.z);

        /*sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxSound1 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_result");
        sfxPlayer.playOnAwake = false;
        sfxPlayer.volume = 0.8f;*/
        SetNPC();
        ResetMoveValue();
    }

    public void SetNPC()
    {
        switch (npcState)
        {
            case NPCState.SIT:
                //David edit 2022-11-15
                //npcImage[0].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/NPC/" + npcImageName);
                npcImage[0].enabled = true;
                npcImage[0].sprite = npcSprites[npcImageIndex * 2];
                anim.SetBool("scared", false);
                anim.SetBool("belt", false);
                anim.SetBool("goldpacking", false);
                anim.SetBool("blackpacking", false);
                npcImage[1].enabled = false;
                npcImage[3].enabled = false;
                npcImage[4].enabled = false;
                npcImage[5].gameObject.SetActive(false);
                if (gameCtrl.gameState == AircrewGameCtrl.GamePlayState.ESCAPE) npcImage[2].enabled = true;
                else npcImage[2].enabled = false;
                npcImage[0].transform.rotation = Quaternion.Euler(resetRot);     // = resetRot;
                break;
            case NPCState.SCARED:
                //David edit 2022-11-15
                //npcImage[0].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/NPC/" + npcImageName);
                npcImage[0].enabled = true;
                npcImage[0].sprite = npcSprites[npcImageIndex * 2 + 1];
                npcImage[0].transform.rotation = Quaternion.Euler(resetRot);     //  = resetRot;
                npcImage[1].enabled = true;
                npcImage[2].enabled = false;
                npcImage[3].enabled = false;
                npcImage[4].enabled = true;
                npcImage[5].gameObject.SetActive(false);
                //npcImage[1].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/job_aircrew_object/img_job_aircrew_scare");
                //npcImage[1].SetNativeSize();
                anim.SetBool("goldpacking", false);
                anim.SetBool("blackpacking", false);
                anim.SetBool("belt", false);
                anim.SetBool("scared", true);
                break;            
            case NPCState.SAFETY:
                //David edit 2022-11-15
                //npcImage[0].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/NPC/" + npcImageName);
                npcImage[0].enabled = true;
                npcImage[0].sprite = npcSprites[npcImageIndex * 2];
                //npcImage[0].transform.rotation = Quaternion.Euler(npcImage[0].transform.rotation.x, npcImage[0].transform.rotation.y, npcImage[0].transform.rotation.z + 45f);
                npcImage[0].transform.rotation = Quaternion.Euler(resetRot);
                npcImage[1].enabled = false;
                npcImage[2].enabled = true;
                npcImage[3].enabled = false;
                npcImage[4].enabled = false;
                npcImage[5].gameObject.SetActive(false);
                anim.SetBool("goldpacking", false);
                anim.SetBool("blackpacking", false);
                anim.SetBool("scared", false);
                anim.SetBool("belt", false);
                break;            
            case NPCState.BELT:
                //David edit 2022-11-15
                //npcImage[0].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/NPC/" + npcImageName);
                npcImage[0].enabled = true;
                npcImage[0].sprite = npcSprites[npcImageIndex * 2];
                npcImage[0].transform.rotation = Quaternion.Euler(resetRot);     //  = resetRot;
                npcImage[1].enabled = true;
                npcImage[2].enabled = false;
                npcImage[3].enabled = false;
                npcImage[4].enabled = false;
                npcImage[5].gameObject.SetActive(false);
                npcImage[1].sprite = aircrewBelt;
                npcImage[1].SetNativeSize();
                anim.SetBool("goldpacking", false);
                anim.SetBool("blackpacking", false);
                anim.SetBool("scared", false); 
                anim.SetBool("belt", true);

                break;            
            case NPCState.PACKING:
                //David edit 2022-11-15
                //npcImage[0].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/NPC/" + npcImageName);
                npcImage[0].enabled = true;
                npcImage[0].sprite = npcSprites[npcImageIndex * 2];
                npcImage[0].transform.rotation = Quaternion.Euler(resetRot);     //  = resetRot;
                npcImage[1].enabled = true;
                npcImage[2].enabled = false;
                npcImage[3].enabled = false;
                npcImage[4].enabled = false;
                npcImage[5].gameObject.SetActive(false);
                anim.SetBool("scared", false);
                anim.SetBool("belt", false);
                anim.SetBool("goldpacking", false);
                anim.SetBool("blackpacking", false);
                if (randBag == 0) 
                { 
                    npcImage[1].sprite = aircrewBag[0];
                    npcImage[1].SetNativeSize();
                    anim.SetBool("blackpacking", true);
                }
                else if(randBag == 1)
                {
                    npcImage[1].sprite = aircrewBag[1];
                    npcImage[1].SetNativeSize();
                    anim.SetBool("goldpacking", true);
                }
                break;
            //David edit 2022-11-15
            case NPCState.HMOVE:
                //npcImage[0].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/NPC/" + npcImageName);
                //npcImage[0].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/New_NPC/" + npcImageName);
                //npcImage[0].transform.rotation = Quaternion.Euler(resetRot.x, resetRot.y, 0f);
                npcImage[0].enabled = false;
                //anim.SetBool("scared", true);
                anim.SetBool("scared", false);
                anim.SetBool("belt", false);
                anim.SetBool("goldpacking", false);
                anim.SetBool("blackpacking", false);
                //npcImage[1].enabled = true;
                npcImage[1].enabled = false;
                //npcImage[1].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/job_aircrew_object/img_job_aircrew_scare");
                npcImage[3].enabled = false;
                npcImage[2].enabled = false;
                npcImage[4].enabled = false;
                npcImage[5].gameObject.SetActive(false);
                //moveTr[0] = hMove.transform.GetChild(0).transform;
                //moveTr[1] = hMove.transform.GetChild(1).transform;
                //targetTr = moveTr[0];
                //npcImage[0].transform.rotation = Quaternion.Euler(resetRot);     // = resetRot;
                SoundManager.instance.PlayEffectSound("eff_Airplane_fuss", 1f);
                break;
            //David edit 2022-11-15
            case NPCState.VMOVE:
                //npcImage[0].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/NPC/" + npcImageName);
                //npcImage[0].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/New_NPC/" + npcImageName);
                //npcImage[0].transform.rotation = Quaternion.Euler(resetRot.x, resetRot.y, 0f);
                npcImage[0].enabled = false;
                //anim.SetBool("scared", true);
                anim.SetBool("scared", false);
                anim.SetBool("belt", false);
                anim.SetBool("goldpacking", false);
                anim.SetBool("blackpacking", false);
                //npcImage[1].enabled = true;
                npcImage[1].enabled = false;
                //npcImage[1].sprite = Resources.Load<Sprite>("gMiniGame/Aircrew/Images/job_aircrew_object/img_job_aircrew_scare");
                npcImage[3].enabled = false;
                npcImage[2].enabled = false;
                npcImage[4].enabled = false;
                npcImage[5].gameObject.SetActive(false);
                //moveTr[0] = vMove.transform.GetChild(0).transform;
                //moveTr[1] = vMove.transform.GetChild(1).transform;
                //targetTr = moveTr[0];
                //npcImage[0].transform.rotation = Quaternion.Euler(resetRot);     // = resetRot;
                SoundManager.instance.PlayEffectSound("eff_Airplane_fuss", 1f);
                break;
        }
        resetPos = this.transform.position;
        resetScale = this.transform.localScale;
        resetTr = this.transform.parent.transform;
    }

    private void OnDisable()  //게임패널이 비활성화되면 오브젝트를 삭제해서 NPCPos 를 비워준다
    {
        Destroy(this.gameObject);
    }

    int shakeCount = 0;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (this.npcState == NPCState.HMOVE || this.npcState == NPCState.VMOVE) return;

        if (gameCtrl.gameState == AircrewGameCtrl.GamePlayState.ESCAPE && gameCtrl.isGamePlaying)
        {
            if (npcState == NPCState.PACKING)
            {
                if (randBag == 0) { gameCtrl.SquareChange(true); }
                else if (randBag == 1) { gameCtrl.CircleChange(true); }
            }
            else if (npcState == NPCState.BELT)
            {
                beltPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                beltCount = 0f;
            }


            gameCtrl.EscapeGuideSetup(true);
            npcImage[2].enabled = false;
            mouseState = MouseState.DRAGON;
            if (npcState == NPCState.SIT || npcState == NPCState.PACKING)
            {
                this.transform.SetParent(gameCtrl.npcMoveSpace.transform);
                this.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                npcImage[3].enabled = true;
                shakeCount = 0;
            }

        }
        else return;

    }

    public List<Vector2> moveList = new List<Vector2>();

    public void OnDrag(PointerEventData eventData)
    {
        if (mouseState == MouseState.DRAGON)
        {
            Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (npcState == NPCState.SIT || npcState == NPCState.PACKING)
            {
                //Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 _startPos = this.transform.position;
                this.transform.position = _mousePos;
                npcImage[3].enabled = true;
                Vector2 _endPos = this.transform.position;

                v2 = _endPos - _startPos;
                float _angle = Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
                if (_angle < 0) _angle += 360f;
                else if (_angle > 360) _angle -= 360f;


                float _prevAngle = 0f;

                //승객 흔들어서 가방 떨어뜨리기
                //if (_distance.x < (-1f) || _distance.y < (-1f) || _distance.x > 1f || _distance.y > 1f)
                //{
                //    shakeCount++;
                //    StartCoroutine(PassengerShake());
                //    if (shakeCount == 7 && this.npcState != NPCState.SIT)
                //    {
                //        this.npcState = NPCState.SIT;
                //        if (npcImage[1].enabled) npcImage[1].enabled = false;
                //        StartCoroutine(ThrowBag(_distance));
                //        return;
                //    }
                //}

                //가방 종류에 따라 다른 제스처로 가방 떨어뜨리기
                moveList.Add(_mousePos);
                //사각형
                if (randBag == 0)
                {
                    if (moveList.Count > 21)
                    {
                        Vector2 _v2 = moveList[moveList.Count - 1] - moveList[moveList.Count - 20];
                        float _moveAngle = Mathf.Atan2(_v2.y, _v2.x) * Mathf.Rad2Deg;
                        if (_moveAngle < 0) _moveAngle += 360f;
                        else if (_moveAngle > 360) _moveAngle -= 360f;
                        SquareGesture(_moveAngle);
                    }
                }
                else if (randBag == 1) CircleGesture(_angle, _prevAngle);
            }
            else if (npcState == NPCState.BELT)
            {
                if ((beltPos.y - _mousePos.y) < 0f) beltCount += Time.deltaTime * 2f;
                
                if(beltCount > 0.10f)
                {
                    mouseState = MouseState.DRAGOFF;
                    this.npcState = NPCState.SIT;
                    SetNPC();
                }
            }
        }
        else return;
    }

    void SquareGesture(float _moveAngle) //사각형 제스처 동작
    {
        if (gameCtrl.gameState != AircrewGameCtrl.GamePlayState.ESCAPE) return;

        if (_moveAngle >= 0f && _moveAngle < 22.5f || _moveAngle >= 337.5f)
        {
            //Debug.Log("0 오른쪽 == 337.5보다 크고 22.5보다 작음");

            rightValue -= 0.3f;
            if (rightValue <= 0f)
            {
                isRight = true;
                rightValue = 0f;
            }
            gameCtrl.squareImages[0].fillAmount = rightValue;


            //rightValue -= 0.05f;
            //if (rightValue <= 0.75f)
            //{
            //    isRight = true;
            //    rightValue = 0.75f;
            //}
            //gameCtrl.square.fillAmount = rightValue;
            //upValue = 0.75f;
        }
        else if (_moveAngle >= 22.5f && _moveAngle < 67.5f)
        {
            //Debug.Log("1 오른쪽위 == 22.5보다 크고 67.5보다 작음");

        }
        else if (_moveAngle >= 67.5f && _moveAngle < 115.5f)
        {
            //Debug.Log("2 위 == 67.5보다 크고 115.5보다 작음");
            if (!isLeft) return;
            upValue -= 0.3f;
            if (upValue <= 0f)
            {
                isUp = true;
                upValue = 0f;
            }
            gameCtrl.squareImages[3].fillAmount = 0f;


            //if (!isLeft) return;

            //upValue -= 0.05f;
            //if (upValue <= 0.5f)
            //{
            //    isUp = true;
            //    upValue = 0.5f;
            //}
            //gameCtrl.square.fillAmount = upValue;
            //leftValue = 0.5f;

        }
        else if (_moveAngle >= 115.5f && _moveAngle < 157.5f)
        {
            //Debug.Log("3 위왼쪽 == 115.5보다 크고 157.5보다 작음");

        }
        else if (_moveAngle >= 157.5f && _moveAngle < 202.5f)
        {
            //Debug.Log("4 왼쪽 == 157.5보다 크고 202.5보다 작음");
            if (!isDown) return;
            leftValue -= 0.3f;
            if (leftValue <= 0f)
            {
                isLeft = true;
                leftValue = 0f;
            }
            gameCtrl.squareImages[2].fillAmount = 0f;


            //if (!isDown) return;

            //leftValue -= 0.05f;
            //if (leftValue <= 0.25f)
            //{
            //    isLeft = true;
            //    leftValue = 0.25f;
            //}
            //gameCtrl.square.fillAmount = leftValue;
            //downValue = 0.25f;

        }
        else if (_moveAngle >= 202.5f && _moveAngle < 247.5f)
        {
            //Debug.Log("5 왼쪽아래 == 202.5보다 크고 247.5보다 작음");

        }
        else if (_moveAngle >= 247.5f && _moveAngle < 292.5f)
        {
            //Debug.Log("6 아래 == 247.5보다 크고 292.5보다 작음");
            if (!isRight) return;
            downValue -= 0.3f;
            if (downValue <= 0f)
            {
                isDown = true;
                downValue = 0f;
            }

            gameCtrl.squareImages[1].fillAmount = 0f;


            //if (!isRight) return;

            //downValue -= 0.05f;
            //if (downValue <= 0f)
            //{
            //    isDown = true;
            //    downValue = 0f;
            //}

            //gameCtrl.square.fillAmount = downValue;
        }
        else if (_moveAngle >= 292.5f && _moveAngle < 337.5f)
        {
            //Debug.Log("7 아래오른쪽 == 202.5보다 크고 247.5보다 작음");

        }



        if (isUp && isDown && isRight && isLeft)
        {
            if (this.npcState != NPCState.SIT)
            {
                this.npcState = NPCState.SIT;
                if (npcImage[1].enabled) npcImage[1].enabled = false;
                StartCoroutine(ThrowBag(v2));
                return;
            }

        }


        //if (!isUp && !isDown && !isRight && !isLeft)
        //{
        //    if ((_angle < 45f && _angle > 0f || _angle < 0f && _angle > 315f || _angle == 0f))// && v2.x > 0.2f)
        //    {
        //        rightValue += v2.x;
        //        gameCtrl.squareImages[0].fillAmount = 1f - (rightValue * 0.4f);

        //        if (gameCtrl.squareImages[0].fillAmount <= 0f) isRight = true;
        //    }
        //}
        //else if (isLeft && !isUp && isDown && isRight)
        //{

        //    if ((_angle < 135f && _angle > 45f))// && v2.y > 0.2f)
        //    {
        //        upValue += v2.y;
        //        gameCtrl.squareImages[3].fillAmount = 1f - (upValue * 0.4f);

        //        if (upValue > 2.5f && isLeft) isUp = true;
        //    }
        //}
        //else if (isDown && !isUp && isRight && !isLeft)
        //{

        //    if ((_angle < 225f && _angle > 135f)) /*&& v2.x < (-0.2f))*/
        //    {
        //        leftValue += v2.x;


        //        gameCtrl.squareImages[2].fillAmount = 1f + (leftValue * 0.4f);
        //        if (leftValue < (-2.5f)) isLeft = true;

        //    }
        //}

        //else if (isRight && !isUp && !isDown && !isLeft)
        //{

        //    if ((_angle < 315f && _angle > 225f))// && v2.y < (-0.2f))
        //    {
        //        downValue += v2.y;

        //        gameCtrl.squareImages[1].fillAmount = 1f + (downValue * 0.4f);
        //        if (downValue < (-2.5f))
        //            isDown = true;


        //    }
        //}

        //if (isUp && isDown && isRight && isLeft)
        //{
        //    if (this.npcState != NPCState.SIT)
        //    {
        //        this.npcState = NPCState.SIT;
        //        if (npcImage[1].enabled) npcImage[1].enabled = false;
        //        StartCoroutine(ThrowBag(v2));
        //        return;
        //    }

        //}
    }

    void CircleGesture(float _angle, float _prevAngle)  //동그라미 제스처
    {
        if (gameCtrl.gameState != AircrewGameCtrl.GamePlayState.ESCAPE) return;

        if (moveList.Count > 21)
        {
            Vector2 _v2 = moveList[moveList.Count - 1] - moveList[moveList.Count - 20];
            Vector2 _v22 = moveList[moveList.Count - 1] - moveList[moveList.Count - 19];
            float _moveAngle = Mathf.Atan2(_v2.y, _v2.x) * Mathf.Rad2Deg;
            float _moveAngle1 = Mathf.Atan2(_v22.y, _v22.x) * Mathf.Rad2Deg;
            if (_moveAngle < 0) _moveAngle += 360f;
            else if (_moveAngle > 360) _moveAngle -= 360f;
            if (_moveAngle1 < 0) _moveAngle1 += 360f;
            else if (_moveAngle1 > 360) _moveAngle1 -= 360f;

            if (_moveAngle1 - _moveAngle > 0f)
            {
                circleValue -= 0.03f;
                if (circleValue <= 0f)
                {
                    circleValue = 0f;
                    if (this.npcState != NPCState.SIT)
                    {
                        this.npcState = NPCState.SIT;
                        if (npcImage[1].enabled) npcImage[1].enabled = false;
                        StartCoroutine(ThrowBag(v2));
                        return;
                    }
                }
                gameCtrl.circleImage.fillAmount = circleValue;

            }
            else
            {
                if (circleValue <= 0f) return;
                circleValue += 0.02f;
                if (circleValue >= 1f) circleValue = 1f;
                gameCtrl.circleImage.fillAmount = circleValue;
            }
        }







        //if (_prevAngle < _angle && _angle != 0)
        //{
        //    circleValue += Time.deltaTime * 1.2f;
        //    gameCtrl.circleImage.fillAmount = 1f - circleValue;
        //    _prevAngle = _angle;
        //    if (gameCtrl.circleImage.fillAmount <= 0f)
        //    {
        //        if (this.npcState != NPCState.SIT)
        //        {
        //            this.npcState = NPCState.SIT;
        //            if (npcImage[1].enabled) npcImage[1].enabled = false;
        //            StartCoroutine(ThrowBag(v2));
        //            return;
        //        }

        //    }
        //}
        //else /*if (_prevAngle > _angle && _angle != 0)*/ circleValue -= Time.deltaTime * 1.2f;    //ResetMoveValue();
    }

    IEnumerator PassengerShake()  //승객 잡고 흔들기
    {
        yield return new WaitForSeconds(0.5f);
        shakeCount = 0;
        yield break;
    }

    IEnumerator ThrowBag(Vector2 _distance)  //가방이 떨어지는 효과
    {
        yield return new WaitForFixedUpdate();
        GameObject _bag = Instantiate(bagPrefab, this.transform.position, Quaternion.identity);
        Image _bagImage = _bag.GetComponent<Image>();
        if (npcImage[1].sprite.name.Equals("img_job_aircrew_bag_01"))
        {
            _bagImage.sprite = aircrewBag[0];
        } else
        {
            _bagImage.sprite = aircrewBag[1];
        }
        _bag.transform.SetParent(this.transform);
        _bag.transform.localScale = this.transform.localScale;
        Color _bagColor = _bagImage.color;
        _bag.transform.SetParent(gameCtrl.npcMoveSpace.transform);
        _bag.transform.localScale = new Vector3(1f, 1f, 1f);
        Rigidbody2D _rb = _bag.GetComponent<Rigidbody2D>();

        while (_bagImage.color.a > 0)
        {
            yield return new WaitForFixedUpdate();
            
            _bagColor.a -= Time.deltaTime*0.3f;
            _bagImage.color = _bagColor;
            //_bag.transform.position += new Vector3(_distance.x, _distance.y).normalized * 1f * Time.deltaTime;
            _rb.velocity = _distance * 100f * Time.deltaTime;
            if (_bagImage.color.a <= 0f) Destroy(_bag.gameObject);
        }

        yield break;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (this.npcState == NPCState.HMOVE || this.npcState == NPCState.VMOVE) return;

        gameCtrl.EscapeGuideSetup(false);
        npcImage[3].enabled = false;
        if (randBag == 0) { gameCtrl.SquareChange(false); }
        else if (randBag == 1) { gameCtrl.CircleChange(false); }
        ResetMoveValue();
        if (!gameCtrl.isPassengerEscape)   //다른 승객이 탈출 중인 상태가 아닐때
        {
            switch (escapeState)
            {
                case EscapeState.IDLE:    //드래그 중이면서 탈출슬라이드와 닿지 않았을 때
                    this.transform.SetParent(resetTr);
                    this.transform.position = resetTr.transform.position;
                    this.transform.localScale = resetScale;
                    mouseState = MouseState.DRAGOFF;
                    SetNPC();
                    break;
                case EscapeState.ESCAPE:   
                    if (this.npcState == NPCState.SIT)  //드래그 중 탈출슬라이드와 닿았을 때 (성공)
                    {
                        /*sfxSound1 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_result");
                        sfxPlayer.clip = sfxSound1;
                        sfxPlayer.Play();*/
                        SoundManager.instance.PlayEffectSound("eff_Common_result", 1f);
                        mouseState = MouseState.DRAGON;
                        this.transform.position = escapeTr[0].transform.position;
                        npcImage[0].transform.rotation = Quaternion.Euler(0f, 0f, 45f);
                        npcImage[0].transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
                        gameCtrl.PassengerEscapeSuccess(this.gameObject);
                        StartCoroutine(NpcEscapeSlidingAction());
                    }
                    else
                    {
                        /*sfxSound1 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_denied");
                        sfxPlayer.clip = sfxSound1;
                        sfxPlayer.Play();*/
                        SoundManager.instance.PlayEffectSound("eff_Common_fail", 1f);
                        mouseState = MouseState.DRAGOFF;  //드래그 중 탈출슬라이드와 닿았을 때 (실패 / 가방을 가진 상태)
                        gameCtrl.PassengerEscapeFail();
                        this.transform.SetParent(resetTr);
                        this.transform.position = resetTr.transform.position;
                        this.transform.localScale = resetScale;
                        SetNPC();
                    }
                    break;
            }
        }
        else  //다른 승객이 탈출 중이라면 제자리로 되돌리기
        {
            this.transform.SetParent(resetTr);
            this.transform.position = resetTr.transform.position;
            this.transform.localScale = resetScale;
            mouseState = MouseState.DRAGOFF;
            SetNPC();
        }
    }

    void ResetMoveValue()
    {
        upValue = 1f;
        downValue = 1f;
        leftValue = 1f;
        rightValue = 1f;
        circleValue = 1f;

        isUp = false;
        isDown = false;
        isLeft = false;
        isRight = false;
    }


    IEnumerator NpcEscapeSlidingAction()  //탈출 장소로 npc 를 옮기면 실행될 애니메이션 연출
    {
        yield return new WaitForFixedUpdate();
        gameCtrl.escapeGuide[0].SetActive(false);
        npcImage[0].raycastTarget = false;
        Color _npcColor = npcImage[0].color;
        while (npcImage[0].color.a > 0)
        {

            this.transform.position = Vector3.MoveTowards(this.transform.position, escapeTr[1].position, 2f * Time.deltaTime);
            _npcColor.a -= Time.deltaTime *1.5f;
            npcImage[0].color = _npcColor;
            yield return new WaitForFixedUpdate();

        }

        gameCtrl.isPassengerEscape = false;
        Destroy(this.gameObject);
        
    }

    public void ClickPassengerPrefab()  //승객을 클릭하면... (난기류에서 안전상태로 변경 / 탈출상황에서 가방, 벨트 변경)
    {
        if (gameCtrl.gameState == AircrewGameCtrl.GamePlayState.ESCAPE && this.npcState == NPCState.VMOVE 
            || gameCtrl.gameState == AircrewGameCtrl.GamePlayState.ESCAPE && this.npcState == NPCState.HMOVE)
        { //돌아다니는 승객일 경우

        }


        if (gameCtrl.gameState == AircrewGameCtrl.GamePlayState.TUBULENCEAIR && this.npcState == NPCState.SCARED)
        {//게임이 난기류 상태일때 승객을 클릭하면 안전(SAFETY) 상태로 변경
            for (int i = 0; i < gameCtrl.moveNPC.Length; i++)
            {
                if (gameCtrl.moveNPC[i] != null)
                {
                    StartCoroutine(ShowDialog());
                    return;
                }
            }
            npcState = NPCState.SAFETY;
            SetNPC();
        }


        // 클릭 2회, 3회시 벨트/가방 제거
        //else if (gameCtrl.gameState == AircrewGameCtrl.GamePlayState.ESCAPE && this.npcState == NPCState.BELT)
        //{
        //    clickCount++;
        //    if (clickCount >= 2)
        //    {
        //        this.npcState = NPCState.SIT;
        //        SetNPC();
        //    }
        //    StartCoroutine(ClickCountReset());
        //}
        //else if (gameCtrl.gameState == AircrewGameCtrl.GamePlayState.ESCAPE && this.npcState == NPCState.PACKING)
        //{
        //    clickCount++;
        //    if (clickCount >= 3)
        //    {
        //        this.npcState = NPCState.SIT;
        //        SetNPC();
        //    }
        //    StartCoroutine(ClickCountReset());
        //}
    }

    public void MovePassengerPopup()   //돌아다니는 승객의 컴플레인 팝업 (돌아다니는 승객을 클릭하면)
    {
        //if (this.transform.parent == hMove || this.transform.parent == vMove)
        if (gameCtrl.popupNPC != null)
        {
            return;
        }
        if(this.npcState == NPCState.HMOVE || this.npcState == NPCState.VMOVE) {
            gameCtrl.SetComplainPopup(true, this.gameObject);
        }

        else return;
    }

    IEnumerator ClickCountReset()  //마우스클릭시 카운트 입력 시간 리셋
    {
        yield return new WaitForSeconds(0.6f);
        clickCount = 0;
    }



    private void Update()
    {
        if(this.npcState == NPCState.HMOVE && targetTr !=null)
        {
            //David edit 2022-11-16
            if (gameCtrl.moveNPC[npcIndex] == null) return;
            if (gameCtrl.moveNPC[npcIndex].transform.position != targetTr.position)
            {
                gameCtrl.moveNPC[npcIndex].transform.localScale = new Vector3(25f, 25f, 25f);
                gameCtrl.moveNPC[npcIndex].transform.position = Vector3.MoveTowards(gameCtrl.moveNPC[npcIndex].transform.position, targetTr.position, moveSpeed * Time.deltaTime);
                if (gameCtrl.moveNPC[npcIndex].transform.position == moveTr[0].position) targetTr = moveTr[1];
                else if (gameCtrl.moveNPC[npcIndex].transform.position == moveTr[1].position) targetTr = moveTr[0];

                if (targetTr == moveTr[0])
                {
                    gameCtrl.moveNPC[npcIndex].transform.localScale = new Vector3(25f, 25f, 25f);
                }
                else if (targetTr == moveTr[1])
                {
                    gameCtrl.moveNPC[npcIndex].transform.localScale = new Vector3(-25f, 25f, 25f);
                }
            }
        }
        else if (this.npcState == NPCState.VMOVE && targetTr != null)
        {
            //David edit 2022-11-16
            if (gameCtrl.moveNPC[npcIndex] == null) return;
            if (gameCtrl.moveNPC[npcIndex].transform.position != targetTr.position)
            {
                Vector3 _distance = moveTr[1].position - gameCtrl.moveNPC[npcIndex].transform.position;
                gameCtrl.moveNPC[npcIndex].transform.position = Vector3.MoveTowards(gameCtrl.moveNPC[npcIndex].transform.position, targetTr.position, moveSpeed * Time.deltaTime);
                gameCtrl.moveNPC[npcIndex].transform.localScale = new Vector3(25f, 25f, 25f);
                gameCtrl.moveNPC[npcIndex].transform.localScale = new Vector3((gameCtrl.moveNPC[npcIndex].transform.localScale.x + _distance.y * 1.6f),
                    (gameCtrl.moveNPC[npcIndex].transform.localScale.y + _distance.y * 1.6f), gameCtrl.moveNPC[npcIndex].transform.localScale.z);
                if (gameCtrl.moveNPC[npcIndex].transform.position == moveTr[0].position)
                {
                    targetTr = moveTr[1];
                }
                else if (gameCtrl.moveNPC[npcIndex].transform.position == moveTr[1].position)
                {
                    targetTr = moveTr[0];
                }
            }
        }


        if (gameCtrl.gameState == AircrewGameCtrl.GamePlayState.ESCAPE)
        {
            PassengerStateChange(0);
        }
        else if (gameCtrl.gameState == AircrewGameCtrl.GamePlayState.TUBULENCEAIR)
        {
            if (complainCount > 3) return;
            if (gameCtrl.npcTimer == -1) return;
            if (gameCtrl.npcTimer <= 0)
            {
                int randomPos = Random.Range(1, 3);
                PassengerStateChange(randomPos);
                gameCtrl.moveNpcCount++;
            }
        }
    }

    public static int complainCount = 0;


    void PassengerStateChange(int randInt)  //비상탈출 상황의 승객 상태 변경
    {
        if (mouseState == MouseState.DRAGOFF && this.npcState == NPCState.SIT || mouseState == MouseState.DRAGOFF && this.npcState == NPCState.SCARED)
        {
            if (randInt == 1)
            {
                //David edit 2022-11-15
                if (gameCtrl.moveNPC[npcIndex] == null)
                {
                    gameCtrl.npcTimer = gameCtrl.respawnNpcTimer;
                    moveSpeed = Random.Range(3, 7.5f);
                    complainCount++;
                    npcState = NPCState.HMOVE;
                    SetNPC();
                    stateTimer = 0f;
                    _randTimer = Random.Range(3f, 7f);
                    //gameCtrl.hNPC = this.gameObject;
                    gameCtrl.moveNPC[npcIndex] = Instantiate(aircrewAvatarImage);
                    AvatarImage newAvatarImage = gameCtrl.moveNPC[npcIndex].GetComponent<AvatarImage>();
                    newAvatarImage.SetBaseAnimation("Front_half_side_walking");
                    newAvatarImage.SettingAvatarImage(npcImageInfo);
                    gameCtrl.moveNPC[npcIndex].GetComponent<AircrewAvatarManager>().airPassenger = GetComponent<AirPassenger>();
                    moveTr[0] = hMove.transform.Find("Pos0").gameObject.transform;
                    moveTr[1] = hMove.transform.Find("Pos1").gameObject.transform;
                    targetTr = moveTr[0];
                    //this.transform.SetParent(hMove.transform);
                    //this.transform.position = targetTr.position;
                    gameCtrl.moveNPC[npcIndex].transform.SetParent(hMove.transform);
                    gameCtrl.moveNPC[npcIndex].transform.position = targetTr.position;
                    gameCtrl.moveNPC[npcIndex].transform.localScale = new Vector3(25f, 25f, 25f);
                    targetTr = moveTr[1];
                }
                else return;
            }
            else if (randInt == 2)
            {
                //David edit 2022-11-15
                if (gameCtrl.moveNPC[npcIndex] == null)
                {
                    gameCtrl.npcTimer = gameCtrl.respawnNpcTimer;
                    moveSpeed = Random.Range(2, 5f);
                    complainCount++;
                    npcState = NPCState.VMOVE;
                    SetNPC();
                    stateTimer = 0f;
                    _randTimer = Random.Range(3f, 7f);
                    //gameCtrl.vNPC = this.gameObject;
                    gameCtrl.moveNPC[npcIndex] = Instantiate(aircrewAvatarImage);
                    AvatarImage newAvatarImage = gameCtrl.moveNPC[npcIndex].GetComponent<AvatarImage>();
                    newAvatarImage.SetBaseAnimation("Front_half_side_walking");
                    newAvatarImage.SettingAvatarImage(npcImageInfo);
                    gameCtrl.moveNPC[npcIndex].GetComponent<AircrewAvatarManager>().airPassenger = GetComponent<AirPassenger>();
                    moveTr[0] = vMove.transform.Find("Pos0").gameObject.transform;
                    moveTr[1] = vMove.transform.Find("Pos1").gameObject.transform;
                    targetTr = moveTr[0];
                    //this.transform.SetParent(vMove.transform);
                    //this.transform.position = targetTr.position;
                    gameCtrl.moveNPC[npcIndex].transform.SetParent(vMove.transform);
                    gameCtrl.moveNPC[npcIndex].transform.position = targetTr.position;
                    gameCtrl.moveNPC[npcIndex].transform.localScale = new Vector3(25f, 25f, 25f);
                    targetTr = moveTr[1];
                }
                else return;
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.name == "bg")
        {
            escapeState = EscapeState.ESCAPE;
            gameCtrl.escapeGuide[0].SetActive(true);
            escapeTr[0] = collision.transform.Find("EscapePos1").transform;
            escapeTr[1] = collision.transform.Find("EscapePos2").transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "bg")
        {
            escapeState = EscapeState.IDLE;
            gameCtrl.escapeGuide[0].SetActive(false);
            escapeTr[0] = null;
            escapeTr[1] = null;
        }
    }

    //David edit 2022-11-15
    public void SetEnableNpcConfuse()
    {
        npcImage[4].enabled = false;
    }

    public void SetIndex(int index)
    {
        npcIndex = index;
    }

    IEnumerator ShowDialog()
    {
        npcImage[5].gameObject.SetActive(true);
        int rand = Random.Range(0, 3);
        if (rand == 0) {
            npcImage[5].transform.GetChild(0).GetComponent<TMP_Text>().text = "무슨 상황이야!! 너무 불안해!!";
        } else if (rand == 1)
        {
            npcImage[5].transform.GetChild(0).GetComponent<TMP_Text>().text = "이런 상황에 어떻게 진정하란거야!!";
        } else
        {
            npcImage[5].transform.GetChild(0).GetComponent<TMP_Text>().text = "저 사람들부터 어떻게든 해봐요!!";
        }
        yield return new WaitForSeconds(1.0f);
        npcImage[5].gameObject.SetActive(false);
    }
}
