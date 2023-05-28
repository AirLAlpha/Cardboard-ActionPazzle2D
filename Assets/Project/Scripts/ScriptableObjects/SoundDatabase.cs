using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundDatabase", menuName = "Create SoundDatabase")]
public class SoundDatabase : ScriptableObject
{
	[SerializeField]
	private List<AudioClip>		clips;

	public List<AudioClip> Clips { get { return clips; } }
}
