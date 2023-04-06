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

[RequireComponent(typeof(PlayerMove))]
public class PlayerBoxManager : MonoBehaviour
{
	//	コンポーネント
	private PlayerMove		playerMove;         //	PlayerMove

	//	入力
	[SerializeField]
	private ButtonHint		buttonHint;			//	ボタンの表示されるアクション名を変更

	public bool				DisableInput { set; get; }		//	入力の無効フラグ

	private bool			inputGenerate;      //	生成
	private bool			inputPut;			//	設置

	//	生成
	[Header("生成")]
	[SerializeField]
	private CardboardBox	boxOriginal;		//	生成するPrefabのオリジナル
	[SerializeField]
	private Vector2			generatePos;        //	生成する座標（ローカル）
	[SerializeField]
	private Transform		generateParent;     //	生成時の親

	//	管理
	private CardboardBox	currentBox;         //	所持しているハコ

	//	設置
	[Header("設置")]
	[SerializeField]
	private Vector2			putOffset;          //	設置の際のオフセット

	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		playerMove = GetComponent<PlayerMove>();

		//	Nullチェック
		if (boxOriginal == null)
			Debug.LogError("ハコのPrefabが設定されていません。");

		//	アクションの登録

	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		InputUpdate();      //	入力処理

		if (inputGenerate)
			GenerateBox();

		if (inputPut)
			PutBox();
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
		var newObj = Instantiate(
			boxOriginal, 
			generateParent.TransformPoint(generatePos),
			Quaternion.identity, 
			generateParent);

		//	Rigidbodyを無効化
		newObj.SetRigidbodyActive(false);
		//	所持しているハコに設定
		currentBox = newObj;

		//	ハコ使用数を加算
		StageManager.Instance.UsedBoxCount++;

		//	アクションの表示名を変更
		buttonHint.SetDisplayNameIndex("Fire1", 1);
	}

	/*--------------------------------------------------------------------------------
	|| 設置処理
	--------------------------------------------------------------------------------*/
	[ContextMenu("PutBox")]
	private void PutBox()
	{
		//	ハコを所持していないときは処理しない
		if (currentBox == null)
			return;

		//	プレイヤーの現在の向きを取得
		Direction dir = playerMove.CurrentDir;
		//	ハコのローカル座標を設定
		if (!currentBox.TryPut(new Vector2(putOffset.x * (int)dir, putOffset.y)))
			return;
		//	Rigidbodyを有効化
		currentBox.SetRigidbodyActive(true);
		//	ハコの親子関係を削除
		currentBox.transform.parent = null;
		//	手持ちから解除
		currentBox = null;

		//	アクションの表示名を変更
		buttonHint.SetDisplayNameIndex("Fire1", 0);
	}
}
