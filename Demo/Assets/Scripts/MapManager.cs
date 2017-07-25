using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public GameObject[] outWall;
    public GameObject[] floor;
    public GameObject[] wall;
    public GameObject[] food;
    public GameObject[] enemy;
    public GameObject exit;


    public int rows=10;
    public int cols=10;

    public int minWall = 2;
    public int maxWall = 8;

    private Transform mapHolder;
    private List<Vector2> positionList=new List<Vector2>();

    private GameManager gameManager;


    //初始化地图
    public void InitMap()
    {
        gameManager = this.GetComponent<GameManager>();
        mapHolder = new GameObject("Map").transform;

        //创建围墙和地板
        for (int x = 0; x < cols; x++)
        {
            for (int y = 0; y < rows;y++ )
            {
                if (x == 0 || y == 0 || x == cols - 1 || y == rows - 1)
                {
                    int index = Random.Range(0, outWall.Length);
                    GameObject go = GameObject.Instantiate(outWall[index], new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    go.transform.SetParent(mapHolder);
                }
                else
                {
                    int index = Random.Range(0, floor.Length);
                    GameObject go = GameObject.Instantiate(floor[index], new Vector3(x, y, 0), Quaternion.identity) as GameObject;
                    go.transform.SetParent(mapHolder);
                }

            }
        }

        
        positionList.Clear();
        for (int x = 2; x < cols - 2; x++)
        {
            for (int y = 2; y < rows - 2; y++)
            {
                positionList.Add(new Vector2(x, y));
            }
        }

        //创建障碍物
        int wallCount = Random.Range(minWall, maxWall + 1);  //障碍物个数
        InstantiateItems(wallCount, wall);
        
        //创建食物
        int foodCount = Random.Range(2, gameManager.level *2 );
        InstantiateItems(foodCount, food);

        //创建敌人
        int enemyCount = gameManager.level / 2;
        InstantiateItems(enemyCount, enemy);

        //创建出口
        GameObject ex= Instantiate(exit, new Vector2(cols - 2, rows - 2), Quaternion.identity)as GameObject;
        ex.transform.SetParent(mapHolder);

    }

    private void InstantiateItems(int count, GameObject[] prefabs)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 pos = RandomPosition();
            GameObject Prefab = RandomPrefab(prefabs);
            GameObject go = Instantiate(Prefab, pos, Quaternion.identity) as GameObject;
            go.transform.SetParent(mapHolder);
        }
    }


    private Vector2 RandomPosition()
    {
        int positionIndex = Random.Range(0, positionList.Count);
        Vector2 pos = positionList[positionIndex];
        positionList.RemoveAt(positionIndex);
        return pos;
    }

    private GameObject RandomPrefab(GameObject[] prefabs)
    {
        int index = Random.Range(0, prefabs.Length);
        return prefabs[index];
    }
}
