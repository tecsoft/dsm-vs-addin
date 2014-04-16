using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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

        private void AddMenu()
        {
            MenuItem exit = new MenuItem() { Text = "Exit" };
            exit.Click += new EventHandler(exit_Click);

            MenuItem install = new MenuItem() { Text = "Install" };
            install.Click += new EventHandler(install_Click);

            _icon.ContextMenu = new ContextMenu(new MenuItem[] { install, exit });

            _icon.MouseClick += new MouseEventHandler(_icon_MouseClick);
        }

        private void _icon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                DoInstall();
        }

        private void DoInstall()
        {
            _installer.Run();
        }

        private void exit_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void install_Click(object sender, EventArgs e)
        {
            DoInstall();
        }

        private void icon_BalloonTipClicked(object sender, EventArgs e)
        {
            AddMenu();

            DoInstall();
        }

        private void icon_BalloonTipClosed(object sender, EventArgs e)
        {
            AddMenu();
        }

        public void Dispose()
        {
            _icon.BalloonTipClosed -= new EventHandler(icon_BalloonTipClosed);
            _icon.BalloonTipClicked -= new EventHandler(icon_BalloonTipClicked);
            _icon.MouseClick -= new MouseEventHandler(_icon_MouseClick);

            _icon.Dispose();
        }
    }
}