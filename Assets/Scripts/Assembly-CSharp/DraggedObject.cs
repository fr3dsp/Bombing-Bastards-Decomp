using TNet;
using UnityEngine;

public class DraggedObject : TNBehaviour
{
	private Transform mTrans;

	private Player mOwner;

	private Vector3 mTarget;

	private void Awake()
	{
		mTrans = base.transform;
		mTarget = mTrans.position;
	}

	private void Update()
	{
		mTrans.position = Vector3.Lerp(mTrans.position, mTarget, Time.deltaTime * 20f);
	}

	private void OnPress(bool isPressed)
	{
		if (isPressed)
		{
			if (mOwner == null)
			{
				ClaimObject(TNManager.playerID, mTrans.position);
				base.tno.Send(2, Target.OthersSaved, TNManager.playerID, mTrans.position);
			}
		}
		else if (mOwner == TNManager.player)
		{
			ClaimObject(0, mTrans.position);
			base.tno.Send(2, Target.OthersSaved, 0, mTrans.position);
		}
	}

	[RFC(2)]
	private void ClaimObject(int playerID, Vector3 pos)
	{
		mOwner = TNManager.GetPlayer(playerID);
		mTrans.position = pos;
		mTarget = pos;
		base.gameObject.layer = LayerMask.NameToLayer((mOwner == null) ? "Default" : "Ignore Raycast");
	}

	private void OnDrag(Vector2 delta)
	{
		if (mOwner == TNManager.player)
		{
			mTarget = TouchHandler.worldPos;
			base.tno.SendQuickly(3, Target.OthersSaved, mTarget);
		}
	}

	[RFC(3)]
	private void MoveObject(Vector3 pos)
	{
		mTarget = pos;
	}
}
