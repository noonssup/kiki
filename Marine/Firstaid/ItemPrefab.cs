using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPrefab : MonoBehaviour
{
    //아이템 이미지 이름
    //item_003 = 식염수
    //item_004 = 생수
    //item_005 = 탄산음료
    //item_006 = 온수
    //item_007 = 에너지드링크
    //item_008 = 에탄올
    //item_009 = 바닷물
    //item_010 = 나무젓가락
    //item_011 = 조개껍데기
    //item_012 = 플라스틱카드
    //item_013 = 핀셋
    //item_014 = 연고


    public string[] itemLevel1 = { "item_003", "item_004", "item_005", "item_006", "item_007", "item_008", "item_009" };
    public string[] itemLevel2 = { "item_010" , "item_011" , "item_012" , "item_013" };
    public string[] itemLevel3 = { "item_009", "item_006", "item_003", "item_014", "item_004", "item_008" };
    public FirstaidGameCtrl gameCtrl;

    public string itemName;
    public string itemImagePath;
    public GameObject itemImage;
    public Transform itemSlotTr;   //선택한 아이템 슬롯의 트랜스폼 정보
    public Transform collTr;       //아이템과 충돌한 오브젝트의 tr 정보

    public bool isCorrect;  //아이템의 정답 여부
    public int gameLevel;  //게임레벨
    public bool isCheckCorrect;

    public Sprite[] itemSprites;

    private void Start()
    {
        gameCtrl = FindObjectOfType<FirstaidGameCtrl>();
        gameLevel = gameCtrl.gameLevel;
        ItemInforInit();
    }

    void ItemInforInit()  //아이템 정보 초기화
    {
        itemImage = this.transform.Find("ItemImage").gameObject;
        switch(itemImagePath)
        {
            case "object_34": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[0]; break;
            case "object_33": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[1]; break;
            case "object_32": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[2]; break;
            case "object_35": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[3]; break;
            case "object_31": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[4]; break;
            case "object_30": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[5]; break;
            case "object_36": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[6]; break;
            case "object_41": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[7]; break;
            case "object_44": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[8]; break;
            case "object_45": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[9]; break;
            case "object_46": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[10]; break;
            case "object_42": itemImage.GetComponent<SpriteRenderer>().sprite = itemSprites[11]; break;
        }

        switch (this.itemName)
        {
            case "item_003": isCorrect = true; break; //식염수 (1단계)
            case "item_009": isCorrect = true; break; //바닷물 (1단계)
            case "item_010": isCorrect = true; ColliderValueSet(-0.7f, -1.1f, 0.3f, 0.3f); break; //나무젓가락 (2단계)
            case "item_011": isCorrect = true; ColliderValueSet(0.9f, -0.05f, 0.3f, 1.3f); break; //조개껍데기 (2단계)
            case "item_012": isCorrect = true; ColliderValueSet(0.7f, 0.05f, 0.25f, 2.2f); break; //플라스틱카드 (2단계)
            case "item_013": isCorrect = true; ColliderValueSet(-0.6f, -1f, 0.4f, 0.4f); break; //핀셋 (2단계)
            case "item_006":
                if (gameCtrl.gameLevel == 3) isCorrect = true;
                else isCorrect = false;
                break; //온수 (3단계)
            case "item_014": isCorrect = true; ColliderValueSet(-0.5f, -0.45f, 0.3f, 0.3f); break; //연고 (3단계)

            default: isCorrect = false; break;
        }
        isCheckCorrect = false;
        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, null, itemName);
    }

    void ColliderValueSet(float _offsetX, float _offsetY, float _sizeX, float _sizeY)
    {
        Vector2 _offset = new Vector2(_offsetX, _offsetY);
        Vector2 _size = new Vector2(_sizeX, _sizeY);
        this.transform.GetComponent<BoxCollider2D>().offset = _offset;
        this.transform.GetComponent<BoxCollider2D>().size = _size;
    }

    //젓가락 콜라이더 사이즈 / 좌표
    // size.x = 0.3f , size.y = 0.3f
    // offset.x = -0.5f, offset.y = -0.75f

    //핀셋 콜라이더 사이즈 / 좌표
    // size.x = 0.3f , size.y = 0.3f
    // offset.x = -0.4f, offset.y = -0.7f

    //카드 콜라이더 사이즈 / 좌표
    // size.x = 0.9f , size.y = 0.2f
    // offset.x = -0.01f, offset.y = -0.7f



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collTr == null) collTr = collision.transform;
        else return;

        if (collision.name == "ArmColliderLv1" && gameLevel == 1)
        {
            isCheckCorrect = true;
            itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);

        }
        else if (gameLevel == 2)
        {
            isCheckCorrect = true;
            Color _color2 = new Color(1f, 1f, 1f, 0.5f);
            if (this.itemName == "item_012") this.transform.GetComponentInChildren<SpriteRenderer>().color = _color2;
            //collision.transform.GetComponent<Image>().color = _color2;

            switch (collision.name)
            {
                //촉수
                case "ArmColliderLv2_0":
                    gameCtrl.jellyfishImages[0].GetComponent<Image>().color = _color2;
                    if (itemName == "item_010" || itemName == "item_013")
                    {
                        isCorrect = true;
                        itemSlotTr.GetComponent<ItemSlot>().itemAnimSpawnTr = collision.transform;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    else
                    {
                        isCorrect = false;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    break;
                case "ArmColliderLv2_1":
                    gameCtrl.jellyfishImages[1].GetComponent<Image>().color = _color2;
                    if (itemName == "item_010" || itemName == "item_013")
                    {
                        isCorrect = true;
                        itemSlotTr.GetComponent<ItemSlot>().itemAnimSpawnTr = collision.transform;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    else
                    {
                        isCorrect = false;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    break;
                case "ArmColliderLv2_2":
                    gameCtrl.jellyfishImages[2].GetComponent<Image>().color = _color2;
                    if (itemName == "item_010" || itemName == "item_013")
                    {
                        isCorrect = true;
                        itemSlotTr.GetComponent<ItemSlot>().itemAnimSpawnTr = collision.transform;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    else
                    {
                        isCorrect = false;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    break;

                //독침
                case "ArmColliderLv2_3":
                    gameCtrl.jellyfishImages[3].GetComponent<Image>().color = _color2;
                    if (itemName == "item_012")
                    {
                        isCorrect = true;
                        itemSlotTr.GetComponent<ItemSlot>().itemAnimSpawnTr = collision.transform;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    else
                    {
                        isCorrect = false;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    break;
                case "ArmColliderLv2_4":
                    gameCtrl.jellyfishImages[4].GetComponent<Image>().color = _color2;
                    if (itemName == "item_012")
                    {
                        isCorrect = true;
                        itemSlotTr.GetComponent<ItemSlot>().itemAnimSpawnTr = collision.transform;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    else
                    {
                        isCorrect = false;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    break;
                case "ArmColliderLv2_5":
                    gameCtrl.jellyfishImages[5].GetComponent<Image>().color = _color2;
                    if (itemName == "item_012")
                    {
                        isCorrect = true;
                        itemSlotTr.GetComponent<ItemSlot>().itemAnimSpawnTr = collision.transform;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    else
                    {
                        isCorrect = false;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    break;
                case "ArmColliderLv2_6":
                    gameCtrl.jellyfishImages[6].GetComponent<Image>().color = _color2;
                    if (itemName == "item_012")
                    {
                        isCorrect = true;
                        itemSlotTr.GetComponent<ItemSlot>().itemAnimSpawnTr = collision.transform;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    else
                    {
                        isCorrect = false;
                        itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
                    }
                    break;
            }
            
        }
        else if (gameLevel == 3)
        {
            isCheckCorrect = true;
            itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);

        }
    }
    

    private void OnTriggerStay2D(Collider2D collision)
    {
        //if(collision == null)
        //{
        //    Color _color2 = new Color(1f, 1f, 1f, 0.5f);
        //    collision.transform.GetComponentInChildren<SpriteRenderer>().color = _color2;
        //}
        //if (collision.transform == collTr)
        //{
        //    Color _color2 = new Color(1f, 1f, 1f, 0.5f);
        //    if (gameLevel == 2)
        //    {
        //        collision.transform.GetComponentInChildren<SpriteRenderer>().color = _color2;
        //    }
        //}
        //else
        //{
        //    Color _color2 = new Color(1f, 1f, 1f, 1f);

        //    collision.transform.GetComponentInChildren<SpriteRenderer>().color = _color2;

        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        collTr = null;

        Color _color2 = new Color(1f, 1f, 1f, 1f);

        if (gameLevel == 2 && collTr == null)
        {
            //collision.transform.GetComponentInChildren<SpriteRenderer>().color = _color2;

            for (int i = 0; i < gameCtrl.jellyfishImages.Length; i++)
            {
                if (collision.name == gameCtrl.jellyfishImages[i].name)
                {
                    gameCtrl.jellyfishImages[i].color = _color2;
                }
            }
        }

        if (collision.name == "ArmColliderLv1")
        {
            isCheckCorrect = false;
            itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
        }
        else if (collision.name == "ArmColliderLv2_0" || collision.name == "ArmColliderLv2_1" || collision.name == "ArmColliderLv2_2" || collision.name == "ArmColliderLv2_3" || collision.name == "ArmColliderLv2_4" || collision.name == "ArmColliderLv2_5" || collision.name == "ArmColliderLv2_6")
        {
            isCheckCorrect = false;
            itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
        }
        else if (collision.name == "ArmColliderLv3")
        {
            isCheckCorrect = false;
            itemSlotTr.GetComponent<ItemSlot>().CheckItemPrefabStatus(gameLevel, isCorrect, isCheckCorrect, collision.transform, itemName);
        }
        if (this.itemName == "item_012") this.transform.GetComponentInChildren<SpriteRenderer>().color = _color2;
    }

}
