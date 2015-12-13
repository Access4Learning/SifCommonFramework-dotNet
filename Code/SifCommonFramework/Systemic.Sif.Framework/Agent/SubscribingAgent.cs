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
using System.Collections.Generic;
using System.Reflection;
using OpenADK.Library;
using Systemic.Sif.Framework.Subscriber;

namespace Systemic.Sif.Framework.Agent
{

    /// <summary>
    /// A subscribing Agent that runs based upon settings from a pre-defined SIFWorks ADK configuration file.
    /// </summary>
    public abstract class SubscribingAgent : BaseAgent, ISubscribingAgent
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// This method returns the Subscribers used by this Agent.
        /// </summary>
        /// <returns>Collection of Subscribers.</returns>
        public virtual IList<IBaseSubscriber> GetSubscribers()
        {
            return GetInstances<IBaseSubscriber>(AgentType.Subscriber);
        }

        /// <summary>
        /// This constructor will create a subscribing Agent using the default "agent.cfg" file. If this configuration
        /// file does not exist, the Agent will not run when called.
        /// </summary>
        public SubscribingAgent()
            : base()
        {
        }

        /// <summary>
        /// This constructor defines the configuration file associated with this Agent.
        /// </summary>
        /// <param name="cfgFileName">Configuration file associated with this Agent. If not provided, assumes "agent.cfg".</param>
        public SubscribingAgent(String cfgFileName)
            : base(cfgFileName)
        {
        }

        /// <summary>
        /// This method will run the SIF Agent and Unsubscribe on shut down. Only the first call to this method will be
        /// recognised; subsequent calls will be ignored.
        /// </summary>
        public override void Run()
        {
            Run(ProvisioningFlags.Unsubscribe);
        }

        /// <summary>
        /// Connect to the Zones and configure the Subscribers.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The Agent has not been initialised first.</exception>
        /// <exception cref="Edustructures.SifWorks.AdkException">The Agent was unable to connect to a Zone, or there is an error with the event processing for a Subscriber.</exception>
        protected override void StartAgent()
        {

            // If the Agent has not been initialised, throw an exception.
            if (!Initialized)
            {
                throw new InvalidOperationException("Subscribing Agent " + this.Id + " has not been initialised.");
            }

            IList<IBaseSubscriber> subscribers;

            try
            {
                // Get the Subscribers handled by this Agent.
                subscribers = GetSubscribers();
            }
            catch (TargetException)
            {
                Shutdown();
                throw;
            }

            if (log.IsDebugEnabled) log.Debug("Starting subscribing Agent " + this.Id + "...");

            IZone[] zones = ZoneFactory.GetAllZones();

            if (zones.Length == 0)
            {
                if (log.IsWarnEnabled) log.Warn("No Zones specified for subscribing Agent " + this.Id + ". This subscribing Agent will do nothing.");
            }
            else
            {

                try
                {

                    // Connect to each Zone specified in the Agent configuration file.
                    foreach (IZone zone in zones)
                    {

                        // For each Subscriber, register (provision) it with the Zone.
                        foreach (IBaseSubscriber subscriber in subscribers)
                        {
                            zone.SetSubscriber(subscriber, subscriber.SifObjectType, new SubscriptionOptions());
                            zone.SetQueryResults(subscriber, subscriber.SifObjectType, new QueryResultsOptions());
                            if (log.IsDebugEnabled) log.Debug("Registered Subscriber " + subscriber.GetType().FullName + " with Zone " + zone.ZoneId + ".");
                        }

                        try
                        {
                            // Connect to the Zone.
                            zone.Connect(ProvisioningFlags.Register);
                            if (log.IsDebugEnabled) log.Debug("Connected subscribing Agent " + this.Id + " to Zone " + zone.ZoneId + ".");
                        }
                        catch (AdkException e)
                        {
                            if (log.IsErrorEnabled) log.Error("Subscribing Agent " + this.Id + " was unable to connect to Zone " + zone.ZoneId + ".", e);
                            throw;
                        }

                    }

                    // Start request processing for each Subscriber.
                    foreach (IBaseSubscriber subscriber in subscribers)
                    {

                        try
                        {
                            subscriber.StartRequestProcessing(zones);
                        }
                        catch (AdkException e)
                        {
                            if (log.IsErrorEnabled) log.Error("Error with request processing of Subscriber " + subscriber.GetType().FullName + ".", e);
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
