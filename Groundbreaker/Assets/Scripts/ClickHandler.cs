using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour {

	GameObject activePoint = null;

	void Start () {
		
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(0)){ 
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit)){
				if(hit.transform.tag == "POA"){
					if(activePoint == null){
						activePoint = hit.transform.gameObject;
						Camera.main.GetComponent<CameraMovement>().lookAtObject(activePoint);
						Config.attackedPoint = activePoint.GetComponent<PointBehaviour>().point;
						CanvasBehaviour.Instance.redOverlayVisibility(true);
					} else if(activePoint.Equals(hit.transform.gameObject)){
						activePoint = null;
						Camera.main.GetComponent<CameraMovement>().resetView();
						Config.attackedPoint = null;
						CanvasBehaviour.Instance.redOverlayVisibility(false);
					}
				}
			}
		}
	}
}
