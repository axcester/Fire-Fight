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
    private Vector3 spawnPoint, destination;
    private Transform playerTransform;
    private Animator anim;
    private NavMeshAgent navAgent;
    private Collider collider;
    //private HealthController healthController;
    private EnemyState state = EnemyState.Idle;
    private bool chase = true;
    private float waitTime;
    private float attackTime;

    [SerializeField] private float ranged_attack_range = 10f, min_ranged_attack_range = 5f;
    [SerializeField] private float ranged_attack_ratio = 0.5f;
    [SerializeField] private Transform bulletSpawnPos;
    [SerializeField] private GameObject fireball;
    [SerializeField] private float agro_distance = 10f, chase_distance = 15f;
    [SerializeField] private float attack_range = 0.5f;
    [SerializeField] private float minShootWaitTime = 0.5f, maxShootWaitTime = 2f;
    [SerializeField] private float minAttackTimeout = 5f, maxAttackTimeout = 8f;
    [SerializeField] private int health = 2;


    private void Awake()
    {
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        collider = GetComponent<Collider>();
    }

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        spawnPoint = gameObject.transform.position;
        navAgent.destination = spawnPoint;

        navAgent.updatePosition = true;
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(playerTransform.position, transform.position);
        float playerAngle = Vector3.SignedAngle(this.transform.forward, playerTransform.transform.position - this.transform.position, Vector3.up);
        bool facingPlayer = Mathf.Abs(playerAngle) < 10f;

        navAgent.destination = playerTransform.position;

        if (distToPlayer > chase_distance)
        {
            state = EnemyState.Idle;
            navAgent.isStopped = true;
        }

        //if (distToPlayer < attack_range && facingPlayer)
        //{
        //    anim.SetTrigger("Attack");
        //    playerTransform.gameObject.GetComponent<ThirdPersonShooterController>().Damage(1);
        //}

        switch (state)
        {
            case EnemyState.Idle:
                if (distToPlayer < agro_distance)
                {
                    state = EnemyState.Agro;
                    //waitTime = Time.time + Random.Range(minShootWaitTime, maxShootWaitTime);
                    anim.SetTrigger("Agro");
                    StartCoroutine(WaitForAnimation());
                }
                break;
            case EnemyState.Agro:
                navAgent.isStopped = false;
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
                    break;
                }

                if (distToPlayer < attack_range && facingPlayer)
                {
                    anim.SetTrigger("Attack");
                    //playerTransform.gameObject.GetComponent<ThirdPersonShooterController>().Damage(1);
                }

                break;
            case EnemyState.Ranged:
                if (Time.time > attackTime)
                {
                    state = EnemyState.Agro;
                    break;
                }

                if (distToPlayer < ranged_attack_range && distToPlayer > min_ranged_attack_range)
                {
                    waitTime = Time.time + Random.Range(minShootWaitTime, maxShootWaitTime);
                    state = EnemyState.InRange;
                }

                break;
            case EnemyState.InRange:
                if (Time.time > attackTime)
                {
                    state = EnemyState.Agro;
                    break;
                }

                navAgent.isStopped = true;
                LookAtTarget();

                if (distToPlayer > ranged_attack_range)
                {
                    state = EnemyState.Agro;
                    break;
                }

                if (Time.time > waitTime)
                {
                    anim.SetTrigger("Ranged Attack");
                    var bullet = Instantiate(fireball, bulletSpawnPos.position, bulletSpawnPos.rotation);
                    Destroy(bullet, 10f);
                    waitTime = Time.time + Random.Range(minShootWaitTime, maxShootWaitTime);
                }
                break;
            case EnemyState.Staggered:
                if (Time.time > waitTime)
                {
                    state = EnemyState.Agro;
                }
                break;
            case EnemyState.Die:
                anim.SetTrigger("Death");
                navAgent.isStopped = true;
                Destroy(gameObject, 30f);
                state = EnemyState.Dead;
                collider.enabled = false;
                break;
        }

        anim.SetFloat("Speed", navAgent.velocity.magnitude / navAgent.speed);

        //if (distToPlayer < 1.5) navAgent.updatePosition = false;
    }

    private void LookAtTarget()
    {
        var targetPosition = navAgent.pathEndPosition;
        var targetPoint = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        var _direction = (targetPoint - transform.position).normalized;
        var _lookRotation = Quaternion.LookRotation(_direction);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, _lookRotation, 360);
    }

    private IEnumerator WaitForAnimation()
    {
        navAgent.isStopped = true;

        while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            yield return null;

        navAgent.isStopped = false;
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
            waitTime = Time.time + 1.5f;
            anim.SetTrigger("Damage");
        }
    }
}
