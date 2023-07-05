//not ok version
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    int[,] testMap;
    int[,] terrianIndex;
    List<int> notOver = new List<int>();
    bool[,] centerHeightPushed;
    int mapLevel = 1;
    int maxLevel = 7;
    Vector2[] dirRndBase = new Vector2[4] { new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0), new Vector2(0, -1) };

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
            AddFourDir(depth, maxDepth, oldLand[i], newLand,ref nowIndex);
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

    void AddFourDir(int depth, int maxDepth, int oldLand, int[] newLand,ref int nowIndex)
    {
        List<int> dirRndIndex = new List<int>();
        int oldX = oldLand % width;
        int baseX = oldX;
        int oldY = oldLand / width;
        int baseY = oldY;
        double notPossible = 0.55 * (1 - (maxDepth - depth) / maxDepth);
        double perPossible = notPossible;
        for (int j = 0; j <= 3; j += 1)
        {
            dirRndIndex.Add(Random.Range(0, 4));
            int listInd = dirRndIndex.Count - 1;
            for (int k = 0; k < listInd; k += 1)
            {
                if (dirRndIndex[k] <= dirRndIndex[listInd]) dirRndIndex[listInd] = ArrayCycle(dirRndIndex[listInd] + 1, 4);
            }
                          int nextX = ArrayCycle(oldX + (int)dirRndBase[dirRndIndex[listInd]].x, width);
            int nextY = ArrayCycle(oldY + (int)dirRndBase[dirRndIndex[listInd]].y, width);
            if (checkMake(nextX, nextY, depth, maxDepth, 1.0 - perPossible))
            {
                newLand[nowIndex++] = (nextX + nextY * width);
            }
            else
            {
                perPossible = perPossible * notPossible;
            }
            dirRndIndex.Sort();
        }
    }
    private bool checkMake(int _x, int _y, int depth, int maxDepth, double probability)
    {
        double getRandom = ((double)Random.Range(depth / 3, maxDepth)) / ((double)maxDepth) * (testMap[_x, _y] > 0 ? 100.0 : 1.0);
        if (getRandom < probability)
        {
            testMap[_x, _y] = mapLevel;
            notOver.Remove(_x + _y * width);
            return true;
        }
        return false;
    }

    bool noBreakMapLink(int _x, int _y,int checkMap,ref int depth)
    {
        if (depth < 0)  return true;
        int minusCount = 0;
        if (testMap[_x, _y] == checkMap)
        {
            minusCount++;
        }
        depth -= minusCount;
        return (minusCount > 0) ? noBreakMapLink(_x, _y, checkMap, ref depth) : false;
    }
    //int rndPlus = 0;
    void BaseLoad()
    {
        //rndPlus = 100;
        for (int i =0;i<width*width;i++)
        {
            notOver.Add(i);
        }
        testMap = new int[width, height];
        centerHeightPushed = new bool[width, height];
        int[] startXY = new int[0];
        while (notOver.Count > 10)
        {
            startXY = new int[] { notOver[Random.Range(0, notOver.Count)] };
            testMap[startXY[0] % width, startXY[0] / width] = mapLevel;
            startXY = RandamMakeMap(0, 10, startXY, 1);
            mapLevel = ArrayCycle(mapLevel+1,1,maxLevel);
        }
    }
    int testMapDis(int _x, int _y, int t)
    {
        switch (t)
        {
            case 0:
                return Mathf.Abs(testMapDis(_x, _y, 8) - testMapDis(_x, _y, 4));
            case 1:
                return Mathf.Abs(testMapDis(_x, _y, 8) - testMapDis(_x, _y, 5));
            case 2:
                return Mathf.Abs(testMapDis(_x, _y, 8) - testMapDis(_x, _y, 6));
            case 3:
                return Mathf.Abs(testMapDis(_x, _y, 8) - testMapDis(_x, _y, 7));
            case 4:
                return testMapDis(ArrayCycle(_x + 1), _y, 8);
            case 5:
                return testMapDis(_x, ArrayCycle(_y + 1), 8);
            case 6:
                return testMapDis(ArrayCycle(_x - 1), _y, 8);
            case 7:
                return testMapDis(_x, ArrayCycle(_y - 1), 8);
            default:
                return testMap[_x, _y];

        }
    }
    int testMapDis2(int _x, int _y, int t, int w)
    {
        switch (t)
        {
            case 0:
                return Mathf.Abs(testMapDis(_x / w, _y / w, 8) - testMapDis(ArrayCycle((_x + 1) / w), _y / w, 8));
            case 1:
                return Mathf.Abs(testMapDis(_x / w, _y / w, 8) - testMapDis(_x / w, ArrayCycle((_y + 1) / w), 8));
            case 2:
                return Mathf.Abs(testMapDis(_x / w, _y / w, 8) - testMapDis(ArrayCycle((_x - 1) / w), _y / w, 8));
            case 3:
                return Mathf.Abs(testMapDis(_x / w, _y / w, 8) - testMapDis(_x / w, ArrayCycle((_y - 1) / w), 8));
        }
        return 0;
    }

    int ArrayCycle(int i,int limitDown, int limit)
    {
        if (i < limitDown) return i + limit;
        if (i >= limit) return i - limit + limitDown;
        return i;
    }
    int ArrayCycle(int i, int limit)
    {
        if (i < 0) return i + limit;
        if (i >= limit) return i - limit;
        return i;
    }
    int ArrayCycle(int i)
    {
        return ArrayCycle(i, width);
    }

    //要有一個給每個區塊的中心點，表示母座標

    //要有一個以中心點為主，找出十字
    //要有一個以十字為主找出四個象限
    float makeTerrain2(int _x, int _y, float[,] tH)
    {
        /*if (centerHeightPushed[_x, _y]) return tH[_x, _y];
        centerHeightPushed[_x, _y] = true;*/
        int map_Scale = testMap.GetLength(0);//
        int helfRatio = areaRatio / 2;
        int thisX = _x * areaRatio + helfRatio;
        int thisY = _y * areaRatio + helfRatio;
        tH[thisX, thisY] = makeCenter(_x, _y, tH, true);// 要再除錯


        for (int x = 0; x < width; x++)
        {
            thisX = x * areaRatio + helfRatio;
            for (int y = 0; y < width; y++)
            {
                thisY = y * areaRatio + helfRatio;
                for (int i = 1; i >= -1; i -= 2)
                {
                    int nextX = ArrayCycle(x + i, map_Scale);
                    int nextY = ArrayCycle(y + i, map_Scale);
                    int nextTHX = nextX * areaRatio + helfRatio;
                    int nextTHY = nextY * areaRatio + helfRatio;
                    CrossOfTerrain(thisX, thisY, i, 0, tH[thisX, thisY], tH[nextTHX, thisY], tH);
                    CrossOfTerrain(thisX, thisY, 0, i, tH[thisX, thisY], tH[thisX, nextTHY], tH);
                }
            }
        }
        for (int x = 0; x < width; x++)
        {
            thisX = x * areaRatio + helfRatio;
            for (int y = 0; y < width; y++)
            {
                thisY = y * areaRatio + helfRatio;
                for (int i = 1; i >= -1; i -= 2)
                {
                    OneAreaTerrain(thisX, thisY, tH);
                    //OneAreaTerrain(thisX, thisY, tH);
                }
            }
        }

        return 0.0f;
    }
    float makeCenter(int _x, int _y, float[,] tH,bool start)
    {
        int helfRatio = areaRatio / 2;
        int thisX = _x * areaRatio + helfRatio;
        int thisY = _y * areaRatio + helfRatio;
        if (centerHeightPushed[_x, _y]) return tH[thisX, thisY];
        tH[thisX, thisY] = terrainChangeValue(3);
        makeCenter(_x, _y, tH, tH[thisX, thisY], 1);
        return 0.0f;
        
    }
    float makeCenter(int _x, int _y, float[,] tH,float baseHeight, int nowIndex)
    {
        
        int helfRatio = areaRatio / 2;
        int thisX = _x * areaRatio + helfRatio;
        int thisY = _y * areaRatio + helfRatio;
        if (centerHeightPushed[_x, _y]) return tH[thisX, thisY];
        if (float.IsInfinity( baseHeight))
        {
            return  baseHeight;
        }
        tH[thisX, thisY] = baseHeight;
        centerHeightPushed[_x, _y] = true;
        //if (nowIndex > 128) return tH[thisX, thisY];
        List<Vector3> nextIndex = new List<Vector3>();
        float totalValue = 0;

        for (int i = 1; i >= -1; i -= 2)
        {
            int nextX = ArrayCycle(_x + i, width);
            int nextY = ArrayCycle(_y + i, width);
            if (testMapDis(_x, _y, i + 1) == 0)
            {
                totalValue += makeCenter(nextX, _y, tH, baseHeight + terrainChangeValue(0), nowIndex + 1);
            }
            else
            {
                nextIndex.Add(new Vector3(nextX, _y, i + 2));
            }

            if (testMapDis(_x, _y, i + 2) == 0)
            {
                totalValue += makeCenter(_x, nextY, tH, baseHeight + terrainChangeValue(0), nowIndex + 1);
            }
            else
            {
                nextIndex.Add(new Vector3(_x, nextY, i + 2));
            }
        }


        for (int j = 0; j < nextIndex.Count; j++)
        {
            int nextX = (int)nextIndex[j].x;
            int nextY = (int)nextIndex[j].y;
            int nextTHX = nextX * areaRatio + helfRatio;
            int nextTHY = nextY * areaRatio + helfRatio;
            totalValue += makeCenter(nextX, nextY, tH, baseHeight + terrainChangeValue(testMapDis(_x, _y, (int)nextIndex[j].z)), nowIndex + 1);
        }
        return totalValue/4;
    }

    float CrossOfTerrain(int _x, int _y, int tx, int ty, float lastHeight, float nextCenterHeight1, float[,] tH)
    { //_x , _y 是tH的座標
        int ntx = (tx > 0 ? 1 : tx < 0 ? -1 : 0);
        int nty = (ty > 0 ? 1 : ty < 0 ? -1 : 0);
        int helfRatio = areaRatio / 2;
        
        float thisHeight = lastHeight + (nextCenterHeight1- lastHeight) / (areaRatio - Mathf.Abs(tx + ty)) + terrainChangeValue(0);
        int nextTHX = ArrayCycle(_x + ntx, tH.GetLength(0) - 1);
        int nextTHY = ArrayCycle(_y + nty, tH.GetLength(0) - 1);
        tH[_x, _y] = (Mathf.Abs(tx + ty) >= helfRatio ? thisHeight : CrossOfTerrain(nextTHX, nextTHY, tx + ntx, ty + nty, thisHeight, nextCenterHeight1, tH));
        return lastHeight;
    }
    void OneAreaTerrain(int _x, int _y, float[,] tH)
    {
        for (int ntx = -1; ntx <= 1; ntx += 2)
        {
            for (int nty =  -1; nty <= 1; nty += 2)
            {
                OneAreaTerrain(_x + ntx, _y + nty, ntx, nty, tH);
            }
        }
    }
    void OneAreaTerrain(int _x, int _y, int tx, int ty, float[,] tH)
    {
        int helfRatio = areaRatio / 2;
        float[] lastHeight1 = new float[helfRatio];
        for (int j = 0; j < helfRatio; j++)
        {
            lastHeight1[j] = tH[_x-tx, _y + j * ty];
        }
        float lastHeight2 = tH[_x, _y-ty]; 
        for (int x = _x + tx; x % helfRatio != 0; x += tx)
        {
            int nCH2Y = ArrayCycle((_y / helfRatio + (ty * 3 + 1) / 2) * helfRatio, tH.GetLength(0));
            float nextCenterHeight2 = tH[x, nCH2Y];
            int i = 0;
            for (int y = _y + ty; y % helfRatio != 0; y += ty)
            {
                int nCH2X = ArrayCycle((x / helfRatio + (tx * 3 + 1) / 2) * helfRatio, tH.GetLength(0));
                float nextCenterHeight1 = tH[nCH2X, y];
                float thisHeight1 = lastHeight1[i] + (nextCenterHeight1 - lastHeight1[i]) / (areaRatio - Mathf.Abs(x % areaRatio - helfRatio));
                float thisHeight2 = lastHeight2 + (nextCenterHeight2 - lastHeight2) / (areaRatio - Mathf.Abs(y % areaRatio - helfRatio));
                tH[x, y] = (thisHeight1 + thisHeight2) / 2 + terrainChangeValue(0);
                lastHeight1[i++] = tH[x, y];
                lastHeight2 = tH[x, y];
            }

            lastHeight2 = tH[x, _y - ty];
        }
    }
    
    float terrainChangeValue(float dis)
    {
        return Random.Range(-0.00001f, 0.00001f) * Mathf.Pow(3, dis);
    }
    void terrainNomalize(int _wh, float[,] tH)
    {
        float min = float.MaxValue, max = float.MinValue;
        for (int x = 0; x < _wh - 1; x++)
        {
            for (int y = 0; y < _wh - 1; y++)
            {
                if (min > tH[x, y]) min = tH[x, y];
                if (max < tH[x, y]) max = tH[x, y];
            }
        }
        for (int xy = 0; xy < _wh; xy++)
        {
            tH[xy, _wh - 1] = tH[xy, 0] + tH[xy, _wh - 2];
            tH[_wh - 1, xy] = tH[0, xy] + tH[_wh - 2, xy];
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

    public int width, height;
    int areaRatio;
    public Vector3 planeSize;
    private Terrain thePlane;
    public Canvas outputText;
    void Start()
    {
        //Instantiate(thePlane, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        //TerrainData terrainData = thePlane.terrainData;
        thePlane = GetComponent<Terrain>();
        TerrainData terrainData = thePlane.terrainData;
        terrainData.size = planeSize;
        int planeWidth = terrainData.heightmapResolution;//it is square
        areaRatio = planeWidth / width;
        float[,] terrainHeights = new float[planeWidth,planeWidth] ;
        BaseLoad();
        makeTerrain2(Random.Range(0, width), Random.Range(0, width), terrainHeights);
        terrainNomalize(planeWidth, terrainHeights);
        terrainData.SetHeights(0, 0, terrainHeights);
        Vector3 terrainSize = terrainData.size;
        terrainData.size = new Vector3(2000, 1000, 2000);
        thePlane.terrainData = terrainData;

        string mapOutput = "";
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                mapOutput += testMap[i, j].ToString() + " ";
            }
            mapOutput += "\n";
        }

        Canvas _c = Instantiate(outputText);
        _c.GetComponentInChildren<Text>().text = mapOutput;
        _c.GetComponentInChildren<Text>().lineSpacing = 1;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
