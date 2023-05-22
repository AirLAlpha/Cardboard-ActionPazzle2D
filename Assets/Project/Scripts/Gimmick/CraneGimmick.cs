/**********************************************
 * 
 *  CraneGimmick.cs 
 *  クレーンのギミックに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/03
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using System.Windows.Forms.VisualStyles;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CraneGimmick : ReceiveGimmick, IPauseable
{
	//	ギミック固有の設定
	private struct CraneSetting
	{
		public float speed;		//	速度
		public float rangeY;	//	可動域（Y）
	}

	//	クレーンの状態
	private enum CraneState
	{
		IDLE,		//	待機
		FALL,		//	下降
		GRAB,		//	つかむ
		RISING,		//	上がる

		RELEASE,	//	離す
	}
	private CraneState currentState;


	[Header("コンポーネント")]
	[SerializeField]
	private Animator		anim;

	private LineRenderer	lineRenderer;		//	ラインレンダー

	[Header("クレーン")]
	[SerializeField]
	private Transform		spriteRoot;
	[SerializeField]
	private float			speed;				//	移動速度
	[SerializeField]
	private float			heightRange;        //	降りる長さ
	[SerializeField]
	private float			grabWait;			//	掴むまでの待ち時間

	private float			progress;           //	現在の進行度
	private float			grabTimer;			//	掴む時間の計測用
	private const float		ARM_X = 0.5f;		//	SpriteRootのオフセット（X)

	public float Speed { get { return this.speed; } set { this.speed = value; } }
	public float RangeY { get { return this.heightRange; } set { this.heightRange = value; } }

	[Header("アーム")]
	[SerializeField]
	private Vector2			grabArea;           //	つかむ範囲
	[SerializeField]
	private Vector2			grabAreaOffset;     //	つかむ範囲のオフセット
	[SerializeField]
	private Vector2			grabedOffset;       //	つかんだオブジェクトのオフセット
	[SerializeField]
	private LayerMask		grabMask;           //	つかめるオブジェクトのレイヤーマスク

	private Transform		grabParent;			//	つかんでいるオブジェクトの前の親
	private Transform		grabTarget;         //	つかんでいるオブジェクト

	private Vector3 GrabCenter => spriteRoot.position + transform.rotation * new Vector3(grabAreaOffset.x, grabAreaOffset.y);

	[Header("ボタン")]
	[SerializeField]
	private bool			active;

	private bool			closeArms;          //	アームの閉じるフラグ	

	//	ポーズ
	private bool			isPause;

	System.Action<bool> buttonAction => (bool pressed) =>
	{
		this.active = pressed;

		//	ボタン投下時
		if(pressed)
		{
			//	待機中 or 上昇中なら下降に変更
			if (currentState == CraneState.IDLE || currentState == CraneState.RISING)
				currentState = CraneState.FALL;
		}
		//	ボタン開放時
		else
		{
			//	上昇中以降なら開放する
			if ((int)currentState >= (int)CraneState.RISING ||
			currentState == CraneState.GRAB && closeArms)
				currentState = CraneState.RELEASE;
			//	上昇前なら上昇に変更
			else
				currentState = CraneState.RISING;
		}
	};


	//	実行前初期化処理
	private void Awake()
	{
		lineRenderer = GetComponent<LineRenderer>();
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		MoveUpdate();
		ArmUpdate();
		WireUpdate();
	}

	//	終了処理
	private void OnDestroy()
	{
	}
	private void OnDisable()
	{
	}



	/*--------------------------------------------------------------------------------
	|| 移動処理
	--------------------------------------------------------------------------------*/
	private void MoveUpdate()
	{
		//	進行度を進める
		int direction = -1;
		if (currentState == CraneState.FALL) direction = 1;
		if (currentState == CraneState.GRAB) direction = 0;

		progress += Time.deltaTime * speed * direction;
		progress = Mathf.Clamp01(progress);

		//	イージングを適応
		float easeProgress = EasingFunctions.EaseInOutSine(progress);
		//	座標を更新する
		spriteRoot.localPosition = Vector3.Lerp(Vector3.zero, Vector3.down * heightRange, easeProgress) + Vector3.right * ARM_X;
	}

	/*--------------------------------------------------------------------------------
	|| アームの処理
	--------------------------------------------------------------------------------*/
	private void ArmUpdate()
	{
		{
			//	下がっていく処理
			//if (active)
			//{
			//	//	下がり切るまでは開けておく
			//	if (progress < 1)
			//	{
			//		//	なにかつかんでいれば離す
			//		if (closeArms)
			//			Release();

			//		closeArms = false;
			//	}
			//	//	下がりきったら閉める
			//	else
			//	{
			//		//	アームが閉まる前ならつかむ処理を実行する
			//		if (!closeArms)
			//			Grab();

			//		closeArms = true;
			//	}
			//}
			////	上がっていく処理
			//else
			//{
			//	//	上がり切るまでは閉めておく
			//	if(progress > 0)
			//	{
			//		//	閉める瞬間ならつかむ
			//		if (!closeArms)
			//			Grab();

			//		closeArms = true;
			//	}
			//	//	上がりきったら開ける
			//	else
			//	{
			//		//	アームが開く前なら離す処理を実行する
			//		if (closeArms)
			//			Release();

			//		closeArms = false;
			//	}
			//}
		}

		//	降下時に進行度が1に到達したら、掴むに移行
		if(currentState == CraneState.FALL &&
			progress >= 1.0f)
		{
			currentState = CraneState.GRAB;
			grabTimer = 0;
		}

		//	つかむ時はタイマーが指定時間を超えたらつかむを実行し、上昇させる
		if (currentState == CraneState.GRAB)
		{
			if (grabTimer >= grabWait)
			{
				//	掴む前
				if (!closeArms)
				{
					Grab();
					closeArms = true;

					grabTimer = 0;
				}
				//	掴んだあと
				else
				{
					currentState = CraneState.RISING;
					grabTimer = 0;
				}
			}
			else
			{
				grabTimer += Time.deltaTime;
				return;
			}
		}

		//	上昇後に開放を行う
		if (currentState == CraneState.RISING &&
			progress <= 0.0f &&
			!closeArms)
			currentState = CraneState.RELEASE;

		//	開放状態のときに開放を行い、待機に戻す
		if(currentState == CraneState.RELEASE)
		{
			closeArms = false;
			Release();
			currentState = CraneState.IDLE;
		}

		//	アームにフラグを適応する
		anim.SetBool("Close", closeArms);	
	}

	/*--------------------------------------------------------------------------------
	|| つかむ処理
	--------------------------------------------------------------------------------*/
	private void Grab()
	{
		var hit = Physics2D.OverlapBox(GrabCenter, grabArea, 0.0f, grabMask);

		//	オブジェクトがなければ処理しない
		if (hit == null)
			return;

		//	Transformを保持する
		grabTarget = hit.transform;

		//	プレイヤーなら移動を止める
		if(hit.transform.tag == "Player")
		{
			PlayerMove player = hit.GetComponent<PlayerMove>();
			player.DisableInput = true;
			player.CantMove = true;
			player.SecondaryVelocity = Vector2.zero;
		}
		//	Rigidbodyがついていれば無効化する
		if(hit.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
		{
			rb.isKinematic = true;
			rb.velocity = Vector2.zero;
			rb.angularVelocity = 0.0f;
		}

		//	親がクレーンなら主導権を切り替える
		if (hit.transform.parent != null &&
			hit.transform.parent.parent != null &&
			hit.transform.parent.parent.TryGetComponent<CraneGimmick>(out CraneGimmick otherCrane))
		{
			this.grabParent = otherCrane.grabParent;
			this.grabTarget = otherCrane.grabTarget;

			otherCrane.grabParent = null;
			otherCrane.grabTarget = null;
		}
		else
		{
			//	親を保持しておく
			grabParent = hit.transform.parent;
		}

		//	親子関係を設定する
		hit.transform.parent = spriteRoot;
		//	座標を補正する
		hit.transform.localPosition = grabedOffset;
	}

	/*--------------------------------------------------------------------------------
	|| 離す処理
	--------------------------------------------------------------------------------*/
	private void Release()
	{
		//	つかんでいるオブジェクトがなければ処理しない
		if (grabTarget == null)
			return;

		//	親子関係を元に戻す
		grabTarget.parent = grabParent;

		//	Rigidbodyがついていれば有効化する
		if(grabTarget.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
		{
			rb.isKinematic = false;
		}
		//	プレイヤーなら元に戻す
		if (grabTarget.tag == "Player")
		{
			PlayerMove player = grabTarget.GetComponent<PlayerMove>();
			player.DisableInput = false;
			player.CantMove = false;
		}

		//	変数を初期化
		grabTarget = null;
		grabParent = null;
	}

	/*--------------------------------------------------------------------------------
	|| ワイヤーの更新処理
	--------------------------------------------------------------------------------*/
	private void WireUpdate()
	{
		var a = transform.rotation * (Vector3.right * ARM_X);
		lineRenderer.SetPosition(0, transform.position + a);
		lineRenderer.SetPosition(1, new Vector3(spriteRoot.position.x, spriteRoot.position.y));
	}

	/*--------------------------------------------------------------------------------
	|| ギミック固有の設定を取得
	--------------------------------------------------------------------------------*/
	public override string GetExtraSetting()
	{
		CraneSetting setting = new CraneSetting { speed = this.speed, rangeY = this.heightRange };
		return JsonUtility.ToJson(setting);
	}

	/*--------------------------------------------------------------------------------
	|| ギミック固有の設定を適応
	--------------------------------------------------------------------------------*/
	public override void SetExtraSetting(string json)
	{
		if (json == string.Empty)
			return;

		CraneSetting setting = JsonUtility.FromJson<CraneSetting>(json);

		this.speed = setting.speed;
		this.heightRange = setting.rangeY;
	}

	protected override void RemoveAction()
	{
		if (Sender == null)
			return;

		Sender.RemoveAction(buttonAction);
	}

	protected override void AddAction()
	{
		if (Sender == null)
			return;

		Sender.AddAction(buttonAction);
	}

	public void Pause()
	{
		isPause = true;
	}

	public void Resume()
	{
		isPause = false;

		//	Rigidbodyがついていれば無効化する
		if (grabTarget != null &&
			grabTarget.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
		{
			rb.isKinematic = true;
			rb.velocity = Vector2.zero;
			rb.angularVelocity = 0.0f;
		}
	}


#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = new Color(1, 0, 0, 0.5f);

		Gizmos.DrawCube(GrabCenter, grabArea);
	}
#endif
}
