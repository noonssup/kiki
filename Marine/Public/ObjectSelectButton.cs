using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectButton : MonoBehaviour
{
    public Image buttonImage; //제목풍선의 이미지
    public Animator anim;

    private void Start()
    {
        buttonImage = this.transform.GetChild(1).GetComponent<Image>();
        anim = this.transform.GetChild(1).GetComponent<Animator>();
    }

    private void OnMouseEnter()
    {
        this.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        anim.enabled = false;
    }

    private void OnMouseExit()
    {
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        anim.enabled = true;
    }
}
