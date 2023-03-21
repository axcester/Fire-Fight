using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyController : MonoBehaviour
{
    private Vector3 spawnPoint;
    private Transform playerTransform;
    private Animator anim;
    private NavMeshAgent navAgent;

    [SerializeField]
    private Transform destination;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        spawnPoint = gameObject.transform.position;
        navAgent.destination = destination.position;
        navAgent.updatePosition = false;

    }

    void Update()
    {
        
    }
}
