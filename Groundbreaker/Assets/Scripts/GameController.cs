using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

	public Helper.LocationData CurrentGpsPosition;
	public int CurrentZoom = 17; 

	void Start () {
		CurrentGpsPosition = new Helper.LocationData();
	}

	void Update () {
		
	}
}
