﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ZH.Code.IEX.V2.Helper;
using ZH.Code.IEX.V2.Model.Shared.Response;
using ZH.Code.IEX.V2.Model.Stock.Request;
using ZH.Code.IEX.V2.Model.Stock.Response;

namespace ZH.Code.IEX.V2.Service.Stock
{
    internal class StockService : IStockService
    {
        private readonly string _pk;
        private readonly Executor _executor;

        public StockService(HttpClient client, string pk)
        {
            _pk = pk;
            _executor = new Executor(client);
        }

        public async Task<BalanceSheetResponse> BalanceSheetAsync(string symbol, Period period = Period.Quarter,
            int last = 1)
        {
            const string urlPattern = "stock/[symbol]/balance-sheet/[last]";

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);
            qsb.Add("period", period.ToString().ToLower());

            var pathNvc = new NameValueCollection
            {
                {"symbol", symbol},
                {"last", last.ToString()}
            };

            return await _executor.ExecuteAsync<BalanceSheetResponse>(urlPattern, pathNvc, qsb);
        }

        public async Task<string> BalanceSheetFieldAsync(string symbol, string field, Period period = Period.Quarter,
            int last = 1)
        {
            const string urlPattern = "stock/[symbol]/balance-sheet/[last]/[field]";

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);
            qsb.Add("period", period.ToString().ToLower());

            var pathNvc = new NameValueCollection { { "symbol", symbol }, { "last", last.ToString() }, { "field", field } };

            return await _executor.ExecuteAsync<string>(urlPattern, pathNvc, qsb);
        }

        public async Task<BatchBySymbolResponse> BatchBySymbolAsync(string symbol, IEnumerable<BatchType> types,
            string range = "", int last = 1)
        {
            if (types?.Count() < 1)
            {
                throw new ArgumentNullException(nameof(types));
            }

            const string urlPattern = "stock/[symbol]/batch";

            var qsType = new List<string>();
            foreach (var x in types)
            {
                switch (x)
                {
                    case BatchType.Quote:
                        qsType.Add("quote");
                        break;

                    case BatchType.News:
                        qsType.Add("news");
                        break;

                    case BatchType.Chart:
                        qsType.Add("chart");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(types));
                }
            }

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);
            qsb.Add("types", string.Join(",", qsType));
            if (!string.IsNullOrWhiteSpace(range))
            {
                qsb.Add("range", range);
            }

            qsb.Add("last", last);

            var pathNvc = new NameValueCollection { { "symbol", symbol } };

            return await _executor.ExecuteAsync<BatchBySymbolResponse>(urlPattern, pathNvc, qsb);
        }

        public async Task<Dictionary<string, BatchBySymbolResponse>> BatchByMarketAsync(IEnumerable<string> symbols,
            IEnumerable<BatchType> types, string range = "", int last = 1)
        {
            if (types?.Count() < 1)
            {
                throw new ArgumentNullException("batchTypes cannot be null");
            }
            else if (symbols?.Count() < 1)
            {
                throw new ArgumentNullException("symbols cannot be null");
            }

            const string urlPattern = "stock/market/batch";

            var qsType = new List<string>();
            foreach (var x in types)
            {
                switch (x)
                {
                    case BatchType.Quote:
                        qsType.Add("quote");
                        break;

                    case BatchType.News:
                        qsType.Add("news");
                        break;

                    case BatchType.Chart:
                        qsType.Add("chart");
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(nameof(types));
                }
            }

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);
            qsb.Add("symbols", string.Join(",", symbols));
            qsb.Add("types", string.Join(",", qsType));
            if (!string.IsNullOrWhiteSpace(range))
            {
                qsb.Add("range", range);
            }

            qsb.Add("last", last);

            var pathNvc = new NameValueCollection();

            return await _executor.ExecuteAsync<Dictionary<string, BatchBySymbolResponse>>(urlPattern, pathNvc, qsb);
        }

        public async Task<BookResponse> BookAsync(string symbol) =>
            await _executor.SymbolExecuteAsync<BookResponse>("stock/[symbol]/book", symbol, _pk);

        public async Task<CashFlowResponse> CashFlowAsync(string symbol, Period period = Period.Quarter, int last = 1)
        {
            const string urlPattern = "stock/[symbol]/cash-flow/[last]";

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);
            qsb.Add("period", period.ToString().ToLower());

            var pathNvc = new NameValueCollection { { "symbol", symbol }, { "last", last.ToString() } };

            return await _executor.ExecuteAsync<CashFlowResponse>(urlPattern, pathNvc, qsb);
        }

        public async Task<string> CashFlowFieldAsync(string symbol, string field, Period period = Period.Quarter,
            int last = 1)
        {
            const string urlPattern = "stock/[symbol]/cash-flow/[last]/[field]";

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);
            qsb.Add("period", period.ToString().ToLower());

            var pathNvc = new NameValueCollection { { "symbol", symbol }, { "last", last.ToString() }, { "field", field } };

            return await _executor.ExecuteAsync<string>(urlPattern, pathNvc, qsb);
        }

        public async Task<IEnumerable<Quote>> CollectionsAsync(CollectionType collection, string collectionName)
        {
            const string urlPattern = "stock/market/collection/[collectionType]";

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);

            var pathNvc = new NameValueCollection { { "collectionType", collection.ToString().ToLower() } };

            return await _executor.ExecuteAsync<IEnumerable<Quote>>(urlPattern, pathNvc, qsb);
        }

        public async Task<CompanyResponse> CompanyAsync(string symbol) =>
            await _executor.SymbolExecuteAsync<CompanyResponse>("stock/[symbol]/company", symbol, _pk);

        public async Task<DelayedQuoteResponse> DelayedQuoteAsync(string symbol) =>
            await _executor.SymbolExecuteAsync<DelayedQuoteResponse>("stock/[symbol]/delayed-quote", symbol, _pk);

        public async Task<IEnumerable<DividendResponse>> DividendAsync(string symbol, DividendRange range)
        {
            const string urlPattern = "stock/[symbol]/dividends/[range]";

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);

            var pathNvc = new NameValueCollection
            {
                {"symbol", symbol}, {"range", range.ToString().ToLower().Replace("_", "")}
            };

            return await _executor.ExecuteAsync<IEnumerable<DividendResponse>>(urlPattern, pathNvc, qsb);
        }

        public async Task<EarningResponse> EarningAsync(string symbol, int last = 1) =>
            await _executor.SymbolLastExecuteAsync<EarningResponse>("stock/[symbol]/earnings/[last]", symbol, last,
                _pk);

        public async Task<string> EarningFieldAsync(string symbol, string field, int last = 1) =>
            await _executor.SymbolLastFieldExecuteAsync("stock/[symbol]/earnings/[last]/[field]", symbol, field, last,
                _pk);

        public async Task<Dictionary<string, EarningTodayResponse>> EarningTodayAsync() =>
            await _executor.NoParamExecute<Dictionary<string, EarningTodayResponse>>("stock/market/today-earnings",
                _pk);

        public async Task<IEnumerable<EffectiveSpreadResponse>> EffectiveSpreadAsync(string symbol) =>
            await _executor.SymbolExecuteAsync<IEnumerable<EffectiveSpreadResponse>>("stock/[symbol]/effective-spread",
                symbol, _pk);

        public async Task<EstimateResponse> EstimateAsync(string symbol, int last = 1) =>
            await _executor.SymbolLastExecuteAsync<EstimateResponse>("stock/[symbol]/estimates/[last]", symbol, last,
                _pk);

        public async Task<string> EstimateFieldAsync(string symbol, string field, int last = 1) =>
            await _executor.SymbolLastFieldExecuteAsync("stock/[symbol]/estimates/[last]/[field]", symbol, field, last,
                _pk);

        public async Task<FinancialResponse> FinancialAsync(string symbol, int last = 1) =>
            await _executor.SymbolLastExecuteAsync<FinancialResponse>("stock/[symbol]/financials/[last]", symbol, last,
                _pk);

        public async Task<string> FinancialFieldAsync(string symbol, string field, int last = 1) =>
            await _executor.SymbolLastFieldExecuteAsync("stock/[symbol]/financials/[last]/[field]", symbol, field, last,
                _pk);

        public async Task<FundOwnershipResponse> FundOwnershipAsync(string symbol) =>
            await _executor.SymbolExecuteAsync<FundOwnershipResponse>("stock/[symbol]/fund-ownership", symbol, _pk);

        public async Task<HistoricalPriceResponse> HistoricalPriceAsync(string symbol,
            ChartRange range = ChartRange._1m, DateTime? date = null, QueryStringBuilder qsb = null)
        {
            const string urlPattern = "stock/[symbol]/chart/[range]/[date]";

            qsb = qsb ?? new QueryStringBuilder();
            if (qsb.Exist("token"))
            {
                qsb.Add("token", _pk);
            }

            var pathNvc = new NameValueCollection
            {
                {"symbol", symbol},
                {"range", range.ToString().Replace("_", "")},
                {"date", date == null ? DateTime.Now.ToString("yyyyMMdd") : ((DateTime) date).ToString("yyyyMMdd")}
            };

            return await _executor.ExecuteAsync<HistoricalPriceResponse>(urlPattern, pathNvc, qsb);
        }

        public async Task<HistoricalPriceDynamicResponse> HistoricalPriceDynamicAsync(string symbol,
            QueryStringBuilder qsb = null)
        {
            const string urlPattern = "stock/[symbol]/chart/dynamic";

            qsb = qsb ?? new QueryStringBuilder();
            if (qsb.Exist("token"))
            {
                qsb.Add("token", _pk);
            }

            var pathNvc = new NameValueCollection
            {
                {"symbol", symbol}
            };

            return await _executor.ExecuteAsync<HistoricalPriceDynamicResponse>(urlPattern, pathNvc, qsb);
        }

        public async Task<IncomeStatementResponse> IncomeStatementAsync(string symbol, Period period = Period.Quarter,
            int last = 1)
        {
            const string urlPattern = "stock/[symbol]/income/[last]";

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);

            var pathNvc = new NameValueCollection { { "symbol", symbol }, { "last", last.ToString() } };

            return await _executor.ExecuteAsync<IncomeStatementResponse>(urlPattern, pathNvc, qsb);
        }

        public async Task<string> IncomeStatementFieldAsync(string symbol, string field, Period period = Period.Quarter,
            int last = 1)
        {
            const string urlPattern = "stock/[symbol]/income/[last]/[field]";

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);

            var pathNvc = new NameValueCollection { { "symbol", symbol }, { "last", last.ToString() }, { "field", field } };

            return await _executor.ExecuteAsync<string>(urlPattern, pathNvc, qsb);
        }

        public async Task<IEnumerable<InsiderRosterResponse>> InsiderRosterResponseAsync(string symbol) =>
            await _executor.SymbolExecuteAsync<IEnumerable<InsiderRosterResponse>>("stock/[symbol]/insider-roster",
                symbol, _pk);

        public async Task<IEnumerable<InsiderSummaryResponse>> InsiderSummaryAsync(string symbol) =>
            await _executor.SymbolExecuteAsync<IEnumerable<InsiderSummaryResponse>>("stock/[symbol]/insider-summary",
                symbol, _pk);

        public async Task<IEnumerable<InsiderTransactionResponse>> InsiderTransactionAsync(string symbol) =>
            await _executor.SymbolExecuteAsync<IEnumerable<InsiderTransactionResponse>>(
                "stock/[symbol]/insider-transactions", symbol, _pk);

        public async Task<IEnumerable<InstitutionalOwnershipResponse>> InstitutionalOwnerShipAsync(string symbol) => await _executor.SymbolExecuteAsync<IEnumerable<InstitutionalOwnershipResponse>>("stock/[symbol]/institutional-ownership", symbol, _pk);

        public async Task<IEnumerable<IntradayPriceResponse>> IntradayPriceAsync(string symbol) => await _executor.SymbolExecuteAsync<IEnumerable<IntradayPriceResponse>>("stock/[symbol]/intraday-prices", symbol, _pk);

        public async Task<IPOCalendar> IPOCanlendarAsync(IPOType ipoType)
        {
            const string urlPattern = "stock/market/[ipoType]";

            var qsb = new QueryStringBuilder();
            qsb.Add("token", _pk);

            var pathNvc = new NameValueCollection { { "ipoType", $"{ipoType.ToString().ToLower()}-ipos" } };

            return await _executor.ExecuteAsync<IPOCalendar>(urlPattern, pathNvc, qsb);
        }

        public async Task<KeyStatsResponse> KeyStatsAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<string> KeyStatsStatAsync(string symbol, string stat)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LargestTradeResponse>> LargestTradesAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Quote>> ListAsync(string listType)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<LogoResponse>> LogoAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<USMarketVolumeResponse>> USMarketVolumeAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<NewsResponse>> NewsAsync(string symbol, int last = 10)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<OHLCResponse>> OHLCAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> PeersAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<HistoricalPriceResponse> PreviousDayPriceAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<decimal> PriceAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<PriceTargetResponse> PriceTargetAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<Quote> QuoteAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<string> QuoteFieldAsync(string symbol, string field)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RecommendationTrendResponse>> RecommendationTrendAsync(string symbol)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SectorPerformanceResponse>> SectorPerformanceAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SplitResponse>> SplitAsync(string symbol, SplitRange range)
        {
            throw new NotImplementedException();
        }

        public async Task<UpcomingEventSymbolResponse> UpcomingEventSymbolAsync(string symbol, UpcomingEventType type)
        {
            throw new NotImplementedException();
        }

        public async Task<UpcomingEventMarketResponse> UpcomingEventMarketAsync(string symbol, UpcomingEventType type)
        {
            throw new NotImplementedException();
        }

        public async Task<VolumeByVenueResponse> VolumeByVenueAsync(string symbol)
        {
            throw new NotImplementedException();
        }
    }
}