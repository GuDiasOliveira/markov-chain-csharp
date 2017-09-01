using System;

namespace MarkovChainCSharp
{
    class Program
    {
        /* Here, the MarkovChain class will be tested */
        static void Main(string[] args)
        {
            CalcPiTest();
            GetProbabilityTest();
            GetChainProbabilityTest();
            Console.WriteLine("Allright! :D");
            Console.ReadKey();
        }

        private static void AssertEquals(double expected, double result, double delta)
        {
            if (Math.Abs(expected - result) < delta)
                return;
            throw new Exception("Assertion fail");
        }

        private static void AssertEquals(int expected, int result)
        {
            if (expected == result)
                return;
            throw new Exception("Assertion fail");
        }

        public static void CalcPiTest()
        {
            MarkovChain<int> matrix = new MarkovChain<int>(
                    new double[,] {
                    {1.0 / 3,  2.0 / 3,  0,        0      },
                    {1.0 / 3,  1.0 / 3,  1.0 / 3,  0      },
                    {1.0 / 3,  1.0 / 3,  1.0 / 6,  1.0 / 6},
                    {0,        0,        0,        1}
                    }, 100, 200, 300, 400);

            // p_2
            double[] expectedResult = { 1.0 / 3, 4.0 / 9, 11.0 / 54, 1.0 / 54 };
            double[] result = matrix.GetPi(
                    new double[] { 2.0 / 3, 1.0 / 3, 0, 0 }, // p_0
                    2);
            AssertEquals(expectedResult.Length, result.Length);
            for (int i = 0; i < result.Length; i++)
                AssertEquals(expectedResult[i], result[i], 0.001);

            // p_3
            expectedResult = new double[] { 0.32716, 0.43827, 0.18210, 0.05247 };
            result = matrix.GetPi(
                    new double[] { 2.0 / 3, 1.0 / 3, 0, 0 }, // p_0
                    3);
            AssertEquals(expectedResult.Length, result.Length);
            for (int i = 0; i < result.Length; i++)
                AssertEquals(expectedResult[i], result[i], 0.0001);

            // p_1
            expectedResult = new double[] { 1.0 / 3, 5.0 / 9, 1.0 / 9, 0 };
            result = matrix.GetPi(
                    new double[] { 2.0 / 3, 1.0 / 3, 0, 0 },  // p_0
                    1);
            AssertEquals(expectedResult.Length, result.Length);
            for (int i = 0; i < result.Length; i++)
                AssertEquals(expectedResult[i], result[i], 0.0001);
        }

        public static void GetProbabilityTest()
        {
            MarkovChain<string> matrix = new MarkovChain<string>(
                new double[,] {
                    {1.0 / 3,  2.0 / 3,  0,        0      },
                    {1.0 / 3,  1.0 / 3,  1.0 / 3,  0      },
                    {1.0 / 3,  1.0 / 3,  1.0 / 6,  1.0 / 6},
                    {0,        0,        0,        1}
                    }, "A", "B", "C", "D");

            double res = matrix.GetProbability(new double[] { 2.0 / 3, 1.0 / 3, 0, 0 }, "C", 2);
            AssertEquals(11.0 / 54, res, 0.001);
            res = matrix.GetProbability(new double[] { 2.0 / 3, 1.0 / 3, 0, 0 }, "A", 3);
            AssertEquals(0.32716, res, 0.00001);
            res = matrix.GetProbability(new double[] { 2.0 / 3, 1.0 / 3, 0, 0 }, "D", 1);
            AssertEquals(0, res, 0.001);

            res = matrix.GetProbability("B", "A", 2);
            AssertEquals(1.0 / 3, res, 0.001);
            res = matrix.GetProbability("B", "D", 3);
            AssertEquals(9.0 / 108, res, 0.0001);
            res = matrix.GetProbability("C", "A", 1);
            AssertEquals(1.0 / 3, res, 0.0001);
            res = matrix.GetProbability("C", "C", 0);
            AssertEquals(1.0, res, 0.0001);
            res = matrix.GetProbability("C", "D", 0);
            AssertEquals(0.0, res, 0.0001);
            res = matrix.GetProbability("C", "B", 0);
            AssertEquals(0.0, res, 0.0001);
            res = matrix.GetProbability("C", "A", 0);
            AssertEquals(0.0, res, 0.0001);
        }

        public static void GetChainProbabilityTest()
        {
            MarkovChain<char> markov = new MarkovChain<char>(
                new double[,] {
                    {1.0 / 3,  2.0 / 3,  0,        0      },
                    {1.0 / 3,  1.0 / 3,  1.0 / 3,  0      },
                    {1.0 / 3,  1.0 / 3,  1.0 / 6,  1.0 / 6},
                    {0,        0,        0,        1}
                    }, 'A', 'B', 'C', 'D');

            double res;
            res = markov.GetProbability('B', 'A', 2);
            AssertEquals(1.0 / 3, res, 0.0001);
            res = markov.GetProbability('B', 'D', 3);
            AssertEquals(9.0 / 108, res, 0.0001);

            res = markov.GetProbability('C', 'A', 1);
            AssertEquals(1.0 / 3, res, 0.0001);
            res = markov.GetProbability('C', 'D', 1);
            AssertEquals(1.0 / 6, res, 0.0001);
            res = markov.GetProbability('C', 'C', 0);
            AssertEquals(1.0, res, 0.0001);
            res = markov.GetProbability('C', 'A', 0);
            AssertEquals(0.0, res, 0.0001);
            res = markov.GetProbability('C', 'B', 0);
            AssertEquals(0.0, res, 0.0001);
            res = markov.GetProbability('C', 'D', 0);
            AssertEquals(0.0, res, 0.0001);

            // condition: state letter must be after B
            res = markov.GetProbability('C', 2, state => state > 'B');
            AssertEquals(12.0 / 36, res, 0.0001);
        }
    }
}
