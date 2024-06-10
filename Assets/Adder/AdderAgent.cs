using System;
using Unity.MLAgents;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Adder
{
    public sealed class AdderAgent : Agent
    {
        private float num1;
        private float num2;

        private Material material;

        private void Awake()
        {
            var meshRenderer = GetComponent<MeshRenderer>();
            material = meshRenderer.material;
        }

        public override void OnEpisodeBegin()
        {
            num1 = Random.Range(0, 0.5f);
            num2 = Random.Range(0, 0.5f);
        }

        public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
        {
            sensor.AddObservation(num1);
            sensor.AddObservation(num2);
        }

        public override void OnActionReceived(Unity.MLAgents.Actuators.ActionBuffers actions)
        {
            var action = actions.ContinuousActions[0];
            float sum = num1 + num2;
            float dif = Mathf.Abs(sum - action);
            float reward = 1 - dif;
            AddReward(reward);
            if (dif < 0.025f)
            {
                Debug.Log($"Correct sum: {num1:F2} + {num2:F2} = {sum:F2}");
            }
            else
            {
                Debug.LogWarning("Wrong.");
            }

            material.color = Color.Lerp(
                Color.red,
                Color.green,
                reward);
            EndEpisode();
        }

        public override void Heuristic(in Unity.MLAgents.Actuators.ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            continuousActionsOut[0] = num1 + num2;
        }
    }
}