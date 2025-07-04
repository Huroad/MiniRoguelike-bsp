using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    //public float fireRate;
    public GameObject bulletPrefab;
    public Transform firePoint;
    
    public float fireInterval;  // 간격
    public float fireSpeed;  // 속도
    public float fireDamage;  // 데미지
    // [HideInInspector] 가리기
    
    protected virtual void Awake()
    {
        firePoint = transform.Find("FirePoint");
    }

    public virtual void Fire()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        bullet.GetComponent<BulletBase>().Setup(fireSpeed, fireDamage);
    }
    
    
}
