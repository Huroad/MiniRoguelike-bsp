using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    public Transform player;
    
    void LateUpdate()
    {
        if (player == null) return;
        Vector3 newPos = player.position;
        newPos.z = -1f;
        transform.position = newPos;
    }
}

