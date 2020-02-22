using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System.Linq;

public class Pathfinding
{
    HexMap world;
    Hex start;
    Hex end;


   public Pathfinding(HexMap world, Hex start, Hex end)
    {
        this.world = world;
        this.start = start;
        this.end = end;
   }


    public static Queue<Hex> FindPath(HexMap world, Hex start, Hex end)
    {

        Pathfinding pathfinder = new Pathfinding(world, start, end);
        return pathfinder.AStar();
    }


        
    public Queue<Hex> AStar()
    {
        List<Hex> hexes = new List<Hex>();
        foreach (Hex h in world.Hexes)
        {
            hexes.Add(h);
        }


        HashSet<Hex> closedSet = new HashSet<Hex>();

        SimplePriorityQueue<Hex> openSet = new SimplePriorityQueue<Hex>();
        openSet.Enqueue(start, 0);

        Dictionary<Hex, Hex> cameFrom = new Dictionary<Hex, Hex>();

        Dictionary<Hex, float> gScore = new Dictionary<Hex, float>();
        foreach (Hex n in hexes)
        {
            gScore[n] = Mathf.Infinity;
        }
        gScore[start] = 0;

        Dictionary<Hex, float> fScore = new Dictionary<Hex, float>();
        foreach (Hex n in hexes)
        {
            fScore[n] = Mathf.Infinity;
        }
        fScore[start] = CostEstimate(start, end);

        while (openSet.Count > 0)
        {
            Hex current = openSet.Dequeue();// get Lowest fScore
            if (current == end)
                return Reconstruct_path(cameFrom, current);
            closedSet.Add(current);
            foreach (Hex neighbor in current.GetNieghbors(world))
            {
                if (neighbor == null || closedSet.Contains(neighbor) || neighbor.MovementCost < 0 || neighbor.unit != null)
                    continue;
                
                float tentGScore = gScore[current] + CostEstimate(current, neighbor);
                if (openSet.Contains(neighbor) && tentGScore >= gScore[neighbor])
                    continue;

                cameFrom[neighbor] = current;
                gScore[neighbor] = tentGScore;
                fScore[neighbor] = gScore[neighbor] + CostEstimate(neighbor, end);
                if (!openSet.Contains(neighbor))
                {
                    openSet.Enqueue(neighbor, fScore[neighbor]);
                }

            }

        }

        return null;
    }

    public float CostEstimate(Hex start, Hex end)
    {
        return Hex.Distance(start, end) * end.MovementCost;
    }

    public Queue<Hex> Reconstruct_path(Dictionary<Hex, Hex> cameFrom, Hex current)
    {
        Queue<Hex> total_path = new Queue<Hex>();
        total_path.Enqueue(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            total_path.Enqueue(current);
        }
        return new Queue<Hex>(total_path.Reverse());
    }
    
}
