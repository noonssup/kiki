using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UrbanWayPointController : MonoBehaviour
{
    public GameObject wayPointPrefab;
    public List<Transform> points;
    public float xPos = 9f;
    public float yPos = 3f;

    void Start()
    {
        SetWayPoints();
    }

    void SetWayPoints()
    {
        float x = xPos;
        float y = yPos;
        for(x=xPos;x>= -xPos;)
        {
            for(y=yPos;y>= -yPos;)
            {
                GameObject _point = Instantiate(wayPointPrefab);
                _point.transform.SetParent(transform, false);
                _point.transform.localPosition = new Vector2(x, y);
                _point.name = "WayPoint(" + x + "," + y + ")";
                points.Add(_point.transform);
                y -= 1f;
            }
            x -= 1f;
        }
    }


}
