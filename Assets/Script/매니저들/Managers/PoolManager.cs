using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // 프리팹을 보관할 변수
    public GameObject[] prefabs;
    public GameObject[] bulletPrefabs;

    // 풀 담당 리스트
    private List<GameObject>[] pool;
    private List<GameObject>[] poolBullet;

    private void Awake()
    {
        //풀의 길이는 프리팹 종류의 갯수
        pool = new List<GameObject>[prefabs.Length];
        poolBullet = new List<GameObject>[bulletPrefabs.Length];

        for (int i = 0; i < prefabs.Length; i++)
        {
            //풀 리스트에 프리팹의 종류만큼 리스트를 생성
            pool[i] = new List<GameObject>();
        }
        for (int i = 0; i < bulletPrefabs.Length; i++)
        {
            //풀 리스트에 프리팹의 종류만큼 리스트를 생성
            poolBullet[i] = new List<GameObject>();
        }
    }

    public void DisableAllObject()
    {
        //풀의 모든 오브젝트 비활성화
        for (int i = 0; i < pool.Length; i++)
        {
            foreach (GameObject item in pool[i])
            {
                if(!item.CompareTag("UI"))
                {
                    item.SetActive(false);
                }            
            }
        }
    }

    public GameObject GetFromPoolBullet(int index)
    {
        GameObject obj = null; // 반환할 객체

        //index 풀을 순회하여 비활성화 오브젝트 감지
        foreach (GameObject item in poolBullet[index])
        {
            if (!item.activeSelf)
            {
                obj = item;
                obj.SetActive(true);//비활성화 오브젝트 재활용
                break;
            }
        }
        //비활성화 오브젝트 없으면 생성
        if (obj == null)
        {
            obj = Instantiate(bulletPrefabs[index], transform);
            poolBullet[index].Add(obj);
        }

        return obj;//반환
    }

    public GameObject GetFromPool(int index)
    {
        GameObject obj = null; // 반환할 객체
        if(pool.Length > 300) // 스폰 제한
        {
            return null;
        }
        //index 풀을 순회하여 비활성화 오브젝트 감지
        foreach (GameObject item in pool[index])
        {
            if (!item.activeSelf)
            {
                obj = item;
                obj.SetActive(true);//비활성화 오브젝트 재활용
                break;
            }
        }
        //비활성화 오브젝트 없으면 생성
        if (obj == null)
        {
            obj = Instantiate(prefabs[index], transform);   
            pool[index].Add(obj);
        }
        

        return obj;//반환
    }

}
