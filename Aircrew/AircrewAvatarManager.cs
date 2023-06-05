using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircrewAvatarManager : MonoBehaviour
{
    public AirPassenger airPassenger;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 클릭한 좌표를 가져 옴
            Ray2D ray = new Ray2D(wp, Vector2.zero); // 원점에서 터치한 좌표 방향으로 Ray를 쏨
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, float.MaxValue);

            if (hit = Physics2D.Raycast(ray.origin, ray.direction, float.MaxValue))
            {
                if (hit.collider.tag.Equals("AvatarImage"))
                {
                    OnAvataButtonClick();
                }
            }
        }
    }

    public void OnAvataButtonClick()
    {
        airPassenger.MovePassengerPopup();
    }
}
