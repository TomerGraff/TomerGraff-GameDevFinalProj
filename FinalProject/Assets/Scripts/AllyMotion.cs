using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AllyMotion : MonoBehaviour
{
    //gun picking attributes
    public bool hasGun = false, reachedGun = false;
    public int numOfGunsToPick = 4;
    private int curGunIndexToSearch = 0;
    public Text msgText;

    //attacking attributes
    public GameObject gunInHand;
    public float shootingDistance = 15f, allyDmg = 5f,coolDownTime=2f;
    private bool inShooting = false;
    private ParticleSystem shootEffect;
    private RaycastHit hit;
    public AudioSource shot;



    //general 
    public NavMeshAgent agent;
    public GameObject player, target1, target2;
    private Animator animator;
    private MeshRenderer gunMesh;
    private GameObject[] guns;
    public AudioSource step;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        curGunIndexToSearch = (Random.Range(0, numOfGunsToPick - 1)+1)%numOfGunsToPick;

        //creating an arr of all the guns to pick up
        guns = new GameObject[numOfGunsToPick];
        for (int i = 0; i < numOfGunsToPick; i++)
        {
            guns[i] = GameObject.Find("GunOnGround (" + i + ")");
        }

        //gun in hand handeling
        gunMesh = gunInHand.GetComponent<MeshRenderer>();
        gunMesh.enabled = false;
        shootEffect = gunInHand.GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        
            if (hasGun) //escort player
            {
                    patrolling();
            }
            else //search for gun
            {
                searchForWeapon();
            }
        

    }




    void pickup(GameObject gunTopick)
    {
        gunTopick.SetActive(false);
        hasGun = true;
        gunMesh.enabled = true;
        msgText.GetComponent<msgOnScreen>().updateMsg(this.name + " picked up a gun!");
    }

    void patrolling()
    {

        if (checkDist(player)) //if in range for escorting player 
        {
            agent.SetDestination(this.transform.position);
            animator.SetInteger("state", 0);
            //check if targets are in shooting dist
            if (checkIfCanShoot(target1))
                StartCoroutine(shoot(target1));
            else if (checkIfCanShoot(target2))
                StartCoroutine(shoot(target2));

        }
        else
        {
            agent.SetDestination(player.transform.position);
            animator.SetInteger("state", 1);
            if (!step.isPlaying)
                step.Play();
        }
    }

    bool checkIfCanShoot(GameObject target) //check line of sight and dist from player
    {
        this.transform.LookAt(target.transform);
        if (Vector3.Distance(this.transform.position, target.transform.position) <= shootingDistance && Physics.Raycast(this.transform.position, this.transform.forward, out hit, shootingDistance))
        {
            if (hit.transform.tag == "enemyComander")
                return true;
        }
        return false;
    }

    //check if the ally is in escrot range;
    bool checkDist(GameObject obj)
    {
        if (Vector3.Distance(this.transform.position, obj.transform.position) <= shootingDistance)
            return true;
        
        else
            return false;
    }

    IEnumerator shoot(GameObject target)
    {
        if (!inShooting)
        {
            inShooting = true;
            this.transform.LookAt(target.transform);
            this.animator.SetInteger("state", 3);
            yield return new WaitForSeconds(0.3f);
            shot.Play();
            shootEffect.Play();
            target.transform.GetComponent<enemyMotion>().getHit(allyDmg);
            yield return new WaitForSeconds(coolDownTime);//time betweenn shots
            this.animator.SetInteger("state", 0);
            inShooting = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "gunOnGround")
            reachedGun = true;
    }

    void searchForWeapon()
    {
        if (!step.isPlaying)
            step.Play();
        if (!guns[curGunIndexToSearch].active) //if the gun that enemy is looking for is already picked up (disabled) then search for the next one
        {
            curGunIndexToSearch = (curGunIndexToSearch + 1) % numOfGunsToPick; //search for the next gun
        }
        agent.SetDestination(guns[curGunIndexToSearch].transform.position);

        // Check if we've reached the weapon     
        if (reachedGun)
            pickup(guns[curGunIndexToSearch]);
    }
}
