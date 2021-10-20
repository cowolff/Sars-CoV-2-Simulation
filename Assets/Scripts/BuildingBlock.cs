using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    List<Detector> detectors;
    List<Transform> exhibitors;
    public string type;

    public GameObject UI;
    // Start is called before the first frame update

    private int i;
    void Start()
    {
        detectors = new List<Detector>();
        i = 0;
        List<Transform> children = GetChildObject(transform, "trigger");
        foreach (Transform child in children)
        {
            Detector new_detector = child.GetComponent<Detector>();
            detectors.Add(new_detector);
        }
        exhibitors = new List<Transform>();
    }

    public void addExhibitor(Transform exhibitor)
    {
        exhibitors.Add(exhibitor);
    }

    // Update is called once per frame
    void Update()
    {
        i = i + 1;
        if(i < 100){
            int j = 0;
            while (j < exhibitors.Count){
                if(exhibitors[j] == null){
                    exhibitors.RemoveAt(j);
                    j = j + 1;
                }
                j = j + 1;
            }
        }
        if(i == 100){
            i = 0;
        }
    }

    private List<Transform> GetChildObject(Transform parent, string tag)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in parent)
        {
            if (child.tag == tag)
            {
                children.Add(child);
            }
        }
        return children;
    }

    public Transform get_floor()
    {
        List<Transform> floor = GetChildObject(transform, "floor");
        return floor[0];
    }

    public int get_floor_id()
    {
        Transform floor = GetChildObject(transform, "floor")[0];
        return floor.GetInstanceID();
    }

    public List<Transform> get_Neighbors()
    {
        int own_id = get_floor_id();
        List<Transform> neighbors = new List<Transform>();
        foreach(Detector dete in detectors)
        {
            if(dete.get_neighbor_id() != own_id)
            {
                Transform neighbor = dete.get_neighbor();
                if (neighbor != null)
                {
                    neighbors.Add(neighbor);
                }
            }
        }
        return neighbors;
    }

    public List<Transform> getExhibitors()
    {
        return exhibitors;
    }

    public GameObject getUI(){
        return UI;
    }

    public void setUI(GameObject obj){
        UI = obj;
    }

    public void setLength(float length){
        if(type == "hallway"){
            Transform floor = transform.GetChild(0);
            Transform wall1 = transform.GetChild(1);
            Transform wall2 = transform.GetChild(2);
            Transform collider1 = transform.GetChild(3);
            Transform collider2 = transform.GetChild(4);
            Transform stores = transform.GetChild(5);

            float current_length = floor.GetComponent<Collider>().bounds.size.x;
            float z = floor.GetComponent<Collider>().bounds.size.z;

            floor.localScale = new Vector3(length, 1f, z);
            wall1.localScale = new Vector3(length, 7f, 0.5f);
            wall2.localScale = new Vector3(length, 7f, 0.5f);
            stores.localScale = new Vector3(length, 2f, z);

            float rel = length / current_length;

            Vector3 relPosCol1 = collider1.position - transform.position;
            Vector3 relPosCol2 = collider2.position - transform.position;

            relPosCol1 = relPosCol1 * rel;
            relPosCol2 = relPosCol2 * rel;

            collider1.position = transform.position + relPosCol1;
            collider2.position = transform.position + relPosCol2;
        }
        else if(type == "hall"){
            Transform floor = transform.GetChild(0);
            Transform sideWall1 = transform.GetChild(1);
            Transform sideWall2 = transform.GetChild(2);
            Transform backwall1 = transform.GetChild(3);
            Transform backwall2 = transform.GetChild(4);
            Transform backwall3 = transform.GetChild(5);
            Transform collider = transform.GetChild(6);
            Transform stores = transform.GetChild(8);

            float current_length = floor.GetComponent<Collider>().bounds.size.x;
            float z = floor.GetComponent<Collider>().bounds.size.z;

            // Applying the new length to each part of the hall
            floor.localScale = new Vector3(length, 1f, z);
            sideWall1.localScale = new Vector3(length, 7f, 0.5f);
            sideWall2.localScale = new Vector3(length, 7f, 0.5f);
            stores.localScale = new Vector3(length, 2f, z);

            float rel = length / current_length;

            // Calculating the relative positions of each wall to the centre
            Vector3 relPosCol = collider.position - transform.position;
            Vector3 relPosBack1 = backwall1.position - transform.position;
            Vector3 relPosBack2 = backwall2.position - transform.position;
            Vector3 relPosBack3 = backwall3.position - transform.position;

            // Calculating the new relative positions of each wall
            relPosCol = new Vector3(relPosCol.x * rel, relPosCol.y, relPosCol.z);
            relPosBack1 = new Vector3(relPosBack1.x * rel, relPosBack1.y, relPosBack1.z);
            relPosBack2 = new Vector3(relPosBack2.x * rel, relPosBack2.y, relPosBack2.z);
            relPosBack3 = new Vector3(relPosBack3.x * rel, relPosBack3.y, relPosBack3.z);

            // Applying those new positions
            collider.position = transform.position + relPosCol;
            backwall1.position = transform.position + relPosBack1;
            backwall2.position = transform.position + relPosBack2;
            backwall3.position = transform.position + relPosBack3;
        }
        else if(type == "spawn" || type == "exit"){
            Debug.Log(type);
            Transform floor = transform.GetChild(0);
            Transform wall1 = transform.GetChild(1);
            Transform wall2 = transform.GetChild(2);
            Transform collider1 = transform.GetChild(3);
            Transform spawnarea = transform.GetChild(4);
            Transform stores = transform.GetChild(5);
            Transform backWall = transform.GetChild(7);

            float current_length = floor.GetComponent<Collider>().bounds.size.x;
            float z = floor.GetComponent<Collider>().bounds.size.z;

            floor.localScale = new Vector3(length, 1f, z);
            wall1.localScale = new Vector3(length, 7f, 0.5f);
            wall2.localScale = new Vector3(length, 7f, 0.5f);
            stores.localScale = new Vector3(length, 2f, z);

            float rel = length / current_length;

            Vector3 relPosCol1 = collider1.position - transform.position;
            Vector3 relPosCol2 = spawnarea.position - transform.position;
            Vector3 relPosCol3 = backWall.position - transform.position;

            relPosCol1 = new Vector3(relPosCol1.x * rel, relPosCol1.y, relPosCol1.z);
            relPosCol2 = new Vector3(relPosCol2.x * rel, relPosCol2.y, relPosCol2.z);
            relPosCol3 = new Vector3(relPosCol3.x * rel, relPosCol3.y, relPosCol3.z);

            collider1.position = transform.position + relPosCol1;
            spawnarea.position = transform.position + relPosCol2;
            backWall.position = transform.position + relPosCol3;
        }
        else if(type == "crossway"){

        }
    }

    public void setWidht(float width){
        if(type == "hall"){

        }
        else if(type == "spawn"){

        }
        else if(type == "crossway"){

        }
    }
}
