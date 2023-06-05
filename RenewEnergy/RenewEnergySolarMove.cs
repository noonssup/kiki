using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RenewEnergySolarMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public float minLeft = -8f;
    public float maxRight = 8f;
    float posY;
    public bool isMove = false;
    public float freezeTime = 2f;

    //인터페이스로 led 정보를 받아와서 발전량 조정??

    private void Start()
    {
        posY = this.transform.position.y;
        isMove= true;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //isMove = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isMove) return;

        //if (isMove)
        //{

            Vector2 _mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            this.transform.position = new Vector3(_mousePos.x, posY, this.transform.position.z);

            if (this.transform.position.x >= maxRight) this.transform.position = new Vector3(maxRight, posY, this.transform.position.z);
            if (this.transform.position.x <= minLeft) this.transform.position = new Vector3(minLeft, posY, this.transform.position.z);
        //}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //isMove = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.name == "Thunder")
        {
            if (isMove)
            {
                isMove = false;
                StartCoroutine(HitThunder());
            }
        }
    }

    IEnumerator HitThunder()
    {
        yield return new WaitForSeconds(freezeTime);
        isMove = true;
    }
}
