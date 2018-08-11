using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pose
{
    public int Frame;
    public List<Vector3> Positions = new List<Vector3> { };

    public bool HasRotations = false;
    public List<float> BoneLengths = new List<float>();
    public List<Quaternion> Rotations = new List<Quaternion>();

    public void UpdateBoneLengthAndRotations()
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
            var translate = positions[boneJoints[src]] - positions[boneJoints[dst]];
            var boneLength = translate.magnitude;
            BoneLengths.Add(boneLength);
        }

        // todo
        for (int i = 0; i < 13; i++)
        {
            Rotations.Add(Quaternion.identity);
        }

        HasRotations = true;
    }


    public List<Vector3> FK()
    {
        // todo
        return Positions;
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

