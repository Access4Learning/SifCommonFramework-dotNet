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

using System.Collections.Generic;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Framework.Agent
{

    /// <summary>
    /// This interface defines common methods used by publishing Agents of this framework.
    /// </summary>
    interface IPublishingAgent
    {

        /// <summary>
        /// This method returns the Publishers used by a SIF Agent.
        /// </summary>
        /// <returns>Collection of Publishers.</returns>
        IList<IBasePublisher> GetPublishers();

    }

}
