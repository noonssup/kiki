using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RescuePadPrefab : MonoBehaviour
{
    public RescuePad padCtrl;

    private void Start()
    {
        padCtrl = FindObjectOfType<RescuePad>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        padCtrl.CheckCorrect(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        padCtrl.CheckCorrect(null);
    }
}
