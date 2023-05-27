using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCamera : MonoBehaviour
{
	static readonly float CAMERA_Z = -10.0f;

	[Header("スクロール")]
	[SerializeField]
	private Transform		cameraTarget;       //	目標のオブジェクト
	[SerializeField]
	private float			scrollSpeed;        //	スクロール速度
	[SerializeField]
	private Vector2			scrollOffset;       //	スクロールのオフセット
	[SerializeField]
	private float			goalDistance;		//	到着したとする距離

	//	スクロール対象の座標
	private Vector3 ScrollTargetPosition => cameraTarget.position + (Vector3)scrollOffset + Vector3.forward * CAMERA_Z;

	//	プロパティ
	public Transform	CameraTarget { get { return cameraTarget; } set { cameraTarget = value; } }
	public bool			IsGoal { get; private set; }

	//	更新処理
	private void Update()
	{
		//	目標のものがないときは処理しない
		if (cameraTarget == null)
			return;

		transform.position = Vector3.Lerp(transform.position, ScrollTargetPosition, Time.deltaTime * scrollSpeed);

		//	距離を求めてゴールしたかどうかを判定する
		float sqrDist = Vector3.SqrMagnitude(ScrollTargetPosition - transform.position);
		if (sqrDist < goalDistance * goalDistance)
			IsGoal = true;
		else
			IsGoal = false;
	}

	/*--------------------------------------------------------------------------------
	|| カメラの位置を強制的にターゲットに移動させる
	--------------------------------------------------------------------------------*/
	public void SetPosToTarget()
	{
		transform.position = ScrollTargetPosition;
	}
}
