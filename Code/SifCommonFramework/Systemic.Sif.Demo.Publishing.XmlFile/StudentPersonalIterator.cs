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
using OpenADK.Library;
using OpenADK.Library.au.Student;
using OpenADK.Library.Tools.Cfg;
using OpenADK.Library.Tools.Mapping;
using Systemic.Sif.Framework.Model;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.XmlFile
{

    /// <summary>
    /// This implementation simply works off an XML string read from a file (Agent configuration file).
    /// </summary>
    public class StudentPersonalIterator : ISifEventIterator<StudentPersonal>, ISifResponseIterator<StudentPersonal>
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int eventMessageCount = 0;

        private bool onceOnly = true;
        private int responseMessageCount = 0;
        private ObjectMapping objectMapping;
        private SifParser sifParser = SifParser.NewInstance();
        private StudentPersonal studentPersonal;

        /// <summary>
        /// Create an instance using the Agent configuration file.
        /// </summary>
        /// <param name="agentConfig">Configuration file for the SIF Agent.</param>
        /// <exception cref="System.ArgumentException">agentConfig parameter is null.</exception>
        /// <exception cref="Systemic.Sif.Framework.Publisher.IteratorException">Error parsing student details from the Agent configuration file.</exception>
        public StudentPersonalIterator(AgentConfig agentConfig)
        {

            if (agentConfig == null)
            {
                throw new ArgumentException("agentConfig parameter is null.");
            }

            Mappings mappings = agentConfig.Mappings.GetMappings("Default");

            if (mappings == null)
            {
                throw new IteratorException("<mappings id=\"Default\"> has not been specified for Agent " + agentConfig.SourceId + ".");
            }

            objectMapping = mappings.GetObjectMapping(typeof(StudentPersonal).Name, true);

            if (objectMapping == null)
            {
                throw new IteratorException("An object mapping for StudentPersonal has not been specified for Agent " + agentConfig.SourceId + ".");
            }

            string xml = objectMapping.XmlElement.InnerXml;
            IElementDef elementDef = Adk.Dtd.LookupElementDef(objectMapping.ObjectType);

            try
            {
                studentPersonal = (StudentPersonal)sifParser.Parse(xml);
            }
            catch (AdkParsingException e)
            {
                throw new IteratorException("The following event message from StudentPersonalIterator has parsing errors: " + xml + ".", e);
            }

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
        /// Simply return the StudentPersonal read from the Agent configuration file.
        /// </summary>
        /// <returns>The next SIF Event.</returns>
        public SifEvent<StudentPersonal> GetNextEvent()
        {
            if (log.IsDebugEnabled) log.Debug("Next StudentPersonal event record:\n" + studentPersonal.ToXml());
            eventMessageCount++;
            return new SifEvent<StudentPersonal>(studentPersonal, EventAction.Change);
        }

        /// <summary>
        /// Simply return the StudentPersonal read from the Agent configuration file.
        /// </summary>
        /// <returns>The next response.</returns>
        public StudentPersonal GetNextResponse()
        {
            if (log.IsDebugEnabled) log.Debug("Next StudentPersonal response record:\n" + studentPersonal.ToXml());
            responseMessageCount++;
            return studentPersonal;
        }

        /// <summary>
        /// If the onceOnly flag is True, then return True for each message read from the Agent configuration file.
        /// If the onceOnly flag is False, then always return True.
        /// </summary>
        /// <returns>True if there are further events; false otherwise.</returns>
        public bool HasNextEvent()
        {
            bool hasNext = (eventMessageCount < 1);

            if (!onceOnly && !hasNext)
            {
                eventMessageCount = 0;
            }

            return hasNext;
        }

        /// <summary>
        /// This method will return True for each message read from the Agent configuration file.
        /// </summary>
        /// <returns>True if there are further responses; false otherwise.</returns>
        public bool HasNextResponse()
        {
            return responseMessageCount < 1;
        }

    }

}
