using Algorithms_programm.Algorithms.Abstractions;

namespace Algorithms_programm.Algorithms.MatrixOperations;

/// <summary>
/// Алгоритм 8 методички: классическое (тройной цикл) матричное умножение C = A*B.
/// Теоретическая сложность O(n*m*k), в квадратном случае (n=m=k) — O(n^3).
/// Используется для 3D-графика время(n, m) в GUI.
/// </summary>
public sealed class MatrixMultiplicationAlgorithm : ITimedAlgorithm<MatrixPair, double[,]>
{
    public string Name => "MatrixMultiplication";

    public double[,] Execute(MatrixPair input)
    {
        if (input.ColsA != input.RowsB)
        {
            throw new ArgumentException(
                $"Несовместимые размеры матриц: A имеет {input.ColsA} столбцов, " +
                $"B имеет {input.RowsB} строк. Умножение A*B невозможно.");
        }

        var rowsA = input.RowsA;
        var colsA = input.ColsA;
        var colsB = input.ColsB;
        var result = new double[rowsA, colsB];

        for (var i = 0; i < rowsA; i++)
        {
            for (var j = 0; j < colsB; j++)
            {
                double sum = 0;
                for (var k = 0; k < colsA; k++)
                {
                    sum += input.A[i, k] * input.B[k, j];
                }

                result[i, j] = sum;
            }
        }

        return result;
    }
}
