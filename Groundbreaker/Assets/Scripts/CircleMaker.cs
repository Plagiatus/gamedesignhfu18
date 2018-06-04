using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleMaker : MonoBehaviour {

	[Range(0,100)]
    public int segments = 50;
	float radius;
    LineRenderer line;
	bool drawn = false;
	GameController GC;
	void Start () {
		line = gameObject.GetComponent<LineRenderer>();

		line.positionCount = segments + 1;
		line.useWorldSpace = false;
		line.loop = true;

		GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!drawn && GC.circle != null)
		{
			DrawCircle();
			drawn = true;
		}
	}

	public void DrawCircle(){
		// Debug.Log((GC.circle.radius / Config.TileSizeInRL) * Config.TileSizeInGame);
		float radius = (GC.circle.radius / Config.TileSizeInRL) * Config.TileSizeInGame;
		float x, y, z;
		float angle = 20f;
		y = 0.1f;

		for(int i = 0; i < (segments + 1); i++){
			x=Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
			z=Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
			line.SetPosition (i, new Vector3(x, y, z));

			angle += (360f / segments);
		}
	}
}
