using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeverEffect : MonoBehaviour
{
    public Transform[] movePos;
    public GameObject effect;
    public Image effectImage;
    public Transform targetTr;

    public float moveSpeed;

    public GameObject feverTimeTextImage;

    private void Awake()
    {
        //movePos[0].position = new Vector3(730f, 350f,0f);
        //movePos[1].position = new Vector3(-730f, 350f,0f);
        //movePos[2].position = new Vector3(-730f, -350f,0f);
        //movePos[3].position = new Vector3(730f, -350f,0f);
        effect = transform.GetChild(0).gameObject;
        effectImage = effect.GetComponent<Image>();
        targetTr = movePos[1];
        moveSpeed = 15f;
        if(feverTimeTextImage.activeSelf) feverTimeTextImage.SetActive(false);
    }

    private void OnEnable()
    {
        feverTimeTextImage.GetComponent<Animator>().Rebind();
        feverTimeTextImage.SetActive(true);
    }

    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            if(effect.transform.position != targetTr.position)
            {
                effect.transform.position = Vector3.MoveTowards(effect.transform.position, targetTr.position, moveSpeed * Time.deltaTime);
            }
            else
            {
                if(targetTr == movePos[0])
                {
                    targetTr = movePos[1];
                }
                else if (targetTr == movePos[1])
                {
                    targetTr = movePos[2];
                }
                else if (targetTr == movePos[2])
                {
                    targetTr= movePos[3];
                }
                else if (targetTr == movePos[3])
                {
                    targetTr = movePos[0];
                }
            }
        }

        if (Input.GetKey(KeyCode.Alpha1))
        {
            effectImage.sprite = Resources.Load<Sprite>("gMiniGame/Barista/Images/img_job_game_success_01");
        }
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            effectImage.sprite = Resources.Load<Sprite>("gMiniGame/Barista/Images/img_job_game_success_02");
        }
    }
}
