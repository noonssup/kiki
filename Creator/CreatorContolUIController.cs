using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreatorContolUIController : MonoBehaviour
{
    public Animator anim;

    [Header("게임오브젝트")]
    public GameObject contentSetController;
    public GameObject contentSetButton;
    public GameObject contentOptionPrefab;
    public Transform[] contentTr;
    public ScrollRect scrollView;
    public Button broadcastButton;
    public Button[] tabButtons;
    public Image playerImage;
    public Sprite[] genderSprites;
    public Sprite[] avatarSprites;
    public Image contentBgImage;
    public Sprite[] contentBgSprites;
    public Image itemImage;
    public Sprite[] itemSprites;
    public Sprite[] secondItemSprites;
    public Image addImage;
    public Sprite[] addSprites;

    [Header("게임데이터")]
    public bool isReady = false;
    private bool isContentPanelOn = false;
    public string channelName;
    public int gender = 0;   //0==남, 1==여
    public int contentPage;
    public int itemId = 0;
    public List<Dictionary<string, object>> tableData;

    [Header("텍스트")]
    public TextMeshProUGUI textChannelName;
    public TextMeshProUGUI[] textContentItems;
    public TextMeshProUGUI textBroadcastButton;
    public TextMeshProUGUI[] textPopularityRate;


    private void Start()
    {
        anim = contentSetController.GetComponent<Animator>();
        textBroadcastButton = broadcastButton.GetComponentInChildren<TextMeshProUGUI>();
        tableData = CreatorGameManager.instance.tableData;
        ContentOptionSetup();
    }   


    public void GameSetup() //방송 준비
    {
        isReady = false;
        gender = CreatorGameManager.instance.gender;
        channelName = CreatorGameManager.instance.channelName;
        textChannelName.text = channelName;
        playerImage.sprite = genderSprites[gender];

        contentSetController.SetActive(true);
        ContentTabSetup(0);
        textContentItems[0].text = "";
        textContentItems[1].text = "";
        textContentItems[2].text = "";
        textBroadcastButton.text = "방송 준비";
    }

    void ContentTabSetup(int pageIndex)  //컨텐츠 설정 탭 선택
    {
        ColorBlock _colorBlock;
        Color _color;

        //활성/비활성탭 구분
        for (int i = 0; i < tabButtons.Length; i++)
        {
            _colorBlock = tabButtons[i].colors;
            ColorUtility.TryParseHtmlString("#2C3040", out _color);
            _colorBlock.normalColor = _color;
            tabButtons[i].colors = _colorBlock;
            contentTr[i].gameObject.SetActive(false);
        }

        _colorBlock = tabButtons[pageIndex].colors;
        ColorUtility.TryParseHtmlString("#6397ED", out _color);
        _colorBlock.highlightedColor = _color;
        _colorBlock.normalColor = _color;
        _colorBlock.selectedColor = _color;
        tabButtons[pageIndex].colors = _colorBlock;

        contentPage = pageIndex;
        contentTr[pageIndex].gameObject.SetActive(true);
        scrollView.content = contentTr[pageIndex].GetComponent<RectTransform>();
    }

    void ContentOptionSetup()  //방송 컨텐츠, 스타일, 아이템 버튼 생성
    {
        List<string> _optionName = new List<string>();
        List<string> _optionIDs = new List<string>();
        for (int i = 0; i < contentTr.Length; i++)
        {
            int _itemLength = 0;

            for (int t = 0; t < tableData.Count; t++)
            {
                if (tableData[t]["id"].ToString().FirstOrDefault().ToString() == (i + 1).ToString())
                {
                    _itemLength++;
                    _optionIDs.Add(tableData[t]["id"].ToString());
                    _optionName.Add(tableData[t]["name"].ToString());
                }
                
            }

            //방송 아이템 개수에 맞춰 버튼 생성
            for (int b = 0; b < _itemLength; b++)
            {
                GameObject _contentButton = Instantiate(contentOptionPrefab);
                _contentButton.transform.SetParent(contentTr[i].transform);
                _contentButton.transform.localScale = Vector3.one;
                CreatorContentSelectButton btnCtrl = _contentButton.GetComponent<CreatorContentSelectButton>();
                btnCtrl.controlUICtrl = this;


                btnCtrl.nameValue = _optionName[b];
                btnCtrl.id = int.Parse(_optionIDs[b]);
            }

            //버튼 순서 섞기
            contentTr[i].GetSiblingIndex();

            /*
            Transform.SetAsLastSibling
해당 오브젝트의 순위를 마지막으로 변경(가장 나중에 출력되므로 겹쳐졋을 경우 앞으로 나옵니다.)

Transform.SetAsFirstSibling
해당 오브젝트의 순위를 처음으로 변경(가장 처음 출력되므로 겹쳐졋을 경우 가려집니다.)

Transform.SetSiblingIndex(int nIndex)
nIndex를 매개변수를 넣어서 순위를 지정합니다.(0이 처음입니다.)

Transform.GetSiblingIndex()
해당 오브젝트의 순위를 얻어옵니다.

            */

            _optionIDs.RemoveRange(0,_optionIDs.Count);
            _optionName.RemoveRange(0, _optionName.Count);
        }
    }


    public void OnClickCloseContentSetController()  //컨텐츠컨트롤러 버튼 작동
    {
        GameObject _button = EventSystem.current.currentSelectedGameObject;

        switch (_button.name)
        {
            case "CloseButton":   //컨텐츠 선택 패널 닫기
                anim.SetBool("PanelOpen", false);
                anim.SetBool("PanelClose", true);
                isContentPanelOn = false;
                break;
            case "BroadcastButton":   //방송 준비, 방송 시작 버튼
                if (!isContentPanelOn)  //컨텐츠 선택 패널 열기
                {
                    anim.SetBool("PanelOpen", true);
                    anim.SetBool("PanelClose", false);
                    isContentPanelOn = true;
                }
                else if (isContentPanelOn)   //컨텐츠 선택 패널 닫기 (방송 준비가 된 경우 방송 시작)
                {
                    anim.SetBool("PanelOpen", false);
                    anim.SetBool("PanelClose", true);
                    isContentPanelOn = false;

                    if (isReady && CreatorGameManager.instance.isReady) //방송 시작
                    {
                        SoundManager.instance.PlayEffectSound("eff_Creator_ broadcastOn", 1f);
                        CreatorGameManager.instance.BroadcastStart();
                    }
                }
                break;
            case "ContentTab": 
                ContentTabSetup(0); 
                break;
            case "StyleTab": 
                ContentTabSetup(1); 
                break;
            case "ItemTab": 
                ContentTabSetup(2); 
                break;
        }
        SetBroadButtonText();
    }

    public void SetCreatorContent(int _id, string _itemName)  //컨텐츠 아이템 버튼을 누르면 실행
    {
        int itemIndex = int.Parse(_id.ToString().FirstOrDefault().ToString());
        itemId = _id;

        //선택한 컨텐츠 아이템의 이름 출력
        textContentItems[itemIndex - 1].text = _itemName;

        //선택한 컨텐츠 아이템의 이미지 출력
        if (itemIndex == 1)  //컨텐츠에 따른 배경 변경
        {
            switch (_id)
            {
                case 101: contentBgImage.sprite = contentBgSprites[0]; break;
                case 102: contentBgImage.sprite = contentBgSprites[1]; break;
                case 103: contentBgImage.sprite = contentBgSprites[1]; break;
                case 104: contentBgImage.sprite = contentBgSprites[3]; break;
                case 105: contentBgImage.sprite = contentBgSprites[2]; break;
                case 106: contentBgImage.sprite = contentBgSprites[0]; break;
            }
        }
        else if (itemIndex == 2)  //컨텐츠에 따른 아바타 변경
        {
            if (gender == 0) SetContentImage(playerImage, avatarSprites, "m_" + _id.ToString());
            else if (gender == 1) SetContentImage(playerImage, avatarSprites, "f_" + _id.ToString());
        }
        else if (itemIndex == 3)
        {
            //아이템에 따라 추가 이미지 할당
            addImage.enabled = true;
            switch (_id)
            {
                case 303: addImage.sprite = addSprites[0]; break;
                case 304: addImage.sprite = addSprites[0]; break;
                case 305: addImage.sprite = addSprites[1]; break;
                default: addImage.enabled = false; break;
            }

            //아이템에 따른 이미지 할당
            if (!itemImage.enabled) itemImage.enabled = true;
            SetContentImage(itemImage, itemSprites, "item_" + _id.ToString());
        }

        isReady = CreatorGameManager.instance.SetContent(_id);
        SetBroadButtonText();
    }

    void SetBroadButtonText()
    {
        if (isReady && isContentPanelOn) textBroadcastButton.text = "촬영 시작";
        else textBroadcastButton.text = "촬영 준비";
    }

    //컨텐츠, 아이템 등에 따른 각각의 이미지 할당
    void SetContentImage(Image _image, Sprite[] _sprite, string _name) 
    {
        foreach(Sprite sp in _sprite)
        {
            if (sp.name.Equals(_name))
            {
                _image.sprite = sp;
                break;
            }
        }
    }

    //방송 후반부 아이템 이미지 변경 처리
    public void SetSecondItemImage()
    {
        foreach (Sprite sp in secondItemSprites)
        {
            if (sp.name.Equals("item_"+itemId+"_01"))
            {
                itemImage.sprite = sp;
                break;
            }
        }
    }


    //흥미도, 인기도 설정
    public void PopularityValueSetup(int _interestValue, float _popularityValue)  
    {
        textPopularityRate[0].text = _interestValue.ToString() + "%";
        textPopularityRate[1].text =((int)(_popularityValue*100)).ToString() + "%";
    }

    public void OffAir(bool _value)  //방송 종료
    {
        broadcastButton.interactable = _value;
    }


}
