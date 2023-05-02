using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationComplete : MonoBehaviour
{
	[SerializeField]
	private bool		animationCompleted;      //	アニメーション終了フラグ
	[SerializeField]
	private float		invokeWait;             //	イベントを呼び出すまで待つ時間

	private float		waitTimer;
	private bool		invoked;				//	イベントを呼び出したフラグ

	public bool			AnimationCompleted { get { return animationCompleted; } }

	public UnityEvent	OnAnimationCompleted;

	//	更新処理
	private void Update()
	{
		//	すでに処理が呼び出されたあとなら処理しない
		if (invoked)
			return;
		//	アニメーションが終了していないときは処理しない
		if (!animationCompleted)
			return;

		//	タイマーを進める
		if (waitTimer > 0)
		{
			waitTimer -= Time.deltaTime;
		}
		else
		{
			//	時間になったら処理を呼び出し、呼び出し済みとする
			OnAnimationCompleted?.Invoke();
			invoked = true;
		}
	}

	/*--------------------------------------------------------------------------------
	|| アニメーション終了時にイベントより呼び出す処理
	--------------------------------------------------------------------------------*/
	public void OnAnimationComplete()
	{
		animationCompleted = true;
		waitTimer = invokeWait;
	}

	/*--------------------------------------------------------------------------------
	|| アニメーション開始時にイベントより呼び出す処理
	--------------------------------------------------------------------------------*/
	public void Reset()
	{
		animationCompleted = false;
		invoked = false;
	}

}
