/**********************************************
 * 
 *  GlassEnemy.cs 
 *  ガラスの的に関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/16
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassEnemy : Enemy, IPackable
{
	//	初期化処理
	private void Start()
	{
		
	}

	/*--------------------------------------------------------------------------------
	|| 待機中の更新処理
	--------------------------------------------------------------------------------*/
	protected override void IdleUpdate()
	{
		//throw new System.NotImplementedException();
	}

	/*--------------------------------------------------------------------------------
	|| 警戒中の更新処理
	--------------------------------------------------------------------------------*/
	protected override void AttentionUpdate()
	{
		//throw new System.NotImplementedException();
	}

	/*--------------------------------------------------------------------------------
	|| 攻撃の更新処理
	--------------------------------------------------------------------------------*/
	protected override void AttackUpdate()
	{
		//throw new System.NotImplementedException();
	}

	/*--------------------------------------------------------------------------------
	|| アニメーションの更新処理
	--------------------------------------------------------------------------------*/
	protected override void AnimationUpdate()
	{
		bool attention = currentState != State.IDLE;
		anim.SetBool("Attention", attention);

		bool open = currentState == State.ATTACK;
		anim.SetBool("Open", open);
	}

	/*--------------------------------------------------------------------------------
	|| 梱包処理
	--------------------------------------------------------------------------------*/
	public CardboardType Packing()
	{
		//	自身を削除する
		Destroy(gameObject);

		return this.packedType;
	}
}
