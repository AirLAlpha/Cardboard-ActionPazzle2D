using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScrollCamera : MonoBehaviour
{
	[SerializeField]
	private Transform trackingTarget;				//	追跡対象

	public Transform overrideTarget { get; set; }	//	上書き用追跡対象

	[SerializeField]
	protected Vector2		trackingOffset;     //	追跡のオフセット
	[SerializeField]
	private float			trackingSpeed;      //	追跡速度
	[SerializeField]
	private float			overrideSpeedd;		//	上書き時の追跡速度

	[SerializeField]
	private Vector2			trackingMinPos;
	[SerializeField]
	private Vector2			trackingMaxPos;

	[Space]
	[SerializeField]
	protected bool			ignoreY;

	protected float			ignoredPosY;		//	ignoreY有効時のY座標
	protected Vector3		saveNonOffsetPos;	//	オフセットを考慮しない座標
	protected Vector3		currentTargetPos;	//	現在のターゲット座標

	//	更新処理
	protected virtual void LateUpdate()
	{
		//	追跡対象が設定されていないときは処理しない
		if (!trackingTarget && !overrideTarget)
			return;

		//	追跡対象
		currentTargetPos = overrideTarget == null ? trackingTarget.position : overrideTarget.position;
		if (ignoreY)
		{
			currentTargetPos.y = ignoredPosY;
		}
		//	追跡速度
		float speed	= overrideTarget == null ? trackingSpeed : overrideSpeedd;

		saveNonOffsetPos = Vector3.Lerp(saveNonOffsetPos, currentTargetPos, Time.deltaTime * speed);

		Vector3 pos = new Vector3(trackingOffset.x, trackingOffset.y) + saveNonOffsetPos;
		pos.z = -10;

		pos.x = Mathf.Clamp(pos.x, trackingMinPos.x, trackingMaxPos.x);
		pos.y = Mathf.Clamp(pos.y, trackingMinPos.y, trackingMaxPos.y);

		transform.position = pos;
	}

	//	有効化時処理
	private void OnEnable()
	{
		saveNonOffsetPos = transform.position;
	}
}
