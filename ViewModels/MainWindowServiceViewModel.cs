using StartStopServices.Helpers;
using StartStopServices.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace StartStopServices.ViewModels
{
    public class MainWindowServiceViewModel : BaseViewModel
    {
        public Service SelectedItemService
        {
            get => _selectedItemService;
            set
            {
                if(_selectedItemService != value)
                    RaisePropertyChanged(()=> SelectedItemService);
            }
        }

        private Service _selectedItemService = null;

        readonly string machineName;

        private ObservableCollection<Service> _servicesCollection = null;

        public ObservableCollection<Service> ServicesCollection
        {
            get { return _servicesCollection; }
            set{
                if (_servicesCollection != value)
                {
                    _servicesCollection = value;
                    RaisePropertyChanged(() => ServicesCollection);
                }
            }
        }
        #region ctors
        public MainWindowServiceViewModel() {
            this.machineName = Environment.MachineName;
            this._servicesCollection = new ObservableCollection<Service>(ServiceHelper.GetAllServicesRunning(this.machineName));

            CollectionView view = (CollectionView)CollectionViewSource.GetDefaultView(this.ServicesCollection);
            view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Status", ListSortDirection.Ascending));
        }
        #endregion ctors


        #region Commands
        public ICommand CmdStartSelectedServices { get { return new DelegateCommand(OnStartSelectedServices); } }
        public ICommand CmdListAllServices { get { return new DelegateCommand(OnListAllServices); } }
        public ICommand CmdStopSelectedServices { get { return new DelegateCommand(OnStopServices); } }
        public ICommand CmdListAllStartedServices { get { return new DelegateCommand(OnListAllStartedServices); } }
        #endregion Commands


        #region Commands Handlers
        /// <summary>
        /// 
        /// </summary>
        private void OnStartSelectedServices()
        {
            ProcessAction(ServiceHelper.StartService, _servicesCollection);
            RaisePropertyChanged(() => ServicesCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnStopServices()
        {
            ProcessAction(ServiceHelper.StopService, _servicesCollection);
            RaisePropertyChanged(() => ServicesCollection);
        }
        /// <summary>
        /// 
        /// </summary>
        private void OnListAllStartedServices() 
        {
            _servicesCollection = new ObservableCollection<Service>(ServiceHelper.GetAllServicesRunning(this.machineName));
            RaisePropertyChanged(() => ServicesCollection);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnListAllServices()
        {
            _servicesCollection = new ObservableCollection<Service>(ServiceHelper.GetAllServices(this.machineName));
            RaisePropertyChanged(() => ServicesCollection);
        }
        #endregion Handlers

        #region private methods
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        private bool IsValidSelectedServiceName(string serviceName)
        {
            if (serviceName == null) return false;
            if (string.IsNullOrWhiteSpace(serviceName)) return false;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="services"></param>
        private void ProcessAction(Func<string, ServiceResult> action, ObservableCollection<Service> services)
        {
            if (services == null || !services.Any()) 
                return;
            foreach (var service in ServicesCollection.Where(s=>s.IsSelected && IsValidSelectedServiceName(s.Name)))
            {
                ServiceResult result = action(service.Name);
                if (result.Success)
                    service.Status = result.Status;
            }
            
        }
        #endregion

    }
}
