using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
{
    private bool gunInHand = false, canThrow = true;
    private ParticleSystem muzzleFire;
    public float shootingDmg = 50f, shootingRange = 15f;
    public GameObject camera;
    private RaycastHit hit;
    public GameObject hitEffect;
    public Text uiMsg;
    MeshRenderer gunMesh;
    public float grenadeAmount = 0f, throwingRange = 10f, throwingHeight = 6f, grenadeDmg = 100f, dmgRadius = 5f;
    public Text amount;
    public GameObject grenadeInHand;
    public ParticleSystem grenadeExplosion;
    private AudioSource shot;
    private AudioSource explosion;
    

    // Start is called before the first frame update
    void Start()
    {
        muzzleFire = GetComponentInChildren<ParticleSystem>();
        gunMesh = this.gameObject.GetComponent<MeshRenderer>();
        shot = GetComponent<AudioSource>();
        explosion = grenadeInHand.GetComponent<AudioSource>();
        gunMesh.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {   //shooting code
        if (gunInHand && Input.GetKeyDown(KeyCode.Space))
            shoot();
        //throwing grenade code
        if (grenadeAmount > 0 && Input.GetKeyDown(KeyCode.Q) && canThrow)
            throwGrenade();
        //pick up code
        if ( Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, shootingRange))//check if hit is not empty
        {
            
            if (!gunInHand && hit.collider.tag == "gunOnGround")//check if hit = gun that we can pick
            {
                uiMsg.text = "Pick up gun [e]"; //show pickup msg
                uiMsg.enabled = true;
                if (Input.GetKeyDown(KeyCode.E)) //pickup gun
                    pickUpGun(hit.transform.gameObject);
            }
            else if (hit.collider.tag == "grenadeOnGround")//check if hit = grenade that we can pick
            {
                uiMsg.text = "Pick up grenade [e]"; //show pickup msg
                uiMsg.enabled = true;
                if (Input.GetKeyDown(KeyCode.E)) //pickup grenade
                    pickUpGrenade(hit.transform.gameObject);
            }
            else
                uiMsg.enabled = false;
        }


    }   
        
    void shoot()
    {
        muzzleFire.Play();
        shot.Play();
        if (Physics.Raycast(camera.transform.position, camera.transform.forward, out hit, shootingRange))//check if hit is not empty
        {
            enemyMotion e  = hit.transform.GetComponent<enemyMotion>();
            if (e != null)//meaning we hit an enemy that can be dmged
            {
                e.getHit(shootingDmg);
            }
            //**hit impact**
           GameObject cloneHitEffect= Instantiate(hitEffect, hit.point,Quaternion.LookRotation(hit.normal)); //hit impact is normalized (out of hit surface)
            cloneHitEffect.GetComponent<ParticleSystem>().Play();
            Destroy(cloneHitEffect,1f); //destroy every clone of the hit effect
        }
    }

    void throwGrenade()
    {
        canThrow = false;
        grenadeAmount--;
        amount.text = "grenades: " + grenadeAmount;
        grenadeInHand.transform.position = this.transform.position;
        activeGrenadeMesh(true); //make grenade visable while throwing
        float x, y, z;
        x = camera.transform.forward.x * throwingRange;
        y = throwingHeight;
        z = camera.transform.forward.z * throwingRange;
        grenadeInHand.GetComponent<Rigidbody>().AddForce(x, y, z, ForceMode.Impulse);
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(2);
       
        Collider[] targetsHit = Physics.OverlapSphere(grenadeInHand.transform.position, dmgRadius); //get all target effected from the explosion
        foreach(Collider enemy in targetsHit)
        {
            
            if (enemy.tag== "enemyComander" || enemy.tag =="enemy") //check if the colider is an enemy
            {
                enemy.GetComponent<enemyMotion>().getHit(grenadeDmg);
            }
        }
        grenadeExplosion.Play();
        explosion.Play();
        activeGrenadeMesh(false); //make grenade  not visable after explode
        yield return new WaitForSeconds(1.2f); //throw cool down
        canThrow = true;
    }
    void pickUpGun(GameObject gun)
    {
        gunInHand = true;
        gun.SetActive(false);
        gunMesh.enabled = true;
        uiMsg.enabled = false;
    }
    void pickUpGrenade(GameObject granade)
    {
        grenadeAmount++;
        amount.text = "grenades: " + grenadeAmount;
        granade.SetActive(false);
    }

   void activeGrenadeMesh(bool state)
    {
        MeshRenderer[] grenadeMeshes = grenadeInHand.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer i in grenadeMeshes)
        {
            i.enabled = state;
        }
    }


}

