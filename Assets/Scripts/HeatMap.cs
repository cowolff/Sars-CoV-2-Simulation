using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatMap : MonoBehaviour
{
    List<Vector3> positions;
    Bounds bounds;
    Vector3 extends;
    int width;
    int depth;
    int[,] map;

    // Start is called before the first frame update
    void Start()
    {
        positions = new List<Vector3>();
        bounds = transform.GetComponent<Renderer>().bounds;
        width = (int)bounds.size.x;
        depth = (int)bounds.size.z;
        map = new int[width, depth];
        extends = bounds.extents;
    }

    public void add_Position(Vector3 position)
    {
        positions.Add(position + extends);
    }

    public void generate_Heatmap()
    {
        int maxValue = 0;
        foreach(Vector3 pos in positions)
        {
            map[(int)pos.x, (int)pos.z] += 1;

            if(maxValue < map[(int)pos.x, (int)pos.z])
            {
                maxValue = map[(int)pos.x, (int)pos.z];
            }
        }
        int normalize = maxValue / 255;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                map[i, j] = map[i, j] * normalize;
                if (map[i, j] < 0 || map[i,j] > 255)
                {
                    Debug.Log("BIG OOOOF");
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
