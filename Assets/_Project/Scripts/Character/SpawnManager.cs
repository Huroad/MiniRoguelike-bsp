using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    
    public List<GameObject> monsterPrefabs;
    
    // 랜덤으로 스폰될 몬스터 수
    public int monsterCntMin = 3;
    public int monsterCntMax = 6;
    
    void Awake()
    {
        Instance = this;
        monsterCntMin = GameManager.Instance.currentDifficulty.MonsterCntMin;
        monsterCntMax = GameManager.Instance.currentDifficulty.MonsterCntMax;
    }
    
    public void SpawnMonsters(Node room, Tilemap tileMap, Vector2Int dungeonOffset)
    {
        int count = Random.Range(monsterCntMin, monsterCntMax);
        for (int i = 0; i < count; i++)
        {
            var prefab = monsterPrefabs[Random.Range(0, monsterPrefabs.Count)];

            int margin = 3; 
            int randX = Random.Range(room.roomRect.x + margin, room.roomRect.x + room.roomRect.width - margin);
            int randY = Random.Range(room.roomRect.y + margin, room.roomRect.y + room.roomRect.height - margin);
            
            Vector3Int cell = new Vector3Int(randX - dungeonOffset.x / 2, randY - dungeonOffset.y / 2, 0);
            
            Vector3 spawnPos = tileMap.CellToWorld((Vector3Int)cell) + new Vector3(0.5f, 0.5f, 0);
            spawnPos.z = 0f;
            
            var enemyObj = Instantiate(prefab, spawnPos, Quaternion.identity);
            var enemy = enemyObj.GetComponent<EnemyBase>();
            enemy.room = room;
            room.monsters.Add(enemy);
        }
    }
}
