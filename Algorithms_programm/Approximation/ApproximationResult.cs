namespace Algorithms_programm.Approximation;

/// <summary>
/// Результат подбора одной теоретической функции под эмпирические данные: тип f(n), подобранная
/// константа C и её MSE. Evaluate(n) удобен для построения кривой аппроксимации на графике (Этап 4).
/// </summary>
public sealed record ApproximationResult(TheoreticalFunctionType FunctionType, double C, double Mse)
{
    public double Evaluate(double n) => C * TheoreticalFunctions.Evaluate(FunctionType, n);

    public string DisplayFormula => $"T(n) ≈ {C:G4} · {TheoreticalFunctions.GetDisplayName(FunctionType)}";
}
