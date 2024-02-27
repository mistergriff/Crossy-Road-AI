using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    [SerializeField] private Transform targetTransform;
    [SerializeField] private float moveSpeed = 4f;

    public override void OnEpisodeBegin()
    {
        // Agent
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));

        //Target
        targetTransform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations : position de l'agent et de la balle
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(targetTransform.localPosition);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Mouvement de l'agent
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        Vector3 velocity = new Vector3(moveX, 0f, moveZ);

        velocity = velocity.normalized * Time.deltaTime * moveSpeed;

        transform.localPosition += velocity;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Contrôle manuel de l'agent
        ActionSegment<float> continuousAction = actionsOut.ContinuousActions;
        continuousAction[0] = Input.GetAxisRaw("Horizontal");
        continuousAction[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            AddReward(2f);
            Debug.Log("<color=green>Target</color>");
            EndEpisode();
        }

        if (other.gameObject.tag == "Wall")
        {
            AddReward(-1f);
            Debug.Log("<color=red>Wall</color>");
            EndEpisode();
        }
    }
}
