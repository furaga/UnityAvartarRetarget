using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnityChanAvatar : MonoBehaviour
{
    public GameObject MotionPlayer;
    public bool EnableRetargetting = false;

    List<GameObject> joints = new List<GameObject>();
    List<GameObject> bones_ = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        var root = gameObject.transform.Find("Character1_Reference").gameObject;
        var hips = root.gameObject.transform.Find("Character1_Hips").gameObject;
        var leftUpLeg = hips.transform.Find("Character1_LeftUpLeg").gameObject;
        var leftLeg = leftUpLeg.transform.Find("Character1_LeftLeg").gameObject;
        var leftFoot = leftLeg.transform.Find("Character1_LeftFoot").gameObject;
        var leftToeBase = leftFoot.transform.Find("Character1_LeftToeBase").gameObject;
        var rightUpLeg = hips.transform.Find("Character1_RightUpLeg").gameObject;
        var rightLeg = rightUpLeg.transform.Find("Character1_RightLeg").gameObject;
        var rightFoot = rightLeg.transform.Find("Character1_RightFoot").gameObject;
        var rightToeBase = rightFoot.transform.Find("Character1_RightToeBase").gameObject;
        var spine = hips.transform.Find("Character1_Spine").gameObject;
        var spine1 = spine.transform.Find("Character1_Spine1").gameObject;
        var spine2 = spine1.transform.Find("Character1_Spine2").gameObject;
        var leftShoulder = spine2.transform.Find("Character1_LeftShoulder").gameObject;
        var leftArm = leftShoulder.transform.Find("Character1_LeftArm").gameObject;
        var leftForeArm = leftArm.transform.Find("Character1_LeftForeArm").gameObject;
        var leftHand = leftForeArm.transform.Find("Character1_LeftHand").gameObject;
        var rightShoulder = spine2.transform.Find("Character1_RightShoulder").gameObject;
        var rightArm = rightShoulder.transform.Find("Character1_RightArm").gameObject;
        var rightForeArm = rightArm.transform.Find("Character1_RightForeArm").gameObject;
        var rightHand = rightForeArm.transform.Find("Character1_RightHand").gameObject;
        var neck = spine2.transform.Find("Character1_Neck").gameObject;
        var head = neck.transform.Find("Character1_Head").gameObject;

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

        //bones_.AddRange(new GameObject[]
        //{
        //    hips,
        //    rightUpLeg,
        //    rightLeg,
        //    leftUpLeg,
        //    leftLeg,
        //    leftFoot,
        //    spine,
        //    rightShoulder,
        //    rightArm,
        //    rightForeArm,
        //    leftShoulder,
        //    leftArm,
        //    leftForeArm,
        //    neck,
        //}); 
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

    // Update is called once per frame
    void Update()
    {

        //{
        //    var positions = new List<Vector3>();
        //    var rotations = new List<Quaternion>();

        //    foreach (var j in joints)
        //    {
        //        positions.Add(j.transform.position);
        //        rotations.Add(j.transform.rotation);
        //    }

        //    // positions.Add(hip);
        //    int[] jointIdx = new int[] {
        //        2, 3,
        //        2, 1,
        //        1, 0,
        //        3, 4,
        //        4, 5,
        //        14, 12, // 14 is hip
        //        12, 8,
        //        8, 7,
        //        7, 6,
        //        12, 9,
        //        9, 10,
        //        10, 11,
        //        12, 13,
        //    };

        //    for (int i = 0; i < 13; i++)
        //    {
        //        float axisLen = 0.15f;
        //        var rot = rotations[i];
        //        var xAxis = rot * Vector3.right;
        //        var yAxis = rot * Vector3.up;
        //        var zAxis = rot * Vector3.forward;
        //        Vector3 pos = positions[i]; // (positions[jointIdx[2 * i]] + positions[jointIdx[2 * i + 1]]) * 0.5f;
        //        Debug.DrawLine(pos, pos + xAxis * axisLen, Color.red);
        //        Debug.DrawLine(pos, pos + yAxis * axisLen, Color.green);
        //        Debug.DrawLine(pos, pos + zAxis * axisLen, Color.blue);
        //    }
        //}
    }

    void LateUpdate()
    {
        Pose pose = null;
        if (MotionPlayer)
        {
            var component = MotionPlayer.GetComponent<MotionPlayer>();
            if (component)
            {
                pose = component.GetCurrentPose();
            }
        }

        if (pose != null)
        {
            if (EnableRetargetting)
            {
                pose.UpdateBoneLengthAndRotations();


                for (int i = 0; i < 13; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }
                    bones_[i].transform.localRotation = pose.LocalRotations[i];
                }


                //drawSkeletonLines(pose.FK(), new Vector3(1.5f, 0, 0));
                drawSkeletonLines(pose.Positions, new Vector3(1.5f, 0, 0));
                printRotations(pose.LocalRotations);
            }
        }
    }

    Color color(int r, int g, int b)
    {
        return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
    }

    public GameObject DebugText;

    void printRotations(List<Quaternion> rotations)
    {
        if (DebugText)
        {
            string text = "";
            for (int i = 0; i < rotations.Count; i++)
            {
                var q = rotations[i];
                text += string.Format("{0}: {1: 0.00}, {2: 0.00}, {3: 0.00}, {4: 0.00}\n", i, q.x, q.y, q.z, q.w);
            }

            DebugText.GetComponent<UnityEngine.UI.Text>().text = text;
        }
    }

    void drawSkeletonLines(List<Vector3> joints3D, Vector3 offset)
    {
        joints3D = joints3D.ToList();
        for (int i = 0; i < joints3D.Count; i++)
        {
            //            joints3D[i] += offset;
        }

        var light_pink = color(233, 163, 201);
        var pink = color(197, 27, 125);
        var light_blue = color(145, 191, 219);
        var blue = color(69, 117, 180);
        var purple = color(118, 42, 131);

        // line
        Debug.DrawLine(joints3D[(int)LSPJoints.RightAnkle], joints3D[(int)LSPJoints.RightKnee], light_pink);
        Debug.DrawLine(joints3D[(int)LSPJoints.RightKnee], joints3D[(int)LSPJoints.RightHip], light_pink);
        Debug.DrawLine(joints3D[(int)LSPJoints.RightShoulder], joints3D[(int)LSPJoints.RightHip], light_pink);
        Debug.DrawLine(joints3D[(int)LSPJoints.LeftHip], joints3D[(int)LSPJoints.LeftKnee], pink);
        Debug.DrawLine(joints3D[(int)LSPJoints.LeftKnee], joints3D[(int)LSPJoints.LeftAnkle], pink);
        Debug.DrawLine(joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.LeftHip], pink);
        Debug.DrawLine(joints3D[(int)LSPJoints.RightWrist], joints3D[(int)LSPJoints.RightElbow], light_blue);
        Debug.DrawLine(joints3D[(int)LSPJoints.RightElbow], joints3D[(int)LSPJoints.RightShoulder], light_blue);
        Debug.DrawLine(joints3D[(int)LSPJoints.RightShoulder], joints3D[(int)LSPJoints.Neck], light_blue);
        Debug.DrawLine(joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.LeftElbow], blue);
        Debug.DrawLine(joints3D[(int)LSPJoints.LeftElbow], joints3D[(int)LSPJoints.LeftWrist], blue);
        Debug.DrawLine(joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.Neck], blue);
        Debug.DrawLine(joints3D[(int)LSPJoints.Neck], joints3D[(int)LSPJoints.Head], purple);
    }

}
