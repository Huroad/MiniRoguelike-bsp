using System.Collections;
using UnityEngine;

public class EnemyNormal1 : EnemyBase
{
    public GameObject bulletPrefab;
    public float shootInterval = 1.5f;
    public float bulletSpeed = 6f;
    public int bulletDamage = 10;

    private float shootTimer;

    protected override void Start()
    {
        base.Start();
        shootTimer = shootInterval;
    }

    void Update()
    {
        if (isDead) return;

        // 이동
        Move();

        // 총알 쏘기
        shootTimer -= Time.deltaTime;
        if (shootTimer <= 0f && target != null)
        {
            Shoot();
            shootTimer = shootInterval;
        }
    }

    void Shoot()
    {
        SetState(CharacterState.Attack);
        Collider2D playerCollider = target.GetComponent<Collider2D>();
        Vector2 playerCenter = playerCollider.bounds.center;
        Vector2 fireDir = (playerCenter - (Vector2)transform.position).normalized;
        
        GameObject bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        BulletBase bullet = bulletObj.GetComponent<BulletBase>();
        bullet.Setup(fireDir, bulletSpeed, bulletDamage);
    }
}