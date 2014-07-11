using BizLayer.BlogsAnalysis.Concrete;
using GenericServices;

namespace BizLayer.BlogsAnalysis
{
    public interface IBlogAnalyser : IActionSync<AnalysisResult, int> {}
}