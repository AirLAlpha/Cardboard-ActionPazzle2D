/**********************************************
 * 
 *  StageManagere.cs 
 *  ステージ全体の管理をする処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/02
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
	//	インスタンス
	private static StageManager instance;       //	インスタンス
	public static StageManager Instance			//	インスタンスを取得するプロパティ
	{
		get
		{
			if (instance == null)
			{
				Debug.LogError("StageManagerがアタッチされたオブジェクトはありません。");
				return null;
			}
			else
			{
				return instance;
			}
		}
	}

	//	ステージデータベース
	[Header("DataBase")]
	[SerializeField]
	private StageDataBase stageDataBase;

	//	段ボール
	public int UsableBoxCount	{ get; set; }							//	使用可能なハコの数
	public int UsedBoxCount		{ get; set; }							//	使用した箱の数
	public int RemainingBoxCount => UsableBoxCount - UsedBoxCount;      //	残り使用可能な箱の数

	private int saveRemainingBoxCount;                                   //	前回処理時の残り使用可能な箱の数

	//	アクション
	public System.Action OnChangedRemainingBoxCount { get; set; }		//	残りの箱の数が更新されたときに呼び出される
	public System.Action OnNonRemaining				{ get; set; }		//	箱の残りがなかったときに呼び出される

	//	実行前初期化処理
	private void Awake()
	{
		InitInstance();     //	シングルトンインスタンスの初期化
		InitStage();		//	ステージの初期化を行う
	}

	//	初期化処理
	private void Start()
	{
	}

	//	更新処理
	private void Update()
	{
		CheckRemainingBoxCount();
	}

	/*--------------------------------------------------------------------------------
	|| シングルトンインスタンスの初期化
	--------------------------------------------------------------------------------*/
	private void InitInstance()
	{
		//	すでに自分自身がアタッチされているときは処理しない
		if (instance == this)
		{
			return;
		}
		//	インスタンスが設定されていないときは自身を設定する
		else if (instance == null)
		{
			instance = this;
			return;
		}

		//	すでに別のインスタンスが設定されているときは自身を破棄する
		Destroy(this);
	}

	/*--------------------------------------------------------------------------------
	|| ステージの初期化
	--------------------------------------------------------------------------------*/
	private void InitStage()
	{
		//	ステージデータベースのNullチェック
		if (stageDataBase == null)
		{
			Debug.LogError("ステージデータベースが設定されていません。");
			return;
		}

		//	シーン名を取得
		int loadedSceneCount = SceneManager.sceneCount;
		string baseSceneName = SceneManager.GetActiveScene().name;
		string stageSceneNamae = baseSceneName;
		//for(int i = 0; i<loadedSceneCount; i++)
		//{
		//	string sceneName = SceneManager.GetSceneAt(i).name;
		//	if (sceneName == baseSceneName)
		//		continue;

		//	stageSceneNamae = sceneName;
		//	break;
		//}

		//	シーン名からステージIDを取得
		int stageId = int.Parse(stageSceneNamae.Replace("Stage_", ""));
		//	データベース内のステージIDから検索
		StageData findResult = stageDataBase.StageList.Find(n => n.StageID == stageId);

		//	変数を保持
		UsableBoxCount = findResult.UsableBoxCount;
		saveRemainingBoxCount = -1;
	}

	/*--------------------------------------------------------------------------------
	|| 残りの箱の数の変化を確認
	--------------------------------------------------------------------------------*/
	private void CheckRemainingBoxCount()
	{
		if(saveRemainingBoxCount != RemainingBoxCount)
		{
			//	処理を呼び出す
			OnChangedRemainingBoxCount.Invoke();

			//	変数を保持
			saveRemainingBoxCount = RemainingBoxCount;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 箱を置くことが出来るかどうか
	--------------------------------------------------------------------------------*/
	public bool CheckRemainingBoxes()
	{
		//	箱の残数が1以上
		if (RemainingBoxCount > 0)
			return true;

		//	箱の残数が0以下
		OnNonRemaining.Invoke();
		return false;
	}

}
