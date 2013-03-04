/*
* Copyright 2010-2013 Systemic Pty Ltd
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
using System.Data.Common;

namespace Systemic.Sif.Demo.Publishing.Database
{

    /// <summary>
    /// This class represents general database exceptions.
    /// </summary>
    class AgentDbException : DbException
    {

        /// <summary>
        /// Initialises a new instance of this class.
        /// </summary>
        public AgentDbException()
            : base()
        {
        }

        /// <summary>
        /// Initialises a new instance of this class with the specified error message.
        /// </summary>
        /// <param name="message">The message to display for this exception.</param>
        public AgentDbException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Initialises a new instance of this class with the specified error message and a reference to the inner
        /// exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message string.</param>
        /// <param name="inner">The inner exception reference.</param>
        public AgentDbException(String message, Exception inner)
            : base(message)
        {
        }

    }

}
