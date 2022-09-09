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

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
        }
    }
}
