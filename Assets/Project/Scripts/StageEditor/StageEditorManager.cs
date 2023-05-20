/**********************************************
 * 
 *  StageEditorManager.cs 
 *  ステージエディター全般の処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/17
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class StageEditorManager : MonoBehaviour
{
	//	エディターモード
	public enum EditorMode
	{
		EDIT,		//	エディタ
		PLAY,		//	プレイ
	}
	[SerializeField]
	private EditorMode currentMode;     //	現在のモード

	//	コンポーネント
	[Header("コンポーネント")]
	[SerializeField]
	private PauseManager		poseManager;
	[SerializeField]
	private StageObjectPallet	pallet;
	[SerializeField]
	private StageEditorMenu editorMenu;
	[SerializeField]
	private StageDetailMenu detailMenu;
	[SerializeField]
	private BackgroundSetter bgSetter;
	[SerializeField]
	private GimmickSettings	gimmickSetting;
	[SerializeField]
	private ErrorMessage	errorMessage;
	[SerializeField]
	private CanvasAlphaController	editorUIAlphaController;
	[SerializeField]
	private CanvasAlphaController	playUIAlphaController;
	[SerializeField]
	private PlayerDamageReciver		playerDamageReciver;

	private StagePainter	painter;
	private StageExporter	exporter;
	private StageLoader		loader;
	private StageManager	stageManager;

	[Header("カメラ")]
	[SerializeField]
	private Camera		editorCamera;   //	エディタ用のカメラ
	[SerializeField]
	private float		inPlayZoom;     //	プレイモード時のズーム倍率
	[SerializeField]
	private float		inEditZoom;     //	編集モード時のズーム倍率
	[SerializeField]
	private float		zoomSpeed;      //	ズーム速度

	[Header("背景")]
	[SerializeField]
	private Transform	backgroundImage;
	[SerializeField]
	private float		inPlayScale;
	[SerializeField]
	private float		inEditScale;

	//	タイルマップ
	[Header("タイルマップ")]
	[SerializeField]
	private Tilemap baseTilemap;
	[SerializeField]
	private Tilemap stageTilemap;

	//	ステージリセット
	[Header("リセット")]
	[SerializeField]
	private string[]	onResetRemoveTags;      //	ステージの初期化時に削除するオブジェクトのタグ

	[Header("グリッド")]
	[SerializeField]
	private SpriteAlphaController gridAlphaController;

	[Header("編集")]
	[SerializeField]
	private string		tempStageFileName;      //	エディット中のステージを保持するファイル名

	//	メニュー
	private bool		menuStateChanged;


	//	実行前初期化処理
	private void Awake()
	{
		this.painter = GetComponent<StagePainter>();
		this.loader = GetComponent<StageLoader>();
		this.exporter = GetComponent<StageExporter>();
	}

	//	初期化処理
	private void Start()
	{
		//	ギミックの初期化のみ行う
		loader.InitReveiceGimmicks();

		//	ステージマネージャーの取得
		stageManager = StageManager.Instance;
		//	リスタートを無効化
		stageManager.DisableRestart = true;

		//	モードの初期化
		ChangeMode(currentMode);
	}

	//	更新処理
	private void Update()
	{
		//	カーソルを非表示に設定
		Cursor.visible = false;

		EditCameraUpdate();     //	エディター用カメラのズーム処理

		if (!editorMenu.IsActive &&
			!detailMenu.IsActive && 
			!gimmickSetting.IsActive &&
			Input.GetButtonDown("Restart"))
		{
			var nextMode = currentMode == EditorMode.EDIT ? EditorMode.PLAY : EditorMode.EDIT;
			ChangeMode(nextMode);
		}

		if (currentMode != EditorMode.PLAY)
		{
			EditorMenuActivate();
			DetailMenuActivate();
		}
	}

	/*--------------------------------------------------------------------------------
	|| エディターメニューの処理
	--------------------------------------------------------------------------------*/
	private void EditorMenuActivate()
	{
		if (detailMenu.IsActive)
			return;

			bool menuActive = editorMenu.IsActive;
		if (Input.GetButtonDown("Menu"))
			menuActive = !menuActive;

		if (editorMenu.IsActive != menuActive)
		{
			if (menuActive)
			{
				painter.Disable();
			}
			else 
			{
				painter.Enable();
			}

			//	メニューのアクティブを設定
			editorMenu.IsActive = menuActive;
		}

	}

	/*--------------------------------------------------------------------------------
	|| 詳細メニューの処理
	--------------------------------------------------------------------------------*/
	private void DetailMenuActivate()
	{
		if (editorMenu.IsActive)
			return;

			bool menuActive = detailMenu.IsActive;
		if (Input.GetButtonDown("Menu2"))
			menuActive = !menuActive;

		if (detailMenu.IsActive != menuActive)
		{
			if (menuActive)
			{
				detailMenu.UsableBoxCount = stageManager.UsableBoxCount;
				detailMenu.TargetBoxCount = stageManager.TargetBoxCount;
				detailMenu.BackgroundType = bgSetter.Index;
				painter.Disable();
			}
			else 
			{
				stageManager.UsableBoxCount = detailMenu.UsableBoxCount;
				stageManager.TargetBoxCount = detailMenu.TargetBoxCount;
				painter.Enable();
			}

			//	メニューのアクティブを設定
			detailMenu.IsActive = menuActive;
			//	メニューのテキストを更新する
			detailMenu.CounterTextUpdateAll();
		}

		//	背景の設定を更新する
		if (detailMenu.IsActive &&
			bgSetter.Index != detailMenu.BackgroundType)
		{
			bgSetter.SetBackground(detailMenu.BackgroundType);
		}
	}

	/*--------------------------------------------------------------------------------
	|| モードの切替処理
	--------------------------------------------------------------------------------*/
	public void ChangeMode(EditorMode mode)
	{
		//	プレイモードに変更するときにギミックが正しく設定されていなければ処理しない
		if (mode == EditorMode.PLAY)
		{
			if (!painter.CheckConnectedGimmick())
			{
				errorMessage.DispErrorMessage("There are unconnected gimmicks");
				Debug.LogError("未接続のギミックがあります。");
				return;
			}
		}

		//	プレイヤーが死亡 or ステージクリア演出時は処理しない
		if (playerDamageReciver.IsDead ||
			stageManager.IsStageClear)
			return;


		//	モードを書き換える
		currentMode = mode;

		//	モード変更時の初期化処理を行う
		switch (mode)
		{
			case EditorMode.EDIT:
				InitEditMode();
				break;

			case EditorMode.PLAY:
				InitPlayMode();
				break;
		}

	}

	/*--------------------------------------------------------------------------------
	|| エディター用カメラのズーム処理
	--------------------------------------------------------------------------------*/
	private void EditCameraUpdate()
	{
		float targetZoom = 0.0f;
		float targetScale = 0.0f;
		if (currentMode == EditorMode.EDIT)
		{
			targetZoom = inEditZoom;
			targetScale = inEditScale;
		}
		else
		{
			targetZoom = inPlayZoom;
			targetScale = inPlayScale;
		}

		//	カメラに徐々にズームを適応する
		editorCamera.orthographicSize = Mathf.Lerp(editorCamera.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
		//	背景に大きさを適応
		backgroundImage.localScale = Vector3.Lerp(backgroundImage.localScale, Vector3.one * targetScale, Time.deltaTime * zoomSpeed);

		if (Mathf.Abs(editorCamera.orthographicSize - inPlayZoom) < 0.05f)
			editorCamera.gameObject.SetActive(false);
		else 
			editorCamera.gameObject.SetActive(true);
	}

	/*--------------------------------------------------------------------------------
	|| 編集モード時の初期化
	--------------------------------------------------------------------------------*/
	private void InitEditMode()
	{
		//	ステージの再読み込み
		loader.LoadStageFromJson(tempStageFileName);
		//	ポーズの有効化
		poseManager.Pause();

		//	オブジェクトの削除
		foreach (var tag in onResetRemoveTags)
		{
			GameObject[] obj = GameObject.FindGameObjectsWithTag(tag);

			foreach (var item in obj)
			{
				Destroy(item);
			}
		}

		//	タイルマップのCompositeを無効化
		if(baseTilemap.TryGetComponent<TilemapCollider2D>(out TilemapCollider2D baseCollider))
		{
			baseCollider.usedByComposite = false;
		}
		if (stageTilemap.TryGetComponent<TilemapCollider2D>(out TilemapCollider2D stageCollider))
		{
			stageCollider.usedByComposite = false;
		}


		//	グリッドの透明度を設定
		gridAlphaController.TargetAlpha = 1.0f;
		pallet.IsActive = true;

		//	タイルの設置に関する処理を初期化
		painter.Enable();

		//	エラーメッセージオブジェクトの有効化
		errorMessage.gameObject.SetActive(true);

		//	プレイヤーの無敵を有効化
		playerDamageReciver.DontDeath = true;

		//	プレイUIの非表示
		playUIAlphaController.TargetAlpha = 0.0f;
		//	エディターUIの表示
		editorUIAlphaController.TargetAlpha = 1.0f;
	}

	/*--------------------------------------------------------------------------------
	|| プレイモード時の初期化
	--------------------------------------------------------------------------------*/
	private void InitPlayMode()
	{
		//	エラーメッセージオブジェクトの無効化
		errorMessage.gameObject.SetActive(false);

		//	仮の状態を書き出す
		exporter.ExportJson(tempStageFileName);

		//	タイルマップのCompositeを有効化
		if (baseTilemap.TryGetComponent<TilemapCollider2D>(out TilemapCollider2D baseCollider))
		{
			baseCollider.usedByComposite = true;
		}
		if (stageTilemap.TryGetComponent<TilemapCollider2D>(out TilemapCollider2D stageCollider))
		{
			stageCollider.usedByComposite = true;
		}

		//	タイルの設置に関する処理を初期化
		painter.Disable();

		//	ステージの再読み込み
		loader.LoadStageFromJson(tempStageFileName);

		//	グリッドの透明度を設定
		gridAlphaController.TargetAlpha = 0.0f;
		pallet.IsActive = false;

		//	ポーズの解除
		poseManager.Resume();

		//	プレイヤーの無敵を解除
		playerDamageReciver.DontDeath = false;

		//	エディターUIの非表示
		editorUIAlphaController.TargetAlpha = 0.0f;
		//	プレイUIの表示
		playUIAlphaController.TargetAlpha = 1.0f;
	}

	/*--------------------------------------------------------------------------------
	|| ステージエディタの終了処理
	--------------------------------------------------------------------------------*/
	public void ExitEditor()
	{
		//	Tempファイルの削除
		exporter.DeleteFile(tempStageFileName);
		//	シーン遷移
		stageManager.ReturnTitle();
	}

#if UNITY_EDITOR
	[ContextMenu("Change Edit ")]
	public void ChangeModeEdit()
	{
		ChangeMode(EditorMode.EDIT);
	}
	[ContextMenu("Change Play ")]
	public void ChangeModePlay()
	{
		ChangeMode(EditorMode.PLAY);
	}

#endif

}
