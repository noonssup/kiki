using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectBtn : MonoBehaviour
{
    public Image buttonImage; //제목풍선의 이미지
    public Animator anim;
    public BoxCollider2D collider2d;

    private void Start()
    {
        buttonImage = this.transform.GetChild(1).GetComponent<Image>();
        anim = this.transform.GetChild(0).GetComponent<Animator>();
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        collider2d = GetComponent<BoxCollider2D>();
        collider2d.enabled = false;
        collider2d.enabled = true;
        //buttonImage.enabled = false;
        //anim.enabled = true;
    }

    private void OnMouseEnter()
    {
        anim.transform.localScale = new Vector3(1.15f, 1.15f, 1.15f);
        buttonImage.enabled = true;
        anim.enabled = false;
    }

    private void OnMouseExit()
    {
        anim.transform.localScale = new Vector3(1f, 1f, 1f);
        buttonImage.enabled = false;
        anim.enabled = true;
    }

    public void ButtonClick()  //버튼 클릭 시 버튼 오브젝트 상태 원래대로 되돌리기 (OnMouseExit 와 동일하게 처리)
    {
        this.transform.localScale = new Vector3(1f, 1f, 1f);
        buttonImage.enabled = false;
        anim.enabled = true;
    }
}
