using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarDesignGuide : MonoBehaviour
{
    public Image helpImage;
    public Sprite[] helpSprites;
    public Button[] buttons;
    public int page;

    private void OnEnable()
    {
        page = 0;
        PageChange(page);
        buttons[0].interactable = false;
        buttons[1].interactable = true;
        buttons[2].interactable = true;
    }

    public void ClickButton(int _index)
    {
        switch (_index)
        {
            case 0: 
                page--;
                break; 
            case 1:
                page++;
                break; 
            case 2:
                this.gameObject.SetActive(false);
                break;
        }

        PageChange(page);
    }

    void PageChange(int _index)
    {
        buttons[0].interactable = true;
        buttons[1].interactable = true;
        if (_index >= (helpSprites.Length - 1))
        {
            _index = helpSprites.Length - 1;
            buttons[1].interactable = false;
            buttons[0].interactable = true;
        }
        else if(_index <= 0)
        {
            _index = 0;
            buttons[1].interactable = true;
            buttons[0].interactable = false;
        }
        page = _index;
        helpImage.sprite = helpSprites[page];
    }
}
