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
	[Header("刃")]
	[SerializeField]
	private Transform	blade;			//	刃
	[SerializeField]
	private float		rotateSpeed;	//	回転速度

	[Header("梱包")]
	[SerializeField]
	private bool packable;		//	梱包可能フラグ

	public bool Packable { get { return packable; } set { packable = value; } }


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

		blade.rotation *= Quaternion.AngleAxis(Time.deltaTime * rotSpeed, Vector3.forward);
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

		if(collision.transform.tag == "Player")
		{
			if(collision.transform.TryGetComponent<PlayerDamageReciver>(out PlayerDamageReciver pdr))
			{
				pdr.Burn();
			}
		}
	}
}
