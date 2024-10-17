using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TearUpButtonStudy : TearUpButton
{
    public override void AfterTearUp()
    {
        GameManager.instance.Resume();
    }
}
