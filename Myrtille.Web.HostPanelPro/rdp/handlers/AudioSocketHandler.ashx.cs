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

using System.Web;
using System.Web.SessionState;
using Microsoft.Web.WebSockets;
using Myrtille.Library;

namespace Myrtille.Web
{
    public class AudioSocketHandler : IHttpHandler, IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.IsWebSocketRequest)
            {
                context.AcceptWebSocketRequest(
                    new RemoteSessionAudioSocketHandler(
                        context,
                        string.IsNullOrEmpty(context.Request["binary"]) ? true : context.Request["binary"] == "true",
                        context.Request["clientId"]));
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}