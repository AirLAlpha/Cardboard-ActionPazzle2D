///**********************************************
// * 
// *  TilemapJsonUtility.cs 
// *  TilemapとJsonを相互変換する処理を記述を
// * 
// *  製作者：牛丸 文仁
// *  制作日：2023/04/18
// * 
// **********************************************/
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public static class TilemapJsonUtility
//{
//	public static List<StageObjectData> TilemapToJson(Tilemap tilemap)
//	{
//		//	バウンディボックスの範囲に納める
//		tilemap.CompressBounds();
//		//	タイルマップの大きさを取得
//		BoundsInt tilemapSize = tilemap.cellBounds;
//		//	戻り地として使用するリストを作成
//		List<StageObjectData> ret = new List<StageObjectData>();

//		for (int y = tilemapSize.min.y; y < tilemapSize.max.y; y++)
//		{
//			for (int x = tilemapSize.min.x; x < tilemapSize.max.x; x++)
//			{

//			}
//		}




//		return "";
//	}


//	/*--------------------------------------------------------------------------------
//	|| JsonをTilemapに変換する
//	--------------------------------------------------------------------------------*/
//	public static void JsonToTilemap(string json, ref Tilemap targetTilemap)
//	{
//	}


//}
