using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotEnemyMoveController : MonoBehaviour
{
    [SerializeField] RobotBattleSceneController gameCtrl;
    [SerializeField] RobotPlayerMoveController player;
    [SerializeField] RobotCrWeaponController chainsaw;
    [SerializeField] Transform targetTr = null;
    [SerializeField] float distance;
    [SerializeField] float moveDistance = -5f;
    [SerializeField] bool isMove;

    [SerializeField] WheelJoint2D[] wjoint;
    [SerializeField] JointMotor2D jMotor;
    [SerializeField] float moveSpeed = 100f;
    [SerializeField] float spd;
    [SerializeField] float h;

    private void OnEnable()
    {
        StatusSetup();
        AttackSetup();
    }

    void StatusSetup()
    {
        gameCtrl = FindObjectOfType<RobotBattleSceneController>();
        player = FindObjectOfType<RobotPlayerMoveController>();
        targetTr = player.transform;

        if (targetTr.GetComponentInChildren<RobotCrWeaponController>() != null) chainsaw = targetTr.GetComponentInChildren<RobotCrWeaponController>();
        else chainsaw = null;
        isMove = false;
        wjoint = this.transform.GetComponentsInChildren<WheelJoint2D>();
        moveSpeed = 100f;
        spd = moveSpeed;
    }

    void CheckDistance()
    {
        if (targetTr == null) return;

        distance = targetTr.position.x - this.transform.position.x;
        if (chainsaw != null) moveDistance = -7.5f;
        else moveDistance = -5f;

        if (aiChainsaw != null) moveDistance = -1f;

        if (distance < moveDistance)
        {
            h = -1f;

        }
        else if (distance > (moveDistance + 1f))
        {
            h = 1f;
        }
        else
        {

            h = 0f;
        }
    }

    private void Update()
    {
        if (gameCtrl.isGamePlay)  CheckDistance();
            MoveAction();
        
    }

    void MoveAction()
    {
        if (gameCtrl.isGameOver || targetTr == null) { h = 0f; }
        if (h > 0)  //오른쪽 이동
        {
            isMove = true;
            foreach (WheelJoint2D j in wjoint)
            {
                if (wjoint != null) j.useMotor = true;
                spd = moveSpeed;
                jMotor.motorSpeed = spd;
                jMotor.maxMotorTorque = 10000f;

                if (wjoint != null) j.motor = jMotor;
            }
        }
        else if (h < 0)  //왼쪽 이동
        {
            if (this.transform.position.x < -4.5f) return;

            isMove = true;
            foreach (WheelJoint2D j in wjoint)
            {
                if (wjoint != null) j.useMotor = true;
                spd = -moveSpeed;
                jMotor.motorSpeed = spd;
                jMotor.maxMotorTorque = 10000f;

                if (wjoint != null) j.motor = jMotor;
            }
        }
        if (h == 0f && isMove)
        {
            isMove = false;
            BreakControl();
        }
    }

    void BreakControl()
    {
        foreach (WheelJoint2D j in wjoint)
        {
            j.useMotor = true;
            jMotor.motorSpeed = 0f;
            jMotor.maxMotorTorque = 1000f;
            j.motor = jMotor;
        }
    }


    ///
    ///상단 코드는 AI 의 이동 함수

    ///하단 코드는 AI 의 공격 함수
    ///

    [SerializeField] GameObject aiChainsaw;
    //[SerializeField] List<RobotWeaponFireController> fireCtrl;

    void AttackSetup()
    {
        //fireCtrl = new List<RobotWeaponFireController>();

        if (this.transform.GetComponentInChildren<RobotCrWeaponController>() != null) aiChainsaw = this.transform.GetComponentInChildren<RobotCrWeaponController>().gameObject;
        else aiChainsaw = null;

        //if (this.transform.GetComponentsInChildren<RobotWeaponFireController>() != null)
        //{
        //    this.transform.GetComponentsInChildren<RobotWeaponFireController>(fireCtrl);
        //}
        //else fireCtrl = null;


    }
}
