/**********************************************
 * 
 *  CameraShake.cs 
 *  画面揺れの処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/07
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	[SerializeField]
	private float		shakePower;     //	揺れの強さ
	[SerializeField]
	private float		shakeTime;

	//	実行前初期化処理
	private void Awake()
	{
		
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void LateUpdate()
	{
		if (shakeTime <= 0)
			return;

		shakeTime -= Time.unscaledDeltaTime;

		Vector3 shakeOffset = Vector3.zero;

		shakeOffset.x = (Random.value - 0.5f) * 2 * shakePower;
		shakeOffset.y = (Random.value - 0.5f) * 2 * shakePower;

		transform.position += shakeOffset;
	}

	/*--------------------------------------------------------------------------------
	|| 画面揺れの開始
	--------------------------------------------------------------------------------*/
	public void StartShake(float time)
	{
		shakeTime = time;
	}
}
