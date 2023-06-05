using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuestCtrl : MonoBehaviour
{
    public BaristaGameCtrl gameCtrl;
    public GameObject orderBox;
    public Image drinkImage;
    public Image avatarImage;
    public int avatarImageNum;
    //public Image effectImage;

    public TMP_Text scoreText;
    public int score = 0;      //에스프레소 : 100, 아메리카노 : 150, 아아 : 200, 카페라떼 : 250, 카푸치노 : 400

    public string npcImageName;
    public TMP_Text drinkText;
    public int drinkNumber;
    public int waitingNumber = 0;
    public Transform targetTr = null;
    public Transform exitTr = null;
    public BoxCollider2D coll2D;
    public float moveSpeed = 2f;
    public float waitingTime = 15f;
    public Image timerGauge;
    public TMP_Text waitingTimeText;
    public bool isOrder;
    public string menuName;
    public bool isOrderSuccess;

    [Header("이미지")]
    public Sprite[] npcSprites;
    public Sprite[] npcSprites02;
    public Sprite[] drinkSprites;

    /*public AudioSource sfxPlayer;
    public AudioClip sfxSound1;*/

    private void Awake()
    {
        gameCtrl = FindObjectOfType<BaristaGameCtrl>();
        coll2D = this.GetComponent<BoxCollider2D>();
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        avatarImage = this.transform.GetChild(0).GetComponent<Image>();
        //effectImage = this.transform.GetChild(1).GetComponent<Image>();
        //effectImage.transform.GetComponent<Animator>().enabled = false;
        //effectImage.enabled = false;
        /*sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxSound1 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_enter");
        sfxPlayer.clip = sfxSound1;
        sfxPlayer.playOnAwake = false;
        sfxPlayer.volume = 0.5f;
        sfxPlayer.loop = false;*/
    }
    private void OnEnable()
    {
        isOrderSuccess = false;
        isOrder = false;
        drinkNumber = Random.Range(0, 5);  //0:에스프레소, 1:아메리카노, 2:아이스아메리카노, 3:카페라떼, 4:카푸치노
        avatarImageNum = Random.Range(0, 6);
        SetNpc();
        SetDrink();
    }

    void SetNpc()  //npc 이미지
    {
        avatarImage.sprite = npcSprites[avatarImageNum];
    }

    void SetDrink()  //손님이 요구할 음료 정보 세팅
    {
        switch (drinkNumber)
        {
            case 0:
                drinkText.text = "에스프레소";
                menuName = "espresso";
                score = 100;
                drinkImage.sprite = drinkSprites[0];
                //Resources.Load<Sprite>("gMiniGame/Barista/Images/item/img_job_barista_coffee01_on")
                drinkImage.SetNativeSize();
                break;
            case 1:
                drinkText.text = "아메리카노";
                menuName = "americano";
                score = 150;
                drinkImage.sprite = drinkSprites[1];
                //Resources.Load<Sprite>("gMiniGame/Barista/Images/item/img_job_barista_coffee02_on_01")
                drinkImage.SetNativeSize();
                break;
            case 2:
                drinkText.text = "아이스 아메리카노";
                menuName = "iceamericano";
                score = 200;
                drinkImage.sprite = drinkSprites[2];
                //Resources.Load<Sprite>("gMiniGame/Barista/Images/item/img_job_barista_coffee03_on_02")
                drinkImage.SetNativeSize();
                break;
            case 3:
                drinkText.text = "카페라떼";
                menuName = "cafelatte";
                score = 250;
                drinkImage.sprite = drinkSprites[3];
                //Resources.Load<Sprite>("gMiniGame/Barista/Images/item/img_job_barista_coffee02_on_02")
                drinkImage.SetNativeSize();
                break;
            case 4:
                drinkText.text = "카푸치노";
                menuName = "cappuccino";
                score = 400;
                drinkImage.sprite = drinkSprites[4];
                //Resources.Load<Sprite>("gMiniGame/Barista/Images/item/img_job_barista_coffee02_on_03")
                drinkImage.SetNativeSize();
                break;
        }
        orderBox.SetActive(false);
        scoreText.gameObject.SetActive(false);
        //음료를 정한 후 바의 대기 위치로 이동
        StartCoroutine(MoveToBar());
    }

    IEnumerator MoveToBar()  //바로 이동
    {
        float delayTime = Random.Range(0f, 1.5f);
        yield return new WaitForSeconds(delayTime);
        coll2D.enabled = false;
        moveSpeed = Random.Range(1.5f, 3f);
        //sfxPlayer.Play();
        while (this.transform.position != targetTr.position)
        {
            yield return new WaitForFixedUpdate();
            this.transform.position = Vector3.MoveTowards(this.transform.position, targetTr.position, moveSpeed * Time.deltaTime);
        }
        /*sfxSound1 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_order");
        sfxPlayer.clip = sfxSound1;
        sfxPlayer.Play();*/
        orderBox.SetActive(true);

        coll2D.enabled = true;
        isOrder = true;
        waitingTime = Random.Range(50f, 65f);
        float _waitingTime = waitingTime;
        Color _color = new Color(38/255f, 235/255f, 117/255f);
        timerGauge.color = _color;
        //waitingTimeText.text = waitingTime.ToString("00");
        while (waitingTime > 0 && isOrder)
        {

            yield return new WaitForFixedUpdate(); // WaitForSeconds(1f);
            waitingTime -= Time.deltaTime; //1f;
            timerGauge.fillAmount = waitingTime / _waitingTime;

            if(timerGauge.fillAmount <= 0.3f)
            {
                _color = new Color(235 / 255f, 38 / 255f, 38 / 255f);
                timerGauge.color = _color;
            }
            else if(timerGauge.fillAmount <= 0.6f)
            {
                _color = new Color(255 / 255f, 205 / 255f, 51 / 255f);
                timerGauge.color = _color;
            }
            //waitingTimeText.text = waitingTime.ToString("00");
            if (waitingTime <= 0 || gameCtrl.timer <= 0)
            {
                isOrder = false;
            }
        }

        //주문 상태가 아닐 경우 (주문 실패 / 주문 성공 / 타임오버)
        if (!isOrder)
        {

            switch (isOrderSuccess)
            {
                case true:  //주문 성공
                    //yield return new WaitForSeconds(1f);
                    /*sfxSound1 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_casher");
                    sfxPlayer.clip = sfxSound1;
                    sfxPlayer.Play();*/
                    orderBox.SetActive(false);
                    scoreText.gameObject.SetActive(true);
                    StartCoroutine(ScoreTextEffect());
                    //스코어텍스트 애니메이션
                    
                        yield return new WaitForSeconds(1f);
                    //effectImage.gameObject.SetActive(false);
                    moveSpeed = Random.Range(2f, 4f);
                    while (this.transform.position != exitTr.position)
                    {
                        yield return new WaitForFixedUpdate();
                        this.transform.position = Vector3.MoveTowards(this.transform.position, exitTr.position, moveSpeed * Time.deltaTime);
                    }
                    break;
                case false:  //주문 실패
                    if (waitingTime <= 0)  //시간 초과에 의한 주문 실패
                    {
                        yield return new WaitForSeconds(0.3f);
                        orderBox.SetActive(false);
                        /*sfxSound1 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_wrong");
                        sfxPlayer.clip = sfxSound1;
                        sfxPlayer.Play();*/
                        yield return new WaitForSeconds(0.3f);
                        avatarImage.sprite = npcSprites02[avatarImageNum];
                        //Resources.Load<Sprite>("gMiniGame/Barista/Images/npc/" + npcImageName + "_02")
                        avatarImage.SetNativeSize();
                        avatarImage.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        yield return new WaitForSeconds(0.4f);
                        while (avatarImage.color.a > 0)
                        {
                            Color _alphaColor = avatarImage.color;
                            _alphaColor.a -= Time.deltaTime;
                            avatarImage.color = _alphaColor;
                            yield return new WaitForFixedUpdate();
                        }
                    }
                    else  //주문한 음료와 다를 때의 실패
                    {
                        gameCtrl.AlertFunction(); //화면테두리 붉은 경고
                        /*sfxSound1 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Barista_wrong");
                        sfxPlayer.clip = sfxSound1;
                        sfxPlayer.Play();*/
                        yield return new WaitForSeconds(0.3f);
                        orderBox.SetActive(false); 
                        yield return new WaitForSeconds(0.3f);
                        avatarImage.sprite = npcSprites02[avatarImageNum];
                        //Resources.Load<Sprite>("gMiniGame/Barista/Images/npc/" + npcImageName + "_02")
                        avatarImage.SetNativeSize();
                        avatarImage.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                        yield return new WaitForSeconds(0.4f);
                        moveSpeed = Random.Range(2f, 4f);
                        while (this.transform.position != exitTr.position)
                        {
                            yield return new WaitForFixedUpdate();
                            this.transform.position = Vector3.MoveTowards(this.transform.position, exitTr.position, moveSpeed * Time.deltaTime);
                        }
                    }
                    break;
            }
            waitingTimeText.text = "";
            coll2D.enabled = false;
        }

        gameCtrl.waitingGuests[waitingNumber] = null;
        Destroy(this.gameObject);

    }

    IEnumerator ScoreTextEffect()   //정답 시 획득 점수 연출
    {
        float _delay = 1f;
        float _yPos = scoreText.transform.localPosition.y;
        Color _textColor = scoreText.color;
        _textColor.a = 1f;
        while(_delay > 0f)
        {
            if(gameCtrl.timer < 46f)
            {
                scoreText.text = "+" + (score*2).ToString();
            }
            else scoreText.text = "+" + score.ToString();
            scoreText.color = _textColor;
            _textColor.a -= Time.deltaTime;
            yield return new WaitForFixedUpdate();
            scoreText.transform.localPosition = new Vector3(scoreText.transform.localPosition.x, _yPos, scoreText.transform.localPosition.z);
            _yPos += 0.5f;
        }
        scoreText.gameObject.SetActive(false);
    }
}
