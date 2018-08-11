using System.Collections;
using System.Collections.Generic;
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
            }
            else
            {
                drawSkeleton(pose.Positions);
            }
        }
    }

    enum LSPJoints
    {
        RightAnkle = 0,
        RightKnee,
        RightHip,

        LeftHip,
        LeftKnee,
        LeftAnkle,

        RightWrist,
        RightElbow,
        RightShoulder,

        LeftShoulder,
        LeftElbow,
        LeftWrist,

        Neck,
        Head,
    }

    void drawSkeleton(List<Vector3> joints3D)
    {
        int boneId = 0;
        drawBone(boneId++, joints3D[(int)LSPJoints.RightAnkle], joints3D[(int)LSPJoints.RightKnee]);
        drawBone(boneId++, joints3D[(int)LSPJoints.RightKnee], joints3D[(int)LSPJoints.RightHip]);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftHip], joints3D[(int)LSPJoints.LeftKnee]);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftKnee], joints3D[(int)LSPJoints.LeftAnkle]);
        drawBone(boneId++, joints3D[(int)LSPJoints.RightWrist], joints3D[(int)LSPJoints.RightElbow]);
        drawBone(boneId++, joints3D[(int)LSPJoints.RightElbow], joints3D[(int)LSPJoints.RightShoulder]);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.LeftElbow]);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftElbow], joints3D[(int)LSPJoints.LeftWrist]);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.LeftHip]);
        drawBone(boneId++, joints3D[(int)LSPJoints.RightShoulder], joints3D[(int)LSPJoints.RightHip]);
        drawBone(boneId++, joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.Neck]);
        drawBone(boneId++, joints3D[(int)LSPJoints.RightShoulder], joints3D[(int)LSPJoints.Neck]);
        drawBone(boneId++, joints3D[(int)LSPJoints.Neck], joints3D[(int)LSPJoints.Head]);
    }

    void drawBone(int boneId, Vector3 src, Vector3 tgt)
    {
        float len = (tgt - src).magnitude;
        bones_[boneId].transform.localScale = new Vector3(1.5f, len, 1.5f);
        bones_[boneId].transform.rotation = Quaternion.FromToRotation(Vector3.up, tgt - src);
        bones_[boneId].transform.position = (tgt + src) * 0.5f;
    }

}
