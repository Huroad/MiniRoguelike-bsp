using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DifficultyData
{
    [SerializeField] private string name;
    // 맵 데이터 (RoomBase)
    [SerializeField] private Vector2Int mapSize;
    [SerializeField] private float minimumDevideRate; //공간이 나눠지는 최소 비율
    [SerializeField] private float maximumDivideRate; //공간이 나눠지는 최대 비율
    [SerializeField] private int maximumDepth; // 방 나누는 댑스
    [SerializeField] private int corridorWidth = 1;  // 복도 폭
    
    // 스폰매니저 데이터 (SpawnManager)
    [SerializeField] private int monsterCntMin;
    [SerializeField] private int monsterCntMax;
    
    
    // Getter
    public string Name => name;
    public Vector2Int MapSize => mapSize;
    public float MinimumDevideRate => minimumDevideRate;
    public float MaximumDivideRate => maximumDivideRate;
    public int MaximumDepth => maximumDepth;
    public int CorridorWidth => corridorWidth;
    public int MonsterCntMin => monsterCntMin;
    public int MonsterCntMax => monsterCntMax;
    
    // Setter
    public DifficultyData(string name, Vector2Int mapSize, float minRate, float maxRate, int maxDepth, int corridorWidth, int minCnt, int maxCnt)
    {
        this.name = name;
        this.mapSize = mapSize;
        this.minimumDevideRate = minRate;
        this.maximumDivideRate = maxRate;
        this.maximumDepth = maxDepth;
        this.corridorWidth = corridorWidth;
        this.monsterCntMin = minCnt;
        this.monsterCntMax = maxCnt;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    // 1=Easy, 2=Normal, 3=Hard
    private int SelectedDifficulty = 1;
    public List<DifficultyData> difficultyDatas;
    public DifficultyData currentDifficulty;    // 현재 설정된 데이터
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (difficultyDatas == null)
            difficultyDatas = new List<DifficultyData>();
        
        if (difficultyDatas.Count == 0)
        {
            difficultyDatas.Add(new DifficultyData(
                "Easy", new Vector2Int(100, 80), 0.4f, 0.6f, 2, 2, 2, 4));
            difficultyDatas.Add(new DifficultyData(
                "Normal", new Vector2Int(150, 120), 0.4f, 0.6f, 3, 2, 3, 5));
            difficultyDatas.Add(new DifficultyData(
                "Hard", new Vector2Int(200, 160), 0.4f, 0.6f, 4, 2, 4, 6));
        }
    }
    
    public DifficultyData GetDifficultyData(int num)
    {
        SelectedDifficulty = num;
        int idx = Mathf.Clamp(SelectedDifficulty - 1, 0, difficultyDatas.Count - 1);
        currentDifficulty = difficultyDatas[idx];
        return currentDifficulty;
    }

}
