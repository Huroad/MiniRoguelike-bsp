using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BulletBase : MonoBehaviour
{
    // WeaponBase에서 값 셋
    protected float _speed;
    protected int _damage;
    protected Vector2 dir = Vector2.right;
    
    public void Setup(Vector2 dir, float speed, int damage)
    {
        this.dir = dir.normalized;
        _speed = speed;
        _damage = damage;
    }

    protected virtual void Update()
    {
        transform.Translate(dir * _speed * Time.deltaTime);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
