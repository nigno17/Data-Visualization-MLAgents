using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class BallAgent : Agent
{
    Rigidbody myRigidBody;
    public GameObject targetGO;
    public float forceCoeff = 10;
    private bool isColliding;

    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody>();
        isColliding = false;
    }

    public override void OnEpisodeBegin()
    {
        if (transform.localPosition.y < 0)
        {
            myRigidBody.velocity = Vector3.zero;
            myRigidBody.angularVelocity = Vector3.zero;
            transform.localPosition = new Vector3(0f, 0.5f, 0f);
        }

        targetGO.transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.5f, Random.Range(-4f, 4f));
    }

    // public override void CollectObservations(VectorSensor sensor)
    // {
    //     sensor.AddObservation(transform.localPosition);
    //     sensor.AddObservation(targetGO.transform.localPosition);
    //     sensor.AddObservation(myRigidBody.velocity.x);
    //     sensor.AddObservation(myRigidBody.velocity.z);
    // }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Vector3 azioni = Vector3.zero;
        azioni.x = actions.ContinuousActions[0];
        azioni.z = actions.ContinuousActions[1];
        myRigidBody.AddForce(azioni * forceCoeff);

        if (isColliding)
        {
            isColliding = false;
            SetReward(1f);
            EndEpisode();
        }

        if (transform.localPosition.y < 0)
        {
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
        if (other.collider.CompareTag("targetLezione"))
        {
            isColliding = true;
        }
    }

}
