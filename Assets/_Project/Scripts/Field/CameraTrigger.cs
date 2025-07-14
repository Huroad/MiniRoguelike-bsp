using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{
    // 방데이터0
    public Node room;
    public SpriteMask roomMask;
    
    void Awake()
    {
        if (roomMask == null)
            roomMask = GetComponentInChildren<SpriteMask>();
        if (roomMask != null)
            roomMask.enabled = false; // 시작은 비활성화
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            var confiner = FindObjectOfType<CinemachineConfiner2D>();
            confiner.m_BoundingShape2D = transform.GetComponent<PolygonCollider2D>();
            print(transform.GetComponent<PolygonCollider2D>());
            confiner.InvalidateCache();
            
        }
    }

    
}
