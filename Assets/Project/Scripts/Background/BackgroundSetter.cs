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
using System.Data.Common;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BackgroundSetter : MonoBehaviour
{
	[Header("コンポーネント")]
	[SerializeField]
	private Animator anim;

	[Header("データベース")]
	[SerializeField]
	private StageBackgroundDatabase backgroundDB;       //	背景のデータベース
	[SerializeField]
	private int						index;

	public int Index { get { return index; } }

	[Header("背景")]
	[SerializeField]
	private SpriteRenderer			backgroundImage;    //	背景画像
	[SerializeField]
	private Vector2					backgroundOffset;	//	背景の生成座標
	[SerializeField]
	private BaseTileJson			baseTilemap;        //	ベースのタイルマップ

	public int BackgroundTypeLength => backgroundDB.Datas.Count();

	private int anim_backgroundID;

	private void Awake()
	{
		//	ハッシュの作成
		anim_backgroundID = Animator.StringToHash("BackgroundID");
	}

	/*--------------------------------------------------------------------------------
	|| 背景の設定処理
	--------------------------------------------------------------------------------*/
	public void SetBackground(int dbIndex = -1)
	{
		if(anim_backgroundID == 0)
			anim_backgroundID = Animator.StringToHash("BackgroundID");

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

		//	アニメーションの有効フラグによってアニメーターのアクティブを切り替える
		anim.enabled = bgData.EnableAnimation;
		//	アニメーターの数値を設定
		int bgId = bgData.EnableAnimation ? index : -1;
		anim.SetInteger(anim_backgroundID, bgId);

		//	タイルを置き換える
		baseTilemap.SetTiles(dbIndex, bgData.TileJson);
	}
}
