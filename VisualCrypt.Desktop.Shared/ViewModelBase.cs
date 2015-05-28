using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;

namespace VisualCrypt.Desktop.Shared
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

        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
                throw new ArgumentNullException("propertyExpression");

            var memberExpression = propertyExpression.Body as MemberExpression;
            if (memberExpression == null)
                throw new ArgumentException(@"The expression is not a member access expression.", "propertyExpression");

            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
                throw new ArgumentException(@"The member access expression does not access a property.",
                    "propertyExpression");

            MethodInfo getMethod = property.GetGetMethod(true);
            if (getMethod.IsStatic)
                throw new ArgumentException(@"The referenced property is a static property.", "propertyExpression");

            return memberExpression.Member.Name;
        }
    }
}