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
using builtin_interfaces.msg;
using geometry_msgs.msg;
using ROS2;
using ROS2.Utils;

namespace Ros2 {
  namespace Tf2DotNet {
    internal class Tf2DotNetDelegates {
      internal static readonly DllLoadUtils dllLoadUtils;

      [UnmanagedFunctionPointer (CallingConvention.Cdecl)]
      internal delegate void NativeTF2InitType (
        out Tf2ExceptionType exceptionType,
        byte[] exceptionMessageBuffer);
      internal static NativeTF2InitType native_tf2_init = null;

      [UnmanagedFunctionPointer (CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal delegate void NativeTF2AddTransformType (
        int sec, uint nanosec, [MarshalAs (UnmanagedType.LPStr)] string frame_id,
        [MarshalAs (UnmanagedType.LPStr)] string child_frame_id,
        double trans_x, double trans_y, double trans_z,
        double rot_x, double rot_y, double rot_z, double rot_w, int is_static,
        out Tf2ExceptionType exceptionType,
        byte[] exceptionMessageBuffer);
      internal static NativeTF2AddTransformType native_tf2_add_transform = null;

      [UnmanagedFunctionPointer (CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal delegate Transform NativeTF2LookUpTransformType (
        [MarshalAs (UnmanagedType.LPStr)] string targetFrame,
        [MarshalAs (UnmanagedType.LPStr)] string sourceFrame,
        int sec, uint nanosec,
        out Tf2ExceptionType exceptionType,
        byte[] exceptionMessageBuffer);
      internal static NativeTF2LookUpTransformType native_tf2_lookup_transform = null;

      [UnmanagedFunctionPointer (CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
      internal delegate Transform NativeTF2LookUpLastTransformType (
        [MarshalAs (UnmanagedType.LPStr)] string targetFrame,
        [MarshalAs (UnmanagedType.LPStr)] string sourceFrame,
        out Tf2ExceptionType exceptionType,
        byte[] exceptionMessageBuffer);
      internal static NativeTF2LookUpLastTransformType native_tf2_lookup_last_transform = null;

      static Tf2DotNetDelegates () {
        dllLoadUtils = DllLoadUtilsFactory.GetDllLoadUtils ();
        string library_name = "tf2_dotnet";
        IntPtr pDll = dllLoadUtils.LoadLibrary (library_name);

        IntPtr native_tf2_init_ptr = dllLoadUtils.GetProcAddress (pDll, "native_tf2_init");
        Tf2DotNetDelegates.native_tf2_init = (NativeTF2InitType) Marshal.GetDelegateForFunctionPointer (
          native_tf2_init_ptr, typeof (NativeTF2InitType));

        IntPtr native_tf2_add_transform_ptr = dllLoadUtils.GetProcAddress (pDll, "native_tf2_add_transform");
        Tf2DotNetDelegates.native_tf2_add_transform = (NativeTF2AddTransformType) Marshal.GetDelegateForFunctionPointer (
          native_tf2_add_transform_ptr, typeof (NativeTF2AddTransformType));

        IntPtr native_tf2_lookup_transform_ptr = dllLoadUtils.GetProcAddress (pDll, "native_tf2_lookup_transform");
        Tf2DotNetDelegates.native_tf2_lookup_transform = (NativeTF2LookUpTransformType) Marshal.GetDelegateForFunctionPointer (
          native_tf2_lookup_transform_ptr, typeof (NativeTF2LookUpTransformType));

        IntPtr native_tf2_lookup_last_transform_ptr = dllLoadUtils.GetProcAddress (pDll, "native_tf2_lookup_last_transform");
        Tf2DotNetDelegates.native_tf2_lookup_last_transform = (NativeTF2LookUpLastTransformType) Marshal.GetDelegateForFunctionPointer (
          native_tf2_lookup_last_transform_ptr, typeof (NativeTF2LookUpLastTransformType));
      }
    }

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

    public class TransformListener {

      private Subscription<tf2_msgs.msg.TFMessage> tf_sub_;
      private Subscription<tf2_msgs.msg.TFMessage> tf_static_sub_;

      private static readonly object _lock = new object();
      private static bool _initialized = false;

      public TransformListener(Node node) {

        lock(_lock)
        {
          if (!_initialized)
          {
            Tf2ExceptionHelper.ResetMessage();
            Tf2DotNetDelegates.native_tf2_init(out Tf2ExceptionType exceptionType, Tf2ExceptionHelper.MessageBuffer);
            Tf2ExceptionHelper.ThrowIfHasException(exceptionType);

            _initialized = true;
          }
          else
          {
            throw new NotSupportedException("Only one TransformListener per process is supported right now.");
          }
        }

        tf_sub_ = node.CreateSubscription<tf2_msgs.msg.TFMessage> (
          "/tf",
          msg => {

            foreach (geometry_msgs.msg.TransformStamped transform in msg.Transforms) {

              Tf2ExceptionHelper.ResetMessage();

              Tf2DotNetDelegates.native_tf2_add_transform (
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
                transform.Transform.Rotation.W,
                is_static: 0,
                out Tf2ExceptionType exceptionType,
                Tf2ExceptionHelper.MessageBuffer
              );

              Tf2ExceptionHelper.ThrowIfHasException(exceptionType);
            }
          }
        );

        tf_static_sub_ = node.CreateSubscription<tf2_msgs.msg.TFMessage> (
          "/tf_static",
          msg => {

            foreach (geometry_msgs.msg.TransformStamped transform in msg.Transforms) {

              Tf2ExceptionHelper.ResetMessage();

              Tf2DotNetDelegates.native_tf2_add_transform (
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
                transform.Transform.Rotation.W,
                is_static: 1,
                out Tf2ExceptionType exceptionType,
                Tf2ExceptionHelper.MessageBuffer
              );

              Tf2ExceptionHelper.ThrowIfHasException(exceptionType);

            }
          }
        );

      }

      public TransformStamped LookupTransform(
        string targetFrame, string sourceFrame,
        Time time) {

        Tf2ExceptionHelper.ResetMessage();

        Transform transform = Tf2DotNetDelegates.native_tf2_lookup_transform(
          targetFrame,
          sourceFrame,
          time.Sec,
          time.Nanosec,
          out Tf2ExceptionType exceptionType,
          Tf2ExceptionHelper.MessageBuffer);

        Tf2ExceptionHelper.ThrowIfHasException(exceptionType);

        TransformStamped transformStamped = transform.ToTransformStamped(targetFrame, sourceFrame);
        return transformStamped;
      }

      public TransformStamped LookupTransform(
        string targetFrame, string sourceFrame) {

        Tf2ExceptionHelper.ResetMessage();

        Transform transform = Tf2DotNetDelegates.native_tf2_lookup_last_transform(
          targetFrame,
          sourceFrame,
          out Tf2ExceptionType exceptionType,
          Tf2ExceptionHelper.MessageBuffer);

        Tf2ExceptionHelper.ThrowIfHasException(exceptionType);

        TransformStamped transformStamped = transform.ToTransformStamped(targetFrame, sourceFrame);
        return transformStamped;
      }

    }
  }
}
