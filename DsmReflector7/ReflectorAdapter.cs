using System;
using System.Windows.Forms;
using Reflector;
using Reflector.CodeModel;
using Tcdev.Dsm;
using Tcdev.Dsm.Engine;
using Tcdev.Dsm.View;
using Tcdev.Dsm.Adapters;

namespace Tcdev.DsmReflector7
{
    /// <summary>
    /// Allows Dsm Plugin to be used as a Reflector Add-in
    /// 
    /// The adapter is responsible for getting the assemblies 
    /// </summary>
    
    public class ReflectorAdapter : Tcdev.DsmReflector6.ReflectorAdapter
    {
        //IDsmParentControl    _mainControl;
        //IWindowManager       _windowManager;
        //ICommandBar          _toolBar;
        //ICommandBarSeparator _separator;
        //ICommandBarButton    _button;
        //IAssemblyManager     _assemblyManager;
        //EventHandler         _onAssemblyLoad;
        //EventHandler _onAssemblyUnload;

        //const string PluginID = "TCDSMPLUGIN";
        //bool _loaded = false;

        

        ////-------------------------------------------------------------------------------------------------
        //public ReflectorAdapter()
        //{
        //    _mainControl = new MainControl();
        //    _onAssemblyLoad = new EventHandler(assemblyManager_AssemblyLoaded);
        //    _onAssemblyUnload = new EventHandler(_assemblyManager_AssemblyUnloaded);
        //}

        ////-------------------------------------------------------------------------------------------------

        //public void Open(string directory, string name)
        //{
        //}

        ////-------------------------------------------------------------------------------------------------
        ///// <summary>
        ///// Load function required by reflector to engage the plugin
        ///// </summary>
        ///// <param name="serviceProvider"></param>
        //public void Load(IServiceProvider serviceProvider)
        //{
        //    try
        //    {
        //        _windowManager = (IWindowManager)serviceProvider.GetService(typeof(IWindowManager));

        //        ICommandBarManager cbm = (ICommandBarManager)serviceProvider.GetService(typeof(ICommandBarManager));

        //        _toolBar   = cbm.CommandBars["Tools"];
        //        _separator = _toolBar.Items.AddSeparator();

        //        _button = _toolBar.Items.AddButton(
        //            "Dependency Structure Matrix", new EventHandler(this.button_Click));

        //        _assemblyManager =  serviceProvider.GetService(typeof(IAssemblyManager)) as IAssemblyManager;

        //        foreach (IAssembly assembly in _assemblyManager.Assemblies)
        //        {
        //            if ( assembly == null ) throw new DsmException( "Assembly manager gives null lassembly" );
        //            Target target = new Target( assembly.Name, assembly.Location );
        //            // list target in the list of assemblies available for analysis
        //            _mainControl.AddAssembly( target );
        //        }

        //        _assemblyManager.AssemblyLoaded += _onAssemblyLoad;
        //        _assemblyManager.AssemblyUnloaded += _onAssemblyUnload;

        //        _loaded = true;
        //    }
        //    catch (Exception dsmEx)
        //    {
        //        MessageBox.Show(dsmEx.ToString(), "Exception thrown by DSM Plugin during Load",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        
        ////-------------------------------------------------------------------------------------------------
        ///// <summary>
        ///// Function required by reflector to disengaged the plugin
        ///// </summary>
        //public void Unload()
        //{
        //    if (_loaded)
        //    {
        //        try
        //        {
        //            bool isClosed = _mainControl.OnClosing();

        //            if (isClosed)
        //            {
        //                _mainControl = null;

        //                _toolBar.Items.Remove(_separator);
        //                _toolBar.Items.Remove(_button);

        //                if (_windowManager.Windows[PluginID] != null)
        //                {
        //                    _windowManager.Windows[PluginID].Visible = false;
        //                    _windowManager.Windows.Remove(PluginID);
        //                }

        //                _assemblyManager.AssemblyLoaded -= _onAssemblyLoad;
        //                _assemblyManager.AssemblyUnloaded -= _onAssemblyUnload;
        //            }
        //        }
        //        catch (Exception dsmEx)
        //        {
        //            MessageBox.Show(dsmEx.ToString(), 
        //                "Exception thrown by DSM Plugin during Unload",
        //                MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //}

        ////-------------------------------------------------------------------------------------------------
        ///// <summary>
        ///// Function called when DSM choses from main menu
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="args"></param>
        //private void button_Click(Object sender, EventArgs args)
        //{
        //    try
        //    {
        //        if (_windowManager.Windows[PluginID] == null)
        //        {
        //            (_mainControl as Control).Dock = DockStyle.Fill;
                    
        //            _windowManager.Windows.Add( PluginID, 
        //                _mainControl as Control, "Dependency Structure Matrix");
        //        }

        //        this._windowManager.Windows[PluginID].Visible = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString(), "Unable to start the DSM PlugIn",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        
        ////-------------------------------------------------------------------------------------------------

        //void assemblyManager_AssemblyLoaded(object sender, EventArgs args)
        //{
        //    try
        //    {
        //        int i = _assemblyManager.Assemblies.Count;

        //        IAssembly a = _assemblyManager.Assemblies[i - 1];


        //        if (a.Status != null && a.Status.Length == 0)
        //        {
        //            _mainControl.AddAssembly(new Target(a.Name, a.Location));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString(), "Unable to start the DSM PlugIn",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //}

        ////-------------------------------------------------------------------------------------------------

        //void _assemblyManager_AssemblyUnloaded(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        _mainControl.ClearAssemblies();

        //        foreach (IAssembly a in _assemblyManager.Assemblies)
        //        {
        //            if (a.Status != null && a.Status.Length == 0)
        //            {
        //                Target target = new Target(a.Name, a.Location);
        //                _mainControl.AddAssembly(target);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.ToString(), "Unable to start the DSM PlugIn",
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }

        //}
    }
}
