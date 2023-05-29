/**********************************************
 * 
 *  BGMPlayer.cs 
 *  BGMの再生処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/27
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlayer : SingletonMonoBehaviour<BGMPlayer>
{
	//	遷移モード
	private enum SoundTransitionMode
	{
		NONE,

		FADE_OUT,
		WAIT,
		FADE_IN
	}
	private SoundTransitionMode mode;   //	現在の遷移モード

	[Header("サウンドプレイヤー")]
	[SerializeField]
	private SoundPlayer player;

	[Header("最大音量")]
	[SerializeField]
	private float maxVolume;

	[Header("遷移速度")]
	[SerializeField]
	private float speed;
	[Header("遷移待機時間")]
	[SerializeField]
	private float waitTime;

	private float	progress;				//	遷移の進行度
	private bool	isPlaying;              //	再生中フラグ
	private int		currentIndex;			//	現在設定中のインデックス

	//	フェード
	private bool	isFade;					//	フェード中フラグ

	public float FadeSpeed { get { return speed; } set { speed = value; } }

	public bool IsPlaying	{ get { return isPlaying; } }
	public bool IsFade		{ get { return isFade; } }
	public int  CurrentIndex { get { return currentIndex; } }
	public SoundPlayer SoundPlayer { get { return player; } }

	private void Start()
	{
		DontDestroyOnLoad(gameObject);
	}

	/*--------------------------------------------------------------------------------
	|| BGMの再生
	--------------------------------------------------------------------------------*/
	public void PlayBGM(int dbIndex, bool enableFade = false)
	{
		if (dbIndex >= player.Database.Clips.Count)
			return;

		var clip = player.Database.Clips[dbIndex];
		if (clip == null)
			return;

		player.Source.clip = clip;
		player.Source.Play();
		isPlaying = true;
		currentIndex = dbIndex;

		if(enableFade)
		{
			StartCoroutine(StartFade());
		}
		else
		{
			player.SetVolume(maxVolume);
		}
	}

	/*--------------------------------------------------------------------------------
	|| BGMの停止
	--------------------------------------------------------------------------------*/
	public void StopBGM(bool enableFade = false)
	{
		if (!enableFade)
		{
			player.Source.Stop();
			isPlaying = false;
		}
		else
		{
			StartCoroutine(StopFade());
		}
	}

	/*--------------------------------------------------------------------------------
	|| 遷移の開始
	--------------------------------------------------------------------------------*/
	public void StartTransition(int dbIndex, string groupName = "")
	{
		if (dbIndex >= player.Database.Clips.Count)
			return;

		var clip = player.Database.Clips[dbIndex];
		if (clip == null)
			return;

		currentIndex = dbIndex;
		StartCoroutine(TransitionCoroutine(dbIndex, groupName));
	}

	/*--------------------------------------------------------------------------------
	|| BGMの遷移処理
	--------------------------------------------------------------------------------*/
	private IEnumerator TransitionCoroutine(int dbIndex, string groupName)
	{
		isFade = true;

		//	進行度をリセット
		progress = 0.0f;
		//	モードを設定
		mode = SoundTransitionMode.FADE_OUT;
		//	フェードアウト
		while (progress < 1.0f)
		{
			progress += Time.deltaTime * speed;
			player.SetVolume(Mathf.Lerp(maxVolume, 0.0f, progress));
			yield return null;
		}

		//	モードを切り替え
		mode = SoundTransitionMode.WAIT;

		isPlaying = false;
		//	音量を0にしておく
		player.SetVolume(0);
		//	クリップの差し替え
		player.Source.clip = player.Database.Clips[dbIndex];
		//	グループの設定
		if (groupName != string.Empty)
			player.SetMixerGroup(groupName);

		//	時間まで待つ
		yield return new WaitForSeconds(waitTime);

		//	クリップの再生
		player.Source.Play();
		isPlaying = true;

		//	モードを切り替え
		mode = SoundTransitionMode.FADE_IN;
		//	進行度をリセット
		progress = 0.0f;

		//	フェードイン
		while (progress < 1.0f)
		{
			progress += Time.deltaTime * speed;
			player.SetVolume(Mathf.Lerp(0.0f, maxVolume, progress));
			yield return null;
		}

		//	終了
		mode = SoundTransitionMode.NONE;

		isFade = false;
	}

	/*--------------------------------------------------------------------------------
	|| BGMの停止処理（フェード）
	--------------------------------------------------------------------------------*/
	private IEnumerator StopFade()
	{
		if (isFade)
			yield break;

		isFade = true;
		progress = 0.0f;

		while(progress < 1.0f)
		{
			progress += Time.deltaTime * speed;
			player.SetVolume(Mathf.Lerp(maxVolume, 0.0f, progress));
			yield return null;
		}

		player.SetVolume(0.0f);
		player.Source.Stop();
		isPlaying = false;
		isFade = false;
	}

	/*--------------------------------------------------------------------------------
	|| スタートフェード
	--------------------------------------------------------------------------------*/
	private IEnumerator StartFade()
	{
		if (isFade)
			yield break;

		isFade = true;
		progress = 0.0f;
		player.SetVolume(0.0f);

		while (progress < 1.0f)
		{
			progress += Time.deltaTime * speed;
			player.SetVolume(Mathf.Lerp(0.0f, maxVolume, progress));
			yield return null;
		}

		player.SetVolume(maxVolume);
		isPlaying = false;
		isFade = false;
	}
}
