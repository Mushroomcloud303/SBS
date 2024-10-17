using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
//using NavMeshPlus.Components;
using System.Linq;
//using static UnityEditor.PlayerSettings;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] Vector2Int mapSize;
    [SerializeField] float minimumDevideRate; //공간이 나눠지는 최소 비율
    [SerializeField] float maximumDivideRate; //공간이 나눠지는 최대 비율
    [SerializeField] private GameObject line; //lineRenderer를 사용해서 공간이 나눠진걸 시작적으로 보여주기 위함
    [SerializeField] private GameObject map; //lineRenderer를 사용해서 첫 맵의 사이즈를 보여주기 위함
    [SerializeField] private GameObject roomLine; //lineRenderer를 사용해서 방의 사이즈를 보여주기 위함
    [SerializeField] private int maximumDepth; //트리의 높이, 높을 수록 방을 더 자세히 나누게 됨
    [SerializeField] Tilemap floorTileMap;
    [SerializeField] Tilemap wallTileMap;
    [SerializeField] Tilemap BlackMap;
    //[SerializeField] Tilemap dambangTileMap; // 그림자 안쓸거라 삭제
    [SerializeField] Tile roomTile; //방을 구성하는 타일
    [SerializeField] Tile wallTile; //방과 외부를 구분지어줄 벽 타일
    [SerializeField] Tile outTile; //방 외부의 타일
    [SerializeField] int minDistance; //방과 방 사이의 최소 거리
    [SerializeField] GameObject[] furnitures; //가구들
    [SerializeField] List<GameObject> furnituresBatch; //배치한 가구들
    public GameObject NextEntrance; // 출구 오브젝트
    public Tilemap Dark; // 어둠 타일맵
    public Tile DarkTile; // 어둠 타일
    public int DarkRadius; // 어둠 크기
    //public NavMeshSurface nav;
    public List<Vector2Int> roomPositons;
    public static MapGenerator instance;
    public bool lol;
    public bool lul;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

    }

    public void Init()
    {
        DisableFurniture();
        FillBackground();//신 로드 시 전부다 바깥타일로 덮음
        roomPositons.Clear();
        // Invoke("LUL", 0.25f);
        /*
        GameObject omg = Instantiate(shadowCas);
        omg.GetComponent<ShadowWrangler>().sourceColliders[0] = wallCol;
        omg.GetComponent<ShadowWrangler>().RefreshWorldShadows();
        */
    }

    public void GenerateMap()
    {
        StartCoroutine(GeneratetorSequence());
    } //이거로 호출하면 됨

    IEnumerator GeneratetorSequence()
    {
        //코루틴 안하면 타일 마다 메쉬 베이크해서 느려짐
        Init();       
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        DarkBuilder(); // 미니맵 어둠으로 뒤덮기
        yield return null;
        Divide(root, 0, maximumDepth);
        yield return null;
        GenerateRoom(root, 0);
        yield return null;
        GenerateNextEntrance();
        yield return null;
        GameManager.instance.MovePlayerstartpoint(roomPositons[UnityEngine.Random.Range(0, roomPositons.Count)]);
        yield return null;
        DarkBuilder(); // 왜 한 번 더하냐고? 게임 시작 전 플레이어 처음 위치가 밝혀져버림
        GenerateLoad(root, 0);
        yield return null;
        FillWall(); //바닥 타일을 주변이 배경이면 벽으로 바꿔줌
        yield return null;
        //nav.BuildNavMesh();
        yield return null;
        Invoke("LUL", 0.25f);
    }  //밑은 노드 클래스, 이 클래스를 이용해서 공간을 나누고 방을 만들어 줄 것이다.

    void DisableFurniture()
    {
        foreach(GameObject furniture in furnituresBatch)
        {
            furniture.SetActive(false);
        }
    }

    void Divide(Node tree, int n, int max)
    {
        //GameObject Square = Instantiate(map, new Vector3(0, 0, 0), Quaternion.identity);
        //map.GetComponent<LineRenderer>().SetPosition(0, new Vector3(tree.nodeRect.x - mapSize.x / 2, tree.nodeRect.y - mapSize.y / 2, 0));
        //map.GetComponent<LineRenderer>().SetPosition(1, new Vector3(tree.nodeRect.x - mapSize.x / 2 + tree.nodeRect.width, tree.nodeRect.y - mapSize.y / 2, 0));
        //map.GetComponent<LineRenderer>().SetPosition(2, new Vector3(tree.nodeRect.x - mapSize.x / 2 + tree.nodeRect.width, tree.nodeRect.y - mapSize.y / 2 + tree.nodeRect.height, 0));
        //map.GetComponent<LineRenderer>().SetPosition(3, new Vector3(tree.nodeRect.x - mapSize.x / 2, tree.nodeRect.y - mapSize.y / 2 + tree.nodeRect.height, 0));
        if (n == max) return; //내가 원하는 높이에 도달하면 더 나눠주지 않는다.
                              //그 외의 경우에는

        int maxLength = Mathf.Max(tree.nodeRect.width, tree.nodeRect.height);
        //가로와 세로중 더 긴것을 구한후, 가로가 길다면 위 좌, 우로 세로가 더 길다면 위, 아래로 나눠주게 될 것이다.
        int split = Mathf.RoundToInt(UnityEngine.Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));
        //나올 수 있는 최대 길이와 최소 길이중에서 랜덤으로 한 값을 선택
        if (tree.nodeRect.width >= tree.nodeRect.height) //가로가 더 길었던 경우에는 좌 우로 나누게 될 것이며, 이 경우에는 세로 길이는 변하지 않는다.
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, split, tree.nodeRect.height));
            //왼쪽 노드에 대한 정보다 
            //위치는 좌측 하단 기준이므로 변하지 않으며, 가로 길이는 위에서 구한 랜덤값을 넣어준다.
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x + split, tree.nodeRect.y, tree.nodeRect.width - split, tree.nodeRect.height));
            //우측 노드에 대한 정보다 
            //위치는 좌측 하단에서 오른쪽으로 가로 길이만큼 이동한 위치이며, 가로 길이는 기존 가로길이에서 새로 구한 가로값을 뺀 나머지 부분이 된다. 
        }
        else
        {

            tree.leftNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y, tree.nodeRect.width, split));
            tree.rightNode = new Node(new RectInt(tree.nodeRect.x, tree.nodeRect.y + split, tree.nodeRect.width, tree.nodeRect.height - split));
            //DrawLine(new Vector2(tree.nodeRect.x , tree.nodeRect.y+ split), new Vector2(tree.nodeRect.x + tree.nodeRect.width, tree.nodeRect.y  + split));
        }
        tree.leftNode.parNode = tree; //자식노드들의 부모노드를 나누기전 노드로 설정
        tree.rightNode.parNode = tree;
        Divide(tree.leftNode, n + 1, max); //왼쪽, 오른쪽 자식 노드들도 나눠준다.
        Divide(tree.rightNode, n + 1, max);//왼쪽, 오른쪽 자식 노드들도 나눠준다.
    }

    private RectInt GenerateRoom(Node tree, int n)
    {
        RectInt rect;
        if (n == maximumDepth) //해당 노드가 4번째 줄기 노드라면 방을 만들어 줄 것이다.
        {
            rect = tree.nodeRect;
            int width = UnityEngine.Random.Range(rect.width / 2 + 6, rect.width - 6);
            //방의 가로 최소 크기는 노드의 가로길이의 절반, 최대 크기는 가로길이보다 10 작게 설정한 후 그 사이 값중 랜덤한 값을 구해준다.
            int height = UnityEngine.Random.Range(rect.height / 2 + 6, rect.height - 6); // 통로가 삐져나오지 안게 최솟값을 조정
            //높이도 위와 같다.
            int x = rect.x + UnityEngine.Random.Range(6, rect.width - width - 6);
            //방의 x좌표이다. 만약 0이 된다면 붙어 있는 방과 합쳐지기 때문에,최솟값은 10 로 해주고, 최댓값은 기존 노드의 가로에서 방의 가로길이를 빼 준 값이다.
            int y = rect.y + UnityEngine.Random.Range(6, rect.height - height - 6);
            //y좌표도 위와 같다.

            SetRoomPositions(x, y, width, height);
            rect = new RectInt(x, y, width, height);
            FillRoom(rect);
            Node root = new Node(new RectInt(x, y, width, height));
            Divide(root, 0, 2);
            GenerateFurniture(root, 0);
        }
        else
        {
            tree.leftNode.roomRect = GenerateRoom(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateRoom(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }
        return rect;
    }

    private void SetRoomPositions(int x, int y, int width, int height)
    {
        roomPositons.Add(new Vector2Int(x + width/2 - mapSize.x/2, y + height/2 - mapSize.y /2));
    }

    private RectInt GenerateFurniture(Node tree, int n)
    {  
        RectInt rect;
        if (n == 2) // 방을 2번 나눈 경우임 이 경우 중심에 가구 프리팹 생성
        {
            rect = tree.nodeRect;
            while(true)
            {
                int randomIndex = UnityEngine.Random.Range(0, furnitures.Length);
                Vector3 boxSize = furnitures[randomIndex].GetComponent<BoxCollider2D>().size;
                if (boxSize.x/2 < rect.width/2 && boxSize.y / 2 < rect.height / 2)
                {
                    GameObject furniture = GameManager.instance.poolManager.GetFromPool(randomIndex+5); // 5번째부터 가구 프리팹
                    furniture.transform.position = new Vector3(rect.x + rect.width / 2 - mapSize.x / 2, rect.y + rect.height / 2 - mapSize.y / 2, 0);
                    furnituresBatch.Add(furniture);
                    break;
                }    
            }                    
        }
        else
        {
            tree.leftNode.roomRect = GenerateFurniture(tree.leftNode, n + 1);
            tree.rightNode.roomRect = GenerateFurniture(tree.rightNode, n + 1);
            rect = tree.leftNode.roomRect;
        }
        return rect;
    }

    private void GenerateNextEntrance() // 가구 중에 하나를 다음층 계단으로 바꾸는 함수
    {
        int ran = UnityEngine.Random.Range(furnituresBatch.Count-63, furnituresBatch.Count-1);
        Vector3 pos = furnituresBatch[ran].transform.position;
        GameObject nex = Instantiate(NextEntrance);
        nex.transform.position = pos;
        furnituresBatch[ran].SetActive(false);
        furnituresBatch[ran] = nex;
    }

    private void GenerateLoad(Node tree, int n)
    {
        if (n == maximumDepth) //리프 노드는 이을수가 없다
            return;
        Vector2Int leftNodeCenter = tree.leftNode.center;
        Vector2Int rightNodeCenter = tree.rightNode.center;
        
        for (int i = Mathf.Min(leftNodeCenter.x, rightNodeCenter.x); i <= Mathf.Max(leftNodeCenter.x, rightNodeCenter.x); i++) // x 선
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -1; y <= 6 ; y++)
                {
                    floorTileMap.SetTile(new Vector3Int(i - mapSize.x / 2 + x, leftNodeCenter.y - mapSize.y / 2 + y, 0), roomTile);
                    BlackMap.SetTile(new Vector3Int(i - mapSize.x / 2 + x, leftNodeCenter.y - mapSize.y / 2 + y, 0), null);
                }
            }

        }

        for (int j = Mathf.Min(leftNodeCenter.y, rightNodeCenter.y); j <= Mathf.Max(leftNodeCenter.y, rightNodeCenter.y); j++) // y 선
        {
            for (int x = -2; x <= 2; x++)
            {
                for (int y = -1; y <= 6; y++)
                {
                    floorTileMap.SetTile(new Vector3Int(rightNodeCenter.x - mapSize.x / 2 + x, j - mapSize.y / 2 + y, 0), roomTile);
                    BlackMap.SetTile(new Vector3Int(rightNodeCenter.x - mapSize.x / 2 + x, j - mapSize.y / 2 + y, 0), null);
                }
            }
        }
        //이전 포스팅에서 선으로 만들었던 부분을 room tile로 채우는 과정
        GenerateLoad(tree.leftNode, n + 1); //�ڽ� ���鵵 Ž��
        GenerateLoad(tree.rightNode, n + 1);
    }
    
    void FillBackground() //배경을 채우는 함수, 씬 load시 가장 먼저 해준다.
    {
        floorTileMap.ClearAllTiles();
        wallTileMap.ClearAllTiles();
        BlackMap.ClearAllTiles();
        //dambangTileMap.ClearAllTiles();
        for (int i = -20; i < mapSize.x + 20; i++) //바깥타일은 맵 가장자리에 가도 어색하지 않게
        //맵 크기보다 넓게 채워준다.
        {
            for (int j = -20; j < mapSize.y + 20; j++)
            {
                floorTileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile);
                BlackMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile);
            }
        }
    }

    void FillWall() //룸 타일과 바깥 타일이 만나는 부분
    {
        for (int i = -10; i < mapSize.x + 10; i++) //타일 전체를 순회
        {
            for (int j = -10; j < mapSize.y + 10; j++)
            {
                if (floorTileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == roomTile)
                {
                    //룸타일일 경우
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 4; y++)
                        {
                            if (x == 0 && y == 0) continue;//바깥 타일 기준 3 x 6방향을 탐색해서 out tile이 있다면 wall tile로 바꿔준다.       
                            if (floorTileMap.GetTile(new Vector3Int(i - mapSize.x / 2 + x, j - mapSize.y / 2 + y, 0)) == outTile)
                            {
                                wallTileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile);
                                BlackMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), null);
                                //int ran = UnityEngine.Random.Range(0,2);
                                //if (ran == 0)
                                //    wallTileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile);
                                //else
                                //    dambangTileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), wallTile);
                                //break;  //그림자 안쓸거라 삭제
                            }
                        }
                        if(wallTileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == wallTile)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private void FillRoom(RectInt rect)
    { 
        for (int i = rect.x; i < rect.x + rect.width; i++)
        {
            for (int j = rect.y; j < rect.y + rect.height + 3; j++)
            {
                floorTileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile);
                BlackMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), null);
            }
        }
    }

    void LUL()
    {
        lol = true;
        lul = true;
    }

    void DarkBuilder()
    {
        Vector3 pos = transform.position; //center of the circle
        for (int x = -DarkRadius; x < DarkRadius; x++)
        {
            for (int y = -DarkRadius; y < DarkRadius; y++) //find the box
            {
                Vector3Int Tilepos = Dark.WorldToCell(new Vector2(pos.x + x, pos.y + y));
                if (Vector3.Distance(pos, Tilepos) <= DarkRadius) //check distance to make it a circle
                {
                    Dark.SetTile(Tilepos, DarkTile);
                }
            }
        }
    }
}