using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class TearUpButton : MonoBehaviour
{
    public int targetIndex;
    public int tearUpIndex;
    public Text weaponName;

    public void TearUpWeapon()
    {       
        AfterTearUp();
    }

    abstract public void AfterTearUp();
}
