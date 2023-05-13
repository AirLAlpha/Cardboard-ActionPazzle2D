/**********************************************
 * 
 *  BackgroundScroll.cs 
 *  背景のスクロールに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/06
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour
{
	//	コンポーネント
	private Camera cam;

	//	スクロール
	[Header("スクロール")]
	[SerializeField]
	private Transform scrollTargets;    //	スクロール対象
	[SerializeField]
	private float scrollSpeed;			//	スクロール速度
	[SerializeField]
	private float scrollMax;            //	最大スクロール量
	[SerializeField]
	private float scrollMin;            //	最小スクロール量
	[SerializeField]
	private float returnOffsetX;        //	折り返し時のオフセット
	[SerializeField]
	private bool ignoreY;				//	Y軸を無視するフラグ


	private Vector3 saveCameraPos;      //	前回処理時のカメラ座標

	//	実行前初期化処理
	private void Awake()
	{
		//	メインカメラの取得
		cam = Camera.main;
	}

	//	初期化処理
	private void Start()
	{
		//	カメラ座標の初期化
		saveCameraPos = cam.transform.position;
	}

	//	更新処理
	private void Update()
	{
		//	カメラの座標を取得する
		Vector3 pos = cam.transform.position;
		if (ignoreY)
			pos.y = 0.0f;
		//	自身をカメラの座標と重ねる
		transform.position = pos;

		//	X軸の移動量をスクロール量として取得
		float scrollValue = scrollTargets.localPosition.x;

		//	最大値を超えたら最小値まで戻す
		if (scrollValue > scrollMax)
		{
			scrollTargets.localPosition = Vector3.right * (scrollMin + returnOffsetX);
		}
		//	最小値を下回ったら最大値まで動かす
		else if (scrollValue < scrollMin)
		{
			scrollTargets.localPosition = Vector3.right * (scrollMax - returnOffsetX);
		}
		//	差分に速度を乗算し加算する
		else
		{
			//	差分（移動量を計算）
			Vector3 sub = pos - saveCameraPos;
			if (ignoreY)
				sub.y = 0.0f;

			scrollTargets.localPosition += sub * Time.deltaTime * scrollSpeed;
		}

		//	カメラの座標を保持する
		saveCameraPos = pos;
	}
}
