using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : CharacterBase
{
    [Tooltip("공격할 대상")]
    public Transform target; // 플레이어 Transform (GameManager 등에서 할당)

    protected override void Awake()
    {
        base.Awake();
    }
    
    void Update()
    {
        if (isDead || target == null) return;
        
        // 간단한 이동
        // Vector2 dir = (target.position - transform.position).normalized;
        // transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        // 맞을 때 이펙트/사운드 등 추가 가능
    }

    protected override void Die()
    {
        base.Die();
        // 적 사망시 점수/드랍/파괴처리 등
        Destroy(gameObject);
    }
}
