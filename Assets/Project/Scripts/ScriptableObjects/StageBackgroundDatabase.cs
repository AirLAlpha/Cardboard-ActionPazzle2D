/**********************************************
 * 
 *  StageBackgroundDatabase.cs 
 *  ステージの背景に関するデータベース
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/28
 * 
 **********************************************/
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TileData
{
	public int groundTileIndex;		//	データベースインデックス
	public Vector2Int pos;			//	座標

	//	行列の回転情報
	public int m00;
	public int m01;
	public int m10;
	public int m11;
}

[System.Serializable]
public struct BaseTilemapData
{
	public List<TileData> tiledatas;
}

[System.Serializable]
public struct BackgroundData
{
	[SerializeField]
	private string		name;
	[SerializeField]
	private Sprite		backgroundImage;
	[SerializeField]
	private List<Tile>	groundTiles;
	[SerializeField]
	private string		tileJson;			//	タイルの配置情報（Json）

	public Sprite		BackgroundImage { get { return backgroundImage; } }
	public List<Tile>	GroundTiles		{ get { return groundTiles; } }
	public string		TileJson		{ get { return tileJson; } }
}

[CreateAssetMenu(menuName = "Create StageBackgroundDatabase")]
public class StageBackgroundDatabase : ScriptableObject
{
	[SerializeField]
	private List<BackgroundData>	datas;

	public List<BackgroundData> Datas { get { return datas; } }

	public int DataCount => datas.Count;
}