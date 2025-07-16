using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public SpriteRenderer sr;
    public Collider2D coll;
    
    public bool isOpen = false;
    public bool firstOpen = false;
    
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!GameManager.Instance.isFight && !isOpen)
        {
            OpenDoor();
            if(GameManager.Instance.GetUseDoorOpenCnt() == 0)
                GameManager.Instance.UseDoorOpen();
            MapManager.Instance.doorUseList.Add(gameObject);
        }
    }
    
    public void OpenDoor()
    {
        isOpen = true;
        sr.enabled = false;
        coll.enabled = false;
        firstOpen = true;
    }
    public void CloseDoor()
    {
        isOpen = false;
        sr.enabled = true;
        coll.enabled = true;
    }

    
}
