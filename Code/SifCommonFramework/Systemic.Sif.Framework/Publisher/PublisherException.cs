/*
* Copyright 2010-2011 Systemic Pty Ltd
* 
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*    http://www.apache.org/licenses/LICENSE-2.0
* 
* Unless required by applicable law or agreed to in writing, software distributed under the License 
* is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
* or implied.
* See the License for the specific language governing permissions and limitations under the License.
*/

using System;

namespace Systemic.Sif.Framework.Publisher
{

    /// <summary>
    /// This class represents exceptions raised from the SIFCommon Framework iterators.
    /// </summary>
    public class PublisherException : ApplicationException
    {

        /// <summary>
        /// Default empty constructor.
        /// </summary>
        public PublisherException() : base()
        {
        }

        /// <summary>
        /// Constructor that holds an error message.
        /// </summary>
        /// <param name="message">Message associated with the exception.</param>
        public PublisherException(String message) : base(message)
        {
        }

        /// <summary>
        /// Constructor that hold an error message and the original exception.
        /// </summary>
        /// <param name="message">Message associated with the exception.</param>
        /// <param name="inner">Original exception.</param>
        public PublisherException(String message, Exception inner)
            : base(message)
        {
        }

    }

}
