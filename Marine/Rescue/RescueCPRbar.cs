using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescueCPRbar : MonoBehaviour
{
    public RescueLevel3Ctrl gameCtrl;
    public bool isMoveOn = false;
    public string barName;

    private void Start()
    {
        gameCtrl = FindObjectOfType<RescueLevel3Ctrl>();
        barName = this.gameObject.name;
        isMoveOn = false;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        isMoveOn = true;
        if(collision.name == "Heart")
        {
            if (barName == "VerticalBar") gameCtrl.CheckTrigger(isMoveOn, barName);
            else if (barName == "HorizontalBar") gameCtrl.CheckTrigger(isMoveOn, barName);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isMoveOn = false;
        if (collision.name == "Heart")
        {
            if (barName == "VerticalBar") gameCtrl.CheckTrigger(isMoveOn, barName);
            else if (barName == "HorizontalBar") gameCtrl.CheckTrigger(isMoveOn, barName);
        }
    }
}
