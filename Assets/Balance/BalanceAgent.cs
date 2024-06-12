using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace Balance
{
    public sealed class BalanceAgent : Agent
    {
        private const float FIELD_SIZE = 10;
        private const float MAX_TIME = 10f;
        private const float MOVE_SPEED = 20f;
        private const float MAX_ANGLE = 20f;
        private const float MIN_ANGLE = 10f;
        private const float SMALL_REWARD_STEP = 0.5f;


        [SerializeField] private Transform target;
        [SerializeField] private Transform targetPoint;

        private float currentTime;
        private float lastSecondRewarded;

        public override void OnEpisodeBegin()
        {
            lastSecondRewarded = SMALL_REWARD_STEP;
            currentTime = 0;
            transform.localPosition = Vector3.zero;
            target.localPosition = Vector3.zero;
            target.rotation = Quaternion.identity;
            var rigidbody = target.GetComponent<Rigidbody>();
            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;

            float rotation = Random.Range(MIN_ANGLE, MAX_ANGLE) * (Random.value > 0.5f ? 1 : -1);
            target.localRotation = Quaternion.Euler(0, 0, rotation);
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
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.localPosition.x);

            Vector3 relativePos = targetPoint.position - target.position;
            sensor.AddObservation(relativePos.x);
            sensor.AddObservation(relativePos.y);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            float moveX = actions.ContinuousActions[0];

            Vector3 position = transform.localPosition
                               + new Vector3(moveX, 0, 0) * Time.deltaTime * MOVE_SPEED;

            position.x = Mathf.Clamp(position.x, -FIELD_SIZE, FIELD_SIZE);

            transform.localPosition = position;

            if (targetPoint.position.y < 0)
            {
                Debug.Log("Fall!");
                SetReward(-10f);
                EndEpisode();
            }

            currentTime += Time.deltaTime;

            if (currentTime > lastSecondRewarded)
            {
                Debug.Log("Small Reward");
                lastSecondRewarded += SMALL_REWARD_STEP;
                SetReward(1f);
            }

            if (currentTime > MAX_TIME)
            {
                Debug.Log("Time's up!");
                SetReward(10);
                EndEpisode();
            }
        }
    }
}