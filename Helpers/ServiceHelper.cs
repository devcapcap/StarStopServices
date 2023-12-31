using StartStopServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace StartStopServices.Helpers
{
    public class ServiceResult
    {
        public ServiceResult() { }
        public bool Success { get; set; }
        public string Status { get; set; }
    } 
    public static class ServiceHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMachineName"></param>
        /// <returns></returns>
    	public static IEnumerable<Service> GetAllServices(string sMachineName)
		{
            if (!string.IsNullOrWhiteSpace(sMachineName))
			{
                IEnumerable<ServiceController> services = ServiceController.GetServices(sMachineName) ;
                IEnumerable<Service> listOfServices = (from p in services
                                             select new Service
                                             {
                                                 Index = Guid.NewGuid().ToString(),
                                                 Name = p.ServiceName,
                                                 Status = p.Status.ToString(),
                                             }).ToList();
				Parallel.ForEach(services, s => s.Dispose());
				return listOfServices;
        	}
			return null;
		}

		static readonly IReadOnlyList<ServiceControllerStatus> _runningServices = new List<ServiceControllerStatus> { 
			ServiceControllerStatus.Running,
		};

        static readonly IReadOnlyList<ServiceControllerStatus> _stoppedServices = new List<ServiceControllerStatus> {
            ServiceControllerStatus.StartPending,
            ServiceControllerStatus.PausePending,
            ServiceControllerStatus.ContinuePending,
            ServiceControllerStatus.Paused,
            ServiceControllerStatus.StopPending,
            ServiceControllerStatus.Stopped

        };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMachineName"></param>
        /// <returns></returns>
        public static IEnumerable<Service> GetAllServicesRunning(string sMachineName)
		{

            if (!string.IsNullOrWhiteSpace(sMachineName))
            {
				IEnumerable<ServiceController> services = ServiceController.GetServices(sMachineName);
                
                IEnumerable<Service> lstServices =
                    (from p in (
                                from s in services
                                where _runningServices.Contains( s.Status)
                                select s)
                     select new Service
                     {
                         Name = p.ServiceName,
                         Status = p.Status.ToString()
                     }).ToList();
				Parallel.ForEach(services, s => s.Dispose());
                return lstServices;
			}
			return null;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sMachineName"></param>
        /// <returns></returns>
        public static IEnumerable<Service> GetAllServices(string sMachineName, string fake = null)
        {

            if (!string.IsNullOrWhiteSpace(sMachineName))
            {
                IEnumerable<ServiceController> services = ServiceController.GetServices(sMachineName);

                IEnumerable<Service> lstServices =
                    (from p in (
                                from s in services
                                select s)
                     select new Service
                     {
                         Index = Guid.NewGuid().ToString(),
                         Name = p.ServiceName,
                         Status = p.Status.ToString()
                     }).ToList();
                Parallel.ForEach(services, s => s.Dispose());
                return lstServices;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sServiceName"></param>
        /// <returns></returns>
		public static bool IsServiceRuning(string sServiceName)
		{
            if (!string.IsNullOrWhiteSpace(sServiceName))
			{
				IEnumerable<ServiceController> services = ServiceController.GetServices();
                return services.Where(s => s.ServiceName.Equals(sServiceName,System.StringComparison.InvariantCultureIgnoreCase)).Any();
			}
            return false;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sServiceName"></param>
        /// <returns></returns>
		public static ServiceResult StartService(string sServiceName)
		{
            bool serviceStarted = false;
            string status = null;

			if ( !string.IsNullOrWhiteSpace(sServiceName))
			{
				IEnumerable<ServiceController> services = ServiceController.GetServices();
				ServiceController serviceController = (from service in services
				where service.ServiceName.Equals( sServiceName, System.StringComparison.InvariantCultureIgnoreCase)
				select service).FirstOrDefault();
				if (serviceController != null)
				{
					if ( _stoppedServices.Contains( serviceController.Status))
					{
						try
						{
							serviceController.Start();
							serviceController.Refresh();
                            serviceStarted = true;
                        }
						catch{
                            serviceStarted = false;
                        }
					}
                    status = serviceController.Status.ToString();
                }
			}
			return new ServiceResult { Status = status,Success = serviceStarted};
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sServiceName"></param>
        /// <returns></returns>
		public static ServiceResult StopService(string sServiceName)
		{
			bool serviceStopped = false;
            string status = null;

			if (!string.IsNullOrWhiteSpace(sServiceName))
			{
				IEnumerable<ServiceController> services = ServiceController.GetServices();
				ServiceController serviceController = (from service in services
				where service.ServiceName.Equals(sServiceName, System.StringComparison.InvariantCultureIgnoreCase)
				select service).FirstOrDefault();
			
                if (serviceController != null)
				{
					if ( _runningServices.Contains(serviceController.Status))
					{
						try
						{
							serviceController.Stop();
							serviceController.Refresh();
                            serviceStopped = true;
						}
						catch{
                            serviceStopped = false;
                        }
					}
                    status = serviceController.Status.ToString();
				}
                services.ToList().ForEach(s => s.Dispose());
			}
			return new ServiceResult { Success = serviceStopped,Status = status };
		}
    }
}
