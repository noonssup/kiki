using System.Collections;
using UnityEngine;

/// <summary>
/// �糭���������� �糭(õ������) ��Ʈ�� ��ũ��Ʈ
/// </summary>

[System.Serializable]
public class DisasterData
{
    public string disasterName; //�糭�� �̸�
    public float moveSpeed = 1f;//�糭 ������Ʈ �̵� �ӵ�
    public float disasterPower=2f;

}

public class DisasterObject : MonoBehaviour
{
    public DisasterData disasterData = new DisasterData();

    private void Start()
    {
        disasterData.disasterName= this.gameObject.name;
        DisasterDataInit();
    }

    void DisasterDataInit()   //�糭�� ������ ����
    {
        switch (disasterData.disasterName)
        {
            case "Rain": StartCoroutine(RainMove()); disasterData.moveSpeed = 1f; break;
            case "Surge": StartCoroutine(SurgeMove());  break;
            case "Typhoon": StartCoroutine(TyphoonMove()); disasterData.moveSpeed = 3f; break;
            case "Earthquake": StartCoroutine(EarthquakeMove()); break;
        }
    }

    IEnumerator RainMove()   //ȣ�� ����
    {
        if(DisasterGameManager.instance.playTime <= 300f)
        {
            disasterData.disasterPower = 50f;
        }
        else { disasterData.disasterPower = Random.Range(50f, 100f); }

        float coolTime = 3f;
        while (coolTime > 0)
        {
            coolTime -= Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    IEnumerator SurgeMove()   //���� ����
    {
        //int range = Random.Range(1, 3);
        if (DisasterGameManager.instance.playTime <= 300f)
        {
            disasterData.disasterPower = 50f;
        }
        else { disasterData.disasterPower = Random.Range(50f, 100f); }

        float coolTime = 3f;

        while (coolTime> 0)
        {
            coolTime-= Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    IEnumerator TyphoonMove()   //��ǳ ����
    {
        if (DisasterGameManager.instance.playTime <= 300f)
        {
            disasterData.disasterPower = 50f;
        }
        else { disasterData.disasterPower = Random.Range(50f, 100f); }

        float coolTime = 3f;

        while (coolTime > 0)
        {
            coolTime -= Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }

    IEnumerator EarthquakeMove()   //���� ����
    {
        var map = this.transform.parent.transform;
        Vector3 resetPos = map.transform.position;

        if (DisasterGameManager.instance.playTime <= 300f)
        {
            disasterData.disasterPower = 50f;
        }
        else { disasterData.disasterPower = Random.Range(50f, 100f); }

        float coolTime = 3f;

        while (coolTime > 0)
        {
            map.position = Random.insideUnitSphere * 0.2f + resetPos;
            coolTime -= Time.deltaTime;
            yield return null;
        }
        map.transform.position = resetPos;
        Destroy(this.gameObject);
    }

}
