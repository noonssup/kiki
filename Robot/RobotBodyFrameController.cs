using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotBodyFrameController : MonoBehaviour
{

    [SerializeField] BoxCollider2D boxColl;
    [SerializeField] GameObject inventory;

    private void Awake()
    {
        boxColl = GetComponent<BoxCollider2D>();
        inventory = GameObject.Find("InventoryCanvas");
    }

    private void OnEnable()
    {
        if (inventory != null && inventory.activeSelf) boxColl.enabled = false;
    }
    private void Update()
    {
        if (inventory != null && !inventory.activeSelf)
        {
            if (this.boxColl.enabled == false)  boxColl.enabled = true;
        }
    }

}
