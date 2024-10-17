using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public delegate void PositionReset();
    public static event PositionReset positionReset;
    public static GameManager instance;
    [Header("Game Control")]
    public bool isLive;
    public float gameTime;
    public float SpawnLvUpTime; // 이 시간이 될 때마다 스폰 레벨이 증가합니다.
    public float SpawnLvTimer; // 30초마다 레벨업 이런 걸 좀 더 원활하게 구현하기 위해 추가
    public float maxGameTime;
    public int floor; // 현재 층
    public int maxfloor; // 마지막 층 여기까지 가면 승리입니다!
    public SceneNav SceneNav;
    public bool escPossible;
    [Header("  씬들의 저장 어레이 \n 방향은 진행 가능이란 뜻으로 반대 방향이 뚤려있는 씬의 코드를 할당\n ex) UP은 down이 있는 맵이어야함")]
    public SceneAsset[] UpScenes;
    public SceneAsset[] DownScenes;
    public SceneAsset[] LeftScenes;
    public SceneAsset[] RightScenes;
    public Coroutine SceneLoadSequence;
    [Header("Player Info")]
    public PlayerStat presentStat;
    public float maxHp = 100;
    public float presentHp;
    public int level;
    public int killCount;
    public int exp;
    public int[] nextExp = {10, 30, 60, 120, 280};
    [Header("Game Object")]
    public PlayerLeader playerLeader;
    public PoolManager poolManager;
    public Spawner spawner;
    public Light2D GLight; // 글로벌 라이트
    public Light2D[] OtherLights; // 글로벌 라이트 외 다른 조명들을 모아놓을 배열
    public OpenAIConnecter openAIConnecter;
    [Header("Weapon")]
    public int[] weaponLevels; // 실제 사용될 레벨업 데이터
    public WeaponType[] weaponTypes; // 대화 출력에 사용될 무기 타입
    public WeaponMaster weaponMaster;
    [Header("Stress")]
    public int[] stress = {0,0,0};
    [Header("UIPanel")]
    public GameObject EscPanel;
    public LevelUp uiLevelUp;
    public Transform UICanvus;
    public GameObject uiMinimap;
    public GameObject uiResult;
    public DialogManager dialogManager;
    public GameObject storyPanel;
    public PromptingParameter promptingParameter;
    public InputField inputField;
    [SerializeField]
    private Text inputFieldPlaceholder;

    private WaitForSeconds umm = new WaitForSeconds(2f);
    
    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 144; // 최적화를 위해 프레임 제한
    }

    public void GameRetry() // 게임 재시작!
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single); // Build Setting 기준 0번 장면을 다시 불러온다.
        Resume();
    }

    public void GameOver() // 게임 오버~
    {
        //Debug.Log("죽으면 플레이어 데드 함수 활용");
        isLive = false; // isLive를 false로
        AudioManager.instance.PlayBgm(false); // 브금 꺼
        playerLeader.Dead(); // 플레이어 죽는 애니메이션 실행
    }

    public void Result()
    {
        //결과창 띄우기 플레이어 사망 애니메이션 끝나면
        promptingParameter.SetParameterWithGameManager();
        Debug.Log("엔딩 생성중");
        openAIConnecter.SendMessageToChatGPT(promptingParameter.userReason);
        StartCoroutine(GameResultSequence());

    }

    private IEnumerator GameResultSequence()
    {
        yield return StartCoroutine(openAIConnecter.PostRequest(promptingParameter.userReason));
        storyPanel.SetActive(true);
        yield return StartCoroutine(dialogManager.ShowDialog(promptingParameter.ReturnPromptingResutToTensor()));
        uiResult.SetActive(true);
        uiResult.GetComponent<Result>().Lose();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lose); // 패배 효과음 재생
    }

    public void GameVictory() // 게임 승리!
    {
        StartCoroutine(GameVictoryRoutine()); // 게임 승리 코루틴 호출
    }

    IEnumerator GameVictoryRoutine() // 게임 승리 액션!
    {
        isLive = false; // isLive를 false로

        yield return new WaitForSeconds(0.5f); // 1초 딜레이

        uiResult.gameObject.SetActive(true); // 결과창 활성화
        uiResult.GetComponent<Result>().Win(); // Result 스크립트에서 Win 불러와 승리 UI 활성화
        TimeStop(); // 게임 멈춰

        AudioManager.instance.PlayBgm(false); // 브금 꺼
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Win); // 승리 효과음 재생
    }

    public void GameStart()
    {
        escPossible = false;
        TimeStop();
        presentHp = maxHp;
        promptingParameter.Init();
        AudioManager.instance.EffectBgm(false); // BGM 흐려짐 효과 해제
        AudioManager.instance.PlayBgm(true); // 브금 켜
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // 선택 효과음 재생
        storyPanel.SetActive(true);
        inputFieldPlaceholder.text = "이름을 입력해주세요";
        inputField.gameObject.SetActive(true);
        inputField.onEndEdit.AddListener(ShowStroyAfterName);
    }

    private void ShowStroyAfterName(string input)
    {
        promptingParameter.userName = input;
        inputField.gameObject.SetActive(false);
        inputField.text = "";
        Debug.Log("이름 입력 완료");
        StartCoroutine(GameStartSequence());
    }

    private void ShowStroyAterReson(string input)
    {
        promptingParameter.userReason = input;
        inputField.gameObject.SetActive(false);
        inputField.text = "";
    }

    IEnumerator GameStartSequence()
    {
        Debug.Log("게임 시작");
        yield return null;
        //시작 대화 출력
        yield return StartCoroutine(dialogManager.ShowDialog(DialogStore.instance.dialogList[DialogType.Start]));
        //인풋 필드 리스너 함수 제거
        inputField.onEndEdit.RemoveAllListeners();
        //인풋 필드 리스너 함수 추가
        inputField.onEndEdit.AddListener(ShowStroyAterReson);
        inputFieldPlaceholder.text = "무엇이 당신을 가장 힘들게 하였습니까?";
        inputField.gameObject.SetActive(true);
        //이유 세팅 안하면 못 지나감
        while (promptingParameter.userReason == "")
        {
            yield return null;
        }
        storyPanel.SetActive(false);
        //여기까지가 대화
        // StartMap 씬을 비동기적으로 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("StartMap", LoadSceneMode.Additive);
        // 씬이 완전히 로드될 때까지 대기
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        //Debug.Log(SceneManager.GetSceneByName("StartMap").GetRootGameObjects()[0]);
        SceneTree start = SceneManager.GetSceneByName("StartMap").GetRootGameObjects()[0].GetComponentInChildren<SceneTree>();
        start.Init(SceneManager.GetSceneByName("StartMap"));
        StartCoroutine(start.StartSequence());
        Resume();
        yield return umm;
        yield return StartCoroutine(uiLevelUp.LevelUPSequence());
        escPossible = true;
        Resume();
    }

    private void Update()
    {
        // esc 기능 구현
        if (Input.GetKeyDown(KeyCode.Escape) && escPossible)
        {
            if(EscPanel.activeSelf)
            {
                EscPanel.SetActive(false);
                Resume();
            }
            else
            {
                EscPanel.SetActive(true);
                TimeStop();
            }
        }
        if (!isLive)
            return;
        gameTime += Time.deltaTime;
        SpawnLvTimer += Time.deltaTime;

        //밤 없앨거라 삭제
        //if (gameTime > maxGameTime)
        //{
        //    gameTime = 0;
        //    // 낮밤 체인지
        //    if (night)
        //        night = false;
        //    else
        //    {
        //        night = true;
        //        spawner.SpawnElite(0);
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.M)) // 미니맵 토글
        {
            uiMinimap.SetActive(!uiMinimap.activeInHierarchy);
        }

        if (SpawnLvTimer > SpawnLvUpTime && spawner.level < spawner.spawnDataD.Length - 1) // 스폰 레벨 타이머가 40초가 지났고 스폰 레벨이 최대 레벨보다 작으면
        {
            SpawnLvTimer = 0;
            spawner.level++; // 스폰 레벨 업
        }
        // 밤 없앨거라 삭제
        //if(night) // night가 true, 즉 밤이면
        //{
        //    if (GLight.intensity >= 0) // 글로벌 라이트 밝기가 0이 될 때까지 점점 어둡게 만든다.
        //        GLight.intensity -= 0.01f;

        //    foreach (var light in OtherLights)
        //    {
        //        if (light.intensity < 1)
        //            light.GetComponent<Light2D>().intensity += 0.01f; // 다른 조명은 밝기가 1이 될 때까지 점점 밝게 만든다.
        //    }
        //}
        //else // 아니면, 즉 아침이면
        //{
        //    if (GLight.intensity < 1) // 글로벌 라이트 밝기가 1이 될 때까지 점점 밝게 만든다.
        //        GLight.intensity += 0.01f;

        //    foreach (var light in OtherLights)
        //    {
        //        if (light.intensity > 0)
        //            light.GetComponent<Light2D>().intensity -= 0.01f; // 다른 조명은 밝기가 0이 될 때까지 점점 어둡게 만든다.
        //    }
        //}
    }

    //public void NextLevel() // 다음 층으로 이동
    //{
    //    Debug.Log("다음 층으로 이동");
    //    poolManager.DisableAllObject();
    //    floor++;
    //    if (floor >= maxfloor)
    //    {
    //        GameVictory();
    //        return;
    //    }
    //    MapGenerator.instance.GenerateMap();
    //}

    public void MovePlayerstartpoint(Vector2Int startPoint)
    {
        playerLeader.transform.position = new Vector3(startPoint.x, startPoint.y, 0);
        // positionReset();
    }

    public IEnumerator GetExp()
    {
        exp++;
        while(uiLevelUp.levelUpOn)
        {
            Debug.Log("좀만 기달여");
            yield return null;
        }
        if(exp >= nextExp[Mathf.Min(level, nextExp.Length - 1)])
        {
            Debug.Log("레벨업!");
            exp -= nextExp[Mathf.Min(level, nextExp.Length - 1)];
            level++;
            yield return StartCoroutine(uiLevelUp.LevelUPSequence());
        }
    }

    public void TimeStop()
    {
        isLive = false;
        uiMinimap.SetActive(false);
        Time.timeScale = 0;
    }

    public void Resume()
    {
        isLive = true;
        uiMinimap.SetActive(true);
        Time.timeScale = 1;
    }

}
