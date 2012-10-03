using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Kinect;

namespace ArmText.Core
{
    public class SkeletonImageReadyArgs: EventArgs
    {

        public Skeleton UserSkeleton { get; set; } 

    }
}
