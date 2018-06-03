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

	private int previousTileX;
	private int previousTileY;

	void Start () {

		children = new QuadPosRelationship[9];
		
		//get all different map tiles (quads) into the children list
		for (int i = 0; i < this.transform.childCount; i++)
		{
			GameObject child = this.transform.GetChild(i).gameObject;
			if(child.tag == "MapTile"){
				//save their position for reference later
				children[i] = new QuadPosRelationship() {x=Mathf.FloorToInt(child.transform.position.x / Config.TileSizeInGame), y= Mathf.FloorToInt(child.transform.position.z / Config.TileSizeInGame) * -1, quad = child};
			}
		}

		try 
		{
			GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		} catch
		{
			throw;
		}
	}

	void Update () {
		if (GC == null){
			GC = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		}
		//when moving onto a new tile, reload all tiles. should be called on the first frame as well.
		if (previousTileX != GC.CurrentGpsPosition.OsmTilePosition.x || previousTileY != GC.CurrentGpsPosition.OsmTilePosition.y)
		{
			LoadAllTiles();
			previousTileX = Mathf.FloorToInt(GC.CurrentGpsPosition.OsmTilePosition.x);
			previousTileY = Mathf.FloorToInt(GC.CurrentGpsPosition.OsmTilePosition.y);
		}
	}

	void LoadAllTiles(){
		for(int i = 0; i < children.Length; i++)
		{
			StartCoroutine(LoadSingleTile(children[i].quad, children[i].x, children[i].y));
		}
	}

	IEnumerator LoadSingleTile(GameObject child, int _x=0, int _y=0){
		//check if map is already in cache
		if(TileExistsInCache(_x, _y))
		{
			LoadMapFromCache(child, _x, _y);
		}
		else
		{
			//load map from openstreet map
			string url = Config.MapLoaderBaseUrl + GC.CurrentZoom + "/" + (GC.CurrentGpsPosition.OsmTilePosition.x +_x) + "/" + (GC.CurrentGpsPosition.OsmTilePosition.y +_y) + ".png";
			WWW www = new WWW(url);
			yield return www;


			Material mat = child.GetComponent<Renderer>().material;
			if(!string.IsNullOrEmpty(www.error)){
				Debug.Log(www.error);
				mat = ErrorMaterial;
			} else {
				//put the returned image onto the texture of the map tile
				Texture2D tex = new Texture2D(256, 256);
				www.LoadImageIntoTexture(tex);
				mat.mainTexture = tex;
				//save the tile to the filesystem so it doesn't need to be loaded from the internet again afterwards.
				SaveTileToCache(tex, _x, _y);
			}
		}
	}

	void SaveTileToCache(Texture2D tex, int _x, int _y)
	{
		try {
			//create /MapCache/ folder
			if (!Directory.Exists(Application.persistentDataPath + "/" + Config.MapCacheFolderName))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/" + Config.MapCacheFolderName);
			}

			//create Zoom Level subfolder
			if (!Directory.Exists(Application.persistentDataPath + "/" + Config.MapCacheFolderName + "/" + GC.CurrentZoom))
			{
				Directory.CreateDirectory(Application.persistentDataPath + "/" + Config.MapCacheFolderName + "/" + GC.CurrentZoom);
			}			

			//write the texture as a png file to the filesystem
			System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + Config.MapCacheFolderName + "/" + GC.CurrentZoom + "/" + (GC.CurrentGpsPosition.OsmTilePosition.x +_x) + "_" + (GC.CurrentGpsPosition.OsmTilePosition.y +_y) + ".png", tex.EncodeToPNG());

		} catch (IOException ex) {
			Debug.Log(ex.Message);
		}
	}

	bool TileExistsInCache(int _x, int _y){
		bool ret = File.Exists(Application.persistentDataPath + "/" + Config.MapCacheFolderName + "/" + GC.CurrentZoom + "/" + (GC.CurrentGpsPosition.OsmTilePosition.x +_x) + "_" + (GC.CurrentGpsPosition.OsmTilePosition.y +_y) + ".png");
		if (ret)
		{
			//check if file is too old
			FileInfo fi = new FileInfo(Application.persistentDataPath + "/" + Config.MapCacheFolderName + "/" + GC.CurrentZoom + "/" + (GC.CurrentGpsPosition.OsmTilePosition.x +_x) + "_" + (GC.CurrentGpsPosition.OsmTilePosition.y +_y) + ".png");
			System.DateTime renewDate = fi.LastWriteTime.AddDays(1);
			// Debug.Log("Comparison: " + renewDate.CompareTo(System.DateTime.Now));
			if(renewDate.CompareTo(System.DateTime.Now) < 0)
			{
				ret = false;
			}
		}
		return ret;
	}

	void LoadMapFromCache(GameObject child, int _x, int _y){
		//check again if file actually exists or not
		if(!File.Exists(Application.persistentDataPath + "/" + Config.MapCacheFolderName + "/" + GC.CurrentZoom + "/" + (GC.CurrentGpsPosition.OsmTilePosition.x +_x) + "_" + (GC.CurrentGpsPosition.OsmTilePosition.y +_y) + ".png"))
		{
			return;
		}

		Material mat = child.GetComponent<Renderer>().material;
		Texture2D tex = new Texture2D(256, 256);

		//read the bytearray of the file, load it as image and apply it to the texture
		byte[] bytes = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/" + Config.MapCacheFolderName + "/" + GC.CurrentZoom + "/" + (GC.CurrentGpsPosition.OsmTilePosition.x +_x) + "_" + (GC.CurrentGpsPosition.OsmTilePosition.y +_y) + ".png");
		tex.LoadImage(bytes);
		mat.mainTexture = tex;

	}
}
