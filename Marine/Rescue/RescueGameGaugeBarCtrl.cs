using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueGameGaugeBarCtrl : MonoBehaviour
{
    public GameObject gaugePoint;
    public RescueLevel2Ctrl lv2Ctrl;

    private void Start()
    {
        lv2Ctrl = FindObjectOfType<RescueLevel2Ctrl>();
        gaugePoint = null;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        gaugePoint = collision.gameObject;
        lv2Ctrl.CheckTriggerGaugeBar(gaugePoint);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gaugePoint = null; lv2Ctrl.CheckTriggerGaugeBar(gaugePoint);
    }
}
