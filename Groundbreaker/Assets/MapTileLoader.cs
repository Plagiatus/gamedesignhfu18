using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public struct QuadPosRelationship{
	public GameObject quad;
	public int x;
	public int y;
}

public class MapTileLoader : MonoBehaviour {

	GameController GC;
	public Material ErrorMaterial;


	public QuadPosRelationship[] children;

	void Start () {

		children = new QuadPosRelationship[9];
		
		for (int i = 0; i < this.transform.childCount; i++)
		{
			GameObject child = this.transform.GetChild(i).gameObject;
			children[i] = new QuadPosRelationship() {x=Mathf.FloorToInt(child.transform.position.x / Config.TileSizeInGame), y= Mathf.FloorToInt(child.transform.position.z / Config.TileSizeInGame) * -1, quad = child};
		}

		try 
		{
			GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		} catch
		{
			throw;
		}
		StartCoroutine(LoadAllTiles());
		// SaveTileToCache();
	}

	void Update () {
		//when moving onto a new tile, reload all tiles
	}

	IEnumerator LoadAllTiles(){
		yield return new WaitForSeconds(1);
		
		for(int i = 0; i < children.Length; i++)
		{
			StartCoroutine(LoadSingleTile(children[i].quad, children[i].x, children[i].y));
		}

	}

	IEnumerator LoadSingleTile(GameObject child, int _x=0, int _y=0){
		//TODO: check if map is already in cache

		string url = Config.MapLoaderBaseUrl + GC.CurrentZoom + "/" + (GC.CurrentGpsPosition.OsmTilePosition.x +_x) + "/" + (GC.CurrentGpsPosition.OsmTilePosition.y +_y) + ".png";
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
			SaveTileToCache();
		}
	}

	public static void SaveTileToCache()
	{
		try {
			if (!Directory.Exists(Application.persistentDataPath + "/" + Config.MapCacheFolderName))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/" + Config.MapCacheFolderName);
			}
		} catch (IOException ex) {
			Debug.Log(ex.Message);
		}
	}
}
