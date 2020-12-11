using System;

namespace MetaSolver
{
    //Solver class contains the mathematical functions used for solving the matrixes
    public static class Solver
    {

        //RecursiveSolve takes a winrate chart and returns player 1's nash equilibrium winrate for that chart
        //the second parameter 'end' indicates the end condition where RecursiveSolve should stop:
        //for example, 3 decks vs 3 decks best out of 5 series would need to be solved all the way (end = 0) because the series ends when one of the players has 0 decks left.
        // 3 decks vs 3 decks best out of 3 only needs to be solved to end = 1 because the series ends when one of the players has 1 deck left.
        public static double RecursiveSolve(double[][] chart, int end)
        {

            //declare and initialize a matrix to feed back into RecursiveSolve if we haven't reached the end
            double[][] matrix = new double[chart.Length][];
            for (int i = 0; i < chart.Length; i++)
            {
                matrix[i] = new double[chart[0].Length];
            }

            //end conditions for series that a player has to win with all their decks (example: 3 decks vs 3 decks best out of 5)
            if (end == 0)
            {

                // if both players have 1 deck left, the winrate is the winrate of that deck v deck matchup
                if (chart.Length == 1 && chart[0].Length == 1)
                {
                    return chart[0][0];
                }

                // if player 1 has one deck left, the winrate is the probability of winning one of any of the remaining possible matches
                if (chart.Length == 1)
                {
                    return chart[0][0] + (1 - chart[0][0]) * RecursiveSolve(MatrixRemove(chart, -1, 0), end);
                }

                // if player 2 has one deck left, the winrate is the probability of winning all of the remaining possible matches
                if (chart[0].Length == 1)
                {
                    return chart[0][0] * RecursiveSolve(MatrixRemove(chart, 0, -1), end);
                }
            }

            //end conditions for series that a player can leave 1 number of decks out (example: 3 decks vs 3 decks best out of 3)
            if (end == 1)
            {

                // if both players have 2 decks left, the winrate is the winrate of this remaining 2x2 matrix
                if (chart.Length == 2 && chart[0].Length == 2)
                {
                    return MatrixValue(chart);
                }

                // if player 1 has 2 decks left, the winrate is the probability of winning one of any of the remaining possible matrixes
                else if (chart.Length == 2)
                {
                    for (int i = 0; i < chart.Length; i++)
                    {
                        for (int j = 0; j < chart[i].Length; j++)
                        {
                            matrix[i][j] = (chart[i][j]) + ((1 - chart[i][j]) * RecursiveSolve(MatrixRemove(chart, -1, j), end));
                        }
                    }

                    return MatrixValue(matrix);
                }

                // if player 2 has 2 decks left, the winrate is the probability of winning all of the remaining possible matrixes
                else if (chart[0].Length == 2)
                {
                    for (int i = 0; i < chart.Length; i++)
                    {
                        for (int j = 0; j < chart[i].Length; j++)
                        {
                            matrix[i][j] = (chart[i][j] * RecursiveSolve(MatrixRemove(chart, i, -1), end));
                        }
                    }

                    return MatrixValue(matrix);
                } 
            }

            // if both players can still lose a game then keep solving until we reach an end condition
            for (int i = 0; i < chart.Length; i++)
            {
                for (int j = 0; j < chart[i].Length; j++)
                {
                    matrix[i][j] = (chart[i][j] * RecursiveSolve(MatrixRemove(chart, i, -1), end)) + ((1 - chart[i][j]) * RecursiveSolve(MatrixRemove(chart, -1, j), end));
                }
            }

            return MatrixValue(matrix);
        }

        //MatrixRemove returns a new matrix with the original matrix minus the selected row and column.
        //-1 can be input for a row or column if you don't want to remove any
        public static double[][] MatrixRemove(double[][] matrix, int row = -1, int column = -1)
        {

            double[][] matrixx;
            int newRowLength;
            int newColumnLength;

            if (row >= 0)
                newRowLength = matrix.Length - 1;              
            else
                newRowLength = matrix.Length;

            matrixx = new double[newRowLength][];

            for (int i = 0; i < matrixx.Length; i++)
            {
                if (column >= 0)
                    newColumnLength = matrix[i].Length - 1;
                else
                    newColumnLength = matrix[i].Length;
                matrixx[i] = new double[newColumnLength];
            }

            int skippedrow = 0;
            for (int i = 0; i < matrixx.Length; i++)
            {
                if (i == row)
                {
                    skippedrow = 1;
                }

                int skippedcolumn = 0;
                for (int j = 0; j < matrixx[i].Length; j++)
                {
                    if (j == column)
                    {
                        skippedcolumn = 1;
                    }

                    matrixx[i][j] = matrix[i + skippedrow][j + skippedcolumn];

                }
            }

            return matrixx;
        }

        //Solves a matrix and puts the solution into solution matrix
        //Math is from Professor Thomas S. Ferguson's matrix game solver: https://www.math.ucla.edu/~tom/gamesolve.html
        public static void MatrixSolver(double[][] matrix, double[][] solution)
        {
            int m = matrix.Length;
            int m1 = matrix.Length + 1;
            int n1 = matrix[0].Length + 1;
            int n = n1 - 1;
            int n2 = n1 + 1;

            double[][] A = new double[m1 + 1][];
            for (int i = 0; i <= m1; i++)
            {
                A[i] = new double[n1 + 1];
                A[i][0] = i;
            }

            for (int i = 1; i < m1; i++)
            {
                int k = 1;
                int j = 1;

                while (j <= matrix[i - 1].Length)
                {
                    A[i][k] = matrix[i - 1][j - 1];
                    k++;
                    j++;
                }
            }

            for (int i = 1; i < m1; i++)
            {
                A[i][n1] = 1;
            }

            for (int j = 1; j < n1; j++)
            {
                A[0][j] = -j;
                A[m1][j] = -1;
            }

            A[m1][n1] = 0;
            A[0][n1] = 0;

            double min = A[1][1];
            for (int j = 2; j < n1; j++)
            {
                if (A[1][j] < min)
                    min = A[1][j];
            }

            min = min - 1;

            for (int i = 1; i < m1; i++)
            {
                for (int j = 1; j < n1; j++)
                {
                    A[i][j] = A[i][j] - min;
                }
            }

            double epsilon = .000000000001;
            double t1 = 0;
            double t2 = 0;
            double d = 0;
            int q = 1;
            int p = 0;

            while (q != 0)
            {
                double r = 0;
                for (int i = 1; i < m1; i++)
                {
                    t1 = A[i][q];

                    if (t1 > epsilon)
                    {
                        t2 = A[i][n1];
                        if (t2 <= 0)
                        {
                            p = i;
                            i = m1;
                        }
                        else if (t1 > t2 * r)
                        {
                            p = i;
                            r = (t1 / t2);
                        }
                    }
                }

                d = A[p][q];
                for (int j = 1; j < n2; j++)
                {
                    if (j != q)
                    {
                        A[p][j] = A[p][j] / d;

                    }
                }

                for (int i = 1; i <= m1; i++)
                {
                    if (i != p)
                    {
                        for (int j = 1; j <= n1; j++)
                        {
                            if (j != q)
                            {
                                A[i][j] = A[i][j] - A[i][q] * A[p][j];
                            }
                        }
                    }
                }

                for (int i = 1; i <= m1; i++)
                {
                    if (i != p) A[i][q] = (0 - A[i][q]) / d;
                }

                A[p][q] = 1 / d;
                t1 = A[p][0];
                A[p][0] = A[0][q];
                A[0][q] = t1;
                q = 0;

                for (int j = 1; j < n1; j++)
                {
                    if (A[m1][j] < 0)
                    {
                        q = j;
                        j = n1;
                    }
                }
            }

            double val = 1 / A[m1][n1];
            double[] x, y;
            x = new double[n1 + m1];
            y = new double[n1 + m1];

            for (int j = 1; j < n1; j++)
            {
                if (A[0][j] < 0)
                {
                    y[Convert.ToInt32(0 - A[0][j])] = 0;

                }
                else
                {
                    x[Convert.ToInt32(A[0][j])] = A[m1][j] * val;
                }
            }

            for (int i = 1; i < m1; i++)
            {
                if (A[i][0] < 0)
                {
                    y[Convert.ToInt32(0 - A[i][0])] = A[i][n1] * val;
                }
                else
                {
                    x[Convert.ToInt32(A[i][0])] = 0;
                }
            }

            val = val + min;


            for (int i = 1; i <= m; i++)
            {
                solution[0][i - 1] = x[i];
            }

            for (int i = 1; i <= n; i++)
            {
                solution[1][i - 1] = y[i];
            }
        }


        //solves a matrix and gives player 1's total probability of winning the matrix
        public static double MatrixValue(double[][] matrix)
        {
            int m = matrix.Length;
            int m1 = matrix.Length + 1;
            int n1 = matrix[0].Length + 1;
            int n = n1 - 1;
            int n2 = n1 + 1;

            double[][] A = new double[m1 + 1][];
            for (int i = 0; i <= m1; i++)
            {
                A[i] = new double[n1 + 1];
                A[i][0] = i;
            }

            for (int i = 1; i < m1; i++)
            {
                int k = 1;
                int j = 1;

                while (j <= matrix[i - 1].Length)
                {
                    A[i][k] = matrix[i - 1][j - 1];
                    k++;
                    j++;
                }
            }


            for (int i = 1; i < m1; i++)
            {
                A[i][n1] = 1;
            }

            for (int j = 1; j < n1; j++)
            {
                A[0][j] = -j;
                A[m1][j] = -1;
            }

            A[m1][n1] = 0;
            A[0][n1] = 0;

            double min = A[1][1];
            for (int j = 2; j < n1; j++)
            {
                if (A[1][j] < min)
                    min = A[1][j];
            }

            min = min - 1;

            for (int i = 1; i < m1; i++)
            {
                for (int j = 1; j < n1; j++)
                {
                    A[i][j] = A[i][j] - min;
                }
            }

            double epsilon = .000000000001;
            double t1 = 0;
            double t2 = 0;
            double d = 0;
            int q = 1;
            int p = 0;

            while (q != 0)
            {
                double r = 0;
                for (int i = 1; i < m1; i++)
                {
                    t1 = A[i][q];

                    if (t1 > epsilon)
                    {
                        t2 = A[i][n1];
                        if (t2 <= 0)
                        {
                            p = i;
                            i = m1;
                        }
                        else if (t1 > t2 * r)
                        {
                            p = i;
                            r = (t1 / t2);
                        }
                    }
                }

                d = A[p][q];
                for (int j = 1; j < n2; j++)
                {
                    if (j != q)
                    {
                        A[p][j] = A[p][j] / d;

                    }
                }

                for (int i = 1; i <= m1; i++)
                {
                    if (i != p)
                    {
                        for (int j = 1; j <= n1; j++)
                        {
                            if (j != q)
                            {
                                A[i][j] = A[i][j] - A[i][q] * A[p][j];
                            }
                        }
                    }
                }

                for (int i = 1; i <= m1; i++)
                {
                    if (i != p) A[i][q] = (0 - A[i][q]) / d;
                }

                A[p][q] = 1 / d;
                t1 = A[p][0];
                A[p][0] = A[0][q];
                A[0][q] = t1;
                q = 0;

                for (int j = 1; j < n1; j++)
                {
                    if (A[m1][j] < 0)
                    {
                        q = j;
                        j = n1;
                    }
                }
            }
            double val = 1 / A[m1][n1];
            double[] x, y;
            x = new double[n1 + m1];
            y = new double[n1 + m1];

            for (int j = 1; j < n1; j++)
            {
                if (A[0][j] < 0)
                {
                    y[Convert.ToInt32(0 - A[0][j])] = 0;

                }
                else
                {
                    x[Convert.ToInt32(A[0][j])] = A[m1][j] * val;
                }
            }

            for (int i = 1; i < m1; i++)
            {
                if (A[i][0] < 0)
                {
                    y[Convert.ToInt32(0 - A[i][0])] = A[i][n1] * val;
                }
                else
                {
                    x[Convert.ToInt32(A[i][0])] = 0;
                }
            }

            val = val + min;

            return val;
        }
    }
}
