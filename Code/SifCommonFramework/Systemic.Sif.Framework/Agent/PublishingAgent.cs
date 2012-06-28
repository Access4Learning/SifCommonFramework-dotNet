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

using System;
using System.Collections.Generic;
using System.Reflection;
using OpenADK.Library;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Framework.Agent
{

    /// <summary>
    /// A publishing Agent that runs based upon settings from a pre-defined SIFWorks ADK configuration file.
    /// </summary>
    public abstract class PublishingAgent : BaseAgent, IPublishingAgent
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// This method returns the Publishers used by this Agent.
        /// </summary>
        /// <returns>Collection of Publishers.</returns>
        public abstract IList<IBasePublisher> GetPublishers();

        /// <summary>
        /// This constructor will create a publishing Agent using the default "agent.cfg" file. If this configuration
        /// file does not exist, the Agent will not run when called.
        /// </summary>
        public PublishingAgent() : base() 
        {
        }

        /// <summary>
        /// This constructor defines the configuration file associated with this Agent.
        /// </summary>
        /// <param name="cfgFileName">Configuration file associated with this Agent. If not provided, assumes "agent.cfg".</param>
        public PublishingAgent(String cfgFileName)
            : base(cfgFileName)
        {
        }

        /// <summary>
        /// Connect to the Zones and configure the Publisher.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The Agent has not been initialised first.</exception>
        /// <exception cref="Edustructures.SifWorks.AdkException">The Agent was unable to connect to a Zone, or there is an error with the event processing for a Publisher.</exception>
        protected override void StartAgent()
        {

            // If the Agent has not been initialised, throw an exception.
            if (!Initialized)
            {
                throw new InvalidOperationException("Publishing Agent " + this.Id + " has not been initialised.");
            }

            IList<IBasePublisher> publishers;

            try
            {
                // Get the Publishers handled by this agent.
                publishers = GetPublishers();
            }
            catch (TargetException)
            {
                Shutdown();
                throw;
            }

            if (log.IsDebugEnabled) log.Debug("Starting publishing Agent " + this.Id + "...");

            IZone[] zones = ZoneFactory.GetAllZones();

            if (zones.Length == 0)
            {
                if (log.IsWarnEnabled) log.Warn("No Zones specified for publishing Agent " + this.Id + ". This publishing Agent will do nothing.");
            }
            else
            {

                try
                {

                    // Connect to each Zone specified in the Agent configuration file.
                    foreach (IZone zone in zones)
                    {

                        // For each Publisher, register (provision) it with the Zone.
                        foreach (IBasePublisher publisher in publishers)
                        {
                            zone.SetPublisher(publisher, publisher.SifObjectType, new PublishingOptions(true));
                            if (log.IsDebugEnabled) log.Debug("Registered Publisher " + publisher.GetType().FullName + " with Zone " + zone.ZoneId + ".");
                        }

                        try
                        {
                            // Connect to the Zone.
                            zone.Connect(ProvisioningFlags.Register);
                            if (log.IsDebugEnabled) log.Debug("Connected publishing Agent " + this.Id + " to Zone " + zone.ZoneId + ".");
                        }
                        catch (AdkException e)
                        {
                            if (log.IsErrorEnabled) log.Error("Publishing Agent " + this.Id + " was unable to connect to Zone " + zone.ZoneId + ".", e);
                            throw;
                        }

                    }

                    // Start event processing for each Publisher.
                    foreach (IBasePublisher publisher in publishers)
                    {

                        try
                        {
                            publisher.StartEventProcessing(zones);
                        }
                        catch (AdkException e)
                        {
                            if (log.IsErrorEnabled) log.Error("Error with event processing of Publisher " + publisher.GetType().FullName + ".", e);
                            throw;
                        }

                    }

                }
                catch (AdkException)
                {

                    // In the event of an error, disconnect from the Zones.
                    foreach (IZone zone in zones)
                    {

                        if (zone.Connected)
                        {
                            zone.Disconnect(ProvisioningFlags.Unregister);
                        }

                    }

                    throw;
                }

            }

        }

    }

}
