using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CompleteCoffee : MonoBehaviour
{
    private void Update()
    {
        if (this.transform.GetChild(1).transform.GetComponent<Image>().enabled == false)
        {
            this.transform.GetComponent<DragItem>().enabled = false;
        }
        else this.transform.GetComponent<DragItem>().enabled = true;
    }
}
