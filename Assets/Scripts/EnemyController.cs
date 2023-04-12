using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Agro,
    Ranged,
    InRange,
    Attack,
    Die,
    Staggered,
    Dead
};

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
//[RequireComponent(typeof(HealthController))]
public class EnemyController : MonoBehaviour
{
    private Vector3 spawnPoint;
    private Transform playerTransform, target;
    private Animator anim;
    private NavMeshAgent navAgent;
    private Collider body;
    private ColliderChecker attackCollider;
    //private HealthController healthController;
    private EnemyState state = EnemyState.Idle;
    private bool chase = true;
    private float waitTime;
    private float attackTime;
    private bool lookAtTarget = false;
    private bool moveForward = true;
    private Coroutine action;

    [SerializeField] private float ranged_attack_range = 10f, min_ranged_attack_range = 5f;
    [SerializeField] private float ranged_attack_ratio = 0.5f;
    [SerializeField] private Transform bulletSpawnPos;
    [SerializeField] private GameObject fireball;
    [SerializeField] private float agro_distance = 10f, chase_distance = 15f;
    [SerializeField] private float attack_range = 0.5f;
    [SerializeField] private float minShootWaitTime = 0.5f, maxShootWaitTime = 2f;
    [SerializeField] private float minAttackTimeout = 5f, maxAttackTimeout = 8f;
    [SerializeField] private float attackDelay = 0.1f;
    [SerializeField] private int health = 2;
    [SerializeField] private LayerMask mask;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        body = GetComponent<Collider>();
        attackCollider = transform.Find("AttackCollider").GetComponent<ColliderChecker>();
    }

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        spawnPoint = gameObject.transform.position;
        navAgent.destination = spawnPoint;

        target = playerTransform;

        navAgent.updatePosition = true;
        navAgent.updateRotation = false;
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(playerTransform.position, transform.position);
        float playerAngle = Vector3.SignedAngle(this.transform.forward, playerTransform.transform.position - this.transform.position, Vector3.up);
        bool facingPlayer = Mathf.Abs(playerAngle) < 10f;

        //navAgent.destination = playerTransform.position;

        if (distToPlayer > chase_distance && state != EnemyState.Dead)
        {
            state = EnemyState.Idle;
            navAgent.destination = transform.position;
            lookAtTarget = false;
        }

        if (attackCollider.IsCollidingWithPlayer && action == null && state != EnemyState.Dead)
        {
            action = StartCoroutine(Attack());
        }

        //if (distToPlayer < attack_range && facingPlayer)
        //{
        //    anim.SetTrigger("Attack");
        //    playerTransform.gameObject.GetComponent<ThirdPersonShooterController>().Damage(1);
        //}

        if (lookAtTarget)
        {
            Vector3 lookPos;
            if (HasLineOfSightTo(target))
            {
                lookPos = target.position - transform.position;
                lookPos.y = 0;
            }
            else
            {
                lookPos = navAgent.steeringTarget;
            }

            Quaternion rotation = Quaternion.LookRotation(target.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, navAgent.angularSpeed * Time.deltaTime);
        }

        switch (state)
        {
            case EnemyState.Idle:
                if (distToPlayer < agro_distance)
                {
                    navAgent.destination = playerTransform.position;
                    lookAtTarget = true;

                    state = EnemyState.Agro;
                    action = StartCoroutine(Agro());
                }
                break;
            case EnemyState.Agro:
                attackTime = Time.time + Random.Range(minAttackTimeout, maxAttackTimeout);

                if (Random.value < ranged_attack_ratio)
                {
                    state = EnemyState.Ranged;
                }
                else
                {
                    state = EnemyState.Attack;
                }

                break;
            case EnemyState.Attack:
                if (Time.time > attackTime)
                {
                    state = EnemyState.Agro;
                }

                break;
            case EnemyState.Ranged:
                if (Time.time > attackTime)
                {
                    state = EnemyState.Agro;
                    break;
                }

                print(HasLineOfSightTo(playerTransform));

                if (HasLineOfSightTo(playerTransform) && distToPlayer < ranged_attack_range)
                {
                    navAgent.destination = transform.position;
                    if (action == null)
                    {
                        action = StartCoroutine(RangedAttack());
                    }
                }
                else navAgent.destination = playerTransform.position;

                break;
            case EnemyState.Staggered:
                StopCoroutine(action);
                action = StartCoroutine(Staggered());
                break;
            case EnemyState.Die:
                StopCoroutine(action);
                action = null;
                navAgent.isStopped = true;
                anim.SetTrigger("Death");
                Destroy(gameObject, 30f);
                state = EnemyState.Dead;
                body.enabled = false;
                break;
        }

        anim.SetFloat("Speed", navAgent.velocity.magnitude / navAgent.speed);

        //if (distToPlayer < 1.5) navAgent.updatePosition = false;
    }

    private IEnumerator Attack()
    {
        navAgent.isStopped = true;
        yield return new WaitForSeconds(attackDelay);
        
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(0.8f);
        
        yield return new WaitForSeconds(attackDelay);

        navAgent.isStopped = false;
        action = null;
    }

    private bool HasLineOfSightTo(Transform target)
    {
        RaycastHit hit;
        Vector3 origin = transform.position + Vector3.up * 1.5f;
        Vector3 targetV = target.position + Vector3.up * 1f;

        Debug.DrawRay(origin, (targetV - origin), Color.green);
        if (Physics.SphereCast(origin, 0.5f, (targetV - origin).normalized, out hit, ranged_attack_range, mask))
        {
            return hit.transform.tag == target.tag;
        }

        return false;
    }

    private IEnumerator RangedAttack()
    {
        navAgent.isStopped = true;
        yield return new WaitForSeconds(Random.Range(minShootWaitTime, maxShootWaitTime));
        lookAtTarget = false;

        anim.SetTrigger("Ranged Attack");
        yield return new WaitForSeconds(0.8f);

        yield return new WaitForSeconds(attackDelay);
        navAgent.isStopped = false;
        lookAtTarget = true;

        action = null;
    }

    private IEnumerator Staggered()
    {
        //yield return new WaitForSeconds(Random.Range(minShootWaitTime, maxShootWaitTime));
        navAgent.destination = transform.position;
        lookAtTarget = false;

        anim.SetTrigger("Damage");
        yield return new WaitForSeconds(0.6f);

        yield return new WaitForSeconds(attackDelay);

        state = EnemyState.Agro;
        action = null;
    }

    private IEnumerator WaitForAnimation()
    {
        //while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        //    yield return null;

        yield return new WaitForSeconds(0.8f);

        navAgent.isStopped = false;

        action = null;
    }

    private IEnumerator Agro()
    {
        navAgent.isStopped = true;

        anim.SetTrigger("Agro");
        yield return new WaitForSeconds(0.8f);

        navAgent.isStopped = false;

        action = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        Damage();
        Destroy(other.gameObject);
    }

    public void Damage()
    {
        health--;
        if (health < 1)
        {
            state = EnemyState.Die;
        }
        else
        {
            state = EnemyState.Staggered;
        }
    }

    public void OnRangedAttack()
    {
        var bullet = Instantiate(fireball, bulletSpawnPos.position, bulletSpawnPos.rotation);
        Destroy(bullet, 5f);
    }

    public void OnAttack()
    {
        if (attackCollider.IsCollidingWithPlayer)
            playerTransform.gameObject.GetComponent<ThirdPersonShooterController>().Damage(1);
    }
}
