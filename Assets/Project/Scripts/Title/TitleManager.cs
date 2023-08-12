/**********************************************
 * 
 *  TitleManager.cs 
 *  タイトルを統括する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/04
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
	[Header("ボタンヒント")]
	[SerializeField]
	private CanvasAlphaController	buttonHintAlphaController;

	[Header("メニュー")]
	[SerializeField]
	private TitleMenu				titleMenu;
	[SerializeField]
	private SettingMenu				settingMenu;

	private bool openMenu;
	public bool OpenMenu { get { return titleMenu.gameObject.activeSelf; } }

	public bool EnableSelectTask { get; set; }

	//	ステージ
	[Header("ステージデータ")]
	[SerializeField]
	private StageDataBase		stageDataBase;
	[SerializeField]
	private SelectedTaskData	selectedData;

	//	オープニング
	[Header("オープニング")]
	[SerializeField]
	private PlayableDirector	titleOpeningDirector;
	[SerializeField]
	private TitlePlayer			player;
	[SerializeField]
	private Vector3				playerStartPos;
	[SerializeField]
	private Vector3				playerEndPos;
	[SerializeField]
	private float				endWait;

	private bool				isOpening;
	private float				waitTime;

	public bool					IsOpening { get { return isOpening; } set { isOpening = value; } }

	//	実行前初期化処理
	private void Awake()
	{
		//if (selectedData.StageID == -1)
		//{
		//	isOpening = true;

		//	player.transform.position = playerStartPos;
		//	player.TargetPos = playerEndPos;
		//	player.SetCapEnable(false);

		//	buttonHintAlphaController.SetAlpha(0.0f);
		//}
	}

	//	初期化処理
	private void Start()
	{
		if (selectedData.StageID == -1)
		{
			isOpening = true;

			player.transform.position = playerStartPos;
			player.TargetPos = playerEndPos;
			player.SetCapEnable(false);

			buttonHintAlphaController.SetAlpha(0.0f);
		}

		//	設定の適応
		Setting setting = SettingLoader.LoadSetting();
		settingMenu.InitSettings(setting);
	}

	//	更新処理
	private void Update()
	{
		if (isOpening && player.IsGoal)
		{
			if (waitTime >= endWait)
			{
				titleOpeningDirector.Play();
			}
			else
			{
				waitTime += Time.deltaTime;
			}
		}

		if(Input.GetButtonDown("Menu") &&
			!EnableSelectTask)
		{
			titleMenu.ActivateMenu();
			openMenu = true;
		}
	}

	/*--------------------------------------------------------------------------------
	|| シーンの読み込み
	--------------------------------------------------------------------------------*/
	[ContextMenu("LoadScene")]
	public void LoadScene()
	{
		if (Transition.Instance.IsTransition)
			return;

		if (stageDataBase.Stages.Length - 1 < selectedData.StageID)
		{
			Debug.LogError("ステージID : " + selectedData.StageID + " は設定されていません。");
			return;
		}

		StageInfo stage = stageDataBase.Stages[selectedData.StageID];
		if(stage == null)
		{
			Debug.LogError("ステージID : " + selectedData.StageID + " は設定されていません。");
			return;
		}

		if(stage.Tasks.Length - 1 < selectedData.TaskIndex)
		{
			Debug.LogError("ステージID : " + selectedData.StageID + " タスクID : " + selectedData.TaskIndex + " は設定されていません。");
			return;
		}

		TaskInfo task = stage.Tasks[selectedData.TaskIndex];
		if(task.StageJson == null)
		{
			Debug.LogError("ステージID : " + selectedData.StageID + " タスクID : " + selectedData.TaskIndex + " は設定されていません。");
			return;
		}

		//	シーン名を配列にする
		string[] scenes =
		{
			//"StageBase",
			//task.Scene.SceneName
			"PlayScene"
		};
		//	シーン遷移の開始
		Transition.Instance.StartTransition(scenes);
		BGMPlayer.Instance.FadeSpeed = 3.0f;
		BGMPlayer.Instance.StartTransition(selectedData.StageID, "NormalBGM");
	}

	public void PlayBGM(int index)
	{
		BGMPlayer.Instance.PlayBGM(index);
	}
}
