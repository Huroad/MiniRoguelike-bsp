using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
    public Slider hpSlider;
    public PlayerController player;
    
    public void SetPlayer(PlayerController p)
    {
        if (player != null)
            player.OnHpChanged -= UpdateHpBar;

        player = p;
        player.OnHpChanged += UpdateHpBar;
        UpdateHpBar(player.CurrentHP, player.maxHP);
    }

    void UpdateHpBar(int current, int max)
    {
        hpSlider.value = (float)current / max;
    }
}

