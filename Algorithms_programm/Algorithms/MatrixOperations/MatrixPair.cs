namespace Algorithms_programm.Algorithms.MatrixOperations;

/// <summary>
/// Пара матриц A (n x m) и B (m x n) для перемножения C = A*B.
/// По методичке: A и B — n×m со случайными неотрицательными элементами, C = A*B имеет O(n^3)
/// при n == m; здесь поддерживаются и прямоугольные матрицы для общности 3D-графика (n, m).
/// </summary>
/// <param name="A">Матрица размера RowsA x ColsA.</param>
/// <param name="B">Матрица размера ColsA x ColsB (число строк B должно совпадать с числом столбцов A).</param>
public sealed record MatrixPair(double[,] A, double[,] B)
{
    public int RowsA => A.GetLength(0);
    public int ColsA => A.GetLength(1);
    public int RowsB => B.GetLength(0);
    public int ColsB => B.GetLength(1);
}
