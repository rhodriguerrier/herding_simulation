using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HerdAgent : MonoBehaviour
{
    // Vector3 acceleration;
    Vector3 velocity;
    float startSpeed = 7.0f;
    public float minSpeed = 5.0f;
    public float maxSpeed = 10.0f;
    public float visibleRadius = 10.0f;
    public float desiredSep = 7.0f;
    [Range(0f, 1f)]
    public float separationWeight = 0.97f;
    [Range(0f, 1f)]
    public float alignmentWeight = 0.65f;
    [Range(0f, 1f)]
    public float cohesiveWeight = 0.13f;
    [Range(1f, 100f)]
    public float collisionAvoidWeight = 50f;

    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        velocity = transform.forward * startSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 steerAwayForce = separateHerdAgents() * separationWeight;
        velocity += steerAwayForce * Time.deltaTime;

        Vector3 steerAlignForce = alignHerdAgents() * alignmentWeight;
        velocity += steerAlignForce * Time.deltaTime;

        Vector3 steerCohesiveForce = cohesiveHerdAgents() * cohesiveWeight;
        velocity += steerCohesiveForce * Time.deltaTime;

        if (collisionInBound())
        {
            Vector3 steerObstacleForce = avoidObstacle() * collisionAvoidWeight;
            velocity += steerObstacleForce * Time.deltaTime;
        }
        
        float speed = velocity.magnitude;
        Vector3 dir = velocity / speed;
        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
        velocity = dir * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = dir;
    }

    Vector3 separateHerdAgents()
    {
        Vector3 steer = new Vector3();
        Vector3 sum = new Vector3();
        int neighbourCount = 0;
        Collider[] neighbours = Physics.OverlapSphere(transform.position, visibleRadius);
        foreach (var neighbour in neighbours)
        {
            if (neighbour.tag == "buffalo")
            {
                if (IsInFront(neighbour))
                {
                    float dist = Vector3.Distance(transform.position, neighbour.transform.position);
                    if ((dist > 0) && (dist < desiredSep))
                    {
                        Vector3 diff = transform.position - neighbour.transform.position;
                        diff.Normalize();
                        diff /= dist;
                        sum += diff;
                        neighbourCount++;
                    }
                }
            }
        }

        if (neighbourCount > 0)
        {
            sum /= neighbourCount;
            sum.Normalize();
            sum *= maxSpeed;
            steer = sum - velocity;
        }

        return steer;
    }

    Vector3 alignHerdAgents()
    {
        Vector3 steer = new Vector3();
        Vector3 sum = new Vector3();
        int neighbourCount = 0;
        Collider[] neighbours = Physics.OverlapSphere(transform.position, visibleRadius);
        foreach (var neighbour in neighbours)
        {
            if (neighbour.tag == "buffalo")
            {
                if (IsInFront(neighbour))
                {
                    sum += neighbour.transform.forward;
                    neighbourCount++;
                }
            }
        }

        if (neighbourCount > 0)
        {
            sum /= neighbourCount;
            sum.Normalize();
            sum *= maxSpeed;
            steer = sum - velocity;
        }

        return steer;
    }

    Vector3 cohesiveHerdAgents()
    {
        Vector3 steer = new Vector3();
        Vector3 sum = new Vector3();
        int neighbourCount = 0;
        Collider[] neighbours = Physics.OverlapSphere(transform.position, visibleRadius);
        foreach (var neighbour in neighbours)
        {
            if (neighbour.tag == "buffalo")
            {
                if (IsInFront(neighbour))
                {
                    if (transform.position != neighbour.transform.position)
                    {
                        sum += neighbour.transform.position;
                        neighbourCount++;
                    }
                }
            }
        }

        if (neighbourCount > 0)
        {
            sum /= neighbourCount;
            Vector3 diff = sum - transform.position;
            diff.Normalize();
            diff *= maxSpeed;
            steer = diff - velocity;
        }

        return steer;
    }

    bool IsInFront(Collider subject)
    {
        Vector3 offSetVec = subject.transform.position - transform.position;
        offSetVec.Normalize();
        float dotOf = Vector3.Dot(transform.forward, offSetVec);
        if (dotOf >= 0.0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    bool collisionInBound()
    {
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, 0.27f, transform.forward, out hit, 15f, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 avoidObstacle()
    {
        int numViewDirections = 90;
        Vector3 posOnSphere = transform.forward * 10f;
        Vector3 dir = new Vector3();
        float newX;
        float newZ;
        float currentRot = transform.eulerAngles.y;
        for (int i = 0; i < numViewDirections; i += 2)
        {
            if (i % 2 == 0)
            {
                newZ = Mathf.Cos(degreesToRads(currentRot + i)) * 15f;
                newX = Mathf.Sin(degreesToRads(currentRot + i)) * 15f;
            }
            else
            {
                newZ = Mathf.Cos(degreesToRads(currentRot - i)) * 15f;
                newX = Mathf.Sin(degreesToRads(currentRot - i)) * 15f;
            }

            posOnSphere = new Vector3(transform.position.x + newX, 0.0f, transform.position.z + newZ);
            dir = posOnSphere - transform.position;
            dir.Normalize();
            Ray ray = new Ray(transform.position, dir);
            if (!Physics.SphereCast(ray, 0.27f, 30f, layerMask))
            {
                return dir * maxSpeed - velocity;
            }
        }
        return dir;
    }

    float degreesToRads(float degree)
    {
        return (degree * (Mathf.PI/180f));
    }

}
