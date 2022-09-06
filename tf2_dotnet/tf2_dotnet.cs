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

using ROS2;
using ROS2.Utils;

namespace ROS2 {
  namespace TF2 {
    internal class TF2dotnetDelegates {
      internal static readonly DllLoadUtils dllLoadUtils;

      [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
      internal delegate void NativeTF2InitType ();
      internal static NativeTF2InitType native_tf2_init = null;

      [UnmanagedFunctionPointer (CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal delegate void NativeTF2AddTransformType (
        int sec, uint nanosec, [MarshalAs (UnmanagedType.LPStr)] string frame_id,
        [MarshalAs (UnmanagedType.LPStr)] string child_frame_id,
        double trans_x, double trans_y, double trans_z,
        double rot_x, double rot_y, double rot_z, double rot_w);
      internal static NativeTF2AddTransformType native_tf2_add_transform = null;

      [UnmanagedFunctionPointer (CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal delegate Transform NativeTF2LookUpTransformType (
        [MarshalAs (UnmanagedType.LPStr)] string frame_id,
        [MarshalAs (UnmanagedType.LPStr)] string child_frame_id,
        int sec, uint nanosec);
      internal static NativeTF2LookUpTransformType native_tf2_lookup_transform = null;

      [UnmanagedFunctionPointer (CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal delegate Transform NativeTF2LookUpLastTransformType (
        [MarshalAs (UnmanagedType.LPStr)] string frame_id,
        [MarshalAs (UnmanagedType.LPStr)] string child_frame_id);
      internal static NativeTF2LookUpLastTransformType native_tf2_lookup_last_transform = null;

      static TF2dotnetDelegates () {
        dllLoadUtils = DllLoadUtilsFactory.GetDllLoadUtils ();
        string library_name = "tf2_dotnet";
        IntPtr pDll = dllLoadUtils.LoadLibrary (library_name);

        IntPtr native_tf2_init_ptr = dllLoadUtils.GetProcAddress (pDll, "native_tf2_init");
        TF2dotnetDelegates.native_tf2_init = (NativeTF2InitType) Marshal.GetDelegateForFunctionPointer (
          native_tf2_init_ptr, typeof (NativeTF2InitType));

        IntPtr native_tf2_add_transform_ptr = dllLoadUtils.GetProcAddress (pDll, "native_tf2_add_transform");
        TF2dotnetDelegates.native_tf2_add_transform = (NativeTF2AddTransformType) Marshal.GetDelegateForFunctionPointer (
          native_tf2_add_transform_ptr, typeof (NativeTF2AddTransformType));

        IntPtr native_tf2_lookup_transform_ptr = dllLoadUtils.GetProcAddress (pDll, "native_tf2_lookup_transform");
        TF2dotnetDelegates.native_tf2_lookup_transform = (NativeTF2LookUpTransformType) Marshal.GetDelegateForFunctionPointer (
          native_tf2_lookup_transform_ptr, typeof (NativeTF2LookUpTransformType));

        IntPtr native_tf2_lookup_last_transform_ptr = dllLoadUtils.GetProcAddress (pDll, "native_tf2_lookup_last_transform");
        TF2dotnetDelegates.native_tf2_lookup_last_transform = (NativeTF2LookUpLastTransformType) Marshal.GetDelegateForFunctionPointer (
          native_tf2_lookup_last_transform_ptr, typeof (NativeTF2LookUpLastTransformType));
      }
    }

    public class TF2dotnet {
      public static void Init () {
        TF2dotnetDelegates.native_tf2_init ();
      }
    }

    public class TransformBroadcaster {

      private Publisher<tf2_msgs.msg.TFMessage> tf_pub_;

      public TransformBroadcaster(ref Node node) {
        tf_pub_ = node.CreatePublisher<tf2_msgs.msg.TFMessage> ("/tf");
      }

      public void SendTransform(ref geometry_msgs.msg.TransformStamped msgtf) {
        List<geometry_msgs.msg.TransformStamped> v1 = new List<geometry_msgs.msg.TransformStamped>();
        v1.Add(msgtf);
        SendTransform(ref v1);
      }

      public void SendTransform(ref List<geometry_msgs.msg.TransformStamped> msgtf) {
        tf2_msgs.msg.TFMessage message = new tf2_msgs.msg.TFMessage();

        foreach (geometry_msgs.msg.TransformStamped value in msgtf)
        {
          message.Transforms.Add(value);
        }

        tf_pub_.Publish(message);
      }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Transform {
      public int Sec;
      public uint Nanosec;
      public double Translation_x;
      public double Translation_y;
      public double Translation_z;
      public double Rotation_x;
      public double Rotation_y;
      public double Rotation_z;
      public double Rotation_w;
      public int Valid;
    }

    public class TransformListener {

      private Subscription<tf2_msgs.msg.TFMessage> tf_sub_;

      public TransformListener(ref Node node) {

        TF2dotnetDelegates.native_tf2_init ();

        tf_sub_ = node.CreateSubscription<tf2_msgs.msg.TFMessage> (
          "/tf",
          msg => {

            foreach (geometry_msgs.msg.TransformStamped transform in msg.Transforms) {
              TF2dotnetDelegates.native_tf2_add_transform (
                transform.Header.Stamp.Sec,
                transform.Header.Stamp.Nanosec,
                transform.Header.FrameId,
                transform.ChildFrameId,
                transform.Transform.Translation.X,
                transform.Transform.Translation.Y,
                transform.Transform.Translation.Z,
                transform.Transform.Rotation.X,
                transform.Transform.Rotation.Y,
                transform.Transform.Rotation.Z,
                transform.Transform.Rotation.W
              );
            }
          }
        );

      }

      public Transform LookUpTransform(
        System.String frame_from, System.String frame_to,
        Tuple<int, uint> time) {

        Transform transform = TF2dotnetDelegates.native_tf2_lookup_transform (frame_from, frame_to,
          time.Item1, time.Item2);

        return transform;
      }
      public Transform LookUpLastTransform(
        System.String frame_from, System.String frame_to) {

        Transform transform = TF2dotnetDelegates.native_tf2_lookup_last_transform (frame_from, frame_to);

        return transform;
      }

    }
  }
}
