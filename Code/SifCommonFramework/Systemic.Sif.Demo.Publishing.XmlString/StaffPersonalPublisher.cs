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
using OpenADK.Library.au.Student;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.XmlString
{

    /// <summary>
    /// Publisher of StaffPersonal data objects.
    /// </summary>
    public class StaffPersonalPublisher : GenericPublisher<StaffPersonal>
    {

        /// <summary>
        /// Return a StaffPersonal iterator.
        /// </summary>
        /// <returns>StaffPersonal iterator.</returns>
        public override ISifEventIterator<StaffPersonal> GetSifEvents()
        {
            return new StaffPersonalIterator();
        }

        /// <summary>
        /// Return a StaffPersonal iterator.
        /// </summary>
        /// <param name="query">SIF Query associated with the data returned from the iterator.</param>
        /// <param name="zone">Zone that the iterator applies to.</param>
        /// <returns>StaffPersonal iterator.</returns>
        public override ISifResponseIterator<StaffPersonal> GetSifResponses(Query query, IZone zone)
        {
            return new StaffPersonalIterator();
        }

    }

}
