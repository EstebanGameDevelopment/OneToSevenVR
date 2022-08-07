using UnityEngine;
using YourVRExperience.Utils;
#if ENABLE_PICONEO
using Pvr_UnitySDKAPI;
#endif

namespace YourVRExperience.VR
{
    public class PicoNeoHandController : MonoBehaviour
    {
        public const string EVENT_PICONEOHANDCONTROLLER_UPDATE_LASER = "EVENT_PICONEOHANDCONTROLLER_UPDATE_LASER";
        public const string EVENT_PICONEOHANDCONTROLLER_LINK_WITH_NETWORK_GAMEHAND = "EVENT_PICONEOHANDCONTROLLER_LINK_WITH_NETWORK_GAMEHAND";

        private static PicoNeoHandController instance;

        public static PicoNeoHandController Instance
        {
            get
            {
                if (!instance)
                {
                    instance = GameObject.FindObjectOfType(typeof(PicoNeoHandController)) as PicoNeoHandController;
                }
                return instance;
            }
        }


        public GameObject ControlledObject;
        public GameObject PicoNeoCamera;
        public GameObject PicoNeoLeftController;
        public GameObject PicoNeoRightController;
        public GameObject LaserPointer;
        public GameObject LaserPointerLeft;
        public GameObject LaserPointerRight;

#if ENABLE_PICONEO
        private int m_handTypeSelected = 0;
        private GameObject m_currentController;
        private GameObject m_linkedWithHandRight;
        private GameObject m_linkedWithHandLeft;

        public GameObject CurrentController
        {
            get { return m_currentController; }
        }
        public int HandTypeSelected
        {
            get { return m_handTypeSelected; }
        }

        private void Start()
        {
            Controller.UPvr_SetHandNess(Pvr_Controller.UserHandNess.Right);

            m_handTypeSelected = -1;
            if (Controller.UPvr_GetMainHandNess() == 0)
            {
                SetLaserToLeftHand();
            }
            else
            {
                SetLaserToRightHand();
            }

            SystemEventController.Instance.Event += OnSystemEvent;
        }

        void OnDestroy()
        {
            SystemEventController.Instance.Event -= OnSystemEvent;
        }

        private void SetLaserToLeftHand()
        {
            if (m_handTypeSelected != 0)
            {
                m_handTypeSelected = 0;
                if (LaserPointerLeft != null)
                {
                    if (LaserPointerRight != null) LaserPointerRight.SetActive(false);
                    if (LaserPointerLeft != null) LaserPointerLeft.SetActive(true);
                    SetMainLaserPoint(LaserPointerLeft);
                }
                m_currentController = PicoNeoLeftController;
            }
        }

        private void SetLaserToRightHand()
        {
            if (m_handTypeSelected != 1)
            {
                m_handTypeSelected = 1;
                if (LaserPointerRight != null)
                {
                    if (LaserPointerRight != null) LaserPointerRight.SetActive(true);
                    if (LaserPointerLeft != null) LaserPointerLeft.SetActive(false);
                    SetMainLaserPoint(LaserPointerRight);
                }
                m_currentController = PicoNeoRightController;
            }
        }

        private void SetMainLaserPoint(GameObject _laserPointer)
        {
            LaserPointer = _laserPointer;
            SystemEventController.Instance.DispatchSystemEvent(EVENT_PICONEOHANDCONTROLLER_UPDATE_LASER, LaserPointer, (m_handTypeSelected == 1));
        }

        private void OnSystemEvent(string _nameEvent, object[] _parameters)
        {
            if (_nameEvent == EVENT_PICONEOHANDCONTROLLER_LINK_WITH_NETWORK_GAMEHAND)
            {
                bool isHandRigth = (bool)_parameters[0];
                if (isHandRigth)
                {
                    m_linkedWithHandRight = (GameObject)_parameters[1];
                }
                else
                {
                    m_linkedWithHandLeft = (GameObject)_parameters[1];
                }
            }
        }

        void Update()
        {
            if (Controller.UPvr_GetKeyDown(0, Pvr_KeyCode.TRIGGER))
            {
                SetLaserToLeftHand();
            }
            else if (Controller.UPvr_GetKeyDown(1, Pvr_KeyCode.TRIGGER))
            {
                SetLaserToRightHand();
            }

            if (ControlledObject != null)
            {
                ControlledObject.transform.position = m_currentController.transform.position;
                ControlledObject.transform.rotation = m_currentController.transform.rotation;
            }

            if ((m_linkedWithHandRight != null) && (PicoNeoRightController != null))
            {
                m_linkedWithHandRight.transform.position = PicoNeoRightController.transform.position;
                m_linkedWithHandRight.transform.forward = PicoNeoRightController.transform.forward;
            }
            if ((m_linkedWithHandLeft != null) && (PicoNeoLeftController != null))
            {
                m_linkedWithHandLeft.transform.position = PicoNeoLeftController.transform.position;
                m_linkedWithHandLeft.transform.forward = PicoNeoLeftController.transform.forward;
            }
        }
#endif
    }
}

