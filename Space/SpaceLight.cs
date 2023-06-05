using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceLight : MonoBehaviour
{
    Image lightImage;
    Color color;
    float delay = 1f;
    public bool isLight = false;

    private void Start()
    {
        lightImage= GetComponent<Image>();
        color = lightImage.color;
    }

    private void Update()
    {
        if (isLight)
        {
            delay -= Time.deltaTime;
            if (delay <= 0 && lightImage.color.a < 1)
            {
                color.a = 1;
                lightImage.color = color;
                delay = 1f;
            }

            if (delay <= 0 && lightImage.color.a > 0)
            {
                color.a = 0;
                lightImage.color = color;
                delay = 1f;
            }
        }
        else
        {
            color.a = 0;
            lightImage.color = color;
        }

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
