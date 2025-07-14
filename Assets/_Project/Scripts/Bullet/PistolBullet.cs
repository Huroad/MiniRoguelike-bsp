using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBullet : BulletBase
{
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            var enemy = collision.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
        base.OnTriggerEnter2D(collision);
    }
}
