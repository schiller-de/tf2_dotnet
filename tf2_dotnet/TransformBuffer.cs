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

using System;
using builtin_interfaces.msg;
using geometry_msgs.msg;

namespace Ros2.Tf2DotNet
{
    // Using "Buffer" as type name like in c++ conflicts with "System.Buffer",
    // so using "TransformBuffer" instead.
    //
    // Make sure to implement the Dispose Pattern when unsealing this class.
    public sealed class TransformBuffer : IDisposable
    {
        private readonly SafeBufferCoreHandle _handle;
        private bool _disposed = false;

        public TransformBuffer()
        {
            Tf2ExceptionHelper.ResetMessage();
            _handle = Interop.tf2_dotnet_native_buffer_core_create(out Tf2ExceptionType exceptionType, Tf2ExceptionHelper.MessageBuffer);
            Tf2ExceptionHelper.ThrowIfHasException(exceptionType);
        }

        public void Dispose()
        {
            _handle.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// Add transform information to the tf data structure.
        /// </summary>
        /// <param name="transform">The transform to store.</param>
        /// <param name="authority">The source of the information for this transform.</param>
        /// <param name="isStatic">Record this transform as a static transform. It will be good across all time. (This cannot be changed after the first call.)</param>
        /// <returns>True unless an error occurred.</returns>
        public bool SetTransform(TransformStamped transform, string authority, bool isStatic = false)
        {
            ThrowIfDisposed();

            Tf2ExceptionHelper.ResetMessage();

            int result = Interop.tf2_dotnet_native_buffer_core_set_transform(
                _handle,
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
                authority,
                isStatic ? 1 : 0,
                out Tf2ExceptionType exceptionType,
                Tf2ExceptionHelper.MessageBuffer);

            Tf2ExceptionHelper.ThrowIfHasException(exceptionType);
            
            return result == 1;
        }

        /// <summary>
        /// Get the transform between two frames by frame ID.
        /// </summary>
        /// <param name="targetFrame">The frame to which data should be transformed.</param>
        /// <param name="sourceFrame">The frame where the data originated.</param>
        /// <param name="time">The time at which the value of the transform is desired. (<c>null</c> will get the latest)</param>
        /// <returns>The transform between the frames.</returns>
        /// <exception cref="LookupException"></exception>
        /// <exception cref="ConnectivityException"></exception>
        /// <exception cref="ExtrapolationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public TransformStamped LookupTransform(
            string targetFrame,
            string sourceFrame,
            Time time = null)
        {
            ThrowIfDisposed();

            int sec;
            uint nanosec;
            if (time != null)
            {
                sec = time.Sec;
                nanosec = time.Nanosec;
            }
            else
            {
                sec = 0;
                nanosec = 0;
            }

            Tf2ExceptionHelper.ResetMessage();

            Transform transform = Interop.tf2_dotnet_native_buffer_core_lookup_transform(
                _handle,
                targetFrame,
                sourceFrame,
                sec,
                nanosec,
                out Tf2ExceptionType exceptionType,
                Tf2ExceptionHelper.MessageBuffer);

            Tf2ExceptionHelper.ThrowIfHasException(exceptionType);

            TransformStamped transformStamped = transform.ToTransformStamped(targetFrame, sourceFrame);
            return transformStamped;
        }

        /// <summary>
        /// Get the transform between two frames by frame ID assuming fixed frame.
        /// </summary>
        /// <param name="targetFrame">The frame to which data should be transformed.</param>
        /// <param name="targetTime">The time to which the data should be transformed. (<c>null</c> will get the latest)</param>
        /// <param name="sourceFrame">The frame where the data originated.</param>
        /// <param name="sourceTime">The time at which the source_frame should be evaluated. (<c>null</c> will get the latest)</param>
        /// <param name="fixedFrame">The frame in which to assume the transform is constant in time.</param>
        /// <returns>The transform between the frames.</returns>
        /// <exception cref="LookupException"></exception>
        /// <exception cref="ConnectivityException"></exception>
        /// <exception cref="ExtrapolationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public TransformStamped LookupTransform(
            string targetFrame,
            Time targetTime,
            string sourceFrame,
            Time sourceTime,
            string fixedFrame)
        {
            ThrowIfDisposed();

            int targetSec;
            uint targetNanosec;
            if (targetTime != null)
            {
                targetSec = targetTime.Sec;
                targetNanosec = targetTime.Nanosec;
            }
            else
            {
                targetSec = 0;
                targetNanosec = 0;
            }

            int sourceSec;
            uint sourceNanosec;
            if (sourceTime != null)
            {
                sourceSec = sourceTime.Sec;
                sourceNanosec = sourceTime.Nanosec;
            }
            else
            {
                sourceSec = 0;
                sourceNanosec = 0;
            }

            Tf2ExceptionHelper.ResetMessage();

            Transform transform = Interop.tf2_dotnet_native_buffer_core_lookup_transform_full(
                _handle,
                targetFrame,
                targetSec,
                targetNanosec,
                sourceFrame,
                sourceSec,
                sourceNanosec,
                fixedFrame,
                out Tf2ExceptionType exceptionType,
                Tf2ExceptionHelper.MessageBuffer);

            Tf2ExceptionHelper.ThrowIfHasException(exceptionType);

            TransformStamped transformStamped = transform.ToTransformStamped(targetFrame, sourceFrame);
            return transformStamped;
        }

        /// <summary>
        /// Test if a transform is possible.
        /// </summary>
        /// <param name="targetFrame">The frame into which to transform.</param>
        /// <param name="sourceFrame">The frame from which to transform.</param>
        /// <param name="time">The time at which to transform.</param>
        /// <param name="errorMessage">The error message why the transform failed.</param>
        /// <returns>True if the transform is possible, false otherwise.</returns>
        public bool CanTransform(
            string targetFrame,
            string sourceFrame,
            Time time,
            out string errorMessage)
        {
            ThrowIfDisposed();

            int sec;
            uint nanosec;
            if (time != null)
            {
                sec = time.Sec;
                nanosec = time.Nanosec;
            }
            else
            {
                sec = 0;
                nanosec = 0;
            }

            var errorMessageBuffer = new byte[Tf2ExceptionHelper.MessageBufferLength];

            Tf2ExceptionHelper.ResetMessage();

            int result = Interop.tf2_dotnet_native_buffer_core_can_transform(
                _handle,
                targetFrame,
                sourceFrame,
                sec,
                nanosec,
                errorMessageBuffer,
                out Tf2ExceptionType exceptionType,
                Tf2ExceptionHelper.MessageBuffer);

            Tf2ExceptionHelper.ThrowIfHasException(exceptionType);

            errorMessage = System.Text.Encoding.UTF8.GetString(errorMessageBuffer).TrimEnd('\0');

            return result == 1;
        }

        /// <summary>
        /// Test if a transform is possible.
        /// </summary>
        /// <param name="targetFrame">The frame into which to transform.</param>
        /// <param name="targetTime">The time into which to transform.</param>
        /// <param name="sourceFrame">The frame from which to transform.</param>
        /// <param name="sourceTime">The time from which to transform.</param>
        /// <param name="fixedFrame">The frame in which to treat the transform as constant in time.</param>
        /// <param name="errorMessage">The error message why the transform failed.</param>
        /// <returns>True if the transform is possible, false otherwise.</returns>
        public bool CanTransform(
            string targetFrame,
            Time targetTime,
            string sourceFrame,
            Time sourceTime,
            string fixedFrame,
            out string errorMessage)
        {
            ThrowIfDisposed();

            int targetSec;
            uint targetNanosec;
            if (targetTime != null)
            {
                targetSec = targetTime.Sec;
                targetNanosec = targetTime.Nanosec;
            }
            else
            {
                targetSec = 0;
                targetNanosec = 0;
            }

            int sourceSec;
            uint sourceNanosec;
            if (sourceTime != null)
            {
                sourceSec = sourceTime.Sec;
                sourceNanosec = sourceTime.Nanosec;
            }
            else
            {
                sourceSec = 0;
                sourceNanosec = 0;
            }
            
            var errorMessageBuffer = new byte[Tf2ExceptionHelper.MessageBufferLength];

            Tf2ExceptionHelper.ResetMessage();

            int result = Interop.tf2_dotnet_native_buffer_core_can_transform_full(
                _handle,
                targetFrame,
                targetSec,
                targetNanosec,
                sourceFrame,
                sourceSec,
                sourceNanosec,
                fixedFrame,
                errorMessageBuffer,
                out Tf2ExceptionType exceptionType,
                Tf2ExceptionHelper.MessageBuffer);

            Tf2ExceptionHelper.ThrowIfHasException(exceptionType);

            errorMessage = System.Text.Encoding.UTF8.GetString(errorMessageBuffer).TrimEnd('\0');

            return result == 1;
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
