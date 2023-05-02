/**********************************************
 * 
 *  BackgroundSetter.cs 
 *  背景の設定を適応する
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/28
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundSetter : MonoBehaviour
{
	[Header("データベース")]
	[SerializeField]
	private StageBackgroundDatabase backgroundDB;       //	背景のデータベース
	[SerializeField]
	private int						index;

	public int Index { get { return index; } }

	[Header("背景")]
	[SerializeField]
	private SpriteRenderer			backgroundImage;	//	背景画像
	[SerializeField]
	private BaseTileJson			baseTilemap;        //	ベースのタイルマップ

	public int BackgroundTypeLength => backgroundDB.Datas.Count();

	/*--------------------------------------------------------------------------------
	|| 背景の設定処理
	--------------------------------------------------------------------------------*/
	public void SetBackground(int dbIndex = -1)
	{
		if (dbIndex == -1)
			dbIndex = index;
		else
			index = dbIndex;

		//	配列範囲外をチェックする
		if (0 > dbIndex || dbIndex >= backgroundDB.Datas.Count())
			return;

		//	データベースから背景情報を取得
		BackgroundData bgData = backgroundDB.Datas[dbIndex];

		//	背景画像を設定する
		backgroundImage.sprite = bgData.BackgroundImage;

		//	タイルを置き換える
		baseTilemap.SetTiles(dbIndex, bgData.TileJson);
	}
}
