using System.Collections;
using UnityEngine;

/// <summary>
/// 우주비행사
/// 발사 모드의 우주선 컨트롤스크립트
/// </summary>

public class SpaceLauncherShipController : MonoBehaviour
{
    private SpaceLauncherGameController launcherCtrl;

    [Header("게임오브젝트")]
    private Transform tr;
    private Transform targetTr;
    public GameObject[] shipParts;
    // 0 = 파츠3의 트레일파티클, 1 = 파츠3의 불꽃 파티클, 2 = 파츠2의 불꽃 파티클, 3 = 파츠1의 불꽃 파티클
    public GameObject[] fireParticles;  //파츠별 파티클

    [Header("게임데이터")]
    private Vector3 targetPos= Vector3.zero;
    private Vector3 finishPos = Vector3.zero;
    public float moveSpeed = 0.1f;
    public bool[] isLaunchLevel = new bool[3];


    private void Start()
    {
        launcherCtrl = transform.parent.GetComponent<SpaceLauncherGameController>();
        SpaceShipSetup();
    }

    void SpaceShipSetup()  //우주선 데이터 셋업
    {
        tr = transform;
        targetTr = transform.parent.transform;
        targetPos = new Vector3(targetTr.position.x, (targetTr.position.y - 1f), targetTr.position.z);
        finishPos = new Vector3(targetTr.position.x, (targetTr.position.y + 8f), targetTr.position.z);

        for (int i = 0; i < fireParticles.Length; i++)  //불꽃파티클 비활성화
        {
            fireParticles[i].SetActive(false);
        }

        for (int i = 0; i < isLaunchLevel.Length; i++)  //우주선 분리 단계
        {
            isLaunchLevel[i] = false;
        }
    }


    #region 우주선 발사
    public void LaunchAction()  //발사 동작
    {
        StartCoroutine(ShakeAction());
    }

    IEnumerator ShakeAction()  //우주선의 흔들림
    {
        Vector3 resetPos = transform.position;
        SoundManager.instance.PlayEffectSoundLoop("eff_Space_fly", 1f);

        fireParticles[0].SetActive(true);

        //우주선 떨림 효과
        float coolTime = 3f;
        while (coolTime > 0)
        {
            tr.position = Random.insideUnitSphere * 0.02f + resetPos;
            coolTime -= Time.deltaTime;
            yield return null;
        }

        SoundManager.instance.PlayEffectSound("eff_Space_fire", 1f);
        transform.position = resetPos;
        fireParticles[1].SetActive(true);

        launcherCtrl.isBgMove= true;
        isLaunchLevel[0] = true;
        yield return new WaitForSeconds(8);

        //우주선 위치 이동
        while (transform.position != targetPos)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetTr.position, moveSpeed * Time.deltaTime);

            if (this.transform.position.y - targetPos.y > -0.2f) break;
            yield return null;
        }
        fireParticles[0].SetActive(false);

    }

    #endregion


    #region 우주선 분리 성공/실패

    public void SeparatePart1()  //우주선 하부체1 분리
    {

        if (isLaunchLevel[0] && !isLaunchLevel[1])
        {
            shipParts[2].GetComponent<Rigidbody2D>().gravityScale = 1;
            fireParticles[1].SetActive(false);
            fireParticles[2].SetActive(true);
            isLaunchLevel[1] = true;
            launcherCtrl.separateButtons[0].interactable = false;
        }
        SoundManager.instance.PlayEffectSound("eff_Space_divide", 1f);
    }

    public void SeparatePart2()  //우주선 하부체2 분리
    {
        if (isLaunchLevel[1] && !isLaunchLevel[2])
        {
            shipParts[1].GetComponent<Rigidbody2D>().gravityScale = 1;
            fireParticles[2].SetActive(false);
            fireParticles[3].SetActive(true);
            isLaunchLevel[2] = true;
            launcherCtrl.separateButtons[1].interactable = false;
        }
        SoundManager.instance.PlayEffectSound("eff_Space_divide", 1f);
    }


    public void FinishMove()  //미션 성공 후 우주선 화면 밖으로 보내기
    {
        StartCoroutine(ShipFinishMove());
    }

    IEnumerator ShipFinishMove()
    {
        while (transform.position.y < 8f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, finishPos, Time.deltaTime);

            if (this.transform.position.y > 7f) break;
            yield return null;
        }

        //도킹 게임으로 이동
        yield return new WaitForSeconds(2f);
        SpaceGameManager.instance.RendezvousDockingGameSetup();
        Destroy(this.gameObject);
    }

    public void FailSeparateParts()  //우주선 하부체 분리 실패
    {
        StopAllCoroutines();
        SoundManager.instance.StopEffSoundLoop();
        StartCoroutine(FailPanelOn());
    }

    IEnumerator FailPanelOn()  //실패 결과창 활성화
    {
        yield return new WaitForSeconds(1);
        //불꽃파티클 비활성화
        for (int i = 0; i < fireParticles.Length; i++)
        {
            fireParticles[i].SetActive(false);
        }
        
        yield return new WaitForSeconds(1);
        //우주선 오브젝트 중력 적용
        this.transform.GetComponent<Rigidbody2D>().gravityScale = 1;
        for(int i = 0; i < shipParts.Length; i++)
        {
            shipParts[i].GetComponent<Rigidbody2D>().gravityScale = 1;
        }

        //화면 아래로 떨어지면 실패 결과창 활성화
        while (this.transform.position.y > -8f)
        {
            this.transform.Rotate(Vector3.forward, 90f * Time.deltaTime);
            yield return null;
        }

        SpaceGameManager.instance.FailLauncherMission();
    }

    #endregion

    private void OnDisable()  //오브젝트가 비활성화될 때 코루틴 멈추기
    {
        StopAllCoroutines();
    }
}
