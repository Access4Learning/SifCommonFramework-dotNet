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
using Edustructures.SifWorks;
using Edustructures.SifWorks.Tools.Mapping;
using Edustructures.Util;
using Systemic.Sif.Framework.Agent;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.XmlFile
{

    /// <summary>
    /// This SIF Agent provides an example implementation of a publishing Agent from the SIF Common Framework.
    /// </summary>
    class DemoPublishingAgent : PublishingAgent
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Names of the explicitly defined publishers handled by this agent. If no publishers specified, an empty
        /// array is returned.
        /// </summary>
        private string[] PublisherNames
        {

            get
            {
                string[] publisherNames = new string[0];
                string publisherList = Properties.GetProperty("publisher.explicit.list", null);

                if (publisherList != null)
                {
                    publisherNames = publisherList.Split(new char[] { ',' });
                }

                return publisherNames;
            }

        }

        /// <summary>
        /// Simply call the default constructor of the super class.
        /// </summary>
        public DemoPublishingAgent()
            : base() 
        {
        }

        /// <summary>
        /// Define the configuration file used for creating this instance.
        /// </summary>
        /// <param name="cfgFileName">Configuration file associated with this SIF Agent.</param>
        public DemoPublishingAgent(String cfgFileName)
            : base(cfgFileName)
        {
        }

        /// <summary>
        /// The Publishers handled by this Agent. Publishers for an Agent can be specified either explicitly (using
        /// the "publisher.explicit.list" property) or implicitly through the <object/> mappings section of the Agent
        /// configuration file.
        /// If no Publishers specified, an empty list is returned.
        /// </summary>
        /// <param name="agentConfig">Configuration information for this Agent.</param>
        /// <returns>Publishers handled by this Agent.</returns>
        /// <exception cref="System.Reflection.TargetException">Unable to create an instance of a Publisher.</exception>
        public override IList<IBasePublisher> GetPublishers()
        {
            IList<IBasePublisher> publishers = new List<IBasePublisher>();
            IList<IElementDef> elementDefinitions = new List<IElementDef>();

            // Instantiate each Publisher handled by this Agent.
            foreach (string publisherName in PublisherNames)
            {

                try
                {
                    // Create an instance of the Publisher.
                    if (log.IsDebugEnabled) log.Debug("Publisher name is " + publisherName);
                    Type type = Type.GetType(publisherName);
                    IBasePublisher publisher = (IBasePublisher)Activator.CreateInstance(type);
                    publisher.AgentConfiguration = AgentConfiguration;
                    publishers.Add(publisher);
                    elementDefinitions.Add(publisher.SifObjectType);
                }
                catch (Exception e)
                {
                    throw new TargetException("Unable to create an instance of Publisher " + publisherName + ".", e);
                }

            }

            Mappings mappings = AgentConfiguration.Mappings.GetMappings("Default");

            if (mappings == null)
            {
                if (log.IsInfoEnabled) log.Info("<mappings id=\"Default\"> has not been specified for Agent " + this.Id + ".");
            }
            else
            {

                foreach (ObjectMapping objectMapping in mappings.GetObjectMappings(false))
                {
                    bool enabled = Properties.GetProperty("publisher.implicit." + objectMapping.ObjectType + ".enable", false);
                    if (log.IsInfoEnabled) log.Info("publisher.implicit." + objectMapping.ObjectType + ".enable is " + enabled);

                    if (enabled)
                    {
                        IElementDef elementDef = Adk.Dtd.LookupElementDef(objectMapping.ObjectType);

                        if (!elementDefinitions.Contains(elementDef))
                        {

                            try
                            {
                                SifDataObject sifDataObject = Adk.Dtd.CreateSIFDataObject(elementDef);
                                // Create an instance of the publisher.
                                Type genericPublisherType = typeof(GenericPublisher<>);
                                Type[] typeArgs = { sifDataObject.GetType() };
                                Type constructed = genericPublisherType.MakeGenericType(typeArgs);
                                IBasePublisher publisher = (IBasePublisher)Activator.CreateInstance(constructed);
                                publisher.AgentConfiguration = AgentConfiguration;
                                publishers.Add(publisher);
                            }
                            catch (Exception e)
                            {
                                throw new TargetException("Unable to create an instance of a generic Publisher for " + elementDef.GetType().FullName + ".", e);
                            }

                        }

                    }

                }

            }

            if (publishers.Count == 0)
            {
                if (log.IsInfoEnabled) log.Info("No Publishers have been specified for Agent " + this.Id + ".");
            }

            return publishers;
        }

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            DemoPublishingAgent agent = null;

            // Check if the appropriate number of parameters has been passed.
            if (args.Length == 0)
            {
                agent = new DemoPublishingAgent("PublishingAgent.cfg");
            }
            else if (args.Length == 1)
            {
                agent = new DemoPublishingAgent(args[0]);
            }
            else
            {
                Console.WriteLine("Usage is: " + System.Reflection.MethodBase.GetCurrentMethod().DeclaringType + " <agent_config_file>");
                Environment.Exit(1);
            }

            try
            {
                agent.Run();
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("Unable to run the SIF Agent", e);
            }

        }

    }

}
