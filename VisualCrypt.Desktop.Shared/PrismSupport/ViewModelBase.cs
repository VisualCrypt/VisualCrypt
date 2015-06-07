using System;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace VisualCrypt.Desktop.Shared.PrismSupport
{
	public abstract class ViewModelBase : BindableBase
	{
		readonly List<DelegateCommandBase> _allCommands = new List<DelegateCommandBase>();

		protected void RaiseAllCanExecuteChanged()
		{
			foreach (var c in _allCommands)
			{
				c.RaiseCanExecuteChanged();
			}
		}

		protected DelegateCommand CreateCommand(ref DelegateCommand command, Action execute, Func<bool> canExecute)
		{
			if (command == null)
			{
				command = new DelegateCommand(execute, canExecute);
				_allCommands.Add(command);
			}
			return command;
		}

		protected DelegateCommand<T> CreateCommand<T>(ref DelegateCommand<T> command, Action<T> execute,
			Func<T, bool> canExecute)
		{
			if (command == null)
			{
				command = new DelegateCommand<T>(execute, canExecute);
				_allCommands.Add(command);
			}
			return command;
		}
	}
}