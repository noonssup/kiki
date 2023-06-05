using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 재난관리전문가 건물 컨트롤 스크립트
/// </summary>

public enum BuildingState
{
    BLANK = 0,  //건물이없는상태
    MAKE,       //건물건설중
    BROKEN,     //건물 붕괴
    BUILDING1,  //건물완성
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
    [Header("오브젝트")]
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

    [Header("게임데이터")]
    public string buildingName;
    public string buildingGrade;
    [SerializeField] int goodsRate = 1;
    [SerializeField] float goodsTimer = 1f;
    [SerializeField] float goodsCoolTime = 0.1f;
    [SerializeField] float damage = 0f;
    bool[] isTrigger;
    static int buildingIndex = 0; //건물이름 짓기용 인덱스
    public int buildingImageIndex = 4;
    List<Dictionary<string, object>> data;

    [Header("텍스트")]
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

    void CreateBuildingName()   //건물 이름 설정
    {
        if(buildingName == null) {
            switch (buildingIndex)
            {
                case 0: buildingName = "철수네 집"; break;
                case 1: buildingName = "소방서"; break;
                case 2: buildingName = "동네슈퍼마켓"; break;
                case 3: buildingName = "만나분식집"; break;
                case 4: buildingName = "키키네 집"; break;
                case 5: buildingName = "책읽어주는책방"; break;
                case 6: buildingName = "오늘은치킨이닭집"; break;
                case 7: buildingName = "포키네 집"; break;
                case 8: buildingName = "추억의빵집"; break;
                case 9: buildingName = "최고사양PC방"; break;
                case 10: buildingName = "시끌벅적카페"; break;
                case 11: buildingName = "막썰어횟집"; break;
                case 12: buildingName = "경찰서"; break;
                case 13: buildingName = "연중무휴편의점"; break;
                case 14: buildingName = "영희네 집"; break;
                case 15: buildingName = "깔끔한세탁소"; break;
                case 16: buildingName = "왕매워짬뽕집"; break;
                case 17: buildingName = "아리따운미용실"; break;
            }
            buildingIndex++;
            if (buildingIndex > 17) buildingIndex = 0;
        }
        textBuildingName.text = buildingName;
        
    }

    public void CurSorSelect()  //선택된 건물 외 나머지 건물의 커서 이미지 비활성화
    {
        selectCursorImage.enabled = false;

        if (state >= BuildingState.BUILDING1) buildingImage.sprite = buildingImageSprites[buildingImageIndex];
        else if (state == BuildingState.BROKEN) { buildingImage.sprite = brokenBuildingSprite[buildingImageIndex]; buildIconImage.gameObject.SetActive(false); }
    }

    public void ClickBuildingButton()    //건물을 클릭(터치하면)
    {
        SoundManager.instance.PlayEffectSound("eff_Common_popup", 0.7f);
        this.selectCursorImage.enabled = true;

        DisasterGameManager.instance.SelectBuildingCursor(this.gameObject);
        if (state >= BuildingState.BUILDING1) { buildingImage.sprite = selectBuildingImageSprites[buildingImageIndex]; controller.SetData(this); }
        else if (state == BuildingState.BLANK) { controller.SetData(this); }
        else if (state == BuildingState.MAKE) controller.SetData(this);
        else if (state == BuildingState.BROKEN) { buildingImage.sprite = brokenBuildingSprite[buildingImageIndex]; buildIconImage.gameObject.SetActive(true); controller.SetData(this); }

    }

    //private void OnMouseDown()  //건물 클릭 (터치)
    //{
    //    Debug.Log(this.gameObject.name + " 건물 클릭");
    //    this.selectCursorImage.enabled = true;
    //    buildingImage.sprite = selectBuildingImageSprites[buildingImageIndex];
    //    DisasterGameManager.instance.SelectBuildingCursor(this.gameObject);
    //    if (state >= BuildingState.BUILDING1) controller.SetData(this);
    //    //else if (state == BuildingState.BLANK) { controller.SetData(this); }
    //    //else if (state == BuildingState.MAKE) controller.SetData(this);
    //    else if (state == BuildingState.BROKEN) { controller.SetData(this); }
    //}

    #region 건물 생성
    public void MakeBuilding()   //건물 선택 후 건설 (2023.02.08 현재는 변경, 게임 시작 시 자동으로 건물 건설 처리)
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

    public void RemakeBuilding()    //건물 재건축
    {
        state = BuildingState.MAKE;
        buildIconImage.gameObject.SetActive(false);
        buildingImage.enabled = true;
        //buildingImage.sprite = makeBuildingSprite;
        //anim.enabled = true;
        //anim.SetTrigger("build");
        //StartCoroutine(BuildTime());
        buildingImage.transform.localPosition = Vector3.zero;

        //재건축 이미지 (원래의 건물과 동일한 건물로...)
        buildingImage.sprite = buildingImageSprites[buildingImageIndex];

        state = BuildingState.BUILDING1;
        DisasterGameManager.instance.buildingCount++;

        DataInit();
    }

    void NotenoughGoodsPopup()  //잔액 부족 팝업
    {
        //보유재화가 부족하여 건물을 지을 수 없습니다
        Debug.Log("잔액부족");
    }

    public void BuildingUpgrade()    //건물 강화
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

    //IEnumerator BuildTime()  //건설 시간
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

    void DataInit()  //건물의 기본 능력치
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

    void RenewalData()  //건물 데이터 갱신
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

    public void HealthGauge()  //건물내구도 게이지
    {
        healthGaugeBar.fillAmount = buildingData.health / buildingData.maxPreparation;
    }

    private void Update()
    {
        if(state>= BuildingState.BUILDING1) { MakeGoods(); }
    }

    void MakeGoods()  //재화생산
    {
        if (!DisasterGameManager.instance.isGamePlay) return;
        goodsTimer -= Time.deltaTime;
        if (goodsTimer <= 0f)
        {
            DisasterGameManager.instance.goods = goodsRate;
            goodsTimer = goodsCoolTime; //초기 0.1f --> 0.05f --> 0.025f --> 0.125f....
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

    void BuildingDamage(int defenceLevel)   //건물 피해
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

            //건물 붕괴 이미지 (건설된 이미지와 동일한 건물로...)
            buildingImage.sprite = brokenBuildingSprite[buildingImageIndex];

            //this.transform.GetComponent<BoxCollider2D>().enabled = false;
            if (DisasterGameManager.instance.buildingCount <= 0)
            {
                Debug.Log("게임오버");
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
