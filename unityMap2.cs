//not ok version
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    int[,] testMap;
    bool[,] centerHeightPushed;
    int mapLevel = 1;
    int maxLevel = 7;
    int[] RandamMakeMap(int depth, int maxDepth, int[] oldLand, int oldLength){
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
    private bool checkMake(int _x, int _y, int depth, int maxDepth, double probability){
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
    void BaseLoad(){
        //rndPlus = 100;
        testMap = new int[width, height];
        centerHeightPushed = new bool[width,height];
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
                 return Mathf.Abs(testMapDis(_x, _y, 8) - testMapDis(_x , _y, 4));
            case 1:
                return Mathf.Abs(testMapDis(_x, _y, 8) - testMapDis(_x , _y , 5));
            case 2:
                return Mathf.Abs(testMapDis(_x, _y, 8) - testMapDis(_x , _y, 6));
            case 3:
                return Mathf.Abs(testMapDis(_x, _y, 8) - testMapDis(_x , _y, 7));
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
    int testMapDis2(int _x,int _y,int t, int w)
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
    }

    int ArrayCycle(int i, int limit){
        if (i < 0) return i+limit; 
        if (i > limit) return i-limit;
        return i; 
    }
    int ArrayCycle(int i){
        return ArrayCycle(i,width); 
    }

    //要有一個給每個區塊的中心點，表示母座標

    //要有一個以中心點為主，找出十字
    //要有一個以十字為主找出四個象限
    float makeTerrain2(int _x, int _y, int _w, int _h, float[,] tH)
    {
        if (centerHeightPushed[_x,_y]) return tH[_x,_y];
        centerHeightPushed[_x,_y] = true;

        int map_Scale = testMap.getLength(0);//
        int helfRatio = areaRatio / 2;
        int thisX = _x * areaRatio + helfRatio;
        int thisY = _y * areaRatio + helfRatio;
        int limitValue = 0;
        for (int i = 1 ; i >= -1;i-=2){
            limitValue += map_Scale;
            int nextX =  ArrayCycle(_x+i,map_Scale);
            int nextY =  ArrayCycle(_y+i,map_Scale);
            tH[nextX * areaRatio + helfRatio,thisY] = makeCenter(nextX,_y,i,0,tH);
            CrossOfTerrain(thisX,thisY, c, 0,tH[thisX,thisY],tH[nextX * areaRatio + helfRatio,thisY],tH);
            tH[thisX,nextY * areaRatio + helfRatio] = makeCenter(_x,nextY,0,i,tH);
            CrossOfTerrain(thisX,thisY, 0, i,tH[thisX,thisY],tH[thisX,nextY * areaRatio + helfRatio],tH);
        }
    }
    
    float makeCenter(int _x, int _y,int tx,int ty, float[,] tH){
        int helfRatio = areaRatio / 2;
        int thisX = _x * areaRatio + helfRatio;
        int thisY = _y * areaRatio + helfRatio;
        if (centerHeightPushed[_x,_y]) return tH[thisX,_y];
        centerHeightPushed[_x,_y] = true;
        int _Xleft = ArrayCycle(_x-1),_Xright=ArrayCycle(_x+1),_Yup = ArrayCycle(_y-1),_Ydown=ArrayCycle(_y+1);

        tH[thisX, _y] = tH[_Xleft, _y] + terrainChangeValue(testMapDis(_x, _y, 0));
    }
    float CrossOfTerrain(int _x, int _y,int tx,int ty, float lastHeight, float nextCenterHeight1, float[,] tH)
    {
        
        int helfRatio = areaRatio / 2;
        float thisHeight = 0.0f;
        return OneAreaTerrain(_x+tx,_y+ty,thisHeight,nextCenterHeight1,tH);
    }
    float OneAreaTerrain(int _x, int _y,int tx,int ty, float lastHeight, float centerHeight1, float[,] tH)
    {
        
    }

    float terrainChangeValue(float dis)
    {
        return Random.Range(-0.0001f, 0.0001f) * Mathf.Pow(3, dis);
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

