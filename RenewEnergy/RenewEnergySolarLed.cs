using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenewEnergySolarLed : MonoBehaviour
{
    Image led;
    Color[] ledColor = new Color[2];
    public float generatePower = 1f;

    private void Start()
    {
        generatePower = 1f;
        led = GetComponent<Image>();
        ledColor[0] = new Color(0f, 1f, 0f, 1f);
        ledColor[1] = new Color(1f, 0.6f, 0f, 1f);
        led.color = ledColor[0];
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.name == "Cloud01(Clone)" || collision.name == "TutoCloud(Clone)"|| collision.name == "Cloud02(Clone)")
        led.color = ledColor[1];
        generatePower = 0.5f;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Cloud01(Clone)" || collision.name == "TutoCloud(Clone)" || collision.name == "Cloud02(Clone)")
        {
            led.color = ledColor[0];
            generatePower = 1f;

            if(this.transform.parent.parent.name == "TutorialGamePanel")
            {
                this.transform.parent.parent.GetComponent<RenewEnergyGameController>().tutoCloudCount++;
            }
        }
    }



}
