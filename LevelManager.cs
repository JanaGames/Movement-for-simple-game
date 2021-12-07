using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public GameManager gameManager;
    public List<Transform> walkPoints;
    public List<GameObject> enemiesPrefub;
    List<GameObject> enemies = new List<GameObject>();
    public int LevelPoint;
    public int CurrentPoint;
    public float timeAttack;
    bool pointSet;

    [HideInInspector]
    static public int indexGen;

    [HideInInspector]
    static public int indexLevel;
    private IEnumerator coroutine;
    void Start()
    {
        LoadEnemiesByGen();
        GetWalkPoints();
        SetIndex();
        SetPointsToEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemies.Count > 0) Set();
        if (CurrentPoint > 0) CheckPoint();
    }
    void CheckPoint() 
    {
        if (CurrentPoint == LevelPoint) 
        {
            indexGen++;
            gameManager.ReStartLevelGenUp(indexGen);
        }
    }
    void Set() 
    {
        foreach(GameObject enemy in enemies) 
        {
            if (enemy.GetComponent<Enemy>().readyWalk && !pointSet) 
            {
                //if walk point have inner points
                if (enemy.GetComponent<Enemy>().currentInnerPoint == 0 && walkPoints[enemy.GetComponent<Enemy>().currentPoint].GetComponent<WalkPoint>().innerPoints.Count != 0) 
                {
                    //next point or inner point;
                    int value = Random.Range(0, 9);
                    if (value >= 7) enemy.GetComponent<Enemy>().MoveTo(Random.Range(0, walkPoints.Count));
                    else SetPoint(enemy, walkPoints[enemy.GetComponent<Enemy>().currentPoint].GetComponent<WalkPoint>().innerPoints);
                }
                //if walk point hav'nt inner points
                if (enemy.GetComponent<Enemy>().currentInnerPoint == 0 && walkPoints[enemy.GetComponent<Enemy>().currentPoint].GetComponent<WalkPoint>().innerPoints.Count == 0) 
                {
                    //next point;
                    enemy.GetComponent<Enemy>().MoveTo(Random.Range(0, walkPoints.Count));
                }
                //if enemy walk on inner points
                if (enemy.GetComponent<Enemy>().currentInnerPoint != 0) 
                {
                    //next inner point or parent point;
                    int value = Random.Range(0, 9);
                    if (value >= 5) SetPoint(enemy, walkPoints[enemy.GetComponent<Enemy>().currentPoint].GetComponent<WalkPoint>().innerPoints);
                    else enemy.GetComponent<Enemy>().MoveToInner(0, walkPoints[enemy.GetComponent<Enemy>().currentPoint].GetComponent<WalkPoint>().innerPoints);
                }
                coroutine = SetPoint(3.0f);
                StartCoroutine(coroutine);
            }
            //if (walkPoints[enemy.GetComponent<Enemy>().currentPoint].GetComponent<WalkPoint>().innerPoints != 0)
        }
    }
    void SetPoint(GameObject enemy, List<Transform> innerPoints) 
    {
        enemy.GetComponent<Enemy>().MoveToInner(Random.Range(1, innerPoints.Count + 1), innerPoints);
    }
    private IEnumerator SetPoint(float waitTime)
    {
        pointSet = true;
        yield return new WaitForSeconds(waitTime);
        pointSet = false;
    }
    void LoadEnemiesByGen() 
    {
        for(int i = 0; i < indexGen + 1; i++) 
        {
            enemies.Add(enemiesPrefub[i]);
        }
    }
    void GetWalkPoints() 
    {
        for(int i = 0; i < transform.childCount; i++) 
        {
            if (transform.GetChild(i).CompareTag("WalkPoint")) 
            {
                walkPoints.Add(transform.GetChild(i));
            }
        }
    }
    void SetIndex() 
    {
        for(int i = 0; i < walkPoints.Count; i++) 
        {
            walkPoints[i].GetComponent<WalkPoint>().index = i;
        }
    }
    void SetPointsToEnemies() 
    {
        for(int i = 0; i < enemies.Count; i++) 
        {
            enemies[i].GetComponent<Enemy>().walkPoints = walkPoints;
            GameObject obj = Instantiate(enemies[i], walkPoints[i].position, enemies[i].transform.rotation);
            enemies[i] = obj;
            //
            enemies[i].GetComponent<Enemy>().timeAttack = timeAttack;
            //enemies[i].transform.position = walkPoints[0].position;
        }
    }
}
