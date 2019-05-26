using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Grid gridRef;

    private List<Vector3> affectedNodes;
    private Vector3 previousPos;
    private List<Vector3> prevNodes;

    private void Start()
    {
        prevNodes = new List<Vector3>();
        previousPos = transform.position;
        //transform.position = new Vector3(Random.Range(1, gridRef.gridSize-1),3, Random.Range(1, gridRef.gridSize-1));
        affectedNodes = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position != previousPos)
        {
            foreach(Vector3 v in prevNodes)
            {
                gridRef.UpdateOccupied(v, false);
            }
            prevNodes = new List<Vector3>();
            affectedNodes = new List<Vector3>();
            Vector3 ne, sw;
            ne = transform.position + new Vector3(transform.localScale.x / 2, 0, transform.localScale.z / 2);
            ne = new Vector2(Mathf.CeilToInt(ne.x), Mathf.CeilToInt(ne.z));
            sw = transform.position + new Vector3(-transform.localScale.x / 2, 0, -transform.localScale.z / 2);
            sw = new Vector2(Mathf.FloorToInt(sw.x), Mathf.FloorToInt(sw.z));

            List<Vector3> removeNodes = new List<Vector3>();

            int x, y;
            x = (int)ne.x - (int)sw.x;
            y = (int)ne.y - (int)sw.y;

            for (int xx = 0; xx < x; xx++)
            {
                for (int yy = 0; yy < y; yy++)
                {
                    Vector2 v = new Vector2(Mathf.FloorToInt(sw.x + xx), Mathf.FloorToInt(sw.y + yy));
                    affectedNodes.Add(v);
                    gridRef.UpdateOccupied(v, true);
                }
            }
            while(affectedNodes.Count > 0)
            {
                prevNodes.Add(affectedNodes[0]);
                affectedNodes.Remove(affectedNodes[0]);
            }
        }
    }
}
