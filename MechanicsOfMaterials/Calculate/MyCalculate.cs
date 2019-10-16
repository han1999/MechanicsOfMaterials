using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MechanicsOfMaterials;

namespace MechanicsOfMaterials.Calculate
{
    class MyCalculate
    {
        public static double SumOfSquaresMxyz(int a)
        {
            return Form1.mx[a] * Form1.mx[a] + Form1.my[a] * Form1.my[a] + Form1.mz[a] * Form1.mz[a];
        }
        public static bool IsMxMyMzMore(int a, int b)
        {
            return SumOfSquaresMxyz(a) > SumOfSquaresMxyz(b);
        }

        public static double DefiniteIntegral(double[] m, int a, int b)
        {
            double sum = 0;
            for (int i=a; i<=b; i++)
            {
                sum += m[i];
            }
            return sum;
        }

        public static int NearestMoreEvenNumber(double a)
        {
            int result;
            result = (int)(Math.Ceiling(a));
            if (result % 2!=0)
            {
                result++;
            }
            return result;
        }
    }
}
