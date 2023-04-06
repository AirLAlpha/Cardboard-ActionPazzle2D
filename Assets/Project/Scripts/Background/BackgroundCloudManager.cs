/**********************************************
 * 
 *  BackgroundCloudManager.cs 
 *  背景の雲に関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/06
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundCloudManager : MonoBehaviour
{
	[System.Serializable]
	public struct Cloud
	{
		public Transform	transform;		//	Transform
		public float		speed;          //	移動速度

		//	コンストラクタ
		public Cloud(Transform transform, float speed)
		{
			this.transform = transform;
			this.speed = speed;
		}
	}


	//	雲
	[Header("雲")]
	[SerializeField]
	private Transform[]		cloudPrefabs;		//	雲のスプライト
	[SerializeField]
	private Transform		cloudRoot;          //	雲の親となるTransform
	[SerializeField]
	private float			cloudMinSpeed;		//	雲の最低速度
	[SerializeField]
	private float			cloudMaxSpeed;      //	雲の最高速度
	[SerializeField]
	private float			cloudMaxScale;		//	雲の最大スケール
	[SerializeField]
	private float			cloudMinScale;		//	雲の最小スケール

	[SerializeField]
	private int				cloudCount;			//	雲の数
	[SerializeField]
	private Rect			cloudArea;          //	雲の生成範囲

	private Cloud[]			cloudArray;			//	雲の配列

#if UNITY_EDITOR
	[Header("デバッグ")]
	[SerializeField]
	private bool			drawGizmos;         //	ギズモの描画フラグ
#endif

	//	実行前初期化処理
	private void Awake()
	{
		
	}

	//	初期化処理
	private void Start()
	{
		InitCloud();
	}

	//	更新処理
	private void Update()
	{
		CloudUpdate();	
	}

	/*--------------------------------------------------------------------------------
	|| 雲の初期化処理
	--------------------------------------------------------------------------------*/
	private void InitCloud()
	{
		cloudArray = new Cloud[cloudCount];
		for (int i = 0; i < cloudArray.Length; i++)
		{
			//	生成する雲の種類を決める
			int cloudNum = Random.Range(0, cloudPrefabs.Length);
			//	範囲内のランダムな座標を作成
			float x = Random.Range(cloudArea.xMin, cloudArea.xMax);
			float y = Random.Range(cloudArea.yMin, cloudArea.yMax);
			Vector2 pos = new Vector2(x, y);
			//	スケールの作成
			float scale = Random.Range(cloudMinScale, cloudMaxScale);

			//	インスタンスの作成
			var instance = Instantiate(cloudPrefabs[cloudNum], pos, Quaternion.identity, cloudRoot);
			//	スケールを適応
			instance.transform.localScale *= scale;

			//	速度を作成
			float speed = Random.Range(cloudMinSpeed, cloudMaxSpeed);

			//	配列に情報を格納
			cloudArray[i].transform = instance.transform;
			cloudArray[i].speed		= speed;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 雲の更新処理
	--------------------------------------------------------------------------------*/
	private void CloudUpdate()
	{
		foreach (var cloud in cloudArray)
		{
			cloud.transform.localPosition += Vector3.left * cloud.speed * Time.deltaTime;

			if (cloud.transform.localPosition.x <= cloudArea.xMin)
				cloud.transform.localPosition = new Vector3(cloudArea.xMax, cloud.transform.localPosition.y, 10.0f);
		}
	}

#if UNITY_EDITOR
	/*--------------------------------------------------------------------------------
	|| ギズモの描画処理
	--------------------------------------------------------------------------------*/
	private void OnDrawGizmosSelected()
	{
		if (!drawGizmos)
			return;

		//	雲の生成範囲を描画
		Gizmos.color = Color.red;

		Gizmos.DrawCube(new Vector3(cloudArea.center.x, cloudArea.center.y), new Vector3(cloudArea.width, cloudArea.height));
		
	}
#endif

}
