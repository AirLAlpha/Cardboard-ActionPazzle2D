#if UNITY_EDITOR
/**********************************************
 * 
 *  EditModeRevision.cs 
 *  編集モード中にグリッドに合わせて移動させる処理
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/03/03
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EditModeRevision : MonoBehaviour
{
	//const float GRID_GAP_HALF = 0.5f;			//	グリッドの大きさの半分
	[SerializeField]
	private Vector2 gridOffset;     //	グリッドからのズレ
	[SerializeField]
	private float z;

	private void Update()
	{
		//	プレイモード中は処理しない
		if (Application.isPlaying) return;

		//	スケールからグリッドにおけるズレを計算
		Vector3 offset = transform.localScale / 2;

		//	座標をintに変換してグリッドの座標の半分を足す
		Vector3 posI = Vector3Int.FloorToInt(transform.position - offset);
		transform.position = posI + offset + new Vector3(gridOffset.x, gridOffset.y);
		transform.position = new Vector3(transform.position.x, transform.position.y, z);
	}
}
#endif