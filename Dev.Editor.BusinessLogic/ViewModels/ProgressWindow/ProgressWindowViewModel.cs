using Dev.Editor.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using Spooksoft.VisualStateManager.Conditions;
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
        private readonly SimpleCondition cancelAvailableCondition;

        private object workerParameter;
        private string currentOperation;
        private int progress;
        private string operationTitle;

        public ProgressWindowViewModel(string operationTitle, BackgroundWorker worker, object workerParameter, IProgressWindowAccess access)
        {
            OperationTitle = operationTitle;
            this.worker = worker;
            this.workerParameter = workerParameter;

            this.access = access;
            currentOperation = String.Empty;
            progress = 0;

            worker.ProgressChanged += HandleWorkerProgressChanged;
            worker.RunWorkerCompleted += HandleWorkerCompleted;

            cancelAvailableCondition = new SimpleCondition(worker.WorkerSupportsCancellation);

            CancelCommand = new AppCommand(obj => DoCancel(), cancelAvailableCondition);
        }

        private void HandleWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            access.Close();
        }

        public void NotifyLoaded()
        {
            worker.RunWorkerAsync(workerParameter);
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

        public string OperationTitle
        {
            get => operationTitle;
            set => Set(ref operationTitle, () => OperationTitle, value);
        }
    }
}
