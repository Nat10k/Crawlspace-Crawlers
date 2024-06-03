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
    [SerializeField] TutorialTrigger trigger;
    private Coroutine attentionFallOff;

    public bool cicakVisible;

    void Awake()
    {
        cicakVisible = false;
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
        if (Physics.Raycast(cicakRay, out RaycastHit hit, detectRange) &&
            hit.collider.CompareTag("Player") && 
            Vector3.Angle(transform.forward, target.position - transform.position) < 90)
        {
            if (trigger != null)
            {
                trigger.TriggerEvent();
                trigger = null;
            }
            cicakVisible = true;
            if (attentionFallOff != null)
            {
                StopCoroutine(attentionFallOff);
                attentionFallOff = null;
            }
        } else if (cicakVisible && attentionFallOff == null)
        {
            attentionFallOff = StartCoroutine(AttentionFallOff());
            StopAttack();
        }

        if (cicakVisible)
        {
            Attack();
        }
        else
        {
            StopAttack();
            NextPatrol();
        }
    }

    IEnumerator AttentionFallOff()
    {
        yield return new WaitForSeconds(10);
        cicakVisible = false;
        attentionFallOff = null;
    }

    public abstract void Attack();

    public abstract void StopAttack();

    public abstract void NextPatrol();
}
