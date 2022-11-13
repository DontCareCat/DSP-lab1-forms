using System;
using System.Collections.Generic;
using System.Text;

namespace DSP_lab1_forms
{
    class complex
    {
        public double real;
        public double imag;
        public complex()
        {
            real = 0;
            imag = 0;
        }
        public complex(double real, double imag)
        {
            this.real = real;
            this.imag = imag;
        }
        public override string ToString()
        {
            return $"{real} {imag}i";
        }
        public static complex fromPolar(double r, double rad)
        {
            return new complex(r * Math.Cos(rad), r * Math.Sin(rad));
        }
        public static complex operator +(complex a, complex b)
        {
            return new complex(a.real + b.real, a.imag + b.imag);
        }
        public static complex operator -(complex a,complex b)
        {
            return new complex(a.real - b.real, a.imag - b.imag);
        }
        public static complex operator *(complex a, complex b)
        {
            return new complex(a.real*b.real - a.imag*b.imag, a.real*b.imag + a.imag*b.real);
        }
        public double magnit
        {
            get
            {
                return Math.Sqrt(Math.Pow(real,2)+Math.Pow(imag,2));
            }
        }
        public double phase
        {
            get
            {
                return Math.Atan(imag / real);
            }
        }
    }
}
