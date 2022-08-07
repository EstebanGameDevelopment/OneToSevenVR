using UnityEngine;
using YourVRExperience.Utils;
#if ENABLE_HTCVIVE
using Wave.Essence;
using Wave.Native;
#endif

namespace YourVRExperience.VR
{
    public class HTCHeadController : MonoBehaviour
    {
        public GameObject ControlledHead;

#if ENABLE_HTCVIVE
        void Update()
        {
            if (ControlledHead != null)
            {
                var vec = WaveEssence.Instance.GetCurrentControllerPositionOffset(WVR_DeviceType.WVR_DeviceType_Camera);
                var rot = WaveEssence.Instance.GetCurrentControllerRotationOffset(WVR_DeviceType.WVR_DeviceType_Camera);

                ControlledHead.transform.position = vec;
                ControlledHead.transform.localRotation = rot;
            }
        }
#endif
    }
}

