using System.ComponentModel;

namespace Spooksoft.VisualStateManager.Conditions.LambdaConditions
{
    internal interface IBaseChainedLambdaStep<TInput>
        where TInput : class, INotifyPropertyChanged
    {
        void UpdateSource(TInput newSource, bool force);
    }
}
