using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

public class MyAgent : Agent
{
    private const float EPSILON = 0.3f;
    private const float SIZE = 5;
    private const float MAX_TIME = 5f;
    
    [SerializeField] private Transform target;

    private float currentTime;
    
    public override void OnEpisodeBegin()
    {
        currentTime = 0;
        transform.localPosition = new Vector3(Random.Range(-SIZE, SIZE), 0, Random.Range(-SIZE, SIZE));
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.z);
        
        sensor.AddObservation(target.localPosition.x);
        sensor.AddObservation(target.localPosition.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        float moveSpeed = 5f;
        
        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;

        Vector3 position = transform.localPosition + new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
        
        position.x = Mathf.Clamp(position.x, -SIZE, SIZE);
        position.z = Mathf.Clamp(position.z, -SIZE, SIZE);
        
        transform.localPosition = position;
        
        float distanceToTarget = Vector3.Distance(transform.localPosition, target.localPosition);
        if (distanceToTarget < EPSILON)
        {
            Debug.LogError("Done!");
            SetReward(1f);
            EndEpisode();
        }
        
        currentTime += Time.deltaTime;
        if (currentTime > MAX_TIME)
        {
            Debug.Log("Time's up!");
            SetReward(-1);
            EndEpisode();
        }
    }
}
