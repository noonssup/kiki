using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ExtObjectEventController;

[System.Serializable]
public class PrinterUIController : MonoBehaviour
{
    [Header("OrderPanel")]
    public GameObject orderPanel;
    public PrinterGameState gameState;
    public TextMeshProUGUI textStatus;
    public TextMeshProUGUI textProgress;
    public int progressRate = 0;
    public int npcIndex = 0;
    public Image npcImage;
    public Sprite[] npcSprites;
    public string strOrder;
    public TextMeshProUGUI textOrder;
    public int orderCurrectCount = 0;

    [Header("PrinterControlPanel")]
    public PrinterData printerData;
    public PrinterData orderData;
    public GameObject printerControlPanel;
    public GameObject[] tabs;
    public GameObject itemButton;
    public Transform[] printerContentTr;
    public Button startButton;
    public TextMeshProUGUI[] textPrintData;
    public Sprite[] itemButtonSprites;
    public bool[] isDatas = new bool[3];
    public bool isItemButtonCreate = false;

    [Header("PrinterMachine")]
    public PrinterMachineController machineCtrl;

    [Header("MenuUI")]
    public GameObject pauseMenuPanel;
    public GameObject helpPanel;
    public GameObject resultPanel;
    public GameObject checkResultPanel;

    [Header("중간결과창")]
    public TextMeshProUGUI textCheckDays;
    public TextMeshProUGUI textStatusValue1;
    public TextMeshProUGUI textStatusValue2;


    public List<Dictionary<string, object>> itemData;
    public List<Dictionary<string, object>> npcData;


    public void UIpanelInit() //UI패널 초기화
    {
        machineCtrl.MachineSetup();
        textStatus.text = "대기중";
        progressRate = 0;
        textProgress.text = progressRate.ToString() + "%";
        textOrder.text = string.Empty;
        gameState = PrinterGameManager.instance.gameState;
        itemData = PrinterGameManager.instance.itemData;
        npcData = PrinterGameManager.instance.npcData;
        printerData = DataSetup(printerData);
        orderData = DataSetup(orderData);
        orderCurrectCount = 0;

        //아이템 버튼 생성
        if (!isItemButtonCreate)
        {
            for (int i = 0; i < itemData.Count; i++)
            {
                int _id = int.Parse(itemData[i]["id"].ToString().Substring(0, 1));
                int _itemId = int.Parse(itemData[i]["id"].ToString());
                string _itemName = itemData[i]["name"].ToString();
                PrinterItemButton _btn = Instantiate(itemButton, printerContentTr[_id - 1]).GetComponent<PrinterItemButton>();
                _btn.ButtonTypeSetup(_id);
                _btn.ButtonInformationSetup(_itemId, _itemName, this);
                _btn.transform.localScale = Vector3.one;
                isItemButtonCreate = true;
            }
        }

        for(int i = 0; i < textPrintData.Length; i++)
        {
            TabPrinterDataTextSetup(i, "-");
            isDatas[i] = false;
        }

        TabSelect(0);
        ClientUISetup(false);
        OnOffPrinterControlPanel(false);
    }

    PrinterData DataSetup(PrinterData _data)  //프린터 주문 정보 초기화
    {
        _data = new PrinterData();
        _data.shapeIndex = 0;
        _data.shapeItemName = string.Empty;
        _data.materialIndex = 0;
        _data.materialItemName = string.Empty;
        _data.afterIndex = 0;
        _data.afterItemName = string.Empty;

        return _data;
    }

    public void ClientSetup() //NPC 이미지, 주문 정보 초기화
    {
        int _index = Random.Range(1, npcData.Count + 1);
        for (int i = 0; i < npcData.Count; i++)
        {
            int _id = int.Parse(npcData[i]["id"].ToString());
            if (_id == _index)
            {
                int randomNpc = Random.Range(0, npcSprites.Length);
                npcImage.sprite = npcSprites[randomNpc];
                strOrder = npcData[i]["order"].ToString();
                textOrder.text = "";
                orderData.shapeIndex = int.Parse(npcData[i]["shape"].ToString());
                orderData.materialIndex = int.Parse(npcData[i]["matl"].ToString());
                orderData.afterIndex = int.Parse(npcData[i]["after"].ToString());
                break;
            }
        }

        for (int i = 0; i < itemData.Count; i++)
        {
            if(orderData.shapeIndex == int.Parse(itemData[i]["id"].ToString()))
            {
                orderData.shapeItemName = itemData[i]["name"].ToString();
            }
            else if(orderData.materialIndex == int.Parse(itemData[i]["id"].ToString()))
            {
                orderData.materialItemName = itemData[i]["name"].ToString();
            }
            else if (orderData.afterIndex == int.Parse(itemData[i]["id"].ToString()))
            {
                orderData.afterItemName = itemData[i]["name"].ToString();
                break;
            }
        }
        ClientUISetup(true);
    }


    void ClientUISetup(bool _value)  //고객 이미지, 주문 내역 활성화, 프린터 조작 패널UI 활성화
    {
        startButton.interactable = !_value;
        npcImage.enabled = _value;
        textOrder.enabled = _value;
        OnOffPrinterControlPanel(_value);
        StartCoroutine(OrderTextPrint());
    }


    IEnumerator OrderTextPrint()  //주문정보 텍스트 출력
    {
        WaitForSeconds wfs = new WaitForSeconds(0.01f);
        string _orderText = string.Empty;
        for(int i = 0; i < strOrder.Length; i++)
        {
            _orderText += strOrder[i];
            textOrder.text = _orderText;
            yield return wfs;
        }
    }
    

    public void TabSelect(int _index)  //탭 선택
    {
        for (int i=0;i<tabs.Length;i++)
        {
            TabSetup(i, "#2C3040", "#FFFFFF");
            printerContentTr[i].gameObject.SetActive(false);
        }

        TabSetup(_index, "#6397ED", "#FFFFFF");
        printerContentTr[_index].gameObject.SetActive(true);
    }

    void TabSetup(int _index, string _colorValue1, string _colorValue2) //탭 컬러 설정
    {
        Color _color;
        Image btnImage;
        TextMeshProUGUI text;

        btnImage = tabs[_index].transform.GetComponent<Image>();
        _color = btnImage.color;
        ColorUtility.TryParseHtmlString(_colorValue1, out _color);
        btnImage.color = _color;

        ColorUtility.TryParseHtmlString(_colorValue2, out _color);
        for(int i=0;i<2;i++)
        {
            text = tabs[_index].transform.GetChild(i).GetComponent<TextMeshProUGUI>();
            text.color = _color;
        }
    }

    public void TabPrinterDataTextSetup(int _index, string _str)  //프린터 형태/재료/후처리 텍스트
    {
        textPrintData[_index].text = _str;
    }

    public void OnOffPrinterControlPanel(bool _value)  //프린터컨트롤 패널 열고 닫기
    {
        printerControlPanel.SetActive(_value);
    }


    public void SetTabsInfor(int _index, int _itemId, string _itemName)  //탭의 아이템이름에 선택한 아이템의 텍스트 표시 (아이템 버튼 클릭 시)
    {
        _itemName = _itemName.Replace('\n', ' ');
        tabs[_index].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = _itemName.ToString();
        switch (_index)
        {
            case 0: 
                printerData.shapeIndex = _itemId;
                printerData.shapeItemName = _itemName;
                foreach (Sprite s in itemButtonSprites)
                {
                    if("item_"+_itemId == s.name)
                    {
                        machineCtrl.productImage.sprite = s; break;
                    }
                }
                break;
            case 1: printerData.materialIndex = _itemId; printerData.materialItemName = _itemName; break;
            case 2: printerData.afterIndex = _itemId; printerData.afterItemName = _itemName; break;
        }

        if (printerData.shapeIndex > 0)
        {
            isDatas[0] = true;
        }
        else isDatas[0] = false;

        if (printerData.materialIndex > 0)
        {
            isDatas[1] = true;
        }
        else isDatas[1] = false;

        if (printerData.afterIndex > 0)
        {
            isDatas[2] = true;
        }
        else isDatas[2] = false;

        if (isDatas[0] && isDatas[1] && isDatas[2]) 
        {
            startButton.interactable = true;
            PrinterGameManager.instance.ReadyGame(true);
        }
        else
        {
            startButton.interactable = false;
            PrinterGameManager.instance.ReadyGame(false);
        }
    }


    public void PrinterWorking(int _index)  //프린터 작업 중
    {
        switch (_index)
        {
            case 0: textStatus.text = "제작중"; break;
            case 1: textStatus.text = "대기중"; CheckResult(); break;
        }
        

    }

    public void ProgressCheck(int _progressValue)  //프린터 작업 현황
    {
        progressRate = _progressValue;
        textProgress.text = _progressValue.ToString()+"%";
    }


    public void CheckResult()  //중간 체크
    {
        OrderCheck();

        //checkResultPanel.SetActive(true);
        int basePoint = 1000;
        int count = machineCtrl.overHeatCount;
        float mul = 0f;

        switch (count)
        {
            case 0: mul = 1f; break;
            case 1: mul = 0.75f; break;
            case 2: mul = 0.5f; break;
            case 3: mul = 0.25f; break;
            default: mul = 0.1f; break;
        }

        float total = basePoint * orderCurrectCount * mul;
        textOrder.text = "획득 점수: " + total.ToString("n0");
        StartCoroutine(AddPoint((int)total));
        StartCoroutine(PrinterGameManager.instance.AddPoint((int)total));
    }

    IEnumerator AddPoint(int _point)  //점수 획득
    {
        yield return new WaitForSeconds(3);
        while(_point > 0)
        {
            _point-=10;
            if(_point < 0)
            {
                _point = 0;
            }
            textOrder.text = "획득 점수: " + _point.ToString("n0") + "\n" +
              "주문 물품\n" + "형태: " + orderData.shapeItemName.Replace('\n', ' ') + " / 제품: " + printerData.shapeItemName.Replace('\n', ' ') +
              "\n재료: " + orderData.materialItemName.Replace('\n', ' ') + " / 제품: " + printerData.materialItemName.Replace('\n', ' ') +
              "\n후처리: " + orderData.afterItemName.Replace('\n', ' ') + " / 제품: " + printerData.afterItemName.Replace('\n', ' ');
            yield return null;
        }
    }

    void OrderCheck()   //주문 내용 확인
    {
        if(orderData.shapeIndex == printerData.shapeIndex)
        {
            orderCurrectCount++;
        }
        
        if (orderData.materialIndex == printerData.materialIndex)
        {
            orderCurrectCount++;
        }
        
        if (orderData.afterIndex == printerData.afterIndex)
        {
            orderCurrectCount++;
        }
    }

    public void GameSet()  //게임셋
    {
        resultPanel.SetActive(true);
    }


}
