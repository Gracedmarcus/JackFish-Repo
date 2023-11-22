using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script by Joshua Turner
//Last Revision 22/11/2023

public class MapCamera : MonoBehaviour
{
    //Serialized variables
    [SerializeField] private Vector3 offset;

    //Private variables
    public Transform target;

    /// <summary>
    /// Find necessary references
    /// </summary>
    private void Awake()
    {
        //Find target
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        if(target != null)
        {
            if ((transform.position.y != offset.y) || (transform.position != target.position))
            {
                //Calculate desired position
                Vector3 camPos = new Vector3(target.position.x, offset.y, target.position.z);
                transform.position = Vector3.up+camPos;
            }
        }
        else
        {
            Debug.LogError("Map Camera: Camera target must be tagged as 'Player'!");
        }
    }
}