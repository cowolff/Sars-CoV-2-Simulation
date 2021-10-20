using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class StoreScript : MonoBehaviour
{

    public int start, end, interest;
    public string type;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getTimeframe()
    {
        return Convert.ToInt32(Random.Range(start, end));
    }

    public int getInterestLevel()
    {
        return interest;
    }

    public string getType()
    {
        return type;
    }
}
