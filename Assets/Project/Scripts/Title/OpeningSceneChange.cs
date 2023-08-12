using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.UI;

[RequireComponent(typeof(PlayableDirector))]
public class OpeningSceneChange : MonoBehaviour
{
	static readonly string TITLE_SCENE_NAME = "TitleScene";

	private PlayableDirector director;

	[SerializeField]
	SoundDatabase bgm;

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

	[Header("スキップヒント")]
	[SerializeField]
	private Image	buttonImage;
	[SerializeField]
	private CanvasAlphaController alphaController;
	[SerializeField]
	private Sprite	keySprite;
	[SerializeField]
	private Sprite	controllerSprite;
	[SerializeField]
	private float enableCancelTime;

	private bool enableSkip;
	private float enabledTime;

	private void Awake()
	{
		director = GetComponent<PlayableDirector>();
	}

	private void Start()
	{
		Debug.Log("Test");
	}

	private void Update()
	{
		OpeningStartUpdate();
		VolumeMuteUpdate();

		if (!enableSkip)
		{
			if(Input.GetButtonDown("Jump") ||
				Input.GetButtonDown("Fire1")||
				Input.GetButtonDown("Restart"))
			{
				enableSkip = true;
				alphaController.TargetAlpha = 1.0f;
			}

			enabledTime = 0.0f;
		}
		else
		{
			if (Input.GetButtonDown("Jump"))
			{
				ChangeScene();
			}

			enabledTime += Time.deltaTime;
			if(enabledTime >= enableCancelTime)
			{
				enableSkip = false;
				alphaController.TargetAlpha = 0.0f;
			}
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
		if (Transition.Instance.IsTransition)
			return;

		muteVolume = true;

		Transition.Instance.StartTransition(TITLE_SCENE_NAME);
	}
	
	//	コントローラー接続時
	public void ControllerConnected()
	{
		buttonImage.sprite = controllerSprite;
	}
	//	コントローラー接続解除時
	public void ControllerReleased()
	{
		buttonImage.sprite = keySprite;
	}


}
