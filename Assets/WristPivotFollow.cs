using UnityEngine;

public class WristPivotFollow : MonoBehaviour
{
    public Transform controller;               // Right Controller
    public Vector3 wristOffsetLocal = new(0f, -0.02f, -0.06f); // vers le poignet (à régler)
    public Vector3 wristRotOffsetEuler = Vector3.zero;        // souvent 0

    void LateUpdate()
    {
        if (!controller) return;

        // Le pivot poignet suit la pose du controller + un offset local (vers le poignet)
        transform.position = controller.TransformPoint(wristOffsetLocal);
        transform.rotation = controller.rotation * Quaternion.Euler(wristRotOffsetEuler);
    }
}
