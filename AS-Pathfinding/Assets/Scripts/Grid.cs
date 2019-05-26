using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool DrawGizmos;

    public int gridSize;
    public GameObject underPlane;
    public GameObject planeInstance;


    [HideInInspector]
    public Node[,] grid;

    // Start is called before the first frame update
    void Start()
    {

        underPlane.transform.localScale = new Vector3(gridSize/10, gridSize/10, gridSize/10);
        underPlane.transform.position = underPlane.transform.localScale * 5 - new Vector3(0,underPlane.transform.localScale.y * 5, 0);

        grid = new Node[gridSize,gridSize];
        for(int x = 0; x < gridSize; x++)
        {
            for(int y = 0; y < gridSize; y++)
            {
                grid[x, y] = new Node(new Vector3(x + 0.5f, 0, y + 0.5f));

                GameObject l = Instantiate(planeInstance, grid[x,y].position + new Vector3(0,0.01f,0), Quaternion.identity);
                int r = (int)Random.Range(0, 4);
                switch (r)
                {
                    case 0:
                        l.tag = "Untagged";
                        grid[x, y].tileWeight = 1.0f;
                        l.GetComponent<Renderer>().material.color = Color.cyan;
                        break;
                    case 1:
                        l.tag = "Mud";
                        grid[x, y].tileWeight = 1.5f;
                        l.GetComponent<Renderer>().material.color = Color.red;
                        break;
                    case 2:
                        l.tag = "Road";
                        grid[x, y].tileWeight = 0.75f;
                        l.GetComponent<Renderer>().material.color = Color.green;
                        break;
                    case 3:
                        l.tag = "Forest";
                        grid[x, y].tileWeight = 1.25f;
                        l.GetComponent<Renderer>().material.color = Color.yellow;
                        break;
                }
            }
        }
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                if (y != gridSize - 1) grid[x, y].connectedNodes.Add(grid[x, y + 1]);
                if (y != 0) grid[x, y].connectedNodes.Add(grid[x, y - 1]);
                if (x != gridSize - 1) grid[x, y].connectedNodes.Add(grid[x + 1, y]);
                if (x != 0) grid[x, y].connectedNodes.Add(grid[x - 1, y]);

                if (x != gridSize - 1 && y != gridSize - 1) grid[x, y].connectedNodes.Add(grid[x + 1, y + 1]);
                if (x != 0 && y != gridSize - 1) grid[x, y].connectedNodes.Add(grid[x - 1, y + 1]);
                if (x != gridSize - 1 && y != 0) grid[x, y].connectedNodes.Add(grid[x + 1, y - 1]);
                if (x != 0 && y != 0) grid[x, y].connectedNodes.Add(grid[x - 1, y - 1]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateOccupied(Vector3 pos, bool state)
    {
        if (pos.x >= 0 && pos.x <= gridSize && pos.y >= 0 && pos.y <= gridSize)
        grid[(int)pos.x, (int)pos.y].occupied = state;
    }


    private void OnDrawGizmos()
    {
        if (DrawGizmos && Application.isPlaying)
        {
            foreach (Node n in grid)
            {
                if (n.occupied) Gizmos.color = Color.red;
                else Gizmos.color = Color.green;
                Gizmos.DrawSphere(n.position, .25f);
            }
        }
    }

}
public class Edge
{
    public Vector3 a, b;
    public float priority;
    public Edge previousEdge;

    //constructor for easier conversion from Nodes
    public Edge(Node aa, Node bb)
    {
        a = aa.position;
        a = new Vector3(Mathf.FloorToInt(a.x), Mathf.FloorToInt(a.y), Mathf.FloorToInt(a.z));
        b = bb.position;
        b = new Vector3(Mathf.FloorToInt(b.x), Mathf.FloorToInt(b.y), Mathf.FloorToInt(b.z));
    }
    public Edge()
    {
        a = Vector3.zero;
        b = Vector3.zero;
        previousEdge = null;
        priority = float.MaxValue;
    }
}
public class Node
{
    public Vector3 position;
    public float tileWeight;
    public List<Node> connectedNodes;
    public bool occupied;

    public Node(Vector3 pos)
    {
        position = pos;
        connectedNodes = new List<Node>();
    }
    public Node()
    {
        position = new Vector3(-1, -1);
        connectedNodes = null;
    }
}
