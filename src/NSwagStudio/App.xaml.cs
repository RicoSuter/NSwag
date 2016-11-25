using System;
using System.Windows;
using Microsoft.ApplicationInsights;

namespace NSwagStudio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TelemetryClient Telemetry = new TelemetryClient();

        public App()
        {
            InitializeTelemetry();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Telemetry.TrackEvent("ApplicationStart");
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Telemetry.TrackEvent("ApplicationExit");
            Telemetry.Flush();
        }

        private void InitializeTelemetry()
        {
#if !DEBUG
            Telemetry.InstrumentationKey = "4a2b98c9-f171-4224-bfe1-b53cf227b1de";
            Telemetry.Context.User.Id = ApplicationSettings.GetSetting("TelemetryUserId", Guid.NewGuid().ToString());
            Telemetry.Context.Session.Id = Guid.NewGuid().ToString();
            Telemetry.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
            Telemetry.Context.Component.Version = GetType().Assembly.GetName().Version.ToString();

            ApplicationSettings.SetSetting("TelemetryUserId", Telemetry.Context.User.Id);
#endif
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Telemetry.TrackException(args.ExceptionObject as Exception);
            Telemetry.Flush();
        }
    }
}
