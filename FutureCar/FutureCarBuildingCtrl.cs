using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FutureCarBuildingCtrl : MonoBehaviour
{
    [Header("스크립트컨트롤러")]
    public FutureCarTownCtrl townCtrl;

    [Header("오브젝트")]
    public List<GameObject> buildSprite;
    public Animation btnAnim;

    [Header("데이터")]
    public string buildingName;
    public int buildId;
    public string nameId;
    public int requireBuild;
    public int buildCost;
    List<Dictionary<string, object>> buildingData;
    private bool isLoadData;
    string soundName = "eff_Common_next";

    [Header("텍스트")]
    public TextAsset textAsset;

    private void Start()
    {
        townCtrl = FindObjectOfType<FutureCarTownCtrl>();
        buildSprite.Add(this.transform.Find("Building").gameObject);
        buildSprite.Add(this.transform.Find("Tile").gameObject);
        buildSprite.Add(this.transform.Find("BuildButton").gameObject);
        buildingData = CSVReader.Read(textAsset);

        btnAnim = this.transform.Find("BuildButton").transform.GetChild(1).GetComponent<Animation>();
        isLoadData = false;
    }

    void BuildingDataSetup()  //빌딩 데이터 셋업
    {
        buildingName = this.gameObject.name;

        int _buildNo = int.Parse(buildingName.Substring(buildingName.Length - 2, 2));

        for(int i = 0; i < buildingData.Count; i++)
        {
            if (_buildNo == int.Parse(buildingData[i]["Build_ID"].ToString()))
            {
                buildId = int.Parse(buildingData[i]["Build_ID"].ToString());
                nameId = buildingData[i]["Name_ID"].ToString();
                requireBuild = int.Parse(buildingData[i]["Require_build"].ToString());

                buildCost = int.Parse(buildingData[i]["Value"].ToString());
            }
        }

        if (requireBuild == 0)
        {
            buildSprite[0].SetActive(false);
            buildSprite[1].SetActive(true);
            buildSprite[2].SetActive(true);
        }
        else
        {
            buildSprite[0].SetActive(false);
            buildSprite[1].SetActive(false);
            buildSprite[2].SetActive(false);
        }

        if (townCtrl.buildingList.Count > 0)   //완성된 빌딩리스트가 있을 경우
        {
            if (townCtrl.buildingList.Contains(this.buildingName))   //빌딩리스트에 본 빌딩의 이름이 있으면 건물 이미지 활성화
            {
                buildSprite[0].SetActive(true);
                buildSprite[1].SetActive(false);
                buildSprite[2].SetActive(false);
            }
            else
            {
                foreach(string _buildingName in townCtrl.buildingList)
                {
                    int _No = int.Parse(_buildingName.Substring(buildingName.Length - 2, 2));   //필요건물의 번호
                    if(requireBuild == _No)
                    {
                        buildSprite[0].SetActive(false);
                        buildSprite[1].SetActive(true);
                        buildSprite[2].SetActive(true);
                        break;
                    }
                }
            }
        }

    }

    public void BuildingSetup()
    {
        buildSprite[0].SetActive(true);
        buildSprite[1].SetActive(false);
        buildSprite[2].SetActive(false);
    }

    public void NextBuildingSetup()  //건물을 지으면 다음 건물의 이미지 세팅
    {
        buildSprite[0].SetActive(false);
        buildSprite[1].SetActive(true);
        buildSprite[2].SetActive(true);
    }

    [SerializeField] Image buildingImage;
    //[SerializeField] int cost;// = 750000;

    public void ClickBuildButton()  //건설 가능 버튼을 누르면,
    {
        //townCtrl 을 통해 클릭한 건물의 정보를 전달
        //전달할 내용
        //건물명, 건물이미지명, 비용
        //상기 내용을 전달하면 해당 내용을 기준으로 팝업창 활성화
        //건설을 누르면??

        //일단 정보 전달부터 시작
        SoundManager.instance.PlayEffectSound(soundName, 1f);
        townCtrl.ConstructPopupSet(buildingName, buildingImage, buildCost, nameId, this.gameObject);
    }

    private void Update()
    {
        if (!isLoadData)
        {
            isLoadData = true;
            BuildingDataSetup();
        }

        if (btnAnim.gameObject.activeSelf) ButtonAnimPlay();
    }

    void ButtonAnimPlay()
    {
        if (!btnAnim.isPlaying) btnAnim.Play();
    }

}
