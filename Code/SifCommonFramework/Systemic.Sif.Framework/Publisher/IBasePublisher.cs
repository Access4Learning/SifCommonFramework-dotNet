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

using OpenADK.Library;
using Systemic.Sif.Framework.Agent;

namespace Systemic.Sif.Framework.Publisher
{

    /// <summary>
    /// This interface defines common methods used by the Agents of this framework.
    /// </summary>
    public interface IBasePublisher : IAgentSettings, IPublisher
    {

        /// <summary>
        /// The SIF Data Object type associated with this Publisher.
        /// </summary>
        IElementDef SifObjectType { get; }

        /// <summary>
        /// This method is used to broadcast (publish) SIF Events at regular intervals.
        /// </summary>
        /// <param name="zones">Zones to publish to.</param>
        void StartEventProcessing(IZone[] zones);

    }

}
