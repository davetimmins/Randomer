using ArcGIS.ServiceModel.Common;
using ArcGIS.ServiceModel.Operation;
using TestWeb.ServiceInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TestWeb.ServiceModel
{
    [DataContract]
    public class SearchHostedFeatureServices : ArcGISServerOperation
    {
        public SearchHostedFeatureServices(ArcGISOnlineEndpoint endpoint, String username)
            : base(endpoint, "search")
        {
            Query = String.Format("owner:{0} AND (type:\"Feature Service\")", username);
            SortField = "created";
            SortOrder = "desc";
            NumberToReturn = 100;
            StartIndex = 1;
        }

        [DataMember(Name = "q")]
        public String Query { get; private set; }

        [DataMember(Name = "sortField")]
        public String SortField { get; set; }

        [DataMember(Name = "sortOrder")]
        public String SortOrder { get; set; }

        [DataMember(Name = "num")]
        public int NumberToReturn { get; set; }

        [DataMember(Name = "start")]
        public int StartIndex { get; set; }
    }

    [DataContract]
    public class SearchHostedFeatureServicesResponse : PortalResponse
    {
        [DataMember(Name = "results")]
        public HostedFeatureService[] Results { get; set; }
    }

    [DataContract]
    public class HostedFeatureService
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}