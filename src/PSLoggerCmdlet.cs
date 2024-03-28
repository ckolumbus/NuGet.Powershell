/*
   Copyright (c) Chris Drexler

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
using System.Management.Automation;
using System.Threading.Tasks;
using NuGet.Common;

namespace NuGet.PowerShell
{

    public abstract class PSLoggerCmdlet : PSCmdlet, ILogger
    {
        public void Log(LogLevel level, string data) {
            switch (level) {
                case LogLevel.Debug:
                    LogDebug(data);
                    break;
                case LogLevel.Verbose:
                    LogVerbose(data);
                    break;
                case LogLevel.Minimal: //TODO: really "information"?
                case LogLevel.Information:
                    LogInformation(data);
                    break;
                case LogLevel.Warning:
                    LogWarning(data);
                    break;
                case LogLevel.Error:
                    LogError(data);
                    break;
                default:
                    LogVerbose(data);
                    break;
            }
        }

        public void Log(ILogMessage message)
        {
            Log(message.Level, message.Message);
        }

        public Task LogAsync(LogLevel level, string data)
        {
            Log(level, data);
            return Task.CompletedTask;

        }

        public Task LogAsync(ILogMessage message)
        {
            Log(message);
            return Task.CompletedTask;
        }

        /* TODO: all logging disabled because when Nuget SDK methods actually use the logging faciltiy the following error happens
                 and the root cause/possible solution is not understood
                 Error: The WriteObject and WriteError methods cannot be called from outside the overrides of the
                        BeginProcessing, ProcessRecord, and EndProcessing methods
        */
        /*
        public void LogDebug(string data) => WriteDebug(data);
        public void LogError(string data) => WriteError(new ErrorRecord(new Exception(data), "PSLoggerCmdlet", ErrorCategory.NotSpecified , this));
        public void LogInformation(string data) => WriteInformation(new InformationRecord(data, ""));
        public void LogInformationSummary(string data) => WriteInformation(new InformationRecord(data, ""));
        public void LogMinimal(string data) => WriteVerbose(data);
        public void LogVerbose(string data) => WriteVerbose(data);
        public void LogWarning(string data) => WriteVerbose(data);
        */
        public void LogDebug(string data) { }
        public void LogError(string data) { }
        public void LogInformation(string data) { }
        public void LogInformationSummary(string data) { }
        public void LogMinimal(string data) { }
        public void LogVerbose(string data) { }
        public void LogWarning(string data) { }
    }
}