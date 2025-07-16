using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyBase : CharacterBase
{
    // 공격할 대상 Player
    public GameObject target;
    public int collDamage = 20;

    public Node room;

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        // 맞을 때 이펙트/사운드 등 추가 가능
    }

    protected virtual  void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    protected override void Die()
    {
        base.Die();
        // 적 사망시 점수/드랍/파괴처리 등
        //Destroy(gameObject);
        
        if (room != null)
        {
            room.monsters.Remove(this);
            // 모든 몬스터가 죽으면 문 열기
            if (room.monsters.Count == 0)
            {
                GameManager.Instance.isFight = false;
                /*foreach (var door in room.doors)
                {
                    door.GetComponent<Door>().OpenDoor();
                }*/
            }
        }
    }
    
    protected virtual void Move()
    {
        if (target != null)
        {
            SetState(CharacterState.Walk);
            Vector2 dir2 = target.transform.position - transform.position;
            rigid.velocity  = dir2.normalized * moveSpeed;
        }
    }
    
    // 캐릭터 충돌시 
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(collDamage);
            }
        }
    }
}
