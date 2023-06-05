using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �糭���������� �糭��Ȳ �߻��� ��Ʈ�� ��ũ��Ʈ
/// </summary>

public class DisasterManager : MonoBehaviour
{
    [Header("������Ʈ")]
    public GameObject mapPanel;
    public GameObject[] disasterPrefab;
    public GameObject goDisaster;

    [Header("���ӵ�����")]
    public float disasterCoolTime = 60f;
    public int minTimer = 40;
    public int maxTimer = 60;

    Coroutine disasterCoroutine = null;

    public void RetryGame()  //������ ����۵Ǹ� �糭���� �ڷ�ƾ �����
    {
        if(goDisaster != null) Destroy(goDisaster);
        if(disasterCoroutine != null) StopCoroutine(disasterCoroutine);
        goDisaster= null;
        disasterCoroutine = StartCoroutine(GenerateDisaster());
    }

    IEnumerator GenerateDisaster()
    {
        disasterCoolTime = 60f;
        while (!DisasterGameManager.instance.isGamePlay)
        { yield return null; }

        while (DisasterGameManager.instance.isGamePlay)
        {
            yield return new WaitForSeconds(disasterCoolTime - 10f);

            int index = Random.Range(0, disasterPrefab.Length);
            string name = null;
            float posX = 0f;
            Color _color= Color.white;
            ColorUtility.TryParseHtmlString("#FFFFFF", out _color);

            switch(index)
            {
                case 0: name = "Rain"; posX = 12f; DisasterGameManager.instance.DisasterGuideComment("ȣ��溸!",_color , 48, TextAlignmentOptions.Center,true); break;
                case 1: name = "Surge";posX = 0f; DisasterGameManager.instance.DisasterGuideComment("��������!", _color, 48, TextAlignmentOptions.Center, true); break;
                case 2: name = "Typhoon"; posX = 12f; DisasterGameManager.instance.DisasterGuideComment("��ǳ�溸�߷�!", _color, 48, TextAlignmentOptions.Center, true); break;
                case 3: name = "Earthquake"; posX = 0f; DisasterGameManager.instance.DisasterGuideComment("�Ը� 5.0 �̻��� ���� ����!", _color, 48, TextAlignmentOptions.Center, true); break;
            }

            yield return new WaitForSeconds(10f);

            DisasterCreate(index, name, posX);
            disasterCoolTime = (float)Random.Range((int)minTimer, (int)maxTimer);

            yield return new WaitForSeconds(2f);

            DisasterGameManager.instance.GuideComment();
        }

    }

    void DisasterCreate(int index, string name, float posX)    //�糭�� ������Ʈ ����
    {
        goDisaster = Instantiate(disasterPrefab[index], new Vector3(posX, 0f, 0f), Quaternion.identity);
        goDisaster.transform.SetParent(mapPanel.transform);
        goDisaster.transform.localScale = Vector3.one;
        goDisaster.transform.localPosition = Vector3.zero;
        goDisaster.name = name;
        switch (index)
        {
            case 0: SoundManager.instance.PlayEffectSound("eff_SmartFarm_rain", 1f);break;
            case 1: SoundManager.instance.PlayEffectSound("eff_Common_thunder", 1f);break;
            case 2: SoundManager.instance.PlayEffectSound("eff_Disaster_typhoon", 1f);break;
            case 3: SoundManager.instance.PlayEffectSound("eff_Airplane_shake", 1f);break;
        }
    }
}
