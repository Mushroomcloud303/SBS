using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CallKillTween : MonoBehaviour
{
    public Tween pathTween;
    public Tween rotationTween;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Debug.Log(collision.gameObject.layer);
        if(collision.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            rotationTween.Kill();
            pathTween.Kill();   
        }
    }
}
