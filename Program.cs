using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KNN
{
    class Program
    {
        static void Main(string[] args)
        {
            KNN knn = new KNN();

            knn.LoadData("iris.dat", KNN.DataType.TRAININGDATA);
            knn.LoadData("test.dat", KNN.DataType.TESTDATA);
            knn.Classify(3);
            Console.ReadKey();
        }
    }
}
