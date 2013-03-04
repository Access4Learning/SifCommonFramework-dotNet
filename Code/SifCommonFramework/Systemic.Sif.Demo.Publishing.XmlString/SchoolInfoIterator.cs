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
using OpenADK.Library.au.School;
using Systemic.Sif.Framework.Model;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.XmlString
{

    /// <summary>
    /// This implementation simply works off an XML string.
    /// </summary>
    class SchoolInfoIterator : ISifEventIterator<SchoolInfo>, ISifResponseIterator<SchoolInfo>
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int eventMessageCount = 0;

        private bool onceOnly = true;
        private int responseMessageCount = 0;
        private SifParser sifParser = SifParser.NewInstance();
        private string[] messages = new string[]
        {
            @"
                <SchoolInfo RefId=""D3E34B359D75101A8C3D00AA001A1652"">
                  <LocalId>01011234</LocalId>
                  <StateProvinceId>01011234</StateProvinceId>
                  <CommonwealthId>012345</CommonwealthId>
                  <SchoolName>Lincoln Secondary College</SchoolName>
                  <SchoolDistrict> Southern Metropolitan Region</SchoolDistrict>
                  <SchoolType>Pri/Sec</SchoolType>
                  <SchoolSector>NG</SchoolSector>
                  <IndependentSchool>Y</IndependentSchool>
                  <NonGovSystemicStatus>S</NonGovSystemicStatus>
                  <System>0003</System>
                  <ReligiousAffiliation>2171</ReligiousAffiliation>
                  <LocalGovernmentArea>Cardinia</LocalGovernmentArea>
                  <SLA>205801452</SLA>
                </SchoolInfo>
            "
        };

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
        /// Simply return the SchoolInfo from the XML string representation.
        /// </summary>
        /// <returns>The next SIF Event; null if there are parsing errors with the message.</returns>
        public SifEvent<SchoolInfo> GetNextEvent()
        {
            SifEvent<SchoolInfo> sifEvent = null;

            try
            {
                SchoolInfo schoolInfo = (SchoolInfo)sifParser.Parse(messages[eventMessageCount]);
                if (log.IsDebugEnabled) log.Debug("Next SchoolInfo event record:\n" + schoolInfo.ToXml());
                sifEvent = new SifEvent<SchoolInfo>(schoolInfo, EventAction.Change);
            }
            catch (AdkParsingException e)
            {
                if (log.IsWarnEnabled) log.Warn("The following event message from SchoolInfoIterator has been ignored due to parsing errors: " + messages[eventMessageCount] + ".", e);
            }
            finally
            {
                eventMessageCount++;
            }

            return sifEvent;
        }

        /// <summary>
        /// Simply return the SchoolInfo from the XML string representation.
        /// </summary>
        /// <returns>The next response; null if there are parsing errors with the message.</returns>
        public SchoolInfo GetNextResponse()
        {
            SchoolInfo schoolInfo = null;

            try
            {
                schoolInfo = (SchoolInfo)sifParser.Parse(messages[responseMessageCount]);
                if (log.IsDebugEnabled) log.Debug("Next SchoolInfo response record:\n" + schoolInfo.ToXml());
            }
            catch (AdkParsingException e)
            {
                if (log.IsWarnEnabled) log.Warn("The following response message from SchoolInfoIterator has been ignored due to parsing errors: " + messages[eventMessageCount] + ".", e);
            }
            finally
            {
                responseMessageCount++;
            }

            return schoolInfo;
        }

        /// <summary>
        /// If the onceOnly flag is True, then return True for each message defined in the message list.
        /// If the onceOnly flag is False, then always return True.
        /// </summary>
        /// <returns>True if there are further events; false otherwise.</returns>
        public bool HasNextEvent()
        {
            bool hasNext = (eventMessageCount < messages.Length);

            if (!onceOnly && !hasNext)
            {
                eventMessageCount = 0;
            }

            return hasNext;
        }

        /// <summary>
        /// This method will return True for each message defined in the message list.
        /// </summary>
        /// <returns>True if there are further responses; false otherwise.</returns>
        public bool HasNextResponse()
        {
            return responseMessageCount < messages.Length;
        }

    }

}
