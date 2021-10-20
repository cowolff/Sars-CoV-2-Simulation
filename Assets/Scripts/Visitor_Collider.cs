using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visitor_Collider : MonoBehaviour
{
    private VisitorAgents_2 parent;
    private Transform floor;
    private float wall_reward, visitor_reward;
    // Start is called before the first frame update
    void Start()
    {
        wall_reward = 0f;
        visitor_reward = 0f;
        parent = transform.parent.GetComponent<VisitorAgents_2>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Trigger(Collider collider)
    {
        // Calculation of the Wall Reward
        if (collider.gameObject.tag == "wall" && Vector3.Distance(collider.ClosestPointOnBounds(transform.position), transform.position) < 1.5f)
        {
            wall_reward = wall_reward - 0.02f;
            parent.add_Reward(-0.02f);
        }

        if (collider.tag == "visitor")
        {
            // Checks, whether the contact got infected or not
            if (caluculateInfection(collider))
            {
                try
                {
                    Debug.Log(parent.GetMask());
                    if(Infection.GetInfectionBA(transform.position, collider.ClosestPoint(transform.position), parent.GetMask())){
                        VisitorAgents_2 contact = collider
                            .transform
                            .parent
                            .GetComponent<VisitorAgents_2>();
                        contact.exposed();
                    }  
                }
                catch
                {

                }
            }
            // If the contact comes to close, the agent receives a negative Reward 
            // for violating the social distancing rules
            if(Vector3.Distance(collider.transform.position, transform.position) < 3f)
            {
                visitor_reward = visitor_reward - 0.01f;
                parent.add_Reward(-0.01f);
            }
        }
        if (collider.gameObject.tag == "floor")
        {
            floor = collider.transform;
        }
        if(collider.gameObject.tag == "exit" && collider.transform == parent.getTarget())
        {
            parent.EndEpisode();
        }
    }

    void OnTiggerEnter(Collider collider)
    {
        Trigger(collider);
    }

    void OnTriggerStay(Collider collider)
    {
        Trigger(collider);
    }

    private bool caluculateInfection(Collider collider)
    {
        if(parent.getStatus() == "infected")
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public Transform getFloor()
    {
        return floor;
    }

    public float get_wall_reward()
    {
        return wall_reward;
    }

    public float get_visitor_reward()
    {
        return visitor_reward;
    }

    public void set_to_zero()
    {
        wall_reward = 0f;
        visitor_reward = 0f;
    }
}