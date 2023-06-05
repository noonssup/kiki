using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 재난관리전문가 시작 패널용 스크립트
/// </summary>
/// 
public class DisasterStartPanel : MonoBehaviour
{
    public TextMeshProUGUI text;
    public GameObject startPanel;

    private void Start()
    {
        
        text = transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (text != null)
            text.text = "";
        GameStart();
    }

    //private void OnMouseDown()
    //{
    //    if (Time.timeScale == 0) Time.timeScale = 1f;
    //    SoundManager.instance.PlayEffectSound("eff_Common_next", 1f);
    //    StartCoroutine(GameStartCoroutine());
    //}

    void GameStart()
    {
        if (Time.timeScale == 0) Time.timeScale = 1f;
        SoundManager.instance.PlayEffectSound("eff_Common_next", 1f);
        StartCoroutine(GameStartCoroutine());
    }

    IEnumerator GameStartCoroutine()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f);

        int count = 1280;
        while(count > -1) 
        {
            startPanel.transform.localPosition = new Vector2(count, startPanel.transform.position.y);
            count -= 24;
            yield return null;
        }
        if(count < 24) startPanel.transform.localPosition = new Vector2(0, startPanel.transform.position.y);
        //yield return wfs;
        text.text = "3";
        yield return wfs;
        text.text = "2";
        yield return wfs;
        text.text = "1";
        yield return wfs;
        text.text = "";
        gameObject.SetActive(false);
    }
}
