using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Line : MonoBehaviour
{
	[SerializeField]
	private Tilemap			tilemap;
	[SerializeField]
	private LineRenderer	linerender;
	[SerializeField]
	private Transform		cursor;

	Camera mainCam;

	//	実行前初期化処理
	private void Awake()
	{
		linerender.SetPosition(0, Vector3.zero + Vector3.one * 0.5f);
	}

	//	初期化処理
	private void Start()
	{
		mainCam = Camera.main;
	}

	//	更新処理
	private void Update()
	{
		var mpos = Input.mousePosition;
		var worldMpos = mainCam.ScreenToWorldPoint(mpos);

		Vector3 a = Vector3Int.FloorToInt(worldMpos) + Vector3.one * 0.5f;

		a.z = 0.0f;
		cursor.position = a;

		var b = tilemap.GetTile(new Vector3Int((int)a.x, (int)a.y, (int)a.z));
		if (b != null)
		{
			Debug.Log(b.name);
			linerender.SetPosition(1, a);
		}
		else
		{

		}
		

	}
}
