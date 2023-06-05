using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RenewEnergyPillarMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    RectTransform rectTr;
    public Image pillarImage;

    public float minHeight = -1.1f;
    public float maxHeight = 2.8f;

    public bool isMove = false;
    float posX;

    private void Start()
    {
        rectTr = GetComponent<RectTransform>();
        pillarImage = transform.parent.GetComponent<Image>();
        posX = rectTr.position.x;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isMove = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isMove)
        {

            Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(posX, _mousePos.y, this.transform.position.z);

            if (this.rectTr.position.y >= maxHeight) rectTr.position = new Vector3(posX, maxHeight, this.rectTr.position.z);
            if (this.rectTr.position.y <= minHeight) rectTr.position = new Vector3(posX, minHeight, this.rectTr.position.z);
            float currentHeight = rectTr.position.y +1.1f;
            pillarImage.fillAmount = currentHeight / (maxHeight+1.1f);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isMove = false;
    }
}

