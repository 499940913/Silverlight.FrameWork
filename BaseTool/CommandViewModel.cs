using System;
using System.Windows;
using System.Windows.Input;

namespace BaseTool
{
    public class TabCloseCommandViewModel
    {
        private readonly object _host;

        public ICommand CloseTabCommand { get; set; }

        public TabCloseCommandViewModel(object host)
        {
            _host = host;
            CloseTabCommand = new DelegateCommand(CloseTab, CanCloseTab);
        }

        public virtual void CloseTab(object param)
        {
            var tabItem = _host as TabItem;
            if (tabItem == null) return;
            var parent = tabItem.Parent;
            var tabcontrol = parent as TabControl;
            if (tabcontrol == null) return;
            if (MessageBox.Show("是否关闭标签？", "注意", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                tabcontrol.Items.Remove(tabItem);
            }
        }

        private bool CanCloseTab(object param)
        {
            return true;
        }

    }

    public class DelegateCommand : ICommand
    {
        private readonly Func<object, bool> _canExecute;
        private readonly Action<object> _executeAction;
        private bool _canExecuteCache;

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            _executeAction = executeAction;
            _canExecute = canExecute;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            var temp = _canExecute(parameter);

            if (_canExecuteCache != temp)
            {
                _canExecuteCache = temp;
                if (CanExecuteChanged != null)
                {
                    CanExecuteChanged(this, new EventArgs());
                }
            }

            return _canExecuteCache;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        #endregion
    }
}
