using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EnemyOrc : EnemyBase
{
    // 일반 공격
    public float attackInterval = 5f;
    public float attackRange = 5f;
    private float attackCooldown;
    public int attackDamage = 10;
    
    // 스킬 공격
    public float rushInterval = 3f;
    public float rushRange = 200f;
    private float rushCooldown = 0f;
    public int rushDamage = 20;
    public float rushSpeed = 5f;
    private bool isRush = false;   // 돌진 중
    
    protected override void Awake()
    {
        base.Awake();
        moveSpeed = 2f;
    }
    
    void Update()
    {
        Move();
        if(target != null)
        {
            float dist = Vector2.Distance(transform.position, target.transform.position);
            if (dist <= attackRange)
            {
                if (Time.time >= attackCooldown)
                {
                    animator.SetTrigger("Attack");
                    attackCooldown = Time.time + attackInterval;
                }
            }
            else if (dist > rushRange)
            {
                if (Time.time >= rushCooldown)
                {
                    StartCoroutine(RushToPlayer());
                    rushCooldown = Time.time + rushInterval;
                }
            }
        }
    }

    IEnumerator RushToPlayer()
    {
        Vector2 dir = (target.transform.position - transform.position).normalized;
        float rushTime = 2f;
        float t = 0;
        while (t < rushTime)
        {
            isRush = true;
            transform.position += (Vector3)dir * rushSpeed * Time.deltaTime;
            t += Time.deltaTime;
            yield return null;
        }
        isRush = false;
    }
    
    void OnCollisionEnter2D(Collision2D other) {
        if (other.collider.CompareTag("Player")) {
            int damage = isRush ? rushDamage : attackDamage;
            target.GetComponent<CharacterBase>().TakeDamage(damage);
        }
    }
}
