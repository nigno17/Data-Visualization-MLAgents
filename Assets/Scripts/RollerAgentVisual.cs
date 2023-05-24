using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.IO;

public class RollerAgentVisual : Agent
{
    // Start is called before the first frame update
    Rigidbody rBody;
    public Transform Target;
    public GameObject Obstacle;
    public int numObstacles = 3;
    public float forceMultiplier = 10;
    private List<GameObject> ObstacleList;

    int collideFlag = 0;

    void Start () {
        rBody = GetComponent<Rigidbody>();
        collideFlag = 0;
        ObstacleList = new List<GameObject>();
        for (int i = 0; i < numObstacles; i++)
        {
            GameObject tempObj = Instantiate(Obstacle, new Vector3(0f, 1f, 0f), Quaternion.identity);
            tempObj.transform.parent = transform.parent;
            ObstacleList.Add(tempObj);
        }
    }

    public override void OnEpisodeBegin()
    {
       // If the Agent fell, zero its momentum
        if (transform.localPosition.y < 0)
        {
            rBody.angularVelocity = Vector3.zero;
            rBody.velocity = Vector3.zero;
            transform.localPosition = new Vector3( 0, 0.5f, 0);
        }

        // Move the target to a new spot
        Target.localPosition = new Vector3(Random.Range(-4.0f, 4.0f),
                                           0.5f,
                                           Random.Range(-4.0f, 4.0f));

        foreach (GameObject obs in ObstacleList)
        {
            obs.transform.localPosition = new Vector3(Random.Range(-4.0f, 4.0f),
                                                      0.5f,
                                                      Random.Range(-4.0f, 4.0f));
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        rBody.AddForce(controlSignal * forceMultiplier);

        // Rewards
        //float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);

        // Reached target
        // if (distanceToTarget < 1.42f)
        // {
        //     SetReward(1.0f);
        //     EndEpisode();
        // }
        if (collideFlag == 1)
        {
            collideFlag = 0;
            SetReward(1.0f);
            EndEpisode();
        }
        // Collision with obstacle
        else if (collideFlag == 2)
        {
            collideFlag = 0;
            SetReward(-1f);
            //EndEpisode();
        }
        // Fell off platform
        else if (transform.localPosition.y < 0)
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = -Input.GetAxis("Horizontal");
        continuousActionsOut[1] = -Input.GetAxis("Vertical");
    }

    private void OnCollisionStay(Collision other) {
        if (other.collider.CompareTag("target"))
        {
            collideFlag = 1;
        }

        if (collideFlag == 0 && other.collider.CompareTag("obstacle"))
        {       
            collideFlag = 2;
        }
    }
}
