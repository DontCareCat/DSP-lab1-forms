using System;
using System.Collections.Generic;
using System.Text;

namespace DSP_lab1_forms
{
    static class fourier
    {
        public static complex[] FFT(complex[] x)
        {
            int N = x.Length;
            complex[] X = new complex[N];
            complex[] d, D, e, E;
            if(N==1)
            {
                X[0] = x[0];
                return X;
            }
            int k;
            e = new complex[N / 2];
            d = new complex[N / 2];

            for(k=0;k<N/2;k++)
            {
                e[k] = x[2 * k];
                d[k] = x[2 * k + 1];
            }
            D = FFT(d);
            E = FFT(e);
            for(k=0;k<N/2;k++)
            {
                X[k] = E[k] + D[k];
                X[k + N / 2] = E[k] - D[k];
            }
            return X;
        }
    }
}
