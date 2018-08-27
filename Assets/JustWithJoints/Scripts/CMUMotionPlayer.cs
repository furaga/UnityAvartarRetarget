using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JustWithJoints
{
    public class CMUMotionPlayer : MonoBehaviour
    {
        public string DataPath = "Assets/FAvatarRetarget/Resources/sample.txt";
        public float Timer = 0.0f;
        public float FPS = 120.0f;
        public bool Loop = true;
        public bool Play = true;
        public bool FlipLeftRight = true;

        private Core.Motion motion_ = null;
        private int frame_ = 0;

        // Use this for initialization
        void Start()
        {
            motion_ = CMUMotionLoader.Load(DataPath, FlipLeftRight);
        }

        // Update is called once per frame
        void Update()
        {
            if (Play)
            {
                Timer += Time.deltaTime;
            }

            frame_ = (int)(Timer * FPS);
            if (frame_ >= motion_.Poses.Count)
            {
                Timer = 0.0f;
            }
        }

        public Core.Pose GetCurrentPose()
        {
            if (frame_ < 0 || motion_.Poses.Count <= frame_)
            {
                return null;
            }
            var pose = motion_.Poses[frame_];
            return pose;
        }
    }
}