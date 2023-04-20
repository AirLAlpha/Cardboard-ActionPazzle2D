/**********************************************
 * 
 *  SoundPlayer.cs 
 *  オーディオの再生を管理する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/12
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundPlayer : MonoBehaviour
{
	//	コンポーネント
	private AudioSource		audioSource;

	//	オーディオクリップ
	[Header("オーディオクリップ")]
	[SerializeField]
	private AudioClip[]			clips;      //	オーディオの一覧

	//	実行前初期化処理
	private void Awake()
	{
		audioSource = GetComponent<AudioSource>();
	}

	/*--------------------------------------------------------------------------------
	|| オーディオの再生
	--------------------------------------------------------------------------------*/
	public void Play(int index)
	{
		if (index >= clips.Length)
		{
			Debug.LogError("インデックスが配列範囲外です。");
			return;
		}

		audioSource.PlayOneShot(clips[index]);
	}
}
