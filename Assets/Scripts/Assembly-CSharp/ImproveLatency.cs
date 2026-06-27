using UnityEngine;

public class ImproveLatency : MonoBehaviour
{
	public enum Target
	{
		OnlyOnMobiles = 0,
		Everywhere = 1
	}

	public Target target;

	private void OnNetworkJoinChannel(bool success, string error)
	{
		if (base.enabled && success && !TNManager.canUseUDP && (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer || target == Target.Everywhere))
		{
			TNManager.noDelay = true;
		}
	}

	private void OnNetworkLeaveChannel()
	{
		TNManager.noDelay = false;
	}
}
