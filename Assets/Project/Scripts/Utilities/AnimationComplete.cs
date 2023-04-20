using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationComplete : MonoBehaviour
{
	[SerializeField]
	private bool		animationCompleted;      //	アニメーション終了フラグ

	public bool			AnimationCompleted { get { return animationCompleted; } }

	public UnityEvent	OnAnimationCompleted;

	/*--------------------------------------------------------------------------------
	|| アニメーション終了時にイベントより呼び出す処理
	--------------------------------------------------------------------------------*/
	public void OnAnimationComplete()
	{
		animationCompleted = true;
		OnAnimationCompleted?.Invoke();
	}

	/*--------------------------------------------------------------------------------
	|| アニメーション開始時にイベントより呼び出す処理
	--------------------------------------------------------------------------------*/
	public void Reset()
	{
		animationCompleted = false;
	}

}
