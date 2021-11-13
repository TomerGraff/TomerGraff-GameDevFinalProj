using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class checkWin : MonoBehaviour
{
    public GameObject enemy;
    public GameObject enemyComander;
    enemyMotion ecScript;
    enemyMotion eScript;
    public TextMeshProUGUI winText;
    public Image winImg;
    public Canvas mainCanvas;
       // Start is called before the first frame update
    void Start()
    {   //hide win img and text
        winText.gameObject.SetActive(false);
        winImg.enabled = false;
        //get the scripts that indicates if enemy is dead.
        ecScript = enemyComander.GetComponent<enemyMotion>();
         eScript = enemy.GetComponent<enemyMotion>();
    }

    // Update is called once per frame
    void Update()
    {
        if (eScript.isDead&&ecScript.isDead) //check if both enemys are dead
            StartCoroutine(winMsg());

    }

    IEnumerator winMsg()
    {
        AudioListener.pause = true;
        yield return new WaitForSeconds(3f);
        mainCanvas.enabled = false;
        winText.gameObject.SetActive(true);
        winImg.enabled = true;
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("menu");
    }
}
