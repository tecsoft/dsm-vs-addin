using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Extensibility;
using Microsoft.VisualStudio.CommandBars;
using Tcdev.Dsm.View;

namespace Tcdev.DsmVsAddin
{
	/// <summary>The object for implementing an Add-in.</summary>
	/// <seealso class='IDTExtensibility2' />
	public class Connect : IDTExtensibility2, IDTCommandTarget
	{

        private DTE2 _applicationObject;
        private AddIn _addInInstance;
        
		/// <summary>Implements the constructor for the Add-in object. Place your initialization code within this method.</summary>
		public Connect()
		{
		}

		/// <summary>Implements the OnConnection method of the IDTExtensibility2 interface. Receives notification that the Add-in is being loaded.</summary>
		/// <param term='application'>Root object of the host application.</param>
		/// <param term='connectMode'>Describes how the Add-in is being loaded.</param>
		/// <param term='addInInst'>Object representing this Add-in.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnConnection(object application, ext_ConnectMode connectMode, object addInInst, ref Array custom)
		{
			_applicationObject = (DTE2)application;
			_addInInstance = (AddIn)addInInst;

            if (connectMode == ext_ConnectMode.ext_cm_UISetup)
			{
				object []contextGUIDS = new object[] { };
				Commands2 commands = (Commands2)_applicationObject.Commands;
				string toolsMenuName;
				
				// TODO make sure for international versions the Tools bar is retrieved correectly

				try
				{
					//If you would like to move the command to a different menu, change the word "Tools" to the 
					//  English version of the menu. This code will take the culture, append on the name of the menu
					//  then add the command to that menu. You can find a list of all the top-level menus in the file
					//  CommandBar.resx.
					ResourceManager resourceManager = new ResourceManager("DsmVsAddin.CommandBar", Assembly.GetExecutingAssembly());
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
					Command command = commands.AddNamedCommand2(
                        _addInInstance, 
                        "DSMADDIN", 
                        "Dependency Structure Matrix", 
                        "Generate Dependency Structure Matrix for current solution", 
					     true, 59,
					     ref contextGUIDS, 
					     (int)vsCommandStatus.vsCommandStatusSupported+(int)vsCommandStatus.vsCommandStatusEnabled, 
					     (int)vsCommandStyle.vsCommandStylePict,
					     vsCommandControlType.vsCommandControlTypeButton);
	
					//Add a control for the command to the tools menu:
					if((command != null) && (toolsPopup != null))
					{
						command.AddControl(toolsPopup.CommandBar, 1);
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

            }
		}

		/// <summary>Implements the OnDisconnection method of the IDTExtensibility2 interface. 
        /// Receives notification that the Add-in is being unloaded.</summary>
		/// <param term='disconnectMode'>Describes how the Add-in is being unloaded.</param>
		/// <param term='custom'>Array of parameters that are host application specific.</param>
		/// <seealso class='IDTExtensibility2' />
		public void OnDisconnection(ext_DisconnectMode disconnectMode, ref Array custom)
		{
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
		
		/// <summary>Implements the QueryStatus method of the IDTCommandTarget interface. This is called when the command's availability is updated</summary>
		/// <param term='commandName'>The name of the command to determine state for.</param>
		/// <param term='neededText'>Text that is needed for the command.</param>
		/// <param term='status'>The state of the command in the user interface.</param>
		/// <param term='commandText'>Text requested by the neededText parameter.</param>
		/// <seealso class='Exec' />
		public void QueryStatus(string commandName, vsCommandStatusTextWanted neededText, ref vsCommandStatus status, ref object commandText)
		{
			if(neededText == vsCommandStatusTextWanted.vsCommandStatusTextWantedNone)
			{
                //MessageBox.Show(commandName);
				if(commandName == "Tcdev.DsmVsAddin.Connect.DSMADDIN")
				{
					status = (vsCommandStatus)vsCommandStatus.vsCommandStatusSupported|vsCommandStatus.vsCommandStatusEnabled;
					return;
				}
			}
		}

		/// <summary>Implements the Exec method of the IDTCommandTarget interface. 
        /// This is called when the command is invoked.</summary>
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
				if(commandName == "Tcdev.DsmVsAddin.Connect.DSMADDIN")
				{
                    try
                    {
                        if ( ! _applicationObject.Solution.IsOpen)
                        {
                            MessageBox.Show(
                                "The Dependency Structure Matrix plugin requires an open solution",
                                "Dependency Structure Matrix", 
                                MessageBoxButtons.OK,MessageBoxIcon.Warning
                                );
                        }
                        else
                        {
                            VisualStudioAdapter dsm = new VisualStudioAdapter();
                            FileInfo solutionFile = new FileInfo(_applicationObject.Solution.FullName);

                            FindProjects();
                            IdentifyAssemblies(dsm);
                            IdentifyReferences(dsm);

                            AddAssemblies(dsm);

                            dsm.Open(solutionFile.Directory.FullName, solutionFile.Name);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.ToString());
                    }
					return;
				}
			}
		}

        IList<Project> FindProjects()
        {
            _projects = new List<Project>();

            foreach (Project p in _applicationObject.Solution.Projects)
            {
                if (p != null)
                {
                    if (p.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        GetSolutionFolderProjects(p, _projects);
                    }
                    else
                    {
                        _projects.Add(p);
                    }
                }
            }

            return _projects;
        }

        static void GetSolutionFolderProjects( Project p, IList<Project> projects )
        {
            foreach (ProjectItem pi in p.ProjectItems)
            {
                Project subProject = pi.SubProject;

                if (subProject != null)
                {
                    if (subProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
                    {
                        GetSolutionFolderProjects(subProject, projects);
                    }
                    else
                    {
                        projects.Add(subProject);
                    }
                }
            }
        }

        IList<Project> _projects = new List<Project>();
		IDictionary<string, string > _references = new Dictionary<string,string>();
		IDictionary<string, string > _assemblies = new Dictionary<string, string >(); 
		
		public void IdentifyAssemblies( VisualStudioAdapter dsm )
		{
            foreach( Project p in _projects )
            { 
                try
                {
                    string pathName = AssemblyPath( p );
                    if ( !File.Exists( pathName ) )
                    {
                        MessageBox.Show( "One or more files have not been built - please recompile the current configuration" );
                        return;
                    }

                    _assemblies.Add( p.Name, pathName ); 
                }
                catch(Exception e )
                {
                    System.Diagnostics.Debug.WriteLine("Cannot get path for project " + p.Name + ": " + e.Message);
                }
            }
        }

        public void AddAssemblies(VisualStudioAdapter dsm)
        {
            foreach (var item in _assemblies.OrderBy(x => x.Key ))
            {
                dsm.LoadAssembly(item.Value, false);
            }

            foreach( var item in _references.OrderBy(x=> x.Key ) )
            {
                dsm.LoadAssembly(item.Value,true);
            }
        }
        //---------------------------------------------------------------------------------------------------------
        private string AssemblyPath( Project project )
        {
            string fullPath       = project.Properties.Item( "FullPath" ).Value.ToString();
            string outputPath     = project.ConfigurationManager.ActiveConfiguration.Properties.Item( "OutputPath" ).Value.ToString();
            string outputFileName = project.Properties.Item( "OutputFileName" ).Value.ToString();
            
            return Path.Combine( Path.Combine( fullPath, outputPath ), outputFileName );
        }

        //----------------------------------------------------------------------------------------------------------
        void IdentifyReferences(VisualStudioAdapter dsm)
        {
            foreach( Project p in _projects )
            {
                try
                {
                    VSLangProj.VSProject vsProject = p.Object as VSLangProj.VSProject;

                    if (vsProject != null)
                    {
                        foreach (VSLangProj.Reference r in vsProject.References)
                        {
                            if (r.SourceProject == null) // external reference
                            {
                                AssemblyName an = new AssemblyName(r.Name);
                                if (_references.ContainsKey(an.Name) == false)
                                {
                                    if (File.Exists(r.Path))
                                    {
                                        _references.Add(r.Name, r.Path);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(" resolver " + e.Message);
                }
            }
        }
        //---------------------------------------------------------------------------------------------------------
	}
}