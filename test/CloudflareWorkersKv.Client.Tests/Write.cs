﻿using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http.Testing;
using Xunit;

namespace CloudflareWorkersKv.Client.Tests
{
    public class Write : BaseTest
    {
        private readonly SampleResponse _sample;

        public Write()
        {
            _sample = new SampleResponse
            {
                Cost = 1.99m,
                Name = "test"
            };
        }

        [Fact]
        public async Task WhenWritingSuccessfully_RequestShouldBeConstructedCorrectly()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(SampleResponse, 200);

                await Client.Write(SampleKey, _sample);

                httpTest
                    .ShouldHaveCalled(ValidCloudflareWorkersKvUrl)
                    .WithVerb(HttpMethod.Put)
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("X-Auth-Email", Email)
                    .WithHeader("X-Auth-Key", AuthKey);
            }
        }

        [Fact]
        public async Task WhenAuthenticationErrorIsReturned_UnauthorizedExceptionIsThrown()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(CloudflareErrors.AuthenticationError, 403);

                await Assert.ThrowsAsync<UnauthorizedException>(async () => await Client.Write(SampleKey, _sample));
            }
        }

        [Fact]
        public async Task WhenNamespaceFormattingErrorIsReturned_NamespaceFormattingExceptionIsThrown()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith(CloudflareErrors.NamespaceFormattingError, 400);

                await Assert.ThrowsAsync<NamespaceFormattingException>(async () => await Client.Write(SampleKey, _sample));
            }
        }
    }
}
