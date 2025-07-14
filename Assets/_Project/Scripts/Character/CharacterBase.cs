using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    public enum CharacterState
    {
        Idle,
        Walk,
        Attack,
        Die,
        Hit,
        Dodge
    }
    protected CharacterState characterState;
    protected Animator animator;
    
    protected Rigidbody2D rigid;
    protected SpriteRenderer spriteRenderer;
    
    public int maxHP = 100;
    protected int currentHP;
    public float moveSpeed = 5f;
    public bool isDead { get; protected set; }

    protected virtual void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        currentHP = maxHP;
        isDead = false;
    }

    protected virtual void SetState(CharacterState newState)
    {
        if (characterState == newState) return;
        characterState = newState;
        
        animator.SetBool("IsWalking", false);
        animator.SetBool("IsDead", false);
        
        switch (characterState)
        {
            case CharacterState.Idle:
                break;
            case CharacterState.Walk:
                animator.SetBool("IsWalking", true);
                break;
            case CharacterState.Hit:
                //animator.SetBool("IsHit", true);
                animator.SetTrigger("Hit");
                break;
            case CharacterState.Dodge:
                //animator.SetBool("IsDodge", true);
                animator.SetTrigger("Dodge");
                break;
            case CharacterState.Die:
                animator.SetBool("IsDead", true);
                break;
            case CharacterState.Attack:
                animator.SetTrigger("Attack");
                break;
        }
    }
    
    public virtual void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHP -= dmg;
        SetState(CharacterState.Hit);
        
        // 데미지 주석~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        print(currentHP);
        
        if (currentHP <= 0)
        {
            currentHP = 0;
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        // 공통 사망처리(이팩트)
        SetState(CharacterState.Die);
        Destroy(gameObject);
    }
}
