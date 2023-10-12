using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour
{
    private CharacterController _characterController;
    private PlayerInput _playerInput;
    private Animator _animator;
    
    [SerializeField] private float MoveSpeed = 5f;
    [SerializeField] private float Gravity = -9.8f;
    
    //Movement
    private Vector3 _movementVelocity;
    private float _verticalVelocity;
    
    //敌人 Enemy Variable 
    public bool isPlayer = true;
    private NavMeshAgent _navMeshAgent;
    private Transform TargetPlayer;
    
    //Player Attack Slide
    private float attackStartTime;
    public float AttackSlideDuration = 0.4f;
    public float AttackSlideSpeed = 0.06f;

    //Health
    private Health _health;

    private DamageCaster _damageCaster;
    
    //状态机
    public enum CharacterState
    {
        Normal,Attacking,Dead,BeingHit,Slide,Spawn
    }

    public CharacterState CurrentState;
    
    //Material
    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;
    
    
    //Drop Item
    public GameObject ItemToDrop;
    
    //Impact
    private Vector3 ImpactOnCharacter;
    public float EnemyAttackForce = 10f;
    
    //Being AttackInvincible
    public bool isInvincible;
    public float invincibleDuration = 2f;
    
    //Combo
    private float attackAnimationDuration;
    
    //Slide
    public float SlideSpeed = 9f;
    
    //Spawn
    public float SpawnDuration = 2f;
    private float currentSpawnTime;
    
    //Coin 
    public int Coin;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _health = GetComponent<Health>();
        _damageCaster = GetComponentInChildren<DamageCaster>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);
        
        
        if (!isPlayer)
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = MoveSpeed;
            SwitchStateTo(CharacterState.Spawn);
        }
        else
        {
            _playerInput = GetComponent<PlayerInput>();
        }
        
    }

    private void CalculatePlayerMovement()
    {
        if (_playerInput.SpaceButtonDown && _characterController.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }
        
        //如果鼠标被点击并且角色在地面上，进入攻击状态并且推出。
        if (_playerInput.MouseButtonDown && _characterController.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        
        
        
        _movementVelocity.Set(_playerInput.HorizontalInput,0f,_playerInput.VerticalInput);
        //归一化运动速度，使得在对角线上速度一致
        _movementVelocity.Normalize();
        
        //匹配旋转速度与相机速度
        _movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;
        
        //播放动画
        _animator.SetFloat("Speed", _movementVelocity.magnitude);
        _movementVelocity *= MoveSpeed * Time.deltaTime;

        //角色转向
        if (_movementVelocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        
        _animator.SetBool("AirBorne", !_characterController.isGrounded);
        
        
    }


    private void CalculateEnemyMovement()
    {
        if (Vector3.Distance(TargetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            //_navMeshAgent会将敌人引到目标玩家的位置
            _navMeshAgent.SetDestination(TargetPlayer.position);
            //播放动画
            _animator.SetFloat("Speed",0.2f);
        }
        else
        {
            //让敌人呆在原地
            _navMeshAgent.SetDestination(transform.position);
            //不播放动画
            _animator.SetFloat("Speed",0f);
            //切换状态
            SwitchStateTo(CharacterState.Attacking);
        }

        
    }
    
    
    
    private void FixedUpdate()
    {

        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (isPlayer)
                    CalculatePlayerMovement();
                else
                    CalculateEnemyMovement();
                break;
            
            case CharacterState.Attacking:
                if (isPlayer)
                {
                    if (Time.time < attackStartTime + AttackSlideDuration)
                    {
                        float timepassed = Time.time - attackStartTime;
                        float lerptime = timepassed / AttackSlideDuration;
                        //Vector3.Lerp是一个插值函数，使用0~1在两个向量之间插值。这里是基于角色朝向和零向量进行插值。
                        _movementVelocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerptime);
                    }

                    //翻滚中断攻击
                    if (_playerInput.SpaceButtonDown && _characterController.isGrounded)
                    {
                        SwitchStateTo(CharacterState.Slide);
                        break;
                    }
                    
                    
                    //会出现Combo1直接跳转到Combo3的情况，需要修改。
                    if (_playerInput.MouseButtonDown && _characterController.isGrounded)
                    {
                        //获取正在播放的动画名称
                        string currentClipName = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
                        //获取该动画播放的时间，1 代表在动画的结尾，0.5 代表中间。
                        attackAnimationDuration = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        //只用当Combo_1或者Combo_2的时候才能执行Combo的切换
                        if (currentClipName != "LittleAdventurerAndie_ATTACK_03 " && attackAnimationDuration > 0.5f && attackAnimationDuration < 0.7f)
                        {
                            _playerInput.MouseButtonDown = false;
                            SwitchStateTo(CharacterState.Attacking);
                            
                            CalculatePlayerMovement();
                        }
                    }
                    
                   
                }
                break;
            
            case CharacterState.Dead:
                
                //直接退出，停止以下所有代码的执行
                return;
            
            case CharacterState.BeingHit:
                break;
            
            case CharacterState.Slide:
                _movementVelocity = transform.forward * SlideSpeed * Time.deltaTime;
                break;
            case CharacterState.Spawn:
                currentSpawnTime -= Time.deltaTime;
                if (currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
                
            
        }
        
        //角色受击时受力推动
        if (ImpactOnCharacter.magnitude > 0.2f)
        {
            _movementVelocity = ImpactOnCharacter * Time.deltaTime;
        }
        ImpactOnCharacter = Vector3.Lerp(ImpactOnCharacter,Vector3.zero,Time.deltaTime * 5);

        if (isPlayer)
        {
            //判断角色是否在地板上。即使角色在地板上，也要将角色移下来一点点，因为角色下一帧会漂浮在空中，isGrounded会变成false，会导致一系列其他问题
            if (_characterController.isGrounded == false)
                _verticalVelocity = Gravity;
            else
                _verticalVelocity = Gravity * 0.3f;
        
            //_verticalVelocity是浮点类型，无法与Vector3直接相加，需要使用Vector3.up对其进行变换。
            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime; 
            _characterController.Move(_movementVelocity);
            
            _movementVelocity = Vector3.zero;
        }
        else
        {
            if (CurrentState != CharacterState.Normal)
            {
                _characterController.Move(_movementVelocity);
                _movementVelocity = Vector3.zero;
            }
        }
        
        
    }

    //状态机
    public void SwitchStateTo(CharacterState newState)
    {
        if (isPlayer)
        {
            //Clear Cache 
            _playerInput.ClearCache();
        }
        
        
        //Exiting State,检查退出状态下的状态，并实现相关功能
        switch (CurrentState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if (_damageCaster != null)
                {
                    DisableDamageCaster();
                }

                if (isPlayer)
                {
                    GetComponent<PlayerVFXManager>().StopBlade();
                }
                
                break;
            
            case CharacterState.Dead:
                //当角色死亡时，我们希望代码在这里停止，接下来的部分都不会执行
                return;
            
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                isInvincible = false;
                break;
            
        }
        
        //Enter State,检查进入状态下的状态，并实现相关功能
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:

                //敌人攻击结束后转向到玩家。
                if (!isPlayer)
                { 
                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position - transform.position);
                    transform.rotation = newRotation;

                }
                
                _animator.SetTrigger("Attack");
                if (isPlayer)
                {
                    attackStartTime = Time.time;
                    
                    //角色朝向鼠标位置移动
                    RotateToCursor();
                }
                break;
            
            case CharacterState.Dead:
                //禁用CharacterController
                _characterController.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                
                //当敌人死去，禁止其画面抖动的效果
                if (!isPlayer)
                {
                    SkinnedMeshRenderer meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
                    meshRenderer.gameObject.layer = 0;
                }
                
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");
                if (isPlayer)
                {
                    isInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;
            case CharacterState.Spawn:
                isInvincible = true;
                currentSpawnTime = SpawnDuration;
                StartCoroutine(MaterialAppear());
                break;
            
        }

        CurrentState = newState;

        Debug.Log("Switched to " + CurrentState);
    }

    public void AttackAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void BeingHitAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }
    
    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }
    
    public void ApplyDamage(int damage, Vector3 attackerPos = new Vector3())
    {
        if (isInvincible)
        {
            return;
        }
        
        if (_health != null)
        {
            _health.ApplyDamage(damage);
        }

        if (!isPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackerPos);
            
        }
        
        StartCoroutine(MaterialBlink());

        if (isPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackerPos, EnemyAttackForce);
        }
        else
        {
            AddImpact(attackerPos,2.5f);
        }
        
        
    }

    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(invincibleDuration);
        isInvincible = false;
    }
    
    
    private void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        ImpactOnCharacter = impactDir * force;
    }
    

    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }
    
    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink()
    {
        //使角色看起来是白色的
        _materialPropertyBlock.SetFloat("_blink",0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);
        
        //还原
        _materialPropertyBlock.SetFloat("_blink",0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHeight_start = 20f;
        float dissolveHeigth_target = -10f;
        float dissolveHeight;
        
        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeigth_target,
                currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height",dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        DropItem();
    }

    public void DropItem()
    {
        if (ItemToDrop != null)
        {
            Instantiate(ItemToDrop, transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.Type)
        {
            case PickUp.PickUpType.Heal:
                AddHeal(item.Value);
                break;
            case PickUp.PickUpType.Coin:
                AddCoin(item.Value);
                break;
        }
    }

    public void AddHeal(int health)
    {
        _health.AddHealth(health);
        GetComponent<PlayerVFXManager>().PlayHeal();
    }

    public void AddCoin(int value)
    {
        Coin += value;
    }

    public void RotateToPlayer()
    {
        if (CurrentState != CharacterState.Dead)
        {
            transform.LookAt(TargetPlayer,Vector3.up);
        }
    }

    IEnumerator MaterialAppear()
    {
        float dissolveTimeDuration = SpawnDuration;
        float currentDissolveTime = 0;
        float dissolveHeight_start = -10f;
        float dissolveHeigth_target = 20f;
        float dissolveHeight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHeight = Mathf.Lerp(dissolveHeight_start, dissolveHeigth_target,
                currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height",dissolveHeight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
            
        }
        
        _materialPropertyBlock.SetFloat("_enableDissolve",0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
        
    }


    //将射线绘画出来
    private void OnDrawGizmos()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hit.point;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(cursorPos,1);
        }
        
    }

    private void RotateToCursor()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray,out hit,1000,1<<LayerMask.NameToLayer("CursorTest")))
        {
            Vector3 cursorPos = hit.point;
            transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        }
        
    }
    
    
}
