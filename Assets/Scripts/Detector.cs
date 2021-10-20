using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    Transform neighbor;
    int neighbor_id;

    private MeshRenderer renderer;
    // Start is called before the first frame update
    void Start()
    {
        renderer = transform.GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(neighbor == null){
            renderer.enabled = true;
        } else {
            renderer.enabled = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "floor" && collider.transform.parent != transform.parent)
        {
            neighbor = collider.transform.parent;
            neighbor_id = collider.transform.parent.GetInstanceID();
        }
    }

    void OnTriggerExit(Collider collider){
        if (collider.tag == "floor" && collider.transform.parent != transform.parent){
            neighbor = null;
            neighbor_id = -1;
        }
    }

    public Transform get_neighbor()
    {
        return neighbor;
    }

    public int get_neighbor_id()
    {
        return neighbor_id;
    }

    public Vector3 getAngle()
    {
        Vector3 angle = transform.eulerAngles;
        angle = new Vector3(angle.x, angle.y + 180, angle.z);
        return angle;
    }
}
