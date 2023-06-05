using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenewEnergyWindImage : MonoBehaviour
{
    bool isActive = false;
    Image image;
    Color _color;

    private void Start()
    {
        image = GetComponent<Image>();
        isActive = true;
        _color = image.color;
        _color.a = Random.Range(0f, 1f);
        image.color = _color;
        TimerSet(Random.Range(0f, 1f));
    }

    void TimerSet(float timer)
    {
        isActive = true;
       
        StartCoroutine(ImageEffect(timer));
    }

    IEnumerator ImageEffect(float timer)
    {
        yield return new WaitForSeconds(timer);

        while (_color.a > 0f)
        {
            _color.a -= Time.deltaTime * 0.75f;
            image.color = _color;
            yield return null;
        }

        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));

        while (_color.a < 1f)
        {
            _color.a += Time.deltaTime * 0.75f;
            image.color = _color;
            yield return null;
        }

        //yield return new WaitForSeconds(Random.Range(0.01f, 0.2f));
        isActive = false;
        TimerSet(0.01f);
    }

}
