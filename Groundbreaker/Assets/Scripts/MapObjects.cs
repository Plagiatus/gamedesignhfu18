using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObjects : MonoBehaviour {

	public GameObject PlayerPrefab;
	public GameObject MiddleTile;
	public GameObject circleInGame;
	private int previousTileX = 0;
	private int previousTileY = 0;
	void Start () {
	}
	
	void Update () {
		if(GameController.Instance.circle != null && (previousTileX != GameController.Instance.CurrentGpsPosition.OsmTilePosition.x || previousTileY != GameController.Instance.CurrentGpsPosition.OsmTilePosition.y))
		{
			previousTileX = Mathf.FloorToInt(GameController.Instance.CurrentGpsPosition.OsmTilePosition.x);
			previousTileY = Mathf.FloorToInt(GameController.Instance.CurrentGpsPosition.OsmTilePosition.y);
			Vector3 circlePosition = Helper.LocationToGamePosition(GameController.Instance.circle.center, GameController.Instance);
			circleInGame.transform.localPosition = circlePosition;
			circleInGame.GetComponent<PointController>().createPoints();
			// Debug.Log("Position Circle, "+ GameController.Instance.circle.center);
		}
	}

	public void positionPlayers(List<Helper.PlayerLocation> playerLocations){
		for (int i = 0; i < this.transform.childCount; i++){
			if(this.transform.GetChild(i).tag == "otherPlayer"){
				Destroy(this.transform.GetChild(i).gameObject);
			}
		}

		foreach(Helper.PlayerLocation p in playerLocations){
			if(p.id != GameController.Instance.player.id){
				GameObject newPlayer = GameObject.Instantiate(PlayerPrefab);
				newPlayer.transform.position = Helper.LocationToGamePosition(p.position, GameController.Instance) + MiddleTile.transform.position;
				newPlayer.tag = "otherPlayer";
				newPlayer.GetComponent<PlayerBehaviour>().addName(p.name);
				newPlayer.transform.SetParent(this.transform);
			}
		}
	}
}
