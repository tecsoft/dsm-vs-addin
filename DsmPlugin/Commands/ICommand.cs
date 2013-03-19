
using System;

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
		void Execute();

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
