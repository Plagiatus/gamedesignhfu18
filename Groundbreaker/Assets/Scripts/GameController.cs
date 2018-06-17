using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public Helper.LocationData CurrentGpsPosition;
	public static GameController Instance;
	public int CurrentZoom = 17;
	public Helper.CircleOfAction circle = null;
	public List<Helper.PlayerLocation> playerPositions = new List<Helper.PlayerLocation>();
	public Helper.Player player = null;


	void Start () {
		DontDestroyOnLoad(gameObject);
		CurrentGpsPosition = new Helper.LocationData();
		Instance = this;
		circle = null;
		// player = new Helper.Player("Lukas", "1234567", 1);
	}

	// void OnLevelWasLoaded(){
	// 	Instance = this;
	// }

	void Update () {
		
	}


}
