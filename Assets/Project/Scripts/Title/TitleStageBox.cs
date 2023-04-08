/**********************************************
 * 
 *  TitleStageBox.cs 
 *  ステージセレクトの各ステージのボックスの処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleStageBox : MonoBehaviour
{
	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private Animator		stageClearStatusAnim;
	[SerializeField]
	private TitleManager	titleManager;

	//	ステージ情報
	[Header("ステージ情報")]
	[SerializeField]
	private int stageIndex;

	//	選択
	[Header("選択")]
	[SerializeField]
	private Rect			selectArea;     //	選択中とする範囲
	[SerializeField]
	private LayerMask		selectMask;		//	選択できるレイヤー

	//	選択中とする範囲の中心座標
	private Vector3 SelectAreaCenter => new Vector3(selectArea.x, selectArea.y) + transform.position;

	private bool			inRange;		//	範囲内フラグ

#if UNITY_EDITOR
	[Header("デバッグ")]
	[SerializeField]
	private bool drawGizmos;
#endif


	//	更新処理
	private void Update()
	{
		CheckArea();
		SelectUpdate();

		//	クリア状況のアニメーションパラメータを変更
		stageClearStatusAnim.SetBool("Enable", inRange);
	}

	/*--------------------------------------------------------------------------------
	|| 範囲判定
	--------------------------------------------------------------------------------*/
	private void CheckArea()
	{
		inRange = Physics2D.OverlapBox(SelectAreaCenter, selectArea.size, 0, selectMask);
	}

	/*--------------------------------------------------------------------------------
	|| 選択の適応処理
	--------------------------------------------------------------------------------*/
	private void SelectUpdate()
	{
		//	範囲外
		if(!inRange)
		{
			//	自身の保持するステージ番号のときはリセットする
			if (titleManager.SelectedStage == this.stageIndex)
				titleManager.SelectedStage = -1;
		}
		//	範囲内
		else
		{
			//	選択中のステージとして設定する
			titleManager.SelectedStage = this.stageIndex;
		}
	}


#if UNITY_EDITOR
	/*--------------------------------------------------------------------------------
	|| ギズモの描画処理
	--------------------------------------------------------------------------------*/
	private void OnDrawGizmosSelected()
	{
		//	フラグが有効でないときは処理しない
		if (!drawGizmos)
			return;

		//	色の設定
		Color col = Color.yellow;
		col.a = 0.5f;
		Gizmos.color = col;
		//	範囲の描画
		Gizmos.DrawCube(SelectAreaCenter, selectArea.size);
	}
#endif
}
