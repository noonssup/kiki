using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotInventoryCanvasController : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceCamera;
        GetComponent<Canvas>().worldCamera = Camera.main;
        this.GetComponent<Canvas>().sortingLayerName = "UI";
    }
}
