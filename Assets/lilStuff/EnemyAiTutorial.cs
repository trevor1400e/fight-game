using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAiTutorial : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;
    private Coroutine LookCoroutine;


    public LayerMask whatIsGround, whatIsPlayer;

    public float health;
    
    public float lookSpeed = 3f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;

    [Tooltip("Useful for rough ground")] public float GroundedOffset = -0.14f;

    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float GroundedRadius = 0.28f;

    [Tooltip("What layers the character uses as ground")]
    public LayerMask GroundLayers;

    private bool _hasAnimator;
    private Animator _animator;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDAttack;
    private int _animIDAttackType;
    private int _animIDMotionSpeed;

    // a.i player
    private float _speed;
    [SerializeField]
    private float _speedcooldown = 2f;
    private float _animationBlend;
    // private float _targetRotation = 0.0f;
    // private float _rotationVelocity;
    // private float _verticalVelocity;
    // private float _terminalVelocity = 53.0f;


    [Tooltip("Acceleration and deceleration")]
    public float SpeedChangeRate = 10.0f;

    [Header("ai Player")] [Tooltip("Move speed of the character in m/s")]
    public float MoveSpeed = 2.0f;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    [Header("Weapon")]
    [Tooltip("Weapon Script")]
    public EnemySwordCollision enemyWeaponScript;
    [Tooltip("Level Script")]
    public ArenaLevelManager LevelManager;
    [Tooltip("Time required to pass before being able to attack again. Set to 0f to instantly attack again")]
    public float AttackTimeout = 0.50f;
    
    //Attacking
    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public bool isRanged;
    //I know.. but its a mobile game so who cares if its hacked too lazy to write wrapper
    public bool invulnerable = false;
    public float iframedefault = 0.5f;
    private float _iframetimeout;
    public GameObject projectile;
    public GameObject ragdoll;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("PlayerArmature").transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        _iframetimeout = iframedefault;
        _hasAnimator = TryGetComponent(out _animator);
        LevelManager = GameObject.Find("LevelManager").GetComponent<ArenaLevelManager>();
        enemyWeaponScript = GetComponentInChildren<EnemySwordCollision>();
        AssignAnimationIDs();
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        _hasAnimator = TryGetComponent(out _animator);

        GroundedCheck();

        if (invulnerable) CountDownIframe();

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange && alreadyAttacked == false) ChasePlayer();
        if (playerInAttackRange && playerInSightRange) AttackPlayer();
        
        if(alreadyAttacked) SlowWalk();
    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;

        Move();
    }

    private void SearchWalkPoint()
    {
        //Calculate random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);

        walkPoint = new Vector3(player.position.x, transform.position.y, player.position.z);

        LookPlayerLocked();

        Move();
    }

    private void AttackPlayer()
    {
        
        //Make sure enemy doesn't move
        agent.SetDestination(transform.position);

        // LookPlayerLockedHard();
        

        if (!alreadyAttacked && _animIDSpeed <= 0.0f)
        {
            LookPlayerLocked();
            
            if (isRanged)
            {
                ///Attack code here
                Rigidbody rb = Instantiate(projectile, transform.position, transform.rotation).GetComponent<Rigidbody>();
                rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
                rb.AddForce(transform.up * 3f, ForceMode.Impulse);
            }

            // update animator if using character
            if (_hasAnimator)
            {
                int attackvar = Random.Range(1, 4);
                
                _animator.SetInteger(_animIDAttackType, attackvar);
                _animator.SetBool(_animIDAttack, true);
                enemyWeaponScript.attacking = true;
            }

            ///End of attack code
            alreadyAttacked = true;

            // Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
        
    }

    private void SlowWalk()
    {
        if (_speedcooldown > 0.0f)
        {
            _speedcooldown -= Time.deltaTime * 5;
            _animator.SetFloat(_animIDSpeed, _speedcooldown);
        }
    }
    
    private void ResetAttack()
    {
        alreadyAttacked = false;
        enemyWeaponScript.attacking = false;
        enemyWeaponScript.justAttacked = false;
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDAttack, false);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0f);
    }
    
    public void CountDownIframe()
    {
        if (_iframetimeout >= 0.0f)
        {
            _iframetimeout -= Time.deltaTime;
        }
        else
        {
            invulnerable = false;
            _iframetimeout = iframedefault;
        }
    }

    private void DestroyEnemy()
    {
        //change this later pls srry
        // ES3.Save("level", 2);
        LevelManager.enemyCount -= 1;
        Destroy(gameObject);
        CopyTransformsRecurse(transform, ragdoll.transform);
        GameObject newRagdoll;
        newRagdoll = Instantiate(ragdoll, transform.position, transform.rotation);
        newRagdoll.GetComponentInChildren<Rigidbody>().AddForce(-transform.forward * 7000f);
        Destroy(newRagdoll, 5);
    }

    private void CopyTransformsRecurse (Transform src,  Transform dst) {
        dst.position = src.position;
        dst.rotation = src.rotation;
   
        foreach (Transform child in dst) {
            // Match the transform with the same name
            var curSrc = src.Find(child.name);
            if (curSrc)
                CopyTransformsRecurse(curSrc, child);
        }
 
   
    }

    private void LookPlayerLockedHard()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        
        transform.rotation = Quaternion.LookRotation(dir);
    }
    
    
    public void LookPlayerLocked()
    {
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt());
    }

    private IEnumerator LookAt()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;
        
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        
        float time = 0;

        Quaternion initialRotation = transform.rotation;


        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(initialRotation, lookRotation, time);

            time += Time.deltaTime * lookSpeed;

            yield return null;
        }
    }
    
    
    
    

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Gizmos.DrawWireSphere(spherePosition, GroundedRadius);
        Gizmos.DrawWireSphere(walkPoint, GroundedRadius);

        // Gizmos.color = Color.green;
        // Gizmos.DrawRay(transform.position, agent.destination);
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        // _animIDJump = Animator.StringToHash("Jump");
        _animIDAttack = Animator.StringToHash("Attack");
        _animIDAttackType = Animator.StringToHash("AttackType");
        // _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
            transform.position.z);
        Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
            QueryTriggerInteraction.Ignore);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDGrounded, Grounded);
        }
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = MoveSpeed;

        _speedcooldown = 2f;

        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;


        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, 1.0f);
        }
    }

    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            // if (FootstepAudioClips.Length > 0)
            // {
            //     var index = Random.Range(0, FootstepAudioClips.Length);
            //     AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
            // }
        }
    }
    
    //attack stuff
    private void Turn_On_AttackPoint()
    {
        enemyWeaponScript.Set_Collider_Active(); 
    }
    
    private void Turn_Off_AttackPoint()
    {
        enemyWeaponScript.Set_Collider_Inactive();
        ResetAttack();
    }
    
}