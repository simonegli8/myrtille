﻿/*
    Myrtille: A native HTML4/5 Remote Desktop Protocol client.

    Copyright(c) 2014-2021 Cedric Coste

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using Myrtille.Services.Contracts;

namespace Myrtille.Web
{
    public class EnterpriseClient : ClientBase<IEnterpriseService>, IEnterpriseService
    {
        public EnterpriseMode GetMode()
        {
            try
            {
                return Channel.GetMode();
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to get enterprise adapter mode ({0})", exc);
                return EnterpriseMode.None;
            }
        }

        public EnterpriseSession Authenticate(string username, string password)
        {
            try
            {
                return Channel.Authenticate(username, password);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to authenticate enterprise user {0} ({1})", username, exc);
                return null;
            }
        }

        public void Logout(string sessionID)
        {
            try
            {
                Channel.Logout(sessionID);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to logout enterprise session ({0})", exc);
                throw;
            }
        }

        public long? AddHost(EnterpriseHostEdit editHost, string sessionID)
        {
            try
            {
                return Channel.AddHost(editHost, sessionID);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to add host {0} ({1})", editHost.HostName, exc);
                throw;
            }
        }

        public EnterpriseHostEdit GetHost(long hostID, string sessionID)
        {
            try
            {
                return Channel.GetHost(hostID, sessionID);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to get host {0} ({1})", hostID, exc);
                throw;
            }
        }

        public bool UpdateHost(EnterpriseHostEdit editHost, string sessionID)
        {
            try
            {
                return Channel.UpdateHost(editHost, sessionID);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to update host {0} ({1})", editHost.HostName, exc);
                throw;
            }
        }

        public bool DeleteHost(long hostID, string sessionID)
        {
            try
            {
                return Channel.DeleteHost(hostID, sessionID);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to delete host {0} ({1})", hostID, exc);
                throw;
            }
        }

        public List<EnterpriseHost> GetSessionHosts(string sessionID)
        {
            try
            {
                return Channel.GetSessionHosts(sessionID);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to get hosts for session {0} ({1})", sessionID, exc);
                throw;
            }
        }

        public EnterpriseConnectionDetails GetSessionConnectionDetails(string sessionID, long hostID, string sessionKey)
        {
            try
            {
                return Channel.GetSessionConnectionDetails(sessionID, hostID, sessionKey);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to get connection details for session {0} ({1})", sessionID, exc);
                throw;
            }
        }

        public string CreateUserSession(string sessionID, long hostID, string username, string password, string domain)
        {
            try
            {
                return Channel.CreateUserSession(sessionID, hostID, username, password, domain);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to create user session ({0})", exc);
                throw;
            }
        }

        public bool ChangeUserPassword(string username, string oldPassword, string newPassword)
        {
            try
            {
                return Channel.ChangeUserPassword(username, oldPassword, newPassword);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to change user password ({0})", exc);
                throw;
            }
        }

        public bool AddSessionHostCredentials(EnterpriseHostSessionCredentials credentials)
        {
            try
            {
                return Channel.AddSessionHostCredentials(credentials);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to create session credentials for user({0})", exc);
                throw;
            }
        }
    }
}