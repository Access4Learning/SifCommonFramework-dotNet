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

namespace Systemic.Sif.Demo.Publishing.XmlFile
{

    /// <summary>
    /// This SIF Agent provides an example implementation of a publishing Agent from the SIF Common Framework.
    /// </summary>
    class PublishingAgent : Systemic.Sif.Framework.Agent.PublishingAgent
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Simply call the default constructor of the super class.
        /// </summary>
        public PublishingAgent()
            : base()
        {
        }

        /// <summary>
        /// Define the configuration file used for creating this instance.
        /// </summary>
        /// <param name="cfgFileName">Configuration file associated with this SIF Agent.</param>
        public PublishingAgent(String cfgFileName)
            : base(cfgFileName)
        {
        }

        /// <summary>
        /// Application entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        static void Main(string[] args)
        {
            PublishingAgent agent = null;

            // Check if the appropriate number of parameters has been passed.
            if (args.Length == 0)
            {
                agent = new PublishingAgent("PublishingAgent.cfg");
            }
            else if (args.Length == 1)
            {
                agent = new PublishingAgent(args[0]);
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
