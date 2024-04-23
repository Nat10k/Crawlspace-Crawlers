using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emak : Enemy
{
    [SerializeField] private GameObject sandal;
    [SerializeField] List<Transform> patrolPos;
    private Coroutine shootSandal;
    private bool cicakVisible;
    private int currPatrolIdx;
    public float detectRange, stopDistance;

    private void Start()
    {
        cicakVisible = false;
        currPatrolIdx = 0;
    }

    void Update()
    {
        Ray cicakRay = new Ray(transform.position, cicak.position - transform.position);
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
            if (shootSandal == null)
            {
                shootSandal = StartCoroutine(ShootSandal());
            }
            if (Vector3.Distance(cicak.position, transform.position) <= stopDistance)
            {
                agent.destination = transform.position;
                transform.LookAt(cicak);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
            else
            {
                agent.destination = new Vector3(cicak.position.x, transform.position.y, cicak.position.z);
            }
        }
        else
        {
            if (shootSandal != null)
            {
                StopCoroutine(shootSandal);
                shootSandal = null;
            }
            if (Vector3.Distance(transform.position, patrolPos[currPatrolIdx].position) < 0.1f)
            {
                currPatrolIdx++;
                currPatrolIdx %= patrolPos.Count;
            }
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
            sandalBody.velocity = (cicak.position - currSandal.transform.position).normalized * 10;
            yield return new WaitForSeconds(3);
            Destroy(currSandal);
        }
    }
}
