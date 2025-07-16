using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[System.Serializable]
public class Node
{
    public Node leftNode;   // 왼쪽 자식의 방 정보
    public Node rightNode;  // 오늘쪽 자식의 방 정보
    public Node parNode;    // 부모 방 정보
    public RectInt nodeRect;    // 방, 복도의 rect 
    public RectInt roomRect; // 방 rect
    
    public Vector2Int center // 방의 중심좌표(복도 연결할 떄 사용)
    {
        get {
            return new Vector2Int(roomRect.x+roomRect.width/2, roomRect.y + roomRect.height/ 2);
        }
    }
    
    public MapManager.DoorPos direction;      // 이 노드(방/복도)와 연결된 문 방향
    public Vector2Int entryCell;              // 방/복도 입구(시작점) 셀 좌표
    
    public Node(RectInt rect)
    {
        this.nodeRect = rect;
    }

    public List<Door> doors = new List<Door>(); 
    public List<EnemyBase> monsters = new List<EnemyBase>();
    // public bool isUse = false;  // 방 사용여부
    public bool monsterSpawned = false; // 몬스터 스폰 여부 

    public GameObject roomTrigger;


}

public class MapManager : MonoBehaviour
{
    [HideInInspector] public static MapManager Instance { get; private set; }
    
    [SerializeField] public RoomBase roomBase;
    public List<Node> roomList = new List<Node>();  // 방 리스트
    public List<Node> corridorList = new List<Node>();  // 복도 리스트
    [HideInInspector] public int doorCnt = 0;

    public GameObject playerFab;    
    public GameObject bossFab;
    
    [HideInInspector] public Node playerRoom;   // 플레이어 시작방
    [HideInInspector] public Node bossRoom; // 보스 방

    private Tilemap tileMap;
    public Tilemap GetTilemap() => tileMap;
    
    [Tooltip("배치할 도어 프리팹")] public List<GameObject> doorfabList;
    [Tooltip("생성된 리스트")] [HideInInspector] public List<GameObject> doorList;
    [Tooltip("사용한 문 리스트")] public List<GameObject> doorUseList;
    
    [SerializeField] GameObject roomTriggerPrefab;
    public Transform roomTriggersParent;
    [SerializeField] GameObject cameraTriggerPrefab;
    public Transform cameraTriggerParent;
    
    public CinemachineVirtualCamera virtualCamera;
    
    public enum DoorPos { Nor, Sou, East, West}
    
    public enum RoomType
    {
        Normal,
        Player,
        Boss
    }
    public RoomType roomType;
    
    void Awake()
    {
        Instance = this;
        roomBase = FindObjectOfType<RoomBase>() ? FindObjectOfType<RoomBase>() : null;
        tileMap = FindObjectOfType<Tilemap>();
    }
    
    IEnumerator Start()
    {
        yield return StartCoroutine(MakeMapCheck());
        
        SpawnRoomTriggers();
        roomTriggersParent.position -= new Vector3(roomBase.mapSize.x / 2, roomBase.mapSize.y / 2, 0);
        cameraTriggerParent.position -= new Vector3(roomBase.mapSize.x / 2, roomBase.mapSize.y / 2, 0);
        
        FindTypeRooms(roomList);
        playerRoom.roomTrigger.GetComponent<RoomTrigger>().isUse = true;
        
        // 플레이어 배치
        GameObject player = Instantiate(playerFab, CellToWorldCenter(playerRoom), Quaternion.identity);
        PlayerHpUI hpUI = FindObjectOfType<PlayerHpUI>();
        hpUI.SetPlayer(player.GetComponent<PlayerController>());
        PlayerCamera pc = FindObjectOfType<PlayerCamera>();
        pc.target = player.transform;
        
        virtualCamera.Follow = player.transform;
        var confiner = virtualCamera.GetComponent<CinemachineConfiner2D>();
        confiner.m_BoundingShape2D = playerRoom.roomTrigger.GetComponent<RoomTrigger>().polycoll;
        confiner.InvalidateCache();
        playerRoom.roomTrigger.GetComponent<RoomTrigger>().roomMask.enabled = false;

        bossRoom.roomTrigger.GetComponent<RoomTrigger>().isBossRoom = true;
        
        MiniMap map = FindObjectOfType<MiniMap>();
        map.player = player.transform;
        
    }
    
    IEnumerator MakeMapCheck()
    {
        while (true)
        {
            roomBase.MakeMap();
            yield return null;
        
            if (doorCnt <= 2 * roomBase.corridorWidth * corridorList.Count)
                break;
            
            foreach (var door in doorList)
                Destroy(door.gameObject);

            doorCnt = 0;
            doorList.Clear();
            roomList.Clear();
            corridorList.Clear();
            print("재생성");
        }
    }
    
    // 타일값을 월드 Vector값으로 변환
    public Vector3 CellToWorldCenter(Node node)
    {
        Vector2Int spawnCell = node.center + new Vector2Int(-roomBase.mapSize.x / 2, -roomBase.mapSize.y / 2);
        Vector3 pos = tileMap.CellToWorld((Vector3Int)spawnCell) + new Vector3(0.5f, 0.5f, 0);
        pos.z = 0f;
        return pos;
    }
    
    // 복도 끝점(타일 좌표) → 월드 좌표 변환
    public Vector3 CorridorEndToWorld(Vector2Int corridorEndCell)
    {
        Vector3Int tilePos = new Vector3Int(corridorEndCell.x - roomBase.mapSize.x / 2, corridorEndCell.y - roomBase.mapSize.y / 2, 0);
        Vector3 pos = tileMap.CellToWorld(tilePos) + new Vector3(0.5f, 0.5f, 0);
        pos.z = 0f;
        return pos;
    }
    
    // 방 트리거 생성
    void SpawnRoomTriggers()
    {
        foreach (var roomNode in roomList)
        {
            var rect = roomNode.roomRect;
            Vector2 center = new Vector2(rect.x + rect.width / 2f, rect.y + rect.height / 2f);
            Vector3Int cell = Vector3Int.FloorToInt(center);
            Vector3 worldCenter = tileMap.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0);
            
            var obj = Instantiate(roomTriggerPrefab, worldCenter, Quaternion.identity, roomTriggersParent);
            
            var coll = obj.GetComponent<BoxCollider2D>();
            coll.size = new Vector2(rect.width - 3, rect.height - 3);
            coll.isTrigger = true;

            roomNode.roomTrigger = obj;
            
            var roomTrigger = obj.GetComponent<RoomTrigger>();
            roomTrigger.room = roomNode;

            // RoomTrigger 오브젝트를 worldCenter에 위치
            var cameraObj = Instantiate(cameraTriggerPrefab, worldCenter, Quaternion.identity, cameraTriggerParent);
            //obj.GetComponent<RoomTrigger>().roomMask = cameraObj.GetComponent<SpriteMask>();
            
            var poly = cameraObj.GetComponent<PolygonCollider2D>();
            if (poly == null)
                poly = cameraObj.AddComponent<PolygonCollider2D>();

            roomTrigger.polycoll = poly;
            var spriteMask = cameraObj.GetComponentInChildren<SpriteMask>();
            roomTrigger.roomMask = spriteMask;
            
            // Collider Points는 반드시 "오브젝트의 Pivot(0,0)을 중심"으로 설정!
            float wallThickness = 1f;
            float w = rect.width + wallThickness * 2;
            float h = rect.height + wallThickness * 2;
            Vector2[] points = new Vector2[4];
            points[0] = new Vector2(-w / 2f, -h / 2f); // BottomLeft
            points[1] = new Vector2(w / 2f, -h / 2f);  // BottomRight
            points[2] = new Vector2(w / 2f, h / 2f);   // TopRight
            points[3] = new Vector2(-w / 2f, h / 2f);  // TopLeft
            poly.SetPath(0, points);
            poly.isTrigger = true;
            
            // 마스크 설정
            Vector2 spriteSize = spriteMask.sprite.bounds.size;
            spriteMask.transform.localScale = new Vector3(w / spriteSize.x -2, h / spriteSize.y -2, 1f);
        }
    }
    
    // 플레이어와 보스방 지정
    void FindTypeRooms(List<Node> rooms)
    {
        float maxDist = 0f;
        playerRoom = null;
        bossRoom = null;
    
        for (int i = 0; i < rooms.Count; i++)
        { 
            for (int j = i + 1; j < rooms.Count; j++)
            {
                float dist = Vector2Int.Distance(rooms[i].center, rooms[j].center);
                if (dist > maxDist)
                {
                    maxDist = dist;
                    playerRoom = rooms[i];
                    bossRoom = rooms[j];
                }
            }
        }
    }
}
