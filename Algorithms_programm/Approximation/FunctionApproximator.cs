namespace Algorithms_programm.Approximation;

/// <summary>
/// Верхнеуровневая точка входа Этапа 3 методички: по набору эмпирических точек подбирает
/// C методом наименьших квадратов для одной или для всех шести теоретических функций и считает
/// их MSE. GUI (Этап 4) использует ApproximateAll/FindBestFit, чтобы показать пользователю
/// не только выбранную вручную кривую, но и предложить автоматически "лучшую" по MSE.
/// </summary>
public static class FunctionApproximator
{
    public static ApproximationResult Approximate(IReadOnlyList<DataPoint> points, TheoreticalFunctionType functionType)
    {
        var c = LeastSquaresApproximator.FitConstant(points, functionType);
        var mse = MseCalculator.Calculate(points, functionType, c);
        return new ApproximationResult(functionType, c, mse);
    }

    /// <summary>Подбирает все шесть теоретических функций, отсортированные по возрастанию MSE (лучшая — первая).</summary>
    public static IReadOnlyList<ApproximationResult> ApproximateAll(IReadOnlyList<DataPoint> points)
    {
        return TheoreticalFunctions.AllTypes
            .Select(functionType => Approximate(points, functionType))
            .OrderBy(result => result.Mse)
            .ToList();
    }

    /// <summary>Функция с наименьшим MSE среди всех шести — "автоматическая" рекомендация методички.</summary>
    public static ApproximationResult FindBestFit(IReadOnlyList<DataPoint> points) => ApproximateAll(points)[0];
}
