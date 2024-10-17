using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;
using DG.Tweening;
using System;

public class LevelUp : MonoBehaviour
{
    RectTransform myRect;
    int rerollChance;
    Text rerollText;
    [SerializeField] private Transform buttonPanel;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject[] buttons; 

    public bool levelUpOn; // 레벨업 중인지 확인하는 변수
    private bool levelUpPanelOn; // 레벨업 패널이 켜져있는지 확인하는 변수
    public bool buttonClicked; // 버튼이 클릭되었는지 확인하는 변수
    public int attackIndex; // 공격력을 올릴 무기의 인덱스

    private void Awake()
    {
        myRect = GetComponent<RectTransform>();
        rerollText = transform.GetChild(1).GetComponentsInChildren<Text>()[0];
    }

    public void Init()
    {
        //Debug.Log("Init실행됬네?");
        buttons = new GameObject[GameManager.instance.weaponMaster.weaponObjects.Length];
        for(int i = 0; i < GameManager.instance.weaponMaster.weaponObjects.Length; i++)
        {
            buttons[i] = Instantiate(buttonPrefab, buttonPanel);
            buttons[i].GetComponentInChildren<LevelUpButton>().attackIndex = i;
            //buttons[i].GetComponentInChildren<Button>().onClick.AddListener(() => Hide()); // 하이드는 여기꺼라 여기서 설정 해줘야 하므
            buttons[i].SetActive(false);
        }
    }

    public void Show()
    {
        //detween을 이용해 스케일을 0에서 1로 변경
        GameManager.instance.TimeStop();
        levelUpPanelOn = true;
        DOTween.Kill(myRect);
        myRect.DOScale(Vector3.one, 0.2f).SetUpdate(true);
        rerollChance = 300;
        NextButton();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp); // 레벨업 효과음 재생
        AudioManager.instance.EffectBgm(true); // BGM 흐려짐 효과
    }

    public void Hide()
    {
        //Debug.Log("Hide재실행 작동됨");
        GameManager.instance.Resume();
        myRect.DOScale(Vector3.zero, 0.5f).SetUpdate(true)
            .OnComplete(() => levelUpPanelOn = false);
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // 선택 효과음 재생
        AudioManager.instance.EffectBgm(false); // BGM 흐려짐 효과 해제
    }

    public IEnumerator LevelUPSequence()
    {
        GameManager.instance.TimeStop();
        levelUpOn = true;
        buttonClicked = false;
        Show();
        while (!buttonClicked)
        {
            //Debug.Log("버튼 입력 대기중");
            yield return null;
        }
        Hide();
        while (!levelUpPanelOn)
        {
            Debug.Log("패널이 꺼질때까지 대기중");
            yield return null;
        }
        if (GameManager.instance.weaponLevels[attackIndex] == 1)
        {
            GameManager.instance.TimeStop();
            yield return StartCoroutine(GameManager.instance.dialogManager.ShowDialog(DialogStore.instance.dialogList[(DialogType)(4 + attackIndex)])); // 4는 대화 타입중 무기 레벨업 대화 시작 인덱스 고려한검
        }
        else if (GameManager.instance.weaponLevels[attackIndex] == 11) // 11은 최대레벨
        {
            GameManager.instance.TimeStop();
            yield return StartCoroutine(GameManager.instance.dialogManager.ShowDialog(DialogStore.instance.dialogList[(DialogType)(4 + attackIndex + 9)])); // 9는 무기 최대 레벨 대화 시작 인덱스 고려한검
        }
        GameManager.instance.Resume();
        levelUpOn = false;
    }

    public void Reroll()
    {
        if (rerollChance > 0)
        {
            rerollChance -= 1;
            NextButton();
        }
    }

    public void NextButton()
    {
        foreach(GameObject button in buttons)
        {
            button.SetActive(false);
        }
        for( int i = 0; i < 3 ; i++)
        {
            while(true)
            {
                int random = UnityEngine.Random.Range(0, buttons.Length);
                if (!buttons[random].activeSelf && GameManager.instance.weaponLevels[random] <= 11)
                {
                    buttons[random].SetActive(true);
                    break;
                }
            }
        }
        rerollText.text = string.Format("리롤하기 (남은 기회 : " + rerollChance.ToString() + "회)");
    }
        
}
