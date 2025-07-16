using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    // 방데이터0
    public Node room;
    public SpriteMask roomMask;
    public PolygonCollider2D polycoll;
    
    public GameObject doorFab;
    public bool isUse = false;
    public bool isBossRoom = false;
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            foreach (var rt in FindObjectsOfType<RoomTrigger>())
                if (rt.roomMask != null) rt.roomMask.enabled = true;
            
            if (roomMask != null) roomMask.enabled = false;
            
            // 카메라를 방안에 가두기
            // var confiner = FindObjectOfType<CinemachineConfiner2D>();
            // confiner.m_BoundingShape2D = polycoll;
            // print(polycoll);
            // confiner.InvalidateCache();
            
            if (MapManager.Instance.doorUseList.Count > 0 && MapManager.Instance.doorUseList[0].GetComponent<Door>().isOpen && !isUse)
            {
                
                foreach (var doors in MapManager.Instance.doorUseList)
                    doors.GetComponent<Door>().CloseDoor();
                
                isUse = true;
                //MapManager.Instance.isFight = true;
                GameManager.Instance.isFight = true;
                MapManager.Instance.doorUseList.Clear();

                if (isBossRoom)
                {
                    GameObject boss = Instantiate(MapManager.Instance.bossFab, MapManager.Instance.CellToWorldCenter(room), Quaternion.identity);
                    // BossHpUI hpUI = FindObjectOfType<BossHpUI>();
                    // print(hpUI);
                    // hpUI.gameObject.SetActive(true);
                    // print(boss);
                    // print(boss.GetComponent<EnemyBoss1>());
                    // hpUI.SetBoss(boss.GetComponent<EnemyBoss1>());
                }
                else
                    SpawnManager.Instance.SpawnMonsters(room, MapManager.Instance.GetTilemap(), MapManager.Instance.roomBase.mapSize);
                
                // 문 카운트 초기화
                GameManager.Instance.SetUseDoorOpenCnt(0);

            }
        }
    }
    
}
