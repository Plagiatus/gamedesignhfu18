using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRotation : MonoBehaviour {

	GameController GC;

	void Start () {
	}
	

	void Update () {
		transform.rotation = Quaternion.Euler(0, -GameController.Instance.CurrentGpsPosition.HeadingDirection, 0);
	}
}
