using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class msgOnScreen : MonoBehaviour
{
    public float timeOnScreen = 3f;
    private Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<Text>();
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateMsg(string msg)
    {
        StartCoroutine(showMsg(msg));
    }

    IEnumerator showMsg(string msg)
    {
        text.text = msg;
        yield return new WaitForSeconds(timeOnScreen);
        text.text = "";
    }
    
}
