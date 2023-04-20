using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScrollCamera : MonoBehaviour
{
	[SerializeField]
	private Transform trackingTarget;               //	追跡対象
	[SerializeField]
	private Transform overrideTarget;               //	上書き用追跡対象

	public Transform OverrideTarget { get { return overrideTarget; } set { overrideTarget = value; } }	

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
	protected Vector3		currentTargetPos;   //	現在のターゲット座標

	protected virtual void Start()
	{
		Vector3 pos = OverrideTarget == null ? trackingTarget.position : OverrideTarget.position;
		if (ignoreY)
			pos.y = ignoredPosY;
		saveNonOffsetPos.y = pos.y;
	}

	//	更新処理
	protected virtual void LateUpdate()
	{
		//	追跡対象が設定されていないときは処理しない
		if (!trackingTarget && !OverrideTarget)
			return;

		//	追跡対象
		currentTargetPos = OverrideTarget == null ? trackingTarget.position : OverrideTarget.position;
		if (ignoreY)
		{
			currentTargetPos.y = ignoredPosY;
		}
		//	追跡速度
		float speed	= OverrideTarget == null ? trackingSpeed : overrideSpeedd;

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

	/*--------------------------------------------------------------------------------
	|| 移動アニメーションのスキップ
	--------------------------------------------------------------------------------*/
	public void SkipScroll()
	{
		saveNonOffsetPos = trackingTarget.position;
		Vector3 pos = saveNonOffsetPos + new Vector3(trackingOffset.x, trackingOffset.y);
		pos.z = -10;
		pos.x = Mathf.Clamp(pos.x, trackingMinPos.x, trackingMaxPos.x);
		pos.y = Mathf.Clamp(pos.y, trackingMinPos.y, trackingMaxPos.y);

		transform.position = pos;
	}

}
