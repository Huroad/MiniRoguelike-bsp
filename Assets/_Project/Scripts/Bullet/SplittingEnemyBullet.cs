using System.Collections;
using UnityEngine;

public class SplittingBullet : BulletBase
{
    public GameObject subBulletPrefab; // EnemyBullet 프리팹 할당
    public int splitCount = 8;
    public float splitDelay = 1f;
    public float subBulletSpeed = 8f;
    public int subBulletDamage = 5;

    void Start()
    {
        StartCoroutine(SplitAfterDelay());
    }

    IEnumerator SplitAfterDelay()
    {
        yield return new WaitForSeconds(splitDelay);

        for (int i = 0; i < splitCount; i++)
        {
            float angle = 360f * i / splitCount;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            GameObject subBullet = Instantiate(subBulletPrefab, transform.position, Quaternion.identity);
            // EnemyBullet 역시 BulletBase 상속이므로 Setup 사용
            BulletBase bulletScript = subBullet.GetComponent<BulletBase>();
            if (bulletScript != null)
                bulletScript.Setup(dir, subBulletSpeed, subBulletDamage);
        }
        Destroy(gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(_damage);
                Destroy(gameObject);
            }
        }
        base.OnTriggerEnter2D(collision); // 벽 파괴 처리
    }
}