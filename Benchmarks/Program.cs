using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using Perfolizer.Horology;

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
                .WithIterationTime(new TimeInterval(100, TimeUnit.Millisecond))
                .WithLaunchCount(1)
                .WithEvaluateOverhead(true)
                .WithToolchain(InProcessEmitToolchain.Instance));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var switcher = new BenchmarkSwitcher(new[] {
                typeof(Simplex_1_Scalar),
                typeof(Simplex_4_Sse2),
                typeof(Simplex_8_Avx2),

                typeof(OpenSimplex2_1_Scalar),
                typeof(OpenSimplex2_4_Sse2),
                typeof(OpenSimplex2_8_Avx2),
            });

            switcher.RunAllJoined(new Config());
        }
    }
}
