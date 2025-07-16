using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 pos = target.position;
            pos.z = transform.position.z;
            transform.position = pos;
        }
    }
}
