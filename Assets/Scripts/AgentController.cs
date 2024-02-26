using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    [SerializeField] private Transform targetTransform;

    public override void OnEpisodeBegin()
    {
        transform.localPosition = new Vector3(0f, 0.3f, 0f);

        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            targetTransform.localPosition = new Vector3(-4f, 0.3f, 0f);
        }

        if (rand == 1)
        {
            targetTransform.localPosition = new Vector3(4f, 0.3f, 0f);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        float move = actions.ContinuousActions[0];
        float moveSpeed = 2f;

        transform.localPosition += new Vector3(move, 0f, 0f) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            AddReward(5f);
            EndEpisode();
        }

        if (other.gameObject.tag == "Wall")
        {
            AddReward(-2.5f);
            EndEpisode();
        }
    }
}
