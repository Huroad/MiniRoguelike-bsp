using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterBase
{
    public WeaponBase currentWeapon;
    private Coroutine _fireCoroutine;
    private float _lastFireTime;

    public List<GameObject> weaponPrefabs;
    private List<GameObject> weaponInstance = new List<GameObject>();
    private int currentWeaponIndex = 0;

    // 무적시간
    [HideInInspector]public bool _isInvincible = false;
    public float invincibleTime = 3f;
    
    // UI
    public event Action<int, int> OnHpChanged;
    public int CurrentHP => currentHP;
    [SerializeField] private WeaponUI weaponUI;
    
    protected override void Awake()
    {
        base.Awake();
        
        OnHpChanged?.Invoke(currentHP, maxHP);
    }
    
    void Start()
    {
        Transform hand = transform.Find("WeaponPivot/Hand");
        foreach (var prefab in weaponPrefabs)
        {
            GameObject weapon = Instantiate(prefab, hand);
            weapon.SetActive(false);
            weaponInstance.Add(weapon);
        }

        weaponUI = FindObjectOfType<WeaponUI>();
        
        // 첫 무기만 활성화
        if (weaponInstance.Count > 0)
        {
            currentWeaponIndex = 0;
            weaponInstance[currentWeaponIndex].SetActive(true);
            currentWeapon = weaponInstance[currentWeaponIndex].GetComponent<WeaponBase>();
            
            WeaponBase currentWeaponBase = currentWeapon;
            weaponUI.ShowWeaponUI(currentWeaponBase.weaponType);
        }
    }
    
    void Update()
    {
        // 이동
        Move();

        // 공격
        MousePivot();
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time >= _lastFireTime + currentWeapon.shotInterval)
            {
                _lastFireTime = Time.time;
                currentWeapon.Fire();
            }

            // 연사 시작(코루틴)
            if (_fireCoroutine == null && currentWeapon != null)
                _fireCoroutine = StartCoroutine(ShotRoutine(currentWeapon.shotInterval));
        }
        
        if (Input.GetMouseButtonUp(0) && _fireCoroutine != null)
        {
            StopCoroutine(_fireCoroutine);
            _fireCoroutine = null;
        }
        
        // 무기 스왑
        if (Input.GetKeyDown(KeyCode.Q)) SwapWeapon(-1);
        if (Input.GetKeyDown(KeyCode.E)) SwapWeapon(1);
        
        // 회피
        if (Input.GetKeyDown(KeyCode.Space) && !_isInvincible)
        {
            Dodge();
        }
        
        
        
    }

    public override void TakeDamage(int dmg)
    {
        if (!_isInvincible)
        {
            base.TakeDamage(dmg);
            OnHpChanged?.Invoke(currentHP, maxHP);
        }
        // 추가로 무적/깜빡임/UI 연동 등 구현
    }

    protected override void Die()
    {
        base.Die();
        // 플레이어 사망 처리 (게임오버, UI)
        EndSceneUI endSceneUI = FindObjectOfType<EndSceneUI>();
        endSceneUI.ShowEnding();
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
        StartCoroutine(InvincibleRoutine());
    }
    
    // 무기 스왑 함수
    public void SwapWeapon(int direction)
    {
        if (weaponInstance.Count == 0) return;
        
        weaponInstance[currentWeaponIndex].SetActive(false);
        currentWeaponIndex = (currentWeaponIndex + direction + weaponInstance.Count) % weaponInstance.Count;
        weaponInstance[currentWeaponIndex].SetActive(true);
        currentWeapon = weaponInstance[currentWeaponIndex].GetComponent<WeaponBase>();
        
        WeaponBase currentWeaponBase = currentWeapon;
        weaponUI.ShowWeaponUI(currentWeaponBase.weaponType);
    }
    
    IEnumerator ShotRoutine(float interval)
    {
        while (true)
        {
            if (Time.time >= _lastFireTime + currentWeapon.shotInterval)
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
