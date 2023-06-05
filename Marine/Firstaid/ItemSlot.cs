using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public FirstaidGameCtrl gameCtrl;  //해파리응급처치 스크립트
    public GameObject itemPrefab;      //드래그 시작 시 생성할 아이템 프리펩
    public GameObject itemAnimPrefab;  //상호작용 시 활성화될 애니메이션 오브젝트
    public Transform itemAnimSpawnTr;  //애니메이션 오브젝트가 생성될 위치
    public Transform creamPosTr;
    public Transform lv2ItemAnimSpawnTr;//2단계 애니메이션 오브젝트가 생성될 위치 (collision 오브젝트)
    public GameObject itemObj;         //아이템 프리펩을 담을 게임오브젝트
    public Button btn;

    public string slotName;
    public string itemName;
    public string itemImagePath;
    public string lv3ItemName; //3단계 응급처치에서 선택된 아이템의 이름 (ex. "item_003")

    public bool isCorrect;
    public enum State { TRUE, FALSE, }
    public State state = State.FALSE;
    public int gameLevel;

    //아이템 이미지 이름
    //item_003 = 식염수
    //item_004 = 생수
    //item_005 = 탄산음료
    //item_006 = 온수
    //item_007 = 에너지드링크
    //item_008 = 에탄올
    //item_009 = 바닷물

    public AudioSource sfxPlayer;
    public AudioSource sfxPlayer2;
    public AudioClip sfxSound;
    public AudioClip sfxSound2;

    public Sprite[] Level1Sprites;
    public Sprite[] Level2Sprites;
    public Sprite[] Level3Sprites;

    private void Awake()
    {
        /*sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_dragstart");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.playOnAwake = false;
        sfxPlayer.volume = 0.5f;
        sfxPlayer.loop = false; 
        sfxPlayer2 = gameObject.AddComponent<AudioSource>();
        sfxSound2 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_correct");
        sfxPlayer2.clip = sfxSound2;
        sfxPlayer2.playOnAwake = false;
        sfxPlayer2.volume = 0.5f;
        sfxPlayer2.loop = false;*/
    }
    private void Start()
    {
        gameCtrl = FindObjectOfType<FirstaidGameCtrl>();
        itemAnimSpawnTr = GameObject.Find("ItemSpawnPos1").transform;
        creamPosTr = GameObject.Find("CreamPos").transform;
        slotName = this.transform.name;
        //itemName = this.transform.Find("Item").gameObject.GetComponent<Image>().sprite.name;
        SetItemInfor();
        //Debug.Log("아이템이름 " + itemName);
        //Debug.Log("아이템경로 " + itemImagePath);
    }

    private void OnEnable()
    {
        itemAnimSpawnTr = GameObject.Find("ItemSpawnPos1").transform;
    }

    public void SetItemInfor()  //아이템 이미지 경로 확인 (게임레벨에 따라 슬롯의 이미지와 텍스트 변경)
    {
        if (gameLevel == 1)
        {
            switch (slotName)
            {
                case "ItemSlot1": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level1Sprites[0]; break;
                case "ItemSlot2": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level1Sprites[1]; break;
                case "ItemSlot3": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level1Sprites[2]; break;
                case "ItemSlot4": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level1Sprites[3]; break;
                case "ItemSlot5": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level1Sprites[4]; break;
                case "ItemSlot6": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level1Sprites[5]; break;
                case "ItemSlot7": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level1Sprites[6]; break;
            }
            //this.transform.Find("Item").gameObject.GetComponent<Image>().SetNativeSize();
            //itemName = this.transform.Find("Item").gameObject.GetComponent<Image>().sprite.name;
        }
        else if (gameLevel == 2)
        {
            switch (slotName)
            {
                case "ItemSlot1": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level2Sprites[0]; break;
                case "ItemSlot2": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level2Sprites[1]; break;
                case "ItemSlot3": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level2Sprites[2]; break;
                case "ItemSlot4": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level2Sprites[3]; break;
                case "ItemSlot5": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level2Sprites[4]; break;
                case "ItemSlot6": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level2Sprites[5]; break;
                case "ItemSlot7": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level2Sprites[6]; break;
            }
            //itemName = this.transform.Find("Item").gameObject.GetComponent<Image>().sprite.name;
        }
        else if (gameLevel == 3)
        {
            switch (slotName)
            {
                case "ItemSlot1": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level3Sprites[0]; break;
                case "ItemSlot2": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level3Sprites[1]; break;
                case "ItemSlot3": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level3Sprites[2]; break;
                case "ItemSlot4": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level3Sprites[3]; break;
                case "ItemSlot5": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level3Sprites[4]; break;
                case "ItemSlot6": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level3Sprites[5]; break;
                case "ItemSlot7": this.transform.Find("Item").gameObject.GetComponent<Image>().sprite = Level3Sprites[6]; break;
            }
            //itemName = this.transform.Find("Item").gameObject.GetComponent<Image>().sprite.name;
        }
        this.transform.Find("Item").gameObject.GetComponent<Image>().SetNativeSize();
        itemName = this.transform.Find("Item").gameObject.GetComponent<Image>().sprite.name;


        switch (itemName)
        {
            case "item_003": itemImagePath = "object_34"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "생리 식염수"; break;
            case "item_004": itemImagePath = "object_33"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "생수"; break;
            case "item_005": itemImagePath = "object_32"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "탄산음료"; break;
            case "item_006": itemImagePath = "object_35"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "온수"; break;
            case "item_007": itemImagePath = "object_31"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "에너지 드링크"; break;
            case "item_008": itemImagePath = "object_30"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "소독용 에탄올"; break;
            case "item_009": itemImagePath = "object_36"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "바닷물"; break;
            case "item_010": itemImagePath = "object_41"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "나무젓가락"; break;
            case "item_011": itemImagePath = "object_44"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "조개 껍데기"; break;
            case "item_012": itemImagePath = "object_45"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "플라스틱 카드"; break;
            case "item_013": itemImagePath = "object_46"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "핀셋"; break;
            case "item_014": itemImagePath = "object_42"; this.transform.Find("Text (TMP)").gameObject.GetComponent<TMP_Text>().text = "연고"; break;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_dragstart");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.Play();*/
        GameObject _itemPrefab = Instantiate(itemPrefab, this.transform.position, Quaternion.identity);
        _itemPrefab.GetComponent<ItemPrefab>().itemName = itemName;
        _itemPrefab.GetComponent<ItemPrefab>().itemImagePath = itemImagePath;
        _itemPrefab.GetComponent<ItemPrefab>().itemSlotTr = this.transform;
        itemObj = _itemPrefab;

        btn = this.transform.GetComponent<Button>();
        ColorBlock _colorBlock = btn.colors;
        _colorBlock.normalColor = new Color(0.549f, 0.549f, 0.549f);
        btn.colors = _colorBlock;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        itemObj.transform.position = _mousePos;// eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_dragstop");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.Play();*/
        ColorBlock _colorBlock = btn.colors;
        _colorBlock.normalColor = new Color(1f,1f, 1f);
        btn.colors = _colorBlock;
        itemAnimPrefab = (GameObject)Resources.Load("Marine/" + itemName);

        //아이템별 사운드 할당
        /*switch (gameLevel)
        {
            case 1: sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_water");
                sfxPlayer.clip = sfxSound; sfxPlayer.Play(); break;
            case 3:
                if(itemName == "item_009" || itemName == "item_006" || itemName == "item_003" || itemName == "item_004" || itemName == "item_008")
                {
                    sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_water");
                    sfxPlayer.clip = sfxSound; sfxPlayer.Play();
                }
                break;
        }*/

        switch (state)
        {
            case State.TRUE:
                //게임레벨(단계)와 정답 여부 전달/확인
                if (gameLevel == 1)
                {
                    //레벨1의 애니메이션 위치 (4.3f, 3.0f, 0.0f)
                    gameCtrl.CorrectFunc(gameLevel, isCorrect);
                    GameObject _itemAnim = Instantiate(itemAnimPrefab, itemAnimSpawnTr.position, itemAnimPrefab.transform.localRotation);
                    _itemAnim.transform.SetParent(gameCtrl.gameWindow.transform);
                    _itemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
                    _itemAnim.transform.localPosition = new Vector3(_itemAnim.transform.localPosition.x, _itemAnim.transform.localPosition.y, 0);
                    //정답/오답 사운드
                    if (isCorrect)
                    {
                        /*sfxSound2 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_correct");
                        sfxPlayer2.clip = sfxSound2; sfxPlayer2.Play();*/

                        if (this.itemName + "(Clone)" == _itemAnim.name)
                        {
                            StartCoroutine(ItemSlotActiveFlase(1.5f));  //사용한 아이템슬롯 비활성화
                        }
                    }

                    Destroy(_itemAnim, 1f);
                }
                else if (gameLevel == 2)
                {
                    if (isCorrect)  //정답일 경우
                    {
                        /*sfxPlayer2.clip = sfxSound2; sfxPlayer2.Play();*/
                        gameCtrl.CorrectFunc(gameLevel, isCorrect);
                        //레벨2의 애니메이션 위치 (파트별로 상이)
                        GameObject _itemAnim = Instantiate(itemAnimPrefab, itemAnimSpawnTr.position, Quaternion.identity);
                        _itemAnim.transform.SetParent(gameCtrl.gameWindow.transform);
                        _itemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
                        _itemAnim.transform.localPosition = new Vector3(_itemAnim.transform.localPosition.x, _itemAnim.transform.localPosition.y, 0);
                        Destroy(_itemAnim, 2f);
                        StartCoroutine(Level2InjuryAnimation());
                    }
                    else  //정답이 아니면 컬러값 원래상태로 돌리기
                    {
                        gameCtrl.CorrectFunc(gameLevel, isCorrect);
                        Color _color2 = new Color(1f, 1f, 1f, 1f);

                        Destroy(itemObj);  //드래그 중인 아이템프리팹 삭제
                        for (int i = 0; i < gameCtrl.jellyfishImages.Length; i++)
                        {
                            if (lv2ItemAnimSpawnTr.name == gameCtrl.jellyfishImages[i].name)
                            {
                                gameCtrl.jellyfishImages[i].color = _color2;
                                return;
                            }
                        }
                        //lv2ItemAnimSpawnTr.transform.GetComponentInChildren<SpriteRenderer>().color = _color2;
                    }
                }
                else if (gameLevel == 3)
                {
                    Vector3 _waterAnimPos = new Vector3(4.25f, 2.5f, 0f);
                    Vector3 _creamAnimPos = new Vector3(2.129f, 1.584f, 0f);
                    Vector3 _animPos = new Vector3(0f, 0f, 0f);
                    if (lv3ItemName == "item_014") _animPos = _creamAnimPos;
                    else _animPos = _waterAnimPos;
                    int _correctCount = gameCtrl.correctCount;
                    if (isCorrect && _correctCount == 0)  //1번째 정답
                    {
                        if (lv3ItemName == "item_006")
                        {
                            gameCtrl.CorrectFunc(gameLevel, isCorrect);
                            /*sfxSound2 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_correct");
                            sfxPlayer2.clip = sfxSound2; sfxPlayer2.Play();*/

                            GameObject _itemAnim = Instantiate(itemAnimPrefab, itemAnimSpawnTr.position, itemAnimPrefab.transform.localRotation);
                            _itemAnim.transform.SetParent(gameCtrl.gameWindow.transform);
                            _itemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
                            _itemAnim.transform.localPosition = new Vector3(_itemAnim.transform.localPosition.x, _itemAnim.transform.localPosition.y, 0);
                            if (this.itemName + "(Clone)" == _itemAnim.name)
                            {
                                StartCoroutine(ItemSlotActiveFlase(1.5f));  //정답 아이템슬롯 비활성화
                            }
                            Destroy(_itemAnim, 1.5f);
                        }
                        else
                        {
                            isCorrect = false;
                            gameCtrl.CorrectFunc(gameLevel, isCorrect);
                            GameObject _itemAnim = Instantiate(itemAnimPrefab, itemAnimSpawnTr.position, itemAnimPrefab.transform.localRotation);
                            _itemAnim.transform.SetParent(gameCtrl.gameWindow.transform);
                            _itemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
                            _itemAnim.transform.localPosition = new Vector3(_itemAnim.transform.localPosition.x, _itemAnim.transform.localPosition.y, 0);
                            if (lv3ItemName == "item_014") _itemAnim.transform.position = creamPosTr.position;
                            else _itemAnim.transform.localPosition = new Vector3(450f, 320f, 0f);   //itemAnimSpawnTr.position;
                            /*if (this.itemName + "(Clone)" == _itemAnim.name)
                            {
                                StartCoroutine(ItemSlotActiveFlase(1.5f));  //사용한 아이템슬롯 비활성화
                            }*/
                            Destroy(_itemAnim, 1.5f);
                        }
                    }
                    else if (isCorrect && _correctCount == 1) //2번째 정답
                    {
                        if (lv3ItemName == "item_003" || lv3ItemName == "item_009")
                        {
                            /*sfxSound2 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_correct");
                            sfxPlayer2.clip = sfxSound2; sfxPlayer2.Play();*/
                            gameCtrl.CorrectFunc(gameLevel, isCorrect);
                            GameObject _itemAnim = Instantiate(itemAnimPrefab, itemAnimSpawnTr.position, itemAnimPrefab.transform.localRotation);
                            _itemAnim.transform.SetParent(gameCtrl.gameWindow.transform);
                            _itemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
                            _itemAnim.transform.localPosition = new Vector3(450f, 320f, 0f);   //itemAnimSpawnTr.position;
                            if (this.itemName + "(Clone)" == _itemAnim.name)
                            {
                                StartCoroutine(ItemSlotActiveFlase(1.5f));  //정답 아이템슬롯 비활성화
                            }
                            Destroy(_itemAnim, 1.5f);
                        }
                        else
                        {
                            isCorrect = false;
                            gameCtrl.CorrectFunc(gameLevel, isCorrect);
                            GameObject _itemAnim = Instantiate(itemAnimPrefab, itemAnimSpawnTr.position, itemAnimPrefab.transform.localRotation);
                            _itemAnim.transform.SetParent(gameCtrl.gameWindow.transform);
                            _itemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
                            if (lv3ItemName == "item_014") _itemAnim.transform.position = creamPosTr.position;
                            else _itemAnim.transform.localPosition = new Vector3(450f, 320f, 0f);   //itemAnimSpawnTr.position;
                            /*if (this.itemName + "(Clone)" == _itemAnim.name)
                            {
                                StartCoroutine(ItemSlotActiveFlase(1.5f));  //사용한 아이템슬롯 비활성화
                            }*/
                            Destroy(_itemAnim, 1.5f);
                        }
                    }
                    else if (isCorrect && _correctCount == 2) //3번째 정답
                    {
                        if (lv3ItemName == "item_014")
                        {
                            /*sfxSound2 = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Jellyfish_correct");
                            sfxPlayer2.clip = sfxSound2; sfxPlayer2.Play();*/
                            gameCtrl.CorrectFunc(gameLevel, isCorrect);
                            GameObject _itemAnim = Instantiate(itemAnimPrefab, _creamAnimPos /*itemAnimSpawnTr.position*/, itemAnimPrefab.transform.localRotation);
                            _itemAnim.transform.SetParent(gameCtrl.gameWindow.transform);
                            _itemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
                            _itemAnim.transform.localPosition = new Vector3(_itemAnim.transform.localPosition.x, _itemAnim.transform.localPosition.y, 0);
                            _itemAnim.transform.position = creamPosTr.position;
                            if (this.itemName + "(Clone)" == _itemAnim.name)
                            {
                                StartCoroutine(ItemSlotActiveFlase(1.5f));  //정답 아이템슬롯 비활성화
                            }
                            Destroy(_itemAnim, 1.75f);
                        }
                        else
                        {
                            isCorrect = false;
                            gameCtrl.CorrectFunc(gameLevel, isCorrect);
                            GameObject _itemAnim = Instantiate(itemAnimPrefab, itemAnimSpawnTr.position, itemAnimPrefab.transform.localRotation);
                            _itemAnim.transform.SetParent(gameCtrl.gameWindow.transform);
                            _itemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
                            _itemAnim.transform.localPosition = new Vector3(450f, 320f, 0f);   //itemAnimSpawnTr.position;
                            /*if (this.itemName + "(Clone)" == _itemAnim.name)
                            {
                                StartCoroutine(ItemSlotActiveFlase(1.5f));  //사용한 아이템슬롯 비활성화
                            }*/
                            Destroy(_itemAnim, 1.5f);
                        }
                    }
                    else
                    {
                        gameCtrl.CorrectFunc(gameLevel, isCorrect);
                        GameObject _itemAnim = Instantiate(itemAnimPrefab, itemAnimSpawnTr.position, itemAnimPrefab.transform.localRotation);
                        _itemAnim.transform.SetParent(gameCtrl.gameWindow.transform);
                        _itemAnim.transform.localScale = new Vector3(1f, 1f, 1f);
                        if (lv3ItemName == "item_014") _itemAnim.transform.position = creamPosTr.position;
                        else _itemAnim.transform.localPosition = new Vector3(450f, 320f, 0f);   //itemAnimSpawnTr.position;
                        /*if (this.itemName + "(Clone)" == _itemAnim.name)
                        {
                            StartCoroutine(ItemSlotActiveFlase(1.5f));  //사용한 아이템슬롯 비활성화
                        }*/
                        Destroy(_itemAnim, 1.5f);
                    }
                }

                gameCtrl.ItemSlotButtonSetFalse(true);

                StartCoroutine(ItemSlotButtonSetTrue()); //아이템 사용 이벤트 시작 2초 후 버튼 재사용 가능
                Destroy(itemObj);  //드래그 중인 아이템프리팹 삭제
                break;
            case State.FALSE:
                Destroy(itemObj);  //드래그 중인 아이템프리팹 삭제
                itemObj = null;
                break;
        }
    }

    IEnumerator ItemSlotActiveFlase(float _delay) //사용한 아이템슬롯 삭제
    {
        yield return new WaitForSeconds(_delay);
        gameCtrl.ItemSlotButtonSetFalse(false);
        this.gameObject.SetActive(false);
    }

    IEnumerator Level2InjuryAnimation()   //해파리 촉수/독침 오브젝트 투명하게 없애기
    {
        Color _color = new Color(0f, 0f, 0f);
        int imageNumber = 0;
        for (int i = 0; i < gameCtrl.jellyfishImages.Length; i++)
        {
            if (itemAnimSpawnTr.name == gameCtrl.jellyfishImages[i].name)
            {
                _color = gameCtrl.jellyfishImages[i].color;
                imageNumber = i;
            }
        }
        _color.a = 1f;
        while (_color.a > 0)
        {
            _color.a -= Time.deltaTime*0.5f;
            gameCtrl.jellyfishImages[imageNumber].color = _color;
            yield return new WaitForFixedUpdate();
        }
        itemAnimSpawnTr.gameObject.SetActive(false);
    }

    IEnumerator ItemSlotButtonSetTrue()  //아이템 사용 이벤트 시작 2초 후 버튼 재사용 가능
    {
        yield return new WaitForSeconds(2f);
        gameCtrl.ItemSlotButtonSetFalse(false);
    }


    //아이템 프리펩이 상처부위와 트리거 시작될 때, 벗어났을 때 정보 전달
    //(게임단계, 정답여부, 콜라이더접촉여부, 2단계/접촉중인 콜라이더의 트랜스폼, 3단계/아이템이름)
    public void CheckItemPrefabStatus(int _level, bool _checkCorrect, bool _state, Transform _lv2ObjTr, string _lv3ItemName)
    {
        switch (_state)
        {
            case true: state = State.TRUE; break; //콜라이더 접촉 중
            case false: state = State.FALSE; break;  //콜라이더 접촉 끝
        }
        switch (_level)
        {
            case 1:
                gameLevel = _level;
                isCorrect = _checkCorrect;
                break;
            case 2:
                gameLevel = _level;
                isCorrect = _checkCorrect;
                break;
            case 3:
                gameLevel = _level;
                isCorrect = _checkCorrect;
                lv3ItemName = _lv3ItemName;
                break;
        }
        lv2ItemAnimSpawnTr = _lv2ObjTr;
    }
}
