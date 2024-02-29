using UnityEngine;

public class BallController : MonoBehaviour
{
    public PongController pongController;
    public float initialSpeed = PongController.initialSpeed;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

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
            rb.velocity = newVelocity.normalized * initialSpeed; // Conserve la vitesse constante
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == pongController.winWall)
        {
            pongController.HitWinWall();
            Debug.Log("Trigger win");
        }
        if (other.gameObject == pongController.loseWall)
        {
            pongController.HitLoseWall();
            Debug.Log("Trigger Lose");
        }
    }

}
