using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

public class Pose
{
    public int Frame;
    public List<Vector3> Positions = new List<Vector3> { };

    // Bone
    // 0: Trans
    // 1: R ULeg
    // 2: R Leg
    // 3: L ULeg
    // 4: L Leg
    // 5: Spine
    // 6: R Shoulder
    // 7: R UArm
    // 8: R Arm
    // 9: L Shoulder
    // 10: L UArm
    // 11: L Arm
    // 12: Neck

    public bool HasRotations = false;
    public List<float> BoneLengths = new List<float>();
    public List<Quaternion> Rotations = new List<Quaternion>();
    public List<Quaternion> LocalRotations = new List<Quaternion>();
    
    public void UpdateBoneLengthAndRotations()
    {
        BoneLengths.Clear();
        Rotations.Clear();
        LocalRotations.Clear();
        updateBoneLengths();
        updateBoneRotations();
        HasRotations = true;
    }

    void updateBoneLengths()
    {
        // Posiitons are in the lsp order.
        int[] boneJoints = new int[]
        {
            2, 3,
            2, 1,
            1, 0,
            3, 4,
            4, 5,
            14, 12, // 14 is hip
            12, 8,
            8, 7,
            7, 6,
            12, 9,
            9, 10,
            10, 11,
            12, 13,
        };

        var positions = Positions.ToList();
        var hip = (positions[2] + positions[3]) * 0.5f;
        positions.Add(hip);

        for (int i = 0; i < 13; i++)
        {
            int src = boneJoints[2 * i];
            int dst = boneJoints[2 * i + 1];
            var translate = positions[src] - positions[dst];
            var boneLength = translate.magnitude;
            BoneLengths.Add(boneLength);
        }
    }

    void updateBoneRotations()
    {
        for (int i = 0; i < 13; i++)
        {
            LocalRotations.Add(Quaternion.identity);
        }

        var hip = (Positions[2] + Positions[3]) * 0.5f;

        var rotLeftDown = Quaternion.LookRotation(Vector3.left, Vector3.down);
        LocalRotations[0] = calcRotation(Positions[3] - Positions[2], hip - Positions[12], rotLeftDown);

        // calc global rotations
        var rotLeftForward = Quaternion.LookRotation(Vector3.left, Vector3.forward);
        var rotLeftBack = Quaternion.LookRotation(Vector3.left, Vector3.back);
        // leg
        LocalRotations[1] = calcRotation(Positions[1] - Positions[2], Positions[0] - Positions[1], rotLeftForward);
        LocalRotations[2] = calcRotation(Positions[0] - Positions[1], Positions[2] - Positions[1], rotLeftForward); //
        LocalRotations[3] = calcRotation(Positions[4] - Positions[3], Positions[5] - Positions[4], rotLeftForward);
        LocalRotations[4] = calcRotation(Positions[5] - Positions[4], Positions[3] - Positions[4], rotLeftForward); //
        // spine
        LocalRotations[5] = calcRotation(Positions[12] - hip, Positions[3] - Positions[2], rotLeftDown);
        // arm
        LocalRotations[6] = calcRotation(Positions[8] - Positions[12], Positions[9] - Positions[8], rotLeftDown); //
        LocalRotations[7] = calcRotation(Positions[7] - Positions[8], Positions[6] - Positions[7], rotLeftBack);
        LocalRotations[8] = calcRotation(Positions[6] - Positions[7], Positions[8] - Positions[7], rotLeftBack);
        LocalRotations[9] = calcRotation(Positions[9] - Positions[12], Positions[9] - Positions[8], rotLeftDown);
        LocalRotations[10] = calcRotation(Positions[10] - Positions[9], Positions[11] - Positions[10], rotLeftBack);
        LocalRotations[11] = calcRotation(Positions[11] - Positions[10], Positions[9] - Positions[10], rotLeftBack);
        // neck
        LocalRotations[12] = calcRotation(Positions[13] - Positions[12], Positions[9] - Positions[8], rotLeftDown);

        Rotations = LocalRotations.ToList();

        {
            var positions = Positions.ToList();
            positions.Add(hip);
            int[] jointIdx = new int[] {
                2, 3,
                2, 1,
                1, 0,
                3, 4,
                4, 5,
                14, 12, // 14 is hip
                12, 8,
                8, 7,
                7, 6,
                12, 9,
                9, 10,
                10, 11,
                12, 13,
            };

            for (int i = 0; i < 13; i++)
            {
                float axisLen = 0.15f;
                var rot = Rotations[i];
                var xAxis = rot * Vector3.right;
                var yAxis = rot * Vector3.up;
                var zAxis = rot * Vector3.forward;
                Vector3 pos = (positions[jointIdx[2 * i]] + positions[jointIdx[2 * i + 1]]) * 0.5f;
                Debug.DrawLine(pos, pos + xAxis * axisLen, Color.red);
                Debug.DrawLine(pos, pos + yAxis * axisLen, Color.green);
                Debug.DrawLine(pos, pos + zAxis * axisLen, Color.blue);
            }
        }



        // Convert Global Rotations to Local Rotations
        var invRoot = Quaternion.Inverse(LocalRotations[0]);
        var invRULeg = Quaternion.Inverse(LocalRotations[1]);
        var invLULeg = Quaternion.Inverse(LocalRotations[3]);
        var invSpine = Quaternion.Inverse(LocalRotations[5]);
        var invRShoulder = Quaternion.Inverse(LocalRotations[6]);
        var invRUArm = Quaternion.Inverse(LocalRotations[7]);
        var invLShoulder = Quaternion.Inverse(LocalRotations[9]);
        var invLUArm = Quaternion.Inverse(LocalRotations[10]);

        LocalRotations[1] = invRoot * LocalRotations[1];
        LocalRotations[2] = invRULeg * LocalRotations[2];
        LocalRotations[3] = invRoot * LocalRotations[3];
        LocalRotations[4] = invLULeg * LocalRotations[4];
//        Rotations[5] = Rotations[5];
        LocalRotations[6] = invSpine * LocalRotations[6];
        LocalRotations[7] = invRShoulder * LocalRotations[7];
        LocalRotations[8] = invRUArm * LocalRotations[8];
        LocalRotations[9] = invSpine * LocalRotations[9];
        LocalRotations[10] = invLShoulder * LocalRotations[10];
        LocalRotations[11] = invLUArm * LocalRotations[11];
        LocalRotations[12] = invSpine * LocalRotations[12];
    }

    Quaternion calcRotation(Vector3 boneDirection, Vector3 left, Quaternion init)
    {
        var d = boneDirection;
        var up = Vector3.Cross(d, left);
        d.Normalize();
        up.Normalize();
        var target = Quaternion.LookRotation(d, up);
        var rot = target * Quaternion.Inverse(init);
        return rot;
    }
    
    public List<Vector3> FK()
    {
        var positions = Positions.ToList();
        var hip = (positions[2] + positions[3]) * 0.5f;
        positions.Add(hip);

        var lengths = BoneLengths.ToList();
        var hipLength = lengths[0] * 0.5f;
        lengths[0] = hipLength;
        lengths.Insert(0, -hipLength);
        var rots = LocalRotations.ToList();
        rots.Insert(0, rots[0]);

        int[] joints = new int[]
        {
            2, 3,
            1, 0,
            4, 5,
            12,
            8, 7, 6,
            9, 10, 11,
            13,
        };

        // 14 is hip
        int[] parentJoints = new int[]
        {
            14, 14,
            2, 1,
            3, 4,
            14,
            12, 8, 7,
            12, 9, 10,
            12,
        };

        // fk results = joint positions
        Vector3[] fk = new Vector3[15];
        fk[14] = hip;

        // global rotations
        Quaternion[] gRots = new Quaternion[15];
        gRots[14] = Quaternion.identity;

        for (int i = 0; i < joints.Length; i++)
        {
            float l = lengths[i];
            Quaternion r = rots[i];
            int j = joints[i];
            int p = parentJoints[i];

            Vector3 pPos = fk[p];
            Quaternion pRot = gRots[p];

            Quaternion rot = pRot * r;
            Vector3 translate = rot * Vector3.left * l;
            Vector3 pos = pPos + translate;

            fk[j] = pos;
            gRots[j] = rot;
        }

        return fk.Take(14).ToList();
    }

}

public class Motion
{
    public List<Pose> Poses = new List<Pose>();

    public void UpdateBoneLengthAndRotations()
    {
        foreach (var p in Poses)
        {
            p.UpdateBoneLengthAndRotations();
        }
    }
}

