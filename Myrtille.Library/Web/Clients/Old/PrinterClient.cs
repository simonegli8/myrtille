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
using System.Diagnostics;
using System.IO;
using System.ServiceModel;
using Myrtille.Services.Contracts;

namespace Myrtille.Library
{
    public class PrinterClient : ClientBase<IPrinterService>, IPrinterService
    {
        public Stream GetPdfFile(Guid remoteSessionId, string fileName)
        {
            try
            {
                return Channel.GetPdfFile(remoteSessionId, fileName);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to download pdf file {0}, remote session {1} ({2})", fileName, remoteSessionId, exc);
                throw;
            }
        }

        public void DeletePdfFile(Guid remoteSessionId, string fileName)
        {
            try
            {
                Channel.DeletePdfFile(remoteSessionId, fileName);
            }
            catch (Exception exc)
            {
                Trace.TraceError("Failed to delete pdf file {0}, remote session {1} ({2})", fileName, remoteSessionId, exc);
                throw;
            }
        }
    }
}