using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "MovieDatabase", menuName = "Create MovieDatabase")]
public class MovieDatabase : ScriptableObject
{
	[SerializeField]
	private List<VideoClip> m_clips;

	public List<VideoClip> Clips => m_clips;
}
