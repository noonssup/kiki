using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarDesignParts : MonoBehaviour
{
    public string imageName;
    public Image partsGuideImage;
    public Image partImage;
    public BoxCollider2D coll;
    public bool isCorrect = true;

    private void Awake()
    {
        partsGuideImage = GetComponent<Image>();

        partImage = this.transform.GetChild(0).GetComponent<Image>();
        imageName = partImage.sprite.name;

        //switch()
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name != "MoveItem(Clone)") return;

        Color _color = partImage.color;
        _color.a = 0.5f;
        partImage.color = _color;
        _color.a = 0f;
        partsGuideImage.color = _color;

        string itemName = collision.GetComponent<Image>().sprite.name;
        
        CarDesignMoveItem moveItem =collision.GetComponent<CarDesignMoveItem>();

        if (itemName.Substring(itemName.Length-2, 2) == imageName.Substring(imageName.Length-2, 2))
        {
            moveItem.itemCtrl.isCorrect = true;
            moveItem.itemCtrl.parts = this;
        }
        else
        {
            moveItem.itemCtrl.isCorrect = false;
            moveItem.itemCtrl.parts = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name != "MoveItem(Clone)") return;

        if (!isCorrect)
        {
            Color _color = partImage.color;
            _color.a = 0f;
            partImage.color = _color;
            _color.a = 1f;
            partsGuideImage.color = _color;
            if (collision.GetComponent<CarDesignMoveItem>().itemCtrl.parts == this) collision.GetComponent<CarDesignMoveItem>().itemCtrl.parts = null;
            else return;
        }
    }

    public void CorrectSetup(Color _color)   //정답 시 이미지 변경
    {
        partsGuideImage.enabled = false;
        partImage.color = _color;
        this.transform.GetComponent<BoxCollider2D>().enabled = false;
    }

}
