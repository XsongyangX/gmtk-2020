﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patroller : MonoBehaviour
{

    /// <summary>
    /// This script lets the AI (navmesh agent) patrol using patrolTargets which is checked every frame in Update
    /// and run through a coroutine to move to the next target.
    /// 
    /// It also allows the AI to see the player (or target) using a raycast, and if it hits the target, then it can
    /// see player and chases. When in certain stopping distance, it attacks the player.
    /// 
    /// The AI stores the last known position of the target so that it continually follows the player.
    /// </summary>
    NavMeshAgent agent;
    Animator anim;
    public Transform target;
    Vector3 lastKnownPosition;
    public Transform eye;

    bool patrolling;
    public Transform[] patrolTargets;
    private int destPoints;
    bool arrived;
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        lastKnownPosition = transform.position;
    }

    bool CanSeeTarget()
    {
        bool canSee = false;
        Ray ray = new Ray(eye.position, target.transform.position - eye.position);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            if(hit.transform != target)
            {
                canSee = false;
            }
            else
            {
                lastKnownPosition = target.transform.position;
                canSee = true;
            }
        }
        return canSee;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.pathPending)
            return;

        if (patrolling) {
            if (agent.remainingDistance < agent.stoppingDistance) {
                if (!arrived)
                {
                    arrived = true;
                    StartCoroutine("GoToNextPoint");
                }
            }
            else
            {
                arrived = false;
            }
        }
        if (CanSeeTarget())
        {
            agent.SetDestination(target.transform.position);
            patrolling = false;
            if(agent.remainingDistance < agent.stoppingDistance)
            {
                anim.SetBool("Attack", true);
            }
            else
            {
                anim.SetBool("Attack", false);
            }
        }
        else
        {
            anim.SetBool("Attack", false);
            if (!patrolling)
            {
               
                agent.SetDestination(lastKnownPosition);
                if(agent.remainingDistance < agent.stoppingDistance)
                {
                    Debug.Log("works");
                    patrolling = true;
                    StartCoroutine("GoToNextPoint");
                }
            }
            
        }
        anim.SetFloat("Forward", agent.velocity.sqrMagnitude);
    }

    IEnumerator GoToNextPoint()
    { 
        if(patrolTargets.Length == 0)
        {
            yield break;
        }
       
        patrolling = true;
        yield return new WaitForSeconds(2f);
        arrived = false;
        agent.destination = patrolTargets[destPoints].position;
        destPoints = (destPoints + 1) % patrolTargets.Length;
    }
}
