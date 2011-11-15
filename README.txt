#########################################################################################################
# Copyright 2010-2011 Systemic Pty Ltd
# 
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#    http://www.apache.org/licenses/LICENSE-2.0
# 
# Unless required by applicable law or agreed to in writing, software distributed under the License 
# is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
# or implied.
# See the License for the specific language governing permissions and limitations under the License.
########################################################################################################

#########################################################################################################
# Summary
#########################################################################################################

The SIFCommon Framework abstracts low level functionality that is common to all agents by means of providing 
an easy to use API on top of the Edustructures ADK.  It adds further functionality behind the scene such as interfaces 
to deal with large data sets, multi-threading etc. This functionality is fully transparent to a developer. 
This allows the agent developer to wire up the various components of an agent (subscribers & publishers) in 
an efficient manner by only writing the minimal amount of code to have the skeleton of an agent ready for 
deployment. Many components and their behaviour are controlled by a configuration file rather than writing a 
large amount of code. The developer can then concentrate on the business logic or data access layer to 
retrieve/store data from/to their system rather then spending time writing agent infrastructure code.

For details please refer to the Developer's Guide in the 'UserGuide' directory of this project.


#########################################################################################################
# Download Instructions
#########################################################################################################

How to download this project:

Option 1 - As a Zip.
====================
Click on the button marked "ZIP" available from the Code tab.


Option 2 - Using a Git client.
==============================
From the command-line type: git clone git@github.com:nsip/SifCommonFramework-dotNet.git

Note that if you want to use this option but don't have the client installed, it can be 
downloaded from http://git-scm.com/download.