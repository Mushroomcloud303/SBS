using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements; 혹시 모르니까 여기서 이벤트 시스템 쓴다니까 쓸거면 주석 풀어놓기

public class LevelUpButton : MonoBehaviour
{
    [SerializeField] public int attackIndex;
    [SerializeField] private Text itemName;
    [SerializeField] private Image icon;
    [SerializeField] private Text level;
    [SerializeField] private Text description;
    [SerializeField] private Text stress;
    [SerializeField] private RectTransform rtf;
    [SerializeField] private Vector3 firstPo;

    private void Awake()
    {
        rtf = GetComponent<RectTransform>();
        
    }

    private void Start()
    {
        UpdateInfo();
    }

    private void UpdateInfo()
    {
        WeaponData weaponData = GameManager.instance.weaponMaster.weaponObjects[attackIndex].weaponData;
        itemName.text = weaponData.weaponName;
        level.text = "Lv. " + GameManager.instance.weaponLevels[attackIndex];
        description.text = weaponData.WeaponDescription; // get set 구문으로 필요한 설명을 받이옴 필요한 함수는 데이터에서 구현되어있음
        icon.sprite = weaponData.weaponIcon;
    }

    public void LevelUpMyAttack()
    {
        //Debug.Log("LevelUpMyAttack실행됨" + attackIndex);
        GameManager.instance.weaponLevels[attackIndex]++;
        GameManager.instance.uiLevelUp.buttonClicked = true;
        GameManager.instance.uiLevelUp.attackIndex = attackIndex;
        UpdateInfo();
    }

}
