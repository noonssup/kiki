using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Xml.Linq;
using System;

/// <summary>
/// 운동 선택 버튼용 스크립트
/// </summary>


public class ProgramValue
{
    public string programName = string.Empty;
    public string programDescription = string.Empty;
    public int programDay = 0;
    public int value1Type = 0;
    public int value2Type = 0;
    public float value1Num = 0;
    public float value2Num = 0;
    public int programID = 0;
}



public class HealthProgram : MonoBehaviour
{
    public ProgramValue programValue = new ProgramValue();
    public TextMeshProUGUI textProgramName;
    Image btnImage;

    public List<Dictionary<string, object>> programData;


    public void ProgramSetup(int _id)  //버튼에 운동 프로그램 설정
    {
        //program= _state;
        btnImage = GetComponent<Image>();
        programData = HealthGameManager.instance.programData;
        for (int i = 0; i < programData.Count; i++)
        {
            if (programData[i]["id"].ToString() == _id.ToString())
            {
                programValue.programName = programData[i]["name"].ToString();
                programValue.programDescription = programData[i]["desc"].ToString();
                programValue.programDay = int.Parse(programData[i]["day"].ToString());
                programValue.value1Type = int.Parse(programData[i]["value1_type"].ToString());
                programValue.value1Num = float.Parse(programData[i]["value1_num"].ToString());
                programValue.value2Type = int.Parse(programData[i]["value2_type"].ToString());
                programValue.value2Num = float.Parse(programData[i]["value2_num"].ToString());
                programValue.programID = _id;
            }
        }
        textProgramName.text = programValue.programName;
    }


    public void OnClickProgramButton()
    {
        HealthGameManager.instance.ProgramInformationSetup(programValue, btnImage);
    }



} 