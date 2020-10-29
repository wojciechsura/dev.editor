﻿using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.ExceptionWindow
{
    public class ExceptionWindowViewModel
    {
        private readonly string exceptionText;
        private readonly IExceptionWindowAccess access;

        private void DoOk()
        {
            access.Close();
        }

        public ExceptionWindowViewModel(IExceptionWindowAccess access, Exception exception)
        {
            this.access = access;

            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Type:        {exception.GetType().FullName}");
            sb.AppendLine($"Message:     {exception.Message}");
            sb.AppendLine($"Call stack:  {exception.StackTrace}");

            exceptionText = sb.ToString();

            OkCommand = new AppCommand(obj => DoOk());
        }

        public string ExceptionText => exceptionText;

        public ICommand OkCommand { get; }
    }
}
