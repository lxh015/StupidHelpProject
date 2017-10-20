using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Stupid.Extensions
{
    /// <summary>
    /// WebBrowserExt - WebBrowser extensions
    /// by Noseratio - https://stackoverflow.com/a/22262976/1768303
    /// </summary>
    public static class WebBrowserExt
    {
        const int POLL_DELAY = 500;

        /// <summary>
        /// navigate and download 
        /// </summary>
        /// <param name="webBrowser"></param>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        // navigate and download 
        public static async Task<string> NavigateAsync(this WebBrowser webBrowser, string url, CancellationToken token)
        {
            // navigate and await DocumentCompleted
            var tcs = new TaskCompletionSource<bool>();
            WebBrowserDocumentCompletedEventHandler handler = (s, arg) =>
                tcs.TrySetResult(true);

            using (token.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: true))
            {
                webBrowser.DocumentCompleted += handler;
                try
                {
                    webBrowser.Navigate(url);
                    await tcs.Task; // wait for DocumentCompleted
                }
                finally
                {
                    webBrowser.DocumentCompleted -= handler;
                }
            }

            // get the root element
            var documentElement = webBrowser.Document.GetElementsByTagName("html")[0];

            // poll the current HTML for changes asynchronosly
            var html = documentElement.OuterHtml;
            while (true)
            {
                // wait asynchronously, this will throw if cancellation requested
                await Task.Delay(POLL_DELAY, token);

                // continue polling if the WebBrowser is still busy
                if (webBrowser.IsBusy)
                    continue;

                var htmlNow = documentElement.OuterHtml;
                if (html == htmlNow)
                    break; // no changes detected, end the poll loop

                html = htmlNow;
            }

            // consider the page fully rendered 
            token.ThrowIfCancellationRequested();
            return html;
        }

        /// <summary>
        /// enable HTML5 (assuming we're running IE10+)
        /// </summary>
        // enable HTML5 (assuming we're running IE10+)
        // more info: https://stackoverflow.com/a/18333982/1768303
        public static void SetFeatureBrowserEmulation()
        {
            if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Runtime)
                return;
            var appName = System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                appName, 10000, RegistryValueKind.DWord);
        }



        internal struct VoidTypeStruct { }

        public static Task TimeoutAfter(this Task task, int millisecondsTimeout)
        {
            // Short-circuit #1: infinite timeout or task already completed
            if (task.IsCompleted || (millisecondsTimeout == Timeout.Infinite))
            {
                // Either the task has already completed or timeout will never occur.
                // No proxy necessary.
                return task;
            }

            // tcs.Task will be returned as a proxy to the caller
            TaskCompletionSource<VoidTypeStruct> tcs =
                new TaskCompletionSource<VoidTypeStruct>();

            // Short-circuit #2: zero timeout
            if (millisecondsTimeout == 0)
            {
                // We've already timed out.
                tcs.SetException(new TimeoutException());
                return tcs.Task;
            }

            // Set up a timer to complete after the specified timeout period
            System.Threading.Timer timer = new System.Threading.Timer(state =>
            {
                // Recover your state information
                var myTcs = (TaskCompletionSource<VoidTypeStruct>)state;

                // Fault our proxy with a TimeoutException
                myTcs.TrySetException(new TimeoutException());
            }, tcs, millisecondsTimeout, Timeout.Infinite);

            // Wire up the logic for what happens when source task completes
            task.ContinueWith((antecedent, state) =>
            {
                // Recover our state data
                var tuple =
                    (Tuple<System.Threading.Timer, TaskCompletionSource<VoidTypeStruct>>)state;

                // Cancel the Timer
                tuple.Item1.Dispose();

                // Marshal results to proxy
                MarshalTaskResults(antecedent, tuple.Item2);
            },
            Tuple.Create(timer, tcs),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);

            return tcs.Task;
        }

        internal static void MarshalTaskResults<TResult>(
                Task source, TaskCompletionSource<TResult> proxy)
        {
            switch (source.Status)
            {
                case TaskStatus.Faulted:
                    proxy.TrySetException(source.Exception);
                    break;
                case TaskStatus.Canceled:
                    proxy.TrySetCanceled();
                    break;
                case TaskStatus.RanToCompletion:
                    Task<TResult> castedSource = source as Task<TResult>;
                    proxy.TrySetResult(
                        castedSource == null ? default(TResult) : // source is a Task
                            castedSource.Result); // source is a Task<TResult>
                    break;
            }
        }
    }

    /// <summary>
    /// MessageLoopApartment
    /// STA thread with message pump for serial execution of tasks
    /// by Noseratio - https://stackoverflow.com/a/22262976/1768303
    /// </summary>
    public class MessageLoopApartment : IDisposable
    {
        Thread _thread; // the STA thread

        TaskScheduler _taskScheduler; // the STA thread's task scheduler

        public TaskScheduler TaskScheduler { get { return _taskScheduler; } }

        /// <summary>MessageLoopApartment constructor</summary>
        public MessageLoopApartment()
        {
            var tcs = new TaskCompletionSource<TaskScheduler>();

            // start an STA thread and gets a task scheduler
            _thread = new Thread(startArg =>
            {
                EventHandler idleHandler = null;

                idleHandler = (s, e) =>
                {
                    // handle Application.Idle just once
                    Application.Idle -= idleHandler;
                    // return the task scheduler
                    tcs.SetResult(TaskScheduler.FromCurrentSynchronizationContext());
                };

                // handle Application.Idle just once
                // to make sure we're inside the message loop
                // and SynchronizationContext has been correctly installed
                Application.Idle += idleHandler;
                Application.Run();
            });

            _thread.SetApartmentState(ApartmentState.STA);
            _thread.IsBackground = true;
            _thread.Start();
            _taskScheduler = tcs.Task.Result;
        }

        /// <summary>shutdown the STA thread</summary>
        public void Dispose()
        {
            if (_taskScheduler != null)
            {
                var taskScheduler = _taskScheduler;
                _taskScheduler = null;

                // execute Application.ExitThread() on the STA thread
                Task.Factory.StartNew(
                    () => Application.ExitThread(),
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    taskScheduler).Wait();

                _thread.Join();
                _thread = null;
            }
        }

        /// <summary>Task.Factory.StartNew wrappers</summary>
        public void Invoke(Action action)
        {
            Task.Factory.StartNew(action,
                CancellationToken.None, TaskCreationOptions.None, _taskScheduler).Wait();
        }

        public TResult Invoke<TResult>(Func<TResult> action)
        {
            return Task.Factory.StartNew(action,
                CancellationToken.None, TaskCreationOptions.None, _taskScheduler).Result;
        }

        public Task Run(Action action, CancellationToken token)
        {
            return Task.Factory.StartNew(action, token, TaskCreationOptions.None, _taskScheduler);
        }

        public Task<TResult> Run<TResult>(Func<TResult> action, CancellationToken token)
        {
            return Task.Factory.StartNew(action, token, TaskCreationOptions.None, _taskScheduler);
        }

        public Task Run(Func<Task> action, CancellationToken token)
        {
            return Task.Factory.StartNew(action, token, TaskCreationOptions.None, _taskScheduler).Unwrap();
        }

        public Task<TResult> Run<TResult>(Func<Task<TResult>> action, CancellationToken token)
        {
            return Task.Factory.StartNew(action, token, TaskCreationOptions.None, _taskScheduler).Unwrap();
        }
    }



    /// <summary>
    /// 调用实例
    /// </summary>
    public class WebBrowserUse
    {
        /// <summary>
        /// 调用方法
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        // by Noseratio - https://stackoverflow.com/a/22262976/1768303

        // main logic
        static async Task<string> ScrapSitesAsync(string urls, CancellationToken token)
        {
            using (var apartment = new MessageLoopApartment())
            {
                string result = string.Empty;
                // create WebBrowser inside MessageLoopApartment
                var webBrowser = apartment.Invoke(() => new WebBrowser());
                try
                {

                    Console.WriteLine("URL:\n" + urls);

                    // cancel in 30s or when the main token is signalled
                    var navigationCts = CancellationTokenSource.CreateLinkedTokenSource(token);
                    navigationCts.CancelAfter((int)TimeSpan.FromSeconds(60).TotalMilliseconds);
                    var navigationToken = navigationCts.Token;

                    // run the navigation task inside MessageLoopApartment
                    string html = await apartment.Run(() =>
                        webBrowser.NavigateAsync(urls, navigationToken), navigationToken);

                    result = html.Substring(html.IndexOf("id=\"p\"") + 7);
                    result = result.Remove(result.IndexOf("<"));
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    // dispose of WebBrowser inside MessageLoopApartment
                    apartment.Invoke(() => webBrowser.Dispose());
                }

                return result;
            }
        }
    }


}
