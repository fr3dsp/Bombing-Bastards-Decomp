using UnityEngine;

public class MoveSpinObject : TNBehaviour
{
	private void OnDrag(Vector2 delta)
	{
		if (base.tno.isMine)
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.y -= delta.x * 0.5f;
			base.transform.eulerAngles = eulerAngles;
			Vector3 position = base.transform.position;
			position.y += delta.y * 0.01f;
			base.transform.position = position;
		}
	}
}
