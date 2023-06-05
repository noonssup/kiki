using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RenewEnergyGuide : MonoBehaviour
{
    public string generateName;
    public Text textGenerateName;
    public string guideComment;
    public Text textGuideComment;

    private void Start()
    {
        textGuideComment.text = guideComment;
        textGenerateName.text = generateName;
    }
}
