using System.Collections;
using UnityEngine;

/// <summary>
/// 재난관리전문가 재난(천재지변) 컨트롤 스크립트
/// </summary>

[System.Serializable]
public class DisasterData
{
    public string disasterName; //재난의 이름
    public float moveSpeed = 1f;//재난 오브젝트 이동 속도
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

    void DisasterDataInit()   //재난별 데이터 설정
    {
        switch (disasterData.disasterName)
        {
            case "Rain": StartCoroutine(RainMove()); disasterData.moveSpeed = 1f; break;
            case "Surge": StartCoroutine(SurgeMove());  break;
            case "Typhoon": StartCoroutine(TyphoonMove()); disasterData.moveSpeed = 3f; break;
            case "Earthquake": StartCoroutine(EarthquakeMove()); break;
        }
    }

    IEnumerator RainMove()   //호우 로직
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

    IEnumerator SurgeMove()   //낙뢰 로직
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

    IEnumerator TyphoonMove()   //태풍 로직
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

    IEnumerator EarthquakeMove()   //지진 로직
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
