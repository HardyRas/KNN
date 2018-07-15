using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace KNN
{
    class KNN
    {
        private int lines;
        private int K;


        //MEMORY ALLOCATION FOR THE DATA TO BE USED
        //IN THE CLASSIFICATION

        // this holds the values of the training data
        private List<double[]>   trainingSetValues = new List<double[]>();
        // this holds the class associated with the values
        private List<string>     trainingSetClasses = new List<string>();

        // same for the test input
        private List<double[]>   testSetValues = new List<double[]>();
        private List<string>     testSetClasses = new List<string>();
        

        //CREATING USER-DEFINED DATA TYPES
        //THIS IS REALLY COOL BTW
        public enum DataType
        {
            TRAININGDATA, TESTDATA
        };
        //nOTHING NEW HERE
        //jUST Fetching the data files from
        //a specified path in the bin directory in the projects
        //folder/location
        public void LoadData(string path, DataType dataType)
        {

            //gets data ffrom the bin directory
            StreamReader file = new StreamReader(path);
            string line;

            this.lines = 0;

            Console.WriteLine("Reading data from {0} ...", path);

            while((line = file.ReadLine()) != null)
            {
                // as we have a file basically, split the line at each ','
                string[] splitLine = line.Split(',').ToArray();

                // and add them to a list
                List<string> lineItems = new List<string>(splitLine.Length);           
                lineItems.AddRange(splitLine);

                // create an appropiate array to hold the doubles from this line
                double[] lineDoubles = new double[lineItems.Count - 1];
                // and a string holding the class
                string lineClass = lineItems.ElementAt(lineItems.Count - 1);

                for(int i = 0; i < lineItems.Count - 1; i++)    // last item is the set class
                {
                    // convert each item in the list to a double
                    double val = Double.Parse(lineItems.ElementAt(i));
                    lineDoubles[i] = val;
                }

                // finally, save them...HERE
              
                        //REMEMBER THESE LIST CREATIONS???

                 // private List<double[]> trainingSetValues = new List<double[]>();
                 // private List<string> trainingSetClasses = new List<string>();

                if (dataType == DataType.TRAININGDATA)
                {
                    this.trainingSetValues.Add(lineDoubles);
                    this.trainingSetClasses.Add(lineClass);
                }
                else if(dataType == DataType.TESTDATA)
                {
                    this.testSetValues.Add(lineDoubles);
                    this.testSetClasses.Add(lineClass);
                }
                this.lines++;
            }

            Console.WriteLine("Done. read {0} lines.", this.lines);

            file.Close();
        }


        //CLASSIFICATION
        //THIS IS WHERE THE MAGIC BEGINS.
        public void Classify(int neighborsNumber)
        {
            this.K = neighborsNumber;

            // create an array where we store the distance from our test data and the training data -> [0]
            // plus the index of the training data element -> [1]
            double[][] distances = new double[trainingSetValues.Count][];

            //initialized to zero.
            double accuracy = 0;
            double correct = 0, testNumber = 0;

            for (int i = 0; i < trainingSetValues.Count; i++)
                distances[i] = new double[2];

            Console.WriteLine("Classifying...");

            // start computing
            //MAGIC
            for(var test = 0; test < this.testSetValues.Count; test++)
            {
                Parallel.For(0, trainingSetValues.Count, index =>
                    {
                        //USING EUCLIDEAN DISTANCE  4 distance metric for continuous variables 
                        //EUCLIDEAN DISTANCE HAS BEEN DEFINED AT THE FUNCTION HUKO CHINI...
                        var dist = EuclideanDistance(this.testSetValues[test], this.trainingSetValues[index]);
                        distances[index][0] = dist;
                        distances[index][1] = index;
                        
                    }
                );

                Console.WriteLine("\n\nClosest K={0} neighbors: ",this.K);

                // sort and select first K of them
                var sortedDistances = distances.AsParallel().OrderBy(t => t[0]).Take(this.K);

                string realClass = testSetClasses[test];

                //print and check the result
                //determine correctness of the classification
                //based on the eulidean distance in the data sets.


                //      WE CAN GO AND CHANGE THE TEST DATA IN THE BIN 
                //      DIRECTORY AND SEE WHETHER THE ACCURACY STILL HOLDS.
                //      EITHER INPUT CORRECT VALUES FROM THE TRAINING SET
                //      OR WRONG AND RANDOM VALUES
                //      OR A RATIO OF THE SAME.

                //      HAVE FUN...NI KAZI INGINE MOB SANA XD

                foreach (var d in sortedDistances)
                {
                    string predictedClass = trainingSetClasses[(int) d[1]];
                    if (string.Equals(realClass, predictedClass) == true)
                        correct++;
                    testNumber++;
                    Console.WriteLine(" Test {0}:\n Actual class: {1} \n Predicted class: {2}\n", test, realClass, predictedClass);
                }
            }

            Console.WriteLine();

            Console.WriteLine("Correct {0}", correct);
            Console.WriteLine("TestNumber {0}", testNumber);
            // compute and print the accuracy
            accuracy = (correct / testNumber) * 100;

            Console.WriteLine("Accuracy of predictions: {0}%", accuracy);

        }


        //dEFIniNG tHE eUcLIDEAN DISTanCE
        //no mathematical definitions supported.
        //thats why.
        private static double EuclideanDistance(double[] sampleOne, double[] sampleTwo)
        {
            double d = 0.0;

            for(int i = 0; i < sampleOne.Length; i++)
            {
                double temp = sampleOne[i] - sampleTwo[i];
                d += temp * temp;
            }
            return Math.Sqrt(d);
        }
    }
}
