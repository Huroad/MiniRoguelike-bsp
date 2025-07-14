using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet1 : BulletBase
{
    public float splitDelay = 0.8f;
    public int splitCount = 5; // 퍼질 탄 개수
    
    private bool splitted = false;
    private float spawnTime;
    
    public bool canSplit = true;

    protected virtual void Start()
    {
        spawnTime = Time.time;
    }
    
    protected override void Update()
    {
        base.Update();
        if (canSplit && !splitted && Time.time - spawnTime > splitDelay)
        {
            splitted = true;
            Split();
        }
    }

    void Split()
    {
        float angleStep = 60f / (splitCount - 1);
        float startAngle = -30f;
        for (int i = 0; i < splitCount; i++)
        {
            float angle = startAngle + i * angleStep;
            Vector2 newDir = Quaternion.Euler(0, 0, angle) * dir;

            // 분열탄은 canSplit = false로 생성!
            var bulletObj = Instantiate(gameObject, transform.position, Quaternion.identity);
            var bullet = bulletObj.GetComponent<EnemyBullet1>();
            bullet.Setup(newDir, _speed, _damage);
            bullet.canSplit = false;   // <- 중요!
        }
        Destroy(gameObject); // 기존 탄은 삭제
    }
    
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
