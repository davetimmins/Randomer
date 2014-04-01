using ArcGIS.ServiceModel.Common;
using ArcGIS.ServiceModel.Operation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace TestWeb.ServiceModel
{
    internal class HostedServiceCacheItem
    {
        public String Id { get; set; }

        public String Token { get; set; }

        public String Url { get; set; }

        public HostedServiceDetail Detail { get; set; }
    }

    // Below was just generated from paste JSON as classes command
    
    public class ServiceDetail
    {
        public string id { get; set; }

        public string name { get; set; }

        public string geometryType { get; set; }

        public string fields { get; set; }

        public bool canCreate { get; set; }
    }

    [DataContract]
    public class HostedServiceDetail : PortalResponse
    {
        [DataMember]
        public double currentVersion { get; set; }
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string displayField { get; set; }
        [DataMember]
        public string description { get; set; }
        [DataMember]
        public string geometryType { get; set; }
        [DataMember]
        public Extent extent { get; set; }
        [DataMember]
        public Field[] fields { get; set; }
        [DataMember]
        public string capabilities { get; set; }
    }

    [DataContract]
    public class Field
    {
        [DataMember]
        public string name { get; set; }
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public string alias { get; set; }
        [DataMember]
        public bool nullable { get; set; }
        [DataMember]
        public bool editable { get; set; }
        [DataMember]
        public int length { get; set; }
    }
}