using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownMapMoveController : MonoBehaviour
{
    public Vector2 defaultPos;
    public float moveSpeed = 100f;

    public Vector2 currentMove;
    private void OnMouseDown()
    {
        defaultPos = Input.mousePosition;
    }

    private void OnMouseDrag()
    {
        Vector2 diff = (Vector2)Input.mousePosition - defaultPos;
        Vector2 pos = this.transform.localPosition;

        pos.y += diff.y * Time.deltaTime * moveSpeed;
        pos.x += diff.x * Time.deltaTime * moveSpeed;


        //pos.x = Mathf.Clamp(pos.x, -371f, 371f);
        //pos.y = Mathf.Clamp(pos.y, -669f, 669f);

        //this.transform.position = pos;
        this.transform.localPosition = new Vector3(Mathf.Clamp(pos.x, -695f, 695f), Mathf.Clamp(pos.y, -836f, 836f), this.transform.localPosition.z);

        currentMove = this.transform.position;
        defaultPos = Input.mousePosition;
    }

    //무브락
    //맵
    // x 371, -371
    // y 669, -669
}
