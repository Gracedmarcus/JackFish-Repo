using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sonar : MonoBehaviour
{
    private AudioSource aSrc;
    private float sonarDistance = 5f;
    private float scanTime = -1;
    private LayerMask sonarMask;
    [SerializeField] private AudioClip sonarSend, sonarGet;
    [SerializeField] private GameObject sonarIcon;
    public List<GameObject> iconList = new List<GameObject>();
    public List<GameObject> objName = new List<GameObject>();
    public bool gizmoCheck;

    private void Awake()
    {
        aSrc = GetComponentInParent<AudioSource>();
        sonarMask = LayerMask.GetMask("sonarMask");
    }

    void Update()
    {
        if (scanTime < 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) == true)
            {
                scanTime = 0;//begins sonar
                aSrc.PlayOneShot(sonarSend, 1.0F);
                Debug.Log("Ping!");
                
            }
        }
        else
        {
            
            scanTime += Time.deltaTime;//start timer
            Collider[] sonarScan = Physics.OverlapSphere(transform.position, scanTime*2, sonarMask);
            foreach(Collider indexScan in sonarScan)
            {
                if (!objName.Contains(indexScan.gameObject)) 
                {
                    Debug.Log("Detected " + indexScan.gameObject.name  + ", as list item #" + sonarScan.Length);
                    Vector3 targetPos = indexScan.transform.position;
                    aSrc.PlayOneShot(sonarGet, 1.0F);
                    iconList.Add(Instantiate(sonarIcon, targetPos, Quaternion.Euler(90, 0, 0)));
                    objName.Add(indexScan.gameObject);
                }
            }
            if (scanTime >= sonarDistance)
            {
                foreach(GameObject sonarIcon in iconList)
                {
                    Destroy(sonarIcon);
                }
                objName.Clear();
                iconList.Clear();
                scanTime = -1;//resets
                Debug.Log("Ping ready!");
            }
        }
    }
    private void OnDrawGizmos()
    {
        if (gizmoCheck == true)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, scanTime*2);
        }
    }
}
