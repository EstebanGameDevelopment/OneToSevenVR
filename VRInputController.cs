#if ENABLE_PICONEO
using Pvr_UnitySDKAPI;
#elif ENABLE_HTCVIVE
using Wave.Native;
using Wave.Essence;
#endif
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using YourVRExperience.Utils;

namespace YourVRExperience.VR
{
    public class VRInputController  : InputController, IInputController
    {
#if ENABLE_OCULUS
        private OculusControllerInputs m_currentPointer;
#elif ENABLE_PICONEO
        private PicoNeoHandController m_currentPointer;
#elif ENABLE_HTCVIVE
        private HTCHandController m_currentPointer;
#endif

        public override bool IsVR
        {
            get { return true; }
        }
        public override Transform RayPointerVR
        {
            get
            {
#if ENABLE_OCULUS
            if (m_currentPointer == null)
            {
                m_currentPointer = GameObject.FindObjectOfType<OculusControllerInputs>();
            }
            return m_currentPointer.CurrentController.transform;
#elif ENABLE_PICONEO
            if (m_currentPointer == null)
            {
                m_currentPointer = GameObject.FindObjectOfType<PicoNeoHandController>();
            }
            return m_currentPointer.CurrentController.transform;
#elif ENABLE_HTCVIVE
            if (m_currentPointer == null)
            {
                m_currentPointer = GameObject.FindObjectOfType<HTCHandController>();
            }
            return m_currentPointer.CurrentController.transform;
#else
                return null;
#endif
            }
        }

        public override bool EnableMouseRotation()
        {
            return false;
        }

#if ENABLE_HTCVIVE
        public WVR_DeviceType GetDominantDevice()
        {
            return HTCHandController.Instance.CurrentHandDevice;
        }
#endif

#if ENABLE_PICONEO
        public int GetDominantDevice()
        {
            return PicoNeoHandController.Instance.HandTypeSelected;
        }
#endif

#if ENABLE_OCULUS
        public bool GetDominantDevice()
        {
            return OculusControllerInputs.Instance.HandSelected;
        }
#endif

        protected override Vector2 GetMovementJoystick()
        {
#if ENABLE_OCULUS
            return OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
#elif ENABLE_PICONEO
            return Controller.UPvr_GetAxis2D(GetDominantDevice());
#elif ENABLE_HTCVIVE
            return WXRDevice.ButtonAxis(GetDominantDevice(), WVR_InputId.WVR_InputId_Alias1_Touchpad);
#else
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
#endif
        }


        public override bool JumpPressed()
        {
#if ENABLE_OCULUS
        return OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);
#elif ENABLE_PICONEO
            return Controller.UPvr_GetKey(GetDominantDevice(), Pvr_KeyCode.Right) || Controller.UPvr_GetKey(GetDominantDevice(), Pvr_KeyCode.Left);
#elif ENABLE_HTCVIVE
            return WXRDevice.ButtonHold(GetDominantDevice(), WVR_InputId.WVR_InputId_Alias1_Grip);
#else
            return base.JumpPressed();
#endif
        }

        public override bool ShootPressed()
        {
#if ENABLE_OCULUS
            return OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch) || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
#elif ENABLE_PICONEO
            return Controller.UPvr_GetKey(GetDominantDevice(), Pvr_KeyCode.TRIGGER);
#elif ENABLE_HTCVIVE
            return WXRDevice.ButtonRelease(GetDominantDevice(), WVR_InputId.WVR_InputId_Alias1_Trigger);
#else
            return base.ShootPressed();
#endif
        }

        public override bool SwitchedCameraPressed()
        {
#if ENABLE_OCULUS || ENABLE_PICONEO || ENABLE_HTCVIVE
            return false;
#else
            return base.SwitchedCameraPressed();
#endif
        }

    }
}