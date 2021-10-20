using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnScript : MonoBehaviour
{

    private IEnumerator coroutine;
    public GameObject visitor;

    public GameObject UI;

    private UIScript uIScript;
    // Start is called before the first frame update
    void Start()
    {
        Transform parent = transform.parent;
        uIScript = parent.GetComponent<BuildingBlock>().getUI().transform.GetComponent<UIScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void start_spawn(int num_visitors, float spawn_interval, float share_vaccinated, float prob)
    {
        coroutine = Spawn(num_visitors, spawn_interval, share_vaccinated, prob);
        StartCoroutine(coroutine);
    }

    public void stop_spawn()
    {
        StopCoroutine(coroutine);
    }

    IEnumerator Spawn(int num_visitors, float spawn_interval, float share_vaccinated, float prob)
    {
        for(int i = 0; i < num_visitors; i++)
        {
            Debug.Log(uIScript.getMask());
            visitor.transform.GetComponent<VisitorAgents_2>().spawn_area = transform.gameObject;
            visitor.transform.GetComponent<VisitorAgents_2>().share_vaccinated = share_vaccinated;
            visitor.transform.GetComponent<VisitorAgents_2>().probability_infected = prob;
            visitor.transform.GetComponent<VisitorAgents_2>().mask = uIScript.getMask();
            Vector3 position = ChooseRandomPosition();
            GameObject vis = Instantiate(visitor, position, Quaternion.identity);
            vis.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            yield return new WaitForSeconds(spawn_interval);
        }
    }

    private Vector3 ChooseRandomPosition()
    {
        var size = transform.localScale - new Vector3(1, 0, 1);
        var center = transform.position;

        return center + new Vector3((Random.value - 0.5f) * size.x, 0.5f, (Random.value - 0.5f) * size.z);
    }
}
