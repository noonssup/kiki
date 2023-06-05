using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimCtrl : MonoBehaviour
{
    public Animator anim;
    public string btnName;
    private void Awake()
    {
        anim = GetComponent<Animator>();
        btnName = this.transform.parent.name;
    }

    private void Start()
    {
        switch (btnName)
        {
            case "GameButton": anim.SetBool("gamebtn", true); break;
            case "VideoButton": anim.SetBool("videobtn", true); break;
        }
    }
}
