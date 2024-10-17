using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponObjectMediation : WeaponObject
{

    protected override void Init()
    {
        InstantiatePermernantly();
    }

    protected override void WeaponMechanism()
    {
        myBullet.SetActive(GameManager.instance.weaponLevels[index] != 0);
        myBullet.transform.localScale = Vector2.one * (weaponData.Size /2) ;
    }

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        WeaponMechanism();
    }
}
