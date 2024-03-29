/**********************************************
 * 
 *  PoseManager.cs 
 *  ゲームのポーズに関する処理を記述
 * 
 *  製作者：牛丸 文仁
 *  制作日：2023/04/21
 * 
 **********************************************/
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
	//	ポーズ
	[Header("ポーズ")]
	[SerializeField]
	private string[]	pauseTargetTags;		//	ポーズの対象になるタグの配列

	//	ポーズ中フラグ
	public bool			IsPause { get; private set; }

	private IPauseable[]	poseTargets;        //	ポーズの対象になるオブジェクト配列

	/*--------------------------------------------------------------------------------
	|| 対象オブジェクトの検索
	--------------------------------------------------------------------------------*/
	[ContextMenu("SearchPoseTarget")]
	private void SearchPoseTarget()
	{
		List<GameObject> findObjects = new List<GameObject>();

		foreach (var tag in pauseTargetTags)
		{
			GameObject[] results = GameObject.FindGameObjectsWithTag(tag);
			findObjects.AddRange(results);
		}

		//	見つかったオブジェクトの中からポーズ不可能なオブジェクトを除外
		List<IPauseable> posableObjects = new List<IPauseable>();
		foreach (var obj in findObjects)
		{
			var posables = obj.GetComponents<IPauseable>();
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
	public void Pause()
	{
		//	対象を検索する
		SearchPoseTarget();

		foreach (var target in poseTargets)
		{
			target.Pause();
		}

		IsPause = true;
	}

	/*--------------------------------------------------------------------------------
	|| ポーズの解除
	--------------------------------------------------------------------------------*/
	[ContextMenu("Resume")]
	public void Resume()
	{
		//	対象を検索する
		SearchPoseTarget();

		foreach (var target in poseTargets)
		{
			target?.Resume();
		}

		IsPause = false;
	}
}
