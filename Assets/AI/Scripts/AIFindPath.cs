using System.Collections;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class AIFindPath : MonoBehaviour
{

    public GameObject[] nodes;

    List<List<GameObject>> recentPaths;

    public const float RESET_RECENT_PATHS_TIME = 60;   // time in seconds to reset the resent paths list;
    float timer = 0;

    public const int MAX_STEPS = 300;
    // Start is called before the first frame update
    void Start()
    {
        nodes = GameObject.FindGameObjectsWithTag("Node");
        for (int i = 0; i < nodes.Length; ++i)
        {
            nodes[i].GetComponent<Node>().index = i;
        }
        recentPaths = new List<List<GameObject>>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= RESET_RECENT_PATHS_TIME)
        {
            timer = 0;
            recentPaths.Clear();
        }
    }

    public List<GameObject> findPath(int startNode, int finishNode, bool reverseSearch)
    {
        Stopwatch watch = new Stopwatch();
        foreach ( var recentPath in recentPaths)
        {
            if(recentPath[0].GetComponent<Node>().index == startNode && recentPath[recentPath.Count - 1].GetComponent<Node>().index == finishNode)
            {
                watch.Stop();
                TimeSpan ts = watch.Elapsed;
                string elapsedTime = String.Format("Seconds:{0:00} Milliseconds:{1:00} NanoSeconds:{2:00}",
                    ts.TotalSeconds,
                    ts.TotalMilliseconds,
                    ts.TotalMilliseconds * 1000000);
                //UnityEngine.Debug.Log("Reverse Search: " + reverseSearch + " Time taken " + elapsedTime + "  steps taken: 0");
                return recentPath;
            }
        }
        
        
        if (reverseSearch)
        {
            int temp = startNode;
            startNode = finishNode;
            finishNode = temp;
        }

        watch.Start();
        
        
        List<GameObject> path = new List<GameObject>();
        List<int> deadEnds = new List<int>();
        List<int> explored = new List<int>();
        

        path.Add(nodes[startNode]);
        if (startNode == finishNode)
        {
            return path; ;
        }
        
        
        int pathCount = 0;

        float directDistance = (nodes[startNode].transform.position - nodes[finishNode].transform.position).magnitude;
        
        
        int steps = 0;
        int current_node = startNode;
        int prev_node = startNode;


        while (current_node != finishNode && steps < MAX_STEPS)
        {
            steps++;
            directDistance = (path[pathCount].GetComponent<Node>().transform.position - nodes[finishNode].transform.position).magnitude;
            int index = -1;

            for (int i = 0; i < path[pathCount].GetComponent<Node>().distances.Count; ++i)
            {
                if (directDistance > (path[pathCount].GetComponent<Node>().distances[i].Item1.transform.position - nodes[finishNode].transform.position).magnitude && path[pathCount].GetComponent<Node>().distances[i].Item1.GetComponent<Node>().index != prev_node)
                {
                    if (!deadEnds.Contains(path[pathCount].GetComponent<Node>().distances[i].Item1.GetComponent<Node>().index) && !explored.Contains(path[pathCount].GetComponent<Node>().distances[i].Item1.GetComponent<Node>().index))
                    {
                        directDistance = (path[pathCount].GetComponent<Node>().distances[i].Item1.transform.position - nodes[finishNode].transform.position).magnitude;
                        index = i;
                    }
                }
            }
            
            if (index >= 0)
            {
                prev_node = current_node;
                current_node = path[pathCount].GetComponent<Node>().distances[index].Item1.GetComponent<Node>().index;
                path.Add(nodes[current_node]);
                explored.Add(prev_node);
                pathCount++;
            }
            else
            {
                float distance = 99999999;
                for (int i = 0; i < path[pathCount].GetComponent<Node>().distances.Count; ++i)
                {
                    if (distance > path[pathCount].GetComponent<Node>().distances[i].Item2 && path[pathCount].GetComponent<Node>().distances[i].Item1.GetComponent<Node>().index != prev_node)
                    {
                        if (!deadEnds.Contains(path[pathCount].GetComponent<Node>().distances[i].Item1.GetComponent<Node>().index) && !explored.Contains(path[pathCount].GetComponent<Node>().distances[i].Item1.GetComponent<Node>().index))
                        {
                            distance = path[pathCount].GetComponent<Node>().distances[i].Item2;
                            index = i;
                        }
                    }
                }
                if (distance < 999)
                {
                    prev_node = current_node;
                    current_node = path[pathCount].GetComponent<Node>().distances[index].Item1.GetComponent<Node>().index;
                    path.Add(nodes[current_node]);
                    explored.Add(prev_node);
                    pathCount++;
                }
                else
                {
                    path.RemoveAt(pathCount);
                    deadEnds.Add(current_node);
                    current_node = path[pathCount - 1].GetComponent<Node>().index;
                    prev_node = path[pathCount - 2].GetComponent<Node>().index;
                    pathCount--;
                }
            }
            if (current_node == finishNode)
            {
                if (reverseSearch)
                {
                    path.Reverse();
                }
                
                watch.Stop();
                TimeSpan ts = watch.Elapsed;
                string elapsedTime = String.Format("Seconds:{0:00} Milliseconds:{1:00} NanoSeconds:{2:00}",
            ts.TotalSeconds,
            ts.TotalMilliseconds,
            ts.TotalMilliseconds * 1000000);
                //UnityEngine.Debug.Log("Reverse Search: " + reverseSearch + " Time taken " + elapsedTime + "  steps taken: " + steps);

                recentPaths.Add(path);
                return path;
            }
        }
        
        UnityEngine.Debug.Log("cant find path");
        {
            watch.Stop();
            TimeSpan ts = watch.Elapsed;
            string elapsedTime = String.Format("Seconds:{0:00} Milliseconds:{1:00} NanoSeconds:{2:00}",
        ts.TotalSeconds,
        ts.TotalMilliseconds,
        ts.TotalMilliseconds * 1000000);
            //UnityEngine.Debug.Log("Time taken " + elapsedTime);
        }
        List<GameObject> noPath = new List<GameObject>();
        return noPath;
    }
}
