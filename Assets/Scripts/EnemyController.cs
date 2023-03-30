using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    Idle,
    Agro,
    Die,
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
    //private HealthController healthController;
    private EnemyState state = EnemyState.Idle;
    private bool chase = true;

    [SerializeField]
    private Transform bulletSpawnPos;

    [SerializeField]
    private float agro_distance = 10f, chase_distance = 15f;

    [SerializeField]
    private float attack_range = 0.5f;

    [SerializeField]
    private float ranged_attack_range = 10f, min_ranged_attack_range = 5f;

    [SerializeField]
    private GameObject fireball;

    [SerializeField]
    private float minShootWaitTime = 2f, maxShootWaitTime = 5f;

    [SerializeField]
    private int health = 2;

    private float waitTime;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
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

        switch (state)
        {
            case EnemyState.Idle:
                if (distToPlayer < agro_distance)
                {
                    state = EnemyState.Agro;
                    waitTime = Time.time + Random.Range(minShootWaitTime, maxShootWaitTime);
                    anim.SetTrigger("Agro");
                    StartCoroutine(WaitForAnimation());
                }
                break;
            case EnemyState.Agro:
                navAgent.destination = playerTransform.position;

                if (distToPlayer > chase_distance)
                {
                    state = EnemyState.Idle;
                    navAgent.destination = transform.position;
                    break;
                }

                if (distToPlayer < attack_range && facingPlayer)
                {
                    anim.SetTrigger("Attack");
                    break;
                }

                if (distToPlayer < ranged_attack_range && distToPlayer > min_ranged_attack_range && facingPlayer && Time.time > waitTime)
                {
                    waitTime = Time.time + Random.Range(minShootWaitTime, maxShootWaitTime);
                    anim.SetTrigger("Ranged Attack");
                    Destroy(Instantiate(fireball, bulletSpawnPos.position, bulletSpawnPos.rotation, transform), 10f);
                }
                
                break;
            case EnemyState.Die:
                anim.SetTrigger("Death");
                navAgent.updatePosition = false;
                Destroy(gameObject, 30f);
                state = EnemyState.Dead;
                break;
        }

        anim.SetFloat("Speed", navAgent.velocity.magnitude / navAgent.speed);

        //if (distToPlayer < 1.5) navAgent.updatePosition = false;
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
        if (other.transform.IsChildOf(transform)) return;
        health--;
        if (health < 1)
        {
            state = EnemyState.Die;
        }
        else
        {
            anim.SetTrigger("Damage");
        }
        Destroy(other.gameObject);
    }
}
