using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public float levelStartDelay = 2f;
	public float turnDelay = .1f;
	public static GameManager instance = null;

	public BoardManager boardScript;
	public int playerFoodPoints = 100;
	[HideInInspector] public bool playersTurn = true;

	private Text levelText;
	private GameObject levelImage;
	private int level = 0;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup;

	void Awake () {
//		if (instance == null) { 
//			instance = this;
//		} else if (instance != this) {
//			Destroy(gameObject);
//		}
//
		if (instance == null)

			//if not, set instance to this
			instance = this;

		//If instance already exists and it's not this:
		else if (instance != this)

			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy(gameObject);    



		DontDestroyOnLoad(gameObject);

		enemies = new List<Enemy> ();

		boardScript = GetComponent<BoardManager> ();

		Debug.Log ($"Level orginally at: {level}");
		// InitGame ();
		// ^ tutorial had this in there, but it caused it to increment level incorrectly
		// (started on level 2 all the time) 
	}

	void OnLevelFinishedLoading(Scene scene, LoadSceneMode
		mode)
	{
		//Add one to our level number.
		Debug.Log ($"Level was: {level}");
		level++;
		Debug.Log ($"Level now: {level}");
		//Call InitGame to initialize our level.
		InitGame();
	}

	void OnEnable()
	{
		//Tell our ‘OnLevelFinishedLoading’ function to start listening for a scene change event as soon as this script is enabled.
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	void OnDisable()
	{
		//Tell our ‘OnLevelFinishedLoading’ function to stop listening for a scene change event as soon as this script is disabled.
		//Remember to always have an unsubscription for everydelegate you subscribe to!
		SceneManager.sceneLoaded -= OnLevelFinishedLoading;
	}

	void InitGame()
	{
		doingSetup = true;

		levelImage = GameObject.Find ("LevelImage");
		levelText = GameObject.Find ("LevelText").GetComponent<Text> ();
		levelText.text = "Day " + level;
		levelImage.SetActive (true);
		Invoke ("HideLevelImage", levelStartDelay);

		enemies.Clear ();

		boardScript.SetupScene (level);
	}

	private void HideLevelImage()
	{
		levelImage.SetActive(false);
		doingSetup = false;
	}

	public void GameOver() {
		levelText.text = "After " + level + " days, you starved";
		enabled = false;
	}
		
	void Update () {
		if (playersTurn || enemiesMoving || doingSetup) {
			return;
		}

		StartCoroutine (MoveEnemies());

	}

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add (script);
	}

	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds (turnDelay);
		if (enemies.Count == 0) {
			yield return new WaitForSeconds (turnDelay);
		}
		for (int i = 0; i < enemies.Count; i++) {
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds (enemies[i].moveTime);
		}
		playersTurn = true;
		enemiesMoving = false;
			
	}
}
