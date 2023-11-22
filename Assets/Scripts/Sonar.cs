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
    private List<GameObject> iconList = new List<GameObject>();
    private List<GameObject> objName = new List<GameObject>();
    public GameObject ScanGood, ScanBad;

    private void Awake()
    {
        aSrc = GetComponentInParent<AudioSource>();
        sonarMask = LayerMask.GetMask("sonarMask");
    }

    private void Start()
    {
        ScanBad.SetActive(false);
    }

    void Update()
    {
        if (scanTime < 0)
        {
            if (Input.GetKeyDown(KeyCode.Space) == true)
            {
                scanTime = 0;//begins sonar
                aSrc.PlayOneShot(sonarSend, 1.0F);
                ScanGood.SetActive(false);
                ScanBad.SetActive(true);
            }
        }
        else
        {
            
            scanTime += Time.deltaTime;//start timer
            Collider[] sonarScan = Physics.OverlapSphere(transform.position, scanTime*3, sonarMask);
            foreach(Collider indexScan in sonarScan)
            {
                if (!objName.Contains(indexScan.gameObject)) 
                {
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
                ScanGood.SetActive(true);
                ScanBad.SetActive(false);
            }
        }
    }
}
