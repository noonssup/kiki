using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class FutureCarExhibition : MonoBehaviour
{
    [Header("스크립트컨트롤러")]
    [SerializeField]  FutureCarCanvas sceneCtrl;
    [SerializeField] FutureCarGameCtrl gameCtrl;
    //[SerializeField] FutureCarTownCtrl townCtrl;

    [Header("오브젝트")]
    public GameObject[] carSlot;
    public List<GameObject> goElecCar = new List<GameObject>();
    public List<GameObject> goAutoCar = new List<GameObject>();

    [Header("게임데이터")]
    public int autoAmount = 0;
    public int elecAmount = 0;
    public int elecCarCount = 0;
    public int autoCarCount = 0;
    public bool isClick = false;
    string[] soundName = { "eff_Common_casher","eff_Common_next" }; //차량 판매시 사운드, 팝업 사운드

    [Header("텍스트")]
    public TMP_Text[] textCount;
    public TMP_Text[] textAmount;

    private void Awake()
    {
        sceneCtrl = FindObjectOfType<FutureCarCanvas>();
        gameCtrl = FindObjectOfType<FutureCarGameCtrl>();
        //townCtrl = FindObjectOfType<FutureCarTownCtrl>();
    }

    private void OnEnable()
    {
        ExhibitionSetup();
    }

    void ExhibitionSetup()  //전시 판매장 팝업 셋업
    {
        SoundManager.instance.PlayEffectSound(soundName[1], 1f);
        goElecCar = new List<GameObject>();
        goAutoCar = new List<GameObject>();
        elecCarCount = 0;
        autoCarCount = 0;
        textAmount[0].text = 500000.ToString("#,##0");
        textAmount[1].text = 1000000.ToString("#,##0");
        foreach (GameObject _block in sceneCtrl.totalItemBlock)
        {
            if (_block.name == "47")
            {
                elecCarCount++;
                elecAmount = 500000;
                textAmount[0].text = elecAmount.ToString("#,##0");
                goElecCar.Add(_block);

            }
            else if (_block.name == "87")
            {
                autoCarCount++;
                autoAmount = 1000000;
                textAmount[1].text = autoAmount.ToString("#,##0");
                goAutoCar.Add(_block);
            }
        }

        textCount[0].text = "보유 : " +  elecCarCount.ToString();
        textCount[1].text = "보유 : " + autoCarCount.ToString();
    }

    public void SalesCarFunction()
    {
        if (!isClick)
        {
            SoundManager.instance.PlayEffectSound(soundName[0], 1f);

            isClick = true;
            GameObject _clickButton = EventSystem.current.currentSelectedGameObject;

            if (_clickButton.transform.parent.name == "ElecCarSlot" && elecCarCount > 0)
            {
                elecCarCount--;
                sceneCtrl.gamePoint += elecAmount;
                if (gameCtrl.gameObject.activeSelf)
                {
                    gameCtrl._gamePoint = sceneCtrl.gamePoint;
                    gameCtrl.stockBlock.Remove(goElecCar[0].transform);
                }
                //if (townCtrl.gameObject.activeSelf)
                //{
                //    townCtrl._gamePoint = sceneCtrl.gamePoint;
                //}
                sceneCtrl.totalItemBlock.Remove(goElecCar[0]);

                //차량 판매 시 타일의 콜라이더 활성화하기, 타일선택 이미지 제거
                goElecCar[0].GetComponent<FutureCarItemBlock>().ParentTileSetup();

                Destroy(goElecCar[0]);
                goElecCar.RemoveAt(0);
            }
            else if (_clickButton.transform.parent.name == "AutoCarSlot" && autoCarCount > 0)
            {
                autoCarCount--;
                sceneCtrl.gamePoint += autoAmount;
                if (gameCtrl.gameObject.activeSelf)
                {
                    gameCtrl._gamePoint = sceneCtrl.gamePoint;
                    gameCtrl.stockBlock.Remove(goAutoCar[0].transform);
                }

                //if (townCtrl.gameObject.activeSelf)
                //{
                //    townCtrl._gamePoint = sceneCtrl.gamePoint;
                //}

                sceneCtrl.totalItemBlock.Remove(goAutoCar[0]);

                //차량 판매 시 타일의 콜라이더 활성화하기, 타일선택 이미지 제거
                goAutoCar[0].GetComponent<FutureCarItemBlock>().ParentTileSetup();

                Destroy(goAutoCar[0]);
                goAutoCar.RemoveAt(0);
            }

            sceneCtrl.MergeDataSave();
        }

        textCount[0].text = "보유 : " + elecCarCount.ToString();
        textCount[1].text = "보유 : " + autoCarCount.ToString();

        StartCoroutine(ClickDelay());
    }

    IEnumerator ClickDelay()
    {
        yield return new WaitForSeconds(1f);
        isClick = false;
    }
}
