using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Random = UnityEngine.Random;

public class VisitorAgents_2 : Agent
{
    // Start is called before the first frame update

    private string status;
    private Transform target;
    private Vector3 current_Checkpoint;
    public float probability_infected;
    public float share_vaccinated;
    private List<Transform> exhibitors;
    private List<Transform> halls;
    private List<Transform> path;
    public GameObject spawn_area;
    private GameObject exit;
    private float last_distance;
    private int timestamp;
    private int time_left;
    private int leave_time;
    private Transform floor;
    public string mask;
    private Visitor_Collider collission_detector;
    private AStar aStar;
    private Transform target_hall;
    private float velocity;
    private float[] last_action;
    public float action_reward;
    public float distance_reward;
    public float target_reward;
    private bool startTimer;


    void Start()
    {
        action_reward = 0f;
        distance_reward = 0f;
        target_reward = 0f;
        velocity = 0f;
        last_action = new float[2] {0,0};
        startTimer = false;

        // Gets the collider used to detect agents violating social distancing rules
        collission_detector = transform.GetChild(1).GetComponent<Visitor_Collider>();
        floor = spawn_area.transform.parent.GetComponent<BuildingBlock>().get_floor();

        exit = GetExit();
        timestamp = GetTimestamp();
        SwitchColor();
    }

    // Update is called once per frame
    void Update()
    {
        if(!startTimer){
            timestamp = GetTimestamp();
        }
        SwitchTargets();
        SwitchColor();
        if(target_hall == null)
        {
            OnEpisodeBegin();
        }
        if (collission_detector.getFloor() != null)
        {
            floor = collission_detector.getFloor();
        }
        if(path[0] == null){
            OnEpisodeBegin();
        }
    }

    private Vector3 ChooseRandomPosition()
    {
        var size = spawn_area.transform.localScale - new Vector3(1, 0, 1);
        var center = spawn_area.transform.position;

        return center + new Vector3((Random.value - 0.5f) * size.x, 0.5f, (Random.value - 0.5f) * size.z);
    }

    private string getInfected()
    {
        int prob = Random.Range(0, 100);
        if (prob > (share_vaccinated * 100))
        {
            prob = Random.Range(0, 100);
            if(prob < (probability_infected * 100))
            {
                return "infected";
            } else
            {
                return "healthy";
            }
        }
        else
        {
            return "vaccinated";
        }
    }

    private void InterpretActions(ActionBuffers actionBuffers)
    {
        transform.position = transform.position + new Vector3(actionBuffers.ContinuousActions[0] * 0.05f, 0, actionBuffers.ContinuousActions[1] * 0.05f);
    }

    public void exposed()
    {
        if (status == "healthy")
        {
            
            status = "exposed";
        }
    }

    public override void OnEpisodeBegin()
    {
        collission_detector = transform.GetChild(1).GetComponent<Visitor_Collider>();
        //LogValues();        
        ResetValues();
        ResetExhibitors();
        ChooseSimulationObjects();

        transform.position = ChooseRandomPosition();

        timestamp = GetTimestamp();
        leave_time = timestamp + Convert.ToInt32(Random.Range(780, 1200));

        status = getInfected();

        SwitchColor();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 relativeTarget = (transform.position - current_Checkpoint).normalized;
        sensor.AddObservation(relativeTarget);
    }

    private List<Transform> GetChildObject(Transform parent, List<string> tags)
    {
        List<Transform> children = new List<Transform>();
        foreach (Transform child in parent)
        {
            if (tags.Contains(child.tag)) {
                switch (child.tag)
                {
                    case "high":
                        for(int i = 0; i <= 3; i++)
                        {
                            children.Add(child);
                        }
                        break;

                    case "medium":
                        for(int i = 0; i <= 2; i++)
                        {
                            children.Add(child);
                        }
                        break;

                    case "low":
                        children.Add(child);
                        break;

                    case "floor":
                        children.Add(child);
                        break;
                }
            }
        }
        return children;
    }

    public override void Heuristic(in ActionBuffers actionsOut){
        if(Input.GetKey("up")){
            transform.position = transform.position + new Vector3(0f, 0f, 0.1f);
        }
        if(Input.GetKey("down")){
            transform.position = transform.position + new Vector3(0f, 0f, -0.1f);
        }
        if(Input.GetKey("left")){
            transform.position = transform.position + new Vector3(-0.1f, 0f, 0f);
        }
        if(Input.GetKey("right")){
            transform.position = transform.position + new Vector3(0.1f, 0f, 0f);
        }
    }

    private void CalculateDistanceReward()
    {
        float current_Distance = Vector3.Distance(transform.position, current_Checkpoint);
        float new_reward = last_distance - current_Distance;
        AddReward(new_reward);
        distance_reward = distance_reward + new_reward;
        last_distance = current_Distance;
    }

    private void SwitchColor()
    {
        switch (status)
        {
            case "infected":
                transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
                break;
            case "healthy":
                transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
                break;
            case "vaccinated":
                transform.GetChild(0).GetComponent<Renderer>().material.color = Color.blue;
                break;
            case "exposed":
                transform.GetChild(0).GetComponent<Renderer>().material.color = Color.yellow;
                break;
        }
    }

    private void SwitchTargets()
    {
        
        if (timestamp + time_left <= GetTimestamp() && leave_time >= GetTimestamp())
        {
            timestamp = GetTimestamp();
            int index = Random.Range(0, exhibitors.Count);
            target = exhibitors[index];
            time_left = target.GetComponent<StoreScript>().getTimeframe();
            foreach (Transform hall in halls)
            {
                if (hall.GetComponent<BuildingBlock>().getExhibitors().Contains(target))
                {
                    target_hall = hall;
                }
            }
            aStar = new AStar(floor, target_hall);
            path = GetPath(aStar);
            current_Checkpoint = path[0].position;
            last_distance = Vector3.Distance(transform.position, current_Checkpoint);
            startTimer = false;
        }
        else if(leave_time <= GetTimestamp())
        {
            target = exit.transform;
            target_hall = exit.transform.parent.transform;
            aStar = new AStar(floor, target_hall);
            path = GetPath(aStar);
            current_Checkpoint = path[0].position;
        }
    }

    private List<Transform> GetPath(AStar astar)
    {
        List<Transform> new_path = aStar.getCheckpoints();
        new_path.Reverse();
        new_path.Add(target);
        return new_path;
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        ActionReward(actionBuffers);
        InterpretActions(actionBuffers);
        CalculateDistanceReward();
    }

    private void ActionReward(ActionBuffers actionBuffers)
    {
        try { 
            float reward = -0.05f * (Math.Abs(actionBuffers.ContinuousActions[0] - last_action[0]) + Math.Abs(actionBuffers.ContinuousActions[1] - last_action[1]));
            AddReward(reward);
            action_reward = action_reward + reward;
            last_action[0] = actionBuffers.ContinuousActions[0];
            last_action[1] = actionBuffers.ContinuousActions[1];
        }
        catch(Exception e)
        {
            Debug.Log(e);
            last_action[0] = actionBuffers.ContinuousActions[0];
            last_action[1] = actionBuffers.ContinuousActions[1];
        }
    }

    public bool checkCheckpoint(int id)
    {
        if(path[0].GetInstanceID() == id)
        {
            path.RemoveAt(0);
            current_Checkpoint = path[0].position;
            last_distance = Vector3.Distance(transform.position, current_Checkpoint);
            return true;
        }
        return false;
    }

    public void addTargetReward(float reward){
        AddReward(reward);
        target_reward = target_reward + reward;
    }

    private void ResetValues(){
        action_reward = 0f;
        distance_reward = 0f;
        target_reward = 0f;
        startTimer = false;
    }

    private void ResetExhibitors(){
        exhibitors = new List<Transform>();
        halls = GetFromScene(new List<string>() { "BuildingBlock" });
        foreach (Transform hall in halls)
        {
            exhibitors.AddRange(hall.GetComponent<BuildingBlock>().getExhibitors());
        }
    }

    private void LogValues(string path = "C:\\Users\\wolff\\Desktop\\MLAgents\\Logging.txt", bool safe=true){
        float visitor_reward = collission_detector.get_visitor_reward();
        float wall_reward = collission_detector.get_wall_reward();
        collission_detector.set_to_zero();
        Debug.Log("Agent: " + transform.name + "; Distance Reward: " + distance_reward + "; Action Reward: " + action_reward + "; Wall Reward: " + wall_reward + "; Visitor Reward: " + visitor_reward + "; Target Reward: " + target_reward);
        if(safe){
            using (StreamWriter sw = new StreamWriter(path, true)){
            sw.WriteLine(transform.name + ";" + distance_reward + ";" + action_reward + ";" + wall_reward + ";" + visitor_reward + ";" + target_reward + ";" + GetCumulativeReward() + ";" + GetTimestamp());
            }
        }
    }

    private void ChooseSimulationObjects(){
        int index = Random.Range(0, exhibitors.Count);
        target = exhibitors[index];
        time_left = target.GetComponent<StoreScript>().getTimeframe();
        exit = GetExit();

        foreach (Transform hall in halls)
        {
            if (hall.GetComponent<BuildingBlock>().getExhibitors().Contains(target))
            {
                target_hall = hall;
            }
        }
        floor = spawn_area.transform.parent.GetComponent<BuildingBlock>().get_floor();

        aStar = new AStar(floor, target_hall);
        path = GetPath(aStar);
        current_Checkpoint = path[0].position;
        last_distance = Vector3.Distance(transform.position, current_Checkpoint);
    }

    private int GetTimestamp()
    {
        return (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
    }

    public void add_Reward(float added_reward)
    {
        AddReward(added_reward);
    }

    private List<Transform> GetFromScene(List<string> tags)
    {
        List<Transform> exhibs = new List<Transform>();

        foreach(string tag in tags)
        {
            GameObject[] exhibs_block = GameObject.FindGameObjectsWithTag(tag);
            foreach(GameObject exhib in exhibs_block)
            {
                exhibs.Add(exhib.transform);
            }
        }
        return exhibs;

    }

    private GameObject GetExit()
    {
        GameObject[] exits = GameObject.FindGameObjectsWithTag("exit");
        int index = Convert.ToInt32(Random.Range(0, exits.Length));
        return exits[index];
    }

    public bool isTarget(int id){
        if(path[0].GetInstanceID() == id){
            return true;
        } else {
            return false;
        }
    }

    public void StartTimer(){
        startTimer = true;
    }

    public string getStatus()
    {
        return status;
    }

    public Transform getTarget()
    {
        return target;
    }

    public string GetMask(){
        return mask;
    }
}
