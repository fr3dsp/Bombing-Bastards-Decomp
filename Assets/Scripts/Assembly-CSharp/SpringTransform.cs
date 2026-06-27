using UnityEngine;

[AddComponentMenu("Game/Spring Transform")]
public class SpringTransform : MonoBehaviour
{
	public float springStrength = 10f;

	public bool ignoreOnHost = true;

	private bool mStarted;

	private bool mWasHosting;

	private Transform mParent;

	private Transform mTrans;

	private Vector3 mPos;

	private Quaternion mRot;

	public void Reset()
	{
		mStarted = true;
		mTrans = base.transform;
		mParent = mTrans.parent;
		if (mParent != null)
		{
			mPos = mParent.position;
			mRot = mParent.rotation;
		}
		else
		{
			Object.Destroy(this);
		}
	}

	private void OnEnable()
	{
		if (mStarted)
		{
			Reset();
		}
	}

	private void Start()
	{
		Reset();
	}

	private void OnNetworkJoinChannel(bool success, string error)
	{
		Reset();
	}

	private void LateUpdate()
	{
		if (!mStarted)
		{
			return;
		}
		if (ignoreOnHost && TNManager.isHosting)
		{
			if (!mWasHosting)
			{
				mTrans.position = mParent.position;
				mTrans.rotation = mParent.rotation;
				mWasHosting = true;
			}
		}
		else
		{
			float t = Mathf.Clamp01(Time.deltaTime * springStrength);
			mPos = Vector3.Lerp(mPos, mParent.position, t);
			mRot = Quaternion.Slerp(mRot, mParent.rotation, t);
			mTrans.position = mPos;
			mTrans.rotation = mRot;
			mWasHosting = false;
		}
	}
}
