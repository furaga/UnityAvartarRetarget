using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pose
{
    public int Frame;
    public List<Vector3> Positions = new List<Vector3> { };
    // TODO
    public List<Quaternion> Rotations = new List<Quaternion>();
}

public class Motion
{
    public List<Pose> Poses = new List<Pose>();
}

