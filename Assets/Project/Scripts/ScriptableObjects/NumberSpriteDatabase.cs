using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NumberSpriteDatabase", menuName = "Create NumberSpriteDatabase")]
public class NumberSpriteDatabase : ScriptableObject
{
	[SerializeField]
	private Sprite[]	numberSprites;

	public Sprite[] NumberSprites { get { return numberSprites; } }
}
