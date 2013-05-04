using UnityEngine;
using System.Collections;

public class LevelInteractive : MonoBehaviour
{
	private void Start()
	{
		BoxCollider boxCollider = GetComponent<BoxCollider>();
		Vector3 size = boxCollider.size;
		size.x *= 3f;
		boxCollider.size = size;
	}
}
