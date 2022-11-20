using UnityEngine;

public static class RotationMatrixData
{
    public static readonly float _cos = Mathf.Cos(Mathf.PI / 2f);
    public static readonly float _sin = Mathf.Sin(Mathf.PI / 2f);
    public static readonly float[] _rotationMatrix = new float[] { _cos, _sin, -_sin, _cos };
}
