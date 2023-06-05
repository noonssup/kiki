using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum UrbanWayPointState
{
    NORMAL = 0,
    DANGER = 1,
    SAFE = 2,
}


public class UrbanWayPoint : MonoBehaviour
{
    public UrbanWayPointState state = UrbanWayPointState.NORMAL;

    private void Start()
    {
        state = UrbanWayPointState.NORMAL;
    }

}
