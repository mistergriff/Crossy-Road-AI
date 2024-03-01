using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.UI;

public class PongController : Agent
{
    [Header("Ball")]
    [SerializeField] private GameObject ball;
    static public float initialSpeed = 8f;
    private Rigidbody ballRb;

    [Header("Agent")]
    [SerializeField] private int id;
    [SerializeField] private float moveSpeed = 4f;
    private Vector3 agentLocalPosition;

    [Header("Ennemy")]
    [SerializeField] private GameObject Ennemy;
    [SerializeField] private Transform ennemyTransform;

    [Header("Environement")]
    [SerializeField] public GameObject winWall;
    [SerializeField] public GameObject loseWall;
    private float topBoundary = 4f;
    private float bottomBoundary = -4f;

    [Header("Score")]
    [SerializeField] private Text aiScoreText;
    [SerializeField] private Text playerScoreText;
    private int aiScore;
    private int playerScore;

    public override void Initialize()
    {
        // Initialisation
        ballRb = ball.GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // Réinitialisation de la position de la balle, de la raquette et de l'ennemi
        if (id == 1)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0f, 0f);
            ennemyTransform.localPosition = new Vector3(ennemyTransform.localPosition.x, 0f, 0f);
        }
        ball.transform.localPosition = Vector3.zero;


        // Donne à la balle une vitesse initiale dans une direction aléatoire
        float yOffset = Random.Range(-0.5f, 0.5f);

        int direction = Random.Range(0, 1);

        if (direction == 0)
        {
            Vector2 initialVelocity = new Vector2(-1, yOffset).normalized * initialSpeed;
            ballRb.velocity = initialVelocity;
        }
        if (direction == 1)
        {
            Vector2 initialVelocity = new Vector2(1, yOffset).normalized * initialSpeed;
            ballRb.velocity = initialVelocity;
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Ajouter la position de la balle
        sensor.AddObservation(ball.transform.localPosition);
        // Ajouter la vitesse de la balle
        sensor.AddObservation(ballRb.velocity);
        // Ajouter la position de la raquette
        sensor.AddObservation(transform.localPosition);
        // Ajouter la position de l'ennemi
        sensor.AddObservation(Ennemy.transform.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions pour déplacer la raquette
        float moveY = actionBuffers.ContinuousActions[0];
        transform.localPosition += new Vector3(0, moveY, 0) * Time.deltaTime * moveSpeed;

        // Appliquer le mouvement ici, puis vérifier les limites
        Vector3 newPosition = transform.localPosition; // Ou calculer la nouvelle position basée sur l'action

        
        // Clamper la position de l'agent pour ne pas dépasser les limites supérieures et inférieures
        newPosition.y = Mathf.Clamp(newPosition.y, bottomBoundary, topBoundary);

        // Appliquer la position clamper
        transform.localPosition = newPosition;
        
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
            AddReward(1f);
        }
    }

    public void Hitwall(GameObject wall)
    {
        if (id == 1)
        {
            bool point = false;
            if (wall.CompareTag("RightWall"))
            {
                AddReward(10f);
                point = true;
            }
            if (wall.CompareTag("LeftWall"))
            {
                AddReward(-10f);
                point = false;
            }
            if (point == true)
            {
                aiScore++;
                aiScoreText.text = aiScore.ToString();
            }
            if (point == false)
            {
                playerScore++;
                playerScoreText.text = playerScore.ToString();
            }
        }
        if (id == 2)
        {
            return;
        }
        EndEpisode();
    }
}
