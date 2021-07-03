using System.Collections.Generic;
using System.Text.Json;
using BenchmarkDotNet.Attributes;

namespace Benchmarks
{
    [MemoryDiagnoser]
    [Orderer(BenchmarkDotNet.Order.SummaryOrderPolicy.FastestToSlowest)]
    [RankColumn]
    public class Benchmarks
    {
        private JsonSerializerOptions? _camelOptions;

        private JsonSerializerOptions? _snakeOptions;

        private Dictionary<string, string>? _foo;

        [GlobalSetup]
        public void Setup()
        {
            _camelOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DictionaryKeyPolicy = JsonNamingPolicy.CamelCase };
            _snakeOptions = new JsonSerializerOptions { PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy(), DictionaryKeyPolicy = new JsonSnakeCaseNamingPolicy() };

            _foo = new Dictionary<string, string>()
            {
                ["ID"] = "A",
                ["url"] = "A",
                ["URL"] = "A",
                ["THIS IS SPARTA"] = "A",
                ["IsCIA"] = "A",
                ["iPhone"] = "A",
                ["IPhone"] = "A",
                ["xml2json"] = "A",
                ["already_snake_case"] = "A",
                ["IsJSONProperty"] = "A",
                ["ABCDEFGHIJKLMNOP"] = "A",
                ["Hi!! This is text.Time to test."] = "A",
            };
        }

        [Benchmark(Baseline = true)]
        public string Default() => JsonSerializer.Serialize(_foo);

        [Benchmark]
        public string Camel() => JsonSerializer.Serialize(_foo, _camelOptions);

        [Benchmark]
        public string Snake() => JsonSerializer.Serialize(_foo, _snakeOptions);
    }
}
