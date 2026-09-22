namespace Algorithms_programm.Approximation;

/// <summary>
/// Подбор константы C в T_approx(n) = C * f(n) методом наименьших квадратов (Этап 3 методички).
/// Для фиксированной f(n) это одномерная линейная регрессия без свободного члена:
/// минимизируем sum((T_i - C*f(n_i))^2) по C. Производная по C, приравненная к нулю, даёт
/// закрытую форму: C = sum(T_i * f(n_i)) / sum(f(n_i)^2).
/// </summary>
public static class LeastSquaresApproximator
{
    /// <summary>
    /// Возвращает оптимальную по МНК константу C для заданной теоретической функции.
    /// Если знаменатель вырождается в 0 (все f(n_i) == 0 — на практике возможно только для
    /// Logarithmic/LinearLogarithmic, если все точки имеют n = 1), возвращает 0, а не делит на ноль.
    /// </summary>
    public static double FitConstant(IReadOnlyList<DataPoint> points, TheoreticalFunctionType functionType)
    {
        if (points.Count == 0)
        {
            throw new ArgumentException("Нужна хотя бы одна эмпирическая точка для аппроксимации.", nameof(points));
        }

        double numerator = 0;
        double denominator = 0;

        foreach (var point in points)
        {
            var f = TheoreticalFunctions.Evaluate(functionType, point.N);
            numerator += point.Value * f;
            denominator += f * f;
        }

        return denominator == 0 ? 0 : numerator / denominator;
    }
}
