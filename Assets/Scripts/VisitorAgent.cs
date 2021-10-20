using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System;
using Random = UnityEngine.Random;

public class VisitorAgent : Agent
{

    private Rigidbody _visitor;
    public GameObject contact_collider;
    public float probability;
    private Boolean _infected;
    private GameObject[] _people;
    private BoothList booths;
    private List<List<int>> stands;
    [SerializeField] private GameObject spawn_area;
    private ActionBuffers buffer;
    private int _timeLeft;

    private float distance_reward, wall_reward, rule_reward;

    public override void Initialize()
    {
        base.Initialize();
        stands = new List<List<int>>();
        _visitor = gameObject.GetComponent<Rigidbody>();
        _infected = getInfected();
        _timeLeft = Convert.ToInt32(Random.Range(3000, 5000));
    }
    // Start is called before the first frame update
    void Start()
    {
        booths = new BoothList();
    }

    private Vector3 ChooseRandomPosition()
    {
        var size = spawn_area.transform.localScale - new Vector3(1, 0, 1);
        var center = spawn_area.transform.position;

        return center + new Vector3((Random.value - 0.5f) * size.x, 0.5f, (Random.value - 0.5f) * size.z);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Vector3 relativePosition = transform.position - spawn_area.transform.position;
        Vector3 orientation = transform.forward;

        sensor.AddObservation(relativePosition);
        sensor.AddObservation(orientation);
        if(_timeLeft <= 0)
        {
            sensor.AddObservation(0);
        }
        else
        {
            sensor.AddObservation(1);
        }
    }

    private void switchColor()
    {
        if (_infected)
        {
            transform.GetChild(0).GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            transform.GetChild(0).GetComponent<Renderer>().material.color = Color.green;
        }
    }

    public override void OnEpisodeBegin()
    {
        _timeLeft = Convert.ToInt32(Random.Range(3000, 5000));
        _infected = getInfected();

        Debug.Log("Distance Reward: " + distance_reward + " Wall Rewad: " + wall_reward + " Rule Reward: " + rule_reward);

        distance_reward = 0f;
        wall_reward = 0f;
        rule_reward = 0f;

        switchColor();
        transform.position = ChooseRandomPosition();
    }

    private void InterpretActions(ActionBuffers actionBuffers)
    {
        transform.position = transform.position + new Vector3(actionBuffers.ContinuousActions[0] * 0.05f, 0, actionBuffers.ContinuousActions[1] * 0.05f);
    }

    private Boolean getInfected()
    {
        int prob = Random.Range(0, 100);
        if(prob < (probability * 100))
        {
            return true;
        }
        return false;
    }

    public void infect()
    {
        _infected = true;
    }

    public void add_Reward(float reward)
    {
        AddReward(reward);
    }

    private void DistanceReward(GameObject obj)
    {
        float distance = Vector3.Distance(obj.transform.position, transform.position);
        float reward = booths.Update(obj, distance);
        switch (obj.tag){
            case "high":
                AddReward(reward * 3f);
                distance_reward += reward * 3f;
                break;
            case "medium":
                AddReward(reward * 2f);
                distance_reward += reward * 2f;
                break;
            case "low":
                distance_reward += reward;
                AddReward(reward);
                break;
        }
    }

    private Boolean determineInfection(GameObject obj)
    {
        /// TO-DO: Probability berechnen
        return true;
    }

    private void calculateInfections(GameObject obj)
    {
        if(obj.tag == "visitor" && determineInfection(obj) && _infected && obj != null)
        {

            VisitorAgent agent = obj.GetComponent<VisitorAgent>();
            agent.infect();
        }
    }

    void Trigger(GameObject obj)
    {
        int id = obj.GetInstanceID();


        if (obj.tag == "visitor" && Vector3.Distance(obj.transform.position, transform.position) < 3)
        {
            calculateInfections(obj);
            AddReward(-0.0003f);
            rule_reward += -0.0003f;
        }
        if (obj.tag == "wall")
        {
            if (Vector3.Distance(transform.position, obj.transform.position) < 2f)
            {
                AddReward(-0.0005f);
                wall_reward += -0.0005f;
            }
        }
        DistanceReward(obj);
    }

    void OnTiggerEnter(Collision other)
    {
        Trigger(other.gameObject);
    }

    void OnTriggerStay(Collider other)
    {
        if (new List<string>(){"high", "medium", "low", "wall", "visitor"}.Contains(other.gameObject.tag))
        {
            Trigger(other.gameObject);
            return;
        }
        if (other.gameObject)
        {
            GameObject agent = other.gameObject;
            Trigger(agent.transform.parent.gameObject);
            return;
        }
    }

    // Update is called once per frame
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        switchColor();
        _timeLeft = _timeLeft - 1;
        buffer = actionBuffers;
        InterpretActions(actionBuffers);
        ///AddReward(-1f * new Vector3(buffer.ContinuousActions[1] * 0.05f, 0, buffer.ContinuousActions[0] * 0.2f).magnitude * 0.1f);
        if(_timeLeft <= 0)
        {
            float distance = Vector3.Distance(transform.position, spawn_area.transform.position);
            ///AddReward(Math.Abs(distance * 5) * -1.0f);
        }
    }
}

class BoothList
{
    public List<Booth> booths;

    public BoothList(){
        booths = new List<Booth>();
    }

    public float Update(GameObject obj, float distance){
        for(int i = 0; i < booths.Count; i++){
            if(booths[i].Current == obj){
                float old = booths[i].Distance;
                booths[i].Distance = distance;
                return old - distance;
            }
        }

        Booth newBooth =  new Booth(obj, distance);
        booths.Add(newBooth);
        return 0f;
    }   
}

class Booth
{

    public Booth(GameObject Current, float Distance){
        this.Current = Current;
        this.Distance = Distance;
    }
    public GameObject Current {get; set; }
    public float Distance {get; set; }
}
