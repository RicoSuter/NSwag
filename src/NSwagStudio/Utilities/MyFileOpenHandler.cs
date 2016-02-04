//-----------------------------------------------------------------------
// <copyright file="MyFileOpenHandler.cs" company="MyToolkit">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/MyToolkit/MyToolkit/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using MyToolkit.Utilities;

namespace NSwagStudio.Utilities
{
    // TODO: Use MyToolkit version 2.5.12+!

    /// <summary>Handles the file open command line parameter and assures that files are opened in the same application. </summary>
    public class MyFileOpenHandler
    {
        /// <summary>Gets the window where the events are dispatched to. </summary>
        public Window Window { get; private set; }

        /// <summary>Occurs when a file should be opened. </summary>
        public event EventHandler<FileOpenEventArgs> FileOpen;

        /// <summary>Initializes the file open handler. </summary>
        /// <param name="window">The window. </param>
        public void Initialize(Window window)
        {
            Window = window;

            var fileName = TryGetFileNameFromCommandLineArgs();
            var cancelEvent = new ManualResetEvent(false);
            var thread = new Thread(() =>
            {
                try
                {
                    var pipeName = "FileOpenHandler_" + GetHash(Assembly.GetExecutingAssembly().FullName);
                    if (TryOpenInRunningApplication(pipeName, fileName))
                        return;

                    if (IsValidFilePath(fileName))
                        OnFileOpened(fileName);

                    ListenForFileOpenedPropagations(pipeName, cancelEvent);
                }
                catch { }
            });
            thread.Start();
            window.Dispatcher.ShutdownStarted += (sender, eventArgs) =>
            {
                cancelEvent.Set();
            };
        }

        private string GetHash(string text)
        {
            var bytes = Encoding.Unicode.GetBytes(text);
            var hashstring = new SHA256Managed();
            var hash = hashstring.ComputeHash(bytes);
            var hashString = string.Empty;
            foreach (var x in hash)
                hashString += String.Format("{0:x2}", x);
            return hashString;
        }

        private string TryGetFileNameFromCommandLineArgs()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                if (File.Exists(args[1]))
                    return args[1];
            }
            return null;
        }

        private bool TryOpenInRunningApplication(string pipeName, string fileName)
        {
            try
            {
                using (var client = new NamedPipeClientStream(".", pipeName, PipeDirection.InOut, PipeOptions.Asynchronous))
                {
                    client.Connect(100);
                    if (client.IsConnected)
                    {
                        if (fileName != null)
                        {
                            var bytes = Encoding.UTF8.GetBytes(fileName + "\0");
                            client.Write(bytes, 0, bytes.Length);
                            client.Dispose();
                        }
                        Environment.Exit(0);
                        //Window.Dispatcher.InvokeAsync(() => Window.Close());
                        return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private void ListenForFileOpenedPropagations(string pipeName, ManualResetEvent cancelEvent)
        {
            using (var server = new NamedPipeServerStream(pipeName,
                PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous))
            {
                while (true)
                {
                    server.WaitForConnection(cancelEvent);
                    if (server.IsConnected)
                    {
                        var fileName = ReadFileName(server);

                        if (IsValidFilePath(fileName))
                            OnFileOpened(fileName);

                        server.Disconnect();
                    }
                }
            }
        }

        private static string ReadFileName(NamedPipeServerStream server)
        {
            char ch;
            var fileName = "";
            while ((ch = (char)server.ReadByte()) != 0)
                fileName += ch;
            return fileName;
        }

        private static bool IsValidFilePath(string fileName)
        {
            return !string.IsNullOrEmpty(fileName) && File.Exists(fileName);
        }

        private void OnFileOpened(string fileName)
        {
            Window.Dispatcher.InvokeAsync(() =>
            {
                Window.Activate();

                if (Window.WindowState == WindowState.Minimized)
                    Window.WindowState = WindowState.Normal;

                Window.Dispatcher.InvokeAsync(() =>
                {
                    var copy = FileOpen;
                    if (copy != null)
                        copy(this, new FileOpenEventArgs { FileName = fileName });
                });
            });
        }
    }
}