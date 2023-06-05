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

    [Header("���ӿ�����Ʈ")]
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

    [Header("���ӵ�����")]
    public bool isReady = false;
    private bool isContentPanelOn = false;
    public string channelName;
    public int gender = 0;   //0==��, 1==��
    public int contentPage;
    public int itemId = 0;
    public List<Dictionary<string, object>> tableData;

    [Header("�ؽ�Ʈ")]
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


    public void GameSetup() //��� �غ�
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
        textBroadcastButton.text = "��� �غ�";
    }

    void ContentTabSetup(int pageIndex)  //������ ���� �� ����
    {
        ColorBlock _colorBlock;
        Color _color;

        //Ȱ��/��Ȱ���� ����
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

    void ContentOptionSetup()  //��� ������, ��Ÿ��, ������ ��ư ����
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

            //��� ������ ������ ���� ��ư ����
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

            //��ư ���� ����
            contentTr[i].GetSiblingIndex();

            /*
            Transform.SetAsLastSibling
�ش� ������Ʈ�� ������ ���������� ����(���� ���߿� ��µǹǷ� ���Ġ��� ��� ������ ���ɴϴ�.)

Transform.SetAsFirstSibling
�ش� ������Ʈ�� ������ ó������ ����(���� ó�� ��µǹǷ� ���Ġ��� ��� �������ϴ�.)

Transform.SetSiblingIndex(int nIndex)
nIndex�� �Ű������� �־ ������ �����մϴ�.(0�� ó���Դϴ�.)

Transform.GetSiblingIndex()
�ش� ������Ʈ�� ������ ���ɴϴ�.

            */

            _optionIDs.RemoveRange(0,_optionIDs.Count);
            _optionName.RemoveRange(0, _optionName.Count);
        }
    }


    public void OnClickCloseContentSetController()  //��������Ʈ�ѷ� ��ư �۵�
    {
        GameObject _button = EventSystem.current.currentSelectedGameObject;

        switch (_button.name)
        {
            case "CloseButton":   //������ ���� �г� �ݱ�
                anim.SetBool("PanelOpen", false);
                anim.SetBool("PanelClose", true);
                isContentPanelOn = false;
                break;
            case "BroadcastButton":   //��� �غ�, ��� ���� ��ư
                if (!isContentPanelOn)  //������ ���� �г� ����
                {
                    anim.SetBool("PanelOpen", true);
                    anim.SetBool("PanelClose", false);
                    isContentPanelOn = true;
                }
                else if (isContentPanelOn)   //������ ���� �г� �ݱ� (��� �غ� �� ��� ��� ����)
                {
                    anim.SetBool("PanelOpen", false);
                    anim.SetBool("PanelClose", true);
                    isContentPanelOn = false;

                    if (isReady && CreatorGameManager.instance.isReady) //��� ����
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

    public void SetCreatorContent(int _id, string _itemName)  //������ ������ ��ư�� ������ ����
    {
        int itemIndex = int.Parse(_id.ToString().FirstOrDefault().ToString());
        itemId = _id;

        //������ ������ �������� �̸� ���
        textContentItems[itemIndex - 1].text = _itemName;

        //������ ������ �������� �̹��� ���
        if (itemIndex == 1)  //�������� ���� ��� ����
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
        else if (itemIndex == 2)  //�������� ���� �ƹ�Ÿ ����
        {
            if (gender == 0) SetContentImage(playerImage, avatarSprites, "m_" + _id.ToString());
            else if (gender == 1) SetContentImage(playerImage, avatarSprites, "f_" + _id.ToString());
        }
        else if (itemIndex == 3)
        {
            //�����ۿ� ���� �߰� �̹��� �Ҵ�
            addImage.enabled = true;
            switch (_id)
            {
                case 303: addImage.sprite = addSprites[0]; break;
                case 304: addImage.sprite = addSprites[0]; break;
                case 305: addImage.sprite = addSprites[1]; break;
                default: addImage.enabled = false; break;
            }

            //�����ۿ� ���� �̹��� �Ҵ�
            if (!itemImage.enabled) itemImage.enabled = true;
            SetContentImage(itemImage, itemSprites, "item_" + _id.ToString());
        }

        isReady = CreatorGameManager.instance.SetContent(_id);
        SetBroadButtonText();
    }

    void SetBroadButtonText()
    {
        if (isReady && isContentPanelOn) textBroadcastButton.text = "�Կ� ����";
        else textBroadcastButton.text = "�Կ� �غ�";
    }

    //������, ������ � ���� ������ �̹��� �Ҵ�
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

    //��� �Ĺݺ� ������ �̹��� ���� ó��
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


    //��̵�, �α⵵ ����
    public void PopularityValueSetup(int _interestValue, float _popularityValue)  
    {
        textPopularityRate[0].text = _interestValue.ToString() + "%";
        textPopularityRate[1].text =((int)(_popularityValue*100)).ToString() + "%";
    }

    public void OffAir(bool _value)  //��� ����
    {
        broadcastButton.interactable = _value;
    }


}
