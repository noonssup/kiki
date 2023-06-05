using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SmartFarmHelpGuide : MonoBehaviour
{
    public Image helpImage;
    public Sprite[] helpGuideSprites;
    public Button[] controlButtons;
    public int page;
    int maxPage = 5;
    int minPage = 0;

    private void OnEnable()
    {
        page = 0;
        PageChange(string.Empty);
    }

    public void OnClickButton()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        PageChange(buttonName);
    }

    void PageChange(string buttonName)
    {
        switch (buttonName)
        {
            case "NextButton":++page; if (page >= maxPage) page = maxPage; break;
            case "PrevButton":--page; if (page <= minPage) page = minPage; break;
            case "CloseButton": this.gameObject.SetActive(false); break;
            default: break;
        }

        helpImage.sprite = helpGuideSprites[page];

        if (this.gameObject.activeSelf)
        {
            if(page == minPage)
            {
                controlButtons[1].interactable = false;
                controlButtons[2].interactable = true;
                controlButtons[0].interactable = true;
            }
            else if(page > minPage && page < maxPage)
            {
                controlButtons[1].interactable = true;
                controlButtons[2].interactable = true;
                controlButtons[0].interactable = true;
            }
            else if(page == maxPage)
            {
                controlButtons[0].interactable = false;
                controlButtons[2].interactable = true;
                controlButtons[1].interactable = true;
            }

        }
    }
}
