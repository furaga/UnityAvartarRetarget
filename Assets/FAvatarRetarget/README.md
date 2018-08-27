# JustWithJoint: Avatar Body Controller with Joint Positions

JustWithJoint is a simple examples of controling 3D avatars just with joint locations.
It estimates bone orientations from the joint locations and retarget three 3D avatars (skeleton, Unity-Chan and Alicia).

Especially, I believe this asset helps researchers of 3D pose estimation.
Most of recent deep learning based approach output only 3D joint locations, not bone orientations.
Some 3D mocap systems do not provide 3D joint locations, or these are not reliable.

In these case, they need to find bone rotations just from joint locations.
However, contrary againt intuition, how to do this is not obvious and sample code is hard to find.
(At least, I could not google any good sample)

JustWithJoint contains simple and great examples:
- Scenes/1_Locations - Control a skeleton simply by retargetting joint locations.
- Scenes/2_Skeleton  - Control a skeleton by estimating bone rotations and forward kinematics
- Scenes/3_Avatar    - Control three 3D avatars (skeleton, Unity-Chan and Alicia) with the estimated bone rotations from joint locations.

Internally, we convert LSP-order 14 joints to 13 bone rotations.

You can use this asset for any purpose. Of coource, commercial use is OK, excepting the contents in JustWithJoint/UnityChan/ and JustWithJoint/Alicia/.


## Features
- Pure C# code of converting bone rotations from joint locations.
- Simple examples of retargeting the converted pose to 3D avatars.


## How to use

1. Import JustWithJoint.unitypackage
2. Open any of scenes in Assets/JustWithJoint/Scenes and run
3. You can find one or more avatars are animating.

