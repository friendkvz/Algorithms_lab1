namespace Algorithms_programm.Approximation;

/// <summary>
/// MSE = (1/k) * sum((T_empirical(n_i) - T_approx(n_i))^2) — метрика качества аппроксимации
/// из Этапа 3 методички. Меньше — лучше; используется, чтобы выбрать, какая из шести
/// теоретических функций f(n) лучше всего описывает эмпирические данные (FunctionApproximator).
/// </summary>
public static class MseCalculator
{
    public static double Calculate(IReadOnlyList<DataPoint> points, TheoreticalFunctionType functionType, double c)
    {
        if (points.Count == 0)
        {
            return 0;
        }

        double sumSquaredError = 0;
        foreach (var point in points)
        {
            var approx = c * TheoreticalFunctions.Evaluate(functionType, point.N);
            var error = point.Value - approx;
            sumSquaredError += error * error;
        }

        return sumSquaredError / points.Count;
    }
}
