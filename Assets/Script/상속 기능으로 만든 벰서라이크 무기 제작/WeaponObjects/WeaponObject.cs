using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class WeaponObject: MonoBehaviour
{
    public WeaponData weaponData; // 무기 정보
    protected WaitForSeconds delay;
    protected bool isAttacking = false;
    protected float time = 0;
    protected GameObject myBullet;
    protected RaycastHit2D[] targets;
    public int index;

    //필수 구현 함수
    protected abstract void Init();// 초기화
    protected abstract void WeaponMechanism();// 무기의 기능

    //기능함수
    protected void CallPerFrame()
    {
        if (isAttacking || GameManager.instance.weaponLevels[index] == 0 ) // 레벨 딸리면 리턴 / 공격중이면 리턴
            return;
        time += Time.deltaTime;
        if (time >= weaponData.Rapid)
        {
            StartCoroutine(DelayAttack(delay));
            time = 0;
        }
    } //쿨타임 쓰고 싶을때 , 다다다닥 쏘고 싶을때

    protected void CallPerFrameNoDelay() // 쿨타임 쓰고 싶을때, 탕 쏘고 싶을때
    {
        if (isAttacking || GameManager.instance.weaponLevels[index] == 0) // 레벨 딸리면 리턴 / 공격중이면 리턴
            return;
        time += Time.deltaTime;
        if (time >= weaponData.Rapid)
        {
            WeaponMechanism();
            time = 0;
        }
    }

    protected IEnumerator DelayAttack(WaitForSeconds delay)
    {
        time = 0;
        isAttacking = true;
        for (int i = 0; i < (int)weaponData.Amount; i++)
        {
            WeaponMechanism();
            yield return delay;
        }
        isAttacking = false;
    }// 위에 함수랑 같이 씀

    protected void InstantiatePermernantly()
    {
        myBullet = GameManager.instance.poolManager.GetFromPoolBullet(weaponData.bulletPrefabID);
        myBullet.transform.parent = transform;
        myBullet.transform.position = Vector2.zero;
    }

    protected void ShotToTargets(Vector3 targetPositon)
    {
        if (targetPositon == Vector3.zero)
            return;
        Vector3 direction = targetPositon - transform.position;
        direction.Normalize();
        GameObject bullet = GameManager.instance.poolManager.GetFromPoolBullet(weaponData.bulletPrefabID);
        bullet.transform.position = transform.position;
        bullet.GetComponent<Damage>().data = weaponData;
        bullet.GetComponent<Rigidbody2D>().velocity = direction * weaponData.Speed;
    }

    protected Vector3 GetNearTarget()
    {
        if (GameManager.instance.playerLeader.GetComponent<Scanner>().nearestTarget == null)
        {
            //Debug.Log("No Target");
            return Vector3.zero;
        }    
        Vector3 targetPosition = GameManager.instance.playerLeader.GetComponent<Scanner>().nearestTarget.position;
        return targetPosition;
    }

    protected GameObject SummonAtPoint(Vector3 position)
    {
        if (position == Vector3.zero)
            return null;
        GameObject bullet = GameManager.instance.poolManager.GetFromPoolBullet(weaponData.bulletPrefabID);
        bullet.transform.position = position;
        bullet.GetComponent<Damage>().data = weaponData;
        bullet.transform.position = position;
        return bullet;
    }

    protected Vector3 GetRandomTarget()
    {
        //죽어도 그 자리에 소환될 수 있게 하기
        targets = GameManager.instance.playerLeader.GetComponent<Scanner>().targets;
        if (targets.Length == 0)
            return Vector3.zero;
        Vector3 randomPosition = targets[Random.Range(0, targets.Length)].transform.position;
        return randomPosition;
    }

    protected Vector3 GetRandomPosition(float range)
    {
        return new Vector2(Random.Range(-range, range), Random.Range(-range, range));
    }
}
