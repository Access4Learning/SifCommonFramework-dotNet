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

using Edustructures.SifWorks;

namespace Systemic.Sif.Framework.Model
{

    /// <summary>
    /// A wrapper class for associating a SIF Event with a SIF Data Object.
    /// </summary>
    public class SifEvent<T> where T : SifDataObject
    {

        /// <summary>
        /// The action (type of event) associated with the SIF Event.
        /// </summary>
        public EventAction EventAction { get; set; }

        /// <summary>
        /// The SIF Data Object associated with the SIF Event.
        /// </summary>
        public T SifDataObject { get; set; }

        /// <summary>
        /// Create an instance of a SIF Event with the specified SIF Data Object and action.
        /// </summary>
        /// <param name="sifDataObject">SIF Datas Object associated with the SIF Event.</param>
        /// <param name="eventAction">Action associated with the SIF Event.</param>
        public SifEvent(T sifDataObject, EventAction eventAction)
        {
            SifDataObject = sifDataObject;
            EventAction = eventAction;
        }

    }

}
