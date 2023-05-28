using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEmitter : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem particle;

	public void Play()
	{
		particle.Play();
	}

}
