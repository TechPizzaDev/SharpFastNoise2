using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

namespace Benchmarks
{
    public class Config : ManualConfig
    {
        public Config()
        {
            AddLogger(new ConsoleLogger());
            AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByParams);
            AddExporter(new HtmlExporter());
            AddColumnProvider(DefaultColumnProviders.Instance);

            AddJob(Job.ShortRun
                .WithLaunchCount(1)
                .WithEvaluateOverhead(true)
                .WithToolchain(InProcessEmitToolchain.Instance));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[] { typeof(BenchSimplexScalar), typeof(BenchSimplexSse) });

            switcher.RunAllJoined(new Config());
        }
    }
}
