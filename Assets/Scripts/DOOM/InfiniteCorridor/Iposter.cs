using UnityEngine;

public interface IPoster
{
    void OnShot(RaycastHit hit);
    int GetStateIndex();
}
