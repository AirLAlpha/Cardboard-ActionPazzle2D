/**********************************************
 * 
 *  BladeEnemy.cs 
 *  刃物の敵の処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/03
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BladeEnemy : Enemy, IPackable
{
	[Header("エフェクト")]
	[SerializeField]
	private ParticleSystem sparkEffect;
	[SerializeField]
	private Vector2			particleOffset;

	private ParticleSystem haveEffect;		//	自分の管理下にあるエフェクト

	[Header("接地確認")]
	[SerializeField]
	private Vector2		checkGroundRange;
	[SerializeField]
	private Vector2		checkGroundOffset;
	[SerializeField]
	private LayerMask	checkGroundMask;

	private bool onGround;

	private Vector2 CheckGroundCenter => transform.position + (Vector3)checkGroundOffset;


	[Header("刃")]
	[SerializeField]
	private Transform	body;
	[SerializeField]
	private Transform	blade;			//	刃
	[SerializeField]
	private float		rotateSpeed;	//	回転速度

	[Header("梱包")]
	[SerializeField]
	private bool packable;		//	梱包可能フラグ

	public bool Packable { get { return packable; } set { packable = value; } }

	//	初期化処理
	private void Start()
	{
		haveEffect = Instantiate(sparkEffect, transform.position, Quaternion.identity) as ParticleSystem;
		haveEffect.Stop();
	}

	//	更新処理
	protected override void Update()
	{
		base.Update();

		CheckGround();
		EffectUpdate();
	}


	protected override void IdleUpdate()
	{
	}

	protected override void AttentionUpdate()
	{
	}

	protected override void AttackUpdate()
	{
	}

	protected override void AnimationUpdate()
	{
		float rotSpeed = packable ? 0 : rotateSpeed;

		body.rotation *= Quaternion.AngleAxis(Time.deltaTime * -rotSpeed / 2, Vector3.forward);
		blade.rotation *= Quaternion.AngleAxis(Time.deltaTime * rotSpeed, Vector3.forward);
	}

	//	削除時処理
	private void OnDestroy()
	{
		if (haveEffect != null)
			Destroy(haveEffect.gameObject);
	}

	/*--------------------------------------------------------------------------------
	|| 接地確認
	--------------------------------------------------------------------------------*/
	private void CheckGround()
	{
		var result = Physics2D.OverlapBox(CheckGroundCenter, checkGroundRange, 0, checkGroundMask);
		if (result)
		{
			onGround = true;
			return;
		}

		onGround = false;
	}

	/*--------------------------------------------------------------------------------
	|| エフェクトの更新処理
	--------------------------------------------------------------------------------*/
	private void EffectUpdate()
	{
		if(onGround && haveEffect.isStopped && !packable)
		{
			haveEffect.Play();
		}
		if((!onGround && haveEffect.isPlaying) ||
			(packable && haveEffect.isPlaying))
		{
			haveEffect.Stop();
		}

		haveEffect.transform.position = transform.position + (Vector3)particleOffset;
	}


	/*--------------------------------------------------------------------------------
	|| 梱包処理
	--------------------------------------------------------------------------------*/
	CardboardType IPackable.Packing()
	{
		if(!packable)
		{
			return CardboardType.NONPACKABLE;
		}

		Destroy(gameObject);
		return this.packedType;
	}

	/*--------------------------------------------------------------------------------
	|| 衝突判定
	--------------------------------------------------------------------------------*/
	private void OnCollisionEnter2D(Collision2D collision)
	{
		//	刃が止まっている時は処理しない
		if (packable)
			return;

		if(collision.transform.TryGetComponent<IBurnable>(out IBurnable burnable))
		{
			burnable.Burn();
		}
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);

		Gizmos.DrawCube(CheckGroundCenter, checkGroundRange);
	}
#endif
}
