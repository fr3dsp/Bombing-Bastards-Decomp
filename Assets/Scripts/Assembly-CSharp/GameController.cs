using UnityEngine;

public interface GameController
{
	bool IsConnected();

	float GetHorizontal();

	float GetVertical();

	bool DoBombDrop();

	bool DoBombPick();

	bool DoBombRemote();

	bool ReleaseBombRemote();

	bool DoSelectMenu();

	bool DoStartMenu();

	bool DoActiveButton();

	bool DoMenuButton();

	bool DoCheatButton();

	Vector3 GetPointerPosition();

	bool DoEnterPress(string name);

	bool DoEnterHold(string name);

	bool DoEnterRelease(string name);

	bool DoPadPress(int id);

	bool DoPadRelease(int id);

	bool IsMouseMove();
}
