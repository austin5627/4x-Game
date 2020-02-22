using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HexMap_Continents : HexMap {

    public override void GenerateMap()
    {
        base.GenerateMap();
        

        //gen land
        int numContinents = 3;
        int continentSpacing = Columns/numContinents;

        /*
        if (false) {
            Random.InitState(0);
        }*/
        for (int c = 0; c < numContinents; c++)
        {
            int numSplats = Random.Range(4, 8);
            for (int i = 0; i < numSplats; i++)
            {
                int radius = Random.Range(5, 8);

                int y = Random.Range(radius, Rows - radius);
                int x = Random.Range(0, 10) - y / 2 + (c*continentSpacing);

                ElevateArea(x, y, radius);
            }
        }

        //and randomness w/ perlin noise
        float noiseResolution = .05f;
        Vector2 noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        float noiseScale = 2f;

        for (int col = 0; col < Columns; col++)
        {
            for (int row = 0; row < Rows; row++)
            {
                Hex h = GetHexAt(col, row);
                float n = Mathf.PerlinNoise( ((float)col / Mathf.Max(Columns, Rows) / noiseResolution) + noiseOffset.x,
                    ((float)row / Mathf.Max(Columns, Rows) / noiseResolution) + noiseOffset.y)
                    - .5f;
                h.Elevation += n * noiseScale;
            }
        }

        //moisture
        noiseResolution = .01f;
        noiseOffset = new Vector2(Random.Range(0f, 1f), Random.Range(0f, 1f));
        noiseScale = 2f;

        for (int col = 0; col < Columns; col++)
        {
            for (int row = 0; row < Rows; row++)
            {
                Hex h = GetHexAt(col, row);
                float n = Mathf.PerlinNoise(((float)col / Mathf.Max(Columns, Rows) / noiseResolution) + noiseOffset.x,
                    ((float)row / Mathf.Max(Columns, Rows) / noiseResolution) + noiseOffset.y)
                    - .5f;
                h.Moisture += n * noiseScale;
            }
        }


        UpdateHexVisuals();


        int resourcePercent = 15;
        foreach (Hex h in Hexes)
        {
            int ran = Random.Range(0,100/resourcePercent);
            if (ran == 1 && h.MovementCost > 0)
            {
                Resource type = ResourceTypes.Values.ToArray<Resource>()[Random.Range(0, ResourceTypes.Values.Count)];
                h.Resource = type;
            }
        }
            





        Hex h1 = null;
        Hex h2 = null;
        int numLand = 0;
        do
        {
            int x1 = Mathf.RoundToInt(Random.Range(0+5, Columns-5));
            int y1 = Mathf.RoundToInt(Random.Range(0+5, Rows-5));
            h1 = null;
            h2 = null;
            numLand = 0;
            Hex[] hexes = GetHexesInRadius(GetHexAt(x1, y1), 2);
            foreach (Hex h in hexes)
            {
                if (h.MovementCost > 0)
                {
                    numLand++;
                    if (h1 == null)
                        h1 = h;
                    if (h1 != null)
                        h2 = h;
                }
            }
        } while (numLand<3);

        SpawnUnitAt("Warrior", h1.Q, h1.R);
        SpawnUnitAt("Settler", h2.Q, h2.R);

        Vector3 pos = HexToGo[h1].transform.position;
        pos.z -= 10;
        pos.y = 10;
        Camera.main.transform.position = pos;

    }
    
    public void ElevateArea(int q, int r, int radius, float centerHeight = .7f)
    {
        Hex center = GetHexAt(q,r);
        
        Hex[] areaHexes = GetHexesInRadius(center,radius);

        foreach (Hex h in areaHexes)
        {
            //if(h.Elevation < 0)
              //  h.Elevation = 0;
            h.Elevation = centerHeight * Mathf.Lerp(1,.25f, Mathf.Pow(Hex.Distance(center, h)/radius,2));
        }
    }

}
