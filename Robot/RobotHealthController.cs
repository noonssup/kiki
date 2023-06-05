using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotHealthController : MonoBehaviour, IDamage
{
    //부품 내구도(체력)을 초기화하는 스크립트
    //체력 및 조인트에 대한 설정 처리
    //각 부품에 본 스크립트를 컴포넌트로 할당해줄 것
    RobotCanvas robotCanvas;

    public string weaponName;
    public int maxHp = 0;
    public int hp = 0;
    public List<Dictionary<string, object>> dataTable;  //csv 읽기

    [SerializeField] RobotBodyHealthCtrl healthCtrl;

    //public TextAsset textAsset;

    private void Awake()
    {
        //HpInit();
    }

    private void Start()
    {
        robotCanvas = FindObjectOfType<RobotCanvas>();

        dataTable = robotCanvas.robotData;
        //dataTable = CSVReader.Read(textAsset);
        // if(this.GetComponent<BoxCollider2D>() != null) this.GetComponent<BoxCollider2D>().enabled = true;
        //if (this.transform.GetComponent<RobotBodyHealthCtrl>() != null) healthCtrl = this.transform.GetComponent<RobotBodyHealthCtrl>();
        //else healthCtrl = null;
        RobotBodyHealthCtrl[] hCtrl = FindObjectsOfType<RobotBodyHealthCtrl>();
        for (int i = 0; i < hCtrl.Length; i++)
        {
            if (hCtrl[i].gameObject.layer == this.gameObject.layer) healthCtrl = hCtrl[i];
            else continue;
        }
        if(this.gameObject.name == "BodyFrame") healthCtrl = this.transform.GetComponent<RobotBodyHealthCtrl>();

        HpInit();
    }

    void HpInit()
    {
        weaponName = this.transform.GetComponent<RobotPartsController>().weaponName;

        for (int i = 0; i < dataTable.Count; i++)
        {
            if (dataTable[i]["Parts_ID"].ToString() == weaponName)
            {
                maxHp = int.Parse(dataTable[i]["add_MaxHP"].ToString());
            }
        }
        hp = maxHp;
    }

    public void Damage(int damage)
    {
        if (this.transform.GetComponent<RobotMouseMoveController>() != null && !this.transform.GetComponent<RobotMouseMoveController>().isAssem) return;
        else if (this.transform.GetComponent<RobotMouseMoveController>() == null && this.gameObject.name == "BodyFrame") { }
        healthCtrl.SetDamage(damage);
        RobotUICanvasController.uiCtrl.ChangeHP(this.gameObject.layer, damage);
    }
}
