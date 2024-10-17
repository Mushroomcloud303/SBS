using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

abstract public class SceneTree : MonoBehaviour
{
    public Scene myScene;
    public Vector2 currentPosition;
    public Collider2D col;
    public GameObject tileMaps;
    public SceneTree originScene;
    public SceneTree leftTree;
    public SceneTree rightTree;
    public SceneTree upTree;
    public SceneTree downTree;

    public void Init(Scene originScene) // 시작 씬에서 호출해줄 함수 기본적으로 4방향으로 씬을 랜덤으로 설정
    {
        myScene = originScene;
    }

    protected void Awake()
    {
        tileMaps = transform.parent.gameObject;
        col = GetComponent<Collider2D>();
        col.enabled = false;
    }

    protected void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
        {
            //Debug.Log("플레이어가 아님");
            return;
        }
        col.enabled = false; // 경계에서 비비면 란란루나는거 막기
        try
        {
            StopCoroutine(GameManager.instance.SceneLoadSequence); // 비비면 란란루나는거 막기
        }
        catch
        {
            //Debug.Log("코루틴이 없음");
        }
        Vector2 direction = GetCollisionDirection(collision);
        GameManager.instance.SceneLoadSequence = StartCoroutine(SceneSetSequence(direction)); // 순서를 보장하기 위해 코루틴으로 선언

    }

    protected IEnumerator SceneSetSequence(Vector2 direction) // 순서를 보장하기 위해 코루틴으로 선언 s
    {
        GameManager.instance.spawner.positionSetting = true;
        yield return null;
        DestinationSceneColActivate(direction);
        yield return StartCoroutine(UnLoadSequence(direction));
        yield return StartCoroutine(ReturnSceneTreeUsingDirection(direction).SetLeafSequence(direction, this));
        ReturnSceneTreeUsingDirection(direction).SetLeafPositionAsSpwanerPoint();
        GameManager.instance.SceneNav.OnSceneLoaded();
        GameManager.instance.spawner.positionSetting = false;
    }

    protected void DestinationSceneColActivate(Vector2 direction)
    {
        switch (direction)
        {
            case Vector2 left when direction == Vector2.left:
                leftTree.col.enabled = true;
                break;
            case Vector2 right when direction == Vector2.right:
                rightTree.col.enabled = true;
                break;
            case Vector2 up when direction == Vector2.up:
                upTree.col.enabled = true;
                break;
            case Vector2 down when direction == Vector2.down:
                downTree.col.enabled = true;
                break;
        }
    }

    protected SceneTree ReturnSceneTreeUsingDirection(Vector2 direction)
    {
        SceneTree restult = null;
        switch (direction)
        {
            case Vector2 left when direction == Vector2.left:
                restult = leftTree;
                break;
            case Vector2 right when direction == Vector2.right:
                restult = rightTree;
                break;
            case Vector2 up when direction == Vector2.up:
                restult = upTree;
                break;
            case Vector2 down when direction == Vector2.down:
                restult = downTree;
                break;
        }
        return restult;
    }

    protected IEnumerator UnLoadSequence(Vector2 direction)
    {
        //Debug.Log("언로드 시작" + direction);
        if (direction != Vector2.left)
        {
            //Debug.Log("왼쪽 언로드");
            if (leftTree)
            {
                //Debug.Log("언로드할 적 :" + enemys);
                //foreach (var enemy in enemys)
                //{
                //    try
                //    {
                //        enemy.gameObject.SetActive(false);
                //    }
                //    catch
                //    {
                //        //Debug.Log("이미 비활성화됨");
                //    }
                //}
                //enemys.Clear();
                AsyncOperation unloadLeft = SceneManager.UnloadSceneAsync(leftTree.myScene.name);
                yield return unloadLeft; // 언로드가 완료될 때까지 기다림
            }
        }
        if (direction != Vector2.right)
        {
            //Debug.Log("오른쪽 언로드");
            if (rightTree)
            {
                AsyncOperation unloadRight = SceneManager.UnloadSceneAsync(rightTree.myScene.name);
                yield return unloadRight; // 언로드가 완료될 때까지 기다림
            }
        }
        if (direction != Vector2.up)
        {
            //Debug.Log("위쪽 언로드");
            if (upTree)
            {
                AsyncOperation unloadUp = SceneManager.UnloadSceneAsync(upTree.myScene.name);
                yield return unloadUp; // 언로드가 완료될 때까지 기다림
            }
        }
        if (direction != Vector2.down)
        {
            //Debug.Log("아래쪽 언로드");
            if (downTree)
            {
                AsyncOperation unloadDown = SceneManager.UnloadSceneAsync(downTree.myScene.name);
                yield return unloadDown; // 언로드가 완료될 때까지 기다림
            }
        }
    }

    abstract protected IEnumerator SetLeafSequence(Vector2 direction, SceneTree originTree);// 이놈이 맵을 랜덤으로 설정해주는 함수 상속받아 쓰세연

    protected Scene ReturnRandomSceneUsingDirection(Vector2 dir)// 랜덤으로 씬을 반환
    {
        string result = null;
        switch (dir)
        {
            case Vector2 left when dir == Vector2.left:
                result = ReturnRandomSceneNameInArray(GameManager.instance.LeftScenes);
                break;
            case Vector2 right when dir == Vector2.right:
                result = ReturnRandomSceneNameInArray(GameManager.instance.RightScenes);
                break;
            case Vector2 up when dir == Vector2.up:
                result = ReturnRandomSceneNameInArray(GameManager.instance.UpScenes);
                break;
            case Vector2 down when dir == Vector2.down:
                result = ReturnRandomSceneNameInArray(GameManager.instance.DownScenes);
                break;
            default:
                result = null;
                break;
        }
        return SceneManager.GetSceneByName(result);
    }

    protected string ReturnRandomSceneNameInArray(SceneAsset[] array)
    {
        string result = null;
        while (true)
        {
            int randomIndex = Random.Range(0, array.Length);
            if (!SceneManager.GetSceneByName(array[randomIndex].name).isLoaded)
            {
                result = array[randomIndex].name;
                //Debug.Log("랜덤 씬 로드" + result);
                SceneManager.LoadScene(result, LoadSceneMode.Additive);
                break;
            }
        }
        return result;
    }

    public IEnumerator StartSequence()
    {
        yield return null;
        col.enabled = true;
        // 왼쪽 씬 로드
        yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.left, Vector2.left * 38, this, (sceneTree) => leftTree = sceneTree));

        // 오른쪽 씬 로드
        yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.right, Vector2.right * 38, this, (sceneTree) => rightTree = sceneTree));

        // 위쪽 씬 로드
        yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.up, Vector2.up * 36, this, (sceneTree) => upTree = sceneTree));

        // 아래쪽 씬 로드
        yield return StartCoroutine(LoadAndPositionSceneUsingDir(Vector2.down, Vector2.down * 36, this, (sceneTree) => downTree = sceneTree));

        GameManager.instance.SceneNav.OnSceneLoaded();
        SetLeafPositionAsSpwanerPoint();
    }

    protected IEnumerator LoadAndPositionSceneUsingDir(Vector2 dir, Vector2 offset, SceneTree originScene, System.Action<SceneTree> onSceneLoaded)
    {
        // 랜덤 씬 로드
        //Debug.Log("랜덤 씬 로드");
        Scene randomScene = ReturnRandomSceneUsingDirection(dir);

        // 씬이 로드될 때까지 기다림
        while (!randomScene.isLoaded)
        {
            yield return null;
        }
        //Debug.Log("씬 로드 완료" + originScene.currentPosition);
        // 씬의 루트 오브젝트에서 SceneTree 컴포넌트를 찾음
        GameObject tileMap = randomScene.GetRootGameObjects()[0];
        SceneTree sceneTree = tileMap.GetComponentInChildren<SceneTree>();

        // 씬의 위치를 현재 위치 + 오프셋으로 설정
        //Debug.Log("씬 로드 완료" + originScene.currentPosition);
        sceneTree.currentPosition += originScene.currentPosition + offset;
        tileMap.transform.position = sceneTree.currentPosition;
        sceneTree.myScene = randomScene;
        sceneTree.col.enabled = false;

        // 씬 로드 완료 후 콜백 실행
        onSceneLoaded?.Invoke(sceneTree);
    }

    protected Vector2 GetCollisionDirection(Collider2D collision)//퇴장 방향을 계산해서 반환
    {
        //Debug.Log("퇴장 방향 계산 타일맵 위치 : " + transform.position);
        Vector2 direction = Vector2.zero;
        float xDiff = collision.transform.position.x - transform.position.x;
        float yDiff = collision.transform.position.y - transform.position.y;
        if (xDiff > yDiff)
        {
            if (xDiff > -yDiff)
            {
                direction = Vector2.right;
            }
            else
            {
                direction = Vector2.down;
            }
        }
        else
        {
            if (xDiff > -yDiff)
            {
                direction = Vector2.up;
            }
            else
            {
                direction = Vector2.left;
            }
        }

        return direction;
    }

    protected void SetLeafPositionAsSpwanerPoint()
    {
        GameManager.instance.spawner.enemySpawnPoints.Clear();
        //Debug.Log("스포너 포인트 설정");
        if (leftTree)
        {
            GameManager.instance.spawner.enemySpawnPoints.Add(leftTree);
        }
        if (rightTree)
        {
            GameManager.instance.spawner.enemySpawnPoints.Add(rightTree);
        }
        if (upTree)
        {
            GameManager.instance.spawner.enemySpawnPoints.Add(upTree);
        }
        if (downTree)
        {
            GameManager.instance.spawner.enemySpawnPoints.Add(downTree);
        }
    }
}

