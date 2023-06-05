using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisasterHelp : MonoBehaviour
{
    public Image helpImage;
    public Sprite[] helpSprites;
    public Button[] buttons;
    public int page = 0;

    private void OnEnable()
    {
        OnClickButton(3);
    }

    public void OnClickButton(int _index)
    {
        switch (_index)
        {
            case 0: page--; break;
            case 1: page++; break;
            case 2: this.gameObject.SetActive(false); break;
            default: page = 0; break;
        }

        buttons[0].gameObject.SetActive(true);
        buttons[1].gameObject.SetActive(true);

        if (page >= (helpSprites.Length - 1))
        {
            page = helpSprites.Length - 1;
            buttons[0].gameObject.SetActive(true);
            buttons[1].gameObject.SetActive(false);
        }
        else if (page <= 0)
        {
            page = 0;
            buttons[0].gameObject.SetActive(false);
            buttons[1].gameObject.SetActive(true);
        }

        helpImage.sprite = helpSprites[page];
    }
}
