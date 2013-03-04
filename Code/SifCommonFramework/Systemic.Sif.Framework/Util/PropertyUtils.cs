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
using System.Collections.Generic;
using OpenADK.Library;

namespace Systemic.Sif.Framework.Util
{

    /// <summary>
    /// Utility class for processing of Agent properties.
    /// </summary>
    class PropertyUtils
    {

        /// <summary>
        /// Parse for assembly and class name details. The format of the property string is:
        ///     assembly_name,class_name
        /// If no comma present, the property string is assumed to represent a class name of the current executable
        /// and assembly name is null.
        /// If the property string is null or empty, null values will be returned.
        /// </summary>
        /// <param name="property">Property containing assembly and/or class name.</param>
        /// <returns>An array (of size 2) containing assembly name then class name.</returns>
        private static string[] ParseClassDetails(string property)
        {
            string[] classDetails = new string[2] { null, null };

            if (!String.IsNullOrEmpty(property))
            {
                string[] values = property.Split(',');

                if (values.Length == 1)
                {
                    classDetails[1] = (String.Empty.Equals(values[0].Trim()) ? null : values[0].Trim());
                }
                else if (values.Length == 2)
                {
                    classDetails[0] = (String.Empty.Equals(values[0].Trim()) ? null : values[0].Trim());
                    classDetails[1] = (String.Empty.Equals(values[1].Trim()) ? null : values[1].Trim());
                }

            }

            return classDetails;
        }

        /// <summary>
        /// Parse for the assembly name in the specified property string. The format of the property string is:
        ///     assembly_name,class_name
        /// If no comma present, then the assembly name is null.
        /// </summary>
        /// <param name="property">Property containing assembly name.</param>
        /// <returns>Assembly name if present; null otherwise.</returns>
        internal static string ParseAssemblyName(string property)
        {
            return ParseClassDetails(property)[0];
        }

        /// <summary>
        /// Parse for the class name in the specified property string. The format of the property string is:
        ///     assembly_name,class_name
        /// If no comma present, the property string is assumed to represent a class name of the current executable.
        /// </summary>
        /// <param name="property">Property containing class name</param>
        /// <returns>Class name if present; null otherwise.</returns>
        internal static string ParseClassName(string property)
        {
            return ParseClassDetails(property)[1];
        }

        /// <summary>
        /// Parse the property string for a comma separated list of SIF Object types, e.g. StudentPersonal, SchoolInfo.
        /// If a string does not represent a valid SIF Object type, it will be ignored.
        /// </summary>
        /// <param name="property">Comma separated list of SIF Object types.</param>
        /// <returns>List of SIF Object types.</returns>
        internal static IList<IElementDef> ParseElementDefinitions(string property)
        {
            IList<IElementDef> elementDefinitions = new List<IElementDef>();

            if (!String.IsNullOrEmpty(property))
            {
                string[] values = property.Split(',');

                foreach (string value in values)
                {

                    if (!String.IsNullOrEmpty(value))
                    {
                        IElementDef elementDefinition = Adk.Dtd.LookupElementDef(value.Trim());

                        if (elementDefinition != null && !elementDefinitions.Contains(elementDefinition))
                        {
                            elementDefinitions.Add(elementDefinition);
                        }

                    }

                }

            }

            return elementDefinitions;
        }

    }

}
