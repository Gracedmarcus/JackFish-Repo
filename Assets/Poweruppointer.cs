using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poweruppointer : MonoBehaviour
{
    public GameObject arrow1, arrow2, arrow3, arrow4;
    public Transform target1, target2, target3, target4;
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.barBool)
        {
            //new vector of targets postition
            arrow1.gameObject.transform.LookAt(target1, transform.up);
            arrow2.gameObject.transform.LookAt(target2, transform.up);
            arrow3.gameObject.transform.LookAt(target3, transform.up);
            arrow4.gameObject.transform.LookAt(target4, transform.up);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
