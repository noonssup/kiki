using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
/// <summary>
/// 미래자동차 머지게임에서 사용되는 블럭을 컨트롤할 스크립트
/// 머지게임 화면에서 블럭의 컨트롤을 담당 (블럭 이동, 머지 등 마우스로 클릭해서 하는 모든 동작 수행)
/// </summary>
 

public class FutureCarItemBlock : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] FutureCarCanvas sceneCtrl;
    [SerializeField] FutureCarGameCtrl gameCtrl;  //머지게임컨트롤 스크립트
    //[SerializeField] GarageBoxCtrl garageBox;   //차고 스크립트

    [Header("블럭 정보")]
    [SerializeField] string tilePosName; //현재 블럭이 위치한 타일의 이름
    public string tempItemId;
    [SerializeField] string itemId;      //아이템아이디 (11~87), 저장된 정보가 있을 경우 해당 id 를, 최초 생성 시에는 랜덤으로 id 부여
    [SerializeField] string mergeItemId; //본 스크립트의 오브젝트에 닿은 블럭오브젝트의 아이템아이디
    [SerializeField] string mergeResultId; //본 스크립트의 오브젝트와 닿은 블럭오브젝트가 머지할 경우 생성될 새로운 아이템아이디
    /// <summary>
    /// tilePosName : 블럭이 생성된 위치의 타일 이름을 저장하기 위함 (저장된 데이터 호출 시 해당 정보를 통해 저장된 위치로 이동)
    /// itemId : 아이템아이디를 통해 출력할 이미지와 게임함수 실행 등을 처리
    /// mergeItemId : 머지할 오브젝트의 아이디 정보 (머지 불가능한 아이템일 경우 머지 등의 동작 무시)
    /// mergeResultId : 머지할 오브젝트의 아이디가 본 오브젝트와 머지가능한 경우 결과물의 id 를 호출, 머지 시 결과물id 정보로 변경
    /// </summary>
    

    public int productPart = 0;
    public string productName = null; //부품명
    public string productDescription = null; //부품 설명
    public TextMeshProUGUI textProductName;  //부품명 텍스트
    public TextMeshProUGUI textDescription;  //부품 설명 텍스트
    public Image imageDescription; //부품 이미지
    //public int productPrice = 0;  //차량 가격
    public string itemTier;
    public TextMeshProUGUI textTier;
    public int itemExp;

    [Header("블럭상태정보")]
    public bool isMoving = false;
    public bool isStock = false;
    public bool isBoard = false;
    public bool isMerge = false;
    public  bool isSelected = false;
    public bool isSell = false;



    [Header("트랜스폼정보")]
    [SerializeField] Transform resetTr;  //처음 위치한 자리의 트랜스폼
    [SerializeField] Transform moveTr;   //블럭을 드래그 중일 때 위치할 트랜스폼 (화면 전체에 드래그 중인 블럭이 보이기 위함)
    [SerializeField] Transform mergeTr;  //머지할 블럭이 위치한 트랜스폼
    [SerializeField] Vector3 resetPos;   //블럭이 원위치될 때 돌아갈 위치 정보

    [Header("이미지/테이블")]
    public Sprite itemBoxImage;
    Image itemImage;
    string currentImageName;  //원이미지명
    string changeImageName;   //머지로 변경될 이미지명
    public GameObject carEffect; //자율주행차를 만들면 나타날 연출효과
    List<Dictionary<string, object>> data;  //csv 읽기
    public TextAsset textAsset;
    List<Dictionary<string, object>> stMain;  //csv 읽기

    /*[Header("음향")]
    [SerializeField] AudioSource sfxPlayer;
    [SerializeField] AudioClip grabSound;
    [SerializeField] AudioClip releaseSound;
    [SerializeField] AudioClip mergeSound;
    [SerializeField] AudioClip autoCarSound;*/

    private void Awake()
    {
        sceneCtrl = FindObjectOfType<FutureCarCanvas>();
        gameCtrl = FindObjectOfType<FutureCarGameCtrl>();
        //garageBox = FindObjectOfType<GarageBoxCtrl>();
        itemImage = GetComponent<Image>();

        moveTr = GameObject.Find("BlockMovePanel").transform;
        /*sfxPlayer = this.GetComponent<AudioSource>();
        sfxPlayer.playOnAwake = false;
        sfxPlayer.loop = false;
        sfxPlayer.volume = 1f;*/
        //textDevProductName = gameCtrl.textDevProductName;
        //textDevDescription = gameCtrl.textDevDescription;
        //textLabProductName = gameCtrl.textLabProductName;
        //textLabDescription = gameCtrl.textLabDescription;
    }

    private void Start()
    {
        //data = CSVReader.Read("gMiniGame/FutureCar/Merge_item_table_csv");
        data = CSVReader.Read(textAsset);

        //st_table 완성 시 아래 내용으로 변경
        stMain = sceneCtrl.stMain;
        //stMain = CSVReader.Read("gMiniGame/FutureCar/MerItem_Description_Table_csv"); //추후삭제

        BlockStatusSetup();
    }

    void BlockStatusSetup()  //블럭 정보 세팅
    {


        if (tempItemId == "0")  //블럭 생성시 전달 받은 임시 아이디, 파트를 기준으로 itemId 할당
        {
            switch (productPart)
            {
                case 1: itemId = "11"; break;
                case 2: itemId = "21"; break;
                case 3: itemId = "31"; break;
                case 4: itemId = "41"; break;
                case 5: itemId = "51"; break;
                case 6: itemId = "61"; break;
                case 7: itemId = "71"; break;
                case 8: itemId = "81"; break;
            }
        }
        else
        {
            itemId = tempItemId;
            string _num = tempItemId.Substring(0, 1);
            
            //Debug.Log("블럭임시아이디 : " +tempItemId + " 블럭아이디 : " + itemId);
            //Debug.Log("_num 에 저장된 내용 : " + _num);
            int _number = 0;
            _number = int.Parse(_num);
            productPart = _number;
            //Debug.Log("제품파트번호 : " + productPart);
        }

        mergeItemId = itemId;



        for (int i = 0; i < data.Count; i++)  //아이템아이디 할당 후 이미지 할당
        {
            if (data[i]["Item_ID"].ToString() == itemId)
            {
                //dataLineNo = i;
                //description = data[i]["Name"].ToString();
                string imagePath = data[i]["Image_Path"].ToString();
                //itemImage.sprite = Resources.Load<Sprite>(imagePath);

                for(int j = 0; j < gameCtrl.itemSprites.Count; j++)
                {
                    if(gameCtrl.itemSprites[i].name == imagePath)
                    {
                        itemImage.sprite = gameCtrl.itemSprites[i];
                        break;
                    }
                }


                if (itemImage.sprite != null) currentImageName = itemImage.sprite.name;
                productName = data[i]["Name_ID"].ToString();
                productDescription = data[i]["Context_ID"].ToString();

                //2022.10.18 gene 수정 (자율차 티어 M 으로 출력)
                if (itemId == "87")
                {
                    itemTier = data[i]["Tier"].ToString();
                    textTier.text = "M";
                }
                else
                {
                    itemTier = data[i]["Tier"].ToString();
                    textTier.text = "T" + itemTier;
                }
                itemExp = int.Parse(data[i]["Exp"].ToString());
                //string _price = data[i]["Price"].ToString();
                //productPrice = int.Parse(_price);
                break;
            }
        }

        resetTr = this.transform.parent.transform;
        //타일의 콜라이더 비활성화
        resetTr.GetComponent<BoxCollider2D>().enabled = false;
        this.transform.name = itemId;

        if (productPart == 1 || productPart == 2 || productPart == 3 || productPart == 4)
        {
            textProductName = gameCtrl.textDevProductName;
            textDescription = gameCtrl.textDevDescription;
            imageDescription = gameCtrl.devProductIcon;
        }
        else if (productPart == 5 || productPart == 6 || productPart == 7 || productPart == 8)
        {
            textProductName = gameCtrl.textLabProductName;
            textDescription = gameCtrl.textLabDescription;
            imageDescription = gameCtrl.labProductIcon;
        }

        sceneCtrl.totalItemBlock.Add(this.gameObject);
        gameCtrl.BlockListSave();
    }


    public void OnBeginDrag(PointerEventData eventData)  //드래그(또는 터치) 시작
    {
        PrintItemDescription();

        isMoving = true;
        this.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);

        //타일의 콜라이더 활성화
        resetTr.GetComponent<BoxCollider2D>().enabled = true;
        resetTr.GetComponent<Image>().enabled = true;
        this.transform.SetParent(moveTr);
        //if(!sfxPlayer.isPlaying) sfxPlayer.PlayOneShot(grabSound);
        SoundManager.instance.PlayEffectSound("eff_FutureCar_drag", 1);
    }

    public void OnDrag(PointerEventData eventData)  //드래그 중
    {
        if (isMoving)
        {
            Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = _mousePos;
        }
    }

    void ResetPosition()  //드래그 종료 시 드래그 중이던 아이템블럭의 위치 초기화
    {
        this.transform.position = resetTr.position;
        this.transform.localScale = Vector3.one;
        //if (!sfxPlayer.isPlaying) sfxPlayer.PlayOneShot(releaseSound);
        SoundManager.instance.PlayEffectSound("eff_FutureCar_use", 1f);
        //타일의 콜라이더 비활성화
        resetTr.GetComponent<BoxCollider2D>().enabled = false;
        gameCtrl.BlockListSave();
    }

    public void OnEndDrag(PointerEventData eventData)  //드래그 끝
    {
        //차고에 블럭을 보관
        if (isStock)  
        {
            if (gameCtrl.stockBlock.Count >= 18)
            {
                resetTr.GetComponent<BoxCollider2D>().enabled = false;
                ResetPosition();  //차고에 보관한 아이템이 18개 이상(max 18개)이면 리턴
            }

            if (mergeTr.childCount == 0)
            {
                if (this.resetTr.parent.name != "Content") this.resetTr.GetComponent<Image>().enabled = false;
                this.transform.SetParent(mergeTr); //차고에 닿은 블럭타일의 트랜스폼 정보를 mergeTr 에 넣어 처리
                resetTr = this.transform.parent;
                this.transform.localScale = Vector3.one;
                //BoardSelectImageOnOff(this.gameObject);
                //타일의 콜라이더 비활성화
                //resetTr.GetComponent<BoxCollider2D>().enabled = false;
                if (gameCtrl.stockBlock.Contains(this.transform)) { }
                else gameCtrl.stockBlock.Add(this.transform);

                //this.transform.parent.GetComponent<Image>().enabled = false;
                //sceneCtrl.totalItemBlock.Remove(this.gameObject);
                mergeTr = null;
                isStock = false;

                //if (!sfxPlayer.isPlaying) sfxPlayer.PlayOneShot(releaseSound);
                SoundManager.instance.PlayEffectSound("eff_FutureCar_use", 1f);
                //gameCtrl.BlockListSave();
            }
            else //StockTile 에 이미 아이템이 있을 경우, 원래 자리로 이동
            {
                isStock = false;
                ResetPosition();
                resetTr.GetComponent<BoxCollider2D>().enabled = false;

            }

        }

        //stock 블럭을 보드로 옮기거나 보드의 블럭을 다른 위치로 옮길 때
        if (isBoard)   
        {
            if (mergeTr.childCount == 0)
            {
                if (gameCtrl.stockBlock.Contains(this.transform))
                {
                    gameCtrl.RemoveStockBlock(this.transform);
                }

                if (this.resetTr.parent.name != "Content") this.resetTr.GetComponent<Image>().enabled = false;
                else this.resetTr.GetComponent<Image>().enabled = true;

                this.transform.SetParent(mergeTr); //차고에 닿은 블럭타일의 트랜스폼 정보를 mergeTr 에 넣어 처리
                this.resetTr = this.transform.parent;
                this.transform.localScale = Vector3.one;
                this.resetTr.GetComponent<Image>().enabled = true;

                //타일의 콜라이더 비활성화
                resetTr.GetComponent<BoxCollider2D>().enabled = false;

                mergeTr = null;
                isBoard = false;

                //if (!sfxPlayer.isPlaying) sfxPlayer.PlayOneShot(releaseSound);
                SoundManager.instance.PlayEffectSound("eff_FutureCar_use", 1f);
                //gameCtrl.BlockListSave();
            }
            else //StockTile 에 이미 아이템이 있을 경우, 원래 자리로 이동
            {
                isBoard = false;
                ResetPosition();
                resetTr.GetComponent<BoxCollider2D>().enabled = false;
            }
        }

        //아이템블럭 합성
        if (isMerge)
        {

            if (gameCtrl.stockBlock.Count >= 18 && mergeResultId == "87" && mergeTr.GetComponent<FutureCarItemBlock>().resetTr.transform.parent.name != "Content")   //차고가 가득 찼을 경우 자율주행차 머지 취소
            {
                resetTr.GetComponent<BoxCollider2D>().enabled = false;
                //string imagePath = "gMiniGame/FutureCar/Images/job_future_car_object/" + currentImageName;
                //this.imageDescription.sprite = Resources.Load<Sprite>(imagePath);
                //this.imageDescription.sprite = itemBoxImage;


                for (int i = 0; i < gameCtrl.itemSprites.Count; i++)
                {
                    if (gameCtrl.itemSprites[i].name == currentImageName)
                    {
                        this.imageDescription.sprite = gameCtrl.itemSprites[i];
                        break;
                    }
                }

                ResetPosition();  
                //차고에 보관한 아이템이 18개 이상(max 18개)이면 리턴
                return;
            }

            Transform _mergeBlock = MergeItemBlockSetup();

            

            if (this.resetTr.parent.name != "Content") this.resetTr.GetComponent<Image>().enabled = false;
            else this.resetTr.GetComponent<Image>().enabled = true;
            //this.resetTr.GetComponent<Image>().enabled = false;

            sceneCtrl.totalItemBlock.Remove(this.gameObject);
            gameCtrl.BlockListSave();
            _mergeBlock.GetComponent<FutureCarItemBlock>().resetTr.GetComponent<Image>().enabled = true;
            _mergeBlock.GetComponent<FutureCarItemBlock>().resetTr.GetComponent<BoxCollider2D>().enabled = false;
            resetTr.GetComponent<BoxCollider2D>().enabled = true;

            if (_mergeBlock.GetComponent<FutureCarItemBlock>().itemId == "87")
            {
                this.textDescription.text = "";
                this.textProductName.text = "";
                this.imageDescription.sprite = itemBoxImage;

                //머지한 아이템이 자율자동차가 될 경우, 연출과 함께 차고로 이동
                if (_mergeBlock.GetComponent<FutureCarItemBlock>().resetTr.transform.parent.name != "Content")
                {
                    _mergeBlock.GetComponent<FutureCarItemBlock>().StartCoroutine(AutonomousCarMoveGarage(_mergeBlock.transform));
                }
            }

            Destroy(this.gameObject);
            gameCtrl.BlockListSave();
            return;
        }

        //시설 투자
        if (isSell)
        {
            //경험치 증가 처리
            if (mergeTr.name == "DevFactory") gameCtrl._devExp = itemExp;
            if (mergeTr.name == "LabFactory") gameCtrl._labExp = itemExp;

            //아이템리스트에서 판매할 아이템 삭제
            sceneCtrl.totalItemBlock.Remove(this.gameObject);  //화면에 보이는 전체 아이템 중에서 해당 아이템 요소 삭제
            if (gameCtrl.stockBlock.Contains(this.transform)) gameCtrl.RemoveStockBlock(this.transform);  //차고리스트에 있는 아이템 요소 삭제
            
            //위치하던 자리의 타일 오브젝트 이미지 처리
            if (this.resetTr.parent.name != "Content") this.resetTr.GetComponent<Image>().enabled = false;
            else this.resetTr.GetComponent<Image>().enabled = true;
            this.resetTr.GetComponent<BoxCollider2D>().enabled = true;

            gameCtrl.PlaySoundEffect("eff_Common_casher");

            //오브젝트 삭제
            Destroy(this.gameObject);

            //상태 저장
            gameCtrl.BlockListSave();
            return;
        }


        isMoving = false;
        this.transform.SetParent(resetTr);
        ResetPosition();
        //gameCtrl.BlockListSave();
    }

    public IEnumerator AutonomousCarMoveGarage(Transform _item)  //완성된 자율주행차 차고로 이동
    {
        Transform targetTr = null;
        FutureCarItemBlock _itemBlock = _item.GetComponent<FutureCarItemBlock>();
        WaitForFixedUpdate _wait = new WaitForFixedUpdate();
        yield return _wait;
        _itemBlock.GetComponent<BoxCollider2D>().enabled = false;
        foreach(Transform _stockTile in gameCtrl._stockTileList) //차고의 빈자리 확인
        {
            if (_stockTile.transform.childCount == 0)
            {
                targetTr = _stockTile;
                break;
            }
        }
        _item.transform.SetParent(moveTr);
        _item.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);

        /* if (!_itemBlock.sfxPlayer.isPlaying)*/ //_itemBlock.sfxPlayer.PlayOneShot(autoCarSound);
        SoundManager.instance.PlayEffectSound("eff_FutureCar_complete", 1f);

        //아이템에 반짝이효과 추가 후 이동
        GameObject _carEffect = Instantiate(carEffect, _item.transform.position, Quaternion.identity);
        _carEffect.transform.SetParent(_item.transform);
        //_carEffect.transform.localScale = Vector3.one;
        List<Transform> _particles1List = new List<Transform>();
        _carEffect.transform.GetComponentsInChildren<Transform>(_particles1List);
        _particles1List.RemoveAt(0);
        for (int i = 0; i < _particles1List.Count; i++)
        {
            _particles1List[i].localScale = Vector3.one;
        }
        yield return new WaitForSeconds(1f);
        while (_particles1List[0].localScale.x > 0f)
        {
            yield return null;
            for (int i = 0; i < _particles1List.Count; i++)
            {
                _particles1List[i].localScale -= Vector3.one * Time.deltaTime * 5f;
            }
        }

        Destroy(_carEffect);
        yield return new WaitForSeconds(0.5f);


        while (_item.transform.position != targetTr.position)    //차고의 빈자리 중 가장 앞에 있는 타일을 타겟으로 이동
        {
            yield return null;
            _item.transform.position = Vector3.MoveTowards(_item.transform.position, targetTr.position, 100f * Time.deltaTime);
        }

        if(_item.transform.position == targetTr.position)  //이동 후
        {
            _itemBlock.resetTr.GetComponent<Image>().enabled = false;
            _itemBlock.resetTr.GetComponent<BoxCollider2D>().enabled = true;

            _itemBlock.resetTr = targetTr;   //리셋tr 변경
            _item.transform.SetParent(targetTr);                           //targetTr 하위로 이동
            _item.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);    //스케일 줄임 (타일에 자리하는 애니메이션 효과를 위함)
            _itemBlock.resetTr.GetComponent<Image>().enabled = true;   //타일의 이미지 활성화
            _itemBlock.resetTr.GetComponent<BoxCollider2D>().enabled = false;  //타겟의 콜라이더 비활성화

            //if (!_itemBlock.sfxPlayer.isPlaying) _itemBlock.sfxPlayer.PlayOneShot(releaseSound);
            SoundManager.instance.PlayEffectSound("eff_FutureCar_use", 1f);
            //차고에 위치할 때 스케일 변경으로 애니메이션 효과
            while (_item.transform.localScale.x < 1.8f)   
            {
                yield return null;
                _item.transform.localScale += Vector3.one * Time.deltaTime * 15f;
            }
            while (_item.transform.localScale.x > 1f)
            {
                yield return null;
                _item.transform.localScale -= Vector3.one * Time.deltaTime * 15f;
            }
            while (_item.transform.localScale.x < 1.2f)
            {
                yield return null;
                _item.transform.localScale += Vector3.one * Time.deltaTime * 15f;
            }
            while (_item.transform.localScale.x > 1f)
            {
                yield return null;
                _item.transform.localScale -= Vector3.one * Time.deltaTime * 15f;
            }



            _itemBlock.GetComponent<BoxCollider2D>().enabled = true;
            _item.transform.localScale = Vector3.one;

            if (gameCtrl.stockBlock.Contains(_item.transform)) { }
            else gameCtrl.stockBlock.Add(_item.transform);

            mergeTr = null;
            isStock = false;
            gameCtrl.BlockListSave();
        }
    }

    Transform MergeItemBlockSetup()  //머지한 블럭의 정보 셋업
    {
        FutureCarItemBlock _mergeTrCtrl = mergeTr.transform.GetComponent<FutureCarItemBlock>();
        for (int i = 0; i < data.Count; i++)
        {
            if (data[i]["Item_ID"].ToString() == mergeResultId)
            {

                for (int j = 0; j < gameCtrl.itemSprites.Count; j++)
                {
                    if (gameCtrl.itemSprites[j].name == changeImageName)
                    {
                        _mergeTrCtrl.itemImage.sprite = gameCtrl.itemSprites[j];
                        break;
                    }
                }
                

                //string imagePath = "gMiniGame/FutureCar/Images/job_future_car_object/" + changeImageName;
                //_mergeTrCtrl.itemImage.sprite = Resources.Load<Sprite>(imagePath);

                _mergeTrCtrl.currentImageName = this.itemImage.sprite.name;

                _mergeTrCtrl.itemId = mergeResultId;
                _mergeTrCtrl.tempItemId = mergeResultId;
                _mergeTrCtrl.name = mergeResultId;

                _mergeTrCtrl.mergeItemId = data[i]["Merge_item_ID"].ToString();
                _mergeTrCtrl.mergeResultId = data[i]["Merge_result_ID"].ToString();
                _mergeTrCtrl.productName = data[i]["Name_ID"].ToString();
                _mergeTrCtrl.productDescription = data[i]["Context_ID"].ToString();

                //string_ID 에 맞는 StringTable 대입
                for(int j = 0; j < stMain.Count; j++)
                {
                    if (stMain[j]["String_ID"].ToString() == _mergeTrCtrl.productName)
                    {
                        _mergeTrCtrl.productName = stMain[j]["KO"].ToString();
                    }

                    if (stMain[j]["String_ID"].ToString() == _mergeTrCtrl.productDescription)
                    {
                        _mergeTrCtrl.productDescription = stMain[j]["KO"].ToString();
                        break;
                    }
                }
                _mergeTrCtrl.textProductName.text = _mergeTrCtrl.productName;
                _mergeTrCtrl.textDescription.text = _mergeTrCtrl.productDescription;
                _mergeTrCtrl.imageDescription.sprite = _mergeTrCtrl.itemImage.sprite;
                //티어, 경험치 설정
                _mergeTrCtrl.itemTier = data[i]["Tier"].ToString();
                if (_mergeTrCtrl.itemTier == "7") _mergeTrCtrl.textTier.text = "M";
                else _mergeTrCtrl.textTier.text = "T" + _mergeTrCtrl.itemTier;
                _mergeTrCtrl.itemExp = int.Parse(data[i]["Exp"].ToString());
                _mergeTrCtrl.productPart = int.Parse(_mergeTrCtrl.itemId.Substring(0, 1));
            }
        }



        //아이템합성 + 자율 주행차가 차고에서 만들어질 경우의 애니메이션효과
        if (_mergeTrCtrl.itemId != "87" || _mergeTrCtrl.itemId == "87" && _mergeTrCtrl.transform.parent.transform.parent.name == "Content")
        { 
            _mergeTrCtrl.StartCoroutine(MergeEffect(_mergeTrCtrl.transform));
        }

        gameCtrl.BlockListSave();
        return _mergeTrCtrl.transform;
    }

    
    public IEnumerator MergeEffect(Transform _mergeBlock)  //아이템합성 시 합성된 아이템의 애니메이션 처리
    {
        //if (!_mergeBlock.GetComponent<FutureCarItemBlock>().sfxPlayer.isPlaying) _mergeBlock.GetComponent<FutureCarItemBlock>().sfxPlayer.PlayOneShot(mergeSound);
        SoundManager.instance.PlayEffectSound("eff_FutureCar_complete", 1f);

        _mergeBlock.localScale = Vector3.zero;

        while (_mergeBlock.transform.localScale.x < 1.8f)
        {
            yield return null;
            _mergeBlock.transform.localScale += Vector3.one * Time.deltaTime * 15f;
        }
        while (_mergeBlock.transform.localScale.x > 1f)
        {
            yield return null;
            _mergeBlock.transform.localScale -= Vector3.one * Time.deltaTime * 15f;
        }
        while (_mergeBlock.transform.localScale.x < 1.2f)
        {
            yield return null;
            _mergeBlock.transform.localScale += Vector3.one * Time.deltaTime * 15f;
        }
        while (_mergeBlock.transform.localScale.x > 1f)
        {
            yield return null;
            _mergeBlock.transform.localScale -= Vector3.one * Time.deltaTime * 15f;
        }

        _mergeBlock.transform.localScale = Vector3.one;
    }
    void BoardSelectImageOnOff(GameObject _selectButton)  //보드에서 선택된 아이템의 타일 이미지 끄기
    {
        foreach (GameObject _block in sceneCtrl.totalItemBlock)
        {
            if (_block == _selectButton) 
            { 
                _block.GetComponent<FutureCarItemBlock>().resetTr.GetComponent<Image>().enabled = true;
            }
            else
            {
                if (_block.GetComponent<FutureCarItemBlock>().resetTr.parent.name == "LabGameBoard" || _block.GetComponent<FutureCarItemBlock>().resetTr.parent.name == "DevGameBoard")
                    _block.GetComponent<FutureCarItemBlock>().resetTr.GetComponent<Image>().enabled = false;
            }
        }
    }
    public void PrintItemDescription()  //버튼 클릭 시 해당 아이템 설명 출력
    {
        GameObject _selectButton = EventSystem.current.currentSelectedGameObject;
        BoardSelectImageOnOff(_selectButton);



        for (int j = 0; j < stMain.Count; j++)
        {
            if (stMain[j]["String_ID"].ToString() == productName)
            {
               productName = stMain[j]["KO"].ToString();
            }

            if (stMain[j]["String_ID"].ToString() == productDescription)
            {
                productDescription = stMain[j]["KO"].ToString();
                break;
            }
        }

        textProductName.text = productName;
        textDescription.text = productDescription;
        imageDescription.sprite = this.itemImage.sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isMoving)
        {
            if (collision.transform.parent.name == "Content")   //차고 타일과 충돌
            {
                isStock = true;
                mergeTr = collision.transform;
            }

            if (collision.transform.parent.name == "DevGameBoard")   //생산보드의 타일과 충돌
            {
                string _itemId = null;
                if (itemId != null) { _itemId = itemId.Substring(0, 1); }
                else return;

                if (_itemId == "1" || _itemId == "2" || _itemId == "3" || _itemId == "4")
                {
                    isBoard = true; mergeTr = collision.transform;
                }
                else { return; }
            }

            if (collision.transform.parent.name == "LabGameBoard")  //연구보드의 타일과 충돌
            {
                string _itemId = null;
                if (itemId != null) { _itemId = itemId.Substring(0, 1); }
                else return;

                if (_itemId == "5" || _itemId == "6" || _itemId == "7" || _itemId == "8")
                {
                    isBoard = true;
                    mergeTr = collision.transform;
                }
                else { return; }
            }

            if (collision.tag == "Block")  //다른 아이템 블럭과 충돌
            {
                isMerge = true;
                CheckMerge(collision.transform);
            }

            if (collision.name == "DevFactory")   //생산시설과 충돌
            {

                string _itemId = null;
                if (itemId != null) { _itemId = itemId.Substring(0, 1); }
                else return;

                if (_itemId == "1" || _itemId == "2" || _itemId == "3" || _itemId == "4" || itemId == "87")
                {
                    isSell = true;
                    mergeTr = collision.transform;
                }
                else { return; }
            }

            if (collision.name == "LabFactory")  //연구시설과 충돌
            {

                string _itemId = null;
                if (itemId != null) { _itemId = itemId.Substring(0, 1); }
                else return;

                if (_itemId == "5" || _itemId == "6" || _itemId == "7" || _itemId == "8")
                {
                    isSell = true;
                    mergeTr = collision.transform;
                }
                else { return; }
            }
        }
    }

    void CheckMerge(Transform _otherTr)  //블럭끼리 닿아있을 때, 머지 가능한 블럭일 경우 결과물 이미지 출력
    {

        for (int i = 0; i < data.Count; i++)  //
        {
            if (data[i]["Item_ID"].ToString() == itemId)
            {
                //dataLineNo = i;
                if (data[i]["Merge_item_ID"].ToString() == _otherTr.GetComponent<FutureCarItemBlock>().itemId)
                {

                    isMerge = true;
                    mergeResultId = data[i]["Merge_result_ID"].ToString();
                }
                else
                {
                    isMerge = false;
                    return;
                }
            }


            for (int j = 0; j < data.Count; j++)  //합성이 된 상태의 이미지 출력
            {
                if (mergeResultId == data[i]["Item_ID"].ToString())
                {
                    //string imagePath = "gMiniGame/FutureCar/Images/job_future_car_object/" + data[i]["Image_Path"].ToString();
                    ////Debug.Log("이미지패쓰 " + imagePath);
                    //this.itemImage.sprite = Resources.Load<Sprite>(imagePath);

                    for (int ii = 0; ii < gameCtrl.itemSprites.Count; ii++)
                    {
                        if (gameCtrl.itemSprites[ii].name == data[i]["Image_Path"].ToString())
                        {
                            this.itemImage.sprite = gameCtrl.itemSprites[ii];
                            break;
                        }
                    }

                    this.transform.localScale = Vector3.one;
                    changeImageName = this.itemImage.sprite.name;
                    mergeTr = _otherTr;
                    return;
                }
            }

        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isMoving)  //충돌했던 콜라이더와 떨어지면 이미지, bool 정보 변경
        {
            if(currentImageName != changeImageName)
            {
                //string imagePath = "gMiniGame/FutureCar/Images/job_future_car_object/" + currentImageName;
                //this.itemImage.sprite = Resources.Load<Sprite>(imagePath);


                for (int ii = 0; ii < gameCtrl.itemSprites.Count; ii++)
                {
                    if (gameCtrl.itemSprites[ii].name == currentImageName)
                    {
                        this.itemImage.sprite = gameCtrl.itemSprites[ii];
                        break;
                    }
                }


            }

            this.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            isStock = false;
            isBoard = false;
            isMerge = false;
            isSell = false;
            mergeTr = null;
        }
    }

    public void ParentTileSetup()  //아이템블럭이 위치한 부모타일 컴포넌트 처리
    {
        if (this.resetTr.parent.name != "Content") this.resetTr.GetComponent<Image>().enabled = false;
        else this.resetTr.GetComponent<Image>().enabled = true;

        this.resetTr.GetComponent<BoxCollider2D>().enabled = true;
    }
}
