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
public class FireBall : MonoBehaviour, IPauseable
{
	//	コンポーネント
	[SerializeField]
	private ParticleSystem			destroyEffect;

	private Rigidbody2D rb;         //	Rigidbody2D

	//	移動
	[Header("移動")]
	[SerializeField]
	private float		moveSpeed;                  //	移動速度
	[SerializeField]
	private bool		applyRotate;				//	回転フラグ

	private Vector2		direction;
	public Vector2		Direction {set { direction = value; rb.velocity = direction * moveSpeed; } }     //	移動方向ベクトル

	[Header("拡大")]
	[SerializeField]
	private float		sizeChangeRate;


	//	生存
	[Header("生存")]
	[SerializeField]
	private float	lifeTime;		//	生存時間
	[SerializeField]
	private string	hitTags;        //	衝突するオブジェクトのタグ(半角スペースで区切って入力）

	private float alivedTime;       //	経過した時間

	public Transform Parent { get; set; }		//	親オブジェクト

	//	ポーズ
	private bool	disableUpdate;
	private Vector2 posedVelocity;

	//	サウンド
	private SoundPlayer soundPlayer;
	private int			soundIndex;


	//	実行前初期化処理
	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		transform.localScale = Vector3.zero;
	}

	//	更新処理
	private void Update()
	{
		if (disableUpdate)
			return;

		if (applyRotate)
		{
			float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.5f, Time.deltaTime * sizeChangeRate);

		//	生存時間以上が経過するか親がいなくなった時点で自身を削除する
		alivedTime += Time.deltaTime;
		if (alivedTime >= lifeTime ||
			Parent == null)
		{
			Destroy(gameObject);
			GenerateEffect();
		}
	}

	private void GenerateEffect()
	{
		if (destroyEffect == null)
			return;

		Instantiate(destroyEffect, transform.position, Quaternion.identity);

		//	サウンドの再生
		soundPlayer.PlaySound(soundIndex);
	}

	/*--------------------------------------------------------------------------------
	|| 衝突判定
	--------------------------------------------------------------------------------*/
	private void OnTriggerEnter2D(Collider2D collision)
	{
		//	衝突したオブジェクトにIBurnableが実装されていたら処理を行う
		if (collision.TryGetComponent<IBurnable>(out var hit))
		{
			GenerateEffect();

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
				GenerateEffect();

				Destroy(gameObject);
				return;
			}
		}
	}

	public void Pause()
	{
		disableUpdate = true;

		posedVelocity = rb.velocity;
		rb.isKinematic = true;
		rb.velocity = Vector2.zero;
	}

	public void Resume()
	{
		if (this == null)
			return;

		disableUpdate = false;

		rb.isKinematic = false;
		rb.velocity = posedVelocity;
	}

	public void SetSound(SoundPlayer soundPlayer, int dbIndex)
	{
		this.soundPlayer = soundPlayer;
		soundIndex = dbIndex;
	}
}
