using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class WeaponObjectSmoke : WeaponObject
{
    [SerializeField] private float height;
    protected override void Init()
    {
        delay = new WaitForSeconds(weaponData.SummonDelay);
    }

    protected override void WeaponMechanism()
    {
        time = 0;
        StartCoroutine(DelayAttackPrivate(delay));
    }

    IEnumerator DelayAttackPrivate(WaitForSeconds delay)
    {
        isAttacking = true;

        for (int i = 0; i < (int)weaponData.Amount; i++)
        {
            Fire();
            yield return delay;
        }
        isAttacking = false;
    }

    private void Fire()
    {
        Vector2 target = GetRandomTarget();
        if (target == Vector2.zero)
            return;
        GameObject bullet = GameManager.instance.poolManager.GetFromPoolBullet(weaponData.bulletPrefabID);
        bullet.transform.position = new Vector2(target.x, target.y + height*weaponData.Speed);
        bullet.transform.localScale = Vector2.one*(weaponData.Size/2); // 이런 상수값은 스프라이트와 실 크기가 달라서 보정해주는 것
        bullet.transform.DOMoveY(target.y, weaponData.Speed).SetEase(Ease.OutSine).OnComplete(() => bullet.GetComponentInChildren<Animator>().SetTrigger("Explode"));
    }

    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        CallPerFrameNoDelay();
    }

}
