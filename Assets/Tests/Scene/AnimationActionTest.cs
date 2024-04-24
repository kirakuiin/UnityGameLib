using GameLib.Animation;
using UnityEngine;

namespace Tests.Scene
{
    [RequireComponent(typeof(MoveAction))]
    [RequireComponent(typeof(RotateAction))]
    [RequireComponent(typeof(ScaleAction))]
    [RequireComponent(typeof(VibrationAction))]
    public class AnimationActionTest : MonoBehaviour
    {
        [SerializeField] private Transform obj;

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 300));
            if (GUILayout.Button("位移"))
            {
            }
            if (GUILayout.Button("震动"))
            {
                GetComponent<VibrationAction>().SimpleShake(obj, Vector3.right, 1f, 20f, 0.2f);
            }
            if (GUILayout.Button("旋转"))
            {
            }
            if (GUILayout.Button("缩放"))
            {
            }
            GUILayout.EndArea();
        }
    }
}
