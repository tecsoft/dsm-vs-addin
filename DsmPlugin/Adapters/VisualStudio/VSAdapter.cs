using System;
using Extensibility;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using Tcdev.Dsm.View;
using Tcdev.Dsm.Adapters;
using Tcdev.Dsm.Engine;
using VSLangProj;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using System.Text;

namespace VSAdapter
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class  VisualStudioAdapter : IDTExtensibility2, IDTCommandTarget //, IAdapter
	{
        private DTE2           _applicationObject;
        private AddIn          _addInInstance;
        private Window         _window = null;
        private MainControl    _mainControl = null;
        private WindowEvents   _windowEvents = null;
        private SolutionEvents _solutionEvts = null;
        private Command        _command = null;
        private CommandBarControl _commandCtrl = null;
	
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public VisualStudioAdapter()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. 
        /// Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance     = (AddIn)addInInst;

			if(connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName;

				try
				{
					//If you would like to move the command to a different menu, change the word "Tools" to the 
					//  English version of the menu. This code will take the culture, append on the name of the menu
					//  then add the command to that menu. You can find a list of all the top-level menus in the file
					//  CommandBar.resx.
					ResourceManager resourceManager = new ResourceManager("VSAdapter.CommandBar", Assembly.GetExecutingAssembly());
					CultureInfo cultureInfo = new System.Globalization.CultureInfo(_applicationObject.LocaleID);
					string resourceName = String.Concat(cultureInfo.TwoLetterISOLanguageName, "Tools");
					toolsMenuName = resourceManager.GetString(resourceName);
				}
				catch
				{
					//We tried to find a localized version of the word Tools, but one was not found.
					//  Default to the en-US word, which may work for the current culture.
					toolsMenuName = "Tools";
				}

				//Place the command on the tools menu.
				//Find the MenuBar command bar, which is the top-level command bar holding all the main menu items:
				Microsoft.VisualStudio.CommandBars.CommandBar menuBarCommandBar = 
				    ((Microsoft.VisualStudio.CommandBars.CommandBars)_applicationObject.CommandBars)["MenuBar"];

				//Find the Tools command bar on the MenuBar command bar:
				CommandBarControl toolsControl = menuBarCommandBar.Controls[toolsMenuName];
				CommandBarPopup toolsPopup = (CommandBarPopup)toolsControl;

				//This try/catch block can be duplicated if you wish to add multiple commands to be handled by your Add-in,
				//  just make sure you also update the QueryStatus/Exec method to include the new command names.
				try
				{
					//Add a command to the Commands collection:
					
					int commandStatus = (int)vsCommandStatus.vsCommandStatusSupported;
				
					if( _applicationObject.Solution.IsOpen )
					{
					    commandStatus += (int)vsCommandStatus.vsCommandStatusEnabled;
					 }
					 else
					 {
					 
					    commandStatus += (int)vsCommandStatus.vsCommandStatusInvisible;
					 }
					
					object[] context = { ContextGuids.vsContextGuidSolutionExists };
					                    
					_command = commands.AddNamedCommand2(_addInInstance, 
					    "DSM", "DSM", "Executes the command for VSAdapter", 
					    true, 59, 
					    ref context, commandStatus, 
					    (int)vsCommandStyle.vsCommandStylePictAndText,
					    vsCommandControlType.vsCommandControlTypeButton);
					    
					
					//Add a control for the command to the tools menu:
					if((_command != null) && (toolsPopup != null) )
					{
						this._commandCtrl = _command.AddControl(toolsPopup.CommandBar, 1) as CommandBarControl;
					}

				}
				catch(System.ArgumentException)
				{
					//If we are here, then the exception is probably because a command with that name
					//  already exists. If so there is no need to recreate the command and we can 
                    //  safely ignore the exception.
				}
			}
            else if ( connectMode == ext_ConnectMode.ext_cm_AfterStartup )
            {
                //CreateToolWindow();
                MessageBox.Show( "created after start" );
            }
            
		}

        //void  _solutionEvts_ProjectRenamed(Project Project, string OldName)
        //{
        //    MessageBox.Show( Project.Name + " renamed" );
        //}

        //void _solutionEvts_ProjectRemoved( Project Project )
        //{
        //    MessageBox.Show( Project.Name + " removed" );
        //}

        //void _solutionEvts_ProjectAdded( Project Project )
        //{
        //    MessageBox.Show( Project.Name  + " Added");
        //}


		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
            ////MessageBox.Show("OnDisconnect" );
		    
            //if ( _solutionEvts != null )
            //{
            //    _solutionEvts.ProjectAdded -= new _dispSolutionEvents_ProjectAddedEventHandler( _solutionEvts_ProjectAdded );
            //    _solutionEvts.ProjectRemoved -= new _dispSolutionEvents_ProjectRemovedEventHandler( _solutionEvts_ProjectRemoved );
            //    _solutionEvts.ProjectRenamed -= new _dispSolutionEvents_ProjectRenamedEventHandler( _solutionEvts_ProjectRenamed );
            //    _solutionEvts.Opened -= new _dispSolutionEvents_OpenedEventHandler( SolutionEvents_Opened );
            //    _solutionEvts.BeforeClosing -= new _dispSolutionEvents_BeforeClosingEventHandler( SolutionEvents_BeforeClosing );
            //}
		    
            //if ( _windowEvents != null )
            //{
            //    _windowEvents.WindowActivated -= new _dispWindowEvents_WindowActivatedEventHandler( windowEvents_WindowActivated );
            //    _windowEvents.WindowClosing -= new _dispWindowEvents_WindowClosingEventHandler( windowEvents_WindowClosing );
            //    _windowEvents.WindowCreated -= new _dispWindowEvents_WindowCreatedEventHandler( windowEvents_WindowCreated );
            //    _windowEvents.WindowMoved -= new _dispWindowEvents_WindowMovedEventHandler( windowEvents_WindowMoved );
            //}
            //_mainControl.Dispose();
            //_mainControl = null;
		    
		}

		/// <summary>Implements the OnAddInsUpdate method of the IDTExtensibility2 interface. 
        /// Receives notification when the collection of Add-ins has changed.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />		
		public void OnAddInsUpdate(ref Array custom)
		{
		}

		/// <summary>Implements the OnStartupComplete method of the IDTExtensibility2 interface.
        /// Receives notification that the host application has completed loading.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnStartupComplete(ref Array custom)
		{
		}

		/// <summary>Implements the OnBeginShutdown method of the IDTExtensibility2 interface. 
        /// Receives notification that the host application is being unloaded.</summary>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnBeginShutdown(ref Array custom)
		{
		}
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. 
        /// This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
				if(commandName == "VSAdapter.VisualStudioAdapter.DSM")
				{
				    if ( _applicationObject.Solution.IsOpen == true && _mainControl == null )
				    {
				        status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
			        }
                    else
                    {
                        status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusInvisible;
					}
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. 
        /// This is called when the command is invoked.
        /// </summary>
		/// <param term='commandName'>The name of the command to execute.</param>
		/// <param term='executeOption'>Describes how the command should be run.</param>
		/// <param term='varIn'>Parameters passed from the caller to the command handler.</param>
		/// <param term='varOut'>Parameters passed from the command handler to the caller.</param>
		/// <param term='handled'>Informs the caller if the command was handled or not.</param>
		/// <seealso class='Exec' />
		public void Exec(string commandName, vsCommandExecOption executeOption, ref object varIn, ref object varOut, ref bool handled)
		{
			handled = false;
			if(executeOption == vsCommandExecOption.vsCommandExecOptionDoDefault)
			{
                if ( commandName == "VSAdapter.VisualStudioAdapter.DSM" )
				{
				    MessageBox.Show( "Exec" );
                    //handled = true;
					
                    //if (_mainControl == null )
                    //{
                    //    CreateDsmWindow();
                    //}
                    
                    //_mainControl.BringToFront();
                    //_mainControl.Visible = true;
                    
                    Assembly asm = Assembly .GetExecutingAssembly();

                    ProcessStartInfo pInfo = new ProcessStartInfo( asm.Location );
                    
                    StringBuilder args = new StringBuilder();
                    foreach ( Project p in _applicationObject.Solution.Projects )
                    {
                        string fullPath = p.Properties.Item( "FullPath" ).Value.ToString();
                        string outputPath = p.ConfigurationManager.ActiveConfiguration.Properties.Item( "OutputPath" ).Value.ToString();
                        string outputFileName = p.Properties.Item( "OutputFileName" ).Value.ToString();

                        //Tcdev.Dsm.Target target = new Tcdev.Dsm.Target
                        //(
                        //    outputFileName,
                        //    Path.Combine( Path.Combine( fullPath, outputPath ), outputFileName )
                        //);

                        //_mainControl.AddAssembly( target );
                        
                        args.Append( Path.Combine( Path.Combine( fullPath, outputPath ), outputFileName ) ).Append( ' ' );
                    }
                    
                    pInfo.Arguments = args.ToString();
                    System.Diagnostics.Process.Start( pInfo );
				}
			}
		}

        //private void CreateDsmWindow()
        //{
        //    object programmableObject = null;
        //    string guidString = "{56F3502B-C881-47CA-92C0-9E57BE7F715C}";
            
        //    Windows2 windows2 = _applicationObject.Windows as Windows2;
        //    Assembly asm = Assembly.GetExecutingAssembly();
            
        //    _window = windows2.CreateToolWindow2( _addInInstance, asm.Location, 
        //        "Tcdev.Dsm.View.MainControl", "DSM", guidString, ref programmableObject );

        //    _mainControl = _window.Object as MainControl;
        //    //_window.SetTabPicture( new Bitmap( "images/DSM.gif" ) );
  
        //    _mainControl.Adapter = this;

        //    _windowEvents                  = _applicationObject.Events.get_WindowEvents( null );
        //    _windowEvents.WindowActivated += new _dispWindowEvents_WindowActivatedEventHandler( windowEvents_WindowActivated );
        //    _windowEvents.WindowClosing   += new _dispWindowEvents_WindowClosingEventHandler( windowEvents_WindowClosing );
        //    _windowEvents.WindowCreated   += new _dispWindowEvents_WindowCreatedEventHandler( windowEvents_WindowCreated );
        //    _windowEvents.WindowMoved     += new _dispWindowEvents_WindowMovedEventHandler( windowEvents_WindowMoved );

        //    _solutionEvts                 = _applicationObject.Events.SolutionEvents;
        //    _solutionEvts.ProjectAdded   += new _dispSolutionEvents_ProjectAddedEventHandler( _solutionEvts_ProjectAdded );
        //    _solutionEvts.ProjectRemoved += new _dispSolutionEvents_ProjectRemovedEventHandler( _solutionEvts_ProjectRemoved );
        //    _solutionEvts.ProjectRenamed += new _dispSolutionEvents_ProjectRenamedEventHandler( _solutionEvts_ProjectRenamed );
        //    _solutionEvts.Opened         += new _dispSolutionEvents_OpenedEventHandler( SolutionEvents_Opened );
        //    _solutionEvts.BeforeClosing  += new _dispSolutionEvents_BeforeClosingEventHandler( SolutionEvents_BeforeClosing );

        //    foreach ( Project p in _applicationObject.Solution.Projects )
        //    {
        //        string fullPath       = p.Properties.Item( "FullPath" ).Value.ToString();
        //        string outputPath     = p.ConfigurationManager.ActiveConfiguration.Properties.Item( "OutputPath" ).Value.ToString();
        //        string outputFileName = p.Properties.Item( "OutputFileName" ).Value.ToString();
                
        //        Tcdev.Dsm.Target target = new Tcdev.Dsm.Target
        //        ( 
        //            outputFileName,
        //            Path.Combine( Path.Combine( fullPath, outputPath ), outputFileName )
        //        );

        //        _mainControl.AddAssembly( target );
        //    }
            
        //    _mainControl.Visible = true;
        //    _mainControl.Dock    = DockStyle.Fill;
        //    _window.Visible      = true;
        //}

        //void windowEvents_WindowMoved( Window Window, int Top, int Left, int Width, int Height )
        //{
        //}

        //void windowEvents_WindowCreated( Window Window )
        //{
        //}

        //void windowEvents_WindowClosing( Window Window )
        //{
        //    //MessageBox.Show("Window closing: " + Window.Caption );
        //}

        //void windowEvents_WindowActivated( Window GotFocus, Window LostFocus )
        //{
            
        //   if ( LostFocus.Object is MainControl && LostFocus.Visible == false )
        //   {
        //   MessageBox.Show( "Active closing" );
        //    //LostFocus.Close(vsSaveChanges.vsSaveChangesPrompt );
            
        //    _mainControl.OnClosing();
        //    _mainControl.Dispose();
        //    _mainControl = null;
        //    _window.Close(vsSaveChanges.vsSaveChangesNo );
        //   }
        //}

        //void SolutionEvents_BeforeClosing()
        //{
        //    //MessageBox.Show( "Solution Closing" );
        //    _mainControl.OnClosing();
        //    _mainControl.Dispose();
        //    _mainControl = null;
        //}

        //void VisualStudioAdapter_WindowClosing( Window Window )
        //{
        //    MessageBox.Show( "window closing" );
        //    _mainControl.OnClosing();
        //    _mainControl.Dispose();
        //    _mainControl = null;
        //}

        //void SolutionEvents_Opened()
        //{
        //    MessageBox.Show( "Solution opened");
        //}

		
        //public IAnalyser GetAnalyser()
        //{
        //    return new FrameworkAnalyser();
        //}
		
		
		
	}
}