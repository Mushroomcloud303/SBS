using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectSelfharm : WeaponObject
{
    protected override void Init()
    {
        delay = new WaitForSeconds(weaponData.SummonDelay);
    }

    protected override void WeaponMechanism()
    {
        GameObject bullet = SummonAtPoint(GetNearTarget());
        if (bullet == null)
            return;
        bullet.transform.localScale = new Vector2(weaponData.Size/3, 1);
    }

    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        CallPerFrame();
    }
}
