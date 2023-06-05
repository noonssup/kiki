using UnityEngine;

public class RobotBodyHealthCtrl : MonoBehaviour
{
    [SerializeField] Transform parentTr;
    [SerializeField] WheelJoint2D[] wJoints;
    [SerializeField] RelativeJoint2D[] rJoints;



    public int totalHp = 0;
    [SerializeField] GameObject destroyEffect;
    [SerializeField] RobotHealthController[] _allHp;


    private void Awake()
    {
        totalHp = 0;
        parentTr = this.transform.parent;
    }

    private void Start()
    {
        Invoke("FrameHealthAdd", 0.1f);
    }

    void FrameHealthAdd()  //로봇 부품 정보에 프레임의 체력 수치 더해주기
    {
        RobotBattleSceneController gameCtrl = FindObjectOfType<RobotBattleSceneController>();

        //조립화면이 켜질 때마다 프레임부품의 체력을 UI 체력게이지에 넣어주기
        //Debug.Log("프레임의 체력 : " + this.transform.GetComponent<RobotHealthController>().maxHp);
        if (this.gameObject.layer == LayerMask.NameToLayer("PLAYER")) gameCtrl._healthPoint = this.transform.GetComponent<RobotHealthController>().maxHp;

    }

    public void HealthPointSetup()
    {
        //RobotHealthController[] _allHp = parentTr.GetComponentsInChildren<RobotHealthController>();
        _allHp = parentTr.GetComponentsInChildren<RobotHealthController>();

        foreach(RobotHealthController hp in _allHp)
        {
            //Debug.Log("부품별 체력 : " + hp.gameObject.name + " " + hp.maxHp);
            totalHp += hp.maxHp;
        }

        //Debug.Log(parentTr.name +  "의 토탈 체력 : " + totalHp);
    }

    public void JointRelease()
    {
        rJoints = parentTr.GetComponentsInChildren<RelativeJoint2D>();
        wJoints = parentTr.GetComponentsInChildren<WheelJoint2D>();

        foreach (RelativeJoint2D rj in rJoints)
        {
            rj.connectedBody = null;
            rj.enabled = false;
        }

        foreach (WheelJoint2D rj in wJoints)
        {
            rj.connectedBody = null;
            rj.enabled = false;
        }
    }

    public void SetDamage(int _damage)
    {
        totalHp -= _damage;
        if(totalHp <= 0)// && this.gameObject != null)
        {
           GameObject _effect = Instantiate(destroyEffect, new Vector3(this.transform.position.x, this.transform.position.y-1f, this.transform.position.z), Quaternion.identity);
            Destroy(_effect, 1.5f);
            Destroy(parentTr.gameObject); 
        }
    }
}
