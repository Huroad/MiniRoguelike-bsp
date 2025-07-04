using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    // WeaponBase에서 값 셋
    private float _speed;
    private float _damage;
    
    public void Setup(float speed, float damage)
    {
        _speed = speed;
        _damage = damage;
    }

    protected virtual void Update()
    {
        transform.Translate(Vector2.right * _speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // 데미지 처리, 파괴 등
        print(collision.gameObject.name);
        Destroy(gameObject);
    }
}
