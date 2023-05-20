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
	[Header("ガラス")]
	[SerializeField]
	private float			breakingVelocity;		//	破壊されてしまう速度
	[SerializeField]
	private ParticleSystem	breakEffect;            //	破壊時のエフェクト

	public Sprite LabelSprite { get { return labelSprite; } }


	/*--------------------------------------------------------------------------------
	|| 待機中の更新処理
	--------------------------------------------------------------------------------*/
	protected override void IdleUpdate()
	{
	}

	/*--------------------------------------------------------------------------------
	|| 警戒中の更新処理
	--------------------------------------------------------------------------------*/
	protected override void AttentionUpdate()
	{
	}

	/*--------------------------------------------------------------------------------
	|| 攻撃の更新処理
	--------------------------------------------------------------------------------*/
	protected override void AttackUpdate()
	{
	}

	/*--------------------------------------------------------------------------------
	|| アニメーションの更新処理
	--------------------------------------------------------------------------------*/
	protected override void AnimationUpdate()
	{

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

	/*--------------------------------------------------------------------------------
	|| 衝突判定
	--------------------------------------------------------------------------------*/
	private void OnCollisionEnter2D(Collision2D collision)
	{
		Vector2 hitVel = collision.relativeVelocity;

		//	プレイヤーのときのみの処理
		if (collision.transform.tag == "Player")
		{
			const float IGNORE_RAD = 45.0f;     //	無視するとする角度

			//	プレイヤーとの角度を求める
			Vector2 dir = (collision.transform.position - transform.position).normalized;
			float rad = Mathf.Abs(Mathf.Atan2(dir.y, dir.x));

			//	プレイヤーとの角度が一定の範囲でなければ処理しない
			if (rad < Mathf.Deg2Rad * IGNORE_RAD ||
				rad > Mathf.Deg2Rad * (180.0f - IGNORE_RAD))
				return;
		}

		float hitVelMag = hitVel.sqrMagnitude;
		if (hitVelMag >= breakingVelocity * breakingVelocity)
			Dead();
	}
}
