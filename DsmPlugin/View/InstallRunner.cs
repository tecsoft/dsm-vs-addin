using System;
using System.Windows.Forms;
using Tcdev.Dsm.Install;

namespace Tcdev.Dsm.View
{
    public static class InstallRunner
    {
        static Installer _installer;

        public static void Run( Object o)
        {
            try
            {
                Installer installer = GetInstaller();
                bool available = installer.NewVersion();

                if (available)
                {
                    try
                    {
                        installer.Load();

                        (o as Control).Invoke((MethodInvoker)delegate { Notify(); });
                    }
                    catch (Exception ex )
                    {
                        ErrorDialog.Show( "Sorry, but there was an error while trying to install the latest version:" + 
                            Environment.NewLine + ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                // network etc etc not available we ignore any errors silently
                // and try next time we run the program
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        internal static void Notify()
        {
            var notifier = new InstallNotifier( GetInstaller() );
            notifier.Show();
        }

        internal static Installer GetInstaller()
        {
            if ( _installer == null )
                _installer = new Installer("http://www.tom-carter.net/Version.txt",
                                    "http://www.tom-carter.net/DsmInstaller.msi");

            return _installer;
        }
    }
}
