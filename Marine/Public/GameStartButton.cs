using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameStartButton : MonoBehaviour
{
    public string buttonName;
    private void Start()
    {
    }

    public void ClickButton()
    {
        
        buttonName = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log("클릭한 버튼 이름 " + buttonName);
    }
}
