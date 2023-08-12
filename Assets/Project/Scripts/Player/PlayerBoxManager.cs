/**********************************************
 * 
 *  PlayerBoxManager.cs 
 *  プレイヤーのハコの生成と管理の処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/02
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardboardBox;
using System.Transactions;

[RequireComponent(typeof(PlayerMove))]
[RequireComponent(typeof(PlayerDamageReciver))]
[RequireComponent(typeof(SoundPlayer))]
public class PlayerBoxManager : MonoBehaviour, IPauseable
{
	//	コンポーネント
	private PlayerMove				playerMove;         //	PlayerMove
	private PlayerDamageReciver		playerDamageReciver;
	private StageManager			stageManager;
	private SoundPlayer				soundPlayer;

	//	入力
	[SerializeField]
	private ButtonHint		buttonHint;			//	ボタンの表示されるアクション名を変更

	public bool				DisableInput { set; get; }		//	入力の無効フラグ

	private bool			inputGenerate;      //	生成
	private bool			inputPut;			//	設置

	//	生成
	[Header("生成")]
	[SerializeField]
	private CardboardBox.CardboardBox	boxOriginal;		//	生成するPrefabのオリジナル
	[SerializeField]
	private Vector2			generatePos;        //	生成する座標（ローカル）
	[SerializeField]
	private Transform		generateParent;     //	生成時の親

	//	管理
	private CardboardBox.CardboardBox currentBox;         //	所持しているハコ

	public CardboardBox.CardboardBox CurrentBox { get { return currentBox; } }

	//	設置
	[Header("設置")]
	[SerializeField]
	private float			putSpeed;			//	箱を設置する速度
	[SerializeField]
	private Vector2			putOffset;          //	設置の際のオフセット
	[SerializeField]
	private Vector2			putCheckRange;      //	箱を設置する際の確認範囲
	[SerializeField]
	private LayerMask		putCheckLayer;      //	設置確認用レイヤー
	[SerializeField]
	private LayerMask		throwCheckLayer;

	private bool			isPuting;           //	設置中フラグ
	private Vector2			putStartPos;		//	設置の開始座標
	private Vector2			putTargetPos;       //	箱の設置先座標
	private float			putProgress;        //	設置の進行度

	//	投げる
	[Header("投げ")]
	[SerializeField]
	private Vector2			throwPower;         //	投げる力
	[SerializeField]
	private float			throwingVelocityX;	//	投げになる速度（X)


	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		playerMove = GetComponent<PlayerMove>();
		playerDamageReciver = GetComponent<PlayerDamageReciver>();
		stageManager = StageManager.Instance;
		soundPlayer = GetComponent<SoundPlayer>();

		//	Nullチェック
		if (boxOriginal == null)
			Debug.LogError("ハコのPrefabが設定されていません。");
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		//	死亡 or クリアしていたら処理を辞める
		if (playerDamageReciver.IsDead ||
			stageManager.IsStageClear)
			return;

		InputUpdate();      //	入力処理

		if (inputGenerate)
			GenerateBox();

		if (inputPut)
		{
			if (Mathf.Abs(playerMove.Rigidbody2D.velocity.x) < throwingVelocityX)
			{
				TryPut();
			}
			else
			{
				TryThrow();
			}
		}

		//	箱の設置処理
		if (isPuting && currentBox != null)
			PutingBoxUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| 入力処理
	--------------------------------------------------------------------------------*/
	private void InputUpdate()
	{
		if (DisableInput)
			return;

		inputGenerate = 
			Input.GetKeyDown(KeyCode.R) ||
			Input.GetButtonDown("Fire1");				//	生成


		inputPut = 
			currentBox != null &&
			(Input.GetKeyDown(KeyCode.R) ||
			Input.GetButtonDown("Fire1"));				//	設置
	}

	/*--------------------------------------------------------------------------------
	|| 生成処理
	--------------------------------------------------------------------------------*/
	[ContextMenu("GenerateBox")]
	private void GenerateBox()
	{
		//	すでにハコを所持しているときは処理しない
		if (currentBox != null)
			return;

		//	ステージのハコ残数を確認
		//	残数がないときは処理しない
		if (!StageManager.Instance.CheckRemainingBoxes())
			return;

		//	新しいハコの生成
		var newBox = Instantiate(
			boxOriginal, 
			generateParent.TransformPoint(generatePos),
			Quaternion.identity, 
			generateParent);

		//	Rigidbodyを無効化
		newBox.SetRigidbodyActive(false);
		//	SEのソースを設定
		newBox.SetSoundPlayer(soundPlayer);
		//	所持しているハコに設定
		currentBox = newBox;

		//	SEの再生
		soundPlayer.PlaySound(6);

		//	ハコ使用数を加算
		StageManager.Instance.UsedBoxCount++;

		//	アクションの表示名を変更
		if (buttonHint != null)
			buttonHint.SetDisplayNameIndex("Fire1", 1);
	}

	/*--------------------------------------------------------------------------------
	|| 箱の設置前確認処理
	--------------------------------------------------------------------------------*/
	private bool TryPut()
	{
		//	箱を持っていないときは処理しない
		if (currentBox == null)
			return false;

		//	プレイヤーの向きを取得
		Direction dir = playerMove.CurrentDir;
		//	置こうとしているオフセット（ローカル）を作成
		Vector3 offset = new Vector3(putOffset.x * (int)dir, putOffset.y);
		//	置こうとしている場所にオブジェクトがあるか確認する
		var hit = Physics2D.OverlapBox(currentBox.transform.position + offset, putCheckRange, 0, putCheckLayer);

		if(hit != null)
		{
			//	梱包可能オブジェクトがあった時
			if (hit.TryGetComponent<IPackable>(out IPackable packable))
			{
				//	梱包を行う
				CardboardType type = packable.Packing();
				currentBox.Packing(type, packable);

				//	敵の座標に設置する際は以下のコメントを解除する	//
				//var pos = generateParent.InverseTransformPoint(hit.transform.position);
				//PutBox(pos);        //	設置
				//return true;
			}
			//	それ以外の時
			else
			{
				return false;
			}
		}

		PutBox(offset);     //	設置
		return true;
	}

	/*--------------------------------------------------------------------------------
	|| 設置処理
	--------------------------------------------------------------------------------*/
	private void PutBox(Vector2 putPos)
	{
		//	ハコを所持していないときは処理しない
		if (currentBox == null)
			return;

		//	設置の進行度をリセット
		putProgress = 0.0f;

		//	箱を設置中に設定
		isPuting = true;

		//	地上にいる時
		if (playerMove.IsGrounded)
		{
			//	設置の開始座標を設定
			this.putStartPos = this.generatePos;
			//	設置先座標を設定
			this.putTargetPos = putPos;
		}
		//	空中にいる時
		else
		{
			//	設置の開始座標（ワールド）を設定
			this.putStartPos = generateParent.TransformPoint(generatePos);
			//	設置先座標（ワールド）を設定
			this.putTargetPos = generateParent.TransformPoint(putPos);
			//	箱にベクトルを加算
			currentBox.Rigidbody2D.velocity = Vector2.down * putSpeed * 0.5f;
			//	親子関係を解除
			currentBox.transform.parent = null;
		}

		//	移動SEの再生
		soundPlayer.PlaySound(3);

		//	アクションの表示名を変更
		if (buttonHint != null)
			buttonHint.SetDisplayNameIndex("Fire1", 0);
	}

	/*--------------------------------------------------------------------------------
	|| 箱を投げる前の確認処理
	--------------------------------------------------------------------------------*/
	private bool TryThrow()
	{
		var hit = Physics2D.OverlapBox(currentBox.transform.position, Vector2.one, 0.0f, throwCheckLayer);
		if(hit)
		{
			return false;
		}

		ThrowBox();
		return true;
	}

	/*--------------------------------------------------------------------------------
	|| 箱を投げる処理
	--------------------------------------------------------------------------------*/
	private void ThrowBox()
	{
		const float PLAYER_VELOCITY_MOD = 3.0f;		//	プレイヤーのVelocityに乗算する値

		//	親子関係を解除
		currentBox.transform.parent = null;

		//	力を加える
		currentBox.SetRigidbodyActive(true);
		Vector2 throwVel = new Vector2(throwPower.x * (int)playerMove.CurrentDir, throwPower.y);
		throwVel += Vector2.right * playerMove.Rigidbody2D.velocity.x * PLAYER_VELOCITY_MOD;
		currentBox.Rigidbody2D.AddForce(throwVel, ForceMode2D.Impulse);
		currentBox.Rigidbody2D.angularVelocity = 0;

		//	形を戻す
		currentBox.transform.localScale = Vector3.one;
		//	持っている箱を解除
		currentBox = null;

		//	移動SEの再生
		soundPlayer.PlaySound(1);

		//	アクションの表示名を変更
		if (buttonHint != null)
			buttonHint.SetDisplayNameIndex("Fire1", 0);
	}

	/*--------------------------------------------------------------------------------
	|| 箱を設置する更新処理
	--------------------------------------------------------------------------------*/
	private void PutingBoxUpdate()
	{
		//	設置中でない or 箱を持っていないときは処理しない
		if (!isPuting ||
			currentBox == null)
			return;

		//	設置の進行度を進行させる
		putProgress = Mathf.Clamp01(putProgress + Time.deltaTime * putSpeed);

		//	進行度が 1.0 を超えたら設置完了とする
		if (putProgress >= 1.0f)
		{
			//	設置中を解除する
			isPuting = false;

			//	座標を設定
			currentBox.transform.localPosition = putTargetPos;
			//	進行度をリセット
			putProgress = 0.0f;

			//	箱のRigidbodyを有効化
			currentBox.SetRigidbodyActive(true);
			//	ハコの親子関係を削除
			currentBox.transform.parent = null;
			//	スケールを初期化
			currentBox.transform.localScale = Vector3.one;
			//	手持ちから解除
			currentBox = null;

			return;
		}

		//	進行度で座標を補完する
		float x = Mathf.Lerp(putStartPos.x, putTargetPos.x, putProgress);
		float y = Mathf.Lerp(putStartPos.y, putTargetPos.y, EasingFunctions.EaseInExpo(putProgress));
		//	座標を更新する
		currentBox.transform.localPosition = new Vector3(x, y);
		currentBox.transform.localScale = Vector3.one;
	}

	/*--------------------------------------------------------------------------------
	|| ポーズ処理
	--------------------------------------------------------------------------------*/
	public  void Pause()
	{
		//	入力を無効化
		DisableInput = true;
	}
	/*--------------------------------------------------------------------------------
	|| 再開処理
	--------------------------------------------------------------------------------*/
	public  void Resume()
	{
		//	入力を有効化
		DisableInput = false;
	}

#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (currentBox == null)
			return;

		var col = Color.yellow;
		col.a = 0.5f;
		Gizmos.color = col;

		//	プレイヤーの向きを取得
		Direction dir = playerMove.CurrentDir;
		//	置こうとしているオフセット（ローカル）を作成
		Vector3 offset = new Vector3(putOffset.x * (int)dir, putOffset.y);
		Gizmos.DrawCube(currentBox.transform.position + offset, putCheckRange);
	}
#endif
}
