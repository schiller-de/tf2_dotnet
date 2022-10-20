/* Copyright 2022 Stefan Hoffmann <stefan.hoffmann@schiller.de>
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

using geometry_msgs.msg;

namespace Ros2.Tf2DotNet
{
    public static class Tf2Conversions
    {
        public static PoseStamped ToPoseStamped(this TransformStamped transform)
        {
            var result = new PoseStamped();
            result.Header = transform.Header;
            result.Pose.Position.X = transform.Transform.Translation.X;
            result.Pose.Position.Y = transform.Transform.Translation.Y;
            result.Pose.Position.Z = transform.Transform.Translation.Z;
            result.Pose.Orientation = transform.Transform.Rotation;
            return result;
        }
    }
}
