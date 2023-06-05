using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDesignContentLock : MonoBehaviour
{
    private void Update()
    {
        if (this.transform.position.x < -13.5f) this.transform.position = new Vector3(-13.5f, this.transform.position.y, this.transform.position.z);
        else if(this.transform.position.x > 0) this.transform.position = new Vector3(0f, this.transform.position.y, this.transform.position.z);
    }
}
