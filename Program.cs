using Avalonia;
using Avalonia.LinuxFramebuffer.Input;
using Avalonia.LinuxFramebuffer.Input.NullInput;
using Avalonia.Platform;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MyApp;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static int Main(string[] args)
    {
        FileStream ostrm;
        StreamWriter writer;
        TextWriter oldOut = Console.Out;
        try
        {
            ostrm = new FileStream("./Redirect.txt", FileMode.OpenOrCreate, FileAccess.Write);
            writer = new StreamWriter(ostrm);
        }
        catch (Exception e)
        {
            Console.WriteLine("Cannot open Redirect.txt for writing");
            Console.WriteLine(e.Message);
            return 0;
        }

        Console.SetOut(writer);

        var BobTheBuilder = BuildAvaloniaApp();



        if (args.Contains("--drm"))
        {
            SilenceConsole();
            return BobTheBuilder.StartLinuxDrm(args, null, 1, null);
        }

        if (args.Contains("--fb"))
        {
            SilenceConsole();
            return BobTheBuilder.StartLinuxFbDev(args, null, 1, new NullInputBackend());
        }

        return BobTheBuilder.StartWithClassicDesktopLifetime(args);
    }

    private static void SilenceConsole()
    {
        new Thread(() =>
        {
            Console.CursorVisible = false;
            while (true)
                Console.ReadKey(true);
        })
        { IsBackground = true }.Start();
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
