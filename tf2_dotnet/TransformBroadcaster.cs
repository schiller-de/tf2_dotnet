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

using System.Collections.Generic;
using ROS2;

namespace Ros2.Tf2DotNet
{
    public sealed class TransformBroadcaster
    {
        private readonly Publisher<tf2_msgs.msg.TFMessage> tfPublisher;

        /// <summary>
        /// The TF2 dynamic broadcaster qos profile.
        /// </summary>
        public static QosProfile DynamicBroadcasterQosProfile { get; } = QosProfile.KeepLast(100);

        
        /// <summary>
        /// The TF2 static broadcaster qos profile.
        /// </summary>
        public static QosProfile StaticBroadcasterQosProfile { get; } = QosProfile.KeepLast(1).WithTransientLocal();

        public TransformBroadcaster(Node node, QosProfile qosProfile = null)
        {
            if (qosProfile == null)
            {
                qosProfile = DynamicBroadcasterQosProfile;
            }

            tfPublisher = node.CreatePublisher<tf2_msgs.msg.TFMessage>("/tf", qosProfile);
        }

        public void SendTransform(geometry_msgs.msg.TransformStamped transform)
        {
            var transforms = new List<geometry_msgs.msg.TransformStamped>();
            transforms.Add(transform);
            SendTransform(transforms);
        }

        public void SendTransform(List<geometry_msgs.msg.TransformStamped> transforms)
        {
            var message = new tf2_msgs.msg.TFMessage();

            foreach (geometry_msgs.msg.TransformStamped value in transforms)
            {
                message.Transforms.Add(value);
            }

            tfPublisher.Publish(message);
        }
    }
}
