/*
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
using Newtonsoft.Json;
using Myrtille.Services.Contracts;

namespace Myrtille.Library
{
    public class ConnectionClient: Services.ConnectionService
    {
        private static IDictionary<Guid, RemoteSessionState> connectionsState = new Dictionary<Guid, RemoteSessionState>();
        private static object connectionsLock = new object();

        public ConnectionClient()
        {
        }

        public override ConnectionInfo GetConnectionInfo(Guid connectionId)
        {
            return base.GetConnectionInfo(connectionId);
        }

        public override bool IsUserAllowedToConnectHost(string domain, string userName, string hostIPAddress, Guid vmGuid)
        {
            return base.IsUserAllowedToConnectHost(domain, userName, hostIPAddress, vmGuid);
        }

        public override bool SetConnectionState(Guid connectionId, string IPAddress, Guid vmGuid, RemoteSessionState state)
        {
            return base.SetConnectionState(connectionId, IPAddress, vmGuid, state);
        }

        public override bool SetConnectionExitCode(Guid connectionId, string IPAddress, Guid vmGuid, RemoteSessionExitCode exitCode)
        {
            return base.SetConnectionExitCode(connectionId, IPAddress, vmGuid, exitCode);
        }
    }
}