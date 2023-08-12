/**********************************************
 * 
 *  Transition.cs 
 *  画面遷移のアニメーションに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/11
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.CompilerServices;

public class Transition : SingletonMonoBehaviour<Transition>
{
	public enum TransitionMode
	{
		NONE,

		FADE_OUT	= 1,
		WAIT		= 0,
		FADE_IN		= -1,
	}

	[SerializeField]
	private Image		transitionImage;        //	画面遷移のイメージ

	[Space, SerializeField]
	private TransitionMode		mode;
	[SerializeField]
	private float				speed;
	[SerializeField, Range(0.0f,1.0f)]
	private float				progress;       //	進行度
	[SerializeField]
	private float				fadeinWait;

	private bool isTransition;
	public bool IsTransition { get { return isTransition; } }


	//	実行前初期化処理
	protected override void Awake()
	{
		base.Awake();

		//	自身を削除しないオブジェクトして設定する
		DontDestroyOnLoad(gameObject);
	}

	//	更新処理
	private void Update()
	{
		if (mode == TransitionMode.NONE)
			return;

		PivotUpdate();
		TransitionUpdate();
	}

	/*--------------------------------------------------------------------------------
	|| モードに応じてPivotを設定する
	--------------------------------------------------------------------------------*/
	private void PivotUpdate()
	{
		switch (mode)
		{
			case TransitionMode.FADE_OUT:
				transitionImage.rectTransform.pivot = new Vector2(1.0f, 0.5f);
				transitionImage.rectTransform.anchoredPosition = new Vector2(960.0f, 0.0f);
				break;
			case TransitionMode.FADE_IN:
				transitionImage.rectTransform.pivot = new Vector2(0.0f, 0.5f);
				transitionImage.rectTransform.anchoredPosition = new Vector2(-960.0f, 0.0f);
				break;
		}
	}

	/*--------------------------------------------------------------------------------
	|| 進行度に応じて画面遷移を行う
	--------------------------------------------------------------------------------*/
	private void TransitionUpdate()
	{
		float p = mode == TransitionMode.FADE_IN ? 1.0f - progress : progress;
		float width = Mathf.Lerp(0.0f, 1920, EaseInOutCubic(p));
		transitionImage.rectTransform.sizeDelta = new Vector2(width, 1080);
	}

	/*--------------------------------------------------------------------------------
	|| 画面遷移の開始処理
	--------------------------------------------------------------------------------*/
	public void StartTransition(params string[] scenes)
	{
		StartCoroutine(TransitionCoroutine(scenes));
	}

	/*--------------------------------------------------------------------------------
	|| 画面遷移のコルーチン
	--------------------------------------------------------------------------------*/
	private IEnumerator TransitionCoroutine(string[] scenes)
	{
		isTransition = true;

		//	イメージのアクティブを切り替え
		transitionImage.gameObject.SetActive(true);
		//	進行度をリセット
		progress = 0.0f;
		//	モードを設定
		mode = TransitionMode.FADE_OUT;
		//	フェードアウト
		while(progress < 1.0f)
		{
			progress += Time.deltaTime * speed;
			yield return null;
		}

		//	モードを切り替え
		mode = TransitionMode.WAIT;

		//	自動シーン遷移を無効化
		AsyncOperation[] asyncs = new AsyncOperation[scenes.Length];

		asyncs[0] = SceneManager.LoadSceneAsync(scenes[0]);
		asyncs[0].allowSceneActivation = false;
		//	シーン遷移
		for (int i = 1; i < scenes.Length; i++)
		{
			asyncs[i] = SceneManager.LoadSceneAsync(scenes[i], LoadSceneMode.Additive);
			asyncs[i].allowSceneActivation = false;
		}

		//	時間まで待つ
		yield return new WaitForSeconds(fadeinWait);

		//	シーンのロードが終わるまで待機する
		bool isLoadDone = false;
		while (!isLoadDone)
		{
			isLoadDone = true;

			for (int i = 0; i < asyncs.Length; i++)
			{
				if (asyncs[i].progress >= 0.9f)
					continue;

				isLoadDone = false;
				Debug.Log("ロードが完了していません。");
			}
		}

		//	ステージを有効化
		foreach (var async in asyncs)
		{
			async.allowSceneActivation = true;
		}

		//	モードを切り替え
		mode = TransitionMode.FADE_IN;
		//	進行度をリセット
		progress = 0.0f;

		//	フェードイン
		while(progress < 1.0f)
		{
			progress += Time.deltaTime * speed;
			yield return null;
		}

		//	終了
		mode = TransitionMode.NONE;
		//	イメージのアクティブを切り替え
		transitionImage.gameObject.SetActive(false);

		isTransition = false;
	}

	/*--------------------------------------------------------------------------------
	|| イージング関数（easeInOutCubic）
	--------------------------------------------------------------------------------*/
	private float EaseInOutCubic(float x)
	{
		return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
	}

}
