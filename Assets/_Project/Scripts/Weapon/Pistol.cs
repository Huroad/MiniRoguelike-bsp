using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : WeaponBase
{
    
    protected override void Awake()
    {
        base.Awake();
        
    }

    private void Start()
    {
        // 나중에 데이터 차트에서 값을 넣어서 적용
        fireInterval = 1f;
        fireSpeed = 5f;
        fireDamage = 10f;
    }

    public override void Fire()
    {
        base.Fire();
    }
    
    
    
}
