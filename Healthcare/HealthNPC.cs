using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class NPCData
{
    public int currentHealth = 0;
    public int targetHealth = 0;
    public int currentStrength = 0;
    public int targetStrength = 0;
    public float currentMuscle = 0f;
    public float targetMuscle = 0f;
    public float currentBodyfat = 0f;
    public float targetBodyfat = 0f;
}


public class HealthNPC : MonoBehaviour
{
    public SpriteRenderer sr;
    public Canvas canvas;
    public TextMeshProUGUI text;
    public NPCData data = new NPCData();
    public List<Dictionary<string, object>> npcData;
    bool isComment = false;
    public string[] shoutComment = { "훅! 훅!", "읏샤", "힘들다...", "목표를 향해!!", "힘내자!!!", "내일을 향해서라면~",
        "오늘 저녁 뭐먹지?", "치킨 땡긴다...", "어제 떡볶이를 괜히...", "아자자자자자!!!!!", "하으야!!!", "가자~~~~~!!!!" };


    private void Start()
    {
        npcData = HealthGameManager.instance.npcData;
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera= Camera.main;
        StartCoroutine(FadeInNPC());
    }

    IEnumerator FadeInNPC()  //npc 나타나기 (투명에서 불투명으로)
    {
        Color _color = sr.color;
        _color.a = 0f;
        sr.color = _color;
        text.text = "";
        canvas.gameObject.SetActive(false);

        while (_color.a < 1)
        {
            yield return null;
            _color.a += Time.deltaTime * 0.5f;
            sr.color = _color;
        }

        string _script = string.Empty;
        int index = Random.Range(1, npcData.Count);
        Debug.Log("인덱스: " + index +" / 데이터카운트: "+ npcData.Count);
        for (int i = 0; i < npcData.Count; i++)
        {
            if (npcData[i]["id"].ToString() == index.ToString())
            {
                _script = npcData[i]["talk"].ToString();
                data.currentHealth = int.Parse(npcData[i]["set_con"].ToString());
                data.targetHealth = int.Parse(npcData[i]["goal_con"].ToString());
                data.currentStrength = int.Parse(npcData[i]["set_str"].ToString());
                data.targetStrength = int.Parse(npcData[i]["goal_str"].ToString());
                data.currentMuscle = int.Parse(npcData[i]["set_muscle"].ToString());
                data.targetMuscle = float.Parse(npcData[i]["goal_muscle"].ToString());
                data.currentBodyfat = float.Parse(npcData[i]["set_fat"].ToString());
                data.targetBodyfat = float.Parse(npcData[i]["goal_fat"].ToString());
            }
        }

        canvas.gameObject.SetActive(true);
        string _stringText = string.Empty;
        for (int i = 0; i < _script.Length; i++)
        {
            _stringText += _script[i];
            text.text = _stringText;
            yield return new WaitForSeconds(0.1f);

        }

        HealthGameManager.instance.TargetSetup(data);

        while (HealthGameManager.instance.gameState != HealthGameState.Play)
        {
            yield return null;
        }

        canvas.gameObject.SetActive(false);
    }

    //private void Update()
    //{
    //    if (HealthGameManager.instance.gameState != HealthGameState.Play || isComment) return;
    //    StartCoroutine(ExerciseComment());
    //}

    IEnumerator ExerciseComment()
    {
        isComment = true;
        int index = Random.Range(0, shoutComment.Length);
        canvas.gameObject.SetActive(true);
        text.text = shoutComment[index];

        yield return new WaitForSeconds(2.5f);
        canvas.gameObject.SetActive(false);
        isComment = false;
        yield return new WaitForSeconds(0.2f);
    }
}
