using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JustWithJoints.Avatars
{
    public class AliciaAvatar : MonoBehaviour
    {
        public GameObject MotionPlayer;
        public bool EnableRetargetting = true;
        public bool RetargetTranslation = true;

        List<GameObject> joints = new List<GameObject>();
        List<GameObject> bones_ = new List<GameObject>();
        /*public*/
        Vector3[] correctionRightEulers = new Vector3[13]
{
        new Vector3(0, -90, 180),
        new Vector3(0, 90, -90),
        new Vector3(0, 90, -90),
        new Vector3(0, 90, -90),
        new Vector3(0, 90, -90),
        new Vector3(0, 0, 0),
        new Vector3(0, 0, 0),
        new Vector3(0, -90, 180),
        new Vector3(0, -90, 180),
        new Vector3(0, 0, 0),
        new Vector3(0, 90, 180),
        new Vector3(0, 90, 180),
        new Vector3(0, -90, 180),
};

        //--------------------------------------------------
        // For Debugging
        //--------------------------------------------------

        /*public*/
        bool UseTPose = false;
        const float ratio = 0.5f;
        /*public*/
        Vector3[] TPose = new Vector3[] {
        new Vector3(-1, 0, 0) * ratio,
        new Vector3(-1, 1, 0) * ratio,
        new Vector3(-1, 2, 0) * ratio,
        new Vector3(1, 2, 0) * ratio,
        new Vector3(1, 1, 0) * ratio,
        new Vector3(1, 0, 0) * ratio,
        new Vector3(-3, 4, 0) * ratio,
        new Vector3(-2, 4, 0) * ratio,
        new Vector3(-1, 4, 0) * ratio,
        new Vector3(1, 4, 0) * ratio,
        new Vector3(2, 4, 0) * ratio,
        new Vector3(3, 4, 0) * ratio,
        new Vector3(0, 4, 0) * ratio,
        new Vector3(0, 5, 0) * ratio,
    };
        /*public*/
        GameObject DebugText;
        //--------------------------------------------------
        // For Debugging
        //--------------------------------------------------


        // Use this for initialization
        void Start()
        {
            var root = gameObject.transform.Find("Character001").transform.Find("root").gameObject;
            var hips = root.gameObject.transform.Find("waist").gameObject;
            var rightUpLeg = hips.transform.Find("lowerbody").transform.Find("leg_L").gameObject;
            var rightLeg = rightUpLeg.transform.Find("knee_L").gameObject;
            var rightFoot = rightLeg.transform.Find("ankle_L").gameObject;
            var rightToeBase = rightFoot.transform.Find("toe_L").gameObject;
            var leftUpLeg = hips.transform.Find("lowerbody").transform.Find("leg_R").gameObject;
            var leftLeg = leftUpLeg.transform.Find("knee_R").gameObject;
            var leftFoot = leftLeg.transform.Find("ankle_R").gameObject;
            var leftToeBase = leftFoot.transform.Find("toe_R").gameObject;
            var spine = hips.transform.Find("upperbody").gameObject;
            var spine1 = spine.transform.Find("upperbody01").gameObject;
            var rightShoulder = spine1.transform.Find("shoulder_L").gameObject;
            var rightArm = rightShoulder.transform.Find("arm_L").gameObject;
            var rightForeArm = rightArm.transform.Find("elbow_L").gameObject;
            var rightHand = rightForeArm.transform.Find("wrist_L").gameObject;
            var leftShoulder = spine1.transform.Find("shoulder_R").gameObject;
            var leftArm = leftShoulder.transform.Find("arm_R").gameObject;
            var leftForeArm = leftArm.transform.Find("elbow_R").gameObject;
            var leftHand = leftForeArm.transform.Find("wrist_R").gameObject;
            var neck = spine1.transform.Find("neck").gameObject;
            var head = neck.transform.Find("head").gameObject;

            joints.AddRange(new GameObject[]
            {
            rightFoot,
            rightLeg,
            rightUpLeg,
            leftUpLeg,
            leftLeg,
            leftFoot,
            rightHand,
            rightForeArm,
            rightShoulder,
            leftShoulder,
            leftForeArm,
            leftHand,
            neck,
            head,
            });

            bones_.AddRange(new GameObject[]
            {
            hips,
            rightUpLeg,
            rightLeg,
            leftUpLeg,
            leftLeg,
            spine,
            rightShoulder,
            rightArm,
            rightForeArm,
            leftShoulder,
            leftArm,
            leftForeArm,
            neck,
            });
        }

        void LateUpdate()
        {
            Core.Pose pose = null;
            if (MotionPlayer)
            {
                var component = MotionPlayer.GetComponent<MotionPlayer>();
                if (component)
                {
                    pose = component.GetCurrentPose();
                    if (pose != null && UseTPose)
                    {
                        // Just for debugging
                        pose.Positions = TPose.ToList();
                    }
                }
            }

            if (pose != null)
            {
                if (EnableRetargetting)
                {
                    // Retarget positions
                    if (RetargetTranslation)
                    {
                        bones_[0].transform.position = (pose.Positions[2] + pose.Positions[3]) * 0.5f + gameObject.transform.position;
                    }

                    // Retarget rotations
                    pose.UpdateBoneLengthAndRotations();
                    for (int i = 0; i < 13; i++)
                    {
                        // Use spine's rotation of mocap as root rotation of avatar
                        int boneIndex = i;
                        if (i == 0)
                        {
                            boneIndex = 5;
                        }
                        if (i == 5)
                        {
                            // Instead, do not retarget to spine's rotation
                            continue;
                        }
                        if (i == 6 || i == 9)
                        {
                            // skip shoulders
                            continue;
                        }
                        bones_[i].transform.rotation = pose.Rotations[boneIndex] * Quaternion.Euler(correctionRightEulers[i]);
                    }
                }
            }
        }
    }
}