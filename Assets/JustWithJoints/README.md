# JustWithJoints: Avatar Body Controller with Joint Positions

JustWithJoints privides simple examples to control 3D avatars just with joint locations.
It estimates bone lengths and orientations and retarget them to three 3D avatars (skeleton, Unity-Chan and Alicia).

Especially, I believe this asset helps researchers of 3D pose estimation.
Most of recent deep learning based approaches output only 3D joint locations.

In these case, researchers need to estimate bone orientations just from joint 3D locations.
However, againt intuition, how to do this is not obvious and it is hard to find sample codes.
(At least, I could not google anything helpful)

JustWithJoints is for you! It contains simple and great examples:
- Assets/JustWithJoints/Scenes/1_Locations.unity: Control a skeleton simply by retargetting joint locations.
- Assets/JustWithJoints/Scenes/2_FK.unity       : Control a skeleton by estimating bone rotations and running forward kinematics
- Assets/JustWithJoints/Scenes/3_Avatars.unity  : Control three 3D avatars (skeleton, Unity-Chan and Alicia) with the estimated bone orientations.

Internally, we convert 14 joint locations to 13 bone lengths and orientations.


## Features
- Pure C# code of converting bone rotations from joint locations.
- Simple examples of retargeting the converted pose to 3D avatars.


## How to use

1. Import JustWithJoints.unitypackage
2. Open any of scenes in Assets/JustWithJoints/Scenes and run it.
3. You can find one or more avatars are animating.


## 3D models

We use the folowing third-party 3D models.

1. Unity-chan model, version 1.2.1
   - We downloaded from http://unity-chan.com/download/index.php
   - Copyright: © Unity Technologies Japan/UCL

2. Alicia (ニコニコ立体ちゃん), version 4
   - We downloaded from http://3d.nicovideo.jp/works/td14712


## Motions

The motion files in Assets/JustWithJoints/MotionData are made from `CMU Graphics Lab Motion Capture Database`.
We read asf/amc files, calculated joint locations and save as the text files.
cf. http://mocap.cs.cmu.edu/