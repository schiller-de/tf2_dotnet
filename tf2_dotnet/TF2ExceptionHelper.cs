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

namespace ROS2.TF2
{
    // Mirror of Tf2DotnetExceptionType in C++, keep in Sync!
    internal enum TF2ExceptionType : int
    {
        NoException = 0,
        LookupException = 1,
        ConnectivityException = 2,
        ExtrapolationException = 3,
        InvalidArgumentException = 4,
        TimeoutException = 5,
        TransformException = 6,
        Exception = 1000, // std:runtime_error
        UnknownException = 1001, // all other types
    }

    internal static class TF2ExceptionHelper
    {
        // Mirror of TF2_DOTNET_EXCEPTION_MESSAGE_BUFFER_LENGTH in C++, keep in Sync!
        public const int MessageBufferLength = 256;

        [ThreadStatic]
        public static byte[] MessageBuffer;

        public static void ResetMessage()
        {
            if (MessageBuffer == null)
            {
                MessageBuffer = new byte[MessageBufferLength];
            }
            else
            {
                Array.Clear(MessageBuffer, 0, MessageBufferLength);
            }
        }

        public static void ThrowIfHasException(TF2ExceptionType exceptionType)
        {
            if (exceptionType != TF2ExceptionType.NoException)
            {
                throw CreateFromExceptionType(exceptionType);
            }
        }

        public static Exception CreateFromExceptionType(TF2ExceptionType exceptionType)
        {
            try
            {
                 switch (exceptionType)
                {
                    case TF2ExceptionType.LookupException:
                        return new LookupException(GetMessage());

                    case TF2ExceptionType.ConnectivityException:
                        return new ConnectivityException(GetMessage());

                    case TF2ExceptionType.ExtrapolationException:
                        return new ExtrapolationException(GetMessage());

                    case TF2ExceptionType.InvalidArgumentException:
                        // Use already defined System.ArgumentException.
                        return new ArgumentException(GetMessage());

                    case TF2ExceptionType.TimeoutException:
                        // Use already defined System.TimeoutException.
                        return new TimeoutException(GetMessage());

                    case TF2ExceptionType.TransformException:
                        return new TransformException(GetMessage());

                    case TF2ExceptionType.Exception:
                        return new Exception(GetMessage());

                    case TF2ExceptionType.UnknownException:
                        return new Exception("Unknown C++ exception was thrown, no message available.");
                    
                    case TF2ExceptionType.NoException:
                    default:
                        throw new InvalidOperationException("No valid exception info set.");
                }
            }
            finally
            {
                ResetMessage();
            }
        }
    
        private static string GetMessage()
        {
            if (MessageBuffer == null)
            {
                return "";
            }

            string message = System.Text.Encoding.UTF8.GetString(MessageBuffer).TrimEnd('\0');
            return message;
        }
    }
}
