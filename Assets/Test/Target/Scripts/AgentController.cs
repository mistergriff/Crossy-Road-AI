using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.Collections.Generic;

public class AgentController : Agent
{
    [Header("Target variables")]
    [SerializeField] private Transform targetTransform;
    public int maxTarget;
    public GameObject food;
    [SerializeField] private List<GameObject> targetList = new List<GameObject>();

    [Header("Agent variables")]
    [SerializeField] private float moveSpeed = 4f;
    private Rigidbody rb;

    [Header("Environement variables")]
    [SerializeField] private Transform environementLocation;
    Material environementMaterial;
    [SerializeField] private GameObject env;

    [SerializeField] private int timeForEpisode = 30;
    private float timeLeft;

    // Utilisez un dictionnaire pour enregistrer la fréquence de visite des états
    private Dictionary<string, int> stateVisitCounts = new Dictionary<string, int>();


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        environementMaterial = env.GetComponent<MeshRenderer>().material;
    }

    private string GenerateStateKey(Vector3 position)
    {
        // Exemple simple : utiliser la position arrondie de l'agent comme clé d'état
        return $"{Mathf.Round(position.x)},{Mathf.Round(position.y)},{Mathf.Round(position.z)}";
    }


    public override void OnEpisodeBegin()
    {
        // Agent
        transform.localPosition = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
        transform.Rotate(0f, Random.Range(-180f, 180f), 0f, Space.Self);

        // Target
        CreateTarget();

        // Timer
        EpisodeTimerNew();

    }

    private void Update()
    {
        CheckRemainingTime();
    }

    //Instancie plusieurs cible
    private void CreateTarget()
    {
        if(targetList.Count != 0)
        {
            RemoveTarget();
        }


        for (int i = 0; i < maxTarget; i++)
        {
            int counter = 0;
            bool distanceGood;

            GameObject newTarget = Instantiate(food);
            newTarget.transform.parent = environementLocation;
            Vector3 TargetLocation = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));

            if (targetList.Count != 0)
            {
                for(int j = 0; j < targetList.Count; j++)
                {
                    if (counter < 10)
                    {
                        distanceGood = CheckOverlap(TargetLocation, targetList[j].transform.localPosition, 1f);
                        if (distanceGood == false)
                        {
                            TargetLocation = new Vector3(Random.Range(-4f, 4f), 0.3f, Random.Range(-4f, 4f));
                            j--;
                        }
                        counter++;
                    }
                    else
                    {
                        j = targetList.Count;
                    }
                }
            }

            newTarget.transform.localPosition = TargetLocation;
            targetList.Add(newTarget);
        }
    }

    private bool CheckOverlap(Vector3 TargetAvoidOverlaping, Vector3 alreadyExistingObjects, float minDistanceWanted)
    {
        float disatanceBetweenObjects = Vector3.Distance(TargetAvoidOverlaping, alreadyExistingObjects);
        if(minDistanceWanted <= disatanceBetweenObjects)
        {
            return true;
        }
        return false;
    }

    private void RemoveTarget()
    {
        foreach (GameObject target in targetList)
        {
            Destroy(target.gameObject);
        }
        targetList.Clear();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Observations : position de l'agent et de la balle
        sensor.AddObservation(transform.localPosition);
        //sensor.AddObservation(targetTransform.localPosition);
    }

    // Récompense l'agent pour l'exploration de nouveaux états
    private float CalculateExplorationReward(Vector3 currentState)
    {
        string stateKey = GenerateStateKey(currentState);

        // Vérifie si l'état a été visité et met à jour le dictionnaire
        if (!stateVisitCounts.ContainsKey(stateKey))
        {
            stateVisitCounts[stateKey] = 1; // Premier visite de cet état
            return 1.0f; // Récompense pour un nouvel état
        }
        else
        {
            stateVisitCounts[stateKey] += 1; // Incrémente le compteur de visite
            return 0.1f / stateVisitCounts[stateKey]; // Récompense décroissante pour les états répétés
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        
        // Mouvement de l'agent
        float moveRotate = actions.ContinuousActions[0];
        float moveForward = actions.ContinuousActions[1];
        
        rb.MovePosition(transform.position + transform.forward * moveForward * Time.deltaTime * moveSpeed);
        transform.Rotate(0f, moveRotate * moveSpeed, 0f, Space.Self);

        // Calcul et application de la récompense d'exploration
        Vector3 currentState = transform.position; // Exemple en utilisant la position comme état
        float explorationReward = CalculateExplorationReward(currentState);
        AddReward(explorationReward);


        /*
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];
        
        Vector3 velocity = new Vector3(moveX, 0f, moveZ);

        velocity = velocity.normalized * Time.deltaTime * moveSpeed;

        transform.localPosition += velocity;
        */

        //Malus pour la rapidité
        //float malusSpeed = -0.01f;
        //AddReward(malusSpeed);

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
            Debug.Log("<color=green>Target</color>");
            targetList.Remove(other.gameObject);
            Destroy(other.gameObject);
            AddReward(5f);
            if (targetList.Count == 0) 
            {
                environementMaterial.color = Color.green;
                RemoveTarget();
                AddReward(6f);
                EndEpisode();
            }
        }

        if (other.gameObject.tag == "Wall")
        {
            Debug.Log("<color=red>Wall</color>");
            environementMaterial.color = Color.red;
            RemoveTarget();
            AddReward(-10f);
            EndEpisode();
        }
    }

    private void EpisodeTimerNew()
    {
        timeLeft = Time.time + timeForEpisode;
    }

    private void CheckRemainingTime()
    {
        if (Time.time >= timeLeft)
        {
            Debug.Log("<color=red>Out of time</color>");
            environementMaterial.color = Color.blue;
            AddReward(-15f);
            RemoveTarget();
            EndEpisode();
        }
    }

}