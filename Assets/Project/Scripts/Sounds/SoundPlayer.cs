/**********************************************
 * 
 *  SoundPlayer.cs 
 *  SoundDatabaseより音を再生する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/05/27
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour
{
	[Header("データベース")]
	[SerializeField]
	private SoundDatabase	database;		//	データベース

	[Header("オーディオソース")]
	[SerializeField]
	private AudioSource		source;
	[SerializeField]
	private AudioMixer		mixer;

	//	プロパティ
	public SoundDatabase		Database	{ get { return database; } }
	public AudioSource			Source		{ get { return source; } }

	/*--------------------------------------------------------------------------------
	|| サウンドの再生処理
	--------------------------------------------------------------------------------*/
	public void PlaySound(int dbIndex)
	{
		PlaySound(dbIndex, 1.0f);
	}
	public void PlaySound(int dbIndex, float exVol)
	{
		//	指定のインデックスが範囲外なら処理しない
		if (dbIndex >= database.Clips.Count)
			return;

		var clip = database.Clips[dbIndex];
		//	クリップが設定されていなければ処理しない
		if (clip == null)
			return;

		source.volume = exVol;		//	音量の設定
		source.PlayOneShot(clip);	//	サウンドの再生
	}

	/*--------------------------------------------------------------------------------
	|| ループの有効化
	--------------------------------------------------------------------------------*/
	public void SetEnableLoop(bool enable)
	{
		source.loop = enable;
	}

	/*--------------------------------------------------------------------------------
	|| 音量の変更処理
	--------------------------------------------------------------------------------*/
	public void SetVolume(float vol)
	{
		source.volume = vol;
	}

	/*--------------------------------------------------------------------------------
	|| オーディオミキサーの切り替え
	--------------------------------------------------------------------------------*/
	public bool SetMixerGroup(string groupName)
	{
		var group = mixer.FindMatchingGroups(groupName);
		if (group == null)
			return false;

		source.outputAudioMixerGroup = group[0];
		return true;
	}

	public void SetAudioSource(AudioSource source)
	{
		this.source = source;
	}

}
