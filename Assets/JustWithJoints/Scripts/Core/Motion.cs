using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JustWithJoints.Core
{

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

        public bool HasRotations = false;
        public List<float> BoneLengths = new List<float>();
        public List<Quaternion> LocalRotations = new List<Quaternion>();
        public List<Quaternion> Rotations = new List<Quaternion>();

        public void UpdateBoneLengthAndRotations()
        {
            BoneLengths.Clear();
            LocalRotations.Clear();
            Rotations.Clear();
            updateBoneLengths();
            updateBoneRotations();
            HasRotations = true;
        }

        void updateBoneLengths()
        {
            // Posiitons are in the lsp order.
            // 14 means hip
            int[] boneJoints = new int[]
            {
            2, 3,
            2, 1,
            1, 0,
            3, 4,
            4, 5,
            14, 12,
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
            // [Bone]
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

            // Global Rotations
            for (int i = 0; i < 13; i++)
            {
                Rotations.Add(Quaternion.identity);
            }
            var rotLeftForward = Quaternion.LookRotation(Vector3.left, Vector3.forward);
            var rotLeftBack = Quaternion.LookRotation(Vector3.left, Vector3.back);
            var rotLeftDown = Quaternion.LookRotation(Vector3.left, Vector3.down);
            var rotLeftUp = Quaternion.LookRotation(Vector3.left, Vector3.up);
            var hip = (Positions[2] + Positions[3]) * 0.5f;
            Rotations[0] = globalRotation(Positions[3] - Positions[2], hip - Positions[12], rotLeftDown);
            Rotations[1] = globalRotation(Positions[1] - Positions[2], Positions[0] - Positions[1], rotLeftForward);
            Rotations[2] = globalRotation(Positions[0] - Positions[1], Positions[2] - Positions[1], rotLeftForward); //
            Rotations[3] = globalRotation(Positions[4] - Positions[3], Positions[5] - Positions[4], rotLeftForward);
            Rotations[4] = globalRotation(Positions[5] - Positions[4], Positions[3] - Positions[4], rotLeftForward); //
            Rotations[5] = globalRotation(Positions[12] - hip, Positions[3] - Positions[2], rotLeftDown);
            Rotations[6] = globalRotation(Positions[8] - Positions[12], Positions[9] - Positions[8], rotLeftUp); //
            Rotations[7] = globalRotation(Positions[7] - Positions[8], Positions[6] - Positions[7], rotLeftBack);
            Rotations[8] = globalRotation(Positions[6] - Positions[7], Positions[8] - Positions[7], rotLeftBack);
            Rotations[9] = globalRotation(Positions[9] - Positions[12], Positions[9] - Positions[8], rotLeftUp);
            Rotations[10] = globalRotation(Positions[10] - Positions[9], Positions[11] - Positions[10], rotLeftBack);
            Rotations[11] = globalRotation(Positions[11] - Positions[10], Positions[9] - Positions[10], rotLeftBack);
            Rotations[12] = globalRotation(Positions[13] - Positions[12], Positions[9] - Positions[8], rotLeftDown);

            // Local Rotations
            var invRoot = Quaternion.Inverse(Rotations[0]);
            var invRULeg = Quaternion.Inverse(Rotations[1]);
            var invLULeg = Quaternion.Inverse(Rotations[3]);
            var invSpine = Quaternion.Inverse(Rotations[5]);
            var invRShoulder = Quaternion.Inverse(Rotations[6]);
            var invRUArm = Quaternion.Inverse(Rotations[7]);
            var invLShoulder = Quaternion.Inverse(Rotations[9]);
            var invLUArm = Quaternion.Inverse(Rotations[10]);
            LocalRotations = Rotations.ToList();
            LocalRotations[1] = invRoot * LocalRotations[1];
            LocalRotations[2] = invRULeg * LocalRotations[2];
            LocalRotations[3] = invRoot * LocalRotations[3];
            LocalRotations[4] = invLULeg * LocalRotations[4];
            LocalRotations[6] = invSpine * LocalRotations[6];
            LocalRotations[7] = invRShoulder * LocalRotations[7];
            LocalRotations[8] = invRUArm * LocalRotations[8];
            LocalRotations[9] = invSpine * LocalRotations[9];
            LocalRotations[10] = invLShoulder * LocalRotations[10];
            LocalRotations[11] = invLUArm * LocalRotations[11];
            LocalRotations[12] = invSpine * LocalRotations[12];
        }

        Quaternion globalRotation(Vector3 boneDirection, Vector3 left, Quaternion init)
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
            2, 3, 1, 0, 4, 5, 12, 8, 7, 6, 9, 10, 11, 13,
            };

            // 14 means hip
            int[] parentJoints = new int[]
            {
            14, 14, 2, 1, 3, 4, 14, 12, 8, 7, 12, 9, 10, 12,
            };

            // fk results = joint positions
            Vector3[] fkPositions = new Vector3[15];
            fkPositions[14] = hip;

            // global rotations
            Quaternion[] globalRotations = new Quaternion[15];
            globalRotations[14] = Quaternion.identity;

            for (int i = 0; i < joints.Length; i++)
            {
                float len = lengths[i];
                Quaternion localRot = rots[i];
                int j = joints[i];
                int p = parentJoints[i];
                Vector3 pPos = fkPositions[p];
                Quaternion parentRot = globalRotations[p];
                Quaternion globalRot = parentRot * localRot;
                Vector3 translate = globalRot * Vector3.left * len;
                Vector3 pos = pPos + translate;
                fkPositions[j] = pos;
                globalRotations[j] = globalRot;
            }

            return fkPositions.Take(14).ToList();
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

}