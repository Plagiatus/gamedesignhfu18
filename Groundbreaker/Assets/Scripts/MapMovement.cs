using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovement : MonoBehaviour {

	GameController GC;

	void Start () {
		try 
		{
			GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
			Debug.Log(GC);
		} catch { throw; }
	}

	void Update () {
		this.transform.position = Helper.TileToGamePosition(GC.CurrentGpsPosition.OsmOnTilePosition);
	}
}
