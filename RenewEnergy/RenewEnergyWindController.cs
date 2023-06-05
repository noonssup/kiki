using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenewEnergyWindController : MonoBehaviour
{
    public float moveSpeed;

    private void Start()
    {
        moveSpeed =4f;
        SoundManager.instance.PlayEffectSound("eff_Common_wind", 1f);
    }
    private void Update()
    {
        if (this.transform.parent.name == "TutorialGamePanel")
        {
            if (this.gameObject.activeSelf)
                this.transform.position += Vector3.left * moveSpeed * Time.deltaTime;

            if (this.transform.position.x <= -15f)// || !NewEnergyGameManager.instance.isGamePlaying)
            { Destroy(this.gameObject); }
        }
        else
        {

            if (this.gameObject.activeSelf)
                this.transform.position += -Vector3.right * moveSpeed * Time.deltaTime;

            if (this.transform.position.x <= -15f || !RenewEnergyGameManager.instance.isGamePlaying)
            { Destroy(this.gameObject); }
        }
    }
}
