using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public SpriteRenderer sr;
    public Collider2D coll;
    
    public bool isOpen = false;
    
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!MapManager.Instance.isFight && !isOpen)
        {
            OpenDoor();
            MapManager.Instance.doorUseList.Add(gameObject);
        }
    }
    
    public void OpenDoor()
    {
        isOpen = true;
        sr.enabled = false;
        coll.enabled = false;
    }
    public void CloseDoor()
    {
        isOpen = false;
        sr.enabled = true;
        coll.enabled = true;
    }

    
}
