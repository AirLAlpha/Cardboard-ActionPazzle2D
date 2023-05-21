using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class OpeningSceneChange : MonoBehaviour
{
	static readonly string TITLE_SCENE_NAME = "TitleScene";

	private PlayableDirector director;

	[Header("音")]
	[SerializeField]
	private AudioSource[]	audioSource;
	[SerializeField]
	private float			volumeChangeSpeed;

	private float			volume;
	private bool			muteVolume;

	[Header("開始時間")]
	[SerializeField]
	private float			openingStartWait;

	private float	waitTime;
	private bool	playedOpening;

	private void Awake()
	{
		director = GetComponent<PlayableDirector>();
	}

	private void Update()
	{
		OpeningStartUpdate();
		VolumeMuteUpdate();


		if (Input.GetButtonDown("Jump"))
		{
			ChangeScene();
		}
	}

	//	オープニングの開始処理
	private void OpeningStartUpdate()
	{
		if (playedOpening)
			return;

		if (waitTime >= openingStartWait)
		{
			director.Play();
			playedOpening = true;
		}
		else
		{
			waitTime += Time.deltaTime;
		}
	}

	//	音量の変更処理
	private void VolumeMuteUpdate()
	{
		if (!muteVolume)
			return;

		foreach (var item in audioSource)
		{
			item.volume -= Time.deltaTime * volumeChangeSpeed;
		}
	}


	//	画面遷移の実行処理
	public void ChangeScene()
	{
		muteVolume = true;

		string[] scenes = { TITLE_SCENE_NAME };
		Transition.Instance.StartTransition(scenes);
	}
	
}
