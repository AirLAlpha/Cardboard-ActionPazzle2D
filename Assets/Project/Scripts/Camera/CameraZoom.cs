/**********************************************
 * 
 *  CameraZoom.cs 
 *  カメラのズーム処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/07
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[RequireComponent(typeof(Camera))]
public class CameraZoom : MonoBehaviour
{
	//	コンポーネント
	private new Camera			camera;                 //	自身のカメラ
	private PixelPerfectCamera	pixelPerfectCamera;

	//	ズーム
	[Header("ズーム")]
	[SerializeField]
	private bool			zoomIn;			//	ズームフラグ（有効：ズームイン）
	[SerializeField]
	private float			zoomSpeed;		//	ズーム速度
	[SerializeField]
	private float			zoomDiameter;   //	ズーム倍率
	[SerializeField]
	private Vector2			targetPos;		//	ズーム対象の座標
	[SerializeField]
	private Vector2			targetOffset;	//	ズーム対象のオフセット
	[SerializeField]
	private AnimationCurve	zoomEase;       //	ズームのイージング用カーブ
	[SerializeField]
	private Rect			zoomRect;		//	ズーム可能範囲

	private float			startDiameter;  //	開始時の倍率
	private Vector3			startPos;		//	開始時の座標
	private float			zoomProgress;	//	進行度

	//	プロパティ
	public bool			ZoomIn			{ get { return zoomIn; }		set { zoomIn = value; } }
	public float		ZoomDiameter	{ get { return zoomDiameter; }	set { zoomDiameter = value; } }
	public Vector3		TargetPos		{ get { return targetPos; }		set { targetPos = value; } }
	public Transform	Target			{ set { targetPos = value.position; } }

	//	実行前初期化処理
	private void Awake()
	{
		//	コンポーネントの取得
		camera = GetComponent<Camera>();
		pixelPerfectCamera = GetComponent<PixelPerfectCamera>();

		//	開始時のサイズを取得
		startDiameter = camera.orthographicSize;
		//	開始時の座標を取得
		startPos = transform.position;
	}

	//	初期化処理
	private void Start()
	{
		
	}

	//	更新処理
	private void Update()
	{
		ZoomUpdate();       //	ズーム処理	
		ZoomClump();		//	ズーム中の座標制限
	}

	/*--------------------------------------------------------------------------------
	|| ズーム処理
	--------------------------------------------------------------------------------*/
	private void ZoomUpdate()
	{
		//	ズームの進行方向（1：ズームイン、-1：ズームアウト）
		int zoomMod = zoomIn ? 1 : -1;

		//	ズームを進行させる
		zoomProgress += Time.unscaledDeltaTime * zoomSpeed * zoomMod;
		//	01でクランプする
		zoomProgress = Mathf.Clamp01(zoomProgress);

		//	ズーム倍率が0より大きいときはピクセルパーフェクトを無効化
		pixelPerfectCamera.enabled = zoomProgress > 0 ? false : true;

		float t = zoomEase.Evaluate(zoomProgress);

		//	カメラのサイズを変更する
		float dia = Mathf.Lerp(startDiameter, zoomDiameter, t);
		camera.orthographicSize = dia;

		//	カメラの座標を変更する
		Vector3 pos = 
			Vector3.Lerp(startPos, startPos + new Vector3(targetPos.x + targetOffset.x, targetPos.y + targetOffset.y), t);
		transform.position = pos;
	}

	/*--------------------------------------------------------------------------------
	|| ズーム中の座標制限
	--------------------------------------------------------------------------------*/
	private void ZoomClump()
	{
		//	画面の左下と右上のワールド座標を取得
		Vector3 p1 = camera.ViewportToWorldPoint(Vector2.zero);
		Vector3 p2 = camera.ViewportToWorldPoint(Vector2.one);
		//	現在の座標
		Vector3 pos = transform.position;

		//	カメラの端が範囲外なら押し戻す
		if (p1.x < zoomRect.xMin)
			pos.x += zoomRect.xMin - p1.x;
		if (p2.x > zoomRect.xMax)
			pos.x += zoomRect.xMax - p2.x;
		if (p1.y < zoomRect.yMin)
			pos.y += zoomRect.yMin - p1.y;
		if (p2.y > zoomRect.yMax)
			pos.y += zoomRect.yMax - p2.y;
		//	座標を適応
		transform.position = pos;
	}

	/*--------------------------------------------------------------------------------
	|| ズームの開始
	--------------------------------------------------------------------------------*/
	public void StartZoom(Transform target)
	{
		targetPos = target.position;

		zoomIn = true;
	}

	/*--------------------------------------------------------------------------------
	|| ギズモの描画処理
	--------------------------------------------------------------------------------*/
	private void OnDrawGizmosSelected()
	{
		Color col = Color.red;
		col.a = 0.5f;
		Gizmos.color = col;

		Gizmos.DrawCube(zoomRect.center, zoomRect.size);
	}

}
