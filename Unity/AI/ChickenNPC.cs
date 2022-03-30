using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenNPC : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform origin;
    [SerializeField] private NavMeshAgent agent;
    private Animator anim;
    private AudioSource AS;


    [Header("Player")]
    [SerializeField] private GameObject player;
    [SerializeField] private float distanceFromPlayer, range;

    [Header("States")]
    private bool hasTarget;
    private float randomDelay, timer;
    
    [Header("Audio")]
    [SerializeField] private AudioClip[] CluckSounds;
    private float cluckTimer, cluckFrequency;

    void Start()
    {
        anim = GetComponent<Animator>();
        AS = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Cluck();
        //Distance to player
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (!hasTarget)
        {
            MoveSomewhere();
        }
        else
        {
            if(agent.remainingDistance > 0)
            {
                FaceDirection();
                IdleCheck();
            }
            else if(agent.remainingDistance == 0)
            {
                if(timer >= randomDelay)
                {
                    hasTarget = false;
                }
                else
                {
                    FacePlayer();
                    timer += Time.deltaTime;
                }
            }
        }
    }

    //Turn towards target point
    void FaceDirection()
    {
        Vector3 direction = (agent.destination - transform.position).normalized;
        Quaternion lookAtTarget = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTarget, Time.deltaTime * 10f);
    }

    //Turn towards player
    void FacePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        Quaternion lookatPlayer = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookatPlayer, Time.deltaTime * 5f);

        anim.SetBool("Turn Head", true);
        anim.SetBool("Run", false);
    }

    //Find a random point in a sphere, locate it on the baked navmesh and set it as the target
    void MoveSomewhere()
    {
        Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * range;
        randomDirection += origin.position;

        //Sample point on Navmesh in sphere radius
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, range, -1);
        agent.SetDestination(navHit.position);
        hasTarget = true;
        randomDelay = Random.Range(0f, 3f);
        timer = 0;

        anim.SetBool("Run", true);
        anim.SetBool("Turn Head", false);
    }

    void IdleCheck()
    {
        if(agent.speed == 0)
        {
            MoveSomewhere();
        }
    }

    //Clucking sounds
    private void Cluck()
    {
        if(cluckTimer >= cluckFrequency)
        {
            int randomNum = Random.Range(0, CluckSounds.Length);
            AS.PlayOneShot(CluckSounds[randomNum]);
            cluckTimer = 0;
            cluckFrequency = Random.Range(1f, 4f);
        }
        else
        {
            cluckTimer += Time.deltaTime;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, distanceFromPlayer);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(origin.position, range);
    }
}