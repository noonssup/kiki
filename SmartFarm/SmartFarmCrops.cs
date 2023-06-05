using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SmartFarmCrops : MonoBehaviour
{
    SmartFarmGamePanel gameCtrl;

    //작물의 이미지
    //0 = 땅, 1 = 새싹, 2 = 줄기, 3 = 풋열매, 4 = 열매
    public Image[] cropImages;

    public Sprite[] image4s;
    //작물의 이미지
    //0 = 토마토, 1 = 사과, 2 = 수박, 3 = 딸기, 4 = 바나나
    public Sprite[] cropSprites;
    
    //작물 번호
    public int fruit;

    //성장률
    public float growthRate = 0f;

    //성장 단계
    bool[] isChange = new bool[5];

    public int cropsMoney;

    string growSoundName;

    private void Start()
    {
        gameCtrl = FindObjectOfType<SmartFarmGamePanel>();
        cropSprites = SmartFarmGameManager.instance.cropSprites;
        cropImages[cropImages.Length - 1].sprite = cropSprites[fruit];
        growSoundName = "eff_SmaerFarm_growUp";
        for (int i = 1; i < cropImages.Length; i++)
        {
            Color _color = new Color(1f, 1f, 1f, 0f);
            cropImages[i].color = _color;
        }

        isChange[0] = false;
        isChange[1] = false;
        isChange[2] = false;
        isChange[3] = false;
        isChange[4] = false;

        switch (fruit)
        {
            case 0: cropsMoney = 1000; break;
            case 1: cropsMoney = 2000; break;
            case 2: cropsMoney = 3000; break;
            case 3: cropsMoney = 1500; break;
            case 4: cropsMoney = 2000; break;
            case 5: cropsMoney = 3000; break;
            default: cropsMoney = 2000; break;
        }

        StartCoroutine(GrowthStart());
    }

    public IEnumerator GrowthStart()  //성장률에 따른 이미지 변화
    {
        gameCtrl.GetComponent<SmartFarmGrowthStatus>().CropsImageSet(cropImages[4].sprite);
        while (gameCtrl.isGamePlay)
        {
            if (growthRate < 5f && !isChange[0])
            {
                isChange[0] = true;
                Color _color = new Color(1f, 1f, 1f, 0f);
                cropImages[0].color = _color;
                while (cropImages[0].color.a < 1)
                {
                    _color.a += Time.deltaTime;
                    cropImages[0].color = _color;
                    yield return null;
                }
            }
            else if (growthRate > 80f && !isChange[4])
            {
                isChange[4] = true;
                Color _color1 = new Color(1f, 1f, 1f, 0f);
                Color _color2 = new Color(1f, 1f, 1f, 1f);
                cropImages[4].color = _color1;
                cropImages[3].color = _color2;
                while (cropImages[4].color.a < 1)
                {
                    _color1.a += Time.deltaTime;
                    cropImages[4].color = _color1;
                    _color2.a -= Time.deltaTime;
                    cropImages[3].color = _color2;
                    yield return null;
                }
                _color2 = new Color(1f, 1f, 1f, 0f);
                cropImages[3].color = _color2;
                SoundManager.instance.PlayEffectSound(growSoundName, 1f);
            }
            else if (growthRate > 60f && !isChange[3])
            {
                isChange[3] = true;
                Color _color1 = new Color(1f, 1f, 1f, 0f);
                Color _color2 = new Color(1f, 1f, 1f, 1f);
                cropImages[3].color = _color1;
                cropImages[3].sprite = Crop4SpriteSet(fruit);
                cropImages[2].color = _color2;
                
                while (cropImages[3].color.a < 1)
                {
                    _color1.a += Time.deltaTime;
                    cropImages[3].color = _color1;
                    _color2.a -= Time.deltaTime;
                    cropImages[2].color = _color2;
                    yield return null;
                }
                _color2 = new Color(1f, 1f, 1f, 0f);
                cropImages[2].color = _color2; 
                SoundManager.instance.PlayEffectSound(growSoundName, 1f);
            }
            else if (growthRate > 20f&& !isChange[2])
            {
                isChange[2] = true;
                Color _color1 = new Color(1f, 1f, 1f, 0f);
                Color _color2 = new Color(1f, 1f, 1f, 1f);
                cropImages[2].color = _color1;
                cropImages[1].color = _color2;
                while (cropImages[2].color.a < 1)
                {
                    _color1.a += Time.deltaTime;
                    cropImages[2].color = _color1;
                    _color2.a -= Time.deltaTime;
                    cropImages[1].color = _color2;
                    yield return null;
                }
                _color2 = new Color(1f, 1f, 1f, 0f);
                cropImages[1].color = _color2;
                SoundManager.instance.PlayEffectSound(growSoundName, 1f);
            }
            else if (growthRate >= 5f && !isChange[1])
            {
                isChange[1] = true;
                Color _color1 = new Color(1f, 1f, 1f, 0f);
                Color _color2 = new Color(1f, 1f, 1f, 1f);
                cropImages[1].color = _color1;
                cropImages[0].color = _color2;
                while (cropImages[1].color.a < 1)
                {
                    _color1.a += Time.deltaTime;
                    cropImages[1].color = _color1;
                    _color2.a -= Time.deltaTime;
                    cropImages[0].color = _color2;
                    yield return null;
                }
                _color2 = new Color(1f, 1f, 1f, 0f);
                cropImages[0].color = _color2;
                SoundManager.instance.PlayEffectSound(growSoundName, 1f);
            }

            yield return null;
        }
    }

    Sprite Crop4SpriteSet(int _num)  //풋열매 스프라이트 할당
    {

        return image4s[_num];
    }

    public IEnumerator GrowthRateCheck()  //재배 종료 시 작물의 성장률에 따른 팝업 이미지 출력
    {
        Color _color = new Color(1f, 1f, 1f, 0f);
        if (growthRate < 5f)
        {
            while (_color.a < 1f)
            {
                _color.a += Time.deltaTime;
                this.cropImages[0].color = _color;
                yield return null;
            }
        }
        else if (growthRate > 80f)
        {
            while (_color.a < 1f)
            {
                _color.a += Time.deltaTime;
                this.cropImages[4].color = _color;
                yield return null;
            }
        }
        else if (growthRate > 60f)
        {
            while (_color.a < 1f)
            {
                _color.a += Time.deltaTime;
                this.cropImages[3].color = _color;
                yield return null;
            }
        }
        else if (growthRate > 20f)
        {
            while (_color.a < 1f)
            {
                _color.a += Time.deltaTime;
                this.cropImages[2].color = _color;
                yield return null;
            }
        }
        else if (growthRate >= 5f)
        {
            while (_color.a < 1f)
            {
                _color.a += Time.deltaTime;
                this.cropImages[1].color = _color;
                yield return null;
            }
        }
    }
}
