/*
 * Copyright 2015 Systemic Pty Ltd
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
using OpenADK.Library.au.Infrastructure;
using Systemic.Sif.Framework.Model;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.XmlString
{
    /// <summary>
    /// This implementation simply works off an XML string.
    /// </summary>

    class IdentityIterator : ISifEventIterator<Identity>, ISifResponseIterator<Identity>
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
                <Identity RefId=""4286194F43ED43C18EE2F0A27C4BEF86"">
                  <SIF_RefId SIF_RefObject=""StudentPersonal"">23B08571E4D645C3B82A3E52E5349925</SIF_RefId>
                  <AuthenticationSource>MSActiveDirectory</AuthenticationSource>
                  <IdentityAssertions>
                    <IdentityAssertion SchemaName=""sAmAccountName"">user01</IdentityAssertion>
                    <IdentityAssertion SchemaName=""userPrincipalName"">user01@asdf.edu.au</IdentityAssertion>
                    <IdentityAssertion SchemaName=""distinguishedName"">cn=User01,cn=Users,dc=org</IdentityAssertion>
                  </IdentityAssertions>
                  <AuthenticationSourceGlobalUID>A9A6CB2BC49344278C1FD6587D448B35</AuthenticationSourceGlobalUID>
                </Identity>
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
        /// Simply return the Identity from the XML string representation.
        /// </summary>
        /// <returns>The next SIF Event; null if there are parsing errors with the message.</returns>
        public SifEvent<Identity> GetNextEvent()
        {
            SifEvent<Identity> sifEvent = null;

            try
            {
                Identity identity = (Identity)sifParser.Parse(messages[eventMessageCount]);
                if (log.IsDebugEnabled) log.Debug("Next Identity event record:\n" + identity.ToXml());
                sifEvent = new SifEvent<Identity>(identity, EventAction.Change);
            }
            catch (AdkParsingException e)
            {
                if (log.IsWarnEnabled) log.Warn("The following event message from IdentityIterator has been ignored due to parsing errors: " + messages[eventMessageCount] + ".", e);
            }
            finally
            {
                eventMessageCount++;
            }

            return sifEvent;
        }

        /// <summary>
        /// Simply return the Identity from the XML string representation.
        /// </summary>
        /// <returns>The next response; null if there are parsing errors with the message.</returns>
        public Identity GetNextResponse()
        {
            Identity identity = null;

            try
            {
                identity = (Identity)sifParser.Parse(messages[responseMessageCount]);
                if (log.IsDebugEnabled) log.Debug("Next Identity response record:\n" + identity.ToXml());
            }
            catch (AdkParsingException e)
            {
                if (log.IsWarnEnabled) log.Warn("The following response message from IdentityIterator has been ignored due to parsing errors: " + messages[responseMessageCount] + ".", e);
            }
            finally
            {
                responseMessageCount++;
            }

            return identity;
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
