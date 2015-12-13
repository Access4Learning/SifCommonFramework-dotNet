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
using System.IO;
using System.Reflection;
using OpenADK.Library;
using OpenADK.Library.Tools.Cfg;
using OpenADK.Util;
using Systemic.Sif.Framework.Util;

namespace Systemic.Sif.Framework.Agent
{

    /// <summary>
    /// SIF Agent for managing Publishers and Subscribers.
    /// </summary>
    public abstract class BaseAgent : OpenADK.Library.Agent, IAgentSettings
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        // Configuration file settings used by this Agent.
        private AgentConfig agentConfig;
        // Default name of the configuration file associated with this Agent.
        private String cfgFileName = "agent.cfg";

        protected internal enum AgentType { Publisher, Subscriber };

        /// <summary>
        /// The configuration information associated with the Agent.
        /// </summary>
        public AgentConfig AgentConfiguration
        {
            get { return agentConfig; }
            set { agentConfig = value; }
        }

        /// <summary>
        /// The Agent's home directory.
        /// </summary>
        public override string HomeDir
        {
            get { return base.HomeDir + "\\" + Properties.GetProperty("agent.homeSubdir", ""); }
        }

        /// <summary>
        /// Locale for this SIF Agent, i.e. AU, UK, US.
        /// </summary>
        protected virtual SIFVariant Locale
        {
            get { return (agentConfig == null ? SIFVariant.SIF_US : agentConfig.Variant); }
        }

        /// <summary>
        /// This method is used to connect to the Zones and configure the Publishers/Subscribers.
        /// </summary>
        protected abstract void StartAgent();

        /// <summary>
        /// This constructor will simply call the super class with an appropriate Agent ID.
        /// </summary>
        public BaseAgent()
            : base("BaseAgent")
        {
            this.fName = "General SIF Agent for Publishing and Subscribing";
        }

        /// <summary>
        /// This constructor defines the configuration file associated with this Agent.
        /// </summary>
        /// <param name="cfgFileName">Configuration file associated with this Agent. If not provided, assumes "agent.cfg".</param>
        public BaseAgent(String cfgFileName)
            : this()
        {

            if (!String.IsNullOrEmpty(cfgFileName))
            {
                this.cfgFileName = cfgFileName;
                log4net.GlobalContext.Properties["instance"] = this.cfgFileName;
            }

        }

        /// <summary>
        /// This method returns the Publisher or Subscriber instances used by this Agent.
        /// </summary>
        /// <param name="agentType">Indicates whether Publisher or Subscriber instances are returned.</param>
        /// <returns>Collection of Publishers or Subscribers.</returns>
        protected internal IList<T> GetInstances<T>(AgentType agentType) where T : IAgentSettings
        {
            IList<T> instances = new List<T>();
            string instanceTypesString = Properties.GetProperty(agentType.ToString().ToLower() + ".types", "");
            IList<IElementDef> instanceTypes = PropertyUtils.ParseElementDefinitions(instanceTypesString);

            foreach (IElementDef instanceType in instanceTypes)
            {
                string instanceString = Properties.GetProperty(agentType.ToString().ToLower() + "." + instanceType.Name + ".implementation", null);

                if (!String.IsNullOrEmpty(instanceString))
                {
                    string assemblyName = PropertyUtils.ParseAssemblyName(instanceString);
                    string className = PropertyUtils.ParseClassName(instanceString);

                    try
                    {

                        if (!String.IsNullOrEmpty(className))
                        {
                            Assembly assembly = null;

                            if (String.IsNullOrEmpty(assemblyName))
                            {
                                assembly = Assembly.GetEntryAssembly();
                            }
                            else
                            {
                                assembly = Assembly.LoadFrom(assemblyName);
                            }

                            if (assembly != null)
                            {
                                // Create an instance of the Publisher or Subscriber.
                                Type type = assembly.GetType(className);
                                T instance = (T)Activator.CreateInstance(type);
                                instance.AgentConfiguration = AgentConfiguration;
                                instances.Add(instance);
                            }

                        }

                    }
                    catch (Exception e)
                    {
                        if (log.IsErrorEnabled) log.Error("Unable to create an instance of " + agentType.ToString() + " " + className + ".", e);
                    }

                }

            }

            if (instances.Count == 0)
            {
                if (log.IsInfoEnabled) log.Info("No " + agentType.ToString() + "s have been specified for Agent " + this.Id + ".");
            }

            return instances;
        }

        /// <summary>
        /// Configure the Agent based on the Agent configuration file, then initialise.
        /// </summary>
        /// <exception cref="System.IO.IOException">File or resource exception occurred, possibly while reading Agent configuration file.</exception>
        /// <exception cref="System.Reflection.TargetException">Unable to create an instance of a Publisher or Subscriber for the Agent.</exception>
        public override void Initialize()
        {

            if (Initialized)
            {
                if (log.IsInfoEnabled) log.Info("Agent " + this.Id + " has already been initialised and will not be initialised again.");
            }
            else
            {
                agentConfig = new AgentConfig();

                try
                {
                    // Read the Agent configuration file.
                    agentConfig.Read(this.cfgFileName, false);
                }
                catch (IOException e)
                {
                    throw new IOException("Error reading Agent configuration file " + this.cfgFileName + " when initialising Agent " + this.Id + ".", e);
                }

                // Initialise the ADK to use the latest SIF version and all SIF Data Object modules?
                Adk.Initialize(Locale);

                // Override the SourceId passed to the constructor with the SourceID specified in the configuration
                // file.
                Id = agentConfig.SourceId;
                // Inform the ADK of the version of SIF specified in the sifVersion= attribute of the <agent> element.
                Adk.SifVersion = agentConfig.Version;

                // Call the superclass initialise once the configuration file has been read.
                try
                {
                    base.Initialize();
                }
                catch (Exception e)
                {
                    throw new IOException("Agent " + this.Id + " is unable to initialise due to a file or resource exception.", e);
                }

                // Ask the AgentConfig instance to "apply" all configuration settings to this Agent. This includes
                // parsing and registering all <zone> elements with the Agent's ZoneFactory.
                agentConfig.Apply(this, true);

                // Set the level of debugging applied to the ADK.
                if (Properties.GetProperty("agent.debugAll", false))
                {
                    Adk.Debug = AdkDebugFlags.All;
                }
                else
                {
                    Adk.Debug = AdkDebugFlags.Minimal;
                }

                // Override the Agent display name if provided.
                String displayName = Properties.GetProperty("agent.description", null);

                if (displayName != null)
                {
                    this.fName = displayName;
                }

                if (!Directory.Exists(this.WorkDir))
                {
                    Directory.CreateDirectory(this.WorkDir);
                }

                if (log.IsInfoEnabled) log.Info("Agent " + this.Id + " has been initialised using configuration file " + this.cfgFileName + "...");
                if (log.IsInfoEnabled) log.Info("Agent Property => Display name: " + this.fName);
                if (log.IsInfoEnabled) log.Info("Agent Property => Messaging mode: " + Properties.MessagingMode);
                if (log.IsInfoEnabled) log.Info("Agent Property => Transport protocol: " + Properties.TransportProtocol);
                if (log.IsInfoEnabled) log.Info("Agent Property => Pull frequency: " + Properties.PullFrequency);
                if (log.IsInfoEnabled) log.Info("Agent Property => Maximum buffer size: " + Properties.MaxBufferSize);
                if (log.IsInfoEnabled) log.Info("Agent Property => Override SIF versions: " + Properties.OverrideSifVersions);
                if (log.IsInfoEnabled) log.Info("Agent Property => Home directory: " + this.HomeDir);
                if (log.IsInfoEnabled) log.Info("Agent Property => Work directory: " + this.WorkDir);

                StartAgent();
            }

        }

        /// <summary>
        /// This method will run the SIF Agent and Unregister on shut down. Only the first call to this method will be
        /// recognised; subsequent calls will be ignored.
        /// </summary>
        public virtual void Run()
        {
            this.Run(ProvisioningFlags.Unregister);
        }

        /// <summary>
        /// This method will run the SIF Agent and shut down based upon the provisioning flag specified.
        /// </summary>
        /// <param name="provisioningFlag">Flag to indicate how to shut down.</param>
        protected void Run(ProvisioningFlags provisioningFlag)
        {

            try
            {
                Initialize();
                Console.WriteLine(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType + " is running (press Ctrl-C to stop)...");
                AdkConsoleWait adkConsoleWait = new AdkConsoleWait();
                adkConsoleWait.Exiting += delegate { Shutdown(provisioningFlag); };
                adkConsoleWait.WaitForExit();
            }
            catch (AdkConfigException e)
            {
                if (log.IsErrorEnabled) log.Error("Unable to find the associated configuration file for this SIF Agent.", e);
                throw;
            }
            catch (AdkException e)
            {
                if (log.IsErrorEnabled) log.Error("ADK could not be initialised or error running the SIF Agent.", e);
                throw;
            }
            catch (IOException e)
            {
                if (log.IsErrorEnabled) log.Error("IO error occurred.", e);
                throw;
            }
            catch (Exception e)
            {
                if (log.IsErrorEnabled) log.Error("SIF Agent failed with an unexpected error.", e);
                throw;
            }
            finally
            {
                Shutdown(provisioningFlag);
            }

        }

        /// <summary>
        /// Shutdown the Agent.
        /// </summary>
        /// <param name="provisioningOptions">Flag to indicate what provisioning message is sent to Zones on shut-down.</param>
        public override void Shutdown(ProvisioningFlags provisioningOptions)
        {

            if (this.Initialized && !this.IsShutdown)
            {

                // Always shutdown the Agent on exit.
                // NOTE: There is no reference to an exception in the documentation.
                try
                {
                    base.Shutdown(provisioningOptions);
                    if (log.IsDebugEnabled) log.Debug("Successfully shutdown Agent " + this.Id + ".");
                }
                catch (AdkException e)
                {
                    if (log.IsErrorEnabled) log.Error("Agent " + this.Id + " failed trying to shutdown.", e);
                }

            }

        }

    }

}

