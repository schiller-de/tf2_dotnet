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
    public class TransformException : Exception
    {
        // Allow only internal subclasses.
        internal TransformException(string message)
            : base(message)
        {
        }
    }

    public sealed class ConnectivityException : TransformException
    {
        public ConnectivityException(string message)
            : base(message)
        {
        }
    }

    public sealed class LookupException : TransformException
    {
        public LookupException(string message)
            : base(message)
        {
        }
    }

    public sealed class ExtrapolationException : TransformException
    {
        public ExtrapolationException(string message)
            : base(message)
        {
        }
    }

    // Use System.ArgumentException instead of defining a new one.
    // public sealed class InvalidArgumentException : TransformException  { ... }

    // Use System.TimeoutException instead of defining a new one.
    // public sealed class TimeoutException : TransformException { ... }
}
