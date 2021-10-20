using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class AStar
{
    private Transform target;
    private Location startLoc;
    private Transform floor;

    public AStar(Transform floor, Transform target)
    {
        this.target = target;
        this.floor = floor;

        startLoc = new Location();
        startLoc.CurrentPos = floor.parent;
        startLoc.Cost = 1.0f;
        startLoc.SetDistance(target);
    }

    public List<Transform> getCheckpoints()
    {
        BuildingBlock block = floor.parent.GetComponent<BuildingBlock>();
        if (block.getExhibitors().Contains(target))
        {
            List<Transform> checkpoints = new List<Transform>();
            checkpoints.Add(target);
            return checkpoints;
        }
        Location current_loc = getPath();
        if(current_loc == null)
        {

            throw new Exception("Path to " + target + " from " + floor.parent + " not found");
        }
        else
        {
            List<Transform> checkpoints = new List<Transform>();
            while (current_loc.Parent != null)
            {
                checkpoints.Add(getCheckpoint(current_loc.CurrentPos));
                current_loc = current_loc.Parent;
            }
            return checkpoints;
        }
    }

    private Transform getCheckpoint(Transform building)
    {
        foreach (Transform child in building)
        {
            if(child.tag == "checkpoint")
            {
                return child;
            }
        }
        throw new Exception("No checkpoint found");
    }

    private Location getPath()
    {
        List<Location> visited = new List<Location>();
        List<Location> openList = new List<Location>();
        openList.Add(startLoc);
    
        while (openList.Count != 0)
        {
            Location current = openList.OrderBy(x => x.CostDistance).First();

            visited.Add(current);
            openList.Remove(current);

            if (current.CurrentPos == target)
            {
                return current;
            }

            List<Transform> walkableLocations = current.CurrentPos.GetComponent<BuildingBlock>().get_Neighbors();

            for (int i = 0; i < walkableLocations.Count; i++)
            {
                
                if (visited.Any(x => x.CurrentPos == walkableLocations[i]))
                {
                    continue;
                }
                float cost = current.Cost + Vector3.Distance(walkableLocations[i].position, current.CurrentPos.position);

                if (openList.Any(x => x.CurrentPos == walkableLocations[i]) && cost >= openList.Find(x => x.CurrentPos == walkableLocations[i]).Cost)
                {
                    continue;
                }

                if (openList.Any(x => x.CurrentPos == walkableLocations[i]) && cost < openList.Find(x => x.CurrentPos == walkableLocations[i]).Cost)
                {
                    openList.Find(x => x.CurrentPos == walkableLocations[i]).Cost = cost;
                    openList.Find(x => x.CurrentPos == walkableLocations[i]).Parent = current;
                }
                else
                {
                    Location new_loc = new Location();
                    new_loc.CurrentPos = walkableLocations[i];
                    new_loc.Cost = cost;
                    new_loc.Parent = current;
                    new_loc.SetDistance(target);
                    openList.Add(new_loc);
                }

            }
        }

        return null;
    }

    class Location
    {
        public Transform CurrentPos { get; set; }
        public float Cost { get; set; }
        public float Distance { get; set; }
        public float CostDistance => Cost + Distance;
        public Location Parent { get; set; }

        public void SetDistance(Transform target)
        {
            
            this.Distance = Math.Abs(target.position.x - CurrentPos.position.x) + Math.Abs(target.position.y - CurrentPos.position.y);
            
        }
    }
}
