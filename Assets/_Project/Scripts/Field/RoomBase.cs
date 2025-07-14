using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class RoomBase :MonoBehaviour
{
    [SerializeField] public Vector2Int mapSize;
    [SerializeField] private float minimumDevideRate; //공간이 나눠지는 최소 비율
    [SerializeField] private float maximumDivideRate; //공간이 나눠지는 최대 비율
    [SerializeField] private int maximumDepth; // 방 나누는 댑스
    [SerializeField] private Tilemap tileMap; 
    [SerializeField] private Tile roomTile; // 방 배경
    [SerializeField] private Tile wallTile; // 방 외벽
    [SerializeField] private Tile outTile; // 방과 벽을 제외한 외부
    public int corridorWidth = 1;  // 복도 폭
    [SerializeField] private Transform doorBox;
    
    
    void Start()
    {   
        // MakeMap();
    }
  
    // 전체 맵 생성
    public void MakeMap()
    {
        FillBackground();   // 전체를 바깥 타일로 채움
        Node root = new Node(new RectInt(0, 0, mapSize.x, mapSize.y));
        Divide(root, 0);    // BSP 분할 (트리구조 생성)
        GenerateRoom(root, 0);  // 리프 노드마다 방 생성
        FillWall(); // 방과 바깥 경계선을 벽으로 채움
        AddList(root); // 던전 생성 후 방 리스트에 넣기
        GenerateLoad();
        FillWall(); // 방과 바깥 경계선을 벽으로 채움
        
    }
    
    // BSP 트리로 공간을 분할
    void Divide(Node node,int n)
    {
        // 최대 깊이에 도달하면 분할 종료
        if (n == maximumDepth) return;
        
        // 가로 세로 중 더긴방향으로 나눔
        int maxLength = Mathf.Max(node.nodeRect.width, node.nodeRect.height);
        //나올 수 있는 최대 길이와 최소 길이중에서 랜덤으로 한 값을 선택
        int split = Mathf.RoundToInt(Random.Range(maxLength * minimumDevideRate, maxLength * maximumDivideRate));
        
        if (node.nodeRect.width >= node.nodeRect.height)
        {
            node.leftNode = new Node(new RectInt(node.nodeRect.x,node.nodeRect.y,split,node.nodeRect.height));
            node.rightNode= new Node(new RectInt(node.nodeRect.x + split, node.nodeRect.y, node.nodeRect.width - split, node.nodeRect.height));
            node.leftNode.direction = MapManager.DoorPos.West;
            node.rightNode.direction = MapManager.DoorPos.East;
        }
        else
        {
            node.leftNode = new Node(new RectInt(node.nodeRect.x, node.nodeRect.y, node.nodeRect.width,split));
            node.rightNode = new Node(new RectInt(node.nodeRect.x, node.nodeRect.y + split, node.nodeRect.width , node.nodeRect.height - split));
            node.leftNode.direction = MapManager.DoorPos.Sou;
            node.rightNode.direction = MapManager.DoorPos.Nor;
        }
        
        node.leftNode.parNode = node;
        node.rightNode.parNode = node;
        
        Divide(node.leftNode, n + 1);
        Divide(node.rightNode, n + 1);
    }
    
    // 리프 노드마다 방 Rect을 랜덤하게 생성하고 방 위치/크기 조정
    private RectInt GenerateRoom(Node node,int n)
    {
        RectInt rect;
        if (n == maximumDepth)
        {
           rect = node.nodeRect;
           // 방 크기
           int width = Random.Range(rect.width / 2,rect.width - 1); 
           int height=Random.Range(rect.height / 2,rect.height - 1);  
           // 만약 0이 된다면 붙어 있는 방과 합쳐지기 때문에,최솟값은 1로 해주고, 최댓값은 기존 노드의 가로에서 방의 가로길이를 빼 준 값이다.
           int x = rect.x + Random.Range(1, rect.width - width);
           int y = rect.y + Random.Range(1, rect.height - height);
           rect = new RectInt(x, y, width, height);
           FillRoom(rect); 
        }
        else                    
        {
            node.leftNode.roomRect = GenerateRoom(node.leftNode,n+1);
            node.rightNode.roomRect = GenerateRoom(node.rightNode, n + 1);
            rect = node.leftNode.roomRect;
        }
        return rect;
    }
    
   // 복도와 문 생성
    public void GenerateLoad()
    {
        List<Node> rooms = MapManager.Instance.roomList;
        int n = rooms.Count;
        List<Vector2Int> centers = new List<Vector2Int>();
        for (int i = 0; i < n; i++) centers.Add(rooms[i].center);

        bool[] visited = new bool[n];
        int[] minDist = new int[n];
        int[] prev = new int[n];
        for (int i = 0; i < n; i++) minDist[i] = int.MaxValue;
        minDist[0] = 0;

        // Prim's MST: centers 기준
        for (int i = 0; i < n; i++)
        {
            int u = -1;
            for (int v = 0; v < n; v++)
                if (!visited[v] && (u == -1 || minDist[v] < minDist[u]))
                    u = v;
            visited[u] = true;

            for (int v = 0; v < n; v++)
            {
                if (visited[v]) continue;
                int dist = Mathf.Abs(centers[u].x - centers[v].x) + Mathf.Abs(centers[u].y - centers[v].y);
                if (dist < minDist[v])
                {
                    minDist[v] = dist;
                    prev[v] = u;
                }
            }
        }

        // 복도 연결(중점→중점, L자 경로)
        int totalCnt = 0;
        for (int v = 1; v < n; v++)
        {
            int u = prev[v];
            Vector2Int from = centers[u];
            Vector2Int to = centers[v];
            var path = MakeCorridorPath(from, to);
            int cnt = 0;
            for (int i = 0; i < path.Count; i++)
            {
                Vector2Int pos = path[i];
                for (int w = -corridorWidth / 2; w <= corridorWidth / 2 - 1; w++)
                {
                    bool horizontal = Mathf.Abs(from.x - to.x) > Mathf.Abs(from.y - to.y);
                    Vector2Int offset = horizontal ? new Vector2Int(pos.x, pos.y + w) : new Vector2Int(pos.x + w, pos.y);
                    Vector3Int tilePos = new Vector3Int(offset.x - mapSize.x / 2, offset.y - mapSize.y / 2, 0);

                    // --- 기존 타일이 wallTile이면 문 설치 ---
                    if (tileMap.GetTile(tilePos) == wallTile)
                    {
                        // 방향 인덱스 구하기 (동서남북)
                        int dir = 0;
                        if (horizontal) dir = (from.x < to.x) ? 2 : 3; // east/west
                        else dir = (from.y < to.y) ? 0 : 1; // north/south
                        if (cnt <= 1 && dir == 3)
                            dir = 2;
                        else if (cnt <= 1 && dir == 2)
                            dir = 3;
                        else if (cnt <= 1 && dir == 1)
                            dir = 0;
                        else if (cnt <= 1 && dir == 0)
                            dir = 1;
                        SpawnDoor(offset, MapManager.Instance.doorfabList[dir]);
                        cnt++;
                        totalCnt++;
                    }

                    // 복도 타일 칠하기
                    tileMap.SetTile(tilePos, roomTile);
                }
            }
        }
        MapManager.Instance.doorCnt = totalCnt;

    }

    // L자 복도 경로
    List<Vector2Int> MakeCorridorPath(Vector2Int start, Vector2Int end)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int cur = start;
        path.Add(cur);
        while (cur.x != end.x) { cur.x += (end.x > cur.x) ? 1 : -1; path.Add(cur); }
        while (cur.y != end.y) { cur.y += (end.y > cur.y) ? 1 : -1; path.Add(cur); }
        return path;
    }

    // 문위치 타일에 월드좌표로 변환후 스폰
    public GameObject SpawnDoor(Vector2Int cell, GameObject doorPrefab)
    {
        Vector3Int tilePos = new Vector3Int(cell.x - mapSize.x / 2, cell.y - mapSize.y / 2, 0);
        Vector3 worldPos = tileMap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);
        worldPos.z = 0f;
        GameObject doorObj = Instantiate(doorPrefab, worldPos, Quaternion.identity, doorBox);
        MapManager.Instance.doorList.Add(doorObj);
        return doorObj;
    }
    
    // 밖 배경 채우기
    void FillBackground()
    {
        for(int i = -5; i < mapSize.x + 5; i++)
        {
            for(int j = -5; j < mapSize.y + 5; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), outTile);
            }
        }
    }
    
    // 외부와 내부 공간에 벽 생성
    void FillWall()
    {      
        for (int i = 0; i < mapSize.x; i++) //타일 전체를 순회
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                if(tileMap.GetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0)) == outTile)
                {
                    for(int x = -1; x <= 1; x++)
                    {
                        for(int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0) continue;//바깥 타일 기준 8방향을 탐색해서 room tile이 있다면 wall tile로 바꿔준다.
                            if(tileMap.GetTile(new Vector3Int(i - mapSize.x / 2+x, j - mapSize.y / 2+y, 0)) == roomTile)
                            {
                                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0) , wallTile);
                                break;
                            }
                        }
                    }
                }
            }   
        }
    }
    
    // Rect 범위에 roomTile을 칠해 방 생성
    private void FillRoom(RectInt rect) {
    for(int i = rect.x; i< rect.x + rect.width; i++)
        {
            for(int j = rect.y; j < rect.y + rect.height; j++)
            {
                tileMap.SetTile(new Vector3Int(i - mapSize.x / 2, j - mapSize.y / 2, 0), roomTile);
            }
        }
    }
    
    // 리스트에 넣기
    void AddList(Node node) {
        //MapManager mapManager = FindObjectOfType<MapManager>();
        if (node.leftNode == null && node.rightNode == null) {
            MapManager.Instance.roomList.Add(node);
        } else {
            MapManager.Instance.corridorList.Add(node);
            if (node.leftNode != null) AddList(node.leftNode);
            if (node.rightNode != null) AddList(node.rightNode);
        }
    }
}