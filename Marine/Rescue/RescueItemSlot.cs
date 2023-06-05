using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RescueItemSlot : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public RescueGameCtrl gameCtrl;

    //구조자의 몸에 있는 아이템 이미지를 담을 이미지변수
    //아이템을 끌어다 정해진 위치에 놓으면 콜라이더충돌이 일어난 오브젝트 이미지를 반투명하게 보이도록 처리
    public Image buttonImage;

    public string itemName;        //아이템의 이름
    public string itemImagePath;
    public GameObject itemObj;     //활성화된 아이템오브젝트를 담을 게임오브젝트변수
    public Button btn;             //아이템슬롯 버튼

    public GameObject gameWindow;  //게임팝업창 (아이템프리팹이 활성화될 경우 게임팝업창의 자식으로 생성)

    public bool isCorrect; //정답 여부 저장
    public bool isCollider; //콜라이더 충돌 여부 상태
    public Transform collTr; //정답콜라이더

    public GameObject rescueItemPrefab; //버튼을 눌러 드래그할 때 활성화될 아이템오브젝트

    /*public AudioSource sfxPlayer;
    public AudioClip sfxSound;*/

    private void Awake()
    {
        /*sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_dragstart");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.playOnAwake = false;
        sfxPlayer.volume = 0.8f;
        sfxPlayer.loop = false;*/
    }

    private void Start()
    {
        gameCtrl = FindObjectOfType<RescueGameCtrl>();
    }
    private void OnEnable()
    {
        itemName = this.transform.Find("Image").gameObject.GetComponent<Image>().sprite.name;
        switch (itemName)  //itemName = 아이템슬롯을 드래그할 때 아이템슬롯의 이미지이름으로 받아와서 생성되는 아이템의 이미지를 할당
        {
            case "item_001":
                itemImagePath = "item_001";
                break;
            case "item_002":
                itemImagePath = "item_002";
                break;
            case "item_015":
                itemImagePath = "item_015"; //Item/item_001";
                break;
            //case "object_09":
            //    itemImagePath = "gMiniGame/Marine/Obj/object_09"; //Item/item_001";
            //    break;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject _itemPrefab = Instantiate(rescueItemPrefab, this.transform.position, Quaternion.identity);
        _itemPrefab.transform.SetParent(gameWindow.transform);
        _itemPrefab.GetComponent<RescueItemPrefab>().itemName = itemName;
        _itemPrefab.GetComponent<RescueItemPrefab>().itemImagePath = itemImagePath;
        _itemPrefab.GetComponent<RescueItemPrefab>().itemSlotCtrl = this.transform.GetComponent<RescueItemSlot>();
        _itemPrefab.GetComponentInChildren<Image>().SetNativeSize();
        _itemPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
        Vector2 _imageSize = _itemPrefab.GetComponentInChildren<RectTransform>().rect.size;
        itemObj = _itemPrefab;
        itemObj.GetComponentInChildren<Image>().SetNativeSize();

        btn = this.transform.GetComponent<Button>();
        ColorBlock _colorBlock = btn.colors;
        _colorBlock.normalColor = new Color(0.549f, 0.549f, 0.549f);
        btn.colors = _colorBlock;
        /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_dragstart");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.Play();*/
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("드래그 중");
        Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        itemObj.transform.position = _mousePos;// eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_dragstop");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.Play();*/
        btn = this.transform.GetComponent<Button>();
        //버튼 색상 원래대로 되돌리기
        ColorBlock _colorBlock = btn.colors;
        _colorBlock.normalColor = new Color(1f, 1f, 1f);
        btn.colors = _colorBlock;

        CorrectFunction();
        Destroy(itemObj);
        itemObj = null;

    }

    public void CheckCorrect(bool _isCollider, bool _isCorrect, Transform _collTr)  //정답 여부 및 아이템프리팹이 충돌한 콜라이더 정보 수신
    {
        isCollider = _isCollider;
        isCorrect = _isCorrect;
        collTr = _collTr;
    }

    void CorrectFunction() //정답 처리
    {
        if (isCollider)  //콜라이더 정보가 있는 상태에서
        {
            if (isCorrect)  //정답에 일치하는 오브젝트라면 정답처리
            {
                gameCtrl.CheckCorrect(isCorrect, itemName, collTr);
                //if (collTr.GetComponent<BoxCollider2D>() != null) collTr.GetComponent<BoxCollider2D>().enabled = false;
                /*else*/ collTr.GetComponent<CircleCollider2D>().enabled = false;

                Color _color = collTr.GetComponent<Image>().color;
                _color.a = 1f;
                collTr.GetComponent<Image>().color = _color;
                this.gameObject.SetActive(false);
            }
            else  //정답과 다른 오브젝트라면 오답처리
            {
                gameCtrl.CheckCorrect(isCorrect, itemName, collTr);
            }
        }
        else return;
        
    }
}
