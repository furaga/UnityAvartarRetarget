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
    
    public void UpdateBoneLengthAndRotations()
    {
        BoneLengths.Clear();
        Rotations.Clear();
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
            Rotations.Add(Quaternion.identity);
        }

        var hip = (Positions[2] + Positions[3]) * 0.5f;

        Rotations[0] = calcRotation(Positions[3] - Positions[2], hip - Positions[12]);

        // calc global rotations
        Rotations[1] = calcRotation(Positions[1] - Positions[2], Positions[0] - Positions[1]);
        Rotations[2] = calcRotation(Positions[0] - Positions[1], Positions[2] - Positions[1]); //
        Rotations[3] = calcRotation(Positions[4] - Positions[3], Positions[5] - Positions[4]);
        Rotations[4] = calcRotation(Positions[5] - Positions[4], Positions[3] - Positions[4]); //
        Rotations[5] = calcRotation(Positions[12] - hip, Positions[3] - Positions[2]);
        Rotations[6] = calcRotation(Positions[8] - Positions[12], Positions[12] - hip); //
        Rotations[7] = calcRotation(Positions[7] - Positions[8], Positions[6] - Positions[7]);
        Rotations[8] = calcRotation(Positions[6] - Positions[7], Positions[8] - Positions[7]);
        Rotations[9] = calcRotation(Positions[9] - Positions[12], hip - Positions[12]);
        Rotations[10] = calcRotation(Positions[10] - Positions[9], Positions[10] - Positions[11]);
        Rotations[11] = calcRotation(Positions[11] - Positions[10], Positions[10] - Positions[9]);
        Rotations[12] = calcRotation(Positions[13] - Positions[12], Positions[9] - Positions[8]);


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
        var invRoot = Quaternion.Inverse(Rotations[0]);
        var invRULeg = Quaternion.Inverse(Rotations[1]);
        var invLULeg = Quaternion.Inverse(Rotations[3]);
        var invSpine = Quaternion.Inverse(Rotations[5]);
        var invRShoulder = Quaternion.Inverse(Rotations[6]);
        var invRUArm = Quaternion.Inverse(Rotations[7]);
        var invLShoulder = Quaternion.Inverse(Rotations[9]);
        var invLUArm = Quaternion.Inverse(Rotations[10]);

        Rotations[1] = invRoot * Rotations[1];
        Rotations[2] = invRULeg * Rotations[2];
        Rotations[3] = invRoot * Rotations[3];
        Rotations[4] = invLULeg * Rotations[4];
        Rotations[5] = invRoot * Rotations[5];
        Rotations[6] = invSpine * Rotations[6];
        Rotations[7] = invRShoulder * Rotations[7];
        Rotations[8] = invRUArm * Rotations[8];
        Rotations[9] = invSpine * Rotations[9];
        Rotations[10] = invLShoulder * Rotations[10];
        Rotations[11] = invLUArm * Rotations[11];
        Rotations[12] = invSpine * Rotations[12];
    }

    Quaternion calcRotation(Vector3 boneDirection, Vector3 left)
    {
        var d = boneDirection;
        var up = Vector3.Cross(d, left);
        d.Normalize();
        up.Normalize();
        var target = Quaternion.LookRotation(d, up);
        var init = Quaternion.LookRotation(Vector3.up, Vector3.forward);
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
        var rots = Rotations.ToList();
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
            2,
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
            Vector3 translate = rot * Vector3.up * l;
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

