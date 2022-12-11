using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PeriodicSplineInterpolation
{
    public const int order = 6; // number of reference points (max 10)
    double[] x_k = new double[order + 1];
    double[] y_k = new double[order + 1];
    //const int datapoints = 1000; // number of datapoints for the interpolation
    //const double deltaX = 0.1; // sample time 

    double[,] m = new double[order - 1, order - 1];
    double[] y = new double[order - 1];
    double[] x = new double[order - 1];
    //double[] yp = new double[datapoints];
    //double[] xp = new double[datapoints];

    double[] a = new double[order + 1];
    double[] b = new double[order + 1];
    double[] c = new double[order + 1];
    double[] d = new double[order + 1];
    double[] h = new double[order];
    public PeriodicSplineInterpolation(double[] x, double[] y)
    {
        for (int i = 0; i < order + 1; i++)
        {
            x_k[i] = x[i];
            y_k[i] = y[i];
        }
        x_k[order] = x[0];
        y_k[order] = y[0];
        CalcParameters();
    }

    public double Interpolate(double x)
    {
        int i;
        double y = 0.0;
        for (i = 0; i < order; i++)
        {
            if ((x >= x_k[i]) && (x <= x_k[i + 1]))
            {
                y = a[i] + b[i] * (x - x_k[i]) + c[i] * Math.Pow(x - x_k[i], 2) + d[i] * Math.Pow(x - x_k[i], 3);
                break;
            }
        }
        return y;
    }

    public bool CalcParameters()
    {
        int i, k;

        for (i = 0; i < order; i++)  // calculate a parameters
        {
            a[i] = y_k[i];
        }
        a[order] = y_k[1];
        for (i = 0; i < order - 1; i++)
        {
            h[i] = x_k[i + 1] - x_k[i];
        }
        h[i] = h[0];

        // empty matrix for c parameters
        for (i = 0; i < order - 1; i++)
        {
            for (k = 0; k < order - 1; k++)
            {
                m[i, k] = 0.0;
                y[i] = 0.0;
                x[i] = 0.0;
            }
        }

        // fill matrix for c parameters
        for (i = 0; i < order - 1; i++)
        {
            if (i == 0)
            {
                m[i, 0] = 2.0 * (h[0] + h[1]);
                m[i, 1] = h[1];
            }
            else
            {
                m[i, i - 1] = h[i];
                m[i, i] = 2.0 * (h[i] + h[i + 1]);
                if (i < order - 2)
                    m[i, i + 1] = h[i + 1];
            }
            if ((h[i] != 0.0) && (h[i + 1] != 0.0))
                y[i] = ((a[i + 2] - a[i + 1]) / h[i + 1] - (a[i + 1] - a[i]) / h[i]) * 3.0;
            else
                y[i] = 0.0;
        }
        m[0, order - 2] = h[0];
        m[order - 2, 0] = h[0];

        TGauss gauss = new TGauss(order - 1, m, y);

        if (gauss.Eliminate())
        {
            x = gauss.Solve();
            for (i = 1; i < order; i++)
            {
                c[i] = x[i - 1];
            }
            c[0] = c[order - 1];  // boundry condition for natural spline 
            for (i = 0; i < order; i++) // calculate b and d parameters
            {
                if (h[i] != 0.0)
                {
                    d[i] = 1.0 / 3.0 / h[i] * (c[i + 1] - c[i]);
                    b[i] = 1.0 / h[i] * (a[i + 1] - a[i]) - h[i] / 3.0 * (c[i + 1] + 2 * c[i]);
                }
            }
            return true;
        }
        else
            return false;
    }

    /* public void Interpolate(int order, int resolution)
    {
        int i, k;
        double timestamp;
        for (i = 0; i < order - 1; i++)
        {
            for (k = 0; k < resolution; k++)
            {
                timestamp = (double)(k) / (double)(resolution) * h[i];
                yp[i * resolution + k] = a[i] + b[i] * (timestamp) + c[i] * Math.Pow(timestamp, 2) + d[i] * Math.Pow(timestamp, 3);
                xp[i * resolution + k] = timestamp + x_k[i];
            }
        }
    } */
}
