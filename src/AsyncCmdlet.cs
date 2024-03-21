
// source: copied & slightly adapted from  https://github.com/felixfbecker/PSKubectl/blob/70356d1ece3bda420d4ca87d5b8f9b538d2668c5/src/AsyncCmdlet.cs

/*
    MIT License

    Copyright (c) 2018 Felix Frederick Becker

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/


using System;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;

namespace NuGet.PowerShell
{

    public abstract class AsyncCmdlet
        : PSLoggerCmdlet, IDisposable
    {
        /// <summary>
        ///		The source for cancellation tokens that can be used to cancel the operation.
        /// </summary>
        readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();


        /// <summary>
        ///		Initialise the <see cref="AsyncCmdlet"/>.
        /// </summary>
        protected AsyncCmdlet()
        {
        }


        /// <summary>
        ///		Finaliser for <see cref="AsyncCmdlet"/>.
        /// </summary>
        ~AsyncCmdlet()
        {
            Dispose(false);
        }


        /// <summary>
        ///		Dispose of resources being used by the Cmdlet.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        /// <summary>
        ///		Dispose of resources being used by the Cmdlet.
        /// </summary>
        /// <param name="disposing">
        ///		Explicit disposal?
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _cancellationSource.Dispose();
        }

        /// <summary>
        ///		Asynchronously perform Cmdlet pre-processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task BeginProcessingAsync()
        {
            return BeginProcessingAsync(_cancellationSource.Token);
        }


        /// <summary>
        ///		Asynchronously perform Cmdlet pre-processing.
        /// </summary>
        /// <param name="cancellationToken">
        ///		A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task BeginProcessingAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        /// <summary>
        ///		Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task ProcessRecordAsync()
        {
            return ProcessRecordAsync(_cancellationSource.Token);
        }


        /// <summary>
        ///		Asynchronously perform Cmdlet processing.
        /// </summary>
        /// <param name="cancellationToken">
        ///		A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task ProcessRecordAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        /// <summary>
        ///		Asynchronously perform Cmdlet post-processing.
        /// </summary>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task EndProcessingAsync()
        {
            return EndProcessingAsync(_cancellationSource.Token);
        }


        /// <summary>
        ///		Asynchronously perform Cmdlet post-processing.
        /// </summary>
        /// <param name="cancellationToken">
        ///		A <see cref="CancellationToken"/> that can be used to cancel the asynchronous operation.
        /// </param>
        /// <returns>
        ///		A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        protected virtual Task EndProcessingAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }


        /// <summary>
        ///		Perform Cmdlet pre-processing.
        /// </summary>
        protected sealed override void BeginProcessing()
        {
            ThreadAffinitiveSynchronizationContext.RunSynchronized(
                () => BeginProcessingAsync()
            );
        }


        /// <summary>
        ///		Perform Cmdlet processing.
        /// </summary>
        protected sealed override void ProcessRecord()
        {
            ThreadAffinitiveSynchronizationContext.RunSynchronized(
                () => ProcessRecordAsync()
            );
        }


        /// <summary>
        ///		Perform Cmdlet post-processing.
        /// </summary>
        protected sealed override void EndProcessing()
        {
            ThreadAffinitiveSynchronizationContext.RunSynchronized(
                () => EndProcessingAsync()
            );
        }


        /// <summary>
        ///		Interrupt Cmdlet processing (if possible).
        /// </summary>
        protected sealed override void StopProcessing()
        {
            _cancellationSource.Cancel();


            base.StopProcessing();
        }


        /// <summary>
        ///		Write a progress record to the output stream, and as a verbose message.
        /// </summary>
        /// <param name="progressRecord">
        ///		The progress record to write.
        /// </param>
        protected void WriteVerboseProgress(ProgressRecord progressRecord)
        {
            if (progressRecord == null)
                throw new ArgumentNullException(nameof(progressRecord));


            WriteProgress(progressRecord);
            WriteVerbose(progressRecord.StatusDescription);
        }


        /// <summary>
        ///		Write a progress record to the output stream, and as a verbose message.
        /// </summary>
        /// <param name="progressRecord">
        ///		The progress record to write.
        /// </param>
        /// <param name="messageOrFormat">
        ///		The message or message-format specifier.
        /// </param>
        /// <param name="formatArguments">
        ///		Optional format arguments.
        /// </param>
        protected void WriteVerboseProgress(ProgressRecord progressRecord, string messageOrFormat, params object[] formatArguments)
        {
            if (progressRecord == null)
                throw new ArgumentNullException(nameof(progressRecord));


            if (String.IsNullOrWhiteSpace(messageOrFormat))
                throw new ArgumentException("Argument cannot be null, empty, or composed entirely of whitespace: 'messageOrFormat'.", nameof(messageOrFormat));


            if (formatArguments == null)
                throw new ArgumentNullException(nameof(formatArguments));


            progressRecord.StatusDescription = String.Format(messageOrFormat, formatArguments);
            WriteVerboseProgress(progressRecord);
        }


        /// <summary>
        ///		Write a completed progress record to the output stream.
        /// </summary>
        /// <param name="progressRecord">
        ///		The progress record to complete.
        /// </param>
        /// <param name="completionMessageOrFormat">
        ///		The completion message or message-format specifier.
        /// </param>
        /// <param name="formatArguments">
        ///		Optional format arguments.
        /// </param>
        protected void WriteProgressCompletion(ProgressRecord progressRecord, string completionMessageOrFormat, params object[] formatArguments)
        {
            if (progressRecord == null)
                throw new ArgumentNullException(nameof(progressRecord));


            if (String.IsNullOrWhiteSpace(completionMessageOrFormat))
                throw new ArgumentException("Argument cannot be null, empty, or composed entirely of whitespace: 'completionMessageOrFormat'.", nameof(completionMessageOrFormat));


            if (formatArguments == null)
                throw new ArgumentNullException(nameof(formatArguments));


            progressRecord.StatusDescription = String.Format(completionMessageOrFormat, formatArguments);
            progressRecord.PercentComplete = 100;
            progressRecord.RecordType = ProgressRecordType.Completed;
            WriteProgress(progressRecord);
            WriteVerbose(progressRecord.StatusDescription);
        }
    }
}