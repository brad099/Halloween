using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UncleManager : MonoBehaviour
{

    public GameObject uitalk;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnTriggerEnter(Collider other) 
    {
        if (other.transform.CompareTag("Player"))
        {
            uitalk.SetActive(true);
        }
    }
     public void OnTriggerExit(Collider other) 
    {
        if (other.transform.CompareTag("Player"))
        {
            uitalk.SetActive(false);
        }
    }
}
