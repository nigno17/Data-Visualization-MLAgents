using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class DQNAgent : Agent
{
    // Start is called before the first frame update
    public Transform Target;
    public Transform Obstacle;

    const int k_NoActions = 0;
    const int k_Up = 1;
    const int k_Down = 2;
    const int k_Left = 3;
    const int k_Right = 4;

    void Start () {
    }

    public override void OnEpisodeBegin()
    {

        // Move the agent, target and obstacle to a new spot
        transform.localPosition = new Vector3(Random.Range(-2, 3),
                                              1.0f,
                                              Random.Range(-2, 3));

        do
        {
            Target.localPosition = new Vector3(Random.Range(-2, 3),
                                               0.5f,
                                               Random.Range(-2, 3));
        } 
        while ((Vector3.Distance(transform.localPosition, Target.localPosition) < 0.6f));

        do
        {
            Obstacle.localPosition = new Vector3(Random.Range(-2, 3),
                                                 0.5f,
                                                 Random.Range(-2, 3));
        } 
        while ((Vector3.Distance(transform.localPosition, Obstacle.localPosition) < 0.6f) ||
               (Vector3.Distance(Target.localPosition, Obstacle.localPosition) < 0.6f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation(Target.localPosition);
        sensor.AddObservation(Obstacle.localPosition);
        sensor.AddObservation(transform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var action = actionBuffers.DiscreteActions[0];

        switch (action)
        {
            case k_NoActions:
                break;
            case k_Right:
                transform.localPosition = transform.localPosition + new Vector3(1f, 0, 0f);
                break;
            case k_Left:
                transform.localPosition = transform.localPosition + new Vector3(-1f, 0, 0f);
                break;
            case k_Up:
                transform.localPosition = transform.localPosition + new Vector3(0f, 0, 1f);
                break;
            case k_Down:
                transform.localPosition = transform.localPosition + new Vector3(0f, 0, -1f);
                break;
        }

        // Rewards
        float distanceToTarget = Vector3.Distance(transform.localPosition, Target.localPosition);
        float distanceToObstacle = Vector3.Distance(transform.localPosition, Obstacle.localPosition);

        // Reached target
        if (distanceToTarget < 0.6f)
        {
            SetReward(1.0f);
            EndEpisode();
        }

        // Reached Obstacle
        if (distanceToObstacle < 0.6f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        // Fell off platform
        if (Mathf.Abs(transform.localPosition.x) > 2.1f || Mathf.Abs(transform.localPosition.z) > 2.1f)
        {
            SetReward(-1.0f);
            EndEpisode();
        }

        SetReward(-0.01f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = k_NoActions;
        if (Input.GetKey(KeyCode.RightArrow))
        {
            discreteActionsOut[0] = k_Right;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            discreteActionsOut[0] = k_Up;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            discreteActionsOut[0] = k_Left;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            discreteActionsOut[0] = k_Down;
        }
    }

}
