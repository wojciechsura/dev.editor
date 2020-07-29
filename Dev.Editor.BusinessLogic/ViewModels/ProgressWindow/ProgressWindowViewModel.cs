using Dev.Editor.BusinessLogic.ViewModels.Base;
using Dev.Editor.Common.Commands;
using Dev.Editor.Common.Conditions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Dev.Editor.BusinessLogic.ViewModels.ProgressWindow
{
    public class ProgressWindowViewModel : BaseViewModel
    {
        private readonly BackgroundWorker worker;
        private readonly IProgressWindowAccess access;
        private string currentOperation;
        private Condition cancelAvailableCondition;
        private int progress;
       
        public ProgressWindowViewModel(BackgroundWorker worker, IProgressWindowAccess access)
        {
            this.worker = worker;
            this.access = access;
            currentOperation = String.Empty;
            progress = 0;

            worker.ProgressChanged += HandleWorkerProgressChanged;
            worker.RunWorkerCompleted += HandleWorkerCompleted;

            cancelAvailableCondition = new Condition(worker.WorkerSupportsCancellation);

            CancelCommand = new AppCommand(obj => DoCancel(), cancelAvailableCondition);
        }

        private void HandleWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            access.Close();
        }

        public void NotifyLoaded()
        {
            worker.RunWorkerAsync();
        }

        private void HandleWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            Progress = e.ProgressPercentage;
            if (e.UserState is string strUserState)
                CurrentOperation = strUserState;
        }

        private void DoCancel()
        {
            if (worker.WorkerSupportsCancellation)
            {
                worker.CancelAsync();
            }
        }

        public string CurrentOperation 
        {
            get => currentOperation;
            set => Set(ref currentOperation, () => CurrentOperation, value);
        }

        public ICommand CancelCommand { get; }

        public int Progress
        {
            get => progress;
            set => Set(ref progress, () => Progress, value);
        }
    }
}
