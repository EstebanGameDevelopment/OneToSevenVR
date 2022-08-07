using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace YourVRExperience.VR
{

    public class RotateVRCamera : MonoBehaviour
    {
        [SerializeField] private float DegreeRotation = 30;

        private bool m_applyRotation = false;

        void Update()
        {
#if ENABLE_OCULUS
        Vector2 joystick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
#else
            Vector2 joystick = Vector2.zero;
#endif

            if (Mathf.Abs(joystick.x) > 0.5)
            {
                if (!m_applyRotation)
                {
                    m_applyRotation = true;
                    if (joystick.x > 0)
                    {
                        this.transform.Rotate(new Vector3(0, DegreeRotation, 0));
                    }
                    else
                    {
                        this.transform.Rotate(new Vector3(0, -DegreeRotation, 0));
                    }
                }
            }
            else
            {
                m_applyRotation = false;
            }
        }
    }
}