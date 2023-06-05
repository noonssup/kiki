using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisasterMap : MonoBehaviour
{
    public DisasterController disasterCtrl;

    public void ClickBg()
    {
        disasterCtrl.ReleaseData();
        DisasterGameManager.instance.SelectBuildingCursor(null);
    }
}
