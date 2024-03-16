using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace FindTarget
{
    public sealed class FindTargetAgent : Agent
    {
        private const float EPSILON = 0.3f;
        private const float FIELD_SIZE = 5;
        private const float MAX_TIME = 10f;
        private const float MOVE_SPEED = 5f;

        [SerializeField] private Transform target;

        private float currentTime;

        public override void OnEpisodeBegin()
        {
            currentTime = 0;
            transform.localPosition = GetRandomPosition();
            target.localPosition = GetRandomPosition();
        }

        private static Vector3 GetRandomPosition() 
            => new(
                Random.Range(-FIELD_SIZE, FIELD_SIZE), 
                0,
                Random.Range(-FIELD_SIZE, FIELD_SIZE));

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
            
            Vector3 position = transform.localPosition 
                               + new Vector3(moveX, 0, moveZ) * Time.deltaTime * MOVE_SPEED;

            position.x = Mathf.Clamp(position.x, -FIELD_SIZE, FIELD_SIZE);
            position.z = Mathf.Clamp(position.z, -FIELD_SIZE, FIELD_SIZE);

            transform.localPosition = position;

            float distanceToTarget = Vector3.Distance(position, target.localPosition);
            if (distanceToTarget < EPSILON)
            {
                Debug.Log("Done!");
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
}