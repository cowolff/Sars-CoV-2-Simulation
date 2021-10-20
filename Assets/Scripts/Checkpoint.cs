using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Trigger(Collider collider)
    {
        if (collider.tag == "visitor")
        {
            try
            {
                Transform agent = collider.transform.parent;
                VisitorAgents_2 script = agent.GetComponent<VisitorAgents_2>();
                if (script.checkCheckpoint(transform.GetInstanceID()))
                {

                    ///script.add_Reward(50.0f);
                }
            }
            catch
            {

            }

        }
    }

    void OnTriggerEnter(Collider collider)
    {
        Trigger(collider);
    }

    void OnTriggerStay(Collider collider)
    {
        Trigger(collider);
    }
}
