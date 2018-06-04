using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public Helper.LocationData CurrentGpsPosition;
	public int CurrentZoom = 17;
	public Helper.CircleOfAction circle = null;
	public IDictionary playerPositions = new Dictionary<string, Vector2>();

	void Start () {
		CurrentGpsPosition = new Helper.LocationData();
	}

	void Update () {
		
	}
}
