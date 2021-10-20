using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Infection : MonoBehaviour
{

    private static float[,] matrix = new float[,] {{0f, 0.65f}, {0.3f, 0.65f}, 
            {0.5f, 0.5f}, {0.7f, 0.3f}, {0.75f, 0.2f}, {0.8f, 0.1f},
            {1f, 0.5f}, {1.25f, 0.02f}, {1.5f, 0.005f}, {2f, 0f}};
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static bool GetInfectionBA(Vector3 agent, Vector3 contact, string mask="None"){
        float distance = (Vector3.Distance(agent, contact) / 2f);
        for(int i = 0; i < matrix.Length; i++){
            if(matrix[i,0] > distance){

                float relDistance = (distance - matrix[i-1,0]);
                float localLine = ((matrix[i-1,1] - matrix[i,1]) / (matrix[i-1,0] - matrix[i,0]));
                float prob = (relDistance * localLine) + matrix[i,1];

                var maskFactor = MaskFactor(mask);
                var probPerFrame = ((1.0f - Mathf.Pow(prob, Time.deltaTime))/240) * maskFactor;
                return UnityEngine.Random.value < probPerFrame;
            }
        }
        return false;
    }

    public static bool GetInfectionClassic(Vector3 healthy, Vector3 infectious, string mask="None")
    {
        float ExposureDistanceMeters = 1.8288f;
        float ExposureProbabilityAtMaxDistance = 0.0f;
        float ExposureProbabilityAtZeroDistance = 0.5f;
        
        var distance = Vector3.Distance(healthy, infectious) / 2;
        if (distance > ExposureDistanceMeters)
        {
            // Too far away
            return false;
        }

        // Interpolate the probability parameters based on the distance
        var t = distance / ExposureDistanceMeters;
        var prob = ExposureProbabilityAtZeroDistance * (1.0f - t) + ExposureProbabilityAtMaxDistance * t;
        float maskFactor = MaskFactor(mask);
        var probPerFrame = (1.0f - (Mathf.Pow(1.0f - prob, Time.deltaTime))) * maskFactor;
        return UnityEngine.Random.value < probPerFrame;
    }

    private static float MaskFactor(string mask){
        float mask_factor = 1;
        switch (mask){
            case "N95":
                mask_factor = 0.2f;
                Debug.Log("LOL");
                break;
            case "Surgical":
                mask_factor = 0.5f;
                break;
            case "None":
                mask_factor = 1f;
                break;
        }
        return mask_factor;
    }

}
