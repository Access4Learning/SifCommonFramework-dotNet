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

using OpenADK.Library;
using Systemic.Sif.Framework.Model;

namespace Systemic.Sif.Framework.Publisher
{

    /// <summary>
    /// This interface allows a user to manage how a Publisher loads SIF Events without having to load all events into
    /// memory in a single call.
    /// </summary>
    public interface ISifEventIterator<T> where T : SifDataObject
    {

        /// <summary>
        /// Post-processing that occurs after every call to getNextEvent().
        /// </summary>
        /// <exception cref="Systemic.Sif.Framework.Publisher.IteratorException">All errors should be wrapped by this exception.</exception>
        void AfterEvent();

        /// <summary>
        /// Pre-processing that occurs before every call to getNextEvent().
        /// </summary>
        /// <exception cref="Systemic.Sif.Framework.Publisher.IteratorException">All errors should be wrapped by this exception.</exception>
        void BeforeEvent();

        /// <summary>
        /// This method returns the next available SIF Event.
        /// </summary>
        /// <returns>The next available SIF Event. This must return null if there are no further SIF Events avalable.</returns>
        /// <exception cref="Systemic.Sif.Framework.Publisher.IteratorException">All errors should be wrapped by this exception.</exception>
        SifEvent<T> GetNextEvent();

        /// <summary>
        /// This method will check whether there are more SIF Events available, i.e. a call to getNextEvent() will not
        /// return null.
        /// </summary>
        /// <returns>True if there are more SIF Events available; False otherwise.</returns>
        /// <exception cref="Systemic.Sif.Framework.Publisher.IteratorException">All errors should be wrapped by this exception.</exception>
        bool HasNextEvent();
    
    }

}
