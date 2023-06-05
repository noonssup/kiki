using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveParticle : MonoBehaviour
{
    float posx;
    string dir = "right";
    float delay = 1f;
    BoxCollider2D coll;

    private void Start()
    {
        posx = transform.position.x;
        dir = "right";
        coll = GetComponent<BoxCollider2D>();
        coll.enabled = false;
        delay = 1f;
        SoundManager.instance.PlayEffectSound("eff_Energy_tidal", 1f);
    }

    private void Update()
    {
        delay -= Time.deltaTime;

        if(delay < 0f) coll.enabled = true;

        if (this.transform.position.x > posx+0.2f)
        {
            dir = "left";
        }
        else if(this.transform.position.x < posx-0.2f)
        {
            dir = "right";
        }
        Move();
    }

    private void Move()
    {
        if (dir == "left")
        {
            this.transform.position = new Vector3(this.transform.position.x - Time.deltaTime * 0.1f, this.transform.position.y, this.transform.position.z);
        }
        else if (dir == "right")
        {
            this.transform.position = new Vector3(this.transform.position.x + Time.deltaTime * 0.1f, this.transform.position.y, this.transform.position.z);
        }
    }
}
