/**********************************************
 * 
 *  BeltConveyor.cs 
 *  ベルトコンベアに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltConveyorGimmick : ReceiveGimmick, IPauseable
{
	//	ベルトコンベア固有の設定
	[System.Serializable]
	private struct BeltConveyorSettings
	{
		public float	speedMod;
		public bool		inverse;
	}

	[SerializeField]
	private Transform spriteRoot;
	[SerializeField]
	private Vector2 velocity;           //	与える移動量
	[SerializeField]
	private bool	isActive;			//	稼働フラグ
	public bool		IsActive	{ set { isActive = value; } get { return isActive; } }

	[SerializeField]
	private Animator	anim;
	[SerializeField]
	private float		animationSpeed;

	System.Action<bool> buttonEvent => (bool pressed) => { isActive = pressed; };

	//	ポーズ
	private bool isPause;

	//	プロパティ
	public float	SpeedModifier	{ get; set; }		//	スピード倍率
	public bool		Inverse			{ get; set; }       //	反転フラグ

	//	初期化処理
	private void Awake()
	{
		SpeedModifier = 1.0f;
		Inverse = false;
	}

	//	更新処理
	private void Update()
	{
		if (anim == null)
			return;

		float speed = 0.0f;
		if (isActive && !isPause)
			speed = animationSpeed * SpeedModifier;

		anim.SetFloat("AnimationSpeed", speed);

		//	反転
		int inv = Inverse ? -1 : 1;
		spriteRoot.localScale = new Vector3(inv, 1.0f);
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (!isActive || isPause)
			return;

		//	反転
		int direction = Inverse ? -1 : 1;

		if(collision.tag == "Player")
		{
			if (collision.transform.TryGetComponent<PlayerMove>(out PlayerMove playerMove))
				playerMove.SecondaryVelocity = velocity * Time.deltaTime * SpeedModifier * direction;

			return;
		}

		if (collision.transform.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
		{
			if (rb.bodyType != RigidbodyType2D.Dynamic)
				return;

			rb.velocity = velocity * Time.deltaTime * SpeedModifier * direction;
		}
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if (!isActive || isPause)
			return;

		if(collision.tag == "Player")
		{
			if (collision.transform.TryGetComponent<PlayerMove>(out PlayerMove playerMove))
				playerMove.SecondaryVelocity = Vector2.zero;
		}

	}

	public void Pause()
	{
		this.isPause = true;
	}

	public void Resume()
	{
		this.isPause = false;
	}

	protected override void AddAction()
	{
		if (Sender == null)
			return;

		Sender.AddAction(buttonEvent);
	}

	protected override void RemoveAction()
	{
		if (Sender == null)
			return;

		Sender.RemoveAction(buttonEvent);
	}

	public override void SetExtraSetting(string json)
	{
		if (json == null || json == string.Empty)
			return;

		BeltConveyorSettings settings = JsonUtility.FromJson<BeltConveyorSettings>(json);
		this.SpeedModifier = settings.speedMod;
		this.Inverse = settings.inverse;
	}

	public override string GetExtraSetting()
	{
		BeltConveyorSettings settings = new BeltConveyorSettings();
		settings.speedMod = this.SpeedModifier;
		settings.inverse = this.Inverse;

		string ret = JsonUtility.ToJson(settings);
		return ret;
	}
}
