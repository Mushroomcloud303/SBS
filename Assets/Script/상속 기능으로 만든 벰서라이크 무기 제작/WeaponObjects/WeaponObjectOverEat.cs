using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectOverEat : WeaponObject
{
    private Vector2 summonPoint;

    protected override void Init()
    {
    }

    protected override void WeaponMechanism()
    {
        summonPoint = GameManager.instance.playerLeader.left ? new Vector2(-1 , 2) : new Vector2(1, 2);
        GameObject bullet = GameManager.instance.poolManager.GetFromPoolBullet(weaponData.bulletPrefabID);
        bullet.transform.parent = transform;
        bullet.transform.position = transform.position;
        bullet.transform.localPosition = (Vector3)summonPoint;
        bullet.transform.localScale = new Vector2(weaponData.Size/6, 1);
        bullet.GetComponent<SpriteRenderer>().flipX = GameManager.instance.playerLeader.left;
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
