using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{
    public Transform cicak, tail;
    public float detectRange, stopDistance;
    [HideInInspector] public Transform target;
    [HideInInspector] public NavMeshAgent agent;

    private bool cicakVisible;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        target = cicak;
    }

    void Update()
    {
        if (tail.localScale.magnitude > 0 && tail.parent == null)
        {
            target = tail;
        } else
        {
            target = cicak;
        }

        Ray cicakRay = new Ray(transform.position, target.position - transform.position);
        cicakVisible = false;
        if (Physics.Raycast(cicakRay, out RaycastHit hit, detectRange))
        {
            if (hit.collider.CompareTag("Player"))
            {
                cicakVisible = true;
            }
        }

        if (cicakVisible)
        {
            Attack();
        }
        else
        {
            StopAttack();
            Patrol();
        }
    }

    public abstract void Attack();

    public abstract void StopAttack();

    public abstract void Patrol();
}
