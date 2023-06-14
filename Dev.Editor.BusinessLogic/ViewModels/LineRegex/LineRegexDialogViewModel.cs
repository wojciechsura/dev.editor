using Dev.Editor.BusinessLogic.Models.LineRegex;
using Dev.Editor.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.LineRegex
{
    public class LineRegexDialogViewModel : BaseViewModel
    {
        private readonly ILineRegexDialogViewModelAccess access;
        private SimpleCondition regexValidCondition;

        private string regex;
        private bool notMatching;

        private void DoCancel()
        {
            access.Close(false);
        }

        private void DoOk()
        {
            access.Close(true);
        }

        private void RegexChanged()
        {
            try
            {
                new Regex(regex);
                regexValidCondition.Value = true;
            }
            catch
            {
                regexValidCondition.Value = false;
            }
        }

        public LineRegexDialogViewModel(ILineRegexDialogViewModelAccess access)
        {
            this.access = access;

            regexValidCondition = new SimpleCondition(false);

            OkCommand = new AppCommand(obj => DoOk(), regexValidCondition);
            CancelCommand = new AppCommand(obj => DoCancel());
        }

        public string Regex
        {
            get => regex;
            set => Set(ref regex, () => Regex, value, RegexChanged);
        }

        public bool NotMatching
        {
            get => notMatching;
            set => Set(ref notMatching, () => NotMatching, value);
        }

        public ICommand OkCommand { get; }
        public ICommand CancelCommand { get; }

        public LineRegexResultModel Result => new LineRegexResultModel(Regex, NotMatching);
    }
}
