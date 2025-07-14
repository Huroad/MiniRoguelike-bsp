using System.Collections;
using UnityEngine;

public class EnemyBoss1 : EnemyBase
{
    public Transform[] shotPoints;   // 인스펙터에서 할당
    public GameObject bulletPrefab;  // EnemyBullet1 프리팹

    public float attackInterval = 1.5f;
    public float bulletSpeed = 6f;
    public int bulletDamage = 10;

    private void Start()
    {
        base.Start();
        StartCoroutine(PatternRoutine());
    }

    IEnumerator PatternRoutine()
    {
        yield return new WaitForSeconds(2f); // 등장 후 준비
        while (!isDead)
        {
            int pattern = UnityEngine.Random.Range(0, 2);
            if (pattern == 0)
                yield return StartCoroutine(ShotCirclePattern(12, 0.6f));  // 12방향, 0.6초 딜레이
            else
                yield return StartCoroutine(SnipePlayerPattern(6, 0.25f));  // 플레이어 쏘기 6연사
            yield return new WaitForSeconds(1.5f); // 패턴간 딜레이
        }
    }

    IEnumerator ShotCirclePattern(int count, float delay)
    {
        print("ShotCirclePattern");
        for (int i = 0; i < count; i++)
        {
            float angle = (360f / count) * i;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            Shoot(dir);
            yield return new WaitForSeconds(delay / count);
        }
    }

    IEnumerator SnipePlayerPattern(int count, float delay)
    {
        print("SnipePlayerPattern");
        for (int i = 0; i < count; i++)
        {
            if (target == null) yield break;
            Vector2 dir = (target.transform.position - transform.position).normalized;
            Shoot(dir);
            yield return new WaitForSeconds(delay);
        }
    }

    void Shoot(Vector2 dir)
    {
        var bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        var bullet = bulletObj.GetComponent<BulletBase>();
        bullet.Setup(dir, bulletSpeed, bulletDamage);
    }
}

