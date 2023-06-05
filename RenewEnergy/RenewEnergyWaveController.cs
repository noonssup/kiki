using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenewEnergyWaveController : MonoBehaviour
{
    public float moveSpeed=2f;
    
    public enum MoveDir
    {
        left,
        right
    }
    public MoveDir dir = MoveDir.left;


    private void Start()
    {
        if (this.transform.position.x > 2.5f)
        {
            dir = MoveDir.left;
        }
        else if (this.transform.position.x < 2.5f)
        {
            dir = MoveDir.right;
        }
    }

    private void Update()
    {
        MoveWave();
    }

    void MoveWave()   //조류의 이동
    {
        if (this.transform.parent.name == "TutorialGamePanel")
        {
            if (this.gameObject.activeSelf && dir == MoveDir.left)
                this.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            else if (this.gameObject.activeSelf && dir == MoveDir.right)
                this.transform.position += Vector3.right * moveSpeed * Time.deltaTime;


            if (this.transform.position.x >= 15f || this.transform.position.x <= -15f)
            { Destroy(this.gameObject); }
        }
        else
        {
            if (this.gameObject.activeSelf && dir == MoveDir.left)
                this.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
            else if (this.gameObject.activeSelf && dir == MoveDir.right)
                this.transform.position += Vector3.right * moveSpeed * Time.deltaTime;


            if (this.transform.position.x >= 15f || this.transform.position.x <= -15f || !RenewEnergyGameManager.instance.isGamePlaying)
            { Destroy(this.gameObject); }
        }
    }
}
