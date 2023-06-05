using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CreatorBroadcastSetup : MonoBehaviour
{
    [Header("���ӵ�����")]
    public int gender = 2; //0==��, 1==��
    public string channelName;

    [Header("��ư������Ʈ")]
    public Button[] genderButtons;
    TouchScreenKeyboard keyboard;

    [Header("�ؽ�Ʈ")]
    public TMP_InputField inputFieldChannelName;
    public TextMeshProUGUI textAlert;

    private void OnEnable()
    {
        gender = 0;
        channelName = string.Empty;
        inputFieldChannelName.text = string.Empty;
        OnClickGenderSetup(gender);
    }

    public void OnClickGenderSetup(int index)  //���� ����
    {
        genderButtons[0].transform.GetChild(1).GetComponent<Image>().enabled = false;
        genderButtons[1].transform.GetChild(1).GetComponent<Image>().enabled = false;
        if (index < 2) genderButtons[index].transform.GetChild(1).GetComponent<Image>().enabled = true;

        gender = index;
    }

    public void ChannelNameChange() //Ű���� �Է� �� ä�θ� �ݿ�
    {
        if (inputFieldChannelName.text.Length > 10)
        {
            int maxlength = Mathf.Min(inputFieldChannelName.text.Length, 80);
            inputFieldChannelName.text = inputFieldChannelName.text.Substring(0, maxlength);
        }
        channelName = inputFieldChannelName.text;
    }

    public void OnSelectd()
    {
        keyboard = TouchScreenKeyboard.Open(inputFieldChannelName.text);
    }


    public void InputCheck()
    {
        if(inputFieldChannelName.isFocused == false)
        {
            EventSystem.current.SetSelectedGameObject(inputFieldChannelName.gameObject, null);
            inputFieldChannelName.OnPointerClick(new PointerEventData(EventSystem.current));
        }
    }

    public void OnClickGameStart()  //���� ��ư Ŭ��
    {
        if (channelName == string.Empty)//(inputFieldChannelName.text == string.Empty)
        {
            //if (textAlert.color.a > 0f) return;
            //StartCoroutine(AlertEnterChannelName());
            //return;
            int index = Random.Range(0, 2);
            Debug.Log(index + "�ε���");
            if(index == 0)
            {
                channelName = "ŰŰTV";
            }
            else { channelName = "��ŰTV"; }
            CreatorGameManager.instance.GameStart(gender, channelName);
        }
        else
        {
            //channelName= inputFieldChannelName.text;
            CreatorGameManager.instance.GameStart(gender, channelName);
        }
    }

    IEnumerator AlertEnterChannelName()  //ä�θ� ���Է½� ����
    {
        Color _color = textAlert.color;
        _color.a = 0f;
        textAlert.color = _color;

        while (textAlert.color.a < 1f)
        {
            _color.a += Time.deltaTime * 5f;
            textAlert.color = _color;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        _color.a = 1f;
        while (textAlert.color.a > 0f)
        {
            _color.a -= Time.deltaTime;
            textAlert.color = _color;
            yield return null;
        }
    }

    public void OnClickExitGame()
    {
        CreatorGameManager.instance.OnClickExitButton();
    }

}
