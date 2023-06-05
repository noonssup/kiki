using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarDesignLevelSystem : MonoBehaviour
{
    [SerializeField] Level[] levels;
    [SerializeField] CarDesignGameManager gameCtrl;
   [SerializeField] int currentLevelIndex = -1;

    public void StartLevel()
    {
        if(gameCtrl.CarCount == currentLevelIndex + 2)
        {
            currentLevelIndex++;
            //gameCtrl.GameStart(levels[currentLevelIndex]);
        }
    }
}

[System.Serializable]
public struct Level
{
    public int gameLevel;
    public int maxPartsCount;
    public float gameTimer;
}
