using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using PlatformService;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace CommandService.SynDataServices.Grpc {
    public class PlatformDataClient : IPlatformDataClient {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper) {
            _configuration = configuration;
            _mapper = mapper;
        }
        public IEnumerable<Platform> ReturnAllPlatforms() {
            Console.WriteLine($"--> Calling Grpc Platform service: {_configuration["GrpcPlatform"]}");
            var httpHandler = new HttpClientHandler();
            // Return `true` to allow certificates that are untrusted/invalid
            httpHandler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;

            var channel = GrpcChannel.ForAddress(_configuration["GrpcPlatform"], new GrpcChannelOptions { HttpHandler = httpHandler });
            var clinet = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();
            try {
                var reply = clinet.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            } catch (Exception ex) {
                Console.WriteLine($"--> Could not call Grpc Platform server: {ex.Message}");
                return null;
            }
        }
    }
}
