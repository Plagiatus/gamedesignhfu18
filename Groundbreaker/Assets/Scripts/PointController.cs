using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointController : MonoBehaviour {

	Helper.CircleOfAction circle;
	public GameObject PointPrefab;
	public GameObject MiddleTile;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(GameController.Instance.circle != null && circle != GameController.Instance.circle){
			circle = GameController.Instance.circle;
			// createPoints();
		}
	}

	public void createPoints(){
		circle = GameController.Instance.circle;
		for(int i = 0; i < this.transform.childCount; i++){
			if(this.transform.GetChild(i).tag == "POA")
				Destroy(this.transform.GetChild(i).gameObject);
		}
		foreach(Helper.PointOfAction p in circle.pointsOfAction){
			Vector3 newPos = Helper.LocationToGamePosition(p.position, GameController.Instance) + MiddleTile.transform.position;
			GameObject newP = GameObject.Instantiate(PointPrefab);
			newP.transform.position = newPos;
			newP.transform.SetParent(this.transform);
			newP.GetComponent<PointBehaviour>().setPoint(p);
		}
	}
}
