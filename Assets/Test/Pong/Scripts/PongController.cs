using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PongController : Agent
{
    [Header("Ball")]
    [SerializeField] private GameObject ball;
    static public float initialSpeed = 6;
    private Rigidbody ballRb;

    [Header("Agent")]
    [SerializeField] private float moveSpeed = 4f;

    [Header("Environement")]
    [SerializeField] public GameObject winWall;
    [SerializeField] public GameObject loseWall;
    private float topBoundary = 4f;
    private float bottomBoundary = -4f;

    public override void Initialize()
    {
        // Initialisation
        ballRb = ball.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Réinitialisation de la position de la balle et de la raquette
        transform.position = new Vector3(-9f, 0f, 0f);
        ball.transform.position = Vector3.zero;

        // Donne à la balle une vitesse initiale dans une direction aléatoire
        Vector2 initialVelocity = Random.insideUnitCircle.normalized * initialSpeed;
        ballRb.velocity = initialVelocity;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Ajouter la position de la balle
        sensor.AddObservation(ball.transform.localPosition);
        // Ajouter la vitesse de la balle
        sensor.AddObservation(ballRb.velocity);
        // Ajouter la position de la raquette
        sensor.AddObservation(transform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions pour déplacer la raquette
        float moveY = actionBuffers.ContinuousActions[0];
        transform.localPosition += new Vector3(0, moveY, 0) * Time.deltaTime * moveSpeed;

        // Appliquer le mouvement ici, puis vérifier les limites
        Vector3 newPosition = transform.position; // Ou calculer la nouvelle position basée sur l'action

        // Clamper la position de l'agent pour ne pas dépasser les limites supérieures et inférieures
        newPosition.y = Mathf.Clamp(newPosition.y, bottomBoundary, topBoundary);

        // Appliquer la position clamper
        transform.position = newPosition;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Commandes manuelles pour tester
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            AddReward(5f);
        }
    }

    public void HitWinWall()
    {
        AddReward(15f);
        EndEpisode();
    }

    public void HitLoseWall()
    {
        AddReward(-10f);
        EndEpisode();
    }
}
