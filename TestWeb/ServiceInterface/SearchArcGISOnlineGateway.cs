using ArcGIS.ServiceModel;
using ArcGIS.ServiceModel.Common;
using ArcGIS.ServiceModel.Operation;
using TestWeb.ServiceModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TestWeb.ServiceInterface
{
    public class SearchArcGISOnlineGateway : ArcGISOnlineGateway
    {
        public SearchArcGISOnlineGateway(ISerializer serializer, ArcGISOnlineTokenProvider tokenProvider)
            : base(serializer, tokenProvider)
        { }

        public Task<SearchHostedFeatureServicesResponse> Search(SearchHostedFeatureServices searchRequest)
        {
            return Get<SearchHostedFeatureServicesResponse, SearchHostedFeatureServices>(searchRequest);
        }

        public Task<HostedServiceDetail> ServiceDetail(String serviceUrl)
        {
            return Get<HostedServiceDetail>(serviceUrl);
        }
    }

    public class ArcGISGateway : PortalGateway
    {
        public ArcGISGateway(String root, ISerializer serializer)
            : base(root, serializer)
        { }

        public Task<ApplyEditsResponse> ApplyEdits<T>(ApplyEdits<T> edits) where T : IGeometry
        {
            return Post<ApplyEditsResponse, ApplyEdits<T>>(edits);
        }

        public Task<QueryForIdsResponse> QueryForIds(QueryForIds query)
        {
            return Get<QueryForIdsResponse, QueryForIds>(query);
        }
    }

    public class ServiceStackSerializer : ISerializer
    {
        public ServiceStackSerializer()
        {
            ServiceStack.Text.JsConfig.EmitCamelCaseNames = true;
            ServiceStack.Text.JsConfig.IncludeTypeInfo = false;
            ServiceStack.Text.JsConfig.ConvertObjectTypesIntoStringDictionary = true;
            ServiceStack.Text.JsConfig.IncludeNullValues = false;
        }

        public Dictionary<String, String> AsDictionary<T>(T objectToConvert) where T : CommonParameters
        {
            return ServiceStack.Text.TypeSerializer.ToStringDictionary<T>(objectToConvert);
        }

        public T AsPortalResponse<T>(String dataToConvert) where T : IPortalResponse
        {
            return ServiceStack.Text.JsonSerializer.DeserializeFromString<T>(dataToConvert);
        }
    }
}