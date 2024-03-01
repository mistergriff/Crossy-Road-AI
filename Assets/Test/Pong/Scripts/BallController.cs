using UnityEngine;
using Unity.MLAgents;

public class BallController : MonoBehaviour
{
    public PongController pongController;
    public GameObject agent1;
    private float initialSpeed = PongController.initialSpeed;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    [SerializeField]
    private float speedIncreaseFactor = 1.05f; // Facteur d'augmentation de la vitesse après chaque touche

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Exemple : ajuste la direction de la balle basé sur le point de collision
            Vector3 hitPoint = collision.contacts[0].point;
            Vector3 paddleCenter = collision.gameObject.transform.position;

            // Calcule la différence en y entre le centre de la raquette et le point de collision
            float differenceY = hitPoint.y - paddleCenter.y;

            // Ajuste la direction y de la vélocité de la balle basée sur cette différence
            Vector3 newVelocity = rb.velocity;
            newVelocity.y += differenceY * 5; // Multiplie par un facteur pour augmenter l'effet

            // Augmente la vitesse de la balle
            newVelocity *= speedIncreaseFactor;

            // Applique la nouvelle vélocité à la balle
            rb.velocity = newVelocity;

            // Optionnel : vous pouvez limiter la vitesse maximale pour éviter que la balle ne devienne trop rapide à gérer
            float maxSpeed = 20f; // Définissez une vitesse maximale appropriée
            if (rb.velocity.magnitude > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LeftWall"))
        {
            agent1.GetComponent<PongController>().Hitwall(other.gameObject);
            Debug.Log(other.gameObject.tag);
        }
        if (other.gameObject.CompareTag("RightWall"))
        {
            agent1.GetComponent<PongController>().Hitwall(other.gameObject);
            Debug.Log(other.gameObject.tag);
        }
    }

}
