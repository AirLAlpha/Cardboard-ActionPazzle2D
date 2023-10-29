using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StageImageData
{
	[SerializeField]
	private Sprite		invoice;
	[SerializeField]
	private Sprite[]	taskImage;

	public Sprite Invoice => invoice;
	public Sprite[] TaskImage => taskImage;
}

[CreateAssetMenu(fileName = "StageImageDatabase", menuName = "Create StageImageDatabase")]
public class StageImageDatabase : ScriptableObject
{
	[SerializeField]
	private StageImageData[] stageData;

	public StageImageData[] StageData => stageData;
}
