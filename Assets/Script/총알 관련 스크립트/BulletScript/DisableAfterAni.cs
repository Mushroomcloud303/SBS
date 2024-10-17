using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterAni : MonoBehaviour
{
    private void DisableAfterEffect()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
