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
using Unity.VisualScripting;
using UnityEngine;

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

	
	[Header("カメラ")]
	[SerializeField]
	private Camera		editorCamera;   //	エディタ用のカメラ
	[SerializeField]
	private float		inPlayZoom;     //	プレイモード時のズーム倍率
	[SerializeField]
	private float		inEditZoom;     //	編集モード時のズーム倍率
	[SerializeField]
	private float		zoomSpeed;      //	ズーム速度


	//	実行前初期化処理
	private void Awake()
	{

	}

	//	初期化処理
	private void Start()
	{
		//	メインとなるカメラを非アクティブに変更する
		Camera.main.gameObject.SetActive(false);
	}

	//	更新処理
	private void Update()
	{
		EditCameraUpdate();     //	エディター用カメラのズーム処理

#if UNITY_EDITOR
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			var nextMode = currentMode == EditorMode.EDIT ? EditorMode.PLAY : EditorMode.EDIT;
			ChangeMode(nextMode);
		}
#endif
	}


	/*--------------------------------------------------------------------------------
	|| モードの切替処理
	--------------------------------------------------------------------------------*/
	public void ChangeMode(EditorMode mode)
	{
		//	モードを書き換える
		currentMode = mode;
	}

	/*--------------------------------------------------------------------------------
	|| エディター用カメラのズーム処理
	--------------------------------------------------------------------------------*/
	private void EditCameraUpdate()
	{
		float targetZoom = 0.0f;
		if (currentMode == EditorMode.EDIT)
			targetZoom = inEditZoom;
		else
			targetZoom = inPlayZoom;


		editorCamera.orthographicSize = Mathf.Lerp(editorCamera.orthographicSize, targetZoom, Time.deltaTime * zoomSpeed);
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
