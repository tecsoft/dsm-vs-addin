
using System;
using Tcdev.Dsm.View;

namespace Tcdev.Dsm.Commands
{
	/// <summary>
	/// Interface for all commands - possible which affect the DsmModel
	/// </summary>
	public interface ICommand
	{
        /// <summary>
        /// Run the command
        /// </summary>
        void Execute(MainControl.ProgressUpdateDelegate updateFunction);

        /// <summary>
        /// Set to true if command was run, false if error or if cancelled by user
        /// </summary>
        /// <returns></returns>
        bool Completed
        {
            get;
        }
	}
}
