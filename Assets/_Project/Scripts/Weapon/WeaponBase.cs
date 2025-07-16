using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum WeaponType
{
    Pistol,
    Shotgun,
    AK47,
    AWM
}
public abstract class WeaponBase : MonoBehaviour
{
    //public float fireRate;
    public GameObject bulletPrefab;
    public List<Transform> shotPoint;
    [HideInInspector] public GameObject player;
    
    public float shotInterval;  // 간격
    public float shotSpeed;  // 속도 (플레이어 속도를 곱해서 사용 예정)
    public int shotDamage;  // 데미지
    
    public WeaponType weaponType;
    
    /*protected virtual void Awake()
    {
        
    }*/
    
    protected virtual void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        shotSpeed = player.GetComponent<PlayerController>().moveSpeed * shotSpeed;
        
    }
    
    public virtual void Fire()
    {
        foreach (var sp in shotPoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, sp.position, sp.rotation);
            bullet.GetComponent<BulletBase>().Setup(Vector2.right, shotSpeed, shotDamage);
        }
    }
    
    
}
