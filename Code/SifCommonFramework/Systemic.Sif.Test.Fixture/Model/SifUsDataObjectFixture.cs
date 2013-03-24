/*
* Copyright 2013 Systemic Pty Ltd
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
using NUnit.Framework;
using OpenADK.Library;
using OpenADK.Library.us.Common;
using OpenADK.Library.us.Energymanagement;

namespace Systemic.Sif.Test.Fixture.Model
{

    [TestFixture]
    class SifUsDataObjectFixture
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Adk.Initialize(SIFVariant.SIF_US);
        }

        [Test]
        public void CreateEnergyUsage()
        {
            BinaryData binaryData = new BinaryData() { Value = "urn:sif:school:AcmeMiddleSchool1.CoyoteDistrict.Arizona" };
            UsageLocationInfo usageLocationInfo = new UsageLocationInfo() { SchoolId = binaryData };

            ReadingData readingData = new ReadingData() { EnergyUnits = "$Kilowatt-Hour" };
            ReadingDataList readingDataList = new ReadingDataList() { readingData };
            DataSource dataSource = new DataSource() { ReadingDataList = readingDataList };

            EnergyUsage energyUsage = new EnergyUsage() { UsageLocationInfo = usageLocationInfo, DataSource = dataSource };

            if (log.IsDebugEnabled) log.Debug("EnergyUsage instance: " + energyUsage.ToXml());
            Console.WriteLine("EnergyUsage instance: " + energyUsage.ToXml());
        }

    }

}
