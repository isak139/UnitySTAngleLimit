using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TGauss
{
    private double[,] m;
    private double[] y;
    private readonly int maxOrder;

    public TGauss(int size, double[,] matrix, double[] solution)
    {
        m = matrix;
        y = solution;
        maxOrder = size;
    }

    private void SwitchRows(int n)
    {
        double tempD;
        int i, j;
        for (i = n; i <= maxOrder - 2; i++)
        {
            for (j = 0; j <= maxOrder - 1; j++)
            {
                tempD = m[i, j];
                m[i, j] = m[i + 1, j];
                m[i + 1, j] = tempD;
            }
            tempD = y[i];
            y[i] = y[i + 1];
            y[i + 1] = tempD;
        }
    }

    public void Pivot(int n)
    {
        double tempD = 0;
        int j;
        if (n < maxOrder)
        {
            for (j = 0; j < maxOrder; j++)
            {
                tempD = tempD + Math.Abs(m[n, j]);
            }
            tempD = tempD / maxOrder;
            if (tempD != 0)
            {
                for (j = 0; j < maxOrder; j++)
                {
                    m[n, j] = m[n, j] / tempD;
                }
                y[n] = y[n] / tempD;
            }
        }
    }

    public bool Eliminate()
    {
        int i, k, l;
        bool calcError = false;
        i = 1;


        for (k = 0; k <= maxOrder - 2; k++)
        {
            l = k + 1;
            if (Math.Abs(m[k, k]) < 1e-8)
            {
                SwitchRows(k);
            }
            for (i = k; i <= maxOrder - 2; i++)
            {
                if (!calcError)
                {
                    if (Math.Abs(m[i + 1, k]) > 1e-8)
                    {
                        for (l = k + 1; l <= maxOrder - 1; l++)
                        {
                            m[i + 1, l] = m[i + 1, l] * m[k, k] - m[k, l] * m[i + 1, k];
                        }
                        y[i + 1] = y[i + 1] * m[k, k] - y[k] * m[i + 1, k];
                        m[i + 1, k] = 0;
                    }

                }
            }
        }
        return !calcError;
    }

    public double[] Solve()
    {
        int k, l;
        double[] x = new double[maxOrder];
        for (k = maxOrder - 1; k >= 0; k--)
        {
            for (l = maxOrder - 1; l > k; l--)
            {
                y[k] = y[k] - x[l] * m[k, l];
            }
            x[k] = y[k] / m[k, k];
        }
        return x;
    }
}

