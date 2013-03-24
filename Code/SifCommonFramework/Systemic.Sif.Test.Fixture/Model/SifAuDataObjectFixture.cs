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
using OpenADK.Library.au.Sif3assessment;

namespace Systemic.Sif.Test.Fixture.Model
{

    [TestFixture]
    class SifAuDataObjectFixture
    {
        // Create a logger for use in this class.
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            Adk.Initialize(SIFVariant.SIF_AU);
        }

        [Test]
        public void CreateSif3StudentResponseSet()
        {
            TraitScore traitScore1 = new TraitScore() { TraitScoreType = "type1", TraitScoreCode = "code1", TraitScoreValue = "value1" };
            TraitScore traitScore2 = new TraitScore() { TraitScoreType = "type2", TraitScoreCode = "code2", TraitScoreValue = "value2" };
            TraitScoreList traitscores = new TraitScoreList() { traitScore1, traitScore2 };

            Sif3Item sif3Item = new Sif3Item() { TraitScores = traitscores };
            ItemList itemList = new ItemList() {sif3Item };

            Sif3StudentResponseSet sif3StudentResponseSet = new Sif3StudentResponseSet();
            sif3StudentResponseSet.RefId = "90E298F70E094EE2B8B52DFD88006AF2";
            sif3StudentResponseSet.AssessmentAdministrationRefId = "90E298F70E094EE2B8B52DFD88006AF3";
            sif3StudentResponseSet.StudentPersonalRefId = "90E298F70E094EE2B8B52DFD88006AF4";
            sif3StudentResponseSet.AssessmentRegistrationRefId = "90E298F70E094EE2B8B52DFD88006AF5";
            sif3StudentResponseSet.Items = itemList;

            if (log.IsDebugEnabled) log.Debug("Sif3StudentResponseSet instance: " + sif3StudentResponseSet.ToXml());
            Console.WriteLine("Sif3StudentResponseSet instance: " + sif3StudentResponseSet.ToXml());
        }

    }

}
