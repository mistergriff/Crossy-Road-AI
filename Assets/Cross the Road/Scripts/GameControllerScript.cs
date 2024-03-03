using UnityEngine;
using UnityEngine.UI;

public class GameControllerScript : MonoBehaviour {

	public Camera playerCamera;						// Camera to keep track of current viewport.
	public GameObject player;						// Player object.
	public Vector3 startSpawnPosition;				// Where to spawn the first strip.
	public Transform[] meshPrefabs;					// Possible strips to spawn.
	public Transform safePrefab;					// Safe strip to spawn on.
	public int spawnOffset = 20;					// How far ahead should we spawn strips?
	public Transform coinPrefab;					// Prefab for a coin.
	public float spawnCoinChance = 0.1f;			// Chance to spawn a coin on a given strip.
	public int leftBound = -4;						// Left bound for spawning coins.
	public int rightBound = 5;						// Right bound for spawning coins.
	public Text scoreText;							// Text which displays the current score.
	public Text highScoreText;						// Text which displays the high score.
	public Text coinText;							// Text which displays the current coins.
	public AudioSource coinAudio;					// Audio to play when picking up a coin.
	public GameObject pauseMenu;					// The pause menu.
	public GameObject storeMenu;					// The store menu.
	public GameObject gameOverMenu;					// The game over menu.
	public Text gameOverText;						// Text which displays "Game Over" or "High Score!".
	public Color highScoreTextColor;				// Color to display high score text - replaces game over text color.
	public Text retryButtonText;					// Text which displays on the game over menu's "Retry" button.
	public Color[] storeItems;						// Possible items to purchase from the store.
	public Button[] storeButtons;					// Array of buttons in the store. Used for showing which items are already purchased.
	public Text storeText;							// Title of the store menu. Used to display when an action is taken in the store ("Not enough coins", etc.).
	public Color purchasedButtonColor;				// Color to display buttons which are already purchased.

	int score = 0;									// Current score.
	int highScore = 0;								// High score.
	int coins = 0;									// Total number of coins the player has.
	Vector3 curSpawnPosition;						// Current position to spawn strips.
	bool paused = false;							// Toggle the paused state of the game.

	const string HIGH_SCORE = "Player_High_Score";	// Key for storing high scores in PlayerPrefs.
	const string COINS = "Player_Coins";			// Key for storing coins in PlayerPrefs.
	const string PURCHASES = "Player_Purchase_";	// Key for storing an integers of in-store purchases.
	const string ITEM_INDEX = "Player_Item_Index";	// Key for which item is currently active from the store.

	void Start () {
		// Initalize some vars.
		curSpawnPosition = startSpawnPosition;
		highScore = PlayerPrefs.GetInt (HIGH_SCORE);
		coins = PlayerPrefs.GetInt (COINS);
		// The first item in the store is automatically free.
		PlayerPrefs.SetInt (PURCHASES + "0", 1);

		// Display high score and coins.
		InitGUI();

		// Ensure that the mesh prefabs have been set.
		if (meshPrefabs == null)
			Debug.LogError("Please add prefabs to the Mesh Prefabs variable (road, grass, train tracks, etc) to the GameController object.");

		// Spawn initial strips, and ensure the player starts on a safe area.
		for (int i = (int)startSpawnPosition.z; i<= 1; i++) {
			Instantiate (safePrefab, curSpawnPosition, Quaternion.identity);
			curSpawnPosition.z = curSpawnPosition.z + 1;
		}
	}

	void Update () {
		// If we need to spawn another strip, do so.
		while (SpawnStripNeeded()) {
			// TODO Change spawn patterns based on current distance to add difficulty.
			int rand = Random.Range (0, meshPrefabs.Length);
			ObjectSpawnerScript[] objectSpawnerScripts = ((Instantiate (meshPrefabs[rand], curSpawnPosition, Quaternion.identity) as Transform)
			                                           .gameObject.GetComponentsInChildren<ObjectSpawnerScript>() as ObjectSpawnerScript[]);
			try {
				Destroy(objectSpawnerScripts[Random.Range (0, objectSpawnerScripts.Length)].gameObject);
			} catch (System.IndexOutOfRangeException) {
				// Do nothing. This only means a strip without any ObjectSpawnerScripts was spawned, such as a safe strip.
			}

			// Should we spawn a coin on this strip?
			if (Random.Range (0f, 1f) < spawnCoinChance) {
				SpawnCoin();
			}
			// Move to the next position.
			curSpawnPosition.z = curSpawnPosition.z + 1;
		}
	}

	/// <summary>
	/// Spawns a coin on the current strip.
	/// </summary>
	void SpawnCoin () {
		Vector3 coinLocation = curSpawnPosition;
		coinLocation.x = (float) Random.Range (leftBound, rightBound+1);
		Instantiate (coinPrefab, coinLocation, Quaternion.identity);
	}

	/// <summary>
	/// Do we need to spawn another strip?
	/// </summary>
	/// <returns><c>true</c>, if a strip is needed, <c>false</c> otherwise.</returns>
	bool SpawnStripNeeded () {
		return playerCamera.transform.position.z + spawnOffset > curSpawnPosition.z;
	}

	/// <summary>
	/// Sets the player's score to the provided value.
	/// </summary>
	/// <param name="score">Score.</param>
	public void SetScore (int score) {
		if (score > this.score) {
			this.score = score;
			UpdateScoreText();
		}
	}

	/// <summary>
	/// Plays the coin audio, and increments the coin count.
	/// </summary>
	public void CollectCoin () {
		coinAudio.Play ();
		coins += 1;
		UpdateCoinText();
	}

	/// <summary>
	/// Updates the score text.
	/// </summary>
	void UpdateScoreText () {
		scoreText.text = string.Format ("{0:000}", score);
		// If this is a new high score, update the high score text as well.
		if (score > highScore) {
			highScore = score;
			UpdateHighScoreText();
		}
	}

	/// <summary>
	/// Updates the coin text.
	/// </summary>
	void UpdateCoinText () {
		coinText.text = string.Format ("COINS: {0:000}", coins);
	}

	/// <summary>
	/// Updates the high score text.
	/// </summary>
	void UpdateHighScoreText () {
		highScoreText.text = string.Format ("HIGH SCORE: {0:000}", highScore);
	}

	/// <summary>
	/// Initalizes the GUI by setting high score and coin values.
	/// </summary>
	void InitGUI () {
		UpdateCoinText();
		UpdateHighScoreText();
	}

	/// <summary>
	/// Pauses the game, and displays the popup menu.
	/// </summary>
	public void Pause () {
		if (paused) {
			Time.timeScale = 1.0f;
			paused = false;
			pauseMenu.SetActive(false);
		} else {
			Time.timeScale = 0f;
			paused = true;
			pauseMenu.SetActive(true);
		}
	}

	/// <summary>
	/// Determines whether the game is paused.
	/// </summary>
	/// <returns><c>true</c> if the game is paused; otherwise, <c>false</c>.</returns>
	public bool IsPaused () {
		return paused;
	}

	/// <summary>
	/// Opens the store.
	/// </summary>
	public void OpenStore () {
		// Iterate through the items, and show which items are already purchased.
		UpdateStore();
		// Show the store.
		storeMenu.SetActive(true);
	}

	/// <summary>
	/// Updates the store to show purchased items as purchased.
	/// </summary>
	public void UpdateStore () {
		for (int i = 0; i < storeButtons.Length; i++) {
			int isPurchased = PlayerPrefs.GetInt (PURCHASES + i, defaultValue:-1);
			// If the item is already purchased...
			if (isPurchased == 1) {
				// Change the color.
				storeButtons[i].image.color = purchasedButtonColor;
				// Change the text.
				((storeButtons[i].gameObject as GameObject)
				 .GetComponentsInChildren<Text>(true)[0] as Text).text = "Already Purchased";
			}
		}
	}

	/// <summary>
	/// Closes the store.
	/// </summary>
	public void CloseStore () {
		storeMenu.SetActive(false);
	}

	/// <summary>
	/// Opens the options menu.
	/// </summary>
	public void OpenOptions () {
		// TODO Implement options menu.
		Debug.Log ("The OpenOptions() menu in the script GameControllerScript currently does nothing. Implement your options menu there.");
	}

	
	/// <summary>
	/// Plays a death animation, and loads the GameOver menu.
	/// </summary>
	public void GameOver () {
		if (score >= highScore) {
			// Let the player know that a high score was achieved.
			gameOverText.text = "New High Score!! Play again?";
			gameOverText.color = highScoreTextColor;
			retryButtonText.text = "Play Again";
		}
		// TODO Play death animation.
		// Prevent player movement.
		player.GetComponent<PlayerController>().enabled = false;
		// Keep the player from getting pushed.
		player.GetComponent<Rigidbody>().isKinematic = true;
		// Load the menu.
		gameOverMenu.SetActive (true);
	}

	/// <summary>
	/// Quits the game after saving coins, high scores, etc..
	/// </summary>
	public void QuitGame () {
		// Save high score to PlayerPrefs if a new high score was achieved.
		if (score >= highScore) {
			PlayerPrefs.SetInt (HIGH_SCORE, score);
		}
		// Update the player's amount of coins in PlayerPrefs.
		PlayerPrefs.SetInt (COINS, coins);
		// Quit the game.
		Application.Quit();
	}

	/// <summary>
	/// Restarts the game after saving coins, high scores, etc..
	/// </summary>
	public void RestartGame () {
		// Save high score to PlayerPrefs if a new high score was achieved.
		if (score >= highScore) {
			PlayerPrefs.SetInt (HIGH_SCORE, score);
		}
		// Update the player's amount of coins in PlayerPrefs.
		PlayerPrefs.SetInt (COINS, coins);
		// Quit the game.
		Application.LoadLevel (Application.loadedLevel);
	}

	/// <summary>
	/// Attempts the purchase the selected item from the store.
	/// The string should be formatted as "index,price".
	/// </summary>
	/// <param name="str">String of values to pass.</param>
	public void AttemptPurchase (string str) {
		string[] vals = str.Split (new char[] {','});
		int index = int.Parse (vals[0]);
		int price = int.Parse (vals[1]);
		int isPurchased = PlayerPrefs.GetInt (PURCHASES + index, defaultValue:-1);
		// If it is not already purchased, and the player has enough coins, buy it.
		if (isPurchased == -1) {
			if (coins >= price) {
				PlayerPrefs.SetInt (ITEM_INDEX, index);
				PlayerPrefs.SetInt (PURCHASES + index, 1);
				PlayerPrefs.SetInt (COINS, (coins -= price));
				UpdateStore();
				storeText.text = string.Format ("Item #{0} Purchased", index+1);
				UpdateCoinText();
				return;
			}
			// Not enough coins.
			else {
				storeText.text = "Not Enough Coins...";
				return;
			}
		}
		// Already purhcased.
		else {
			storeText.text = string.Format ("Item #{0} Active", index+1);
			PlayerPrefs.SetInt (ITEM_INDEX, index);
		}
	}

	/// <summary>
	/// Gets the color of the player.
	/// </summary>
	/// <returns>The player color.</returns>
	public Color GetPlayerColor () {
		return storeItems[PlayerPrefs.GetInt (ITEM_INDEX, defaultValue:0)];
	}
}
