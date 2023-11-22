using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    GameManager GameManager { get; set; }
    //static bool menuActive = GameManager.menuActive;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetButton("Cancel"))
        {
            if (menuActive)
            {
                mainMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else if (!menuActive)
            {
                mainMenu.SetActive(true);
                Time.timeScale = 0;
            }
        }*/
    }
}
