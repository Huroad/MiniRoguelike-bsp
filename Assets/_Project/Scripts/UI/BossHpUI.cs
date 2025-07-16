using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BossHpUI : MonoBehaviour
{
    public Slider hpSlider;
    public EnemyBoss1 boss;
    
    public void SetBoss(EnemyBoss1 b)
    {
        if (boss != null)
            boss.OnHpChanged -= UpdateHpBar;

        boss = b;
        boss.OnHpChanged += UpdateHpBar;
        UpdateHpBar(boss.CurrentHP, boss.maxHP);
    }

    void UpdateHpBar(int current, int max)
    {
        hpSlider.value = (float)current / max;
    }
}
