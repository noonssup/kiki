using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreatorReactionComment : MonoBehaviour
{
    public Image boxImage;
    public TextMeshProUGUI textComment;
    public Color colorRed = new Color(0.7f, 0f, 0f);
    public Color colorGreen = new Color(0f, 0.7f, 0f);

    public void CommentSetup(int _type, string _comment)
    {
        if (_type == 1)
        {
            boxImage.color = Color.green;
            textComment.color = colorGreen;

        }
        else if(_type == 2) 
        {
            boxImage.color = Color.red;
            textComment.color = colorRed;
        }

        textComment.text = _comment;
    }
}
