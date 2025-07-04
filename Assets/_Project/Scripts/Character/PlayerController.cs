using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterBase
{
    public WeaponBase currentWeapon;
    private Coroutine _fireCoroutine;
    private float _lastFireTime;

    // 무적시간
    private bool _isInvincible = false;
    public float invincibleTime = 1f;
    
    protected override void Awake()
    {
        base.Awake();
        //rldr
    }
    
    void Start()
    {
        // 기본총 지급
        if (currentWeapon == null)
            currentWeapon = GetComponentInChildren<WeaponBase>();
    }
    
    void Update()
    {
        // 이동
        Move();

        // 공격
        MousePivot();
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= _lastFireTime + currentWeapon.fireInterval)
            {
                _lastFireTime = Time.time;
                currentWeapon.Fire();
            }

            // 연사 시작(코루틴)
            if (_fireCoroutine == null && currentWeapon != null)
                _fireCoroutine = StartCoroutine(FireRoutine(currentWeapon.fireInterval));
        }
        
        if (Input.GetMouseButtonUp(0) && _fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
        
        // 회피
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Dodge();
        }
        
        
        
    }

    public override void TakeDamage(int dmg)
    {
        if (_isInvincible) return;
        base.TakeDamage(dmg);
        // 추가로 무적/깜빡임/UI 연동 등 구현
    }

    protected override void Die()
    {
        base.Die();
        // 플레이어 사망 처리 (게임오버, UI)
    }
    
    // 이동
    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
            
        Vector2 moveDir = new Vector2(x, y);
        moveDir = Vector2.ClampMagnitude(moveDir, 1);
            
        Vector2 movePos = rigid.position + moveDir * moveSpeed * Time.fixedDeltaTime;
        rigid.MovePosition(movePos);
        
        if (moveDir.magnitude > 0.05f)
            SetState(CharacterState.Walk);
        else
            SetState(CharacterState.Idle);
        
        if (false == Mathf.Approximately(x, 0))
        {
            spriteRenderer.flipX = x < 0f;
        }
            
    }
    
    // 회피
    void Dodge()
    {
        SetState(CharacterState.Dodge);
        InvincibleRoutine();
    }
    
    // 공격(마우스) - 무기 교체
    public void ChangeWeapon(WeaponBase newWeapon)
    {
        currentWeapon = newWeapon;
    }
    
    IEnumerator FireRoutine(float interval)
    {
        while (true)
        {
            if (Time.time >= _lastFireTime + currentWeapon.fireInterval)
            {
                _lastFireTime = Time.time;
                currentWeapon.Fire();
            }
            yield return null;
        }
    }
    
    public void MousePivot()
    {
        Transform weaponPivot = transform.Find("WeaponPivot");
        Transform hand = weaponPivot.Find("Hand");
        
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;
        
        Vector2 dir = (mouseWorldPos - weaponPivot.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        weaponPivot.rotation = Quaternion.Euler(0, 0, angle);

        foreach (Transform weapon in hand)
        {
            if (weapon.gameObject.activeSelf)
            {
                var sr = weapon.GetComponentInChildren<SpriteRenderer>();
                if (sr != null)
                {
                    bool flip = angle > 90f || angle < -90f;
                    sr.flipY = flip;
                }
            }
        }
        
    }
    
    // 무적 코루틴
    IEnumerator InvincibleRoutine()
    {
        _isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        _isInvincible = false;
    }
    
    
}
