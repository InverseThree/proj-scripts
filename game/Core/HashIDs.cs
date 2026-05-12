using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HashIDs : MonoBehaviour
{
	public int locomotionState;
	public int speedFloat;

void Awake()
	{
		locomotionState = Animator.StringToHash("Base Layer.Locomotion");
		speedFloat = Animator.StringToHash("Speed");
	}
}
