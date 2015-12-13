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
using System.Timers;
using OpenADK.Library;
using OpenADK.Library.Tools.Cfg;
using Systemic.Sif.Framework.Model;

namespace Systemic.Sif.Framework.Publisher
{

    /// <summary>
    /// Publisher of SIF Data Objects.
    /// </summary>
    public abstract class GenericPublisher<T> : IBasePublisher where T : SifDataObject, new()
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private AgentConfig agentConfig;
        
        protected AgentProperties agentProperties = new AgentProperties(null);

        /// <summary>
        /// The configuration information associated with the Agent for this Publisher.
        /// </summary>
        public AgentConfig AgentConfiguration
        {
            get { return agentConfig; }

            set
            {
                agentConfig = value;
                agentConfig.GetAgentProperties(agentProperties);
            }

        }

        /// <summary>
        /// The SIF Data Object type associated with this Publisher.
        /// </summary>
        public IElementDef SifObjectType
        {
            get { return (new T()).ObjectType; }
        }

        /// <summary>
        /// This method is used to set the frequency (in milliseconds) that SIF Events will be broadcast. If this
        /// value is 0 (or less), no SIF Events will ever be broadcast.
        /// Default implementation returns 1 hour if no event frequency is defined.
        /// </summary>
        public virtual int EventFrequency
        {
            get { return agentProperties.GetProperty("publisher." + SifObjectType.Name + ".eventFrequency", 3600000); }
            set { }
        }

        /// <summary>
        /// Create an instance of the Publisher without referencing the Agent configuration settings.
        /// </summary>
        public GenericPublisher()
        {
        }

        /// <summary>
        /// Create an instance of the Publisher based upon the Agent configuration settings.
        /// </summary>
        /// <param name="agentConfig">Agent configuration settings.</param>
        /// <exception cref="System.ArgumentException">agentConfig parameter is null.</exception>
        public GenericPublisher(AgentConfig agentConfig)
        {

            if (agentConfig == null)
            {
                throw new ArgumentException("agentConfig parameter is null.");
            }

            AgentConfiguration = agentConfig;
            AgentConfiguration.GetAgentProperties(agentProperties);
        }

        /// <summary>
        /// This method is used to provide an iterator for the SIF Events that need to be broadcast.
        /// </summary>
        /// <returns>An iterator for SIF Events to be broadcast.</returns>
        /// <exception cref="Systemic.Sif.Framework.Publisher.IteratorException">All errors should be wrapped by this exception.</exception>
        public abstract ISifEventIterator<T> GetSifEvents();

        /// <summary>
        /// This method is used to provide an iterator for the SIF Data Objects that need to be sent as a result of a
        /// SIF request from a Subscriber.
        /// </summary>
        /// <param name="query">The query conditions for the SIF Request.</param>
        /// <param name="zone">The zone the SIF Request was received on.</param>
        /// <returns>An iterator for SIF Data Objects to be sent in response to a request.</returns>
        /// <exception cref="Systemic.Sif.Framework.Publisher.IteratorException">All errors should be wrapped by this exception.</exception>
        public abstract ISifResponseIterator<T> GetSifResponses(Query query, IZone zone);

        /// <summary>
        /// This method will publish a SIF Event to the Zone.
        /// </summary>
        /// <param name="sifEvent">SIF Event to publish.</param>
        /// <param name="zone">Zone to broadcast SIF Event to.</param>
        /// <exception cref="Systemic.Sif.Framework.Publisher.PublisherException">An error occurred publishing the SIF Event to the Zone.</exception>
        protected virtual void BroadcastEvent(SifEvent<T> sifEvent, IZone zone)
        {

            try
            {
                zone.ReportEvent(sifEvent.SifDataObject, sifEvent.EventAction);
            }
            catch (Exception e)
            {
                string message = "The " + sifEvent.EventAction.ToString() + " event for object ...\n" + sifEvent.SifDataObject.ToXml() + "\nwas not sent to Zone " + zone.ZoneId + " because it returned the following error ...\n" + e.Message;
                if (log.IsErrorEnabled) log.Error(message, e); ;
                throw new PublisherException(message, e);
            }

        }

        /// <summary>
        /// This method will publish a SIF Event to each of the Zones.
        /// </summary>
        /// <param name="zones">Zones to broadcast SIF Events to.</param>
        private void BroadcastEvents(IZone[] zones)
        {

            foreach (IZone zone in zones)
            {

                try
                {
                    ISifEventIterator<T> iterator = GetSifEvents();

                    if (iterator == null)
                    {
                        if (log.IsWarnEnabled) log.Warn("Iterator of SIF Events for Publisher " + this.GetType().Name + " returned null.");
                    }
                    else
                    {
                        int successCount = 0;
                        int failureCount = 0;

                        while (iterator.HasNextEvent())
                        {
                            SifEvent<T> sifEvent = null;

                            try
                            {
                                iterator.BeforeEvent();
                                sifEvent = iterator.GetNextEvent();

                                if (sifEvent == null)
                                {
                                    if (log.IsWarnEnabled) log.Warn("iterator.hasNextEvent() has returned True, but iterator.getNextEvent() has returned null. The SIF Event has been ignored.");
                                    failureCount++;
                                }
                                else
                                {

                                    try
                                    {
                                        BroadcastEvent(sifEvent, zone);
                                        successCount++;
                                    }
                                    catch (PublisherException)
                                    {
                                        failureCount++;
                                    }

                                }

                                iterator.AfterEvent();
                            }
                            catch (Exception e)
                            {
                                if (log.IsWarnEnabled) log.Warn("SIF Event has been ignored because iterator.getNextEvent() failed.", e);
                                failureCount++;
                            }

                        }

                        if (successCount == 0 && failureCount == 0)
                        {
                            if (log.IsInfoEnabled) log.Info("No SIF Events of type " + SifObjectType.Name + " found for Zone " + zone.ZoneId + ".");
                        }
                        else
                        {
                            if (log.IsInfoEnabled) log.Info(this.GetType().Name + " successfully sent " + successCount + " SIF Events for " + this.SifObjectType.Name + " to zone " + zone.ZoneId + ".");
                            if (log.IsInfoEnabled && failureCount > 0) log.Info(failureCount + " SIF Events failed to be sent for " + this.SifObjectType.Name + " to zone " + zone.ZoneId + ".");
                        }

                    }

                }
                catch (IteratorException e)
                {
                    if (log.IsErrorEnabled) log.Error("Error with Iterator of SIF Events: " + e.Message, e); ;
                }

            }

        }

        /// <summary>
        /// Handler for SIF Request messages from the zone.
        /// </summary>
        /// <param name="outStream">The output stream to send SIF Data Object results to.</param>
        /// <param name="query">The query conditions for the SIF Request.</param>
        /// <param name="zone">The zone this SIF Request was received on.</param>
        /// <param name="info">Provides protocol-specific information about the message.</param>
        void IPublisher.OnRequest(IDataObjectOutputStream outStream, Query query, IZone zone, IMessageInfo info)
        {
            if (log.IsInfoEnabled) log.Info(this.GetType().Name + " received SIF Request for " + query.ObjectTag + " from agent " + ((SifMessageInfo)info).SourceId + " in zone " + zone.ZoneId + ".");

            // Use the autoFilter() capability of DataObjectOutputStream so that any object can be written to the
            // output stream and the stream will filter out any objects that don't meet the conditions of the Query.
            // This filter is applied in conjunction with any filtering that may have occurred with a call to
            // GetSifResponses().
            outStream.Filter = query;

            try
            {
                ISifResponseIterator<T> iterator = GetSifResponses(query, zone);

                if (iterator == null)
                {
                    if (log.IsWarnEnabled) log.Warn("getSifResponses() for Publisher " + this.GetType().Name + " returned null.");
                }
                else
                {
                    int successCount = 0;
                    int failureCount = 0;

                    while (iterator.HasNextResponse())
                    {
                        T sifDataObject = null;

                        try
                        {
                            sifDataObject = iterator.GetNextResponse();

                            if (sifDataObject == null)
                            {
                                if (log.IsWarnEnabled) log.Warn("iterator.GetNextResponse() has returned True, but iterator.GetNextResponse() has returned null. The SIF Data Object has been ignored.");
                                failureCount++;
                            }
                            else
                            {

                                try
                                {
                                    // Write the SIF Data Object to the zone via the output stream.
                                    outStream.Write(sifDataObject);
                                    successCount++;
                                }
                                catch (Exception e)
                                {
                                    failureCount++;
                                    if (log.IsErrorEnabled) log.Error("The requested SIF Data Object ...\n" + sifDataObject.ToXml() + "\nwas not sent because it returned the following error ...\n" + e.Message, e); ;
                                }

                            }

                        }
                        catch (Exception e)
                        {
                            if (log.IsWarnEnabled) log.Warn("SIF Response has been ignored because iterator.getNextResponse() failed.", e);
                            failureCount++;
                        }

                    }

                    if (log.IsInfoEnabled) log.Info(this.GetType().Name + " successfully responded to a SIF Request for " + this.SifObjectType.Name + " to zone " + zone.ZoneId + " with " + successCount + " SIF Data Objects.");
                    if (log.IsInfoEnabled && failureCount > 0) log.Info(failureCount + " SIF Data Objects failed to be sent for " + this.SifObjectType.Name + " to zone " + zone.ZoneId + ".");
                }

            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Error responding to SIF Request: " + e.Message, e);
            }

        }

        /// <summary>
        /// This method starts up a thread that periodically checks for SIF Events to publish to the specified Zones.
        /// </summary>
        /// <param name="zones">Zones to publish to.</param>
        void IBasePublisher.StartEventProcessing(IZone[] zones)
        {

            // Create a timer with an appropriate interval.
            if (EventFrequency > 0)
            {
                if (log.IsDebugEnabled) log.Debug(this.GetType().Name + " starting event processing (interval is " + EventFrequency + " milliseconds)...");
                Timer timer = new Timer(EventFrequency);
                timer.Elapsed += delegate { BroadcastEvents(zones); };
                timer.Start();
            }

        }

    }

}
