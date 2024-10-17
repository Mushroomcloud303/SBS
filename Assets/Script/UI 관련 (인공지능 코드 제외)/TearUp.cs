using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TearUp : MonoBehaviour
{
    public WeaponObject targetObject;
    public int targetIndex;
    public int tearUpIndex;
    [SerializeField]private GameObject[] tearUpButtons;


    public void Show()
    {
        transform.DOScale(Vector3.one, 0.3f)
            .OnComplete(() => GameManager.instance.TimeStop());
    }

    public void Hide()
    {
        transform.DOScale(Vector3.zero, 0.5f);
        GameManager.instance.Resume();
    }

}
