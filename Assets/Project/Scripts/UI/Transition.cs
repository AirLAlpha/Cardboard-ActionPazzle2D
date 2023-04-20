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

public class Transition : SingletonMonoBehaviour<Transition>
{
	public enum TransitionMode
	{
		NONE,

		FADE_OUT = 1,
		FADE_IN = -1,
	}

	[SerializeField]
	private Image		transitionImage;        //	画面遷移のイメージ

	[Space, SerializeField]
	private TransitionMode		mode;
	[SerializeField]
	private float				speed;
	[SerializeField, Range(0.0f,1.0f)]
	private float				progress;       //	進行度

	public bool IsTransition { get { return mode != TransitionMode.NONE; } }

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
	public void StartTransition(string[] scenes)
	{
		StartCoroutine(TransitionCoroutine(scenes));
	}

	/*--------------------------------------------------------------------------------
	|| 画面遷移のコルーチン
	--------------------------------------------------------------------------------*/
	private IEnumerator TransitionCoroutine(string[] scenes)
	{
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

		//	シーン遷移
		SceneManager.LoadScene(scenes[0]);
		for (int i = 1; i < scenes.Length; i++)
		{
			SceneManager.LoadScene(scenes[i], LoadSceneMode.Additive);
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
	}

	/*--------------------------------------------------------------------------------
	|| イージング関数（easeInOutCubic）
	--------------------------------------------------------------------------------*/
	private float EaseInOutCubic(float x)
	{
		return x < 0.5f ? 4 * x * x * x : 1 - Mathf.Pow(-2 * x + 2, 3) / 2;
	}

}
