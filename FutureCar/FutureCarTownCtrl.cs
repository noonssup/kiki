using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FutureCarTownCtrl : MonoBehaviour
{
    [Header("스크립트컨트롤러")]
    FutureCarCanvas sceneCtrl;
    
    [Header("컴포넌트")]
    public GameObject constructPopup;  //건설 팝업
    public GameObject buildingObject;  //건물오브젝트
    public Image buildingImage;   //팝업에 표시될 건물이미지

    [Header("게임데이터")]
    public int gamePoint = 0;
    public int _gamePoint
    {
        get { return gamePoint; }
        set
        {
            gamePoint = sceneCtrl.gamePoint;
            textGamePoint.text = gamePoint.ToString("#,##0");
        }
    }
    public List<string> buildingList;
    string[] soundName = { "eff_FutureCar_build", "eff_Barista_wrong", "eff_Common_next" }; //건설 효과음, 재화 부족음, 팝업 비활성화음
    public int buildCost;         //건물 건설에 필요한 비용
    public string buildingName;   //건물 오브젝트의 이름 (리스트 저장용)

    [Header("텍스트")]
    public TMP_Text textGamePoint;
    public TextMeshProUGUI textBuildingName; //팝업에 표시될 건물명
    public TMP_Text textBuildComment;      //건설하시겠습니까?  (포인트 부족 시 "보유 포인트가 부족합니다...") 출력
    public TMP_Text textBuildCost;    //팝업에 표시될 건설비용 텍스트

    private void Awake()
    {
        sceneCtrl = FindObjectOfType<FutureCarCanvas>();
        constructPopup.SetActive(false);
    }

    private void Start()
    {
        buildingList = new List<string>();
        buildingList = sceneCtrl.buildingList;
        GameDataSetup();
    }

    void GameDataSetup()  //게임 데이터 셋업
    {
        if(constructPopup.activeSelf) constructPopup.SetActive(false);
        gamePoint = sceneCtrl.gamePoint;
        textGamePoint.text = gamePoint.ToString("#,##0");
    }

    string nameId = null;
    [SerializeField] Sprite[] buildingIcons;
    public void ConstructPopupSet(string _buildingName, Image _image, int _cost, string _nameId, GameObject _building)  //건설 버튼을 누르면 활성화 (건물명, 건물이미지명, 비용)
    {
        //팝업창에서 변경할 내용들의 변수를 만들고 할당하자

        buildingObject = _building;
        nameId = _nameId;
        textBuildingName.text = _nameId;
        textBuildCost.text = _cost.ToString();
        buildCost = _cost;
        //buildingImage.sprite = _image.sprite;
        buildingImage.sprite = BuildingIconSetup(_image);// _image.sprite;
        buildingName = _buildingName;
        buildingImage.SetNativeSize();
        textBuildComment.text = "건설하시겠습니까?";
        constructPopup.SetActive(true);
    }

    Sprite BuildingIconSetup(Image _image)  //팝업 활성화 시 빌딩 이미지 셋업
    {
        Sprite _buildingSprite = null;
        string _buildingImageName = _image.transform.parent.transform.parent.name;

        for(int i = 0; i < 24; i++)
        {
            if(_buildingImageName == "Building" + (i+1).ToString("000"))
            {
                _buildingSprite = buildingIcons[i];
                break;
            }
        }

        return _buildingSprite;
    }



    public void CancelButton()  //취소버튼 누름
    {
        SoundManager.instance.PlayEffectSound(soundName[2], 1f);
        constructPopup.SetActive(false);
    }

    public void BuildButton()  //건설버튼 누름
    {
        if (tmpText != null && tmpText.activeSelf) return;  //이전에 지은 건물의 텍스트가 표시중인 상태이면 무시



        //건설 비용 삭감 및 부족시 연출 추가
        if (gamePoint < buildCost)
        {
            SoundManager.instance.PlayEffectSound(soundName[1], 1f);
            textBuildComment.text = "보유 포인트가 부족합니다.";
            return;
        }
        else
        {
            SoundManager.instance.PlayEffectSound(soundName[0], 1f);
            sceneCtrl.gamePoint -= buildCost;
            _gamePoint = sceneCtrl.gamePoint;

            StartCoroutine(BuildEffect());
        }

        buildingList.Add(buildingName);   //건설한 건물명 리스트에 추가

        DataManager.instance.futureCarGameData.gamePoint = gamePoint;
        DataManager.instance.futureCarGameData.buildingList = buildingList;
    }

    public GameObject buildEffectPrefab;
    public GameObject buildEffectParticle;
    public GameObject buildTMP;
    public GameObject tmpText=null;

    IEnumerator BuildEffect()
    {
        WaitForSeconds wait1sec = new WaitForSeconds(1f);
        WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();
        yield return waitFixedUpdate;
        buildEffectParticle = Instantiate(buildEffectPrefab, buildingObject.transform.position, Quaternion.identity);
        buildEffectParticle.transform.SetParent(this.transform);
        List<Transform> _particles1List = new List<Transform>();
        buildEffectParticle.transform.GetComponentsInChildren<Transform>(_particles1List);
        _particles1List.RemoveAt(0);
        for (int i = 0; i < _particles1List.Count; i++)
        {
            _particles1List[i].localScale = new Vector3(1f, 1f, 1f);
        }
        yield return waitFixedUpdate;


        tmpText = Instantiate(buildTMP, buildingObject.transform.position, Quaternion.identity);


        tmpText.transform.SetParent(this.transform);
        tmpText.transform.localScale = Vector3.one;
        tmpText.transform.position = buildingObject.transform.position;

        tmpText.GetComponent<TextMeshProUGUI>().text = nameId + "\n건설 완료";
        StartCoroutine(BuildTMPMoveEffect());

        buildingObject.GetComponent<FutureCarBuildingCtrl>().BuildingSetup();

        //지은 건물의 이름 확인 후 다음 건물 이미지 활성화 처리
        FutureCarBuildingCtrl[] _arryBuilding = FindObjectsOfType<FutureCarBuildingCtrl>();

        int _buildingNo = int.Parse(buildingName.Substring(buildingName.Length - 3, 3));

        foreach (FutureCarBuildingCtrl _builing in _arryBuilding)
        {
            if (_builing.requireBuild == _buildingNo)
            {
                _builing.NextBuildingSetup();
            }
        }
        constructPopup.SetActive(false);

        ///호옥시나 건물이 전부 활성화되서 이펙트가 필요하다면???
        ///
        ///여기다 추가하도록 합니다.

        yield return new WaitForSeconds(0.6f);

        Destroy(buildEffectParticle.gameObject);
        buildEffectParticle = null;
    }

    IEnumerator BuildTMPMoveEffect()
    {
        WaitForFixedUpdate waitFixedUpdate = new WaitForFixedUpdate();
        yield return waitFixedUpdate;

        Color _color = tmpText.GetComponent<TextMeshProUGUI>().color;
        _color.a = 1f;
        tmpText.GetComponent<TextMeshProUGUI>().color = _color;
        yield return new WaitForSeconds(0.3f);
       
        while (_color.a > 0f)
        {
            yield return null;
            tmpText.transform.localPosition = new Vector3(tmpText.transform.localPosition.x, tmpText.transform.localPosition.y + 100f * Time.deltaTime, tmpText.transform.localPosition.z);
            _color.a -= Time.deltaTime;
            if (_color.a <= 0f) _color.a = 0f;
            tmpText.GetComponent<TextMeshProUGUI>().color = _color;
        }

        Destroy(tmpText.gameObject);
        tmpText = null;
    }

    private void OnDisable()
    {
        if (tmpText != null)
        {
            Destroy(tmpText.gameObject);
            tmpText = null;
        }
        if (buildEffectParticle != null)
        {
            Destroy(buildEffectParticle.gameObject);
            buildEffectParticle = null;
        }
    }

    private void Update()
    {


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            sceneCtrl.gamePoint = 400000000;
            _gamePoint = sceneCtrl.gamePoint;
        }
#endif
    }
}
