//using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    int[,] testMap;
    int mapLevel = 1;
    int maxLevel = 7;
    int[] RandamMakeMap(int depth, int maxDepth, int[] oldLand, int oldLength)
    {
        int[] newLand;

        if (depth == maxDepth)
        {
            newLand = new int[oldLength];
            for (int i = 0; i < oldLength; i++)
            {
                newLand[i] = oldLand[i];
            }
            return newLand;
        }
        newLand = new int[oldLength * 4];
        int nowIndex = 0;
        for (int i = 0; i < oldLength; i++)
        {
            int oldX = oldLand[i] % width;
            int baseX = oldX;
            int oldY = oldLand[i] / width;
            int baseY = oldY;
            double notPossible = 0.55 * (1 - (maxDepth - depth) / maxDepth);
            double perPossible = notPossible;
            for (int j = oldX - 1; j <= oldX + 1; j += 2)
            {
                if (j < 0) { j += width; oldX += width; }
                if (j >= width) { j -= width; oldX -= width; }
                if (testMap[j, oldY] == 0 && checkMake(j, baseY, depth, maxDepth, 1.0 - perPossible))
                {
                    newLand[nowIndex++] = (j + baseY * width);
                }
                else
                {
                    perPossible = perPossible * notPossible;
                }
            }
            for (int j = oldY - 1; j <= oldY + 1; j += 2)
            {
                if (j < 0) { j += height; oldY += height; }
                if (j >= height) { j -= height; oldY -= height; }
                if (testMap[baseX, j] == 0 && checkMake(baseX, j, depth, maxDepth, 1.0 - perPossible))
                {
                    newLand[nowIndex++] = (baseX + j * width);
                }
                else
                {
                    perPossible = perPossible * notPossible;
                }
            }

        }
        if (nowIndex == 0)
        {
            newLand = new int[oldLength];
            for (int i = 0; i < oldLength; i++)
            {
                newLand[i] = oldLand[i];
            }
            return newLand;
        }
        return RandamMakeMap(depth + 1, maxDepth, newLand, nowIndex);
    }
    private bool checkMake(int _x, int _y, int depth, int maxDepth, double probability)
    {
        double getRandom = ((double) Random.Range(depth / 3, maxDepth)) / ((double) maxDepth);
        if (getRandom < probability)
        {
            testMap[_x, _y] = mapLevel;
            return true;
        }
        //testMap[_x, _y] = maxLevel;
        return false;
    }
    //int rndPlus = 0;
    void BaseLoad()
    {
        //rndPlus = 100;
        testMap = new int[width, height];

        int[] startXY = new int[0];
        while (mapLevel < maxLevel)
        {
            int _xy = startXY.Length != 0 ? startXY[Random.Range(0,startXY.Length)] : Random.Range(0,width) + width * Random.Range(0,height);

            if (testMap[_xy % width, _xy / height] == 0) testMap[_xy % width, _xy / height] = mapLevel;
            startXY = new int[] { _xy };

            startXY = RandamMakeMap(0, 20, startXY, 1);
            mapLevel++;
        }
    }
    // Start is called before the first frame update
    public int width , height ;
    int areaRatio;
    public Vector3 planeSize;
    private Terrain thePlane;
    int testMapDis(int _x,int _y,int t)
    {
        switch (t)
        {
            case 0:
                 return Mathf.Abs(testMapDis(_x, _y, 6) - testMapDis(_x , _y, 3));
            case 1:
                return Mathf.Abs(testMapDis(_x, _y, 6) - testMapDis(_x , _y , 4));
            case 2:
                return Mathf.Abs(testMapDis(_x, _y, 6) - testMapDis(_x , _y, 5));
            case 3:
                return testMapDis(_x - 1, _y, 6);
            case 4:
                return testMapDis(_x , _y - 1, 6);
            case 5:
                return testMapDis(_x + 1, _y, 6);
            default:
                return testMap[_x / areaRatio, _y / areaRatio];

        }
    }
    float makeTerrain2(int _x, int _y, int _w, int _h, float[,] tH)
    {
        
        if (tH[_x,_y] != 0.0f){return 0;}else{tH[_x, _y] = 10.0f;}
        /*float upT = makeTerrain2(_x, upY, _w, _h, tH);
        float downT = makeTerrain2(_x, downY, _w, _h, tH);
        float leftT = makeTerrain2(leftX, _y, _w, _h, tH);
        float rightT = makeTerrain2(rightX, _y, _w, _h, tH);*/
        int upY = (_y.Equals(0) ? _h - 1 : _y - 1);
        int downY = (_y == _h - 1 ? 0 : _y + 1);
        int leftX = (_x == 0 ? _w - 1 : _x - 1);
        int rightX = (_x == _w-1 ? 0 : _x + 1);
        int upMapDis = testMap[_x , upY];
        int downTest = testMap[_x , downY];
        int leftTest = testMap[leftX, _y];
        int rightTest = testMap[rightX, _y];
        float centerHeight = 0;
        for (int i = 0; i < areaRatio; i++)
        {
            for (int j = 0; j < areaRatio; j++)
            {
                if (i < areaRatio / 2)
                {
                    
                }
                else
                {

                }
            }
        }
        return 0;
    }
    float OneAreaTerrain(int _x, int _y,int tx,int ty, float centerHeight, float centerHeight1, float[,] tH)
    {

        int helfRatio = areaRatio / 2;
        int baseX = _x * areaRatio;
        int baseY = _y * areaRatio;

        return 0.0f;
    }
    void makeTerrain(int _x,int _y,int _w,int _h, float[,] tH)
    {
        if (_y == 0)
        {
            tH[_x, _y] = (_x == 0 ? 0 : tH[_x - 1, _y] + terrainChangeValue(testMapDis(_x, _y, 0)));
        }
        else if (_x == _w - 1 || testMapDis(_x, _y, 6) == testMapDis(_x, _y, 4))
        {
            tH[_x, _y] = tH[_x , _y - 1] + terrainChangeValue(testMapDis(_x, _y, 1));
        }
        else
        {
            tH[_x, _y] = tH[_x + 1, _y] + ((testMapDis(_x, _y, 6) == testMapDis(_x, _y, 5)) ? terrainChangeValue( testMapDis(_x, _y, 2)) : terrainChangeValue( testMapDis(_x, _y, 1) + testMapDis(_x, _y, 2)));
        }
        if (_x < _w - 1 && _y == 0)
        {
            makeTerrain(_x + 1, _y, _w, _h, tH);
        }
        if (_y < _h - 1)
        {
            makeTerrain(_x , _y + 1, _w, _h, tH);
        }
    }
    float terrainChangeValue(float dis)
    {
        return Random.Range(-0.0002f, 0.0002f) * Mathf.Pow(3, dis);
    }
    void terrainNomalize(int _wh, float[,] tH)
    {
        float min = float.MaxValue, max = float.MinValue;
        for (int x = 0; x < _wh - 1; x++)
        {
            for (int y = 0;y < _wh - 1; y++)
            {
                if (min > tH[x, y]) min = tH[x, y];
                if (max < tH[x, y]) max = tH[x, y];
            }
        }
        for (int xy = 0;xy < _wh; xy++)
        {
            tH[xy, _wh - 1] = tH[xy, 0] + tH[xy, _wh - 2];
            tH[ _wh - 1,xy] = tH[0,xy] + tH[_wh - 2,xy];
        }
        float range = max - min;
        for (int x = 0; x < _wh; x++)
        {
            for (int y = 0; y < _wh; y++)
            {
                tH[x, y] = (tH[x, y] - min) / range;
            }
        }
    }
    void Start()
    {
        //Instantiate(thePlane, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        //TerrainData terrainData = thePlane.terrainData;
        thePlane = GetComponent<Terrain>();
        TerrainData terrainData = thePlane.terrainData;
        terrainData.size = planeSize;
        int planeWidth = terrainData.heightmapResolution;//it is square
        areaRatio = planeWidth / width;
        float[,] terrainHeights = terrainData.GetHeights(0,0,planeWidth, planeWidth);
        BaseLoad();
        makeTerrain(0,0, planeWidth-1, planeWidth-1, terrainHeights);
        terrainNomalize(planeWidth, terrainHeights);
        terrainData.SetHeights(0, 0, terrainHeights);
        Vector3 terrainSize = terrainData.size;
        terrainData.size = new Vector3(2000,1000,2000);
        thePlane.terrainData = terrainData;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}

