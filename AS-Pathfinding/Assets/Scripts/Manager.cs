using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public List<Unit> units;

    private List<Unit> selectedUnits;

    public float pathFollowWeight;
    public float cohesionWeight;
    public float separationWeight;
    public float alignmentWeight;

    [Range(1, 100)]
    public int accuracy;
    public Camera cam;
    public float completionRadius;
    public float camSpeed;

    private Pathfinder pathFinder;

    bool selecting = false;
    Vector3 boxPointA, boxPointB;

    // Start is called before the first frame update
    void Start()
    {
        selectedUnits = new List<Unit>();
        pathFinder = new Pathfinder(accuracy,GameObject.Find("Manager").GetComponent<Grid>());
    }

    // Update is called once per frame
    void Update()
    {
        cam.transform.position = new Vector3(cam.transform.position.x + (Input.GetAxis("Horizontal") / 10) * camSpeed, cam.transform.position.y, cam.transform.position.z);
        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, cam.transform.position.z + (Input.GetAxis("Vertical") / 10) * camSpeed);


        if (Input.GetMouseButtonDown(0))
        {
            selecting = true;
            RaycastHit hit;
            if (Physics.Raycast(cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, 10000))
            {
                boxPointA = hit.point;
                selectedUnits.Clear();
                foreach (Unit u in units)
                {
                    u.GetComponent<Renderer>().material.color = Color.white;
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            selecting = false;
            RaycastHit hit;
            if (Physics.Raycast(cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, 10000))
            {
                boxPointB = hit.point;
                if (boxPointA.x > boxPointB.x)
                {
                    Vector3 newA = new Vector3(boxPointB.x, 0, boxPointA.z);
                    boxPointB = new Vector3(boxPointA.x, 0, boxPointB.z);
                    boxPointA = newA;
                }
                if (boxPointA.z > boxPointB.z)
                {
                    Vector3 newA = new Vector3(boxPointA.x, 0, boxPointB.z);
                    boxPointB = new Vector3(boxPointB.x, 0, boxPointA.z);
                    boxPointA = newA;
                }
                Debug.Log(boxPointA);
                Debug.Log(boxPointB);
                for (int i = 0; i < units.Count; i++)
                {
                    if (units[i].transform.position.x > boxPointA.x && units[i].transform.position.x < boxPointB.x &&
                        units[i].transform.position.z > boxPointA.z && units[i].transform.position.z < boxPointB.z)
                    {
                        selectedUnits.Add(units[i]);
                        units[i].GetComponent<Renderer>().material.color = Color.blue;
                    }
                }
            }
            boxPointA = Vector3.zero;
            boxPointB = Vector3.zero;
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (selecting)
            {
                selecting = false;
                boxPointA = Vector3.zero;
                boxPointB = Vector3.zero;
            }
            else
            {
                Vector3 middle = Vector3.zero, mousePos;
                RaycastHit hit;
                if (Physics.Raycast(cam.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition), out hit, 10000))
                {
                    mousePos = hit.point;
                    foreach (Unit u in selectedUnits)
                    {
                        middle += u.transform.position;
                    }
                    middle /= selectedUnits.Count;
                    Debug.Log(middle);
                    int[] path = pathFinder.FindPath(mousePos, middle);
                    foreach (Unit u in selectedUnits)
                    {
                        u.SetPath(path);
                    }
                }
            }
        }
    }
}
