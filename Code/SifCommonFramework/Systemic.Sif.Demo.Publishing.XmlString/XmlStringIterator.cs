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
using Systemic.Sif.Framework.Model;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.XmlString
{

    /// <summary>
    /// This implementation simply works off an XML string.
    /// </summary>
    class XmlStringIterator : ISifEventIterator<StudentPersonal>, ISifResponseIterator<StudentPersonal>
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int eventCount = 0;
        private int responseCount = 0;
        private SifParser sifParser = SifParser.NewInstance();
        private string xml =
            @"
              <StudentPersonal RefId=""7C834EA9EDA12090347F83297E1C290C"">
                <LocalId>S1234567</LocalId>
                <PersonInfo>
                  <Name Type=""LGL"">
                    <FamilyName>Smith</FamilyName>
                    <GivenName>Fred</GivenName>
                    <FullName>Fred Smith</FullName>
                  </Name>
                </PersonInfo>
              </StudentPersonal>
            ";

        /// <summary>
        /// No implementation.
        /// </summary>
        public void AfterEvent()
        {
        }

        /// <summary>
        /// No implementation.
        /// </summary>
        public void AfterResponse()
        {
        }

        /// <summary>
        /// No implementation.
        /// </summary>
        public void BeforeEvent()
        {
        }

        /// <summary>
        /// No implementation.
        /// </summary>
        public void BeforeResponse()
        {
        }

        /// <summary>
        /// Simply return the StudentPersonal from the XML string representation.
        /// </summary>
        /// <returns>The next SIF Event.</returns>
        public SifEvent<StudentPersonal> GetNextEvent()
        {
            eventCount++;
            StudentPersonal studentPersonal = (StudentPersonal)sifParser.Parse(xml);
            if (log.IsDebugEnabled) log.Debug("XmlStringIterator data " + studentPersonal.ToXml() + ".");
            return new SifEvent<StudentPersonal>(studentPersonal, EventAction.Change);
        }

        /// <summary>
        /// Simply return the StudentPersonal from the XML string representation.
        /// </summary>
        /// <returns>The next response.</returns>
        public StudentPersonal GetNextResponse()
        {
            responseCount++;
            return (StudentPersonal)sifParser.Parse(xml);
        }

        /// <summary>
        /// Implemented to return True once only.
        /// </summary>
        /// <returns>True if there are further events; false otherwise.</returns>
        public bool HasNextEvent()
        {
            return (eventCount == 0);
        }

        /// <summary>
        /// Implemented to return True once only.
        /// </summary>
        /// <returns>True if there are further responses; false otherwise.</returns>
        public bool HasNextResponse()
        {
            return (responseCount == 0);
        }

    }

}
