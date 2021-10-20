using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreZone : MonoBehaviour
{
    private Transform parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerStay(Collider collider){
        if(collider.tag == "visitor"){
            try{
                Transform agent = collider.transform.parent;
                VisitorAgents_2 script = agent.GetComponent<VisitorAgents_2>();
                if(script.isTarget(parent.GetInstanceID())){
                    script.addTargetReward(0.01f);
                }
            } catch {

            }
        }
    }

    void OnTriggerEnter(Collider collider){
        if(collider.tag == "visitor"){
            try{
                Transform agent = collider.transform.parent;
                VisitorAgents_2 script = agent.GetComponent<VisitorAgents_2>();
                if(script.isTarget(parent.GetInstanceID())){
                    script.StartTimer();
                }
            } catch{

            }
        }
    }
}
