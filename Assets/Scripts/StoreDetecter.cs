using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreDetecter : MonoBehaviour
{
    BuildingBlock parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.GetComponent<BuildingBlock>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "booth")
        {
            Debug.Log(collider.tag);
            StoreScript script = collider.transform.GetComponent<StoreScript>();
            for(int i = 0; i < script.interest; i++){
                parent.addExhibitor(collider.transform);
            }
        }
    }
}
