using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectTablet : WeaponObject
{
    
    protected override void Init()
    {
        delay = new WaitForSeconds(weaponData.SummonDelay);
    }

    protected override void WeaponMechanism()
    {
        delay = new WaitForSeconds(weaponData.SummonDelay);
        ShotToTargets(GetNearTarget()); 
    }

    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if(GameManager.instance.isLive == false)
            return;
        CallPerFrame();
    }
}
