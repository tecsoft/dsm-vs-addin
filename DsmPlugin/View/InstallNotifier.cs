using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Tcdev.Dsm.Install;

namespace Tcdev.Dsm.View
{
    internal class InstallNotifier : IDisposable
    {
        NotifyIcon _icon;
        Installer _installer;
        public InstallNotifier(Installer installer)
        {
            _icon = new NotifyIcon();
            _icon.Icon = new Icon( 
                System.Reflection.Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("Tcdev.Dsm.images.DSM.ico"));
            _icon.Visible = true;
            _icon.BalloonTipText = "A new version is available for install";
            _icon.Text = "Click to install latest version";
            _icon.BalloonTipTitle = "Dependency Structure Matrix";
            _icon.BalloonTipIcon = ToolTipIcon.Info;
            _icon.ShowBalloonTip(5000);
            _icon.BalloonTipClosed += new EventHandler(icon_BalloonTipClosed);
            _icon.BalloonTipClicked += new EventHandler(icon_BalloonTipClicked);

            _installer = installer;
        }

        public void Show()
        {
            _icon.Visible = true;
        }

        void AddMenu()
        {
            MenuItem exit = new MenuItem() { Text = "Exit" };
            exit.Click += new EventHandler(exit_Click);

            MenuItem install = new MenuItem() { Text = "Install" };
            install.Click += new EventHandler(install_Click);

            _icon.ContextMenu = new ContextMenu(new MenuItem[]{install, exit});

            _icon.MouseClick += new MouseEventHandler(_icon_MouseClick);
         }

        void _icon_MouseClick(object sender, MouseEventArgs e)
        {
            DoInstall();
        }

        void DoInstall()
        {
            _installer.Run();
            
        }

        void exit_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        void install_Click(object sender, EventArgs e)
        {
            DoInstall();
        }

        void icon_BalloonTipClicked(object sender, EventArgs e)
        {
            AddMenu();
            DoInstall();
        }

        void icon_BalloonTipClosed(object sender, EventArgs e)
        {
            AddMenu();
        }

        #region IDisposable Members

        public void Dispose()
        {
            _icon.BalloonTipClosed -= new EventHandler(icon_BalloonTipClosed);
            _icon.BalloonTipClicked -= new EventHandler(icon_BalloonTipClicked);
            
            _icon.Dispose();
        }

        #endregion
    }
}
