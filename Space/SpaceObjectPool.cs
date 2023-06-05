using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 우주비행사 장애물 오브젝트풀링 스크립트
/// 유튜브 골드메탈 참고
/// https://www.youtube.com/watch?v=A7mfPH8jyBE
/// </summary>
public class SpaceObjectPool : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;
    List<GameObject>[] obstaclePools;

    List<GameObject> childList = new List<GameObject>();

    private void Awake()
    {
        obstaclePools = new List<GameObject>[obstaclePrefabs.Length];
        for(int i=0;i<obstaclePools.Length;i++)
        {
            obstaclePools[i] = new List<GameObject>();
        }
    }

    public GameObject GetObstacle(int index, Transform tr)
    {
        GameObject obstacle = null;

        foreach(GameObject obstaclePool in obstaclePools[index])
        {
            if (!obstaclePool.activeSelf)
            {
                obstacle = obstaclePool;
                obstacle.SetActive(true);
                break;
            }
        }

        //활성화할 수 있는 오브젝트가 없을 경우 새로 생성하고 풀 리스트에 추가하기
        if (!obstacle)  //GameObject 가 null 인지 아닌지 구분은 ! 로도 확인 가능 (!obstacle 오브젝트가 null 이라면.. 이라는 뜻)
        {
            obstacle = Instantiate(obstaclePrefabs[index], transform);
            obstaclePools[index].Add(obstacle);
        }

        return obstacle;
    }
}
