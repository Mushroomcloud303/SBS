using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Spawner : MonoBehaviour
{
    public List<SceneTree> enemySpawnPoints;
    public bool positionSetting = false;
    public SpawnData[] spawnDataD; // 낮에 스폰될 몹 데이터
    public SpawnData[] spawnDataN; // 밤에 스폰될 몹 데이터
    public SpawnData[] spawnDataE; // 엘리트몹 데이터

    private float timer = 0;
    [SerializeField] public int level;


    private void Update()
    {
        if (!GameManager.instance.isLive || positionSetting)
            return;
        timer += Time.deltaTime;
        // level = Mathf.Min( Mathf.FloorToInt(GameManager.instance.gameTime/10f), spawnDataD.Length-1); // 10초마다 레벨업, 최대 레벨은 spawnData의 길이

        if(timer > spawnDataD[level].spawnTime)
        {
            timer = 0;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {    
        GameObject enemy = GameManager.instance.poolManager.GetFromPool(0);
        int index = Random.Range(0, enemySpawnPoints.Count);
        enemy.GetComponent<Enemy>().SetPosition(enemySpawnPoints[index].transform);
        //enemySpawnPoints[index].enemys.Add(enemy.GetComponent<Collider2D>());
        //Debug.Log("적 씬에 배치 완료!" + enemySpawnPoints[index]);
        enemy.GetComponent<Enemy>().Init(spawnDataD[level]);

    }

    public void SpawnElite(int mobnum)
    {
        Debug.Log("엘리트 몹이 나타납니다!");
        GameObject enemy = GameManager.instance.poolManager.GetFromPool(0);
        int index = Random.Range(0, enemySpawnPoints.Count);
        enemy.GetComponent<Enemy>().SetPosition(enemySpawnPoints[index].transform);
        /*
        float x1 = GameManager.instance.playerLeader.transform.position.x + Random.Range(-5, 5);
        float y1 = GameManager.instance.playerLeader.transform.position.y + Random.Range(-5, 5);
        enemy.GetComponent<NavMeshAgent>().Warp(new Vector3(x1,y1,0));
        */

        enemy.GetComponent<Enemy>().Init(spawnDataE[mobnum]);
    }
}

[System.Serializable]
public class SpawnData
{
    public float spawnTime;
    public int spriteType;
    public float health;
    public float speed;
    public int nightOnly;
    public float damage;
    public bool isElite;
}
