using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// �糭���������� �ǹ� ��Ʈ�� ��ũ��Ʈ
/// </summary>

public enum BuildingState
{
    BLANK = 0,  //�ǹ��̾��»���
    MAKE,       //�ǹ��Ǽ���
    BROKEN,     //�ǹ� �ر�
    BUILDING1,  //�ǹ��ϼ�
    BUILDING2,
    BUILDING3,
    BUILDING4,
    BUILDING5,
}

[System.Serializable]
public class BuildingData
{
    public float health;
    public float maxPreparation = 100;
    public int rainPreparation = 0;
    public int surgePreparation = 0;
    public int typhoonPreparation = 0;
    public int earthquakePreparation = 0;
    public int repairCost = 100;
    public int upgradeCost = 1000;
    public int rebuilingCost = 1000;
}


public class DisasterBuilding : MonoBehaviour
{
    [Header("������Ʈ")]
    public BuildingState state = BuildingState.BLANK;
    public BuildingData buildingData = new BuildingData();
    DisasterController controller;
    [SerializeField] Sprite[] buildingImageSprites;
    [SerializeField] Sprite[] selectBuildingImageSprites;
    [SerializeField] Sprite[] brokenBuildingSprite;
    public Image healthGaugeBar;
    Image selectCursorImage;
    Image buildingImage;
    Image buildIconImage;
    //Animator anim;

    [Header("���ӵ�����")]
    public string buildingName;
    public string buildingGrade;
    [SerializeField] int goodsRate = 1;
    [SerializeField] float goodsTimer = 1f;
    [SerializeField] float goodsCoolTime = 0.1f;
    [SerializeField] float damage = 0f;
    bool[] isTrigger;
    static int buildingIndex = 0; //�ǹ��̸� ����� �ε���
    public int buildingImageIndex = 4;
    List<Dictionary<string, object>> data;

    [Header("�ؽ�Ʈ")]
    TextMeshProUGUI textBuildingName;
    TextMeshProUGUI textBuildingGrade;

    private void Start()
    {
        buildingImageIndex = 4;
        selectCursorImage = transform.GetChild(0).GetComponent<Image>();
        selectCursorImage.enabled= false;
        buildingImage = transform.GetChild(1).GetComponent<Image>();
        //buildingImage.sprite = makeBuildingSprite;
        //buildingImage.enabled = false;
        //anim = transform.GetChild(1).GetComponent<Animator>();
        buildIconImage = transform.GetChild(3).GetComponent<Image>();
        buildIconImage.gameObject.SetActive(false);
        //healthGaugeBar = this.transform.GetChild(3).GetChild(0).GetChild(0).GetComponent<Image>();
        healthGaugeBar.transform.parent.gameObject.SetActive(false);
        textBuildingName = transform.GetChild(4).GetComponent<TextMeshProUGUI>();
        textBuildingName.text = "";
        buildingName = null;
        textBuildingGrade = transform.GetChild(5).GetComponent<TextMeshProUGUI>();

        controller = FindObjectOfType<DisasterController>();
        state = BuildingState.BLANK;
        isTrigger = new bool[4];
        buildingData.repairCost = 100;
        for(int i=0;i< isTrigger.Length; i++)
        {
            isTrigger[i] = false;
        }
        data = DisasterGameManager.instance.data;
        if (buildingImageIndex == 4) buildingImageIndex = Random.Range(0, 3);
        MakeBuilding();
    }

    void CreateBuildingName()   //�ǹ� �̸� ����
    {
        if(buildingName == null) {
            switch (buildingIndex)
            {
                case 0: buildingName = "ö���� ��"; break;
                case 1: buildingName = "�ҹ漭"; break;
                case 2: buildingName = "���׽��۸���"; break;
                case 3: buildingName = "�����н���"; break;
                case 4: buildingName = "ŰŰ�� ��"; break;
                case 5: buildingName = "å�о��ִ�å��"; break;
                case 6: buildingName = "������ġŲ�̴���"; break;
                case 7: buildingName = "��Ű�� ��"; break;
                case 8: buildingName = "�߾��ǻ���"; break;
                case 9: buildingName = "�ְ���PC��"; break;
                case 10: buildingName = "�ò�����ī��"; break;
                case 11: buildingName = "�����Ƚ��"; break;
                case 12: buildingName = "������"; break;
                case 13: buildingName = "���߹���������"; break;
                case 14: buildingName = "����� ��"; break;
                case 15: buildingName = "����Ѽ�Ź��"; break;
                case 16: buildingName = "�ոſ�«����"; break;
                case 17: buildingName = "�Ƹ�����̿��"; break;
            }
            buildingIndex++;
            if (buildingIndex > 17) buildingIndex = 0;
        }
        textBuildingName.text = buildingName;
        
    }

    public void CurSorSelect()  //���õ� �ǹ� �� ������ �ǹ��� Ŀ�� �̹��� ��Ȱ��ȭ
    {
        selectCursorImage.enabled = false;

        if (state >= BuildingState.BUILDING1) buildingImage.sprite = buildingImageSprites[buildingImageIndex];
        else if (state == BuildingState.BROKEN) { buildingImage.sprite = brokenBuildingSprite[buildingImageIndex]; buildIconImage.gameObject.SetActive(false); }
    }

    public void ClickBuildingButton()    //�ǹ��� Ŭ��(��ġ�ϸ�)
    {
        SoundManager.instance.PlayEffectSound("eff_Common_popup", 0.7f);
        this.selectCursorImage.enabled = true;

        DisasterGameManager.instance.SelectBuildingCursor(this.gameObject);
        if (state >= BuildingState.BUILDING1) { buildingImage.sprite = selectBuildingImageSprites[buildingImageIndex]; controller.SetData(this); }
        else if (state == BuildingState.BLANK) { controller.SetData(this); }
        else if (state == BuildingState.MAKE) controller.SetData(this);
        else if (state == BuildingState.BROKEN) { buildingImage.sprite = brokenBuildingSprite[buildingImageIndex]; buildIconImage.gameObject.SetActive(true); controller.SetData(this); }

    }

    //private void OnMouseDown()  //�ǹ� Ŭ�� (��ġ)
    //{
    //    Debug.Log(this.gameObject.name + " �ǹ� Ŭ��");
    //    this.selectCursorImage.enabled = true;
    //    buildingImage.sprite = selectBuildingImageSprites[buildingImageIndex];
    //    DisasterGameManager.instance.SelectBuildingCursor(this.gameObject);
    //    if (state >= BuildingState.BUILDING1) controller.SetData(this);
    //    //else if (state == BuildingState.BLANK) { controller.SetData(this); }
    //    //else if (state == BuildingState.MAKE) controller.SetData(this);
    //    else if (state == BuildingState.BROKEN) { controller.SetData(this); }
    //}

    #region �ǹ� ����
    public void MakeBuilding()   //�ǹ� ���� �� �Ǽ� (2023.02.08 ����� ����, ���� ���� �� �ڵ����� �ǹ� �Ǽ� ó��)
    {
        if (state == BuildingState.MAKE || DisasterGameManager.instance.isFailed) return;

        if(DisasterGameManager.instance.goods < 500)
        {
            NotenoughGoodsPopup();
            return;
        }
        else  DisasterGameManager.instance.goods = -500;

        state = BuildingState.MAKE;
        buildIconImage.gameObject.SetActive(false);
        buildingImage.enabled = true;
        //anim.SetTrigger("build");
        //StartCoroutine(BuildTime());
        buildingImage.transform.localPosition = Vector3.zero;
        buildingImage.sprite = buildingImageSprites[buildingImageIndex];
        state = BuildingState.BUILDING1;
        DisasterGameManager.instance.buildingCount++;

        DataInit();
    }

    public void RemakeBuilding()    //�ǹ� �����
    {
        state = BuildingState.MAKE;
        buildIconImage.gameObject.SetActive(false);
        buildingImage.enabled = true;
        //buildingImage.sprite = makeBuildingSprite;
        //anim.enabled = true;
        //anim.SetTrigger("build");
        //StartCoroutine(BuildTime());
        buildingImage.transform.localPosition = Vector3.zero;

        //����� �̹��� (������ �ǹ��� ������ �ǹ���...)
        buildingImage.sprite = buildingImageSprites[buildingImageIndex];

        state = BuildingState.BUILDING1;
        DisasterGameManager.instance.buildingCount++;

        DataInit();
    }

    void NotenoughGoodsPopup()  //�ܾ� ���� �˾�
    {
        //������ȭ�� �����Ͽ� �ǹ��� ���� �� �����ϴ�
        Debug.Log("�ܾ׺���");
    }

    public void BuildingUpgrade()    //�ǹ� ��ȭ
    {
        if (state >= (BuildingState)7)
        {
            state = (BuildingState)7;
            return;
        }
        state++;
        goodsCoolTime *= 0.5f;
        RenewalData();
    }

    //IEnumerator BuildTime()  //�Ǽ� �ð�
    //{
    //    float _timer = 5f;
    //    while (_timer > 0f)
    //    {
    //        _timer -= Time.deltaTime;
    //        yield return null;
    //    }

    //    //anim.enabled= false;
    //    buildingImage.transform.localPosition = Vector3.zero;
    //    buildingImage.sprite = buildingImageSprites[Random.Range(0, buildingImageSprites.Length)];
    //    state = BuildingState.BUILDING1;
    //    DisasterGameManager.instance.buildingCount++;

    //    DataInit();
    //}

    void DataInit()  //�ǹ��� �⺻ �ɷ�ġ
    {
        this.transform.GetComponent<BoxCollider2D>().enabled = true;
        CreateBuildingName();
        healthGaugeBar.transform.parent.gameObject.SetActive(true);
        RenewalData();
        //buildingData.maxPreparation = 100f;
        buildingData.health = buildingData.maxPreparation;
        healthGaugeBar.fillAmount = buildingData.health / buildingData.maxPreparation;
        buildingData.rainPreparation = 0;
        buildingData.surgePreparation = 0;
        buildingData.typhoonPreparation = 0;
        buildingData.earthquakePreparation = 0;
    }

    void RenewalData()  //�ǹ� ������ ����
    {
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i]["ID"].ToString() == ((int)state - 2).ToString())
            {
                buildingData.maxPreparation = int.Parse(data[i]["health"].ToString());
                buildingData.upgradeCost = int.Parse(data[i]["upgradecost"].ToString());
                buildingData.repairCost = int.Parse(data[i]["repaircost"].ToString());
                buildingGrade = data[i]["name_key"].ToString();
                break;
            }
        }

        switch (state)
        {
            case BuildingState.BUILDING1: textBuildingGrade.text = "Lv.1"; buildingData.rebuilingCost = buildingData.upgradeCost; break;
            case BuildingState.BUILDING2: textBuildingGrade.text = "Lv.2"; buildingData.rebuilingCost = buildingData.upgradeCost; break;
            case BuildingState.BUILDING3: textBuildingGrade.text = "Lv.3"; buildingData.rebuilingCost = buildingData.upgradeCost; break;
            case BuildingState.BUILDING4: textBuildingGrade.text = "Lv.4"; buildingData.rebuilingCost = buildingData.upgradeCost; break;
            case BuildingState.BUILDING5: textBuildingGrade.text = "MAX"; buildingData.rebuilingCost = 5000; break;
        }
        HealthGauge();
    }

    #endregion

    public void HealthGauge()  //�ǹ������� ������
    {
        healthGaugeBar.fillAmount = buildingData.health / buildingData.maxPreparation;
    }

    private void Update()
    {
        if(state>= BuildingState.BUILDING1) { MakeGoods(); }
    }

    void MakeGoods()  //��ȭ����
    {
        if (!DisasterGameManager.instance.isGamePlay) return;
        goodsTimer -= Time.deltaTime;
        if (goodsTimer <= 0f)
        {
            DisasterGameManager.instance.goods = goodsRate;
            goodsTimer = goodsCoolTime; //�ʱ� 0.1f --> 0.05f --> 0.025f --> 0.125f....
            if (buildingData.health < buildingData.maxPreparation) goodsTimer = 0.2f;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (state < BuildingState.BUILDING1) return;
        string name = collision.name;
        DisasterObject disaster = collision.GetComponent<DisasterObject>();
        int defenceLevel = 4;
        damage = disaster.disasterData.disasterPower;

        switch (name)
        {
            case "Rain":
                defenceLevel -= buildingData.rainPreparation; buildingData.rainPreparation = 0; break;
            case "Surge":
                defenceLevel -= buildingData.surgePreparation; buildingData.surgePreparation = 0; break;
            case "Typhoon":
                defenceLevel -= buildingData.typhoonPreparation; buildingData.typhoonPreparation = 0; break;
            case "Earthquake":
                defenceLevel -= buildingData.earthquakePreparation; buildingData.earthquakePreparation = 0; break;
        }
        BuildingDamage(defenceLevel);
    }

    void BuildingDamage(int defenceLevel)   //�ǹ� ����
    {

        switch (defenceLevel)
        {
            case 4: damage *= 1f; break;
            case 3: damage *= 0.75f; break;
            case 2: damage *= 0.5f; break;
            case 1: damage *= 0.25f; break;
            case 0: damage = 0f; break;
        }

        buildingData.health -= damage;
        healthGaugeBar.fillAmount = buildingData.health / buildingData.maxPreparation;
        if (buildingData.health <= 0f)
        {
            buildingData.health = 0f;
            state = BuildingState.BROKEN;
            DisasterGameManager.instance.buildingCount--;

            //�ǹ� �ر� �̹��� (�Ǽ��� �̹����� ������ �ǹ���...)
            buildingImage.sprite = brokenBuildingSprite[buildingImageIndex];

            //this.transform.GetComponent<BoxCollider2D>().enabled = false;
            if (DisasterGameManager.instance.buildingCount <= 0)
            {
                Debug.Log("���ӿ���");
                DisasterGameManager.instance.isFailed = true;
            }
        }

        if (controller.building == this) controller.PrintData();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        string name = collision.name;
        switch (name)
        {
            case "Rain": isTrigger[0] = false;  break;
            case "Surge": isTrigger[1] = false;  break;
            case "Typhoon": isTrigger[2] = false;  break;
            case "Earthquake": isTrigger[3] = false;  break;
        }
    }
}
