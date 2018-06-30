using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
	Vector3 originalPos;
	Vector3 targetPos;
	Quaternion originalRot;
	Quaternion targetRot;
	public float speed = 1;

	void Start () {
		originalPos = transform.position;
		originalRot = transform.rotation;
		resetView();
	}

	void Update () {
		transform.rotation = Quaternion.Slerp(transform.rotation, originalRot, Time.deltaTime * speed);
		transform.position = Vector3.Slerp(transform.position, targetPos, Time.deltaTime * speed);
	}

	public void lookAtObject(GameObject obj){
		targetPos = obj.transform.position + new Vector3(0, 10, -8);
	}

	public void resetView(){
		targetPos = originalPos;
		targetRot = originalRot;
	}
}
