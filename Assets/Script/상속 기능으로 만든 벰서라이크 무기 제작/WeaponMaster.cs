using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMaster : MonoBehaviour
{
    public WeaponObject[] weaponObjects;

    private void Awake()
    {
        int i = 0 ;
        weaponObjects = GetComponentsInChildren<WeaponObject>();//무조건 대화의 순서대로 무기를 배열해야함 그래야 대사가 정확히 나옴
        GameManager.instance.weaponLevels = new int[weaponObjects.Length];
        GameManager.instance.weaponTypes = new WeaponType[weaponObjects.Length];
        foreach(WeaponObject weaponObject in weaponObjects)
        {
            weaponObject.index = i;
            i++;
        } 
        GameManager.instance.uiLevelUp.Init();
    }

}
