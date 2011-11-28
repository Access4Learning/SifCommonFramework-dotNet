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

using System.Timers;
using Edustructures.SifWorks;
using Edustructures.SifWorks.Infra;
using Edustructures.SifWorks.Tools.Cfg;
using Systemic.Sif.Framework.Model;

namespace Systemic.Sif.Framework.Subscriber
{

    /// <summary>
    /// Subscriber of SIF Data Objects.
    /// </summary>
    public abstract class GenericSubscriber<T> : IBaseSubscriber where T : SifDataObject, new()
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private AgentConfig agentConfig;
        private string applicationId;

        /// <summary>
        /// The configuration information associated with the Agent for this Subscriber.
        /// </summary>
        public AgentConfig AgentConfiguration
        {
            get { return agentConfig; }
            set { agentConfig = value; }
        }

        /// <summary>
        /// Unique identfier for the application associated with the Agent for this Subscriber.
        /// </summary>
        public string ApplicationId
        {
            get { return applicationId; }
            set { applicationId = value; }
        }

        /// <summary>
        /// The SIF Data Object type associated with this Subscriber.
        /// </summary>
        public IElementDef SifObjectType
        {
            get { return (new T()).ObjectType; }
        }

        public GenericSubscriber()
        {
        }

        public GenericSubscriber(AgentConfig agentConfig)
        {
            this.agentConfig = agentConfig;
        }

        /// <summary>
        /// This method is used to determine whether a request for SIF data will be made for a zone.
        /// </summary>
        /// <param name="zone">The Zone the request will be made to.</param>
        /// <returns>True if a request is to be made; false otherwise.</returns>
        protected abstract bool MakeRequest(IZone zone);

        /// <summary>
        /// This method is used to process a SIF Event.
        /// </summary>
        /// <param name="sifEvent">SIF Event received.</param>
        /// <param name="zone">Zone that the SIF Event came from.</param>
        protected abstract void ProcessEvent(SifEvent<T> sifEvent, IZone zone);

        /// <summary>
        /// This method is used to process the SIF Data Object associated with a response from a request.
        /// </summary>
        /// <param name="sifDataObject">SIF Data Object associated with the response.</param>
        /// <param name="zone">Zone that the response came from.</param>
        protected abstract void ProcessResponse(T sifDataObject, IZone zone);

        /// <summary>
        /// This method is used to set the frequency (in milliseconds) that a call to MakeRequest(IZone) will be made.
        /// If this value is 0 (or less), no requests for SIF data will ever be made.
        /// </summary>
        protected abstract int RequestFrequency { get; set; }

        /// <summary>
        /// This method can be overwritten to specify further query conditions for a SIF Request. The Query
        /// parameter will already have been instantiated with the correct SIF Data Object type. An example Query
        /// modification is:
        /// <code>query.addCondition( LearnerDTD.LEARNERPERSONAL_UPN, ComparisonOperators.LE, "M830540004340" );</code>
        /// </summary>
        /// <param name="query">SIF Query to modify.</param>
        /// <param name="zone">Zone that the request will be made to.</param>
        protected virtual void AddToBroadcastRequestQuery(Query query, IZone zone)
        {
        }

        /// <summary>
        /// This method can be overwritten to specify further query conditions for synchronisation. The Query
        /// parameter will already have been instantiated with the correct SIF Data Object type. An example Query
        /// modification is:
        /// <code>query.addCondition( LearnerDTD.LEARNERPERSONAL_UPN, ComparisonOperators.LE, "M830540004340" );</code>
        /// </summary>
        /// <param name="query">SIF Query to modify.</param>
        /// <param name="zone">Zone that the request will be made to.</param>
        protected virtual void AddToSyncQuery(Query query, IZone zone)
        {
        }

        /// <summary>
        /// This method will send a SIF Request to the Zone. Before the SIF Request is made, the MakeRequest(IZone)
        /// method is checked. Unless the AddToBroadcastRequestQuery(Query) method is overwritten, the SIF Request
        /// will be made against all SIF Data Objects of the appropriate type.
        /// </summary>
        /// <param name="zone">Zone the SIF Request will be made to.</param>
        protected virtual void BroadcastRequest(IZone zone)
        {

            // Check if a SIF Request needs to be made at this point in time.
            if (MakeRequest(zone))
            {
                if (log.IsDebugEnabled) log.Debug("Making a request for " + SifObjectType.Name + " in zone " + zone.ZoneId);
                // Create a Query for the SIF Data Object type.
                Query query = new Query(SifObjectType);
                // Without this, an error occurs.
                query.SifVersions = new SifVersion[] { AgentConfiguration.Version };
                // Add any query conditions you may have.
                AddToBroadcastRequestQuery(query, zone);
                zone.Query(query);
            }

        }

        /// <summary>
        /// This method will send a SIF Request to all the specified Zones.
        /// </summary>
        /// <param name="zones">Zones the SIF Request will be made to.</param>
        private void BroadcastRequests(IZone[] zones)
        {

            foreach (IZone zone in zones)
            {
                BroadcastRequest(zone);
            }

        }

        /// <summary>
        /// This method is called when a SIF Event is received by the Subscriber.
        /// </summary>
        /// <param name="evnt">SIF Event received.</param>
        /// <param name="zone">Zone that SIF Event was received.</param>
        /// <param name="info">Information on the received message.</param>
        void ISubscriber.OnEvent(Event evnt, IZone zone, IMessageInfo info)
        {

            // Although not generally the case, it is possible that more than 1 SIF Data Object is received with the
            // SIF Event.
            while (evnt.Data.Available)
            {
                T sifDataObject = (T)evnt.Data.ReadDataObject();
                EventAction eventAction = evnt.Action;
                SifEvent<T> sifEvent = new SifEvent<T>(sifDataObject, eventAction);

                if (PreProcessEvent(sifEvent, zone))
                {
                    ProcessEvent(sifEvent, zone);
                }

            }

        }

        /// <summary>
        /// This method from the SIFWorks ADK has no specific implemention within this framework, but may be
        /// overridden as required.
        /// </summary>
        /// <see cref="Edustructures.SifWorks.IQueryResults#OnQueryPending(Edustructures.SifWorks.IMessageInfo, Edustructures.SifWorks.IZone)">OnQueryPending</param>
        public virtual void OnQueryPending(IMessageInfo info, IZone zone)
        {
            if (log.IsInfoEnabled) log.Info("OnQueryPending() is not currently implemented for Subscriber " + this.GetType().Name + ".");
        }

        /// <summary>
        /// This method is called when a response is received to a request made by the Subscriber.
        /// </summary>
        /// <param name="data">The data stream containing the SIF Response.</param>
        /// <param name="error">Error details (if encountered).</param>
        /// <param name="zone">Zone that SIF Response was received.</param>
        /// <param name="info">Information on the received message.</param>
        void IQueryResults.OnQueryResults(IDataObjectInputStream data, SIF_Error error, IZone zone, IMessageInfo info)
        {
            int count = 0;

            // Before reading data from a SIF Response, first check to see if an error was returned.
            if (error != null)
            {
                // The provider returned an error message for this SIF Request.
                if (log.IsErrorEnabled) log.Error("An error was received from the provider of " + SifObjectType.Name + ".");
                if (log.IsErrorEnabled) log.Error(error.SIF_Desc + "\r\n" + error.SIF_ExtendedDesc);
                return;
            }

            // Now, read each object from the DataObjectInputStream until Available returns false.
            while (data.Available)
            {
                count++;
                T sifDataObject = (T)data.ReadDataObject();

                if (PreProcessResponse(sifDataObject, zone))
                {
                    ProcessResponse(sifDataObject, zone);
                }

            }

            // Print out the total number of objects recieved.
            if (log.IsInfoEnabled) log.Info(count + " total objects received.");

            // To determine if you have completed receiving all responses, check the
            // MorePackets property of the SIFMessageInfo object
            if (((SifMessageInfo)info).MorePackets)
            {
                if (log.IsInfoEnabled) log.Info("Waiting for more packets...");
            }
            else
            {
                if (log.IsInfoEnabled) log.Info("All requested packets have been received");
            }

        }

        /// <summary>
        /// This method is used to perform any custom preprocessing of a received SIF Event. If this method determines
        /// that the SIF Event should not be processed further, false should be returned. Returning false would mean
        /// that the SIF Event is discarded.
        /// </summary>
        /// <param name="sifEvent">SIF Event received.</param>
        /// <param name="zone">Zone that the SIF Event came from.</param>
        /// <returns>True is the SIF Event should be processed further; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">A parameter is null.</exception>
        protected virtual bool PreProcessEvent(SifEvent<T> sifEvent, IZone zone)
        {
            return true;
        }

        /// <summary>
        /// This method is used to perform any custom preprocessing of a received SIF Data Object associated with a
        /// response from a request. If this method determines that the SIF Event should not be processed further,
        /// false should be returned. Returning false would mean that the SIF Event is discarded.
        /// </summary>
        /// <param name="sifDataObject">SIF Data Object associated with the response.</param>
        /// <param name="zone">Zone that the response came from.</param>
        /// <returns>True is the SIF Event should be processed further; false otherwise.</returns>
        /// <exception cref="System.ArgumentNullException">A parameter is null.</exception>
        protected virtual bool PreProcessResponse(T sifDataObject, IZone zone)
        {
            return true;
        }

        /// <summary>
        /// This method starts up a thread that periodically sends a SIF Request (if needed) to the specified Zones.
        /// </summary>
        /// <param name="zones">Zones the SIF Request is sent to.</param>
        public virtual void StartRequestProcessing(IZone[] zones)
        {

            // Create a timer with an appropriate interval.
            if (RequestFrequency > 0)
            {
                if (log.IsDebugEnabled) log.Debug(this.GetType().Name + " starting request processing (interval is " + RequestFrequency + " milliseconds)...");
                Timer timer = new Timer(RequestFrequency);
                timer.Elapsed += delegate { BroadcastRequests(zones); };
                timer.Start();
            }

        }

        /// <summary>
        /// Signals this class to begin the syncrhonization process. This class is responsible for querying the zone
        /// for any data it needs to synchronize itself. Unless the AddToSyncQuery(Query) method is overwritten, the
        /// synchronisation will request all SIF Data Objects of the appropriate type.
        /// </summary>
        /// <param name="zone">Zone to synchronise with.</param>
        public virtual void Sync(IZone zone)
        {
            // Create a Query for the SIF Data Object type.
            Query query = new Query(SifObjectType);
            // Without this, an error occurs.
            query.SifVersions = new SifVersion[] { AgentConfiguration.Version };
            // Add any query conditions you may have.
            AddToSyncQuery(query, zone);
            zone.Query(query);
        }

    }

}
