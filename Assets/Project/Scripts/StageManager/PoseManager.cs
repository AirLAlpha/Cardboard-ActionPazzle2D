/**********************************************
 * 
 *  PoseManager.cs 
 *  ゲームのポーズに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/21
 * 
 **********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Android;
using UnityEngine;

public class PoseManager : MonoBehaviour
{
	//	ポーズ
	[Header("ポーズ")]
	[SerializeField]
	private string[]	poseTargetTags;		//	ポーズの対象になるタグの配列

	//	ポーズ中フラグ
	public bool			IsPose { get; private set; }

	private IPoseable[]	poseTargets;        //	ポーズの対象になるオブジェクト配列

	//	初期化処理
	private void Start()
	{
		//	対象を検索する
		SearchPoseTarget();
	}

	//	更新処理
	private void Update()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			if (IsPose)
				Resume();
			else
				Pose();
		}
	}

	/*--------------------------------------------------------------------------------
	|| 対象オブジェクトの検索
	--------------------------------------------------------------------------------*/
	[ContextMenu("SearchPoseTarget")]
	private void SearchPoseTarget()
	{
		List<GameObject> findObjects = new List<GameObject>();

		foreach (var tag in poseTargetTags)
		{
			GameObject[] results = GameObject.FindGameObjectsWithTag(tag);
			findObjects.AddRange(results);
		}

		//	見つかったオブジェクトの中からポーズ不可能なオブジェクトを除外
		List<IPoseable> posableObjects = new List<IPoseable>();
		foreach (var obj in findObjects)
		{
			var posables = obj.GetComponents<IPoseable>();
			if (posables == null || posables.Length == 0)
				continue;

			posableObjects.AddRange(posables);
		}

		//	リストを配列に変換して格納する
		poseTargets = posableObjects.ToArray();
	}

	/*--------------------------------------------------------------------------------
	|| ポーズの有効化
	--------------------------------------------------------------------------------*/
	[ContextMenu("Pose")]
	public void Pose()
	{
		SearchPoseTarget();

		foreach (var target in poseTargets)
		{
			target.Pose();
		}

		IsPose = true;
	}

	/*--------------------------------------------------------------------------------
	|| ポーズの解除
	--------------------------------------------------------------------------------*/
	[ContextMenu("Resume")]
	public void Resume()
	{
		foreach (var target in poseTargets)
		{
			target.Resume();
		}

		IsPose = false;
	}
}
