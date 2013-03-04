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
using System.Configuration;
using System.Data;
using System.Data.Common;
using OpenADK.Library;
using OpenADK.Library.au.Student;
using OpenADK.Library.Tools.Cfg;
using OpenADK.Library.Tools.Mapping;
using Systemic.Sif.Framework.Model;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.Database
{

    /// <summary>
    /// This implementation reads from the database using the OpenADK's mapping facility.
    /// </summary>
    public class StudentPersonalIterator : ISifEventIterator<StudentPersonal>, ISifResponseIterator<StudentPersonal>
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static int eventMessageCount = 0;

        private bool onceOnly = true;
        private int eventTotal = 0;
        private int responseMessageCount = 0;
        private int responseTotal = 0;
        private List<StudentPersonal> students;
        private Mappings mappings;
        private string databaseConnectionString;
        private string databaseDriver;

        /// <summary>
        /// Retrieve StudentPersonal records from the database using the specified SQL query and object mappings
        /// defined in the Agent configuration file.
        /// </summary>
        /// <param name="query">SQL query to run.</param>
        /// <returns>StudentPersonal records.</returns>
        /// <exception cref="NtAgent.AgentDbException">Error occurred trying to retrieve StudentPersonal records.</exception>
        /// <exception cref="System.ArgumentException">query parameter is null or empty.</exception>
        private List<StudentPersonal> GetRecords(String query)
        {

            if (String.IsNullOrEmpty(query))
            {
                throw new ArgumentException("query parameter is null or empty.");
            }

            List<StudentPersonal> records = new List<StudentPersonal>();

            try
            {
                DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(databaseDriver);

                using (IDbConnection conn = dbProviderFactory.CreateConnection())
                {
                    conn.ConnectionString = databaseConnectionString;
                    IDbCommand cmd = conn.CreateCommand();
                    cmd.CommandText = query;
                    conn.Open();
                    IDataReader reader = cmd.ExecuteReader();
                    DataReaderAdaptor dataReaderAdaptor = new DataReaderAdaptor(reader);

                    while (reader.Read())
                    {
                        StudentPersonal record = new StudentPersonal();
                        mappings.MapOutbound(dataReaderAdaptor, record);
                        records.Add(record);
                    }

                }

            }
            catch (Exception e)
            {
                throw new AgentDbException("Unable to retrieve " + typeof(StudentPersonal).Name + " records via database connection " + databaseConnectionString + " due to the following error: " + e.Message, e);
            }

            return records;
        }

        /// <summary>
        /// Create an instance using the Agent configuration file.
        /// </summary>
        /// <param name="agentConfig">Configuration file for the SIF Agent.</param>
        /// <exception cref="System.ArgumentException">agentConfig parameter is null.</exception>
        /// <exception cref="Systemic.Sif.Demo.Publishing.Database.AgentDbException">Database details missing from application configuration file.</exception>
        /// <exception cref="Systemic.Sif.Framework.Publisher.IteratorException">Error parsing mapping section of the Agent configuration file.</exception>
        public StudentPersonalIterator(AgentConfig agentConfig)
        {

            if (agentConfig == null)
            {
                throw new ArgumentException("agentConfig parameter is null.");
            }

            // Read the database connection details from the application configuration file.
            databaseDriver = ConfigurationManager.ConnectionStrings["database.demo"].ProviderName.Trim();
            databaseConnectionString = ConfigurationManager.ConnectionStrings["database.demo"].ConnectionString.Trim();

            if (String.IsNullOrEmpty(databaseDriver))
            {
                throw new AgentDbException("No database driver specified.");
            }

            if (String.IsNullOrEmpty(databaseConnectionString))
            {
                throw new AgentDbException("No database connection string provided");
            }

            //if (log.IsDebugEnabled) log.Debug("Database driver specified: " + databaseDriver + ".");
            //if (log.IsDebugEnabled) log.Debug("Database connection string specified: " + databaseConnectionString + ".");

            mappings = agentConfig.Mappings.GetMappings("Default");

            if (mappings == null)
            {
                throw new IteratorException("<mappings id=\"Default\"> has not been specified for Agent " + agentConfig.SourceId + ".");
            }

            students = GetRecords("select * from student where local_id = '1233493989'");
            eventTotal = students.Count;
            responseTotal = students.Count;
        }

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
        /// Simply return the next StudentPersonal from the collection.
        /// </summary>
        /// <returns>The next SIF Event.</returns>
        public SifEvent<StudentPersonal> GetNextEvent()
        {
            if (log.IsDebugEnabled) log.Debug("Next StudentPersonal event record:\n" + students[eventMessageCount].ToXml());
            return new SifEvent<StudentPersonal>(students[eventMessageCount++], EventAction.Change);
        }

        /// <summary>
        /// Simply return the next StudentPersonal from the collection.
        /// </summary>
        /// <returns>The next response.</returns>
        public StudentPersonal GetNextResponse()
        {
            if (log.IsDebugEnabled) log.Debug("Next StudentPersonal response record:\n" + students[responseMessageCount].ToXml());
            return students[responseMessageCount++];
        }

        /// <summary>
        /// If the onceOnly flag is True, then return True for each message defined in the message list.
        /// If the onceOnly flag is False, then always return True.
        /// </summary>
        /// <returns>True if there are further events; false otherwise.</returns>
        public bool HasNextEvent()
        {
            bool hasNext = (eventMessageCount < eventTotal);

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
            return responseMessageCount < responseTotal;
        }

    }

}
