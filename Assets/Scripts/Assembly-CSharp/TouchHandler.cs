using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TouchHandler : MonoBehaviour
{
	public static Vector3 worldPos;

	public static Vector2 screenPos;

	public LayerMask eventReceiverMask = -1;

	private Camera mCam;

	private GameObject mGo;

	private void Awake()
	{
		mCam = base.GetComponent<Camera>();
	}

	private void Update()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			if (touch.phase == TouchPhase.Began)
			{
				screenPos = touch.position;
				SendPress(touch.position);
			}
			else if (touch.phase == TouchPhase.Moved)
			{
				SendDrag(touch.position);
			}
			else if (touch.phase != TouchPhase.Stationary)
			{
				SendRelease(touch.position);
			}
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			screenPos = Input.mousePosition;
			SendPress(Input.mousePosition);
		}
		if (Input.GetMouseButtonUp(0))
		{
			SendRelease(Input.mousePosition);
		}
		if (mGo != null && Input.GetMouseButton(0))
		{
			SendDrag(Input.mousePosition);
		}
	}

	private void SendPress(Vector2 pos)
	{
		worldPos = pos;
		mGo = Raycast(pos);
		if (mGo != null)
		{
			mGo.SendMessage("OnPress", true, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void SendRelease(Vector2 pos)
	{
		worldPos = pos;
		if (mGo != null)
		{
			GameObject gameObject = Raycast(pos);
			if (mGo == gameObject)
			{
				mGo.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
			}
			mGo.SendMessage("OnPress", false, SendMessageOptions.DontRequireReceiver);
			mGo = null;
		}
	}

	private void SendDrag(Vector2 pos)
	{
		Vector2 vector = pos - screenPos;
		if (vector.sqrMagnitude > 0.001f)
		{
			Raycast(pos);
			mGo.SendMessage("OnDrag", vector, SendMessageOptions.DontRequireReceiver);
			screenPos = pos;
		}
	}

	private GameObject Raycast(Vector2 pos)
	{
		RaycastHit hitInfo;
		if (Physics.Raycast(mCam.ScreenPointToRay(pos), out hitInfo, 300f, eventReceiverMask))
		{
			worldPos = hitInfo.point;
			return hitInfo.collider.gameObject;
		}
		return null;
	}
}
