using UnityEngine;

public class ExampleCreate : MonoBehaviour
{
	public GameObject objectToCreate;

	private void OnClick()
	{
		Vector3 worldPos = TouchHandler.worldPos;
		worldPos.y += 3f;
		Quaternion rot = Quaternion.Euler(Random.value * 180f, Random.value * 180f, Random.value * 180f);
		TNManager.Create(objectToCreate, worldPos, rot);
	}
}
