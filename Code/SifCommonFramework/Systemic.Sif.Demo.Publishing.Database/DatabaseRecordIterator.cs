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
using System.Configuration;
using System.Data;
using System.Data.Common;
using Edustructures.SifWorks;
using Edustructures.SifWorks.Student;
using Edustructures.SifWorks.Tools.Cfg;
using Edustructures.SifWorks.Tools.Mapping;
using Systemic.Sif.Framework.Model;
using Systemic.Sif.Framework.Publisher;

namespace Systemic.Sif.Demo.Publishing.Database
{

    /// <summary>
    /// This implementation reads from the database using the Edustructures mapping facility.
    /// </summary>
    public class DatabaseRecordIterator : ISifEventIterator<StudentPersonal>, ISifResponseIterator<StudentPersonal>
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private string databaseConnectionString;
        private string databaseDriver;
        private int eventCount = 0;
        private int eventTotal = 0;
        private Mappings mappings;
        private int responseCount = 0;
        private int responseTotal = 0;
        private List<StudentPersonal> students;

        /// <summary>
        /// Create an instance using the SIFWorks ADK configuration file.
        /// </summary>
        /// <param name="agentConfig">Configuration file for the SIF Agent.</param>
        public DatabaseRecordIterator(AgentConfig agentConfig)
        {
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
                throw new IteratorException("<mappings id=\"Default\"> has not been specified for agent " + agentConfig.SourceId + ".");
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
            return new SifEvent<StudentPersonal>(students[eventCount++], EventAction.Change);
        }

        /// <summary>
        /// Simply return the next StudentPersonal from the collection.
        /// </summary>
        /// <returns>The next response.</returns>
        public StudentPersonal GetNextResponse()
        {
            return students[responseCount++];
        }

        /// <summary>
        /// Retrieve StudentPersonal records from the database using the specified SQL query and object mappings
        /// defined in the SIFWorks ADK configuration file.
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
                        if (log.IsDebugEnabled) log.Debug(typeof(StudentPersonal).Name + " record retrieved =\n" + record.ToXml());
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
        /// Check to see if there are further events.
        /// </summary>
        /// <returns>True if there are further events; false otherwise.</returns>
        public bool HasNextEvent()
        {
            return (eventCount < eventTotal);
        }

        /// <summary>
        /// Check to see if there are further events.
        /// </summary>
        /// <returns>True if there are further responses; false otherwise.</returns>
        public bool HasNextResponse()
        {
            return (responseCount < responseTotal);
        }

    }

}
