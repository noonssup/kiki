using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RescueItemPrefab : MonoBehaviour
{
    public string itemName;
    public Image itemImage;
    public string itemImagePath;
    public RescueItemSlot itemSlotCtrl;
    public RescueGameCtrl gameCtrl;
    public int rescueCount = 0;   //현재 정답 횟수
    public Sprite[] itemSprites;

    private void Start()
    {
        gameCtrl = FindObjectOfType<RescueGameCtrl>();
        ItemPrefabsSetting();
    }

    //private void OnEnable()
    //{
    //    ItemPrefabsSetting();
    //}

    void ItemPrefabsSetting()
    {
        rescueCount = gameCtrl.rescueCount;
        itemImage = this.transform.Find("Image").GetComponent<Image>();
        switch(itemImagePath)
        {
            case "item_001":
                itemImage.sprite = itemSprites[0];
                break;
            case "item_002":
                itemImage.sprite = itemSprites[1];
                break;
            case "item_015":
                itemImage.sprite = itemSprites[2];
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Color _color = this.transform.GetComponentInChildren<Image>().color;
        _color.a = 0.75f;
        this.transform.GetComponentInChildren<Image>().color = _color;
        if (collision.name == "BoardImage")
        {
            if (itemName == "item_001" && rescueCount==0)
            {
                //아이템슬롯 스크립트에 트리거 정보 전달
                //정답 여부, 충돌한 콜라이더의 트랜스폼 정보(콜라이더를 비활성화하기 위함)
                itemSlotCtrl.CheckCorrect(true, true, collision.transform);  //콜라이더 충돌 상태, 정답여부, 콜라이더의 정보
            }
            else itemSlotCtrl.CheckCorrect(true, false, collision.transform); //콜라이더 충돌 상태, 정답여부, 콜라이더의 정보
        }
        else if(collision.name == "TubeImage")
        {
            if (itemName == "item_002" && rescueCount == 1)
            {
                //아이템슬롯 스크립트에 트리거 정보 전달
                //정답 여부, 충돌한 콜라이더의 트랜스폼 정보(콜라이더를 비활성화하기 위함)
                itemSlotCtrl.CheckCorrect(true, true, collision.transform);  //콜라이더 충돌 상태, 정답여부, 콜라이더의 정보
            }
            else itemSlotCtrl.CheckCorrect(true, false, collision.transform); //콜라이더 충돌 상태, 정답여부, 콜라이더의 정보
        }
        else if (collision.name == "BeltTopImage" && rescueCount == 2 || collision.name == "BeltTopImage" && rescueCount == 3)
        {
            if (itemName == "item_015") // || itemName == "item_004")
            {
                //아이템슬롯 스크립트에 트리거 정보 전달
                //정답 여부, 충돌한 콜라이더의 트랜스폼 정보(콜라이더를 비활성화하기 위함)
                itemSlotCtrl.CheckCorrect(true, true, collision.transform);  //콜라이더 충돌 상태, 정답여부, 콜라이더의 정보
            }
            else itemSlotCtrl.CheckCorrect(true, false, collision.transform); //콜라이더 충돌 상태, 정답여부, 콜라이더의 정보
        }
        else if (collision.name == "BeltBottomImage" && rescueCount == 2 || collision.name == "BeltBottomImage" && rescueCount == 3)
        {
            if (itemName == "item_015")// || itemName == "item_004")
            {
                //아이템슬롯 스크립트에 트리거 정보 전달
                //정답 여부, 충돌한 콜라이더의 트랜스폼 정보(콜라이더를 비활성화하기 위함)
                itemSlotCtrl.CheckCorrect(true, true, collision.transform);  //콜라이더 충돌 상태, 정답여부, 콜라이더의 정보
            }
            else itemSlotCtrl.CheckCorrect(true, false, collision.transform); //콜라이더 충돌 상태, 정답여부, 콜라이더의 정보
        }
        else itemSlotCtrl.CheckCorrect(true, false, collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //아이템슬롯 스크립트에 트리거 정보 전달 (null)
        //정답아이템의 이미지를 반투명에서 투명으로 변경
        itemSlotCtrl.CheckCorrect(false, false, collision.transform);
        Color _color = this.transform.GetComponentInChildren<Image>().color;
        _color.a = 1f;
        this.transform.GetComponentInChildren<Image>().color = _color;
    }
}
