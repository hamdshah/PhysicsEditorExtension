using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class PolygonPath {
	public Vector2[] points;
}

[Serializable]
public class PolygonObject : IComparable<PolygonObject>{
	[SerializeField]
	public string bodyname;
	public PolygonPath[] paths;

	public int TotalPaths {
		get{
			return paths.Length;
		}
	}

	/*Comparing Objects for sorting*/

	public int CompareTo(PolygonObject that) {
		return this.bodyname.CompareTo(that.bodyname);
	}
}

[Serializable]
[ExecuteInEditMode]
[RequireComponent(typeof(PolygonCollider2D))]
public class Polygon : MonoBehaviour {
	
	private PolygonObject _polygonObject;
	
	public int selectedIndex = 0;

	public PolygonCollider2D _polygonCollider = null;

	[SerializeField]
	public TextAsset PlistPath = null;
	public float PixelsPerUnit = 40.0f;
	public PolygonObject[] _totalPolygonsinFile;

	
	void Awake() {
		if(PlistPath != null && _totalPolygonsinFile.Length <= 0) {
			this.ParsePolygonsFromFile();
		}
	}

	// Use this for initialization
	void Start () {

		if (_polygonCollider == null)
			_polygonCollider = this.GetComponent<PolygonCollider2D>();

		//this.ClearBodiesList();

		_polygonObject = new PolygonObject();
	}

	public void ClearBodiesList() {
		_totalPolygonsinFile = new PolygonObject[0];
	}

	public void ParsePolygonsFromFile () {

		this.ClearBodiesList();
		if(PlistPath == null) {
			return;
		}

		Hashtable plistData = new Hashtable();

		PListManager.ParsePListFile(PlistPath.text,ref plistData);

		Hashtable bodies = plistData["bodies"] as Hashtable;
		if(bodies == null) {
			Debug.Log("Bodies not found");
		}

		ArrayList keyNames = new ArrayList(bodies.Keys);

		_totalPolygonsinFile = new PolygonObject[keyNames.Count];


		if(keyNames != null) {
			for(int i = 0; i<keyNames.Count; i++) {
				_totalPolygonsinFile[i] = new PolygonObject();
				_totalPolygonsinFile[i].bodyname = keyNames[i] as String;

				Hashtable bodyDic = bodies[keyNames[i]] as Hashtable;
				/*Using single fixture because unity support single fixture*/
				ArrayList fixtures = bodyDic["fixtures"] as ArrayList;

				Hashtable fixture1 = fixtures[0] as Hashtable;

				ArrayList polygonsArray = fixture1["polygons"] as ArrayList;

				PolygonPath[] totalPaths = new PolygonPath[polygonsArray.Count];

				for(int j = 0; j<totalPaths.Length; j++) {
					ArrayList pointArray = polygonsArray[j] as ArrayList;
					PolygonPath tempPath = new PolygonPath();
					Vector2[] pointsVector = new Vector2[pointArray.Count];
					for(int k = 0; k<pointsVector.Length; k++) {
						string pointInString = pointArray[k] as String;
						pointsVector[k] = this.ConvertToVector2FromString(pointInString);
					}

					tempPath.points = pointsVector;

					totalPaths[j] = tempPath;
				}

				_totalPolygonsinFile[i].paths = totalPaths;

			}


			Array.Sort(_totalPolygonsinFile);
			this.setPolygonOfIndex(selectedIndex);

		}else {
			Debug.Log("Keys not found");
		}

	}

	private Vector2 ConvertToVector2FromString(string strVector) {
		strVector = strVector.Replace("{","");
		strVector = strVector.Replace("}","");
		string[] tempposition = strVector.Split(',');
	


		Vector2 retVector = new Vector2(float.Parse(tempposition[0]),float.Parse(tempposition[1]));

		return retVector;
	}

	public void setPolygonOfIndex(int index) {
		_polygonObject = _totalPolygonsinFile[index];

		if (_polygonCollider == null)
			return;

		_polygonCollider.pathCount = _polygonObject.TotalPaths;

		for(int i = 0; i<_polygonCollider.pathCount; i++) {

			Vector2[] tempPoints = new Vector2[_polygonObject.paths[i].points.Length];
			for(int j = 0; j<_polygonObject.paths[i].points.Length; j++) {
				tempPoints[j] = (_polygonObject.paths[i].points[j]) / PixelsPerUnit;
			}

			_polygonCollider.SetPath(i,tempPoints);
		}
	}
	
	void OnEnable() {
//		Debug.Log("Enabled");
	}

	// Update is called once per frame
	void Update () {
	
	}
}
