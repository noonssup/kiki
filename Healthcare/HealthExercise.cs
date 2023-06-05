using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthExercise : MonoBehaviour
{
    public SpriteRenderer sr;
    public Sprite[] sprites;
    public Sprite[] changeSprite = new Sprite[2];
    public int spriteNumber = 0;
    public float spriteChangeTimer = 0.5f;
    float timer;

    //운동 순서
    //맨몸운동, 유산소운동, 머신웨이트, 프리웨이트, 웨이트트레이닝
    public void SetExercise(int _index)  //운동별 sprite 초기화
    {
        changeSprite[0] = sprites[_index * 2-2];
        changeSprite[1] = sprites[_index * 2-1];
        sr.sprite = changeSprite[0];

        //운동별 이미지 변경 시간간격조절
        switch (_index)
        {
            case 1:Debug.Log("맨몸운동"); break;
            case 2:Debug.Log("유산소운동"); break;
            case 3:Debug.Log("머신웨이트"); break;
            case 4:Debug.Log("프리웨이트"); break;
            case 5: Debug.Log("웨이트트레이닝"); break;
        }
    }

    private void Update()
    {
        if (HealthGameManager.instance.gameState != HealthGameState.Play) return;

        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            sr.sprite = changeSprite[spriteNumber];
            spriteNumber++;
            if (spriteNumber >= changeSprite.Length)
            {
                spriteNumber = 0;
            }
            timer = spriteChangeTimer;
        }

    }
}
