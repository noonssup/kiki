using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoffeeDragItemPrefab : MonoBehaviour
{
    //바리스타 게임에서 드래그로 활성화된 아이템의 조작을 담당하는 스크립트

    public BaristaGameCtrl gameCtrl;
    public Transform guestTr=null;
    public string itemName;  //아이템의 이름
    public string coffeeName=null; //완성된 커피의 이름

    public DragItem dragItem = null;
    /*
    에스프레소 : espresso
    아메리카노 : americano
    아이스아메리카노 : iceamericano
    카페라떼 : cafelatte
    카푸치노 : cappuccino
     */

    [Header("이미지")]
    public Sprite img_job_barista_coffee01_off;
    public Sprite img_job_barista_coffee02_off;
    public Sprite img_job_barista_coffee03_off;

    private void Start()
    {
        gameCtrl = FindObjectOfType<BaristaGameCtrl>();
        itemName = this.transform.GetChild(0).transform.GetComponent<Image>().sprite.name;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        guestTr = null;
        coffeeName = null;
        switch (itemName)
        {
            case "img_job_barista_filter_off":  //포터필터
                if (collision.name == "CoffeeGrinder")
                {
                    gameCtrl.MakeEspresso1();
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_filter_on_02":  //원두가루가 정제된 포터필터
                if (collision.name == "TopImage0" || collision.name == "TopImage1")
                {
                    string _imageName = null;
                    switch (collision.name)
                    {
                        case "TopImage0": _imageName = "TOP0"; break;
                        case "TopImage1": _imageName = "TOP1"; break;

                    }
                    gameCtrl.MakeEspresso3(_imageName);
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee01_off":  //빈에스프레소잔을 완성된 커피 자리에 놓으면
                if (collision.name == "CompleteCoffee" && !collision.transform.GetChild(1).transform.GetComponent<Image>().enabled)
                {
                    collision.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee01_off;
                    collision.transform.GetChild(1).transform.GetComponent<Image>().SetNativeSize();
                    collision.transform.GetChild(1).transform.GetComponent<Image>().enabled = true;
                    Destroy(this.gameObject);
                }
                else if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee02_off":   //뜨거운 컵을 완성된 커피 자리에 놓으면,
                if (collision.name == "CompleteCoffee" && !collision.transform.GetChild(1).transform.GetComponent<Image>().enabled) 
                {
                    collision.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee02_off;
                    collision.transform.GetChild(1).transform.GetComponent<Image>().SetNativeSize();
                    collision.transform.GetChild(1).transform.GetComponent<Image>().enabled = true;
                    Destroy(this.gameObject);
                }
                else if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee03_off":  //빈 얼음컵 (완성된 커피 자리에 놓기)
                if (collision.name == "CompleteCoffee" && !collision.transform.GetChild(1).transform.GetComponent<Image>().enabled)
                {
                    collision.transform.GetChild(1).transform.GetComponent<Image>().sprite = img_job_barista_coffee03_off;
                    collision.transform.GetChild(1).transform.GetComponent<Image>().SetNativeSize();
                    collision.transform.GetChild(1).transform.GetComponent<Image>().enabled = true;
                    Destroy(this.gameObject);
                }
                else if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;

            //case "img_job_barista_coffee01_off001":  //완성된 에스프레소원액을...
            case "img_job_barista_shot_on":  //완성된 에스프레소원액을...
                if (collision.name == "CompleteCoffee")
                {
                    if (!collision.transform.GetChild(1).transform.GetComponent<Image>().enabled) return;
                    

                    if (collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee03_on_01") 
                        //에스프레소가 얼음이 담긴 컵에 닿을 경우
                    {
                        if (gameCtrl.iaState == BaristaGameCtrl.IceAmericano.ICE)
                        {
                            dragItem.transform.GetComponent<Image>().enabled = false;
                            gameCtrl.MakeIceAmericano("ICEESPRESSO", dragItem);
                            Destroy(this.gameObject);
                        }
                        else return;
                    }
                    else if (collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee03_off")  
                        //에스프레소가 빈 얼음컵에 닿을 경우
                    {
                        if (gameCtrl.iaState == BaristaGameCtrl.IceAmericano.EMPTY)
                        {
                            dragItem.transform.GetComponent<Image>().enabled = false;
                            gameCtrl.MakeIceAmericano("ESPRESSO", dragItem);
                            Destroy(this.gameObject);
                        }
                        else return;
                    }
                    else if (collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee02_off")  
                        //빈hotCup에 에스프레소를 갖다대기
                    {
                        dragItem.GetComponent<Image>().enabled = false;
                        gameCtrl.MakeHotAmericano(dragItem);
                        Destroy(this.gameObject);
                    }
                    else if (collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee02_on_05")  
                        //우유가 든 컵에 에스프레소를 갖다대기
                    {
                        dragItem.transform.GetComponent<Image>().enabled = false;
                        gameCtrl.MakeCafelatte("MILK", dragItem);
                        Destroy(this.gameObject);
                    }                    
                    else if (collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee01_off")  
                        //에스프레소컵에 에스프레소를 갖다대기
                    {
                        dragItem.transform.GetComponent<Image>().enabled = false;
                        gameCtrl.MakeEspresso(dragItem);
                        Destroy(this.gameObject);
                    }
                    else return;
                }
                else return;
                break;
            case "img_job_barista_scoop_on":  //얼음스쿱을 빈얼음컵에 갖다대기
                if (collision.name == "CompleteCoffee" && !collision.transform.GetChild(1).transform.GetComponent<Image>().enabled) return;

                if (collision.name == "CompleteCoffee" && collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee03_off"/* && gameCtrl.coffeeMakeStep == 3*/)
                {
                    gameCtrl.MakeIceAmericano("ICE", dragItem);
                    Destroy(this.gameObject);
                }     
                //에스프레소가 담긴 얼음컵에 얼음스쿱을 갖다대기
                else if (collision.name == "CompleteCoffee" && collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee03_on_03"/* && gameCtrl.coffeeMakeStep == 3*/)
                {
                    gameCtrl.MakeIceAmericano("ESPRESSOICE", dragItem);
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_milk_off":  //밀크저그
                if (collision.name == "CompleteCoffee" && !collision.transform.GetChild(1).transform.GetComponent<Image>().enabled) return;

                if (collision.name == "CompleteCoffee" && collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee02_on_01")
                {//아메리카노에 밀크저그가 닿을 경우
                    gameCtrl.MakeCafelatte("AMERICANO", dragItem);
                    Destroy(this.gameObject);
                }
                else if(collision.name == "CompleteCoffee" && collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee02_off")
                {//빈컵에 밀크저그가 닿을 경우
                    gameCtrl.MakeCafelatte("HOTCUP", dragItem);
                    Destroy(this.gameObject);
                }
                else return;
                break;
            //case "img_job_barista_spray_off":
            //    if (collision.name == "CompleteCoffee" && collision.transform.GetComponent<Image>().sprite.name == "object_02")
            //    {
            //        gameCtrl.MakeCappuccino1();
            //        Destroy(this.gameObject);
            //    }
            //    else return;
            //    break;
            case "img_job_barista_powder_off":  //시나몬
                if (collision.name == "CompleteCoffee" && collision.transform.GetChild(1).transform.GetComponent<Image>().sprite.name == "img_job_barista_coffee02_on_02")
                {
                    gameCtrl.MakeCappuccino2();
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee02_on_02": //완성된 카페라떼
                if (collision.tag == "NPC")
                {
                    coffeeName = "cafelatte";
                    guestTr = collision.transform;
                }
                else if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee01_on":   //완성된 에스프레소커피
                if (collision.tag == "NPC")
                {
                    coffeeName = "espresso"; //완성된 커피이름은 소문자로 대입할것
                    guestTr = collision.transform;
                }
                else if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee03_on_02":  //완성된 아이스아메리카노
                if (collision.tag == "NPC")
                {
                    coffeeName = "iceamericano";
                    guestTr = collision.transform;
                }
                else if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee02_on_01":  //완성된 뜨거운아메리카노
                if (collision.tag == "NPC")
                {
                    coffeeName = "americano";
                    guestTr = collision.transform;
                }
                else if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee02_on_03":  //완성된 카푸치노
                if (collision.tag == "NPC")
                {
                    coffeeName = "cappuccino";
                    guestTr = collision.transform;
                }
                else if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee03_on_01": //얼음이 담긴 아이스컵
                if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee03_on_03": //커피가 담긴 아이스컵
                if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
            case "img_job_barista_coffee02_on_05": //커피가 담긴 아이스컵
                if (collision.name == "TrashCan")  //쓰레기통에 갖다대면
                {
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().enabled = false;
                    dragItem.transform.GetChild(1).transform.GetComponent<Image>().sprite = null;
                    dragItem.GetComponent<DragItem>().itemPrefab = null;
                    Destroy(this.gameObject);
                }
                else return;
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        guestTr = null;
        coffeeName = null;
    }

}
