using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [Header("Score")]
    [SerializeField] private Text aiScoreText;
    [SerializeField] private Text playerScoreText;
    private int aiScore;
    private int playerScore;

    public GameController instance;

    private void Awake()
    {
        instance = this;
    }

}
