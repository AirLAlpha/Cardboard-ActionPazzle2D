/**********************************************
 * 
 *  ScrollOverrider.cs 
 *  スクロールの書き換えを行う処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/07
 * 
 **********************************************/
using UnityEngine;

public class ScrollOverrider : MonoBehaviour
{

	//	コンポーネント
	private ScrollCamera			scrollCamera;       //	スクロールカメラの取得

	//	範囲
	[Header("範囲")]
	[SerializeField]
	private Rect					overrideArea;       //	上書き範囲
	[SerializeField]
	private LayerMask				overrideMask;		//　レイヤーマスク

	private bool					inRange;			//	範囲内フラグ

	private Vector3 AreaCenter => new Vector3(overrideArea.x, overrideArea.y) + transform.position;

#if UNITY_EDITOR
	//	デバッグ
	[Header("デバッグ")]
	[SerializeField]
	private bool drawGizmos;		//	ギズモの描画フラグ
#endif


	//	実行前初期化処理
	private void Awake()
	{
		if(!Camera.main.TryGetComponent(out scrollCamera))
		{
			Debug.LogError("MainCameraに ScrollCamear コンポーネントがアタッチされていません。");
		}
	}

	//	更新処理
	private void Update()
	{
		CheckRange();
		TargetOverride();
	}

	/*--------------------------------------------------------------------------------
	|| 範囲判定
	--------------------------------------------------------------------------------*/
	private void CheckRange()
	{
		inRange = Physics2D.OverlapBox(AreaCenter, overrideArea.size, 0.0f, overrideMask);
	}

	/*--------------------------------------------------------------------------------
	|| 追跡対象の上書き処理
	--------------------------------------------------------------------------------*/
	private void TargetOverride()
	{
		if(!inRange)
		{
			if (scrollCamera.OverrideTarget == transform)
				scrollCamera.OverrideTarget = null;
		}
		else
		{
			scrollCamera.OverrideTarget = transform;
		}
	}

#if UNITY_EDITOR
	/*--------------------------------------------------------------------------------
	|| ギズモの描画処理
	--------------------------------------------------------------------------------*/
	private void OnDrawGizmos()
	{
		if (!drawGizmos)
			return;

		Color col = Color.green;
		col.a = 0.5f;
		Gizmos.color = col;

		Gizmos.DrawCube(AreaCenter, overrideArea.size);
	}
#endif
}
