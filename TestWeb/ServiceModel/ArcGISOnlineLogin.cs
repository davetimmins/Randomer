using ArcGIS.ServiceModel.Operation;
using ServiceStack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TestWeb.ServiceModel
{
    [Route("/agologin", "POST")]
    public class ArcGISOnlineLogin : IReturn<Task<HostedFeatureService[]>>
    {
        public string Name { get; set; }
        public string Password { get; set; }
    }

    [Route("/hostedservice/{Id}", "GET")]
    public class HostedServiceRequest : IReturn<Task<ServiceDetail>>
    {
        public string Id { get; set; }
    }

    [Route("/hosted/services/features/edit", "POST")]
    public class EditFeatures
    {
        public String Id { get; set; }
        public String Operation { get; set; }
        public int NumberOfFeatures { get; set; }
    } 
}