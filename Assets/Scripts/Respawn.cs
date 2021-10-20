using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTiggerEnter(Collider collider)
    {
        if (collider.tag == "visitor")
        {
            VisitorAgents_2 contact = collider.transform.parent.GetComponent<VisitorAgents_2>();
            contact.EndEpisode();
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.tag == "visitor")
        {
            VisitorAgents_2 contact = collider.transform.parent.GetComponent<VisitorAgents_2>();
            contact.EndEpisode();
        }
    }
}
