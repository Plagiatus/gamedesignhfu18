using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMovement : MonoBehaviour {

	GameController GC;

	void Start () {
	}

	void Update () {
		this.transform.position = Helper.TileToGamePosition(GameController.Instance.CurrentGpsPosition.OsmOnTilePosition);
	}
}
