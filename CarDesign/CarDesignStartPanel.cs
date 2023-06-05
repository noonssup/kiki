using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CarDesignStartPanel : MonoBehaviour
{
    //CarDesignGameManager gameCtrl;
    public GameObject startPanel;
    [SerializeField] TextMeshProUGUI text;

    private void Start()
    {
        //gameCtrl = FindObjectOfType<CarDesignGameManager>();
    }

    private void OnEnable()
    {
        if(text != null) text.text = "";
        if (Time.timeScale != 1)
        {
            Time.timeScale = 1;
        }
        StartCoroutine(GameStart());
    }

    IEnumerator GameStart()
    {
        WaitForSeconds wfs = new WaitForSeconds(1f);

        //int count = 1280;
        //while (count > -1)
        //{
        //    startPanel.transform.localPosition = new Vector2(count, startPanel.transform.position.y);
        //    count -= 24;
        //    yield return null;
        //}
        //if (count < 24) startPanel.transform.localPosition = new Vector2(0, startPanel.transform.position.y);

        text.text = "3";
        yield return wfs;
        text.text = "2";
        yield return wfs;
        text.text = "1";
        yield return wfs;
        text.text = "";
        gameObject.SetActive(false);
        //gameCtrl.GameStart();
    }
}
