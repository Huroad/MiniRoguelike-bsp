using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletNormal : BulletBase
{
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerController>();
            if (player != null && !player._isInvincible)
            {
                player.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
        base.OnTriggerEnter2D(collision); // 벽 파괴
    }
}
