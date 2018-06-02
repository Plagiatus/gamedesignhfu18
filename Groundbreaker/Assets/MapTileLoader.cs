using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTileLoader : MonoBehaviour {

	GameController GC;
	public Material ErrorMaterial;

	public GameObject child;

	void Start () {
		try 
		{
			GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		} catch
		{
			throw;
		}
		StartCoroutine(LoadSingleTile(child));
	}

	void Update () {
		//when moving onto a new tile, reload all tiles
	}

	IEnumerator LoadAllTiles(){

		yield break;
	}

	IEnumerator LoadSingleTile(GameObject child, int _x=0, int _y=0){
		yield return new WaitForSeconds(1);
		string url = Config.MapLoaderBaseUrl + GC.CurrentZoom + "/" + (GC.CurrentGpsPosition.OsmTilePosition.x +_x) + "/" + (GC.CurrentGpsPosition.OsmTilePosition.y +_y) + ".png";
		Debug.Log(url);
		WWW www = new WWW(url);
		yield return www;


		Material mat = child.GetComponent<Renderer>().material;
		if(!string.IsNullOrEmpty(www.error)){
			Debug.Log(www.error);
			mat = ErrorMaterial;
		} else {
			Texture2D tex = new Texture2D(4, 4, TextureFormat.DXT1,false);
			www.LoadImageIntoTexture(tex);
			mat.mainTexture = tex;
		}
	}
}
