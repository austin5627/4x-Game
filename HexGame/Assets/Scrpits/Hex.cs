using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hex{

    public readonly int Q;
    public readonly int R;
    public readonly int S;

    static readonly float WIDTH_MULTIPLIER = Mathf.Sqrt(3) / 2;
    private readonly float radius = 1f;

    public float Elevation;
    public float Moisture;

    public Unit unit = null;
    public City city;

    public HexMap map;

    public int Production;
    public int Food;
    public int Gold;
    public int Science;
    public int Culture;

    public Resource Resource;

    public int MovementCost { get; set; }

    public enum HexType {Flat, Hill, Mountain, Ocean}
    public enum HexBiome {Desert, Plains, Grassland, Forest, Jungle, Mountain}
    public HexType Type;
    public HexBiome Biome;

    public bool first = true;
    public bool FogOfWar = true;

    public Hex(int q, int r, HexMap map)
    {
        Q = q;
        R = r;
        S = -(Q + R);
        this.map = map;
        MovementCost = 1;

    }
    
    public Vector3 Position()
    {
        return new Vector3(HexHorizontalSpacing() * (this.Q + this.R/2f), 0, this.R * HexVerticalSpacing());
    }

    public float HexHeight()
    {
        return radius * 2;
    }

    public float HexWidth()
    {
        return WIDTH_MULTIPLIER * HexHeight();
    }

    public float HexVerticalSpacing()
    {
        return HexHeight() * .75f;
    }

    public float HexHorizontalSpacing()
    { 
        return HexWidth();
    }

    public Vector3 PositionFromCamera(Vector3 cameraPosition, float numRows, float numColumns, bool horizWrapping, bool vertWrapping)
    {
        float mapHeight = numRows * HexVerticalSpacing();
        float mapWidth = numColumns * HexHorizontalSpacing();

        Vector3 position = Position();

        if (horizWrapping)
        {
            float widthsFromCamera = (position.x - cameraPosition.x) / mapWidth;


            if (widthsFromCamera > 0)
                widthsFromCamera += 0.5f;
            if (widthsFromCamera < 0)
                widthsFromCamera -= 0.5f;

            int widthsToFix = (int)widthsFromCamera;

            position.x -= widthsToFix * mapWidth;
        }
        
        if (vertWrapping)
        {
            float heightsFromCamera = (position.z - cameraPosition.z) / mapHeight;


            if (heightsFromCamera > 0)
                heightsFromCamera += 0.5f;
            if (heightsFromCamera < 0)
                heightsFromCamera -= 0.5f;

            int heightsToFix = (int)heightsFromCamera;

            position.z -= heightsToFix * mapHeight;
        }

        return position;
    }
    

    public static float Distance(Hex a, Hex b)
    {
        return Mathf.Max(
            Mathf.Abs(a.Q - b.Q),
            Mathf.Abs(a.R - b.R),
            Mathf.Abs(a.S - b.S)
            );
    }

    public float AggregateCostToEnter(float costSoFar, Hex sourceHex, Unit unit)
    {
        return Mathf.Infinity;
    }

    public Hex[] GetNieghbors(HexMap map)
    {
        Hex[] neighbors = new Hex[6];
        neighbors[0] = map.GetHexAt(Q - 1,R + 1);
        neighbors[1] = map.GetHexAt(Q,R + 1);
        neighbors[2] = map.GetHexAt(Q + 1,R);
        neighbors[3] = map.GetHexAt(Q + 1,R - 1);
        neighbors[4] = map.GetHexAt(Q,R - 1);
        neighbors[5] = map.GetHexAt(Q - 1,R);

        return neighbors;
    }

    public override string ToString()
    {
        return "(" + this.Q + ", " + this.R + ")";
    }


}
