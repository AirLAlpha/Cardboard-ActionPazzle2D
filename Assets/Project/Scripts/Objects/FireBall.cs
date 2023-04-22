/**********************************************
 * 
 *  FireBall.cs 
 *  炎の丸に関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/02/01
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class FireBall : MonoBehaviour, IPoseable
{
	//	コンポーネント
	private Rigidbody2D rb;         //	Rigidbody2D

	//	移動
	[Header("移動")]
	[SerializeField]
	private float		moveSpeed;					//	移動速度

	public Vector2		Direction { get; set; }     //	移動方向ベクトル

	//	生存
	[Header("生存")]
	[SerializeField]
	private float	lifeTime;		//	生存時間
	[SerializeField]
	private string	hitTags;        //	衝突するオブジェクトのタグ(半角スペースで区切って入力）

	private float alivedTime;       //	経過した時間

	//	ポーズ
	private bool	disableUpdate;
	private Vector2 posedVelocity;

	//	実行前初期化処理
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	//	更新処理
	private void Update()
	{
		if (disableUpdate)
			return;

		rb.velocity = Direction * moveSpeed * Time.deltaTime;

		//	生存時間以上が経過したら自身を削除する
		alivedTime += Time.deltaTime;
		if (alivedTime >= lifeTime)
			Destroy(gameObject);
	}

	/*--------------------------------------------------------------------------------
	|| 衝突判定
	--------------------------------------------------------------------------------*/
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//	衝突したオブジェクトにIBurnableが実装されていたら処理を行う
		if (collision.TryGetComponent<IBurnable>(out var hit))
		{
			//	相手を燃やす処理
			hit.Burn();
			//	自身を削除する
			Destroy(gameObject);
		}

		//	指定されたタグに衝突したら自身を削除する
		string[] tags = hitTags.Split(' ');
		foreach (var tag in tags)
		{
			if(collision.tag == tag)
			{
				Destroy(gameObject);
				return;
			}
		}
	}

	public void Pose()
	{
		disableUpdate = true;

		posedVelocity = rb.velocity;
		rb.isKinematic = true;
		rb.velocity = Vector2.zero;
	}

	public void Resume()
	{
		disableUpdate = false;

		rb.isKinematic = false;
		rb.velocity = posedVelocity;
	}
}
