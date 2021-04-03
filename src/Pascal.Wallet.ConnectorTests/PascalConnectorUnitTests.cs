// © 2021 Contributors to the Pascal.Wallet.Connector
// This work is licensed under the terms of the MIT license.
// See the LICENSE file in the project root for more information.

using Moq;
using Moq.Protected;
using Pascal.Wallet.Connector.DTO;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Pascal.Wallet.Connector.Tests
{
    public class PascalConnectorUnitTests
    {
        [Fact]
        public async Task AddNodeAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":2,\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.AddNodeAsync("123.123.123.123:4004;7.7.7.7:4005");
            Assert.Null(response.Error);
            Assert.Equal(2, response.Result);
        }

        [Fact]
        public async Task GetAccountAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"account\":296,\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"balance\":1500,\"balance_s\":\"1, 500.0000\",\"n_operation\":2,\"updated_b\":8714,\"updated_b_active_mode\":8714,\"updated_b_passive_mode\":8069,\"state\":\"normal\",\"name\":\"test\",\"type\":0,\"data\":\"\",\"seal\":\"F58A811034633B916BF45AD3F3340090475650A9\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetAccountAsync(296);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(296, result.AccountNumber);
            Assert.Equal(1500, result.Balance);
            Assert.Null(result.LockedUntilBlock);
            Assert.Equal("test", result.Name);
            Assert.Null(result.NewPublicKey);
            Assert.Equal<uint>(2, result.NOperation);
            Assert.Null(result.Price);
            Assert.False(result.PrivateSale);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result.EncodedPublicKey);
            Assert.Null(result.SellerAccount);
            Assert.Equal(AccountState.Normal, result.State);
            Assert.Equal<uint>(0, result.Type);
            Assert.Equal("", result.Data);
            Assert.Equal<uint>(8714, result.LastUpdatedBlock);
            Assert.Equal<uint>(8714, result.UpdatedBlockActiveMode);
            Assert.Equal<uint>(8069, result.UpdatedBlockPassiveMode);
            Assert.Equal("F58A811034633B916BF45AD3F3340090475650A9", result.Seal);
        }

        //TODO: add tests for GetWalletAccounts(using encoded key and using b58pubkey). Currently PascalCoint 5.4 beta 5 wallet returns empty list when encoded public key or b58pubkey is provided.
        [Fact]
        public async Task GetWalletAccountsAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"account\":296,\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"balance\":1500,\"balance_s\":\"1, 500.0000\",\"n_operation\":2,\"updated_b\":8714,\"updated_b_active_mode\":8714,\"updated_b_passive_mode\":8069,\"state\":\"normal\",\"name\":\"test\",\"type\":0,\"data\":\"\",\"seal\":\"A22DE88E6DCA3264DA533B2BAF80AA93FEFCC25C\"},{\"account\":32320,\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"balance\":0,\"balance_s\":\"0.0000\",\"n_operation\":2,\"updated_b\":6729,\"updated_b_active_mode\":6729,\"updated_b_passive_mode\":6464,\"state\":\"listed\",\"locked_until_block\":0,\"price\":0.1,\"price_s\":\"0.1000\",\"seller_account\":296,\"private_sale\":false,\"new_enc_pubkey\":\"000000000000\",\"name\":\"\",\"type\":0,\"data\":\"\",\"seal\":\"9FFAC8955DBECF0B0D64BB483208D2F47B4A3426\"}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletAccountsAsync();
            Assert.Null(response.Error);

            var result = response.Result;
            Assert.Equal(2, result.Length);

            Assert.Equal<uint>(296, result[0].AccountNumber);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result[0].EncodedPublicKey);
            Assert.Equal(1500, result[0].Balance);
            Assert.Equal<uint>(2, result[0].NOperation);
            Assert.Equal<uint>(8714, result[0].LastUpdatedBlock);
            Assert.Equal<uint>(8714, result[0].UpdatedBlockActiveMode);
            Assert.Equal<uint>(8069, result[0].UpdatedBlockPassiveMode);
            Assert.Equal(AccountState.Normal, result[0].State);
            Assert.Equal("test", result[0].Name);
            Assert.Equal<uint>(0, result[0].Type);
            Assert.Equal("", result[0].Data);
            Assert.Equal("A22DE88E6DCA3264DA533B2BAF80AA93FEFCC25C", result[0].Seal);
            Assert.Null(result[0].LockedUntilBlock);
            Assert.Null(result[0].Price);
            Assert.False(result[0].PrivateSale);
            Assert.Null(result[0].SellerAccount);
            Assert.Null(result[0].NewPublicKey);

            Assert.Equal<uint>(32320, result[1].AccountNumber);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result[1].EncodedPublicKey);
            Assert.Equal(0, result[1].Balance);
            Assert.Equal<uint>(2, result[1].NOperation);
            Assert.Equal<uint>(6729, result[1].LastUpdatedBlock);
            Assert.Equal<uint>(6729, result[1].UpdatedBlockActiveMode);
            Assert.Equal<uint>(6464, result[1].UpdatedBlockPassiveMode);
            Assert.Equal(AccountState.Listed, result[1].State);
            Assert.Equal("", result[1].Name);
            Assert.Equal<uint>(0, result[1].Type);
            Assert.Equal("", result[1].Data);
            Assert.Equal("9FFAC8955DBECF0B0D64BB483208D2F47B4A3426", result[1].Seal);
            Assert.Equal<uint?>(0, result[1].LockedUntilBlock);
            Assert.Equal(0.1M, result[1].Price);
            Assert.False(result[1].PrivateSale);
            Assert.Equal<uint?>(296, result[1].SellerAccount);
            Assert.Equal("000000000000", result[1].NewPublicKey);
        }

        [Fact]
        public async Task GetWalletAccountsCountAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":3,\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletAccountsCountAsync();
            Assert.Null(response.Error);
            Assert.Equal<uint>(3, response.Result);
        }
        [Fact]
        public async Task GetWalletAccountsCountUsingEncodedKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":2,\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletAccountsCountAsync(encodedPublicKey: "CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB");
            Assert.Null(response.Error);
            Assert.Equal<uint>(2, response.Result);
        }
        [Fact]
        public async Task GetWalletAccountsCountUsingB58KeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":2,\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletAccountsCountAsync(b58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8");
            Assert.Null(response.Error);
            Assert.Equal<uint>(2, response.Result);
        }

        [Fact]
        public async Task GetWalletPublicKeysAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"name\":\"Miner\",\"can_use\":true,\"ec_nid\":714,\"x\":\"662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA\",\"y\":\"05CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"b58_pubkey\":\"3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8\"},{\"name\":\"Alpha\",\"can_use\":true,\"ec_nid\":716,\"x\":\"011FB91A8721A056D9032EBE067BD3A5E764E5BB2B78352A76D4BA7B87A67EBF2E8EB2DEF3D67588B8F101120803248DEF2882C1B0667CDA9BA6A696E569D0CC6376\",\"y\":\"D3A978D8BEF49EDA23F79DB2F2258EDE022D79E17ECF6490B8892C2C4FC8BCA45BEC3B2AD31FB6A8C10364F1A88E58236BED76F517006927AA769FF1D27D58E2E1\",\"enc_pubkey\":\"CC024200011FB91A8721A056D9032EBE067BD3A5E764E5BB2B78352A76D4BA7B87A67EBF2E8EB2DEF3D67588B8F101120803248DEF2882C1B0667CDA9BA6A696E569D0CC63764100D3A978D8BEF49EDA23F79DB2F2258EDE022D79E17ECF6490B8892C2C4FC8BCA45BEC3B2AD31FB6A8C10364F1A88E58236BED76F517006927AA769FF1D27D58E2E1\",\"b58_pubkey\":\"JJj2GZDdgUzFV7UAs27znTVpRdYon49xZwQcrxUjymDb7qzGFAHLjxCEcPLdyCpZXQjXuHF5izgfxhpqWB86pALNAcg5dtbSPwNSp6QJLdjkZJxHahoe5TbhcKZeJ6MsG3MWwowmWtRJZvND64cQUSbEBaENqcWcRznm823xkbyR2QrVSbQ8ypnRSXsakrDdv\"}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletPublicKeysAsync();
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal(2, result.Length);
            Assert.Equal("Miner", result[0].Name);
            Assert.True(result[0].CanUse);
            Assert.Equal(EncryptionType.Secp256k1, result[0].EncryptionNid);
            Assert.Equal("662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA", result[0].X);
            Assert.Equal("05CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result[0].Y);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result[0].EncodedPublicKey);
            Assert.Equal("3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8", result[0].B58PublicKey);

            Assert.Equal("Alpha", result[1].Name);
            Assert.True(result[1].CanUse);
            Assert.Equal(EncryptionType.secp521r1, result[1].EncryptionNid);
            Assert.Equal("011FB91A8721A056D9032EBE067BD3A5E764E5BB2B78352A76D4BA7B87A67EBF2E8EB2DEF3D67588B8F101120803248DEF2882C1B0667CDA9BA6A696E569D0CC6376", result[1].X);
            Assert.Equal("D3A978D8BEF49EDA23F79DB2F2258EDE022D79E17ECF6490B8892C2C4FC8BCA45BEC3B2AD31FB6A8C10364F1A88E58236BED76F517006927AA769FF1D27D58E2E1", result[1].Y);
            Assert.Equal("CC024200011FB91A8721A056D9032EBE067BD3A5E764E5BB2B78352A76D4BA7B87A67EBF2E8EB2DEF3D67588B8F101120803248DEF2882C1B0667CDA9BA6A696E569D0CC63764100D3A978D8BEF49EDA23F79DB2F2258EDE022D79E17ECF6490B8892C2C4FC8BCA45BEC3B2AD31FB6A8C10364F1A88E58236BED76F517006927AA769FF1D27D58E2E1", result[1].EncodedPublicKey);
            Assert.Equal("JJj2GZDdgUzFV7UAs27znTVpRdYon49xZwQcrxUjymDb7qzGFAHLjxCEcPLdyCpZXQjXuHF5izgfxhpqWB86pALNAcg5dtbSPwNSp6QJLdjkZJxHahoe5TbhcKZeJ6MsG3MWwowmWtRJZvND64cQUSbEBaENqcWcRznm823xkbyR2QrVSbQ8ypnRSXsakrDdv", result[1].B58PublicKey);
        }

        [Fact]
        public async Task GetPublicKeyUsingEncodedKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"ec_nid\":714,\"x\":\"662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA\",\"y\":\"05CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"b58_pubkey\":\"3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletPublicKeyAsync(encodedPublicKey: "CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal(EncryptionType.Secp256k1, result.EncryptionNid);
            Assert.Equal("662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA", result.X);
            Assert.Equal("05CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result.Y);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result.EncodedPublicKey);
            Assert.Equal("3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8", result.B58PublicKey);
        }
        [Fact]
        public async Task GetPublicKeyUsingB58KeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"ec_nid\":714,\"x\":\"662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA\",\"y\":\"05CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"b58_pubkey\":\"3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletPublicKeyAsync(b58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal(EncryptionType.Secp256k1, result.EncryptionNid);
            Assert.Equal("662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA", result.X);
            Assert.Equal("05CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result.Y);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result.EncodedPublicKey);
            Assert.Equal("3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8", result.B58PublicKey);
        }

        [Fact]
        public async Task GetWalletCoinsAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":2000,\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletCoinsAsync();
            Assert.Null(response.Error);
            Assert.Equal(2000, response.Result);
        }
        [Fact]
        public async Task GetWalletCoinsUsingEncodedKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":1500,\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletCoinsAsync(encodedPublicKey: "CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB");
            Assert.Null(response.Error);
            Assert.Equal(1500, response.Result);
        }
        [Fact]
        public async Task GetWalletCoinsUsingB58KeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":1500,\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetWalletCoinsAsync(b58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8");
            Assert.Null(response.Error);
            Assert.Equal(1500, response.Result);
        }

        [Fact]
        public async Task GetBlockAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":9781,\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"reward\":100.0000,\"reward_s\":\"100.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"ver\":5,\"ver_a\":5,\"timestamp\":1615490160,\"target\":276415760,\"nonce\":3868460252,\"payload\":\"New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers\",\"sbh\":\"185931E75C7C50F61568A58F59E049250FDF34C0F2C6FCAA1C9E841F7AAE5C39\",\"oph\":\"FD8A3728180E01CF5EDCBB29448BE70BFFBF27BF80A0167A504A57735F0359F6\",\"pow\":\"000076E31CE47B13B47BCA7E59AD437E029EED07A2C9F0B9D18B07CE48268569\",\"hashratekhs\":4,\"maturation\":66,\"operations\":1},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetBlockAsync(9781);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(9781, result.BlockNumber);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result.EncodedPublicKey);
            Assert.Equal(100, result.Reward);
            Assert.Equal(0, result.Fee);
            Assert.Equal<uint>(5, result.Version);
            Assert.Equal<uint>(5, result.MinerVersion);
            Assert.Equal(new DateTime(2021, 3, 11, 19, 16, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal<uint>(276415760, result.Target);
            Assert.Equal(3868460252, result.Nonce);
            Assert.Equal("New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers", result.Payload);
            Assert.Equal("185931E75C7C50F61568A58F59E049250FDF34C0F2C6FCAA1C9E841F7AAE5C39", result.SafeboxHash);
            Assert.Equal("FD8A3728180E01CF5EDCBB29448BE70BFFBF27BF80A0167A504A57735F0359F6", result.OperationsHash);
            Assert.Equal("000076E31CE47B13B47BCA7E59AD437E029EED07A2C9F0B9D18B07CE48268569", result.ProofOfWork);
            Assert.Equal<uint>(4, result.HashRateKhs);
            Assert.Equal<uint>(66, result.Maturation);
            Assert.Equal<uint>(1, result.Operations);
        }

        [Fact]
        public async Task GetBlocksLastAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"block\":9851,\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"reward\":100,\"reward_s\":\"100.0000\",\"fee\":0,\"fee_s\":\"0.0000\",\"ver\":5,\"ver_a\":5,\"timestamp\":1615497166,\"target\":285898593,\"nonce\":89352119,\"payload\":\"New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers\",\"sbh\":\"A2930533A1C42FA7DE6F02ABF572BC5B9421C897737F5C154ABC228FFE77EDE6\",\"oph\":\"48BD9442A1BEF1969C5F670C6B3FF8C01A7D724700B8BE2627BFF250123ACD2F\",\"pow\":\"0000244D2CDA375F6BEFDF75AB97821B7F9EB1388CFA61A8E0FA87E19D4B6E4A\",\"hashratekhs\":0,\"maturation\":0,\"operations\":2},{\"block\":9850,\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"reward\":100,\"reward_s\":\"100.0000\",\"fee\":0,\"fee_s\":\"0.0000\",\"ver\":5,\"ver_a\":5,\"timestamp\":1615497122,\"target\":286543073,\"nonce\":1070113875,\"payload\":\"New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers\",\"sbh\":\"A68B77BFFBF6E45CCCE9420C9CC193F1C74408607989355E3F756876221C5BAC\",\"oph\":\"E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855\",\"pow\":\"0000782DED7BD7FC98969E26062F6FD3F0C47225E08F0E94F5E304F190060664\",\"hashratekhs\":0,\"maturation\":1,\"operations\":0}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetBlocksAsync(2);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(9851, result[0].BlockNumber);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result[0].EncodedPublicKey);
            Assert.Equal(100, result[0].Reward);
            Assert.Equal(0, result[0].Fee);
            Assert.Equal<uint>(5, result[0].Version);
            Assert.Equal<uint>(5, result[0].MinerVersion);
            Assert.Equal(new DateTime(2021, 3, 11, 21, 12, 46, DateTimeKind.Utc), result[0].Time);
            Assert.Equal<uint>(285898593, result[0].Target);
            Assert.Equal<ulong>(89352119, result[0].Nonce);
            Assert.Equal("New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers", result[0].Payload);
            Assert.Equal("A2930533A1C42FA7DE6F02ABF572BC5B9421C897737F5C154ABC228FFE77EDE6", result[0].SafeboxHash);
            Assert.Equal("48BD9442A1BEF1969C5F670C6B3FF8C01A7D724700B8BE2627BFF250123ACD2F", result[0].OperationsHash);
            Assert.Equal("0000244D2CDA375F6BEFDF75AB97821B7F9EB1388CFA61A8E0FA87E19D4B6E4A", result[0].ProofOfWork);
            Assert.Equal<uint>(0, result[0].HashRateKhs);
            Assert.Equal<uint>(0, result[0].Maturation);
            Assert.Equal<uint>(2, result[0].Operations);

            Assert.Equal<uint>(9850, result[1].BlockNumber);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result[1].EncodedPublicKey);
            Assert.Equal(100, result[1].Reward);
            Assert.Equal(0, result[1].Fee);
            Assert.Equal<uint>(5, result[1].Version);
            Assert.Equal<uint>(5, result[1].MinerVersion);
            Assert.Equal(new DateTime(2021, 3, 11, 21, 12, 02, DateTimeKind.Utc), result[1].Time);
            Assert.Equal<uint>(286543073, result[1].Target);
            Assert.Equal<ulong>(1070113875, result[1].Nonce);
            Assert.Equal("New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers", result[1].Payload);
            Assert.Equal("A68B77BFFBF6E45CCCE9420C9CC193F1C74408607989355E3F756876221C5BAC", result[1].SafeboxHash);
            Assert.Equal("E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855", result[1].OperationsHash);
            Assert.Equal("0000782DED7BD7FC98969E26062F6FD3F0C47225E08F0E94F5E304F190060664", result[1].ProofOfWork);
            Assert.Equal<uint>(0, result[1].HashRateKhs);
            Assert.Equal<uint>(1, result[1].Maturation);
            Assert.Equal<uint>(0, result[1].Operations);
        }

        [Fact]
        public async Task GetBlocksIntervalAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"block\":9851,\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"reward\":100,\"reward_s\":\"100.0000\",\"fee\":0,\"fee_s\":\"0.0000\",\"ver\":5,\"ver_a\":5,\"timestamp\":1615497166,\"target\":285898593,\"nonce\":89352119,\"payload\":\"New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers\",\"sbh\":\"A2930533A1C42FA7DE6F02ABF572BC5B9421C897737F5C154ABC228FFE77EDE6\",\"oph\":\"48BD9442A1BEF1969C5F670C6B3FF8C01A7D724700B8BE2627BFF250123ACD2F\",\"pow\":\"0000244D2CDA375F6BEFDF75AB97821B7F9EB1388CFA61A8E0FA87E19D4B6E4A\",\"hashratekhs\":0,\"maturation\":0,\"operations\":2},{\"block\":9850,\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"reward\":100,\"reward_s\":\"100.0000\",\"fee\":0,\"fee_s\":\"0.0000\",\"ver\":5,\"ver_a\":5,\"timestamp\":1615497122,\"target\":286543073,\"nonce\":1070113875,\"payload\":\"New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers\",\"sbh\":\"A68B77BFFBF6E45CCCE9420C9CC193F1C74408607989355E3F756876221C5BAC\",\"oph\":\"E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855\",\"pow\":\"0000782DED7BD7FC98969E26062F6FD3F0C47225E08F0E94F5E304F190060664\",\"hashratekhs\":0,\"maturation\":1,\"operations\":0}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetBlocksAsync(9850, 9851);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(9851, result[0].BlockNumber);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result[0].EncodedPublicKey);
            Assert.Equal(100, result[0].Reward);
            Assert.Equal(0, result[0].Fee);
            Assert.Equal<uint>(5, result[0].Version);
            Assert.Equal<uint>(5, result[0].MinerVersion);
            Assert.Equal(new DateTime(2021, 3, 11, 21, 12, 46, DateTimeKind.Utc), result[0].Time);
            Assert.Equal<uint>(285898593, result[0].Target);
            Assert.Equal<ulong>(89352119, result[0].Nonce);
            Assert.Equal("New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers", result[0].Payload);
            Assert.Equal("A2930533A1C42FA7DE6F02ABF572BC5B9421C897737F5C154ABC228FFE77EDE6", result[0].SafeboxHash);
            Assert.Equal("48BD9442A1BEF1969C5F670C6B3FF8C01A7D724700B8BE2627BFF250123ACD2F", result[0].OperationsHash);
            Assert.Equal("0000244D2CDA375F6BEFDF75AB97821B7F9EB1388CFA61A8E0FA87E19D4B6E4A", result[0].ProofOfWork);
            Assert.Equal<uint>(0, result[0].HashRateKhs);
            Assert.Equal<uint>(0, result[0].Maturation);
            Assert.Equal<uint>(2, result[0].Operations);

            Assert.Equal<uint>(9850, result[1].BlockNumber);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result[1].EncodedPublicKey);
            Assert.Equal(100, result[1].Reward);
            Assert.Equal(0, result[1].Fee);
            Assert.Equal<uint>(5, result[1].Version);
            Assert.Equal<uint>(5, result[1].MinerVersion);
            Assert.Equal(new DateTime(2021, 3, 11, 21, 12, 02, DateTimeKind.Utc), result[1].Time);
            Assert.Equal<uint>(286543073, result[1].Target);
            Assert.Equal<ulong>(1070113875, result[1].Nonce);
            Assert.Equal("New Node 10.03.2021 09:25:48 - TESTNET 5.4.Beta5Rabarbers", result[1].Payload);
            Assert.Equal("A68B77BFFBF6E45CCCE9420C9CC193F1C74408607989355E3F756876221C5BAC", result[1].SafeboxHash);
            Assert.Equal("E3B0C44298FC1C149AFBF4C8996FB92427AE41E4649B934CA495991B7852B855", result[1].OperationsHash);
            Assert.Equal("0000782DED7BD7FC98969E26062F6FD3F0C47225E08F0E94F5E304F190060664", result[1].ProofOfWork);
            Assert.Equal<uint>(0, result[1].HashRateKhs);
            Assert.Equal<uint>(1, result[1].Maturation);
            Assert.Equal<uint>(0, result[1].Operations);
        }

        [Fact]
        public async Task GetBlockCountAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":9922,\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetBlockCountAsync();
            Assert.Null(response.Error);
            Assert.Equal<uint>(9922, response.Result);
        }

        [Fact]
        public async Task GetBlockOperationNoPayloadAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":9781,\"time\":1615490160,\"opblock\":0,\"maturation\":858,\"optype\":1,\"subtype\":11,\"account\":35584,\"signer_account\":35584,\"n_operation\":1,\"senders\":[{\"account\":35584,\"n_operation\":1,\"amount\":-20.0000,\"amount_s\":\" - 20.0000\",\"payload\":\"\",\"payload_type\":0}],\"receivers\":[{\"account\":52,\"amount\":20.0000,\"amount_s\":\"20.0000\",\"payload\":\"\",\"payload_type\":0}],\"changers\":[],\"optxt\":\"Tx - Out 20.0000 PASC from 35584 - 85 to 52 - 11\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":-20.0000,\"amount_s\":\" - 20.0000\",\"payload\":\"\",\"payload_type\":0,\"sender_account\":35584,\"dest_account\":52,\"ophash\":\"35260000008B0000010000009D2EE42FCB0DFDC628C25293B6E20CE1DD551CCB\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetBlockOperationAsync(9781, 0);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(9781, result.BlockNumber);
            Assert.Equal(new DateTime(2021, 3, 11, 19, 16, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(0, result.Index);
            Assert.Equal<uint?>(858, result.Maturation);
            Assert.Equal(OperationType.Transaction, result.Type);
            Assert.Equal(OperationSubType.TransactionSender, result.SubType);
            Assert.Equal<uint>(35584, result.AccountNumber);
            Assert.Equal<uint>(35584, result.SignerAccountNumber);
            Assert.Equal<uint>(1, result.NOperation);
            Assert.Equal<uint>(35584, result.Senders[0].AccountNumber);
            Assert.Equal(-20.0000M, result.Senders[0].Amount);
            Assert.Equal<uint>(1, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.NonDeterministic, result.Senders[0].PayloadType);
            Assert.Equal("", result.Senders[0].Payload);
            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(20.0000M, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.NonDeterministic, result.Receivers[0].PayloadType);
            Assert.Equal("", result.Receivers[0].Payload);
            Assert.Empty(result.Changers);
            Assert.Equal("Tx - Out 20.0000 PASC from 35584 - 85 to 52 - 11", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(-20.0000M, result.Amount);
            Assert.Equal("", result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("35260000008B0000010000009D2EE42FCB0DFDC628C25293B6E20CE1DD551CCB", result.OpHash);
        }
        [Fact]
        public async Task GetBlockOperationWithPayloadAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":10947,\"time\":1615558904,\"opblock\":0,\"maturation\":2,\"optype\":1,\"subtype\":11,\"account\":50045,\"signer_account\":50045,\"n_operation\":1,\"senders\":[{\"account\":50045,\"account_epasa\":\"50045-67[\\\"test\\\"]\", \"unenc_payload\":\"test\", \"unenc_hexpayload\":\"74657374\", \"n_operation\":1, \"amount\":-40, \"amount_s\":\"-40.0000\", \"payload\":\"74657374\", \"payload_type\":17}],\"receivers\":[{ \"account\":52,\"account_epasa\":\"52-11[\\\"test\\\"]\",\"unenc_payload\":\"test\",\"unenc_hexpayload\":\"74657374\",\"amount\":40,\"amount_s\":\"40.0000\",\"payload\":\"74657374\",\"payload_type\":17}],\"changers\":[],\"optxt\":\"Tx-Out 40.0000 PASC from 50045-67 to 52-11\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":-40,\"amount_s\":\"-40.0000\",\"payload\":\"74657374\",\"payload_type\":17,\"sender_account\":50045,\"dest_account\":52,\"ophash\":\"C32A00007DC3000001000000C85E3C6E392106FDF3C7B5EB2808320AC5888CDD\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetBlockOperationAsync(10947, 0);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(10947, result.BlockNumber);
            Assert.Equal(new DateTime(2021, 3, 12, 14, 21, 44, DateTimeKind.Utc), result.Time);
            Assert.Equal(0, result.Index);
            Assert.Equal<uint?>(2, result.Maturation);
            Assert.Equal(OperationType.Transaction, result.Type);
            Assert.Equal(OperationSubType.TransactionSender, result.SubType);
            Assert.Equal<uint>(50045, result.AccountNumber);
            Assert.Equal<uint>(50045, result.SignerAccountNumber);
            Assert.Equal<uint>(1, result.NOperation);

            Assert.Equal<uint>(50045, result.Senders[0].AccountNumber);
            Assert.Equal(-40M, result.Senders[0].Amount);
            Assert.Equal<uint>(1, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.Public | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("74657374", result.Senders[0].Payload);
            Assert.Equal("50045-67[\"test\"]", result.Senders[0].AccountEpasa);
            Assert.Equal("test", result.Senders[0].UnencryptedPayload);
            Assert.Equal("74657374", result.Senders[0].UnencryptedPayloadHexastring);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(40M, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.Public | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("74657374", result.Receivers[0].Payload);
            Assert.Equal("52-11[\"test\"]", result.Receivers[0].AccountEpasa);
            Assert.Equal("test", result.Receivers[0].UnencryptedPayload);
            Assert.Equal("74657374", result.Receivers[0].UnencryptedPayloadHexastring);

            Assert.Empty(result.Changers);

            Assert.Equal("Tx-Out 40.0000 PASC from 50045-67 to 52-11", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(-40M, result.Amount);
            Assert.Equal("74657374", result.Payload);
            Assert.Equal(PayloadType.Public | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("C32A00007DC3000001000000C85E3C6E392106FDF3C7B5EB2808320AC5888CDD", result.OpHash);
        }
        [Fact]
        public async Task GetBlockOperationWithPayloadAndPasswordAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":12239,\"time\":1615622049,\"opblock\":0,\"maturation\":122,\"optype\":1,\"subtype\":11,\"account\":43060,\"signer_account\":43060,\"n_operation\":1,\"senders\":[{\"account\":43060,\"n_operation\":1,\"amount\":-80,\"amount_s\":\" - 80.0000\",\"payload\":\"53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401\",\"payload_type\":24}],\"receivers\":[{\"account\":52,\"amount\":80,\"amount_s\":\"80.0000\",\"payload\":\"53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401\",\"payload_type\":24}],\"changers\":[],\"optxt\":\"Tx - Out 80.0000 PASC from 43060 - 85 to 52 - 11\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":-80,\"amount_s\":\" - 80.0000\",\"payload\":\"53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401\",\"payload_type\":24,\"sender_account\":43060,\"dest_account\":52,\"ophash\":\"CF2F000034A800000100000029438184ED11E4F5AAFFADFF12AA3D03B59B18B1\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetBlockOperationAsync(12239, 0);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(12239, result.BlockNumber);
            Assert.Equal(new DateTime(2021, 3, 13, 7, 54, 9, DateTimeKind.Utc), result.Time);
            Assert.Equal(0, result.Index);
            Assert.Equal<uint?>(122, result.Maturation);
            Assert.Equal(OperationType.Transaction, result.Type);
            Assert.Equal(OperationSubType.TransactionSender, result.SubType);
            Assert.Equal<uint>(43060, result.AccountNumber);
            Assert.Equal<uint>(43060, result.SignerAccountNumber);
            Assert.Equal<uint>(1, result.NOperation);

            Assert.Equal<uint>(43060, result.Senders[0].AccountNumber);
            Assert.Equal(-80M, result.Senders[0].Amount);
            Assert.Equal<uint>(1, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401", result.Senders[0].Payload);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(80M, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("Tx - Out 80.0000 PASC from 43060 - 85 to 52 - 11", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(-80M, result.Amount);
            Assert.Equal("53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401", result.Payload);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("CF2F000034A800000100000029438184ED11E4F5AAFFADFF12AA3D03B59B18B1", result.OpHash);
        }

        [Fact]
        public async Task GetBlockOperationsAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"block\":12239,\"time\":1615622049,\"opblock\":0,\"maturation\":122,\"optype\":1,\"subtype\":11,\"account\":43060,\"signer_account\":43060,\"n_operation\":1,\"senders\":[{\"account\":43060,\"n_operation\":1,\"amount\":-80,\"amount_s\":\" - 80.0000\",\"payload\":\"53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401\",\"payload_type\":24}],\"receivers\":[{\"account\":52,\"amount\":80,\"amount_s\":\"80.0000\",\"payload\":\"53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401\",\"payload_type\":24}],\"changers\":[],\"optxt\":\"Tx - Out 80.0000 PASC from 43060 - 85 to 52 - 11\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":-80,\"amount_s\":\" - 80.0000\",\"payload\":\"53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401\",\"payload_type\":24,\"sender_account\":43060,\"dest_account\":52,\"ophash\":\"CF2F000034A800000100000029438184ED11E4F5AAFFADFF12AA3D03B59B18B1\"}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetBlockOperationsAsync(12239);
            Assert.Null(response.Error);

            var result = response.Result;
            Assert.Single(result);

            var operation = result[0];
            Assert.Equal<uint?>(12239, operation.BlockNumber);
            Assert.Equal(new DateTime(2021, 3, 13, 7, 54, 9, DateTimeKind.Utc), operation.Time);
            Assert.Equal(0, operation.Index);
            Assert.Equal<uint?>(122, operation.Maturation);
            Assert.Equal(OperationType.Transaction, operation.Type);
            Assert.Equal(OperationSubType.TransactionSender, operation.SubType);
            Assert.Equal<uint>(43060, operation.AccountNumber);
            Assert.Equal<uint>(43060, operation.SignerAccountNumber);
            Assert.Equal<uint>(1, operation.NOperation);

            Assert.Equal<uint>(43060, operation.Senders[0].AccountNumber);
            Assert.Equal(-80M, operation.Senders[0].Amount);
            Assert.Equal<uint>(1, operation.Senders[0].NOperation);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, operation.Senders[0].PayloadType);
            Assert.Equal("53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401", operation.Senders[0].Payload);

            Assert.Equal<uint>(52, operation.Receivers[0].AccountNumber);
            Assert.Equal(80M, operation.Receivers[0].Amount);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, operation.Receivers[0].PayloadType);
            Assert.Equal("53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401", operation.Receivers[0].Payload);

            Assert.Empty(operation.Changers);

            Assert.Equal("Tx - Out 80.0000 PASC from 43060 - 85 to 52 - 11", operation.Description);
            Assert.Equal(0, operation.Fee);
            Assert.Equal(-80M, operation.Amount);
            Assert.Equal("53616C7465645F5FEEE54CB6C35734B3F26279DC33FC4684FB68C8BE1203F401", operation.Payload);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, operation.PayloadType);
            Assert.Equal("CF2F000034A800000100000029438184ED11E4F5AAFFADFF12AA3D03B59B18B1", operation.OpHash);
        }

        [Fact]
        public async Task GetPendingsAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"block\":0,\"time\":0,\"opblock\":0,\"maturation\":null,\"optype\":1,\"subtype\":11,\"account\":62815,\"signer_account\":62815,\"n_operation\":1,\"senders\":[{\"account\":62815,\"n_operation\":1,\"amount\":-40,\"amount_s\":\" - 40.0000\",\"payload\":\"\",\"payload_type\":0}],\"receivers\":[{\"account\":52,\"amount\":40,\"amount_s\":\"40.0000\",\"payload\":\"\",\"payload_type\":0}],\"changers\":[],\"optxt\":\"Tx - Out 40.0000 PASC from 62815 - 49 to 52 - 11\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":-40,\"amount_s\":\" - 40.0000\",\"payload\":\"\",\"payload_type\":0,\"sender_account\":62815,\"dest_account\":52,\"ophash\":\"000000005FF50000010000006A0C469FA8B8EF184C407F41F6DCF632D230E1A8\",\"old_ophash\":\"\"},{\"block\":0,\"time\":0,\"opblock\":1,\"maturation\":null,\"optype\":1,\"subtype\":11,\"account\":62819,\"signer_account\":62819,\"n_operation\":1,\"senders\":[{\"account\":62819,\"n_operation\":1,\"amount\":-10,\"amount_s\":\" - 10.0000\",\"payload\":\"53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31\",\"payload_type\":24}],\"receivers\":[{\"account\":52,\"amount\":10,\"amount_s\":\"10.0000\",\"payload\":\"53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31\",\"payload_type\":24}],\"changers\":[],\"optxt\":\"Tx - Out 10.0000 PASC from 62819 - 97 to 52 - 11\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":-10,\"amount_s\":\" - 10.0000\",\"payload\":\"53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31\",\"payload_type\":24,\"sender_account\":62819,\"dest_account\":52,\"ophash\":\"0000000063F5000001000000FCFA8E6D82F890C56248B2CF7FAB14FE947B16A5\",\"old_ophash\":\"\"}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetPendingsAsync();
            Assert.Null(response.Error);

            var result = response.Result;
            Assert.Equal(2, result.Length);

            var operation1 = result[0];
            Assert.Equal<uint?>(0, operation1.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), operation1.Time);
            Assert.Equal(0, operation1.Index);
            Assert.Null(operation1.Maturation);
            Assert.Equal(OperationType.Transaction, operation1.Type);
            Assert.Equal(OperationSubType.TransactionSender, operation1.SubType);
            Assert.Equal<uint>(62815, operation1.AccountNumber);
            Assert.Equal<uint>(62815, operation1.SignerAccountNumber);
            Assert.Equal<uint>(1, operation1.NOperation);

            Assert.Equal<uint>(62815, operation1.Senders[0].AccountNumber);
            Assert.Equal(-40M, operation1.Senders[0].Amount);
            Assert.Equal<uint>(1, operation1.Senders[0].NOperation);
            Assert.Equal(PayloadType.NonDeterministic, operation1.Senders[0].PayloadType);
            Assert.Equal("", operation1.Senders[0].Payload);

            Assert.Equal<uint>(52, operation1.Receivers[0].AccountNumber);
            Assert.Equal(40M, operation1.Receivers[0].Amount);
            Assert.Equal(PayloadType.NonDeterministic, operation1.Receivers[0].PayloadType);
            Assert.Equal("", operation1.Receivers[0].Payload);

            Assert.Empty(operation1.Changers);

            Assert.Equal("Tx - Out 40.0000 PASC from 62815 - 49 to 52 - 11", operation1.Description);
            Assert.Equal(0, operation1.Fee);
            Assert.Equal(-40M, operation1.Amount);
            Assert.Equal("", operation1.Payload);
            Assert.Equal(PayloadType.NonDeterministic, operation1.PayloadType);
            Assert.Equal("000000005FF50000010000006A0C469FA8B8EF184C407F41F6DCF632D230E1A8", operation1.OpHash);

            var operation2 = result[1];
            Assert.Equal<uint?>(0, operation2.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), operation2.Time);
            Assert.Equal(1, operation2.Index);
            Assert.Null(operation2.Maturation);
            Assert.Equal(OperationType.Transaction, operation2.Type);
            Assert.Equal(OperationSubType.TransactionSender, operation2.SubType);
            Assert.Equal<uint>(62819, operation2.AccountNumber);
            Assert.Equal<uint>(62819, operation2.SignerAccountNumber);
            Assert.Equal<uint>(1, operation2.NOperation);

            Assert.Equal<uint>(62819, operation2.Senders[0].AccountNumber);
            Assert.Equal(-10M, operation2.Senders[0].Amount);
            Assert.Equal<uint>(1, operation2.Senders[0].NOperation);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, operation2.Senders[0].PayloadType);
            Assert.Equal("53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31", operation2.Senders[0].Payload);

            Assert.Equal<uint>(52, operation2.Receivers[0].AccountNumber);
            Assert.Equal(10M, operation2.Receivers[0].Amount);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, operation2.Receivers[0].PayloadType);
            Assert.Equal("53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31", operation2.Receivers[0].Payload);

            Assert.Empty(operation2.Changers);

            Assert.Equal("Tx - Out 10.0000 PASC from 62819 - 97 to 52 - 11", operation2.Description);
            Assert.Equal(0, operation2.Fee);
            Assert.Equal(-10M, operation2.Amount);
            Assert.Equal("53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31", operation2.Payload);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, operation2.PayloadType);
            Assert.Equal("0000000063F5000001000000FCFA8E6D82F890C56248B2CF7FAB14FE947B16A5", operation2.OpHash);
        }
        [Fact]
        public async Task GetPendingsUsingParametersAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"block\":0,\"time\":0,\"opblock\":1,\"maturation\":null,\"optype\":1,\"subtype\":11,\"account\":62819,\"signer_account\":62819,\"n_operation\":1,\"senders\":[{\"account\":62819,\"n_operation\":1,\"amount\":-10,\"amount_s\":\"-10.0000\",\"payload\":\"53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31\",\"payload_type\":24}],\"receivers\":[{\"account\":52,\"amount\":10,\"amount_s\":\"10.0000\",\"payload\":\"53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31\",\"payload_type\":24}],\"changers\":[],\"optxt\":\"Tx-Out 10.0000 PASC from 62819-97 to 52-11\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":-10,\"amount_s\":\"-10.0000\",\"payload\":\"53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31\",\"payload_type\":24,\"sender_account\":62819,\"dest_account\":52,\"ophash\":\"0000000063F5000001000000FCFA8E6D82F890C56248B2CF7FAB14FE947B16A5\",\"old_ophash\":\"\"}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetPendingsAsync(start: 1, max: 1);
            Assert.Null(response.Error);
            Assert.Single(response.Result);
        }

        [Fact]
        public async Task GetPendingsCountAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":2,\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetPendingsCountAsync();
            Assert.Null(response.Error);
            Assert.Equal<uint>(2, response.Result);
        }

        [Fact]
        public async Task FindOperationAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":1615644914,\"opblock\":1,\"maturation\":null,\"optype\":1,\"subtype\":11,\"account\":62819,\"signer_account\":62819,\"n_operation\":1,\"senders\":[{\"account\":62819,\"n_operation\":1,\"amount\":-10,\"amount_s\":\"-10.0000\",\"payload\":\"53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31\",\"payload_type\":24}],\"receivers\":[{\"account\":52,\"amount\":10,\"amount_s\":\"10.0000\",\"payload\":\"53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31\",\"payload_type\":24}],\"changers\":[],\"optxt\":\"Tx-Out 10.0000 PASC from 62819-97 to 52-11\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":-10,\"amount_s\":\"-10.0000\",\"payload\":\"53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31\",\"payload_type\":24,\"sender_account\":62819,\"dest_account\":52,\"ophash\":\"0000000063F5000001000000FCFA8E6D82F890C56248B2CF7FAB14FE947B16A5\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.FindOperationAsync("0000000063F5000001000000FCFA8E6D82F890C56248B2CF7FAB14FE947B16A5");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(2021, 3, 13, 14, 15, 14, DateTimeKind.Utc), result.Time);
            Assert.Equal(1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.Transaction, result.Type);
            Assert.Equal(OperationSubType.TransactionSender, result.SubType);
            Assert.Equal<uint>(62819, result.AccountNumber);
            Assert.Equal<uint>(62819, result.SignerAccountNumber);
            Assert.Equal<uint>(1, result.NOperation);

            Assert.Equal<uint>(62819, result.Senders[0].AccountNumber);
            Assert.Equal(-10M, result.Senders[0].Amount);
            Assert.Equal<uint>(1, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31", result.Senders[0].Payload);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(10M, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("Tx-Out 10.0000 PASC from 62819-97 to 52-11", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(-10M, result.Amount);
            Assert.Equal("53616C7465645F5F3E1C8F58FF35EC018DEBF2553E4C704ABF323BD12FD31E31", result.Payload);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("0000000063F5000001000000FCFA8E6D82F890C56248B2CF7FAB14FE947B16A5", result.OpHash);
        }

        [Fact]
        public async Task FindAccountsAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"account\":0,\"enc_pubkey\":\"CA0220006E75266E1865288874BFE1C3A9686BCF4D22CADCC250B134EA2C470A9E3F912B2000F8362CB5EE97113D93880303E99B63208CEBDB62D49874D739E33DFFFD3C4162\",\"balance\":1167.2667,\"balance_s\":\"1,167.2667\",\"n_operation\":16,\"updated_b\":509366,\"updated_b_active_mode\":504177,\"updated_b_passive_mode\":509366,\"state\":\"normal\",\"name\":\"pascalcoin-dev\",\"type\":1000,\"data\":\"\",\"seal\":\"35698E985A010E711D3001A32A555922ED9DDAE3\"},{\"account\":1,\"enc_pubkey\":\"CA0220001FD6019F7FBFCD9A34491643287402FB0CCB77F2A4F99482ADC11137CDF1FBD6200046924461A9069850A64E48E8EDB9C88764D3A0DC74AF929E335719F8A65B809B\",\"balance\":3700.174,\"balance_s\":\"3,700.1740\",\"n_operation\":2,\"updated_b\":479202,\"updated_b_active_mode\":300425,\"updated_b_passive_mode\":479202,\"state\":\"normal\",\"name\":\"\",\"type\":0,\"data\":\"\",\"seal\":\"E1DCD4FB943A323293F842451141B84925F24123\"}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.FindAccountsAsync(max: 2);
            Assert.Null(response.Error);
            Assert.Equal(2, response.Result.Length);
        }

        [Fact]
        public async Task SendToWithoutPayloadAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":1,\"subtype\":11,\"account\":38774,\"signer_account\":38774,\"n_operation\":1,\"senders\":[{\"account\":38774,\"n_operation\":1,\"amount\":-20.0000,\"amount_s\":\"-20.0000\",\"payload\":\"\",\"payload_type\":0}],\"receivers\":[{\"account\":52,\"amount\":20.0000,\"amount_s\":\"20.0000\",\"payload\":\"\",\"payload_type\":0}],\"changers\":[],\"optxt\":\"Tx-Out 20.0000 PASC from 38774-95 to 52-11\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":-20.0000,\"amount_s\":\"-20.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0.0000,\"sender_account\":38774,\"dest_account\":52,\"ophash\":\"0000000076970000010000004CE4F8C1B133C641F6D59A81D7E23D57582D6489\",\"old_ophash\":\"\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.SendToAsync(senderAccount: 38774, receiverAccount: 52, amount: 20, fee: 0);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.Transaction, result.Type);
            Assert.Equal(OperationSubType.TransactionSender, result.SubType);
            Assert.Equal<uint>(38774, result.AccountNumber);
            Assert.Equal<uint>(38774, result.SignerAccountNumber);
            Assert.Equal<uint>(1, result.NOperation);

            Assert.Equal<uint>(38774, result.Senders[0].AccountNumber);
            Assert.Equal(-20M, result.Senders[0].Amount);
            Assert.Equal<uint>(1, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.NonDeterministic, result.Senders[0].PayloadType);
            Assert.Equal("", result.Senders[0].Payload);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(20M, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.NonDeterministic, result.Receivers[0].PayloadType);
            Assert.Equal("", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("Tx-Out 20.0000 PASC from 38774-95 to 52-11", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(-20M, result.Amount);
            Assert.Equal("", result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("0000000076970000010000004CE4F8C1B133C641F6D59A81D7E23D57582D6489", result.OpHash);
        }
        [Fact]
        public async Task SendToWithPublicPayloadAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":1,\"subtype\":11,\"account\":38764,\"signer_account\":38764,\"n_operation\":4,\"senders\":[{\"account\":38764,\"account_epasa\":\"38764-64[\\\"This is a payload!\\\"]\",\"unenc_payload\":\"This is a payload!\",\"unenc_hexpayload\":\"546869732069732061207061796C6F616421\",\"n_operation\":4,\"amount\":-3,\"amount_s\":\"-3.0000\",\"payload\":\"546869732069732061207061796C6F616421\",\"payload_type\":17}],\"receivers\":[{\"account\":52,\"account_epasa\":\"52-11[\\\"This is a payload!\\\"]\",\"unenc_payload\":\"This is a payload!\",\"unenc_hexpayload\":\"546869732069732061207061796C6F616421\",\"amount\":3,\"amount_s\":\"3.0000\",\"payload\":\"546869732069732061207061796C6F616421\",\"payload_type\":17}],\"changers\":[],\"optxt\":\"Tx-Out 3.0000 PASC from 38764-64 to 52-11\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":-3,\"amount_s\":\"-3.0000\",\"payload\":\"546869732069732061207061796C6F616421\",\"payload_type\":17,\"balance\":0,\"sender_account\":38764,\"dest_account\":52,\"ophash\":\"000000006C9700000400000067A92BD681C616ABDD55C395B288F70150F2AED6\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.SendToAsync(senderAccount: 38764, receiverAccount: 52, amount: 3, fee: 0, payload: "This is a payload!", payloadMethod: PayloadMethod.None);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.Transaction, result.Type);
            Assert.Equal(OperationSubType.TransactionSender, result.SubType);
            Assert.Equal<uint>(38764, result.AccountNumber);
            Assert.Equal<uint>(38764, result.SignerAccountNumber);
            Assert.Equal<uint>(4, result.NOperation);

            Assert.Equal<uint>(38764, result.Senders[0].AccountNumber);
            Assert.Equal(-3M, result.Senders[0].Amount);
            Assert.Equal<uint>(4, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.Public | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("546869732069732061207061796C6F616421", result.Senders[0].Payload);
            Assert.Equal("38764-64[\"This is a payload!\"]", result.Senders[0].AccountEpasa);
            Assert.Equal("This is a payload!", result.Senders[0].UnencryptedPayload);
            Assert.Equal("546869732069732061207061796C6F616421", result.Senders[0].UnencryptedPayloadHexastring);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(3M, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.Public | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("546869732069732061207061796C6F616421", result.Receivers[0].Payload);
            Assert.Equal("52-11[\"This is a payload!\"]", result.Receivers[0].AccountEpasa);
            Assert.Equal("This is a payload!", result.Receivers[0].UnencryptedPayload);
            Assert.Equal("546869732069732061207061796C6F616421", result.Receivers[0].UnencryptedPayloadHexastring);

            Assert.Empty(result.Changers);

            Assert.Equal("Tx-Out 3.0000 PASC from 38764-64 to 52-11", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(-3M, result.Amount);
            Assert.Equal("546869732069732061207061796C6F616421", result.Payload);
            Assert.Equal(PayloadType.Public | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("000000006C9700000400000067A92BD681C616ABDD55C395B288F70150F2AED6", result.OpHash);
        }
        [Fact]
        public async Task SendToWithDestinationEncryptedPayloadAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":1,\"subtype\":11,\"account\":38764,\"signer_account\":38764,\"n_operation\":1,\"senders\":[{\"account\":38764,\"account_epasa\":\"38764-64(\\\"This is a payload!\\\")\",\"unenc_payload\":\"This is a payload!\",\"unenc_hexpayload\":\"546869732069732061207061796C6F616421\",\"n_operation\":1,\"amount\":-4,\"amount_s\":\"-4.0000\",\"payload\":\"43101200200002013EF59396C4839158716F73FCC78D4AF860304E485F1C7B685F29F7827219B125537B1A173766EDCC4EEEF47E584BADAF960B4CF63B304196A5C9B086194221A037FF8CDB6A49CE9B2714DA5EB4D5BBA5AC0FAAD19FA43BB0DB2CCB351B5FAF64113C0A1871B7B7723E5D416F8D19109108\",\"payload_type\":18}],\"receivers\":[{\"account\":52,\"account_epasa\":\"52-11(\\\"This is a payload!\\\")\",\"unenc_payload\":\"This is a payload!\",\"unenc_hexpayload\":\"546869732069732061207061796C6F616421\",\"amount\":3,\"amount_s\":\"3.0000\",\"payload\":\"43101200200002013EF59396C4839158716F73FCC78D4AF860304E485F1C7B685F29F7827219B125537B1A173766EDCC4EEEF47E584BADAF960B4CF63B304196A5C9B086194221A037FF8CDB6A49CE9B2714DA5EB4D5BBA5AC0FAAD19FA43BB0DB2CCB351B5FAF64113C0A1871B7B7723E5D416F8D19109108\",\"payload_type\":18}],\"changers\":[],\"optxt\":\"Tx-Out 3.0000 PASC from 38764-64 to 52-11\",\"fee\":-1,\"fee_s\":\"-1.0000\",\"amount\":-3,\"amount_s\":\"-3.0000\",\"payload\":\"43101200200002013EF59396C4839158716F73FCC78D4AF860304E485F1C7B685F29F7827219B125537B1A173766EDCC4EEEF47E584BADAF960B4CF63B304196A5C9B086194221A037FF8CDB6A49CE9B2714DA5EB4D5BBA5AC0FAAD19FA43BB0DB2CCB351B5FAF64113C0A1871B7B7723E5D416F8D19109108\",\"payload_type\":18,\"balance\":0,\"sender_account\":38764,\"dest_account\":52,\"ophash\":\"000000006C97000001000000731FCC7D37120E8918FF492ED3BAE73ED276D09F\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.SendToAsync(senderAccount: 38764, receiverAccount: 52, amount: 3, fee: 0, payload: "This is a payload!");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.Transaction, result.Type);
            Assert.Equal(OperationSubType.TransactionSender, result.SubType);
            Assert.Equal<uint>(38764, result.AccountNumber);
            Assert.Equal<uint>(38764, result.SignerAccountNumber);
            Assert.Equal<uint>(1, result.NOperation);

            Assert.Equal<uint>(38764, result.Senders[0].AccountNumber);
            Assert.Equal(-4M, result.Senders[0].Amount);
            Assert.Equal<uint>(1, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("43101200200002013EF59396C4839158716F73FCC78D4AF860304E485F1C7B685F29F7827219B125537B1A173766EDCC4EEEF47E584BADAF960B4CF63B304196A5C9B086194221A037FF8CDB6A49CE9B2714DA5EB4D5BBA5AC0FAAD19FA43BB0DB2CCB351B5FAF64113C0A1871B7B7723E5D416F8D19109108", result.Senders[0].Payload);
            Assert.Equal("38764-64(\"This is a payload!\")", result.Senders[0].AccountEpasa);
            Assert.Equal("This is a payload!", result.Senders[0].UnencryptedPayload);
            Assert.Equal("546869732069732061207061796C6F616421", result.Senders[0].UnencryptedPayloadHexastring);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(3M, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("43101200200002013EF59396C4839158716F73FCC78D4AF860304E485F1C7B685F29F7827219B125537B1A173766EDCC4EEEF47E584BADAF960B4CF63B304196A5C9B086194221A037FF8CDB6A49CE9B2714DA5EB4D5BBA5AC0FAAD19FA43BB0DB2CCB351B5FAF64113C0A1871B7B7723E5D416F8D19109108", result.Receivers[0].Payload);
            Assert.Equal("52-11(\"This is a payload!\")", result.Receivers[0].AccountEpasa);
            Assert.Equal("This is a payload!", result.Receivers[0].UnencryptedPayload);
            Assert.Equal("546869732069732061207061796C6F616421", result.Receivers[0].UnencryptedPayloadHexastring);

            Assert.Empty(result.Changers);

            Assert.Equal("Tx-Out 3.0000 PASC from 38764-64 to 52-11", result.Description);
            Assert.Equal(-1, result.Fee);
            Assert.Equal(-3M, result.Amount);
            Assert.Equal("43101200200002013EF59396C4839158716F73FCC78D4AF860304E485F1C7B685F29F7827219B125537B1A173766EDCC4EEEF47E584BADAF960B4CF63B304196A5C9B086194221A037FF8CDB6A49CE9B2714DA5EB4D5BBA5AC0FAAD19FA43BB0DB2CCB351B5FAF64113C0A1871B7B7723E5D416F8D19109108", result.Payload);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("000000006C97000001000000731FCC7D37120E8918FF492ED3BAE73ED276D09F", result.OpHash);
        }
        [Fact]
        public async Task SendToWithPasswordEncryptedPayloadAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":1,\"subtype\":11,\"account\":38764,\"signer_account\":38764,\"n_operation\":3,\"senders\":[{\"account\":38764,\"n_operation\":3,\"amount\":-3,\"amount_s\":\"-3.0000\",\"payload\":\"53616C7465645F5FBEB931B3579883F7218B536C4ED88CD6778C65EF4728EA82CF6A2F58D13B3D5C5EE22C3A573A454D\",\"payload_type\":24}],\"receivers\":[{\"account\":52,\"amount\":3,\"amount_s\":\"3.0000\",\"payload\":\"53616C7465645F5FBEB931B3579883F7218B536C4ED88CD6778C65EF4728EA82CF6A2F58D13B3D5C5EE22C3A573A454D\",\"payload_type\":24}],\"changers\":[],\"optxt\":\"Tx-Out 3.0000 PASC from 38764-64 to 52-11\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":-3,\"amount_s\":\"-3.0000\",\"payload\":\"53616C7465645F5FBEB931B3579883F7218B536C4ED88CD6778C65EF4728EA82CF6A2F58D13B3D5C5EE22C3A573A454D\",\"payload_type\":24,\"balance\":0,\"sender_account\":38764,\"dest_account\":52,\"ophash\":\"000000006C97000003000000D97F8D15951B40D2624F2DF5FD093E6E08832204\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.SendToAsync(senderAccount: 38764, receiverAccount: 52, amount: 3, fee: 0, payload: "This is a payload!", payloadMethod: PayloadMethod.Aes, password: "password");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.Transaction, result.Type);
            Assert.Equal(OperationSubType.TransactionSender, result.SubType);
            Assert.Equal<uint>(38764, result.AccountNumber);
            Assert.Equal<uint>(38764, result.SignerAccountNumber);
            Assert.Equal<uint>(3, result.NOperation);

            Assert.Equal<uint>(38764, result.Senders[0].AccountNumber);
            Assert.Equal(-3M, result.Senders[0].Amount);
            Assert.Equal<uint>(3, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("53616C7465645F5FBEB931B3579883F7218B536C4ED88CD6778C65EF4728EA82CF6A2F58D13B3D5C5EE22C3A573A454D", result.Senders[0].Payload);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(3M, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("53616C7465645F5FBEB931B3579883F7218B536C4ED88CD6778C65EF4728EA82CF6A2F58D13B3D5C5EE22C3A573A454D", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("Tx-Out 3.0000 PASC from 38764-64 to 52-11", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(-3M, result.Amount);
            Assert.Equal("53616C7465645F5FBEB931B3579883F7218B536C4ED88CD6778C65EF4728EA82CF6A2F58D13B3D5C5EE22C3A573A454D", result.Payload);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("000000006C97000003000000D97F8D15951B40D2624F2DF5FD093E6E08832204", result.OpHash);
        }

        [Fact]
        public async Task SendDataNoPayloadAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":10,\"subtype\":102,\"account\":32459,\"signer_account\":32459,\"n_operation\":2,\"senders\":[{\"account\":32459,\"n_operation\":2,\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"data\":{\"id\":\"\\u007BC114F2AE-5BF5-470C-8071-4023CC9F4612\\u007D\",\"sequence\":0,\"type\":0}}],\"receivers\":[{\"account\":52,\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0}],\"changers\":[],\"optxt\":\"OpData from:32459 to:52 type:0 sequence:0 Amount:0.0000\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0,\"ophash\":\"00000000CB7E0000020000002CDC9E0AEA3C0C2286D4E0E1C63E703801758999\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var myguid = new Guid("C114F2AE-5BF5-470C-8071-4023CC9F4612");
            var response = await connector.SendDataAsync(senderAccount: 32459, receiverAccount: 52, guid: myguid.ToString());
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.DataOperation, result.Type);
            Assert.Equal(OperationSubType.DataOperationSender, result.SubType);
            Assert.Equal<uint>(32459, result.AccountNumber);
            Assert.Equal<uint>(32459, result.SignerAccountNumber);
            Assert.Equal<uint>(2, result.NOperation);

            Assert.Equal<uint>(32459, result.Senders[0].AccountNumber);
            Assert.Equal(0, result.Senders[0].Amount);
            Assert.Equal<uint>(2, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.NonDeterministic, result.Senders[0].PayloadType);
            Assert.Equal("", result.Senders[0].Payload);

            Assert.Equal("{C114F2AE-5BF5-470C-8071-4023CC9F4612}", result.Senders[0].Data.Id);
            Assert.Equal(DataType.ChatMessage, result.Senders[0].Data.Type);
            Assert.Equal<uint>(0, result.Senders[0].Data.Sequence);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(0, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.NonDeterministic, result.Receivers[0].PayloadType);
            Assert.Equal("", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("OpData from:32459 to:52 type:0 sequence:0 Amount:0.0000", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal("", result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("00000000CB7E0000020000002CDC9E0AEA3C0C2286D4E0E1C63E703801758999", result.OpHash);
        }
        [Fact]
        public async Task SendDataWithPayloadAmountFeeSequenceAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":10,\"subtype\":102,\"account\":32459,\"signer_account\":32459,\"n_operation\":4,\"senders\":[{\"account\":32459,\"account_epasa\":\"32459-54[\\\"This is payload!\\\"]\",\"unenc_payload\":\"This is payload!\",\"unenc_hexpayload\":\"54686973206973207061796C6F616421\",\"n_operation\":4,\"amount\":5,\"amount_s\":\"5.0000\",\"payload\":\"54686973206973207061796C6F616421\",\"payload_type\":17,\"data\":{\"id\":\"\\u007BC114F2AE-5BF5-470C-8071-4023CC9F4612\\u007D\",\"sequence\":6,\"type\":0}}],\"receivers\":[{\"account\":52,\"account_epasa\":\"52-11[\\\"This is payload!\\\"]\",\"unenc_payload\":\"This is payload!\",\"unenc_hexpayload\":\"54686973206973207061796C6F616421\",\"amount\":3,\"amount_s\":\"3.0000\",\"payload\":\"54686973206973207061796C6F616421\",\"payload_type\":17}],\"changers\":[],\"optxt\":\"OpData from:32459 to:52 type:0 sequence:6 Amount:3.0000\",\"fee\":-2,\"fee_s\":\"-2.0000\",\"amount\":-5,\"amount_s\":\"-5.0000\",\"payload\":\"54686973206973207061796C6F616421\",\"payload_type\":17,\"balance\":0,\"ophash\":\"00000000CB7E000004000000FFA7084AFBAE8DD9642F22FF0E3DA87C51965B65\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var myguid = new Guid("C114F2AE-5BF5-470C-8071-4023CC9F4612");
            //Sends 3 pascals (by default = 0) in DataOperation and pays 2 pascals (by default = 0) in transaction
            var response = await connector.SendDataAsync(senderAccount: 32459, receiverAccount: 52, guid: myguid.ToString(), payload: "This is payload!", amount: 3, fee: 2);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.DataOperation, result.Type);
            Assert.Equal(OperationSubType.DataOperationSender, result.SubType);
            Assert.Equal<uint>(32459, result.AccountNumber);
            Assert.Equal<uint>(32459, result.SignerAccountNumber);
            Assert.Equal<uint>(4, result.NOperation);

            Assert.Equal<uint>(32459, result.Senders[0].AccountNumber);
            Assert.Equal("32459-54[\"This is payload!\"]", result.Senders[0].AccountEpasa);
            Assert.Equal("This is payload!", result.Senders[0].UnencryptedPayload);
            Assert.Equal("54686973206973207061796C6F616421", result.Senders[0].UnencryptedPayloadHexastring);
            Assert.Equal(5, result.Senders[0].Amount);
            Assert.Equal<uint>(4, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.Public | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("54686973206973207061796C6F616421", result.Senders[0].Payload);

            Assert.Equal("{C114F2AE-5BF5-470C-8071-4023CC9F4612}", result.Senders[0].Data.Id);
            Assert.Equal(DataType.ChatMessage, result.Senders[0].Data.Type);
            Assert.Equal<uint>(6, result.Senders[0].Data.Sequence);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal("52-11[\"This is payload!\"]", result.Receivers[0].AccountEpasa);
            Assert.Equal("This is payload!", result.Receivers[0].UnencryptedPayload);
            Assert.Equal("54686973206973207061796C6F616421", result.Receivers[0].UnencryptedPayloadHexastring);
            Assert.Equal(3, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.Public | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("54686973206973207061796C6F616421", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("OpData from:32459 to:52 type:0 sequence:6 Amount:3.0000", result.Description);
            Assert.Equal(-2, result.Fee);
            Assert.Equal(-5, result.Amount);
            Assert.Equal("54686973206973207061796C6F616421", result.Payload);
            Assert.Equal(PayloadType.Public | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("00000000CB7E000004000000FFA7084AFBAE8DD9642F22FF0E3DA87C51965B65", result.OpHash);
        }
        [Fact]
        public async Task SendDataWithDestinationEncryptedPayloadAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":10,\"subtype\":102,\"account\":32459,\"signer_account\":32459,\"n_operation\":5,\"senders\":[{\"account\":32459,\"account_epasa\":\"32459-54(\\\"This is payload!\\\")\",\"unenc_payload\":\"This is payload!\",\"unenc_hexpayload\":\"54686973206973207061796C6F616421\",\"n_operation\":5,\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"43101000100003012D6B6BBFA1C45DD193F954C78C649FD12ADB5D19DD6B48C636CBD443DF0B236B4C402DB3375341C77E102ECCCC8939284B633568BF01DC35CD604329F84ADCA8A7C67A862CEDD2501EBCFD1B0ACB02A64E7226C588BF6CE551073BC30423CDEED9\",\"payload_type\":18,\"data\":{\"id\":\"\\u007BC114F2AE-5BF5-470C-8071-4023CC9F4612\\u007D\",\"sequence\":0,\"type\":0}}],\"receivers\":[{\"account\":52,\"account_epasa\":\"52-11(\\\"This is payload!\\\")\",\"unenc_payload\":\"This is payload!\",\"unenc_hexpayload\":\"54686973206973207061796C6F616421\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"43101000100003012D6B6BBFA1C45DD193F954C78C649FD12ADB5D19DD6B48C636CBD443DF0B236B4C402DB3375341C77E102ECCCC8939284B633568BF01DC35CD604329F84ADCA8A7C67A862CEDD2501EBCFD1B0ACB02A64E7226C588BF6CE551073BC30423CDEED9\",\"payload_type\":18}],\"changers\":[],\"optxt\":\"OpData from:32459 to:52 type:0 sequence:0 Amount:0.0000\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"43101000100003012D6B6BBFA1C45DD193F954C78C649FD12ADB5D19DD6B48C636CBD443DF0B236B4C402DB3375341C77E102ECCCC8939284B633568BF01DC35CD604329F84ADCA8A7C67A862CEDD2501EBCFD1B0ACB02A64E7226C588BF6CE551073BC30423CDEED9\",\"payload_type\":18,\"balance\":0,\"ophash\":\"00000000CB7E000005000000E4491A57673595BCBB2A0466D3784558F2025E4C\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var myguid = new Guid("C114F2AE-5BF5-470C-8071-4023CC9F4612");
            var response = await connector.SendDataAsync(senderAccount: 32459, receiverAccount: 52, guid: myguid.ToString(), payload: "This is payload!", payloadMethod: PayloadMethod.Dest);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.DataOperation, result.Type);
            Assert.Equal(OperationSubType.DataOperationSender, result.SubType);
            Assert.Equal<uint>(32459, result.AccountNumber);
            Assert.Equal<uint>(32459, result.SignerAccountNumber);
            Assert.Equal<uint>(5, result.NOperation);

            Assert.Equal<uint>(32459, result.Senders[0].AccountNumber);
            Assert.Equal("32459-54(\"This is payload!\")", result.Senders[0].AccountEpasa);
            Assert.Equal("This is payload!", result.Senders[0].UnencryptedPayload);
            Assert.Equal("54686973206973207061796C6F616421", result.Senders[0].UnencryptedPayloadHexastring);
            Assert.Equal(0, result.Senders[0].Amount);
            Assert.Equal<uint>(5, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("43101000100003012D6B6BBFA1C45DD193F954C78C649FD12ADB5D19DD6B48C636CBD443DF0B236B4C402DB3375341C77E102ECCCC8939284B633568BF01DC35CD604329F84ADCA8A7C67A862CEDD2501EBCFD1B0ACB02A64E7226C588BF6CE551073BC30423CDEED9", result.Senders[0].Payload);

            Assert.Equal("{C114F2AE-5BF5-470C-8071-4023CC9F4612}", result.Senders[0].Data.Id);
            Assert.Equal(DataType.ChatMessage, result.Senders[0].Data.Type);
            Assert.Equal<uint>(0, result.Senders[0].Data.Sequence);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal("52-11(\"This is payload!\")", result.Receivers[0].AccountEpasa);
            Assert.Equal("This is payload!", result.Receivers[0].UnencryptedPayload);
            Assert.Equal("54686973206973207061796C6F616421", result.Receivers[0].UnencryptedPayloadHexastring);
            Assert.Equal(0, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("43101000100003012D6B6BBFA1C45DD193F954C78C649FD12ADB5D19DD6B48C636CBD443DF0B236B4C402DB3375341C77E102ECCCC8939284B633568BF01DC35CD604329F84ADCA8A7C67A862CEDD2501EBCFD1B0ACB02A64E7226C588BF6CE551073BC30423CDEED9", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("OpData from:32459 to:52 type:0 sequence:0 Amount:0.0000", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal("43101000100003012D6B6BBFA1C45DD193F954C78C649FD12ADB5D19DD6B48C636CBD443DF0B236B4C402DB3375341C77E102ECCCC8939284B633568BF01DC35CD604329F84ADCA8A7C67A862CEDD2501EBCFD1B0ACB02A64E7226C588BF6CE551073BC30423CDEED9", result.Payload);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("00000000CB7E000005000000E4491A57673595BCBB2A0466D3784558F2025E4C", result.OpHash);
        }
        [Fact]
        public async Task SendDataWithAesEncryptedPayloadAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":10,\"subtype\":102,\"account\":32459,\"signer_account\":32459,\"n_operation\":6,\"senders\":[{\"account\":32459,\"n_operation\":6,\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"53616C7465645F5F9E6A2FCAC924B38477536EB9ADE1AA431D2F2D1E1208A64C88A77AB360F65857BD87955FBC2EBFD3\",\"payload_type\":24,\"data\":{\"id\":\"\\u007BC114F2AE-5BF5-470C-8071-4023CC9F4612\\u007D\",\"sequence\":0,\"type\":0}}],\"receivers\":[{\"account\":52,\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"53616C7465645F5F9E6A2FCAC924B38477536EB9ADE1AA431D2F2D1E1208A64C88A77AB360F65857BD87955FBC2EBFD3\",\"payload_type\":24}],\"changers\":[],\"optxt\":\"OpData from:32459 to:52 type:0 sequence:0 Amount:0.0000\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"53616C7465645F5F9E6A2FCAC924B38477536EB9ADE1AA431D2F2D1E1208A64C88A77AB360F65857BD87955FBC2EBFD3\",\"payload_type\":24,\"balance\":0,\"ophash\":\"00000000CB7E00000600000081929CB765F3A62167DEF492E31149CFEC3929CA\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var myguid = new Guid("C114F2AE-5BF5-470C-8071-4023CC9F4612");
            var response = await connector.SendDataAsync(senderAccount: 32459, receiverAccount: 52, guid: myguid.ToString(), payload: "This is payload!", payloadMethod: PayloadMethod.Aes, password: "password");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.DataOperation, result.Type);
            Assert.Equal(OperationSubType.DataOperationSender, result.SubType);
            Assert.Equal<uint>(32459, result.AccountNumber);
            Assert.Equal<uint>(32459, result.SignerAccountNumber);
            Assert.Equal<uint>(6, result.NOperation);

            Assert.Equal<uint>(32459, result.Senders[0].AccountNumber);
            Assert.Equal(0, result.Senders[0].Amount);
            Assert.Equal<uint>(6, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("53616C7465645F5F9E6A2FCAC924B38477536EB9ADE1AA431D2F2D1E1208A64C88A77AB360F65857BD87955FBC2EBFD3", result.Senders[0].Payload);

            Assert.Equal("{C114F2AE-5BF5-470C-8071-4023CC9F4612}", result.Senders[0].Data.Id);
            Assert.Equal(DataType.ChatMessage, result.Senders[0].Data.Type);
            Assert.Equal<uint>(0, result.Senders[0].Data.Sequence);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal(0, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("53616C7465645F5F9E6A2FCAC924B38477536EB9ADE1AA431D2F2D1E1208A64C88A77AB360F65857BD87955FBC2EBFD3", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("OpData from:32459 to:52 type:0 sequence:0 Amount:0.0000", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal("53616C7465645F5F9E6A2FCAC924B38477536EB9ADE1AA431D2F2D1E1208A64C88A77AB360F65857BD87955FBC2EBFD3", result.Payload);
            Assert.Equal(PayloadType.PasswordEncrypted | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("00000000CB7E00000600000081929CB765F3A62167DEF492E31149CFEC3929CA", result.OpHash);
        }

        [Fact]
        public async Task FindDataOperationsKnowingSenderReceiverGuidAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"block\":18261,\"time\":0,\"opblock\":0,\"maturation\":64,\"optype\":10,\"subtype\":102,\"account\":32459,\"signer_account\":32459,\"n_operation\":7,\"senders\":[{\"account\":32459,\"account_epasa\":\"32459-54(\\\"This is payload!\\\")\",\"unenc_payload\":\"This is payload!\",\"unenc_hexpayload\":\"54686973206973207061796C6F616421\",\"n_operation\":7,\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"431010001000020015AD49FE5C3016B214D3B7021906EF746A737BBF72F4FE42FB0613DA5391CAE2D0CF7C802B43A93A11F84EAD4B6189392D95FCF7004347D99722117E800B9324E77433DD6334D178C4C71AF5F6D25216B1CF28F146263B82FB43EF3D3D68E5436E\",\"payload_type\":18,\"data\":{\"id\":\"\\u007B7244FE2E-0A87-4567-A472-72951D1B4F20\\u007D\",\"sequence\":0,\"type\":0}}],\"receivers\":[{\"account\":52,\"account_epasa\":\"52-11(\\\"This is payload!\\\")\",\"unenc_payload\":\"This is payload!\",\"unenc_hexpayload\":\"54686973206973207061796C6F616421\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"431010001000020015AD49FE5C3016B214D3B7021906EF746A737BBF72F4FE42FB0613DA5391CAE2D0CF7C802B43A93A11F84EAD4B6189392D95FCF7004347D99722117E800B9324E77433DD6334D178C4C71AF5F6D25216B1CF28F146263B82FB43EF3D3D68E5436E\",\"payload_type\":18}],\"changers\":[],\"optxt\":\"OpData from:32459 to:52 type:0 sequence:0 Amount:0.0000\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"431010001000020015AD49FE5C3016B214D3B7021906EF746A737BBF72F4FE42FB0613DA5391CAE2D0CF7C802B43A93A11F84EAD4B6189392D95FCF7004347D99722117E800B9324E77433DD6334D178C4C71AF5F6D25216B1CF28F146263B82FB43EF3D3D68E5436E\",\"payload_type\":18,\"ophash\":\"55470000CB7E0000070000002C4E5E0EC8AE7B82A3EA413D5A2BACE7D1D35742\"}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var guid = new Guid("7244FE2E-0A87-4567-A472-72951D1B4F20");
            var response = await connector.FindDataOperationsAsync(senderAccount: 32459, receiverAccount: 52, guid: guid.ToString());
            Assert.Null(response.Error);
            Assert.Single(response.Result);

            var result = response.Result[0];

            Assert.Equal<uint?>(18261, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(0, result.Index);
            Assert.Equal<uint?>(64, result.Maturation);
            Assert.Equal(OperationType.DataOperation, result.Type);
            Assert.Equal(OperationSubType.DataOperationSender, result.SubType);
            Assert.Equal<uint>(32459, result.AccountNumber);
            Assert.Equal<uint>(32459, result.SignerAccountNumber);
            Assert.Equal<uint>(7, result.NOperation);

            Assert.Equal<uint>(32459, result.Senders[0].AccountNumber);
            Assert.Equal("32459-54(\"This is payload!\")", result.Senders[0].AccountEpasa);
            Assert.Equal("This is payload!", result.Senders[0].UnencryptedPayload);
            Assert.Equal("54686973206973207061796C6F616421", result.Senders[0].UnencryptedPayloadHexastring);
            Assert.Equal(0, result.Senders[0].Amount);
            Assert.Equal<uint>(7, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("431010001000020015AD49FE5C3016B214D3B7021906EF746A737BBF72F4FE42FB0613DA5391CAE2D0CF7C802B43A93A11F84EAD4B6189392D95FCF7004347D99722117E800B9324E77433DD6334D178C4C71AF5F6D25216B1CF28F146263B82FB43EF3D3D68E5436E", result.Senders[0].Payload);

            Assert.Equal("{7244FE2E-0A87-4567-A472-72951D1B4F20}", result.Senders[0].Data.Id);
            Assert.Equal(DataType.ChatMessage, result.Senders[0].Data.Type);
            Assert.Equal<uint>(0, result.Senders[0].Data.Sequence);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal("52-11(\"This is payload!\")", result.Receivers[0].AccountEpasa);
            Assert.Equal("This is payload!", result.Receivers[0].UnencryptedPayload);
            Assert.Equal("54686973206973207061796C6F616421", result.Receivers[0].UnencryptedPayloadHexastring);
            Assert.Equal(0, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("431010001000020015AD49FE5C3016B214D3B7021906EF746A737BBF72F4FE42FB0613DA5391CAE2D0CF7C802B43A93A11F84EAD4B6189392D95FCF7004347D99722117E800B9324E77433DD6334D178C4C71AF5F6D25216B1CF28F146263B82FB43EF3D3D68E5436E", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("OpData from:32459 to:52 type:0 sequence:0 Amount:0.0000", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal("431010001000020015AD49FE5C3016B214D3B7021906EF746A737BBF72F4FE42FB0613DA5391CAE2D0CF7C802B43A93A11F84EAD4B6189392D95FCF7004347D99722117E800B9324E77433DD6334D178C4C71AF5F6D25216B1CF28F146263B82FB43EF3D3D68E5436E", result.Payload);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("55470000CB7E0000070000002C4E5E0EC8AE7B82A3EA413D5A2BACE7D1D35742", result.OpHash);
        }
        [Fact]
        public async Task FindDataOperationsKnowingSenderReceiverGuidDataTypeSequenceAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"block\":18374,\"time\":0,\"opblock\":0,\"maturation\":23227,\"optype\":10,\"subtype\":102,\"account\":32459,\"signer_account\":32459,\"n_operation\":11,\"senders\":[{\"account\":32459,\"account_epasa\":\"32459-54(\\\"This is payload!\\\"):b0a9\",\"unenc_payload\":\"This is payload!\",\"unenc_hexpayload\":\"54686973206973207061796C6F616421\",\"n_operation\":11,\"amount\":1.0000,\"amount_s\":\"1.0000\",\"payload\":\"43101000100003001E747CE916B0A09053E7E564A8A00327257EA42A995029889CF2B7AEB304097F0217CDBE90ACC2ACDD54A74214F5AB487EF28D2E54CE5058AF8F2098091D70511A3E306D930DE9073E89CB4BD230DF8D7F477C206D969BFA27C5D0BC765EB57BE4\",\"payload_type\":18,\"data\":{\"id\":\"\\u007B7244FE2E-0A87-4567-A472-72951D1B4F20\\u007D\",\"sequence\":5,\"type\":1}}],\"receivers\":[{\"account\":52,\"account_epasa\":\"52-11(\\\"This is payload!\\\"):5568\",\"unenc_payload\":\"This is payload!\",\"unenc_hexpayload\":\"54686973206973207061796C6F616421\",\"amount\":1.0000,\"amount_s\":\"1.0000\",\"payload\":\"43101000100003001E747CE916B0A09053E7E564A8A00327257EA42A995029889CF2B7AEB304097F0217CDBE90ACC2ACDD54A74214F5AB487EF28D2E54CE5058AF8F2098091D70511A3E306D930DE9073E89CB4BD230DF8D7F477C206D969BFA27C5D0BC765EB57BE4\",\"payload_type\":18}],\"changers\":[],\"optxt\":\"OpData from:32459 to:52 type:1 sequence:5 Amount:1.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":-1.0000,\"amount_s\":\"-1.0000\",\"payload\":\"43101000100003001E747CE916B0A09053E7E564A8A00327257EA42A995029889CF2B7AEB304097F0217CDBE90ACC2ACDD54A74214F5AB487EF28D2E54CE5058AF8F2098091D70511A3E306D930DE9073E89CB4BD230DF8D7F477C206D969BFA27C5D0BC765EB57BE4\",\"payload_type\":18,\"ophash\":\"C6470000CB7E00000B000000A6C54A60987B0011CE658F24CF3611CE67CC7B0B\"}],\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var guid = new Guid("7244FE2E-0A87-4567-A472-72951D1B4F20");
            var response = await connector.FindDataOperationsAsync(senderAccount: 32459, receiverAccount: 52, guid: guid.ToString(), dataType: DataType.PrivateMessage, dataSequence: 5);
            Assert.Null(response.Error);
            Assert.Single(response.Result);

            var result = response.Result[0];

            Assert.Equal<uint?>(18374, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(0, result.Index);
            Assert.Equal<uint?>(23227, result.Maturation);
            Assert.Equal(OperationType.DataOperation, result.Type);
            Assert.Equal(OperationSubType.DataOperationSender, result.SubType);
            Assert.Equal<uint>(32459, result.AccountNumber);
            Assert.Equal<uint>(32459, result.SignerAccountNumber);
            Assert.Equal<uint>(11, result.NOperation);

            Assert.Equal<uint>(32459, result.Senders[0].AccountNumber);
            Assert.Equal("32459-54(\"This is payload!\"):b0a9", result.Senders[0].AccountEpasa);
            Assert.Equal("This is payload!", result.Senders[0].UnencryptedPayload);
            Assert.Equal("54686973206973207061796C6F616421", result.Senders[0].UnencryptedPayloadHexastring);
            Assert.Equal(1, result.Senders[0].Amount);
            Assert.Equal<uint>(11, result.Senders[0].NOperation);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.Senders[0].PayloadType);
            Assert.Equal("43101000100003001E747CE916B0A09053E7E564A8A00327257EA42A995029889CF2B7AEB304097F0217CDBE90ACC2ACDD54A74214F5AB487EF28D2E54CE5058AF8F2098091D70511A3E306D930DE9073E89CB4BD230DF8D7F477C206D969BFA27C5D0BC765EB57BE4", result.Senders[0].Payload);

            Assert.Equal("{7244FE2E-0A87-4567-A472-72951D1B4F20}", result.Senders[0].Data.Id);
            Assert.Equal(DataType.PrivateMessage, result.Senders[0].Data.Type);
            Assert.Equal<uint>(5, result.Senders[0].Data.Sequence);

            Assert.Equal<uint>(52, result.Receivers[0].AccountNumber);
            Assert.Equal("52-11(\"This is payload!\"):5568", result.Receivers[0].AccountEpasa);
            Assert.Equal("This is payload!", result.Receivers[0].UnencryptedPayload);
            Assert.Equal("54686973206973207061796C6F616421", result.Receivers[0].UnencryptedPayloadHexastring);
            Assert.Equal(1, result.Receivers[0].Amount);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.Receivers[0].PayloadType);
            Assert.Equal("43101000100003001E747CE916B0A09053E7E564A8A00327257EA42A995029889CF2B7AEB304097F0217CDBE90ACC2ACDD54A74214F5AB487EF28D2E54CE5058AF8F2098091D70511A3E306D930DE9073E89CB4BD230DF8D7F477C206D969BFA27C5D0BC765EB57BE4", result.Receivers[0].Payload);

            Assert.Empty(result.Changers);

            Assert.Equal("OpData from:32459 to:52 type:1 sequence:5 Amount:1.0000", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(-1, result.Amount);
            Assert.Equal("43101000100003001E747CE916B0A09053E7E564A8A00327257EA42A995029889CF2B7AEB304097F0217CDBE90ACC2ACDD54A74214F5AB487EF28D2E54CE5058AF8F2098091D70511A3E306D930DE9073E89CB4BD230DF8D7F477C206D969BFA27C5D0BC765EB57BE4", result.Payload);
            Assert.Equal(PayloadType.RecipientKeyEncrypted | PayloadType.AsciiFormatted, result.PayloadType);
            Assert.Equal("C6470000CB7E00000B000000A6C54A60987B0011CE658F24CF3611CE67CC7B0B", result.OpHash);
        }

        [Fact]
        public async Task ChangeKeyUsingNewEncodedPublicKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":2,\"subtype\":21,\"account\":38764,\"signer_account\":38764,\"n_operation\":5,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":38764,\"n_operation\":5,\"new_enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"changes\":\"public_key\"}],\"optxt\":\"Change Key to secp256k1\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0,\"enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"ophash\":\"000000006C97000005000000ACAB2705463EAD7D261554DA4C84CC5988735BF3\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.ChangeKeyAsync(account: 38764, newEncodedPublicKey: "CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.ChangeKey, result.Type);
            Assert.Equal(OperationSubType.ChangeKey, result.SubType);
            Assert.Equal<uint>(38764, result.AccountNumber);
            Assert.Equal<uint>(38764, result.SignerAccountNumber);
            Assert.Equal<uint>(5, result.NOperation);

            Assert.Empty(result.Senders);
            Assert.Empty(result.Receivers);
            Assert.Single(result.Changers);

            Assert.Equal<uint>(38764, result.Changers[0].AccountNumber);
            Assert.Equal<uint>(5, result.Changers[0].NOperation);
            Assert.Equal("CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167", result.Changers[0].NewEncodedPublicKey);
            Assert.Equal("public_key", result.Changers[0].Changes);

            Assert.Equal("Change Key to secp256k1", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal("", result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("000000006C97000005000000ACAB2705463EAD7D261554DA4C84CC5988735BF3", result.OpHash);
        }
        [Fact]
        public async Task ChangeKeyUsingNewB58PublicKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":2,\"subtype\":21,\"account\":38764,\"signer_account\":38764,\"n_operation\":5,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":38764,\"n_operation\":5,\"new_enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"changes\":\"public_key\"}],\"optxt\":\"Change Key to secp256k1\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0,\"enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"ophash\":\"000000006C97000005000000ACAB2705463EAD7D261554DA4C84CC5988735BF3\",\"old_ophash\":\"\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.ChangeKeyAsync(account: 38764, newB58PublicKey: "3GhhbouLD4QsBCZbL3ozd6zBaQQYWgoHLPKLmrby5n4MgN6HknS2q825dVvZg6qJEFrikQdoiNbw5TsWJdYPZhCKTRomcSePRS7gE9");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.ChangeKey, result.Type);
            Assert.Equal(OperationSubType.ChangeKey, result.SubType);
            Assert.Equal<uint>(38764, result.AccountNumber);
            Assert.Equal<uint>(38764, result.SignerAccountNumber);
            Assert.Equal<uint>(5, result.NOperation);

            Assert.Empty(result.Senders);
            Assert.Empty(result.Receivers);
            Assert.Single(result.Changers);

            Assert.Equal<uint>(38764, result.Changers[0].AccountNumber);
            Assert.Equal<uint>(5, result.Changers[0].NOperation);
            Assert.Equal("CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167", result.Changers[0].NewEncodedPublicKey);
            Assert.Equal("public_key", result.Changers[0].Changes);

            Assert.Equal("Change Key to secp256k1", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal("", result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("000000006C97000005000000ACAB2705463EAD7D261554DA4C84CC5988735BF3", result.OpHash);
        }

        [Fact]
        public async Task ChangeKeysUsingNewEncodedPublicKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"block\":0,\"time\":0,\"opblock\":0,\"maturation\":null,\"optype\":2,\"subtype\":21,\"account\":43929,\"signer_account\":43929,\"n_operation\":1,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":43929,\"n_operation\":1,\"new_enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"changes\":\"public_key\"}],\"optxt\":\"Change Key to secp256k1\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":20,\"enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"ophash\":\"0000000099AB0000010000003E50D327F4236D8FD7AEE04D791103B0DA1DB667\",\"old_ophash\":\"\"},{\"block\":0,\"time\":0,\"opblock\":1,\"maturation\":null,\"optype\":2,\"subtype\":21,\"account\":43930,\"signer_account\":43930,\"n_operation\":1,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":43930,\"n_operation\":1,\"new_enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"changes\":\"public_key\"}],\"optxt\":\"Change Key to secp256k1\",\"fee\":0,\"fee_s\":\"0.0000\",\"amount\":0,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":80,\"enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"ophash\":\"000000009AAB0000010000000BB50E15C3A25DB5DDB33ADB7793C2C4571670F4\",\"old_ophash\":\"\"}],\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.ChangeKeysAsync(accounts: new uint[] { 43929, 43930 }, newEncodedPublicKey: "CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167");
            Assert.Null(response.Error);

            var result = response.Result;
            Assert.Equal(2, result.Length);

            var result0 = result[0];

            Assert.Equal<uint?>(0, result0.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result0.Time);
            Assert.Equal(0, result0.Index);
            Assert.Null(result0.Maturation);
            Assert.Equal(OperationType.ChangeKey, result0.Type);
            Assert.Equal(OperationSubType.ChangeKey, result0.SubType);
            Assert.Equal<uint>(43929, result0.AccountNumber);
            Assert.Equal<uint>(43929, result0.SignerAccountNumber);
            Assert.Equal<uint>(1, result0.NOperation);

            Assert.Empty(result0.Senders);
            Assert.Empty(result0.Receivers);
            Assert.Single(result0.Changers);

            Assert.Equal<uint>(43929, result0.Changers[0].AccountNumber);
            Assert.Equal<uint>(1, result0.Changers[0].NOperation);
            Assert.Equal("CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167", result0.Changers[0].NewEncodedPublicKey);
            Assert.Equal("public_key", result0.Changers[0].Changes);

            Assert.Equal("Change Key to secp256k1", result0.Description);
            Assert.Equal(0, result0.Fee);
            Assert.Equal(0, result0.Amount);
            Assert.Equal("", result0.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result0.PayloadType);
            Assert.Equal("0000000099AB0000010000003E50D327F4236D8FD7AEE04D791103B0DA1DB667", result0.OpHash);


            var result1 = result[1];

            Assert.Equal<uint?>(0, result1.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result1.Time);
            Assert.Equal(1, result1.Index);
            Assert.Null(result1.Maturation);
            Assert.Equal(OperationType.ChangeKey, result1.Type);
            Assert.Equal(OperationSubType.ChangeKey, result1.SubType);
            Assert.Equal<uint>(43930, result1.AccountNumber);
            Assert.Equal<uint>(43930, result1.SignerAccountNumber);
            Assert.Equal<uint>(1, result1.NOperation);

            Assert.Empty(result1.Senders);
            Assert.Empty(result1.Receivers);
            Assert.Single(result1.Changers);

            Assert.Equal<uint>(43930, result1.Changers[0].AccountNumber);
            Assert.Equal<uint>(1, result1.Changers[0].NOperation);
            Assert.Equal("CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167", result1.Changers[0].NewEncodedPublicKey);
            Assert.Equal("public_key", result1.Changers[0].Changes);

            Assert.Equal("Change Key to secp256k1", result1.Description);
            Assert.Equal(0, result1.Fee);
            Assert.Equal(0, result1.Amount);
            Assert.Equal("", result1.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result1.PayloadType);
            Assert.Equal("000000009AAB0000010000000BB50E15C3A25DB5DDB33ADB7793C2C4571670F4", result1.OpHash);
        }

        [Fact]
        public async Task ListAccountForSaleAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":4,\"subtype\":41,\"account\":32325,\"signer_account\":32325,\"n_operation\":23,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":32325,\"n_operation\":23,\"seller_account\":52,\"account_price\":20.0000,\"account_price_s\":\"20.0000\",\"locked_until_block\":0,\"changes\":\"list_for_public_sale\"}],\"optxt\":\"List account 32325-48 for sale price 20.0000 PASC pay to 52-11\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":0.0000,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0.0000,\"ophash\":\"00000000457E000017000000583A4F0AF73DED73ED43E8AD16EED5A8DE62A2EC\",\"old_ophash\":\"\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.ListAccountForSaleAsync(accountForSale: 32325, sellerAccount: 52, price: 20, signerAccount: 32325);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.ListAccountForSale, result.Type);
            Assert.Equal(OperationSubType.ListAccountForPublicSale, result.SubType);
            Assert.Equal<uint>(32325, result.AccountNumber);
            Assert.Equal<uint>(32325, result.SignerAccountNumber);
            Assert.Equal<uint>(23, result.NOperation);

            Assert.Empty(result.Senders);
            Assert.Empty(result.Receivers);

            Assert.Single(result.Changers);
            Assert.Equal<uint>(32325, result.Changers[0].AccountNumber);
            Assert.Equal<uint>(23, result.Changers[0].NOperation);
            Assert.Equal<uint?>(52, result.Changers[0].SellerAccount);
            Assert.Equal(20, result.Changers[0].AccountPrice);
            Assert.Equal<uint?>(0, result.Changers[0].LockedUntilBlock);
            Assert.Equal("list_for_public_sale", result.Changers[0].Changes);

            Assert.Equal("List account 32325-48 for sale price 20.0000 PASC pay to 52-11", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal(string.Empty, result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("00000000457E000017000000583A4F0AF73DED73ED43E8AD16EED5A8DE62A2EC", result.OpHash);
        }

        //TODO: test private sale, atomic swap account, atomic swap coin, 

        [Fact]
        public async Task DelistAccountForSaleAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":5,\"subtype\":51,\"account\":32325,\"signer_account\":32325,\"n_operation\":11,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":32325,\"n_operation\":11,\"changes\":\"delist\"}],\"optxt\":\"Delist account 32325-48 for sale\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":0.0000,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0.0000,\"ophash\":\"00000000457E00000B000000417D4029C97B7948FE0E1B59FB4F3C91945D8D2F\",\"old_ophash\":\"\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.DelistAccountForSaleAsync(accountNumber: 32325, signerAccount: 32325);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.DelistAccount, result.Type);
            Assert.Equal(OperationSubType.DelistAccount, result.SubType);
            Assert.Equal<uint>(32325, result.AccountNumber);
            Assert.Equal<uint>(32325, result.SignerAccountNumber);
            Assert.Equal<uint>(11, result.NOperation);

            Assert.Empty(result.Senders);
            Assert.Empty(result.Receivers);

            Assert.Single(result.Changers);
            Assert.Equal<uint>(32325, result.Changers[0].AccountNumber);
            Assert.Equal<uint>(11, result.Changers[0].NOperation);
            Assert.Equal("delist", result.Changers[0].Changes);

            Assert.Equal("Delist account 32325-48 for sale", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal(string.Empty, result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("00000000457E00000B000000417D4029C97B7948FE0E1B59FB4F3C91945D8D2F", result.OpHash);
        }

        [Fact]
        public async Task BuyAccountAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":6,\"subtype\":62,\"account\":117842,\"signer_account\":52,\"n_operation\":345,\"senders\":[{\"account\":52,\"n_operation\":345,\"amount\":-1.0000,\"amount_s\":\"-1.0000\",\"payload\":\"\",\"payload_type\":0}],\"receivers\":[{\"account\":117842,\"amount\":0.0000,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0},{\"account\":75,\"amount\":1.0000,\"amount_s\":\"1.0000\",\"payload\":\"\",\"payload_type\":0}],\"changers\":[{\"account\":117842,\"new_enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"changes\":\"public_key\"}],\"optxt\":\"Purchased account 117842-82 by 52-11 for 1.0000 PASC\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":0.0000,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0.0000,\"ophash\":\"000000003400000059010000D245B2DC95B564C0CF15D5A6A44AD1E797A9E074\",\"old_ophash\":\"\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.BuyAccountAsync(buyerAccount: 52, accountToPurchase: 117842, amount: 1, newB58PublicKey: "3GhhbouLD4QsBCZbL3ozd6zBaQQYWgoHLPKLmrby5n4MgN6HknS2q825dVvZg6qJEFrikQdoiNbw5TsWJdYPZhCKTRomcSePRS7gE9");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.BuyAccount, result.Type);
            Assert.Equal(OperationSubType.BuyAccountTarget, result.SubType);
            Assert.Equal<uint>(117842, result.AccountNumber);
            Assert.Equal<uint>(52, result.SignerAccountNumber);
            Assert.Equal<uint>(345, result.NOperation);

            Assert.Single(result.Senders);
            Assert.Equal<uint>(52, result.Senders[0].AccountNumber);
            Assert.Equal<uint>(345, result.Senders[0].NOperation);
            Assert.Equal(-1, result.Senders[0].Amount);
            Assert.Equal(string.Empty, result.Senders[0].Payload);
            Assert.Equal(-1, result.Senders[0].Amount);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);

            Assert.Equal(2, result.Receivers.Length);
            Assert.Equal<uint>(117842, result.Receivers[0].AccountNumber);
            Assert.Equal(0, result.Receivers[0].Amount);
            Assert.Equal(string.Empty, result.Receivers[0].Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.Receivers[0].PayloadType);
            Assert.Equal<uint>(75, result.Receivers[1].AccountNumber);
            Assert.Equal(1, result.Receivers[1].Amount);
            Assert.Equal(string.Empty, result.Receivers[1].Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.Receivers[1].PayloadType);

            Assert.Single(result.Changers);
            Assert.Equal<uint>(117842, result.Changers[0].AccountNumber);
            Assert.Equal("CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167", result.Changers[0].NewEncodedPublicKey);
            Assert.Equal("public_key", result.Changers[0].Changes);

            Assert.Equal("Purchased account 117842-82 by 52-11 for 1.0000 PASC", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal(string.Empty, result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("000000003400000059010000D245B2DC95B564C0CF15D5A6A44AD1E797A9E074", result.OpHash);
        }

        [Fact]
        public async Task ChangeAccountInfoNameAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":8,\"subtype\":81,\"account\":32320,\"signer_account\":32320,\"n_operation\":5,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":32320,\"n_operation\":5,\"new_name\":\"newname\",\"changes\":\"account_name\"}],\"optxt\":\"Changed name of account 32320-77\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":0.0000,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0.0000,\"ophash\":\"00000000407E00000500000022BE257C8D811012A01D3A477C830B78AA2A32B8\",\"old_ophash\":\"\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.ChangeAccountInfoAsync(accountTarget: 32320, signerAccount: 32320, newName: "newname");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.ChangeAccountInfo, result.Type);
            Assert.Equal(OperationSubType.ChangeAccountInfo, result.SubType);
            Assert.Equal<uint>(32320, result.AccountNumber);
            Assert.Equal<uint>(32320, result.SignerAccountNumber);
            Assert.Equal<uint>(5, result.NOperation);

            Assert.Empty(result.Senders);
            Assert.Empty(result.Receivers);
            Assert.Single(result.Changers);
            Assert.Equal<uint>(32320, result.Changers[0].AccountNumber);
            Assert.Equal<uint>(5, result.Changers[0].NOperation);
            Assert.Equal("newname", result.Changers[0].NewName);
            Assert.Equal("account_name", result.Changers[0].Changes);

            Assert.Equal("Changed name of account 32320-77", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal(string.Empty, result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("00000000407E00000500000022BE257C8D811012A01D3A477C830B78AA2A32B8", result.OpHash);
        }

        [Fact]
        public async Task ChangeAccountInfoTypeAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":8,\"subtype\":81,\"account\":32321,\"signer_account\":32321,\"n_operation\":2,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":32321,\"n_operation\":2,\"new_type\":1,\"changes\":\"account_type\"}],\"optxt\":\"Changed type of account 32321-89\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":0.0000,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0.0000,\"ophash\":\"00000000417E000002000000E690F6536D8EA30E800E8AACCCF277E42153EE3A\",\"old_ophash\":\"\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.ChangeAccountInfoAsync(accountTarget: 32321, signerAccount: 32321, newType: 1);
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.ChangeAccountInfo, result.Type);
            Assert.Equal(OperationSubType.ChangeAccountInfo, result.SubType);
            Assert.Equal<uint>(32321, result.AccountNumber);
            Assert.Equal<uint>(32321, result.SignerAccountNumber);
            Assert.Equal<uint>(2, result.NOperation);

            Assert.Empty(result.Senders);
            Assert.Empty(result.Receivers);
            Assert.Single(result.Changers);
            Assert.Equal<uint>(32321, result.Changers[0].AccountNumber);
            Assert.Equal<uint>(2, result.Changers[0].NOperation);
            Assert.Equal(1, result.Changers[0].NewType);
            Assert.Equal("account_type", result.Changers[0].Changes);

            Assert.Equal("Changed type of account 32321-89", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal(string.Empty, result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("00000000417E000002000000E690F6536D8EA30E800E8AACCCF277E42153EE3A", result.OpHash);
        }

        [Fact]
        public async Task ChangeAccountInfoPublicKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"block\":0,\"time\":0,\"opblock\":-1,\"maturation\":null,\"optype\":8,\"subtype\":81,\"account\":32321,\"signer_account\":32321,\"n_operation\":3,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":32321,\"n_operation\":3,\"new_enc_pubkey\":\"CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167\",\"changes\":\"public_key\"}],\"optxt\":\"Changed key of account 32321-89\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":0.0000,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"balance\":0.0000,\"ophash\":\"00000000417E000003000000F355BFDF5B2A0FC59C6A4555B395133D77616803\",\"old_ophash\":\"\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.ChangeAccountInfoAsync(accountTarget: 32321, signerAccount: 32321, newB58PublicKey: "3GhhbouLD4QsBCZbL3ozd6zBaQQYWgoHLPKLmrby5n4MgN6HknS2q825dVvZg6qJEFrikQdoiNbw5TsWJdYPZhCKTRomcSePRS7gE9");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint?>(0, result.BlockNumber);
            Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), result.Time);
            Assert.Equal(-1, result.Index);
            Assert.Null(result.Maturation);
            Assert.Equal(OperationType.ChangeAccountInfo, result.Type);
            Assert.Equal(OperationSubType.ChangeAccountInfo, result.SubType);
            Assert.Equal<uint>(32321, result.AccountNumber);
            Assert.Equal<uint>(32321, result.SignerAccountNumber);
            Assert.Equal<uint>(3, result.NOperation);

            Assert.Empty(result.Senders);
            Assert.Empty(result.Receivers);
            Assert.Single(result.Changers);
            Assert.Equal<uint>(32321, result.Changers[0].AccountNumber);
            Assert.Equal<uint>(3, result.Changers[0].NOperation);
            Assert.Equal("CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167", result.Changers[0].NewEncodedPublicKey);
            Assert.Equal("public_key", result.Changers[0].Changes);

            Assert.Equal("Changed key of account 32321-89", result.Description);
            Assert.Equal(0, result.Fee);
            Assert.Equal(0, result.Amount);
            Assert.Equal(string.Empty, result.Payload);
            Assert.Equal(PayloadType.NonDeterministic, result.PayloadType);
            Assert.Equal("00000000417E000003000000F355BFDF5B2A0FC59C6A4555B395133D77616803", result.OpHash);
        }

        [Fact]
        public async Task SignSendToAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":1,\"amount\":1.0000,\"amount_s\":\"1.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"rawoperations\":\"0100000001000500407E00000E00000034000000102700000000000000000000000000000000000000000000001F00F41A7FE4E858F4124D183714E9AE5300386E65A4D4F7FC846FD51A762E58E42000A02568AF9B5F50A92202A2422AA220826CD2E19E67B4CFA4B4BBD3724A1BD239\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.SignSendToAsync(senderAccount: 38764, receiverAccount: 52, amount: 1, lastNOperation: 7, fee: 0,
                senderB58PublicKey: "3GhhbouLD4QsBCZbL3ozd6zBaQQYWgoHLPKLmrby5n4MgN6HknS2q825dVvZg6qJEFrikQdoiNbw5TsWJdYPZhCKTRomcSePRS7gE9",
                targetB58PublicKey: "JJj2GZDdgUzFV7UAs27znTVpRdYon49xZwQcrxUjymDb7qzGFAHLjxCEcPLdyCpZXQjXuHF5izgfxhpqWB86pALNAcg5dtbSPwNSp6QJLdjkZJxHahoe5TbhcKZeJ6MsG3MWwowmWtRJZvND64cQUSbEBaENqcWcRznm823xkbyR2QrVSbQ8ypnRSXsakrDdv");
            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(1, result.OperationsCount);
            Assert.Equal(1, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("0100000001000500407E00000E00000034000000102700000000000000000000000000000000000000000000001F00F41A7FE4E858F4124D183714E9AE5300386E65A4D4F7FC846FD51A762E58E42000A02568AF9B5F50A92202A2422AA220826CD2E19E67B4CFA4B4BBD3724A1BD239", result.RawOperations);
        }

        [Fact]
        public async Task SignDataUnencryptedAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":1,\"amount\":0,\"amount_s\":\"0.0000\",\"fee\":0,\"fee_s\":\"0.0000\",\"rawoperations\":\"010000000A000500407E0000407E000034000000090000008797A1AA47F8234389872E652F593BCE010001000000000000000000000000000000000012470021100C001000031FB6D4C719B961F286E6E571FDB765BFD450013998ED4C13167BBB0C735A6B22E48C40F8D1CE85C123A1D253DD98E36F11318D1EBD0664690019B53468BB1313200020A0518A52B6009230F15BEEC512AF99D68F67566A97AAF8F5E57517B2AC3BDC20000BE6C6CCF433ED1AEFB7869570EEB703023CDE9DCA000A64BE99BF37408F35D8\"},\"id\":100,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.SignDataAsync(signerAccount: 32320, senderAccount: 32320, receiverAccount: 52, guid: "AAA19787-F847-4323-8987-2E652F593BCE", lastNOperation: 8, dataType: DataType.PrivateMessage,
                dataSequence: 1, fee: 0, payload: "Hello world!", payloadMethod: PayloadMethod.None,
                signerB58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(1, result.OperationsCount);
            Assert.Equal(0, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("010000000A000500407E0000407E000034000000090000008797A1AA47F8234389872E652F593BCE010001000000000000000000000000000000000012470021100C001000031FB6D4C719B961F286E6E571FDB765BFD450013998ED4C13167BBB0C735A6B22E48C40F8D1CE85C123A1D253DD98E36F11318D1EBD0664690019B53468BB1313200020A0518A52B6009230F15BEEC512AF99D68F67566A97AAF8F5E57517B2AC3BDC20000BE6C6CCF433ED1AEFB7869570EEB703023CDE9DCA000A64BE99BF37408F35D8", result.RawOperations);
        }
        [Fact]
        public async Task SignDataDestinationEncryptedAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":1,\"amount\":0.0000,\"amount_s\":\"0.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"rawoperations\":\"010000000A000500407E0000407E0000340000000B0000008797A1AA47F8234389872E652F593BCE010001000000000000000000000000000000000012690043100C001000030045BF2281F9E2180203226394F33B9F6595F408F38D37410165275FA9E394C829BC87D48F4172393092E987ACFC981093A77FF5DFF4655010247A9360FE46CB103DAEFD14A6E854B7DD6F470901F2A047AF4A68F9CFF25F0970D6957A506F25D4B72000FBF4A671ABE6E0E264D21D99B5A6BDC2CFA325B5DF866A5A676168F97DAC6060200023040EBB32C11F35712AEEAD1E0FD02538D9748D168135B58EB740415B7FF1F0\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.SignDataAsync(signerAccount: 32320, senderAccount: 32320, receiverAccount: 52, guid: "AAA19787-F847-4323-8987-2E652F593BCE", lastNOperation: 8, dataType: DataType.PrivateMessage,
                dataSequence: 1, fee: 0, payload: "Hello world!", payloadMethod: PayloadMethod.Dest,
                signerB58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8",
                receiverB58PublicKey: "JJj2GZDdgUzFV7UAs27znTVpRdYon49xZwQcrxUjymDb7qzGFAHLjxCEcPLdyCpZXQjXuHF5izgfxhpqWB86pALNAcg5dtbSPwNSp6QJLdjkZJxHahoe5TbhcKZeJ6MsG3MWwowmWtRJZvND64cQUSbEBaENqcWcRznm823xkbyR2QrVSbQ8ypnRSXsakrDdv");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(1, result.OperationsCount);
            Assert.Equal(0, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("010000000A000500407E0000407E0000340000000B0000008797A1AA47F8234389872E652F593BCE010001000000000000000000000000000000000012690043100C001000030045BF2281F9E2180203226394F33B9F6595F408F38D37410165275FA9E394C829BC87D48F4172393092E987ACFC981093A77FF5DFF4655010247A9360FE46CB103DAEFD14A6E854B7DD6F470901F2A047AF4A68F9CFF25F0970D6957A506F25D4B72000FBF4A671ABE6E0E264D21D99B5A6BDC2CFA325B5DF866A5A676168F97DAC6060200023040EBB32C11F35712AEEAD1E0FD02538D9748D168135B58EB740415B7FF1F0", result.RawOperations);
        }
        [Fact]
        public async Task SignDataSenderEncryptedAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":1,\"amount\":0.0000,\"amount_s\":\"0.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"rawoperations\":\"010000000A000500407E0000407E0000340000000C0000008797A1AA47F8234389872E652F593BCE010001000000000000000000000000000000000014470021100C0010000221B6FB6CAA8FC81256E7E1E1F50201026941391ECC3A2804C45E0AE34A30E77C6B489981541A7426296F7777F7444035356AD2108D32391B6765905A426EF96220002516402C8C07E9C7F04B6134542D7A71733F90162E7EDB2DC81617CBD0CE84AA2000654AB1A991D1F4D106D8600DF39444B845541CDA0AF6C0AF3EEAF20D5006A755\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.SignDataAsync(signerAccount: 32320, senderAccount: 32320, receiverAccount: 52, guid: "AAA19787-F847-4323-8987-2E652F593BCE", lastNOperation: 8, dataType: DataType.PrivateMessage,
                dataSequence: 1, fee: 0, payload: "Hello world!", payloadMethod: PayloadMethod.Dest,
                signerB58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8",
                receiverB58PublicKey: "JJj2GZDdgUzFV7UAs27znTVpRdYon49xZwQcrxUjymDb7qzGFAHLjxCEcPLdyCpZXQjXuHF5izgfxhpqWB86pALNAcg5dtbSPwNSp6QJLdjkZJxHahoe5TbhcKZeJ6MsG3MWwowmWtRJZvND64cQUSbEBaENqcWcRznm823xkbyR2QrVSbQ8ypnRSXsakrDdv");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(1, result.OperationsCount);
            Assert.Equal(0, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("010000000A000500407E0000407E0000340000000C0000008797A1AA47F8234389872E652F593BCE010001000000000000000000000000000000000014470021100C0010000221B6FB6CAA8FC81256E7E1E1F50201026941391ECC3A2804C45E0AE34A30E77C6B489981541A7426296F7777F7444035356AD2108D32391B6765905A426EF96220002516402C8C07E9C7F04B6134542D7A71733F90162E7EDB2DC81617CBD0CE84AA2000654AB1A991D1F4D106D8600DF39444B845541CDA0AF6C0AF3EEAF20D5006A755", result.RawOperations);
        }

        [Fact]
        public async Task SignChangeKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":1,\"amount\":0.0000,\"amount_s\":\"0.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"rawoperations\":\"0100000002000500385B01000100000000000000000000000000000000000000008900CC024200011FB91A8721A056D9032EBE067BD3A5E764E5BB2B78352A76D4BA7B87A67EBF2E8EB2DEF3D67588B8F101120803248DEF2882C1B0667CDA9BA6A696E569D0CC63764100D3A978D8BEF49EDA23F79DB2F2258EDE022D79E17ECF6490B8892C2C4FC8BCA45BEC3B2AD31FB6A8C10364F1A88E58236BED76F517006927AA769FF1D27D58E2E12000B5ED15DF49B33BC83CBFC8D2977C275AD8C482FD510DF0ED4199474467552B6420006BFD60224C887CBD74857088A2DF5CFD8FDA90F419450F142BAE74ADF4BB2E70\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.SignChangeKeyAsync(account: 88888, lastNOperation: 0, oldB58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8",
                newB58PublicKey: "JJj2GZDdgUzFV7UAs27znTVpRdYon49xZwQcrxUjymDb7qzGFAHLjxCEcPLdyCpZXQjXuHF5izgfxhpqWB86pALNAcg5dtbSPwNSp6QJLdjkZJxHahoe5TbhcKZeJ6MsG3MWwowmWtRJZvND64cQUSbEBaENqcWcRznm823xkbyR2QrVSbQ8ypnRSXsakrDdv");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(1, result.OperationsCount);
            Assert.Equal(0, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("0100000002000500385B01000100000000000000000000000000000000000000008900CC024200011FB91A8721A056D9032EBE067BD3A5E764E5BB2B78352A76D4BA7B87A67EBF2E8EB2DEF3D67588B8F101120803248DEF2882C1B0667CDA9BA6A696E569D0CC63764100D3A978D8BEF49EDA23F79DB2F2258EDE022D79E17ECF6490B8892C2C4FC8BCA45BEC3B2AD31FB6A8C10364F1A88E58236BED76F517006927AA769FF1D27D58E2E12000B5ED15DF49B33BC83CBFC8D2977C275AD8C482FD510DF0ED4199474467552B6420006BFD60224C887CBD74857088A2DF5CFD8FDA90F419450F142BAE74ADF4BB2E70", result.RawOperations);
        }

        [Fact]
        public async Task SignListAccountForSaleAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":1,\"amount\":0.0000,\"amount_s\":\"0.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"rawoperations\":\"010000000400050050CC010050CC0100040006000000400D03000000000034000000000000000000060000000000000000000000020020000000000000000000000000000000000000000000000000000000000000000000000000000000000000000020009D8D6B9F4B2134654EB9DE8A20C3F22E15434BA64FB467A19C77E4F09183DB6920009B8D15473D83FD8712C179D061D513122B0CE0367F376DF006E94719A3B16B7A\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.SignListAccountForSaleAsync(accountForSale: 117840, sellerAccount: 52, price: 20, lastNOperation: 5, signerAccount: 117840,
                signerB58PublicKey: "3GhhbouLD4QsBCZbL3ozd6zBaQQYWgoHLPKLmrby5n4MgN6HknS2q825dVvZg6qJEFrikQdoiNbw5TsWJdYPZhCKTRomcSePRS7gE9");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(1, result.OperationsCount);
            Assert.Equal(0, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("010000000400050050CC010050CC0100040006000000400D03000000000034000000000000000000060000000000000000000000020020000000000000000000000000000000000000000000000000000000000000000000000000000000000000000020009D8D6B9F4B2134654EB9DE8A20C3F22E15434BA64FB467A19C77E4F09183DB6920009B8D15473D83FD8712C179D061D513122B0CE0367F376DF006E94719A3B16B7A", result.RawOperations);
        }

        [Fact]
        public async Task SignDelistAccountForSaleAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":1,\"amount\":0.0000,\"amount_s\":\"0.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"rawoperations\":\"010000000500050050CC010050CC010005000300000000000000000000000000002000B79A3B04C46AA8F0E21106BA2D2C2D505729A147B68142670E571458D5AF65232000819D256C0C2374BD64442EA27ECA492F0E8050EC25863A40D142982227F5B559\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.SignDelistAccountForSaleAsync(accountNumber: 117840, signerAccount: 117840, lastNOperation: 2,
                signerB58PublicKey: "3GhhbouLD4QsBCZbL3ozd6zBaQQYWgoHLPKLmrby5n4MgN6HknS2q825dVvZg6qJEFrikQdoiNbw5TsWJdYPZhCKTRomcSePRS7gE9");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(1, result.OperationsCount);
            Assert.Equal(0, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("010000000500050050CC010050CC010005000300000000000000000000000000002000B79A3B04C46AA8F0E21106BA2D2C2D505729A147B68142670E571458D5AF65232000819D256C0C2374BD64442EA27ECA492F0E8050EC25863A40D142982227F5B559", result.RawOperations);
        }

        [Fact]
        public async Task SignBuyAccountAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":1,\"amount\":1.0000,\"amount_s\":\"1.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"rawoperations\":\"0100000006000500340000005D010000F049020010270000000000000000000000000000000000000000000000021027000000000000407E0000CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167410041A70B3B7E9320EEC75D034E9F75A680AD8F2BC225B6BD93D6CD4CD1715F380EC2BBCE95A6A39E30730F69EAC06154CE85D65E1AC291A1C45A2C9B493FCDBCC9B2420001C1699A0FE390ECA3ACBC174BC1785752A5EECB25D3BA8F5C209390024F6DDBCD450D096798D2AE9C6A762064B52D0AF7A098DD9BFE8FBEA97040363EC3FEE70279\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.SignBuyAccountAsync(buyerAccount: 52, accountToPurchase: 150000, sellerAccount: 32320, price: 1, amount: 1, lastNOperation: 348,
                newB58PublicKey: "3GhhbouLD4QsBCZbL3ozd6zBaQQYWgoHLPKLmrby5n4MgN6HknS2q825dVvZg6qJEFrikQdoiNbw5TsWJdYPZhCKTRomcSePRS7gE9",
                signerB58PublicKey: "JJj2GZDdgUzFV7UAs27znTVpRdYon49xZwQcrxUjymDb7qzGFAHLjxCEcPLdyCpZXQjXuHF5izgfxhpqWB86pALNAcg5dtbSPwNSp6QJLdjkZJxHahoe5TbhcKZeJ6MsG3MWwowmWtRJZvND64cQUSbEBaENqcWcRznm823xkbyR2QrVSbQ8ypnRSXsakrDdv");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(1, result.OperationsCount);
            Assert.Equal(1, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("0100000006000500340000005D010000F049020010270000000000000000000000000000000000000000000000021027000000000000407E0000CA022000DC8EFC0C6A63B08BE93D4C8511052A4BE709DF728CCB5A047E29E921F76EA13B20003B77BAAE6461E66BEF8575CBB588C15982118B1AFD8DE0BF2753CEE4E760E167410041A70B3B7E9320EEC75D034E9F75A680AD8F2BC225B6BD93D6CD4CD1715F380EC2BBCE95A6A39E30730F69EAC06154CE85D65E1AC291A1C45A2C9B493FCDBCC9B2420001C1699A0FE390ECA3ACBC174BC1785752A5EECB25D3BA8F5C209390024F6DDBCD450D096798D2AE9C6A762064B52D0AF7A098DD9BFE8FBEA97040363EC3FEE70279", result.RawOperations);
        }

        [Fact]
        public async Task SignChangeAccountInfoAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":1,\"amount\":0.0000,\"amount_s\":\"0.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"rawoperations\":\"0100000008000500417E0000417E00000300000000000000000000000000000000000000000200000000000004006E616D6500000000200089BE5D0EE80EFA9C3DDB7453E0BE87FD6437CE6651E14F4D3BDB09F61F489E10200070A6A1BC0E40E5E574108CD66C883DC26042CB316488B1E634F5A7C968D3635E\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.SignChangeAccountInfoAsync(accountTarget: 32321, signerAccount: 32321, lastNOperation: 2, newName: "name",
                signerB58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(1, result.OperationsCount);
            Assert.Equal(0, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("0100000008000500417E0000417E00000300000000000000000000000000000000000000000200000000000004006E616D6500000000200089BE5D0EE80EFA9C3DDB7453E0BE87FD6437CE6651E14F4D3BDB09F61F489E10200070A6A1BC0E40E5E574108CD66C883DC26042CB316488B1E634F5A7C968D3635E", result.RawOperations);
        }

        [Fact]
        public async Task OperationsInfoAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"block\":0,\"time\":0,\"opblock\":0,\"maturation\":null,\"optype\":8,\"subtype\":81,\"account\":32321,\"signer_account\":32321,\"n_operation\":10,\"senders\":[],\"receivers\":[],\"changers\":[{\"account\":32321,\"n_operation\":10,\"new_name\":\"name3\",\"changes\":\"account_name\"}],\"optxt\":\"Changed name of account 32321-89\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"amount\":0.0000,\"amount_s\":\"0.0000\",\"payload\":\"\",\"payload_type\":0,\"ophash\":\"00000000417E00000A000000CDCFA9B954E05548DF877CF3855561AA9141C941\",\"old_ophash\":\"\"}],\"id\":2,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.OperationsInfoAsync("0100000008000500417E0000417E00000A00000000000000000000000000000000000000000200000000000005006E616D653300000000200003AB5101A9B84B19B8AD4697D9CE92BD9BDFD3127C9F71C7C356B5D49DE324B92000EF5F290E79F972B76393E63A7E2D8D8C0FF1B7CA26F9847D14A4087327A38DE6");

            Assert.Null(response.Error);
            Assert.NotNull(response.Result);
        }

        [Fact]
        public async Task NodeStatusAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"ready\":true,\"ready_s\":\"\",\"status_s\":\"Discovering servers\",\"port\":4204,\"locked\":false,\"timestamp\":1616744180,\"version\":\"TESTNET 5.4\",\"netprotocol\":{\"ver\":12,\"ver_a\":12},\"blocks\":32117,\"sbh\":\"338F8DE4F17BFA8891FD5C9FFFE61BA91666299311E15AAB727ADD7AADD5C420\",\"pow\":\"0000398ACB55A6091BEB7C03B724E0C30F19FC764A218CF412C9CBFCA1F4E081\",\"netstats\":{\"active\":1,\"connectors\":0,\"servers\":1,\"servers_t\":1,\"total\":2,\"tconnectors\":0,\"tservers\":2,\"breceived\":407313,\"bsend\":2744122,\"ips\":1},\"openssl\":\"101010AF\",\"nodeservers\":[{\"ip\":\"136.244.108.43\",\"port\":4204,\"lastcon\":1616744175,\"attempts\":0}],\"datafolder\":\"C:\\\\Users\\\\ape\\\\AppData\\\\Roaming\\\\PascalCoin_TESTNET\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.NodeStatusAsync();

            Assert.Null(response.Error);
            Assert.NotNull(response.Result);
            //TODO: can add more asserts
        }

        [Fact]
        public async Task EncodePublicKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":\"CA0220000E60B6F76778CFE8678E30369BA7B2C38D0EC93FC3F39E61468E29FEC39F13BF2000572EDE3C44CF00FF86AFF651474D53CCBDF86B953F1ECE5FB8FC7BB6FA16F114\",\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.EncodePublicKeyAsync(EncryptionType.Secp256k1, "0E60B6F76778CFE8678E30369BA7B2C38D0EC93FC3F39E61468E29FEC39F13BF", "572EDE3C44CF00FF86AFF651474D53CCBDF86B953F1ECE5FB8FC7BB6FA16F114");

            Assert.Null(response.Error);
            Assert.Equal("CA0220000E60B6F76778CFE8678E30369BA7B2C38D0EC93FC3F39E61468E29FEC39F13BF2000572EDE3C44CF00FF86AFF651474D53CCBDF86B953F1ECE5FB8FC7BB6FA16F114", response.Result);
        }

        [Fact]
        public async Task DecodePublicKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"ec_nid\":714,\"x\":\"0E60B6F76778CFE8678E30369BA7B2C38D0EC93FC3F39E61468E29FEC39F13BF\",\"y\":\"572EDE3C44CF00FF86AFF651474D53CCBDF86B953F1ECE5FB8FC7BB6FA16F114\",\"enc_pubkey\":\"CA0220000E60B6F76778CFE8678E30369BA7B2C38D0EC93FC3F39E61468E29FEC39F13BF2000572EDE3C44CF00FF86AFF651474D53CCBDF86B953F1ECE5FB8FC7BB6FA16F114\",\"b58_pubkey\":\"3GhhbokGhwwor3R9umFLq4DhoSuLkJAoWfizpj7adrBuFDmfx6QBToBXJS4W7PyQ2k6HkhurMpQhBBSsKP4ub9v3mFHNvs4V2eeE54\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.DecodePublicKeyAsync(encodedPublicKey: "CA0220000E60B6F76778CFE8678E30369BA7B2C38D0EC93FC3F39E61468E29FEC39F13BF2000572EDE3C44CF00FF86AFF651474D53CCBDF86B953F1ECE5FB8FC7BB6FA16F114");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal("CA0220000E60B6F76778CFE8678E30369BA7B2C38D0EC93FC3F39E61468E29FEC39F13BF2000572EDE3C44CF00FF86AFF651474D53CCBDF86B953F1ECE5FB8FC7BB6FA16F114", result.EncodedPublicKey);
            Assert.Equal("3GhhbokGhwwor3R9umFLq4DhoSuLkJAoWfizpj7adrBuFDmfx6QBToBXJS4W7PyQ2k6HkhurMpQhBBSsKP4ub9v3mFHNvs4V2eeE54", result.B58PublicKey);
            Assert.Equal(EncryptionType.Secp256k1, result.EncryptionNid);
            Assert.Equal("0E60B6F76778CFE8678E30369BA7B2C38D0EC93FC3F39E61468E29FEC39F13BF", result.X);
            Assert.Equal("572EDE3C44CF00FF86AFF651474D53CCBDF86B953F1ECE5FB8FC7BB6FA16F114", result.Y);
        }

        [Fact]
        public async Task PayloadEncryptUsingPasswordAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":\"53616C7465645F5F7704130B9533A72BFBB085E30D3B0B16FA07B0854ED6493A\",\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.PayloadEncryptAsync(payload: "DODO", method: AbstractPayloadMethod.Aes, password: "mypassword");

            Assert.Null(response.Error);
            Assert.Equal("53616C7465645F5F7704130B9533A72BFBB085E30D3B0B16FA07B0854ED6493A", response.Result);
        }
        [Fact]
        public async Task PayloadEncryptUsingEncodedPublicKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":\"211004001000034B1CCC561512D4D1F7677511FDAAC2F053010C46C5148879ACA67F1D115BB0CA065223A8F43686F83F15EB9F64C082134F37BCF033E41F6694CE3897AFED3AAF\",\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.PayloadEncryptAsync(payload: "DODO", method: AbstractPayloadMethod.PubKey, encodedPublicKey: "CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB");

            Assert.Null(response.Error);
            Assert.Equal("211004001000034B1CCC561512D4D1F7677511FDAAC2F053010C46C5148879ACA67F1D115BB0CA065223A8F43686F83F15EB9F64C082134F37BCF033E41F6694CE3897AFED3AAF", response.Result);
        }

        [Fact]
        public async Task PayloadDecryptUsingPasswordAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"unenc_payload\":\"DODO\",\"unenc_hexpayload\":\"444F444F\",\"payload_method\":\"pwd\",\"pwd\":\"mypassword\",\"result\":true,\"enc_payload\":\"53616C7465645F5F8312C92E9BFFD6068ADA9F2F7CEA90505B50CE2CAE995C28\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.PayloadDecryptAsync("53616C7465645F5F8312C92E9BFFD6068ADA9F2F7CEA90505B50CE2CAE995C28", "mypassword");

            Assert.Null(response.Error);

            var result = response.Result;
            Assert.Equal("DODO", result.UnencryptedPayload);
            Assert.Equal("444F444F", result.UnencryptedPayloadHexastring);
            Assert.Equal("pwd", result.PayloadMethod);
            Assert.Null(result.EncodedPublicKey);
            Assert.Equal("mypassword", result.Password);
            Assert.True(result.Result);
            Assert.Equal("53616C7465645F5F8312C92E9BFFD6068ADA9F2F7CEA90505B50CE2CAE995C28", result.EncryptedPayload);
        }
        [Fact]
        public async Task PayloadDecryptUsingWalletPrivateKeysAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"unenc_payload\":\"DODO\",\"unenc_hexpayload\":\"444F444F\",\"payload_method\":\"key\",\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"result\":true,\"enc_payload\":\"21100400100003E2978A4DD3A11B638BD2A31D404FA941A9C4CDA0FFAF90B601F50FC1777A0F80C5440FBCCC08FF121567467E4EB46BF6F7B54250E580E1CA01F8924069CC6219\"},\"id\":2,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.PayloadDecryptAsync("21100400100003E2978A4DD3A11B638BD2A31D404FA941A9C4CDA0FFAF90B601F50FC1777A0F80C5440FBCCC08FF121567467E4EB46BF6F7B54250E580E1CA01F8924069CC6219");

            Assert.Null(response.Error);

            var result = response.Result;
            Assert.Equal("DODO", result.UnencryptedPayload);
            Assert.Equal("444F444F", result.UnencryptedPayloadHexastring);
            Assert.Equal("key", result.PayloadMethod);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", result.EncodedPublicKey);
            Assert.Null(result.Password);
            Assert.True(result.Result);
            Assert.Equal("21100400100003E2978A4DD3A11B638BD2A31D404FA941A9C4CDA0FFAF90B601F50FC1777A0F80C5440FBCCC08FF121567467E4EB46BF6F7B54250E580E1CA01F8924069CC6219", result.EncryptedPayload);
        }

        [Fact]
        public async Task GetConnectionsAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":[{\"server\":true,\"ip\":\"136.244.108.43\",\"port\":4204,\"secs\":6896,\"sent\":5005329,\"recv\":592171,\"appver\":\"TESTNET 5.4amLf64bof\",\"netver\":12,\"netver_a\":12,\"timediff\":-86}],\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.GetConnectionsAsync();

            Assert.Null(response.Error);
            Assert.Single(response.Result);

            var result = response.Result[0];

            Assert.True(result.Server);
            Assert.Equal("136.244.108.43", result.Ip);
            Assert.Equal<uint>(4204, result.Port);
            Assert.Equal<uint>(6896, result.Seconds);
            Assert.Equal<uint>(5005329, result.BytesSent);
            Assert.Equal<uint>(592171, result.BytesReceived);
            Assert.Equal("TESTNET 5.4amLf64bof", result.AppVersion);
            Assert.Equal<uint>(12, result.NetVersion);
            Assert.Equal<uint>(12, result.NetAvailableVersion);
            Assert.Equal(-86, result.TimeDifference);
        }

        [Fact]
        public async Task AddNewKeyAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"ec_nid\":714,\"x\":\"D646B767D1A0E912CE223952709E6D910E32BA7C0AB2DD0FA3460C707BA0C399\",\"y\":\"29333E49D9E84D6CC4AEAA18C1F186B39016D9F7A9FA3EF057C2F8C6F922518A\",\"enc_pubkey\":\"CA022000D646B767D1A0E912CE223952709E6D910E32BA7C0AB2DD0FA3460C707BA0C399200029333E49D9E84D6CC4AEAA18C1F186B39016D9F7A9FA3EF057C2F8C6F922518A\",\"b58_pubkey\":\"3Ghhbou4CN9qLBYHrmyHMszJji7nBDo33y7h42CBo4rR6hRczvfbSH8PPZCg8gwkRgxfZFrSic5xPnyyc5meAXRjHb6hfGanGkXQFx\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.AddNewKeyAsync(EncryptionType.Secp256k1, "TestKey");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal(EncryptionType.Secp256k1, result.EncryptionNid);
            Assert.Equal("D646B767D1A0E912CE223952709E6D910E32BA7C0AB2DD0FA3460C707BA0C399", result.X);
            Assert.Equal("29333E49D9E84D6CC4AEAA18C1F186B39016D9F7A9FA3EF057C2F8C6F922518A", result.Y);
            Assert.Equal("CA022000D646B767D1A0E912CE223952709E6D910E32BA7C0AB2DD0FA3460C707BA0C399200029333E49D9E84D6CC4AEAA18C1F186B39016D9F7A9FA3EF057C2F8C6F922518A", result.EncodedPublicKey);
            Assert.Equal("3Ghhbou4CN9qLBYHrmyHMszJji7nBDo33y7h42CBo4rR6hRczvfbSH8PPZCg8gwkRgxfZFrSic5xPnyyc5meAXRjHb6hfGanGkXQFx", result.B58PublicKey);
        }

        [Fact]
        public async Task LockAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":false,\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.LockAsync();

            Assert.Null(response.Error);
            Assert.False(response.Result); //test wallet was not password protected therefore lock should not be successful
        }

        [Fact]
        public async Task UnlockAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":true,\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.UnlockAsync("test");

            Assert.Null(response.Error);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task SetWalletPasswordAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":true,\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.SetWalletPasswordAsync("test");

            Assert.Null(response.Error);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task StopNodeAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":true,\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.StopNodeAsync();

            Assert.Null(response.Error);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task StartNodeAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":true,\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.StartNodeAsync();

            Assert.Null(response.Error);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task SignMessageAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"digest\":\"4D657373616765\",\"enc_pubkey\":\"CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB\",\"signature\":\"2000A7E8DBC16710D79155CC0F8DD91F94446E18B066D0E0BFF984C43EC83FC69BB520006293C2D4CBC924D23BD14AEE287E8DB6C29A824150D5A047A9CD9D229758F439\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));
            var response = await connector.SignMessageAsync(digest: "Message", b58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8");

            Assert.Null(response.Error);
            Assert.Equal("4D657373616765", response.Result.DigestHexaString);
            Assert.Equal("CA022000662084946291B2620108EBD6A0653B742E3673529751FF6BB565D9F47D920ADA200005CDF25090FFFA9A72181D13E457C7CF061CCAF4D4618EBCF9EA1D124E39EDCB", response.Result.EncodedPublicKey);
            Assert.Equal("2000A7E8DBC16710D79155CC0F8DD91F94446E18B066D0E0BFF984C43EC83FC69BB520006293C2D4CBC924D23BD14AEE287E8DB6C29A824150D5A047A9CD9D229758F439", response.Result.Signature);
        }

        [Fact]
        public async Task VerifySignedMessageAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.Content.ReadAsStringAsync().Result.Contains("verifysign")), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"digest\":\"4D657373616765\",\"enc_pubkey\":\"CA022000A09FE9F5916F442FD28D4C9E86F7B4D410269BC1BCF295E7DC42D8BD06978A3E20001116BD43863A449C21A36E512FEF1076AD59A7389E40F9C7483FF501E3EC8441\",\"signature\":\"20008448306B9549D798D873A10E6BA8BA101099C92B992903A835ED4A177C29848120000B34DB7160A776CB95F7AFF0BFA2EBAB4685B71ED9C63D6ECF922BE29D23A147\"},\"id\":2,\"jsonrpc\":\"2.0\"}")
               });
            //if in calling 'verifysign' is provided public key in b58 format, then is used additional RPC call 'decodepubkey' to get public key in encoded format.
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.Is<HttpRequestMessage>(x => x.Content.ReadAsStringAsync().Result.Contains("decodepubkey")), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"ec_nid\":714,\"x\":\"A09FE9F5916F442FD28D4C9E86F7B4D410269BC1BCF295E7DC42D8BD06978A3E\",\"y\":\"1116BD43863A449C21A36E512FEF1076AD59A7389E40F9C7483FF501E3EC8441\",\"enc_pubkey\":\"CA022000A09FE9F5916F442FD28D4C9E86F7B4D410269BC1BCF295E7DC42D8BD06978A3E20001116BD43863A449C21A36E512FEF1076AD59A7389E40F9C7483FF501E3EC8441\",\"b58_pubkey\":\"3GhhborhTCvmruFryDD6kCnteRmFaYs8XTvsS47uwQLnWa85RLwTHZnHN3QMy2Y97wjSmcXhkpP98c8peXXqELJDMBvoW5K59AnvDP\"},\"id\":2,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.VerifySignedMessageAsync(digest: "Message", b58PublicKey: "3GhhborhTCvmruFryDD6kCnteRmFaYs8XTvsS47uwQLnWa85RLwTHZnHN3QMy2Y97wjSmcXhkpP98c8peXXqELJDMBvoW5K59AnvDP",
                signature: "20008448306B9549D798D873A10E6BA8BA101099C92B992903A835ED4A177C29848120000B34DB7160A776CB95F7AFF0BFA2EBAB4685B71ED9C63D6ECF922BE29D23A147");

            Assert.Null(response.Error);
            Assert.True(response.Result);
        }

        [Fact]
        public async Task MultioperationAddOperationAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"rawoperations\":\"010000000900050005000200407E0000112700000000000020000000000500746573743100000000417E0000102700000000000014000000000500746573743200000000010034000000204E00000000000000050074657374330000\",\"senders\":[{\"account\":32320,\"n_operation\":32,\"amount\":-1.0001,\"payload\":\"7465737431\",\"payload_type\":0},{\"account\":32321,\"n_operation\":20,\"amount\":-1.0000,\"payload\":\"7465737432\",\"payload_type\":0}],\"receivers\":[{\"account\":52,\"amount\":2.0000,\"payload\":\"7465737433\",\"payload_type\":0}],\"changers\":[],\"amount\":2.0000,\"fee\":0.0001,\"digest\":\"9B289293F9D4912A9350B7921990E434529FDFE1B80827524830AD619330FEBF\",\"senders_count\":2,\"receivers_count\":1,\"changesinfo_count\":0,\"signed_count\":0,\"not_signed_count\":2,\"signed_can_execute\":false},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var senders = new List<Sender>
            {
                new Sender(accountNumber: 32320, amount: 1.0001M, payload: "test1"),
                new Sender(accountNumber: 32321, amount: 1, payload: "test2")
            };
            var receivers = new List<Receiver> { new Receiver(accountNumber: 52, amount: 2, payload: "test3") };
            var response = await connector.MultioperationAddOperationAsync(senders: senders, receivers: receivers);

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal("010000000900050005000200407E0000112700000000000020000000000500746573743100000000417E0000102700000000000014000000000500746573743200000000010034000000204E00000000000000050074657374330000", result.RawOperations);
            Assert.Equal(2, result.Senders.Length);
            Assert.Single(result.Receivers);
            Assert.Empty(result.Changers);
            Assert.Equal(2, result.Amount);
            Assert.Equal(0.0001M, result.Fee);
            Assert.Equal("9B289293F9D4912A9350B7921990E434529FDFE1B80827524830AD619330FEBF", result.Digest);
            Assert.Equal<uint>(2, result.SendersCount);
            Assert.Equal<uint>(1, result.ReceiversCount);
            Assert.Equal<uint>(0, result.ChangesInfoCount);
            Assert.Equal<uint>(0, result.SignedCount);
            Assert.Equal<uint>(2, result.NotSignedCount);
            Assert.False(result.CanExecute);

            var sender1 = result.Senders[0];
            Assert.Equal<uint>(32320, sender1.AccountNumber);
            Assert.Equal<uint>(32, sender1.NOperation.Value);
            Assert.Equal(-1.0001M, sender1.Amount);
            Assert.Equal("7465737431", sender1.Payload);
            Assert.Equal(PayloadType.NonDeterministic, sender1.PayloadType);

            var sender2 = result.Senders[1];
            Assert.Equal<uint>(32321, sender2.AccountNumber);
            Assert.Equal<uint>(20, sender2.NOperation.Value);
            Assert.Equal(-1M, sender2.Amount);
            Assert.Equal("7465737432", sender2.Payload);
            Assert.Equal(PayloadType.NonDeterministic, sender2.PayloadType);

            var receiver = result.Receivers[0];
            Assert.Equal<uint>(52, receiver.AccountNumber);
            Assert.Equal(2M, receiver.Amount);
            Assert.Equal("7465737433", receiver.Payload);
            Assert.Equal(PayloadType.NonDeterministic, receiver.PayloadType);
        }

        [Fact]
        public async Task MultiOperationSignOfflineAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"rawoperations\":\"010000000900050005000200407E00001127000000000000200000000005007465737431200050DCA91A244A28698099274664D16216466BDA1C00DA696A9F8B9C85D21E233320006931B4E43AB7C2D0EBC1853039F421650150000159655609A05C672B2F2D17BA417E0000102700000000000014000000000500746573743220000530FEA1BD7F0487E75F87B567F964E4E33FFB1C33C7B2E06436AE6DC583AE9920002BC48C9494A172F24C27B9F197965191FCE1D1E959E864D9E4B1472F1354DF23010034000000204E00000000000000050074657374330000\",\"senders\":[{\"account\":32320,\"n_operation\":32,\"amount\":-1.0001,\"payload\":\"7465737431\",\"payload_type\":0},{\"account\":32321,\"n_operation\":20,\"amount\":-1.0000,\"payload\":\"7465737432\",\"payload_type\":0}],\"receivers\":[{\"account\":52,\"amount\":2.0000,\"payload\":\"7465737433\",\"payload_type\":0}],\"changers\":[],\"amount\":2.0000,\"fee\":0.0001,\"digest\":\"9B289293F9D4912A9350B7921990E434529FDFE1B80827524830AD619330FEBF\",\"senders_count\":2,\"receivers_count\":1,\"changesinfo_count\":0,\"signed_count\":2,\"not_signed_count\":0,\"signed_can_execute\":true},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var rawOperation = "010000000900050005000200407E0000112700000000000020000000000500746573743100000000417E0000102700000000000014000000000500746573743200000000010034000000204E00000000000000050074657374330000";
            var keys = new List<AccountKeyPair>
            {
                new AccountKeyPair(accountNumber: 32320, b58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8"),
                new AccountKeyPair(accountNumber: 32321, b58PublicKey: "3Ghhbop8Mfdp8P7Ltuwu8nVpNXMAcEc8KSVWQ7ZgyHjYeHpBg8ezuKi1DJw5EoZYKhwbXqFLgb4YnfzJT3yQTupfVorNrtWdpqVEp8")
            };
            var response = await connector.MultiOperationSignOfflineAsync(rawOperation, keys);

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal("010000000900050005000200407E00001127000000000000200000000005007465737431200050DCA91A244A28698099274664D16216466BDA1C00DA696A9F8B9C85D21E233320006931B4E43AB7C2D0EBC1853039F421650150000159655609A05C672B2F2D17BA417E0000102700000000000014000000000500746573743220000530FEA1BD7F0487E75F87B567F964E4E33FFB1C33C7B2E06436AE6DC583AE9920002BC48C9494A172F24C27B9F197965191FCE1D1E959E864D9E4B1472F1354DF23010034000000204E00000000000000050074657374330000", result.RawOperations);
            Assert.Equal(2, result.Senders.Length);
            Assert.Single(result.Receivers);
            Assert.Empty(result.Changers);
            Assert.Equal(2, result.Amount);
            Assert.Equal(0.0001M, result.Fee);
            Assert.Equal("9B289293F9D4912A9350B7921990E434529FDFE1B80827524830AD619330FEBF", result.Digest);
            Assert.Equal<uint>(2, result.SendersCount);
            Assert.Equal<uint>(1, result.ReceiversCount);
            Assert.Equal<uint>(0, result.ChangesInfoCount);
            Assert.Equal<uint>(2, result.SignedCount);
            Assert.Equal<uint>(0, result.NotSignedCount);
            Assert.True(result.CanExecute);

            var sender1 = result.Senders[0];
            Assert.Equal<uint>(32320, sender1.AccountNumber);
            Assert.Equal<uint>(32, sender1.NOperation.Value);
            Assert.Equal(-1.0001M, sender1.Amount);
            Assert.Equal("7465737431", sender1.Payload);
            Assert.Equal(PayloadType.NonDeterministic, sender1.PayloadType);

            var sender2 = result.Senders[1];
            Assert.Equal<uint>(32321, sender2.AccountNumber);
            Assert.Equal<uint>(20, sender2.NOperation.Value);
            Assert.Equal(-1M, sender2.Amount);
            Assert.Equal("7465737432", sender2.Payload);
            Assert.Equal(PayloadType.NonDeterministic, sender2.PayloadType);

            var receiver = result.Receivers[0];
            Assert.Equal<uint>(52, receiver.AccountNumber);
            Assert.Equal(2M, receiver.Amount);
            Assert.Equal("7465737433", receiver.Payload);
            Assert.Equal(PayloadType.NonDeterministic, receiver.PayloadType);
        }

        [Fact]
        public async Task MultioperationSignOnlineAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"rawoperations\":\"010000000900050005000200407E000011270000000000002000000000050074657374312000DA366B31B5F17659B19C5C6C32D8278E9030EBE16C961FDA5F755893CC5DFA672000B61917185F7D9B5A4A0A7F90CACE01EEFF973A66831168DE51F27CE5403FEBA3417E00001027000000000000140000000005007465737432200007D3B911DE3670A76172E586E78A8F69FB3F175AD219F22FAEA6767CDCD3940120009B9B06FE7673C59DBC07280657D4AB491A2489064884FA7C1075498BE73F9F87010034000000204E00000000000000050074657374330000\",\"senders\":[{\"account\":32320,\"n_operation\":32,\"amount\":-1.0001,\"payload\":\"7465737431\",\"payload_type\":0},{\"account\":32321,\"n_operation\":20,\"amount\":-1.0000,\"payload\":\"7465737432\",\"payload_type\":0}],\"receivers\":[{\"account\":52,\"amount\":2.0000,\"payload\":\"7465737433\",\"payload_type\":0}],\"changers\":[],\"amount\":2.0000,\"fee\":0.0001,\"digest\":\"9B289293F9D4912A9350B7921990E434529FDFE1B80827524830AD619330FEBF\",\"senders_count\":2,\"receivers_count\":1,\"changesinfo_count\":0,\"signed_count\":2,\"not_signed_count\":0,\"signed_can_execute\":true},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var response = await connector.MultioperationSignOnlineAsync("010000000900050005000200407E0000112700000000000020000000000500746573743100000000417E0000102700000000000014000000000500746573743200000000010034000000204E00000000000000050074657374330000");

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal("010000000900050005000200407E000011270000000000002000000000050074657374312000DA366B31B5F17659B19C5C6C32D8278E9030EBE16C961FDA5F755893CC5DFA672000B61917185F7D9B5A4A0A7F90CACE01EEFF973A66831168DE51F27CE5403FEBA3417E00001027000000000000140000000005007465737432200007D3B911DE3670A76172E586E78A8F69FB3F175AD219F22FAEA6767CDCD3940120009B9B06FE7673C59DBC07280657D4AB491A2489064884FA7C1075498BE73F9F87010034000000204E00000000000000050074657374330000", result.RawOperations);
            Assert.Equal(2, result.Senders.Length);
            Assert.Single(result.Receivers);
            Assert.Empty(result.Changers);
            Assert.Equal(2, result.Amount);
            Assert.Equal(0.0001M, result.Fee);
            Assert.Equal("9B289293F9D4912A9350B7921990E434529FDFE1B80827524830AD619330FEBF", result.Digest);
            Assert.Equal<uint>(2, result.SendersCount);
            Assert.Equal<uint>(1, result.ReceiversCount);
            Assert.Equal<uint>(0, result.ChangesInfoCount);
            Assert.Equal<uint>(2, result.SignedCount);
            Assert.Equal<uint>(0, result.NotSignedCount);
            Assert.True(result.CanExecute);

            var sender1 = result.Senders[0];
            Assert.Equal<uint>(32320, sender1.AccountNumber);
            Assert.Equal<uint>(32, sender1.NOperation.Value);
            Assert.Equal(-1.0001M, sender1.Amount);
            Assert.Equal("7465737431", sender1.Payload);
            Assert.Equal(PayloadType.NonDeterministic, sender1.PayloadType);

            var sender2 = result.Senders[1];
            Assert.Equal<uint>(32321, sender2.AccountNumber);
            Assert.Equal<uint>(20, sender2.NOperation.Value);
            Assert.Equal(-1M, sender2.Amount);
            Assert.Equal("7465737432", sender2.Payload);
            Assert.Equal(PayloadType.NonDeterministic, sender2.PayloadType);

            var receiver = result.Receivers[0];
            Assert.Equal<uint>(52, receiver.AccountNumber);
            Assert.Equal(2M, receiver.Amount);
            Assert.Equal("7465737433", receiver.Payload);
            Assert.Equal(PayloadType.NonDeterministic, receiver.PayloadType);
        }

        [Fact]
        public async Task OperationsDeleteAsync()
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock.Protected()
               .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("{\"result\":{\"operations\":0,\"amount\":0.0000,\"amount_s\":\"0.0000\",\"fee\":0.0000,\"fee_s\":\"0.0000\",\"rawoperations\":\"00000000\"},\"id\":1,\"jsonrpc\":\"2.0\"}")
               });

            using var connector = new PascalConnector(new Uri("http://127.0.0.1:4003"), new HttpClient(handlerMock.Object));

            var rawOperation = "010000000900050005000200407E0000112700000000000020000000000500746573743100000000417E0000102700000000000014000000000500746573743200000000010034000000204E00000000000000050074657374330000";
            var response = await connector.OperationsDeleteAsync(rawOperation, 0);

            Assert.Null(response.Error);

            var result = response.Result;

            Assert.Equal<uint>(0, result.OperationsCount);
            Assert.Equal(0, result.Amount);
            Assert.Equal(0, result.Fee);
            Assert.Equal("00000000", result.RawOperations);
        }

        
    }
}
