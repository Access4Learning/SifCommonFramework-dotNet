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
using Edustructures.SifWorks.Student;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.Database
{

    /// <summary>
    /// Publisher of StudentPersonal data objects.
    /// </summary>
    public class StudentPersonalPublisher : GenericPublisher<StudentPersonal>
    {
        int eventFrequency = 20000;

        /// <summary>
        /// This implementation will define an event frequency of 20 seconds.
        /// </summary>
        public override int EventFrequency
        {
            get { return eventFrequency; }
            set { eventFrequency = value; }
        }

        /// <summary>
        /// Return a StudentPersonal iterator.
        /// </summary>
        /// <returns>StudentPersonal iterator.</returns>
        public override ISifEventIterator<StudentPersonal> GetSifEvents()
        {
            return new DatabaseRecordIterator(AgentConfiguration);
        }

        /// <summary>
        /// Return a StudentPersonal iterator.
        /// </summary>
        /// <param name="query">SIF Query associated with the data returned from the iterator.</param>
        /// <param name="zone">Zone that the iterator applies to.</param>
        /// <returns>StudentPersonal iterator.</returns>
        public override ISifResponseIterator<StudentPersonal> GetSifResponses(Query query, IZone zone)
        {
            return new DatabaseRecordIterator(AgentConfiguration);
        }

    }

}
