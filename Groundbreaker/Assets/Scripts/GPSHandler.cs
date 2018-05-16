using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPSHandler : MonoBehaviour {

	private GameController GC;

	void Start()
	{
		try 
		{
			GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		} catch (Exception)
		{
			throw;
		}

		if (Config.UseDebugGpsPosition)
		{
			GC.CurrentGpsPosition = Config.DebugGpsPosition;
		}
		else
		{
			if(Input.location.isEnabledByUser)
			{
				Input.location.Start();
			}
		}

	}

}
