/* Copyright 2019 Francisco Martin <fmrico@gmail.com>
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

 using System;
 using System.Collections.Concurrent;
 using System.Collections.Generic;
 using System.Linq;
 using System.Runtime.InteropServices;
 using System.Text;
 using System.Threading.Tasks;
using geometry_msgs.msg;
using ROS2;
using ROS2.Utils;

namespace Ros2 {
  namespace Tf2DotNet {
    public class TransformBroadcaster {

      private Publisher<tf2_msgs.msg.TFMessage> tf_pub_;

      public TransformBroadcaster(Node node) {
        tf_pub_ = node.CreatePublisher<tf2_msgs.msg.TFMessage> ("/tf");
      }

      public void SendTransform(geometry_msgs.msg.TransformStamped transform) {
        List<geometry_msgs.msg.TransformStamped> transforms = new List<geometry_msgs.msg.TransformStamped>();
        transforms.Add(transform);
        SendTransform(transforms);
      }

      public void SendTransform(List<geometry_msgs.msg.TransformStamped> transforms) {
        tf2_msgs.msg.TFMessage message = new tf2_msgs.msg.TFMessage();

        foreach (geometry_msgs.msg.TransformStamped value in transforms)
        {
          message.Transforms.Add(value);
        }

        tf_pub_.Publish(message);
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Transform {
      public int Sec;
      public uint Nanosec;
      public double Translation_x;
      public double Translation_y;
      public double Translation_z;
      public double Rotation_x;
      public double Rotation_y;
      public double Rotation_z;
      public double Rotation_w;

      internal TransformStamped ToTransformStamped(string targetFrame, string sourceFrame)
      {
        TransformStamped transformStamped = new TransformStamped();

        transformStamped.Header.Stamp.Sec = Sec;
        transformStamped.Header.Stamp.Nanosec = Nanosec;
        transformStamped.Header.FrameId = targetFrame;
        transformStamped.ChildFrameId = sourceFrame;
        transformStamped.Transform.Translation.X = Translation_x;
        transformStamped.Transform.Translation.Y = Translation_y;
        transformStamped.Transform.Translation.Z = Translation_z;
        transformStamped.Transform.Rotation.X = Rotation_x;
        transformStamped.Transform.Rotation.Y = Rotation_y;
        transformStamped.Transform.Rotation.Z = Rotation_z;
        transformStamped.Transform.Rotation.W = Rotation_w;
        
        return transformStamped;
      }
    }
  }
}
