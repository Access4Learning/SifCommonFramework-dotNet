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
using OpenADK.Library.au.Student;
using OpenADK.Library.Tools.Cfg;
using OpenADK.Library.Tools.Mapping;
using Systemic.Sif.Framework.Model;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.XmlFile
{

    /// <summary>
    /// This implementation simply works off an XML string read from a file (SIFWorks ADK configuration file).
    /// </summary>
    public class XmlFileIterator : ISifEventIterator<StudentPersonal>, ISifResponseIterator<StudentPersonal>
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private int eventCount = 0;
        private ObjectMapping objectMapping;
        private int responseCount = 0;
        private SifParser sifParser = SifParser.NewInstance();
        private StudentPersonal studentPersonal;

        /// <summary>
        /// Create an instance using the SIFWorks ADK configuration file.
        /// </summary>
        /// <param name="agentConfig">Configuration file for the SIF Agent.</param>
        public XmlFileIterator(AgentConfig agentConfig)
        {
            Mappings mappings = agentConfig.Mappings.GetMappings("Default");

            if (mappings == null)
            {
                throw new IteratorException("<mappings id=\"Default\"> has not been specified for agent " + agentConfig.SourceId + ".");
            }

            objectMapping = mappings.GetObjectMapping(typeof(StudentPersonal).Name, true);
            string xml = objectMapping.XmlElement.InnerXml;
            if (log.IsDebugEnabled) log.Debug("Object mapping is: " + objectMapping.XmlElement.InnerXml);
            IElementDef elementDef = Adk.Dtd.LookupElementDef(objectMapping.ObjectType);
            studentPersonal = (StudentPersonal)sifParser.Parse(xml);
        }

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
            return new SifEvent<StudentPersonal>(studentPersonal, EventAction.Change);
        }

        /// <summary>
        /// Simply return the StudentPersonal from the XML string representation.
        /// </summary>
        /// <returns>The next response.</returns>
        public StudentPersonal GetNextResponse()
        {
            responseCount++;
            return studentPersonal;
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
