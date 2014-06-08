using ArcGIS.ServiceModel;
using ArcGIS.ServiceModel.Common;
using ArcGIS.ServiceModel.Operation;
using TestWeb.ServiceModel;
using ServiceStack;
using ServiceStack.FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace TestWeb.ServiceInterface
{
    public class AgoLoginValidator : AbstractValidator<ArcGISOnlineLogin>
    {
        public AgoLoginValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }

    public class CreateFeaturesValidator : AbstractValidator<EditFeatures>
    {
        public CreateFeaturesValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.Operation).NotEmpty();
            //RuleFor(x => x.NumberOfFeatures).NotEmpty().InclusiveBetween(1, 1000);
        }
    }
    
    public class RandomDataService : Service
    {
        static ISerializer _serializer = new ServiceStackSerializer();

        public async Task<HostedFeatureService[]> Post(ArcGISOnlineLogin request)
        {
            var agoGateway = new SearchArcGISOnlineGateway(_serializer,
                new ArcGISOnlineTokenProvider(request.Name, request.Password, _serializer));

             var token = await agoGateway.TokenProvider.CheckGenerateToken();
             var searchResult = await agoGateway.DescribeSite();

            foreach (var result in searchResult.Results)
            {
                var cacheItem = new HostedServiceCacheItem
                {
                    Id = result.Id,
                    Url = result.Url,
                    Token = token.Value
                };
                var cacheKey = UrnId.Create<HostedServiceCacheItem>(cacheItem.Id);
                Cache.Set<HostedServiceCacheItem>(cacheKey, cacheItem, token.Expiry.FromUnixTime());
            }

            return searchResult.Results;
        }

        public async Task<ServiceDetail> Get(HostedServiceRequest request)
        {
            var cacheKey = UrnId.Create<HostedServiceCacheItem>(request.Id);
            var hostedServiceCacheItem = Cache.Get<HostedServiceCacheItem>(cacheKey);

            HostedServiceDetail detail = hostedServiceCacheItem.Detail;

            if (detail == null)
            {
                var agoGateway = new SearchArcGISOnlineGateway(_serializer, null);
                detail = await agoGateway.ServiceDetail(String.Format("{0}/0?f=json&token={1}", hostedServiceCacheItem.Url.Trim('/'), hostedServiceCacheItem.Token));

                hostedServiceCacheItem.Detail = detail;
                Cache.Set<HostedServiceCacheItem>(cacheKey, hostedServiceCacheItem);
            }

            return new ServiceDetail
            {
                id = request.Id,
                name = detail.name,
                canCreate = detail.capabilities.ToLower().Contains("create"),
                fields = detail.fields.Where(f => f.editable == true && f.nullable == false).Select(f => f.alias).ToArray().Join(","),
                geometryType = detail.geometryType.Replace("esriGeometry", "")
            };
        }

        public async Task<object> Post(EditFeatures request)
        {
            if (request.Operation == "delete") return DeleteFeatures(request);

            var cacheKey = UrnId.Create<HostedServiceCacheItem>(request.Id);
            var hostedServiceCacheItem = Cache.Get<HostedServiceCacheItem>(cacheKey);

            if (hostedServiceCacheItem == null || hostedServiceCacheItem.Detail == null) return new HttpException(500, "Invalid cache");

            if (!String.Equals(hostedServiceCacheItem.Detail.geometryType, "esrigeometrypoint", StringComparison.OrdinalIgnoreCase)) 
                return new HttpException(500, "Only point features are currently supported");

            var randomNumber = new Random();
            var randomX = new Random();
            var randomY = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ 0123456789";
            var randomString = new Random();


            var features = new List<Feature<Point>>();
            for (var i = 0; i < request.NumberOfFeatures; i++)
            {
                var feature = new Feature<Point>
                {
                    Geometry = new Point
                    {
                        X = randomX.Next((int)hostedServiceCacheItem.Detail.extent.XMin, (int)hostedServiceCacheItem.Detail.extent.XMax),
                        Y = randomY.Next((int)hostedServiceCacheItem.Detail.extent.YMin, (int)hostedServiceCacheItem.Detail.extent.YMax),
                        SpatialReference = hostedServiceCacheItem.Detail.extent.SpatialReference
                    }
                };

                foreach (var field in hostedServiceCacheItem.Detail.fields.Where(f => f.editable == true && f.nullable == false))
                {
                    switch (field.type)
                    {
                        case "esriFieldTypeInteger":
                            feature.Attributes.Add(field.name, randomNumber.Next(1, 100));
                            break;

                        case "esriFieldTypeString":
                            feature.Attributes.Add(field.name, String.Format("'{0}'", new string(Enumerable.Repeat(chars, field.length -1).Select(s => s[randomString.Next(s.Length)]).ToArray())));
                            break;

                        case "esriFieldTypeDate":
                            feature.Attributes.Add(field.name, new DateTime().ToUnixTime());
                            break;
                    }
                }

                features.Add(feature);
            }

            // currently assuming that there is only one layer
            var adds = new ApplyEdits<Point>((hostedServiceCacheItem.Url.TrimEnd('/') + "/0").AsEndpoint())
            {
                Adds = features,
                Token = hostedServiceCacheItem.Token
            };
            var resultAdd = await new PortalGateway(hostedServiceCacheItem.Url, _serializer).ApplyEdits(adds);

            return new HttpResult(200);
        }

        async Task<object> DeleteFeatures(EditFeatures request)
        {
            var cacheKey = UrnId.Create<HostedServiceCacheItem>(request.Id);
            var hostedServiceCacheItem = Cache.Get<HostedServiceCacheItem>(cacheKey);

            if (hostedServiceCacheItem == null) return new HttpException(500, "Invalid cache");

            var gateway = new PortalGateway(hostedServiceCacheItem.Url, _serializer);

            var endpoint = (hostedServiceCacheItem.Url.TrimEnd('/') + "/0").AsEndpoint();
            var queryIds = new QueryForIds(endpoint) { Token = hostedServiceCacheItem.Token };
            var queryIdsResult = await gateway.QueryForIds(queryIds);

            if (queryIdsResult.ObjectIds != null && queryIdsResult.ObjectIds.Any())
            {
                var deletes = new ApplyEdits<Point>(endpoint)
                {
                    Deletes = queryIdsResult.ObjectIds.ToList(),
                    Token = hostedServiceCacheItem.Token
                };
                var resultDelete = await gateway.ApplyEdits(deletes);
            }
            return new HttpResult(200);
        }

    }
}