using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController controller;
    private float x, z;
    public float speed = 10f, groundDistance = 1f;
    public Transform groundCheck;
    public LayerMask groundMask;
    private bool onGround;
    public float gravity = -9.8f, jumpHeight = 3f, hp, maxHp = 100;
    private Vector3 velocity;
    private AudioSource step;
    public Image hpBar;
    public Image gameOver;
    public TextMeshProUGUI gameOverText;
   
   
    // Start is called before the first frame update
    void Start()
    {
        gameOverText.gameObject.SetActive(false);
        gameOver.enabled = false;
        hp = maxHp;
        step = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (onGround && velocity.y < 0)//reset the y velocity when player on the ground
            velocity.y = -2;

        x = Input.GetAxis("Horizontal");
        z = Input.GetAxis("Vertical");
        Vector3 moveDirection = x * transform.right + z * transform.forward;
        controller.Move(moveDirection * speed * Time.deltaTime);
        onGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (onGround && !step.isPlaying&&(z < -0.1 || z > 0.1))
            step.Play();

        //****jumping****
        if (Input.GetKeyDown(KeyCode.LeftAlt) && onGround)
            velocity.y = Mathf.Sqrt(gravity * jumpHeight * -2);


        velocity.y += gravity * Time.deltaTime;//twice delta time because of the free fall equasion
        controller.Move(velocity * Time.deltaTime);
    }

    public void getHit(float dmgAmount)
    {
        hp -= dmgAmount;
        Debug.Log("My hp=" + hp);
         hpBar.fillAmount = hp / maxHp;
        if (hp <= 0)
            StartCoroutine(die());
    }

    IEnumerator die()
    {
        gameOverText.gameObject.SetActive(true);
        gameOver.enabled = true;
        Debug.Log("gameOver");
        //need to mute al sound's
        AudioListener.pause = true;

        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("menu");
    }

  
}
