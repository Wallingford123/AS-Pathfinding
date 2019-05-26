using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    public float speed;
    public float nRadius;

    private Vector3 force;
    private int[] path;
    private Manager manager;
    private Pathfinder pathFinder;

    [HideInInspector]
    public bool hasPath;
    int nextNode, currentNode, finalNode;
    Rigidbody rig;
    // Start is called before the first frame update
    void Start()
    {
        
        rig = GetComponent<Rigidbody>();
        pathFinder =  new Pathfinder(GameObject.Find("Manager").GetComponent<Grid>());
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        hasPath = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (hasPath)
        {
            force += FollowPath() * manager.pathFollowWeight;
            force += Alignment() * manager.alignmentWeight;
            force += Cohesion() * manager.cohesionWeight;
        }
        force += Separation() * manager.separationWeight;
        ApplyForce();
        //Separation
        //Alignment
        //Cohesion
    }

    Vector3 FollowPath()
    {
        if (path[currentNode] != -1)
        {
            Node nodeRef = pathFinder.IntToNode(path[currentNode]);
            Vector3 steeringForce = new Vector3(force.x + (nodeRef.position.x - transform.position.x), 0, force.z + (nodeRef.position.z - transform.position.z));
            if (Vector3.Distance(transform.position, nodeRef.position + new Vector3(0, 0.5f, 0)) < 0.4f)
            {
                currentNode++;
                nextNode++;
            }
            else if (path[nextNode] != -1 && path[currentNode] != -1)
            {
                if (Vector3.Distance(transform.position, pathFinder.IntToNode(path[currentNode]).position) > Vector3.Distance(transform.position, pathFinder.IntToNode(path[nextNode]).position))
                {
                    currentNode++;
                    nextNode++;
                }
            }
            Rigidbody rig = GetComponent<Rigidbody>();

            if (Vector3.Distance(transform.position, pathFinder.IntToNode(finalNode).position) < manager.completionRadius)
            {
                foreach (Unit u in manager.units)
                {
                    if (!u.hasPath)
                    {
                        if (Vector3.Distance(u.transform.position, transform.position) < 0.2f)
                        {
                            hasPath = false;
                        }
                    }
                }
            }
            return steeringForce;
        }
        else
        {
            hasPath = false;
            return Vector3.zero;
        }
    }

    Vector3 Separation()
    {
        Vector3 steeringForce = Vector3.zero;
        
        foreach (Unit u in manager.units)
        {
            if (u != this)
            {
                float distance = Vector3.Distance(u.transform.position, transform.position);
                Vector3 direction = u.transform.position - transform.position;
                if (distance < 0.25f)
                {
                    direction.Normalize();
                    direction /= (distance*10);
                    steeringForce -= direction;
                }
            }
        }
        return steeringForce;
    }


    Vector3 Alignment()
    {
        Vector3 steeringForce = Vector3.zero;
        Vector3 averageForward = Vector3.zero;

        int count = 0;
        foreach(Unit u in manager.units)
        {
            if (u != this)
            {
                if (Vector3.Distance(u.transform.position, transform.position) < nRadius)
                {
                    averageForward += u.transform.forward;
                    count++;
                }
            }
        }
        averageForward /= count;
        steeringForce = averageForward - transform.forward;

        return steeringForce;
    }

    Vector3 Cohesion()
    {
        Vector3 steeringForce = Vector3.zero;
        Vector3 averagePosition = Vector3.zero;

        int count = 0;
        foreach (Unit u in manager.units)
        {
            if (u != this)
            {
                if(Vector3.Distance(u.transform.position, transform.position) < nRadius)
                {
                    averagePosition += u.transform.position;
                    count++;
                }
            }
        }
        averagePosition /= count;

        steeringForce = averagePosition - transform.position;
        return steeringForce;
    }

    void ApplyForce()
    {
        force.Normalize();
        force = new Vector3(force.x, 0, force.z);
        if (force != Vector3.zero)
            transform.forward = force;
        rig.velocity = force;
        force = Vector3.zero;
    }

    public void SetPath(int[] p)
    {
        int finalInt = 0;
        for (int i = 0; p[i] != -1; i++)
        {
            finalInt = i;
        }
        path = p;
        hasPath = true;
        currentNode = 1;
        nextNode = 2;
        finalNode = p[finalInt];
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            Gizmos.color = Color.red;
            float maxLength = GetComponent<Rigidbody>().velocity.magnitude;
            //Check if there has been a hit yet
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, rig.velocity * maxLength);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(transform.position + rig.velocity * maxLength, transform.localScale);
        }
    }

}
