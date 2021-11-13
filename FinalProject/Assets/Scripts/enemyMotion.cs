using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyMotion : MonoBehaviour
{
    //gun picking attributes
    public bool hasGun=false, reachedGun=false;
    public int numOfGunsToPick = 4;
    private int curGunIndexToSearch = 0;
    public Text msgText;

    //attacking attributes
    public  GameObject gunInHand;
    public float enemyShootingDistance = 15f, enemyDmg = 10f, CoolDownTime=2f;
    private bool inRange=false,inShooting=false;
    private ParticleSystem shootEffect;
    private RaycastHit hit;
    public AudioSource shot;



    //general 
    public NavMeshAgent agent;
    public GameObject player;
    private Animator animator; 
    private MeshRenderer gunMesh;
    public float maxHp = 100f;
    public float hp;
   public bool isDead = false;
    public Image hpBar;
    private GameObject[] guns;
    public AudioSource step;


    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        curGunIndexToSearch = Random.Range(0, numOfGunsToPick-1);

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
        if (!isDead)//make sure enemy is alive befor updating his dest and animations
        {
                if (hasGun) //chase player
                {
                    if (inRange)
                        StartCoroutine(shoot(player));
                    else
                        patrolling();
                }
                else //search for gun
                {
                    searchForWeapon();
                }
        }

    }
   

    

    void pickup(GameObject gunTopick)
    {
        gunTopick.SetActive(false);
        hasGun = true;
        gunMesh.enabled = true;
        msgText.GetComponent<msgOnScreen>().updateMsg(this.name+" picked up a gun!");
    }

    void patrolling()
    {
        if (checkIfCanShoot()) //if in range 
        {
            agent.SetDestination(this.transform.position);
            animator.SetInteger("state", 0);
            inRange = true;
        }
        else
        {
            agent.SetDestination(player.transform.position);
            animator.SetInteger("state", 1);
            if (!step.isPlaying)
                step.Play();
        }
    }

    bool checkIfCanShoot() //check line of sight and dist from player
    {
        this.transform.LookAt(player.transform);
        if (Vector3.Distance(this.transform.position, player.transform.position) <= enemyShootingDistance && Physics.Raycast(this.transform.position, this.transform.forward, out hit, enemyShootingDistance))
        {
            if (hit.transform.tag == "player")
                return true;
        }
            return false;
    }

    IEnumerator shoot(GameObject player)
    {
        if (!inShooting)
        {
            inShooting = true;
            this.transform.LookAt(player.transform);
            this.animator.SetInteger("state", 3);
            yield return new WaitForSeconds(0.3f);
            shot.Play();
            shootEffect.Play();
            player.transform.GetComponent<PlayerMovement>().getHit(enemyDmg);
            yield return new WaitForSeconds(CoolDownTime);//time betweenn shots
            this.animator.SetInteger("state", 0);
            inRange = false;
            
            inShooting = false;
        }
    }

    public void getHit(float dmgAmount)
    {
        hp -= dmgAmount;
        Debug.Log("Enemy hp=" + hp);
        hpBar.fillAmount = hp / maxHp;
        if (hp <= 0)
            die();
    }

    void die()
    {
        if (!isDead)
        {
            isDead = true;
            agent.destination = this.transform.position;
            animator.SetInteger("state", 2);
            hpBar.GetComponentInParent<Canvas>().enabled = false;
            msgText.GetComponent<msgOnScreen>().updateMsg(this.name+" died!");
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
            curGunIndexToSearch=(curGunIndexToSearch+1)%numOfGunsToPick; //search for the next gun
        }
        agent.SetDestination(guns[curGunIndexToSearch].transform.position);

        // Check if we've reached the weapon     
        if (reachedGun)
            pickup(guns[curGunIndexToSearch]);
    }
}
