using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Distance_Collider : MonoBehaviour
{

    public GameObject obj;
    private VisitorAgent parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<VisitorAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Trigger(GameObject obj)
    {
        float distance = Vector3.Distance(transform.position, obj.transform.position);
        if (obj.tag == "high")
        {
            parent.add_Reward(1.5f * (23-distance));
        }
        if (obj.tag == "medium")
        {
            parent.add_Reward(1f * (23-distance));
        }
        if (obj.tag == "low")
        {
            parent.add_Reward(0.5f * (23-distance));
        }
       
    }

    void onTriggerEnter(Collision other)
    {
        Trigger(other.gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        if (new List<string>() { "high", "medium", "low", "wall" }.Contains(other.gameObject.tag))
        {
            Trigger(other.gameObject);
            return;
        }
        if (other.gameObject.transform.parent.gameObject)
        {
            GameObject agent = other.gameObject;
            Trigger(agent.transform.parent.gameObject);
            return;
        }
    }
}
