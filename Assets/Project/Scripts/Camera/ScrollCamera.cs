using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollCamera : MonoBehaviour
{
	[SerializeField]
	private Transform trackingTarget;       //	追跡対象

	[SerializeField]
	private Vector2		trackingOffset;     //	追跡のオフセット
	[SerializeField]
	private float		trackingSpeed;      //	追跡速度

	[SerializeField]
	private Vector2		trackingMinPos;
	[SerializeField]
	private Vector2		trackingMaxPos;

	//	更新処理
	private void Update()
	{
		//	追跡対象が設定されていないときは処理しない
		if (!trackingTarget)
			return;

		Vector3 pos = Vector3.Lerp(transform.position, trackingTarget.position, Time.deltaTime * trackingSpeed);
		pos += new Vector3(trackingOffset.x, trackingOffset.y);
		pos.z = -10;

		pos.x = Mathf.Clamp(pos.x, trackingMinPos.x, trackingMaxPos.x);
		pos.y = Mathf.Clamp(pos.y, trackingMinPos.y, trackingMaxPos.y);

		transform.position = pos;


	}
}
