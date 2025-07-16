using System;
using System.Collections;
using UnityEngine;

public class EnemyBoss1 : EnemyBase
{
    public Transform[] shotPoints;   // 인스펙터에서 할당
    public GameObject bulletPrefab;  // EnemyBullet1 프리팹

    public float attackInterval = 1.5f;
    public float bulletSpeed = 6f;
    public int bulletDamage = 10;
    
    // UI
    public event Action<int, int> OnHpChanged;
    public int CurrentHP => currentHP;
    [SerializeField] private GameObject hpObj;

    protected override void Awake()
    {
        base.Awake();
        hpObj = GameObject.Find("BossHp");
    }
    
    protected override void Start()
    {
        base.Start();
        StartCoroutine(PatternRoutine());
        StartCoroutine(BasicAttackRoutine());
        StartCoroutine(MoveAroundRoutine());

        
        BossHpUI hpUI = hpObj.GetComponent<BossHpUI>();
        //hpUI.gameObject.SetActive(true);
        hpObj.transform.Find("BossHpBar").gameObject.SetActive(true);
        hpUI.SetBoss(this);
    }

    IEnumerator BasicAttackRoutine()
    {
        while (!isDead)
        {
            Vector2 dir = (target.transform.position - transform.position).normalized;
            Shoot(dir);
            yield return new WaitForSeconds(1f);
        }
    }

    
    // IEnumerator PatternRoutine()
    // {
    //     yield return new WaitForSeconds(2f);
    //     while (!isDead)
    //     {
    //         int pattern = UnityEngine.Random.Range(0, 2);
    //         if (pattern == 0)
    //             yield return StartCoroutine(ShotCirclePattern(12, 0.6f));  // 12방향, 0.6초 딜레이
    //         else
    //             yield return StartCoroutine(SnipePlayerPattern(6, 0.25f));  // 플레이어 쏘기 6연사
    //         yield return new WaitForSeconds(1.5f); // 패턴간 딜레이
    //     }
    // }

    IEnumerator PatternRoutine()
    {
        yield return new WaitForSeconds(2f); // 등장 후 대기
        while (!isDead)
        {
            int pattern = UnityEngine.Random.Range(0, 5); // 0~4

            switch (pattern)
            {
                case 0:
                    yield return StartCoroutine(ShotCirclePattern(12, 0.6f));
                    break;
                case 1:
                    yield return StartCoroutine(SnipePlayerPattern(6, 0.25f));
                    break;
                case 2:
                    yield return StartCoroutine(SpiralPattern(20, 20f));
                    break;
                case 3:
                    yield return StartCoroutine(RandomShotPattern(10));
                    break;
                case 4:
                    yield return StartCoroutine(CrossShotPattern());
                    break;
            }

            yield return new WaitForSeconds(1.5f);
        }
    }

    
    IEnumerator ShotCirclePattern(int count, float delay)
    {
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
        for (int i = 0; i < count; i++)
        {
            if (target == null) yield break;
            Vector2 dir = (target.transform.position - transform.position).normalized;
            Shoot(dir);
            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator SpiralPattern(int count, float rotationSpeed)
    {
        float angle = 0f;
        for (int i = 0; i < count; i++)
        {
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            Shoot(dir);
            angle += rotationSpeed;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator RandomShotPattern(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float angle = UnityEngine.Random.Range(0f, 360f);
            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;
            Shoot(dir);
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator CrossShotPattern()
    {
        Vector2[] dirs = new Vector2[]
        {
            Vector2.up, Vector2.down, Vector2.left, Vector2.right
        };

        foreach (var dir in dirs)
        {
            Shoot(dir);
        }

        yield return new WaitForSeconds(0.5f);
    }
    
    IEnumerator MoveAroundRoutine()
    {
        while (!isDead)
        {
            Vector3 offset = new Vector3(UnityEngine.Random.Range(-2f, 2f), UnityEngine.Random.Range(-2f, 2f), 0);
            Vector3 dest = transform.position + offset;

            float t = 0f;
            Vector3 start = transform.position;
            while (t < 1f)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(start, dest, t);
                yield return null;
            }

            yield return new WaitForSeconds(2f);
        }
    }

    
    void Shoot(Vector2 dir)
    {
        var bulletObj = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        var bullet = bulletObj.GetComponent<BulletBase>();
        bullet.Setup(dir, bulletSpeed, bulletDamage);
    }
    
    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
        OnHpChanged?.Invoke(currentHP, maxHP);
    }
    
    protected override void Die()
    {
        base.Die();
        EndSceneUI endSceneUI = FindObjectOfType<EndSceneUI>();
        endSceneUI.ShowEnding();
    }
}

