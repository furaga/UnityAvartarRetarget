using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkeletonAvatar : MonoBehaviour {

    public GameObject MotionPlayer;
    public bool UseFK = false;

    private GameObject[] bones_ = null;

    // Use this for initialization
    void Start () {
        bones_ = new GameObject[13];
        for (int i = 0; i < bones_.Length; i++)
        {
            bones_[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            bones_[i].transform.parent = gameObject.transform;
        }
    }
	
	// Update is called once per frame
	void Update () {

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
            if (UseFK)
            {
                pose.UpdateBoneLengthAndRotations();
                drawSkeleton(pose.FK());
                //drawSkeletonLines(pose.FK(), new Vector3(1.5f, 0, 0));
                drawSkeletonLines(pose.Positions, new Vector3(1.5f, 0, 0));
                printRotations(pose.LocalRotations);
            }
            else
            {
                drawSkeleton(pose.Positions);
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

    void drawSkeleton(List<Vector3> joints3D)
    {
        var light_pink = color(233, 163, 201);
        var pink = color(197, 27, 125);
        var light_blue = color(145, 191, 219);
        var blue = color(69, 117, 180);
        var purple = color(118, 42, 131);

        int boneId = 0;
        drawBone(boneId++, joints3D[(int)LSPJoints.RightAnkle], joints3D[(int)LSPJoints.RightKnee], light_pink);
        drawBone(boneId++, joints3D[(int)LSPJoints.RightKnee], joints3D[(int)LSPJoints.RightHip], light_pink);
        drawBone(boneId++, joints3D[(int)LSPJoints.RightShoulder], joints3D[(int)LSPJoints.RightHip], light_pink);

        drawBone(boneId++, joints3D[(int)LSPJoints.LeftHip], joints3D[(int)LSPJoints.LeftKnee], pink);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftKnee], joints3D[(int)LSPJoints.LeftAnkle], pink);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.LeftHip], pink);

        drawBone(boneId++, joints3D[(int)LSPJoints.RightWrist], joints3D[(int)LSPJoints.RightElbow], light_blue);
        drawBone(boneId++, joints3D[(int)LSPJoints.RightElbow], joints3D[(int)LSPJoints.RightShoulder], light_blue);
        drawBone(boneId++, joints3D[(int)LSPJoints.RightShoulder], joints3D[(int)LSPJoints.Neck], light_blue);

        drawBone(boneId++, joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.LeftElbow], blue);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftElbow], joints3D[(int)LSPJoints.LeftWrist], blue);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.Neck], blue);

        drawBone(boneId++, joints3D[(int)LSPJoints.Neck], joints3D[(int)LSPJoints.Head], purple);
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


    void drawBone(int boneId, Vector3 src, Vector3 tgt, Color c)
    {
        float len = (tgt - src).magnitude;
        bones_[boneId].transform.localScale = new Vector3(0.1f, len, 0.1f);
        bones_[boneId].transform.rotation = Quaternion.FromToRotation(Vector3.up, tgt - src);
        bones_[boneId].transform.position = (tgt + src) * 0.5f;
        bones_[boneId].GetComponent<Renderer>().material.color = c;
    }

}
