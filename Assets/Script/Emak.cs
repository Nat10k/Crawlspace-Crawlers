using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emak : Enemy
{
    [SerializeField] private GameObject sandal;
    [SerializeField] List<Transform> patrolPos;
    private Coroutine shootSandal;
    public int currPatrolIdx;

    private void Start()
    {
        currPatrolIdx = 0;
        agent.destination = patrolPos[currPatrolIdx].position;
    }

    public override void Attack()
    {
        if (shootSandal == null)
        {
            shootSandal = StartCoroutine(ShootSandal());
        }
        if (Vector3.Distance(target.position, transform.position) <= stopDistance)
        {
            agent.destination = transform.position;
            transform.LookAt(target);
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        else
        {
            agent.destination = target.position;
        }
    }

    public override void StopAttack()
    {
        if (shootSandal != null)
        {
            StopCoroutine(shootSandal);
            shootSandal = null;
            agent.destination = patrolPos[currPatrolIdx].position;
        }
    }

    public override void Patrol()
    {
        if (Vector3.Distance(transform.position, patrolPos[currPatrolIdx].position) < 2f)
        {
            currPatrolIdx++;
            currPatrolIdx %= patrolPos.Count;
            agent.destination = patrolPos[currPatrolIdx].position;
        }
    }

    IEnumerator ShootSandal()
    {
        yield return new WaitForSeconds(3);
        while (true)
        {
            GameObject currSandal = Instantiate(sandal, transform.position, Quaternion.identity);
            Rigidbody sandalBody = currSandal.GetComponent<Rigidbody>();
            sandalBody.velocity = (target.position - currSandal.transform.position).normalized * 5;
            sandalBody.angularVelocity = sandalBody.transform.forward * 5;
            yield return new WaitForSeconds(0.5f);
            sandalBody.velocity += (target.position - currSandal.transform.position).normalized * 8;
            yield return new WaitForSeconds(3);
            Destroy(currSandal);
        }
    }
}
