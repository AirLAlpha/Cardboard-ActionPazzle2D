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

public class GlassEnemy : Enemy, IPackable, IBurnable
{
	[Header("ガラス")]
	[SerializeField]
	private float			breakingVelocity;		//	破壊されてしまう速度
	[SerializeField]
	private ParticleSystem	breakEffect;            //	破壊時のエフェクト

	private Transform		effectRoot;             //	エフェクトの親オブジェクト

	[Header("めり込み")]
	[SerializeField]
	private LayerMask overlapMask;

	//	初期化処理
	private void Start()
	{
		//	エフェクトの親を検索
		effectRoot = GameObject.Find("EffectRoot").transform;
	}

	//	更新処理
	protected override void Update()
	{
		base.Update();

		CheckOverlap();
	}

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
			Burn();
	}

	/*--------------------------------------------------------------------------------
	|| 破壊処理
	--------------------------------------------------------------------------------*/
	public void Burn()
	{
		var effect = Instantiate(breakEffect, transform.position, Quaternion.identity, effectRoot);
		effect.Play();

		Destroy(gameObject);
	}

	/*--------------------------------------------------------------------------------
	|| めり込み判定
	--------------------------------------------------------------------------------*/
	private void CheckOverlap()
	{
		var hit = Physics2D.OverlapBox(transform.position, Vector2.one * 0.01f, 0.0f, overlapMask);
		if (hit != null)
		{
			Burn();
		}
	}
}
