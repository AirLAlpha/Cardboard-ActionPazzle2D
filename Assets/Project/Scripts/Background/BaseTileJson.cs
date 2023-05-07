using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class BaseTileJson : MonoBehaviour
{
	[SerializeField]
	private StageBackgroundDatabase backgroundDB;       //	背景のデータベース

#if UNITY_EDITOR
	[SerializeField]
	private int exportDatabaseIndex;
	[SerializeField, Multiline]
	private string exportResult;

	/*--------------------------------------------------------------------------------
	|| 現在のタイルマップを書き出す
	--------------------------------------------------------------------------------*/
	[ContextMenu("Export Json")]
	public void ExportJson()
	{
		//	タイルマップの取得
		Tilemap tilemap = GetComponent<Tilemap>();
		//	タイルマップの大きさを制限する
		tilemap.CompressBounds();
		//	タイルマップの大きさを取得
		BoundsInt tilemapSize = tilemap.cellBounds;

		//	データベースから使用する背景のセットを取得
		BackgroundData bgData = backgroundDB.Datas[exportDatabaseIndex];
		//	書き出しに使用する構造体
		BaseTilemapData tilemapData;
		tilemapData.tiledatas = new List<TileData>();

		for (int y = tilemapSize.xMin; y < tilemapSize.xMax; y++)
		{
			for (int x = tilemapSize.xMin; x < tilemapSize.xMax; x++)
			{
				Vector3Int pos = new Vector3Int(x, y);
				//	タイルの取得
				var tile = tilemap.GetTile(pos);
				//	タイルがなければ処理しない
				if (tile == null)
					continue;

				//	データベースからタイルを検索する
				int tileIndex = bgData.GroundTiles.FindIndex(dbTile => dbTile == tile);
				if (tileIndex == -1)
					continue;

				//	タイルデータを設定
				TileData newTileData;
				newTileData.pos = new Vector2Int(x, y);
				newTileData.groundTileIndex = tileIndex;
				//	タイルデータに回転を設定
				Matrix4x4 mat = tilemap.GetTransformMatrix(pos);
				newTileData.m00 = (int)mat.m00;
				newTileData.m01 = (int)mat.m01;
				newTileData.m10 = (int)mat.m10;
				newTileData.m11 = (int)mat.m11;
				//	タイルデータに反転を設定
				newTileData.flipX = Mathf.Sign(mat.m00) < 0;
				newTileData.flipY = Mathf.Sign(mat.m11) < 0;

				//	書き出し対象に追加する
				tilemapData.tiledatas.Add(newTileData);
			}
		}

		string ret = JsonUtility.ToJson(tilemapData);
		exportResult= ret;
	}
#endif

	/*--------------------------------------------------------------------------------
	|| タイルマップを読み込む
	--------------------------------------------------------------------------------*/
	public void SetTiles(int bgIndex, string json)
	{
		//	タイルマップの取得
		Tilemap tilemap = GetComponent<Tilemap>();
		//	すべてのタイルを削除
		tilemap.ClearAllTiles();

		//	データベースから使用する背景のセットを取得
		BackgroundData bgData = backgroundDB.Datas[bgIndex];
		//	Jsonから構造体に変換する
		var tilemapData = JsonUtility.FromJson<BaseTilemapData>(json);

		//	タイルの設置
		foreach (var tileData in tilemapData.tiledatas)
		{
			//	データベースよりタイルを取得
			var tile = bgData.GroundTiles[tileData.groundTileIndex];
			//	行列を作成
			Matrix4x4 mat = Matrix4x4.identity;
			mat.m00 = tileData.m00;
			mat.m01 = tileData.m01;
			mat.m10 = tileData.m10;
			mat.m11 = tileData.m11;

			//	設置
			Vector3Int pos = new Vector3Int(tileData.pos.x, tileData.pos.y);
			tilemap.SetTile(pos, tile);
			tilemap.SetTransformMatrix(pos, mat);
		}
	}

}