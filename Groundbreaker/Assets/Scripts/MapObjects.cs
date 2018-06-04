using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjects : MonoBehaviour {

	GameObject circleInGame;
	GameController GC;
	private int previousTileX;
	private int previousTileY;
	void Start () {
		try 
		{
			GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			circleInGame = GameObject.Find("Circle");
		} catch
		{
			throw;
		}
	}
	
	void Update () {
		if(GC.circle != null && (previousTileX != GC.CurrentGpsPosition.OsmTilePosition.x || previousTileY != GC.CurrentGpsPosition.OsmTilePosition.y))
		{
			previousTileX = Mathf.FloorToInt(GC.CurrentGpsPosition.OsmTilePosition.x);
			previousTileY = Mathf.FloorToInt(GC.CurrentGpsPosition.OsmTilePosition.y);
			Vector3 circlePosition = Helper.LocationToGamePosition(GC.circle.center, GC);
			circleInGame.transform.localPosition = circlePosition;
		}	
	}
}
