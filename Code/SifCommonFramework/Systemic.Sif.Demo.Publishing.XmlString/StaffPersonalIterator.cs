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
using Systemic.Sif.Framework.Model;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.XmlString
{

    /// <summary>
    /// This implementation simply works off an XML string.
    /// </summary>
    class StaffPersonalIterator : ISifEventIterator<StaffPersonal>, ISifResponseIterator<StaffPersonal>
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
                <StaffPersonal RefId=""D3E34F419D75101A8C3D00AA001A1652"">
                  <LocalId>946379881</LocalId>
                  <PersonInfo>
                    <Name Type=""LGL"">
                      <FamilyName>Smith</FamilyName>
                      <GivenName>Fred</GivenName>
                      <FullName>Fred Smith</FullName>
                    </Name>
                  </PersonInfo>
                </StaffPersonal>
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
        /// Simply return the StaffPersonal from the XML string representation.
        /// </summary>
        /// <returns>The next SIF Event; null if there are parsing errors with the message.</returns>
        public SifEvent<StaffPersonal> GetNextEvent()
        {
            SifEvent<StaffPersonal> sifEvent = null;

            try
            {
                StaffPersonal staffPersonal = (StaffPersonal)sifParser.Parse(messages[eventMessageCount]);
                if (log.IsDebugEnabled) log.Debug("Next StaffPersonal event record:\n" + staffPersonal.ToXml());
                sifEvent = new SifEvent<StaffPersonal>(staffPersonal, EventAction.Change);
            }
            catch (AdkParsingException e)
            {
                if (log.IsWarnEnabled) log.Warn("The following event message from StaffPersonalIterator has been ignored due to parsing errors: " + messages[eventMessageCount] + ".", e);
            }
            finally
            {
                eventMessageCount++;
            }

            return sifEvent;
        }

        /// <summary>
        /// Simply return the StaffPersonal from the XML string representation.
        /// </summary>
        /// <returns>The next response; null if there are parsing errors with the message.</returns>
        public StaffPersonal GetNextResponse()
        {
            StaffPersonal staffPersonal = null;

            try
            {
                staffPersonal = (StaffPersonal)sifParser.Parse(messages[responseMessageCount]);
                if (log.IsDebugEnabled) log.Debug("Next StaffPersonal response record:\n" + staffPersonal.ToXml());
            }
            catch (AdkParsingException e)
            {
                if (log.IsWarnEnabled) log.Warn("The following response message from StaffPersonalIterator has been ignored due to parsing errors: " + messages[eventMessageCount] + ".", e);
            }
            finally
            {
                responseMessageCount++;
            }

            return staffPersonal;
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
