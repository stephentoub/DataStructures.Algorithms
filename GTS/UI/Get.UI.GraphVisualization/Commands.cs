﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Get.UI
{
    private class LoadGraphCommand : ICommand
    {
        private readonly object _viewModel;
        //-----------------------------------------------------------------
        public LoadGraphCommand(object viewModel)
        {
            _viewModel = viewModel;
        }
        //-----------------------------------------------------------------
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        //-----------------------------------------------------------------
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        //-----------------------------------------------------------------
        public void Execute(object parameter)
        {
            //_viewModel.Start();
        }
        #endregion
    }
    private class SaveGraphCommand : ICommand
    {
        private readonly object _viewModel;
        //-----------------------------------------------------------------
        public SaveGraphCommand(object viewModel)
        {
            _viewModel = viewModel;
        }
        //-----------------------------------------------------------------
        #region ICommand Members
        public bool CanExecute(object parameter)
        {
            return true;
        }
        //-----------------------------------------------------------------
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        //-----------------------------------------------------------------
        public void Execute(object parameter)
        {
            //_viewModel.Start();
        }
        #endregion
    }
}