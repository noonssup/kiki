using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    //커피 제조 시 터치(클릭) / 드래그로 이동되는 아이템의 조작 스크립트
    /*
    000 노말 포터필터 : NormalPoterFilter
    001 탬핑매트 포터필터 : TemperPorterFilter
    002 탬퍼 : Temper
    003 에스프레소컵 : EspressoCup
    -완성커피
    004 에스프레소 (완성) : EspressoCoffee
    005 아메리카노 (완성) : AmericanoCoffee
    006 아이스 아메리카노 (완성) : IceAmericanoCoffee
    007 카페라떼 (완성) : CaffeLatteCoffee
    008 카푸치노 (완성) : CappuccinoCoffee
    -
    009 아이스 아메리카노 빈컵 (얼음담기용)  : IceCup
    010 밀크저그  : MilkJug
    011 휘핑크림  : WhippingCream
    012 시나몬파우더  : CinnamonPowder
    */


    public BaristaGameCtrl gameCtrl;
    public string itemName;
    public GameObject coffeeTable;    //커피제조 아이템들의 부모로 지정할 오브젝트
    public GameObject itemPrefab;
    public GameObject dragItemPrefab;

    [Header("이미지")]
    public Sprite img_job_barista_filter_off;
    public Sprite img_job_barista_filter_on_02;
    public Sprite img_job_barista_coffee01_off;
    public Sprite img_job_barista_coffee03_off;
    public Sprite img_job_barista_coffee02_off;
    public Sprite img_job_barista_scoop_ing;
    public Sprite img_job_barista_milk_off;
    public Sprite img_job_barista_spray_off;
    public Sprite img_job_barista_powder_off;
    public Sprite img_job_barista_shot_on;


    public bool isIce = false; //아이스컵 얼음여부


    /*public AudioSource sfxPlayer;
    public AudioClip sfxSound;*/


    private void Awake()
    {
        gameCtrl = FindObjectOfType<BaristaGameCtrl>();
        coffeeTable = GameObject.Find("CoffeeTable");
        itemName = this.gameObject.name;
        /*sfxPlayer = gameObject.AddComponent<AudioSource>();
        sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_dragstart");
        sfxPlayer.playOnAwake = false;
        sfxPlayer.volume = 0.5f;
        sfxPlayer.loop = false;
        sfxPlayer.clip = sfxSound;*/
    }




    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(1)) return;
        else
        {
            itemPrefab = (GameObject)Instantiate(dragItemPrefab, this.transform.position, Quaternion.identity);
            /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_dragstart");
            sfxPlayer.clip = sfxSound;
            sfxPlayer.Play();*/
            switch (itemName)
            {
                case "NormalPorterFilter":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_filter_off;
                    break;
                case "TemperPorterFilter":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_filter_on_02;
                    break;
                //case "Temper":
                //    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("gMiniGame/Barista/Images/item/img_job_barista_temping_02");
                //    break;
                case "EspressoCup":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_coffee01_off;
                    break;
                case "IceCup":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_coffee03_off;
                    break;
                case "HotCup":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_coffee02_off;
                    break;
                case "Scoop":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_scoop_ing;
                    break;
                case "MilkJug":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_milk_off;
                    break;
                case "WhippingCream":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_spray_off;
                    break;
                case "CinnamonPowder":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_powder_off;
                    break;
                case "EspressoCupM0":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_shot_on;
                    break;
                case "EspressoCupM1":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = img_job_barista_shot_on;
                    break;
                case "CompleteCoffee":
                    itemPrefab.transform.GetChild(0).GetComponent<Image>().sprite = this.transform.GetChild(1).transform.GetComponent<Image>().sprite;
                    break;
            }
            itemPrefab.transform.GetChild(0).GetComponent<Image>().SetNativeSize();
            itemPrefab.transform.SetParent(coffeeTable.transform);     //parent = coffeeTable.transform;
            itemPrefab.transform.localScale = new Vector3(1f, 1f, 1f);
            itemPrefab.GetComponent<CoffeeDragItemPrefab>().dragItem = this;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (itemPrefab != null) itemPrefab.transform.position = _mousePos;
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        /*sfxSound = (AudioClip)Resources.Load("gMiniGame/Sounds/eff_Common_dragstop");
        sfxPlayer.clip = sfxSound;
        sfxPlayer.Play();*/
        if (itemPrefab != null)
        {
            string _coffeeName = itemPrefab.GetComponent<CoffeeDragItemPrefab>().coffeeName;
            Transform _guestTr = itemPrefab.GetComponent<CoffeeDragItemPrefab>().guestTr;
            //string _orderMenu = null;
            
            if (_guestTr != null)
            {
                string _orderMenu = _guestTr.GetComponent<GuestCtrl>().menuName;
                if (_coffeeName == _orderMenu)
                {

                    gameCtrl.ScoreUp(_coffeeName);
                    _guestTr.GetComponent<GuestCtrl>().isOrder = false;
                    _guestTr.GetComponent<GuestCtrl>().isOrderSuccess = true;
                    Destroy(itemPrefab); gameCtrl.coffeeMakeStep = 0;
                    //this.gameObject.SetActive(false);
                    //this.transform.GetComponent<Image>().enabled = false;
                    this.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                }
                else
                {
                    Destroy(itemPrefab);
                    _guestTr.GetComponent<GuestCtrl>().isOrder = false;
                    _guestTr.GetComponent<GuestCtrl>().isOrderSuccess = false;
                    //this.transform.GetComponent<Image>().enabled = false;
                    this.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    //this.gameObject.SetActive(false);
                }
                //아이템프리팹이 삭제되지 않았을 경우
                if (itemPrefab != null) Destroy(itemPrefab);
            }
            else Destroy(itemPrefab);
        }
        Destroy(itemPrefab);
        itemPrefab = null;
    }

}
