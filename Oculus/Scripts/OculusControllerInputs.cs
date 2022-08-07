#if ENABLE_OCULUS
using OculusSampleFramework;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using YourVRExperience.Utils;

namespace YourVRExperience.VR
{
    public enum HAND { right = 0, left = 1, both = 2, none = 3 }

    public class OculusControllerInputs : MonoBehaviour
    {
        public GameObject HandLeftController;
        public GameObject HandRightController;

#if ENABLE_OCULUS
        public const string EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_ACTIVATION = "EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_ACTIVATION";
        public const string EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_DEACTIVATION = "EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_DEACTIVATION";
        public const string EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_DOWN = "EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_DOWN";
        public const string EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_UP = "EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_UP";

        public const string EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_TOUCH = "EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_TOUCH";
        public const string EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_UNTOUCH = "EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_UNTOUCH";
        public const string EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_DOWN = "EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_DOWN";
        public const string EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_UP = "EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_UP";

        public const string EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_TOUCH = "EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_TOUCH";
        public const string EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_UNTOUCH = "EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_UNTOUCH";
        public const string EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_DOWN = "EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_DOWN";
        public const string EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_UP = "EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_UP";

        public const string EVENT_OCULUSINPUTCONTROLLER_ONE_DOWN = "EVENT_OCULUSINPUTCONTROLLER_ONE_DOWN";
        public const string EVENT_OCULUSINPUTCONTROLLER_ONE_UP = "EVENT_OCULUSINPUTCONTROLLER_ONE_UP";
        public const string EVENT_OCULUSINPUTCONTROLLER_TWO_DOWN = "EVENT_OCULUSINPUTCONTROLLER_TWO_DOWN";
        public const string EVENT_OCULUSINPUTCONTROLLER_TWO_UP = "EVENT_OCULUSINPUTCONTROLLER_TWO_UP";

        private static OculusControllerInputs _instance;

        public static OculusControllerInputs Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = GameObject.FindObjectOfType(typeof(OculusControllerInputs)) as OculusControllerInputs;
                    return _instance;
                }
                else
                {
                    return _instance;
                }
            }
        }

        private bool m_joystickPressedRight = false;
        private bool m_joystickPressedLeft = false;

        private LineRenderer m_raycastLineLeft;
        private LineRenderer m_raycastLineRight;

        private bool m_indexTriggerLeftPressed;
        private bool m_indexTriggerRightPressed;

        private bool m_handTriggerLeftPressed;
        private bool m_handTriggerRightPressed;

        private bool m_oneButtonLeftPressed = false;
        private bool m_twoButtonLeftPressed = false;

        private bool m_oneButtonRightPressed = false;
        private bool m_twoButtonRightPressed = false;
        
        private bool m_handSelected = false;

        private OVRInputModule m_ovrInputModule;
        private OVRGazePointer m_ovrGazePointer;

        public LineRenderer RaycastLineLeft
        {
            get { return m_raycastLineLeft; }
        }
        public LineRenderer RaycastLineRight
        {
            get { return m_raycastLineRight; }
        }

        public GameObject CurrentController
        {
            get {
                if (m_handSelected)
                {
                    return m_raycastLineRight.gameObject;
                }
                else
                {
                    return m_raycastLineLeft.gameObject;
                }
            }
        }
        public bool HandSelected
        {
            get { return m_handSelected; }
            set {
                 m_handSelected = value;
                 if (CurrentController != null)
                 {
                    DisableRays();
                    CurrentController.SetActive(true);
                    if (m_ovrInputModule == null)
                    {
                        m_ovrInputModule = GameObject.FindObjectOfType<OVRInputModule>();
                    }
                    m_ovrInputModule.rayTransform = CurrentController.transform;
                    if (m_ovrGazePointer == null)
                    {
                        m_ovrGazePointer = GameObject.FindObjectOfType<OVRGazePointer>();
                    }
                    m_ovrGazePointer.rayTransform = CurrentController.transform;                     
                 }
             }
        }

        void Start()
        {
            m_raycastLineLeft = HandLeftController.GetComponentInChildren<LineRenderer>();
            m_raycastLineRight = HandRightController.GetComponentInChildren<LineRenderer>();
            DisableRays();
        }

        private void OnDestroy()
        {
            if (Instance !=  null)
            {
                Deactivate();

                GameObject.Destroy(_instance.gameObject);
                _instance = null;
            }
        }

        private void DisableRays()
        {
            if (m_raycastLineLeft != null) m_raycastLineLeft.gameObject.SetActive(false);
            if (m_raycastLineRight != null) m_raycastLineRight.gameObject.SetActive(false);
        }

        public void Deactivate()
        {
            m_joystickPressedRight = false;
            m_joystickPressedLeft = false;
            m_indexTriggerLeftPressed = false;
            m_indexTriggerRightPressed = false;

            if (HandLeftController != null) HandLeftController.SetActive(false);
            if (HandRightController != null) HandRightController.SetActive(false);

            if (m_raycastLineLeft != null) m_raycastLineLeft.gameObject.SetActive(false);
            if (m_raycastLineRight != null) m_raycastLineRight.gameObject.SetActive(false);
        }

        public void Activate()
        {
            HandLeftController.SetActive(true);
            HandRightController.SetActive(true);

            if (m_raycastLineLeft != null) m_raycastLineLeft.gameObject.SetActive(false);
            if (m_raycastLineRight != null) m_raycastLineRight.gameObject.SetActive(false);
        }

        private void GetIndexTriggerInputs()
        {
            // TRIGGER RIGHT
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                HandSelected = true;
                OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_TOUCH, HAND.right, HandRightController.transform, m_raycastLineRight);
            }
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
            {
                HandSelected = true;
                OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_UNTOUCH, HAND.right, HandRightController.transform, m_raycastLineRight);
            }

            float triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.RTouch);
            if (!m_indexTriggerRightPressed)
            {
                if (triggerValue > 0.4f)
                {
                    m_indexTriggerRightPressed = true;
                    m_handSelected = true;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_DOWN, HAND.right, HandRightController.transform, m_raycastLineRight);
                }
            }
            else
            {
                if (triggerValue < 0.2f)
                {
                    m_indexTriggerRightPressed = false;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_UP, HAND.right, HandRightController.transform, m_raycastLineRight);
                }
            }

            // TRIGGER LEFT
            if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)) 
            {
                HandSelected = false;
                OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_TOUCH, HAND.left, HandLeftController.transform, m_raycastLineLeft);
            }
            if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch)) 
            {
                HandSelected = false;
                OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_UNTOUCH, HAND.left, HandLeftController.transform, m_raycastLineLeft);
            }

            triggerValue = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, OVRInput.Controller.LTouch);
            if (!m_indexTriggerLeftPressed)
            {
                if (triggerValue > 0.4f)
                {
                    m_indexTriggerLeftPressed = true;
                    m_handSelected = false;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_DOWN, HAND.left, HandLeftController.transform, m_raycastLineLeft);
                }
            }
            else
            {
                if (triggerValue < 0.2f)
                {
                    m_indexTriggerLeftPressed = false;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_INDEX_TRIGGER_UP, HAND.left, HandLeftController.transform, m_raycastLineLeft);
                }
            }
        }

        private void GetHandTriggerInputs()
        {
            // HAND RIGHT
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch)) 
            {
                HandSelected = true;
                OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_TOUCH, HAND.right, HandRightController.transform);
            }
            if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch))
            {
                HandSelected = true;
                OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_UNTOUCH, HAND.right, HandRightController.transform);
            }

            float handValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.RTouch);
            if (!m_handTriggerRightPressed)
            {
                if (handValue > 0.4f)
                {
                    m_handTriggerRightPressed = true;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_DOWN, HAND.right, HandRightController.transform);
                }
            }
            else
            {
                if (handValue < 0.2f)
                {
                    m_handTriggerRightPressed = false;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_UP, HAND.right, HandRightController.transform);
                }
            }

            // TRIGGER LEFT
            if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch))
            {
                HandSelected = false;
                OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_TOUCH, HAND.left, HandLeftController.transform);
            }
            if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch)) 
            {
                HandSelected = false;
                OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_UNTOUCH, HAND.left, HandLeftController.transform);
            }

            handValue = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, OVRInput.Controller.LTouch);
            if (!m_handTriggerLeftPressed)
            {
                if (handValue > 0.4f)
                {
                    m_handTriggerLeftPressed = true;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_DOWN, HAND.left, HandLeftController.transform);
                }
            }
            else
            {
                if (handValue < 0.2f)
                {
                    m_handTriggerLeftPressed = false;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_HAND_TRIGGER_UP, HAND.left, HandLeftController.transform);
                }
            }
        }

        private void GetButtonsInputs()
        {
            if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.LTouch))
            {
                if (!m_oneButtonLeftPressed)
                {
                    m_oneButtonLeftPressed = true;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_ONE_DOWN, HAND.left, m_raycastLineLeft.gameObject);
                }
            }
            else
            {
                if (m_oneButtonLeftPressed)
                {
                    m_oneButtonLeftPressed = false;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_ONE_UP, HAND.right, m_raycastLineRight.gameObject);
                }
            }
            if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.LTouch))
            {
                if (!m_twoButtonLeftPressed)
                {
                    m_twoButtonLeftPressed = true;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_TWO_DOWN, HAND.left, m_raycastLineLeft.gameObject);
                }
            }
            else
            {
                if (m_twoButtonLeftPressed)
                {
                    m_twoButtonLeftPressed = false;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_TWO_UP, HAND.left, m_raycastLineLeft.gameObject);
                }
            }
            if (OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
            {
                if (!m_oneButtonRightPressed)
                {
                    m_oneButtonRightPressed = true;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_ONE_DOWN, HAND.right, m_raycastLineRight.gameObject);
                }
            }
            else
            {
                if (m_oneButtonRightPressed)
                {
                    m_oneButtonRightPressed = false;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_ONE_UP, HAND.right, m_raycastLineRight.gameObject);
                }
            }
            if (OVRInput.Get(OVRInput.Button.Two, OVRInput.Controller.RTouch))
            {
                if (!m_twoButtonRightPressed)
                {
                    m_twoButtonRightPressed = true;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_TWO_DOWN, HAND.right, m_raycastLineRight.gameObject);
                }
            }
            else
            {
                if (m_twoButtonRightPressed)
                {
                    m_twoButtonRightPressed = false;
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_TWO_UP, HAND.right, m_raycastLineRight.gameObject);
                }
            }
        }

        public Vector2 GetInputThumbsticks(bool _checkVectorOnly = false)
        {
            Vector2 vectorThumbstick = Vector2.zero;
            Vector2 vectorLeftThumbstick = Vector2.zero;
            Vector2 vectorRightThumbstick = Vector2.zero;

            // LEFT CONTROLLER
            if (!_checkVectorOnly)
            {
                if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch))
                {
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_DOWN, HAND.left, HandLeftController.transform);
                }
                if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.LTouch)) OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_UP, HAND.left, HandLeftController.transform);

                // RIGHT CONTROLLER
                if (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch))
                {
                    OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_DOWN, HAND.right, HandRightController.transform);
                }
                if (OVRInput.GetUp(OVRInput.Button.PrimaryThumbstick, OVRInput.Controller.RTouch)) OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_UP, HAND.right, HandRightController.transform);
            }

            // LEFT CONTROLLER
            if (_checkVectorOnly)
            {
                if (m_joystickPressedLeft)
                {
                    vectorLeftThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
                }
            }
            else
            {
                if (!m_joystickPressedLeft)
                {
                    // PRESSED
                    vectorThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
                    if (vectorThumbstick.magnitude > 0.5)
                    {
                        m_joystickPressedLeft = true;
                        vectorLeftThumbstick = vectorThumbstick;
                        OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_ACTIVATION, HAND.left, HandLeftController.transform, vectorThumbstick);
                    }
                }
                else
                {
                    // RELEASED
                    vectorThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);
                    if (vectorThumbstick.magnitude < 0.1)
                    {
                        m_joystickPressedLeft = false;
                        vectorLeftThumbstick = vectorThumbstick;
                        OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_DEACTIVATION, HAND.left, HandLeftController.transform, vectorThumbstick);
                    }
                }
            }

            // RIGHT CONTROLLER
            if (_checkVectorOnly)
            {
                if (m_joystickPressedRight)
                {
                    vectorRightThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
                }
            }
            else
            {
                if (!m_joystickPressedRight)
                {
                    // PRESSED
                    vectorThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
                    if (vectorThumbstick.magnitude > 0.5)
                    {
                        m_joystickPressedRight = true;
                        vectorRightThumbstick = vectorThumbstick;
                        OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_ACTIVATION, HAND.right, HandRightController.transform, vectorThumbstick);
                    }
                }
                else
                {
                    // RELEASE
                    vectorThumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);
                    if (vectorThumbstick.magnitude < 0.1)
                    {
                        m_joystickPressedRight = false;
                        vectorRightThumbstick = vectorThumbstick;
                        OculusEventObserver.Instance.DispatchOculusEvent(EVENT_OCULUSINPUTCONTROLLER_THUMBSTICK_DEACTIVATION, HAND.right, HandLeftController.transform, vectorThumbstick);
                    }
                }
            }

            if (m_joystickPressedRight)
            {
                return vectorRightThumbstick;
            }
            else
            {
                if (m_joystickPressedLeft)
                {
                    return vectorLeftThumbstick;
                }
            }
            return Vector2.zero;
        }

        void Update()
        {
            if ((HandLeftController == null) || (HandRightController == null))
            {
                throw new Exception("The controllers should be specified for OculusControllerInputs");
            }
            else
            {
                GetInputThumbsticks();

                GetIndexTriggerInputs();

                GetHandTriggerInputs();

                GetButtonsInputs();
            }
        }
#endif
    }
}