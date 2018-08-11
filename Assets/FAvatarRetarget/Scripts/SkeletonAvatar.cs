using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAvatar : MonoBehaviour {

    public GameObject LocationsPlayer;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

    }

    void LateUpdate()
    {
        Pose sk = null;
        if (LocationsPlayer)
        {
            var component = LocationsPlayer.GetComponent<MotionPlayer>();
            if (component)
            {
                 sk = component.GetCurrentPose();
            }
        }

        if (sk!= null)
        {
            drawSkeleton(sk.Positions);
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
        Debug.DrawLine(joints3D[(int)LSPJoints.RightAnkle], joints3D[(int)LSPJoints.RightKnee], Color.red);
        Debug.DrawLine(joints3D[(int)LSPJoints.RightKnee], joints3D[(int)LSPJoints.RightHip], Color.red);

        Debug.DrawLine(joints3D[(int)LSPJoints.LeftHip], joints3D[(int)LSPJoints.LeftKnee], Color.red);
        Debug.DrawLine(joints3D[(int)LSPJoints.LeftKnee], joints3D[(int)LSPJoints.LeftAnkle], Color.red);

        Debug.DrawLine(joints3D[(int)LSPJoints.RightWrist], joints3D[(int)LSPJoints.RightElbow], Color.red);
        Debug.DrawLine(joints3D[(int)LSPJoints.RightElbow], joints3D[(int)LSPJoints.RightShoulder],
            Color.red);

        Debug.DrawLine(joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.LeftElbow], Color.red);
        Debug.DrawLine(joints3D[(int)LSPJoints.LeftElbow], joints3D[(int)LSPJoints.LeftWrist], Color.red);

        Debug.DrawLine(joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.LeftHip], Color.red);
        Debug.DrawLine(joints3D[(int)LSPJoints.RightShoulder], joints3D[(int)LSPJoints.RightHip], Color.red);

        Debug.DrawLine(joints3D[(int)LSPJoints.LeftShoulder], joints3D[(int)LSPJoints.Neck], Color.red);
        Debug.DrawLine(joints3D[(int)LSPJoints.RightShoulder], joints3D[(int)LSPJoints.Neck], Color.red);

        Debug.DrawLine(joints3D[(int)LSPJoints.Neck], joints3D[(int)LSPJoints.Head], Color.red);
    }
}
