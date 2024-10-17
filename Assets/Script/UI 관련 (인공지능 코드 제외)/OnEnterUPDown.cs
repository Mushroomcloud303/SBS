using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;

public class OnEnterUPDown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private RectTransform rtf;
    [SerializeField] private Vector3 firstPo;

    private void Awake()
    {
        firstPo = rtf.position;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rtf.DOLocalMoveY(30, 0.1f).SetUpdate(true); // 0.1초 동안 30만큼 위로 이동
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rtf.DOLocalMoveY(0, 0.1f).SetUpdate(true); // 0.1초 동안 0으로 이동
    }
}
