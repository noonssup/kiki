using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotItemSlotController : MonoBehaviour
{
    [Header("스크립트컨트롤러")]
    [SerializeField] RobotBattleSceneController gameCtrl;

    [Header("컴포넌트")]
    [SerializeField] GameObject[] itemPrefabs;
    [SerializeField] GameObject itemGo = null;
    [SerializeField] Image imageItemImage;

    [Header("게임데이터")]
    [SerializeField] int totalArmorCount = 0;
    [SerializeField] int totalWeaponCount = 0;
    [SerializeField] int totalWheelCount = 0;
    [SerializeField] int partsCount = 0;  //부품 카운트 (아이템슬롯 클릭 시 해당하는 부품카운트를 할당)
    [SerializeField] string soundName = "eff_Robot_partsOver";


    [Header("텍스트")]
    [SerializeField] TMP_Text textItemName;

    //[SerializeField] AudioSource sfxPlayer;
    //[SerializeField] AudioClip notEnoughSound;

    private void Start()
    {
        gameCtrl = FindObjectOfType<RobotBattleSceneController>();
        itemPrefabs = gameCtrl.itemPrefabs;
        ItemNameSetup();
    }

    void ItemNameSetup()   //아이템명 셋업
    {
        switch (imageItemImage.sprite.name)
        {
            case "item_robot_04": itemGo = itemPrefabs[0]; partsCount = 1; break;
            case "item_robot_05": itemGo = itemPrefabs[1]; partsCount = 2; break;
            case "item_robot_06": itemGo = itemPrefabs[2]; partsCount = 3; break;
            case "item_robot_07": itemGo = itemPrefabs[3]; partsCount = 1; break;
            case "item_robot_08": itemGo = itemPrefabs[4]; partsCount = 2; break;
            case "item_robot_09": itemGo = itemPrefabs[5]; partsCount = 1; break;
            case "item_robot_10": itemGo = itemPrefabs[6]; partsCount = 1; break;
            case "item_robot_11": itemGo = itemPrefabs[7]; partsCount = 1; break;
        }

    }

    public void GenerateItems()  //버튼을 눌러 아이템 생성 (인벤토리의 아이템슬롯 버튼에 할당)
    {
        foreach(Transform _posTr in gameCtrl.itemPosTr)
        {
            if(_posTr.childCount == 0)
            {
                GameObject _item = CheckGenerateCondition();
                if (_item != null)
                {
                    _item.transform.position = _posTr.position;//, Quaternion.identity);
                    _item.transform.SetParent(_posTr);
                    gameCtrl.itemListTr.Add(_item.transform);
                    break;
                }
                else
                {
                    break;
                }
            }
        }
    }

    GameObject CheckGenerateCondition()   //아이템 생성 조건 처리
    {
        GameObject _item = null;
       if (itemGo != null)
        {
            if (itemGo.name == itemPrefabs[0].name || itemGo.name == itemPrefabs[1].name || itemGo.name == itemPrefabs[2].name)
            {
                gameCtrl._armorCount = partsCount;
                if (gameCtrl.armorCount <= gameCtrl.maxArmorCount)
                {
                    _item = Instantiate(itemGo);
                }
                else
                {
                    gameCtrl._armorCount = -partsCount;
                    SoundManager.instance.PlayEffectSound(soundName, 1f);
                }
            }
            else if(itemGo.name == itemPrefabs[3].name || itemGo.name == itemPrefabs[4].name)
            {
                gameCtrl._wheelCount = partsCount;
                if (gameCtrl.wheelCount <= gameCtrl.maxWheelCount)
                {
                    _item = Instantiate(itemGo);
                }
                else
                {
                    gameCtrl._wheelCount = -partsCount;
                    SoundManager.instance.PlayEffectSound(soundName, 1f);
                }
            }
            else if(itemGo.name == itemPrefabs[5].name || itemGo.name == itemPrefabs[6].name || itemGo.name == itemPrefabs[7].name)
            {
                gameCtrl._weaponCount = partsCount;
                if (gameCtrl.weaponCount <= gameCtrl.maxWeaponCount)
                {
                    _item = Instantiate(itemGo);
                }
                else
                {
                    gameCtrl._weaponCount = -partsCount;
                    SoundManager.instance.PlayEffectSound(soundName, 1f);
                }
            }
        }

        return _item;
    }
}
