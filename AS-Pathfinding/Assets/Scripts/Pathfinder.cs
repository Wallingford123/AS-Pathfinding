using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder
{

    public Grid gridRef;
    public int accuracy;
    private int[] path;
    private float[] cost;
    private List<Edge> traversedEdges;

    int totalNodes;
    // Start is called before the first frame update
    public Pathfinder(Grid grid)
    {
        gridRef = grid;
        totalNodes = gridRef.gridSize * gridRef.gridSize;
        path = new int[totalNodes];
        cost = new float[totalNodes];
        accuracy = 50;
        traversedEdges = new List<Edge>();
        for(int i = 0; i < totalNodes; i++)
        {
            cost[i] = float.MaxValue;
            path[i] = -1;
        }
    }
    public Pathfinder(int acc, Grid grid)
    {
        gridRef = grid;
        totalNodes = gridRef.gridSize * gridRef.gridSize;
        path = new int[totalNodes];
        cost = new float[totalNodes];
        accuracy = acc;
        traversedEdges = new List<Edge>();
        for (int i = 0; i < totalNodes; i++)
        {
            cost[i] = float.MaxValue;
        }
    }

    // Update is called once per frame
    public int[] FindPath(Vector3 target, Vector3 current)
    {
        ResetNodes();
        CalculatePath(target, current);
        return path;
    }

    private void CalculatePath(Vector3 targetPos, Vector3 currentPos)
    {
        int targetNode = PosToInt(targetPos);
        List<Edge> minPriorityQueue = new List<Edge>();
        Node startingNode = PosToNode(currentPos);
        path[0] = PosToInt(currentPos);
        cost[path[0]] = 0;
        foreach (Node n in startingNode.connectedNodes)
        {
            if (!n.occupied)
            {
                Edge e = new Edge(startingNode, n);
                e.priority = (Vector3.Distance(e.a, e.b) + Vector3.Distance(e.b, targetPos)) * n.tileWeight;
                minPriorityQueue.Add(e);
                e.previousEdge = null;
            }
        }
        int a = 0;
        Edge nextEdge = new Edge();
        if (path[0] != targetNode)
        {
            while (minPriorityQueue.Count > 0)
            {
                Edge next = FindLowest(minPriorityQueue);
                int nextA, nextB;
                nextA = PosToInt(next.a);
                nextB = PosToInt(next.b);
                minPriorityQueue.Remove(next);
                traversedEdges.Add(next);
                if (cost[nextB] > cost[nextA] + next.priority)
                {
                    cost[nextB] = cost[nextA] + next.priority;
                    if (nextB == targetNode)
                    {
                        Debug.Log("Found with: " + a + " nodes explored");
                        FinalisePath(next);
                        return;
                    }
                    else
                    {
                        Node nodeRef = PosToNode(next.b);
                        foreach (Node n in nodeRef.connectedNodes)
                        {
                            if (!n.occupied)
                            {
                                bool add = true;
                                foreach (Edge e in minPriorityQueue)
                                {
                                    if (PosToNode(e.a) == nodeRef && PosToNode(e.b) == n)
                                    {
                                        add = false;
                                    }
                                    else if (PosToNode(e.b) == nodeRef && PosToNode(e.a) == n)
                                    {
                                        add = false;
                                    }
                                }
                                if (add)
                                {
                                    Edge e = new Edge(nodeRef, n);
                                    e.priority = (Vector3.Distance(e.a, e.b) + Vector3.Distance(e.b, targetPos)) * n.tileWeight + cost[nextA]/((100-accuracy)+1);
                                    minPriorityQueue.Add(e);
                                    e.previousEdge = next;
                                }
                            }
                        }
                    }
                }
                a++;
            }
            Debug.Log("PATH NOT FOUND");
        }
    }

    private void FinalisePath(Edge nextEdge)
    {
        List<Edge> edges = new List<Edge>();
        while (nextEdge.previousEdge != null)
        {
            edges.Add(nextEdge);
            nextEdge = nextEdge.previousEdge;
        }
        edges.Add(nextEdge);
        int pathNo = 1;
        while (edges.Count > 0)
        {
            path[pathNo] = PosToInt(edges[edges.Count - 1].b);
            edges.Remove(edges[edges.Count - 1]);
            pathNo++;
        }
    }

    private void ResetNodes()
    {
        totalNodes = gridRef.gridSize * gridRef.gridSize;
        path = new int[totalNodes];
        cost = new float[totalNodes];
        traversedEdges = new List<Edge>();
        for (int i = 0; i < totalNodes; i++)
        {
            cost[i] = float.MaxValue;
            path[i] = -1;
        }
    }

    private Edge FindLowest(List<Edge> list)
    {
        Edge lowest = null;
        foreach(Edge e in list)
        {
            if (lowest != null)
            {
                if (e.priority < lowest.priority)
                {
                    lowest = e;
                }
            }
            else lowest = e;
        }
        return lowest;
    }
    public int PosToInt(Vector3 pos)
    {
        int l = (Mathf.FloorToInt(pos.x) * gridRef.gridSize) + (Mathf.FloorToInt(pos.z));
        return l;
    }
    public Node PosToNode(Vector3 pos)
    {
        Node n = gridRef.grid[Mathf.FloorToInt(pos.x),Mathf.FloorToInt(pos.z)];
        return n;
    }
    public Node IntToNode(int nodeInt)
    {
        int x = Mathf.FloorToInt(nodeInt / gridRef.gridSize);
        int z = nodeInt - x * gridRef.gridSize;
        return gridRef.grid[x, z];
    }
    public int NodeToInt(Node node)
    {
        return PosToInt(node.position);
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            foreach (int i in path)
            {
                Gizmos.DrawSphere(IntToNode(i).position + Vector3.up/5, .1f);
            }
        }
    }
}
