using ObjCRuntime;
using UIKit;

namespace mauiworkshop
{
    public class Program
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // Set up the unhandled exception event handler
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler;

            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }

        // Event handler for unhandled exceptions
        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            // Handle the exception here (e.ExceptionObject contains the exception)
            // You can log the exception, display an error message, etc.
            // For example, you can use a logging library or write to the console:
            Console.WriteLine($"Unhandled Exception: {e.ExceptionObject}");
        }
    }
}
