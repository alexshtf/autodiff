using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoDiff;

namespace NeuralNetworkSample
{
    /// <summary>
    /// In this sample we will build an artificial neural network that recognizes the digit 5.
    /// The neural network receives binary images of size 20 x 40 pixels, has 1 hidden layer where
    /// each neuron is connected to 4 x 5 pixels sub-image, and one output layer.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // create our neural network
            Neuron[,] hiddenLayer = new Neuron[5, 8]; // 5 x 8 hidden layer neurons cover the whole 20x40 pixels image
            Neuron outputNeuron = new Neuron(5, 8); // output layer covers all 5x8 hideen layer neurons
            for (int i = 0; i < 5; ++i)
                for (int j = 0; j < 5; ++j)
                    hiddenLayer[i, j] = new Neuron(4, 5); // each hideen layer neuron coverx 4x5 pixels

            // constructs a dictionary that maps each variable to information describing what weight
            // in which neuron it represents.
            Dictionary<Variable, VariableInfo> variableInfos = GetVariableInfos(EnumerateAllNeurons(hiddenLayer, outputNeuron));
        }

        private static Term EvalNetwork(Neuron[,] hiddenLayer, Neuron outputNeuron, double[,] input, int hiddenWidth, int hiddenHeight)
        {
            var width = input.GetLength(0);
            var height = input.GetLength(1);
            var cellWidth = width / hiddenWidth;
            var cellHeight = height / hiddenHeight;

            foreach (var idx in Indices(hiddenLayer))
            {
                Term[,] hiddenEval = new Term[hiddenWidth, hiddenHeight];
                for (int i = 0; i < width; i += cellWidth)
                {
                    for (int j = 0; j < height; j += cellHeight)
                    {
                        var inputPortion = new Term[cellWidth, cellHeight];
                        ToTerms(input, i, j, inputPortion);
                        hiddenEval[i, j] = hiddenLayer[i, j].Eval(inputPortion);
                    }
                }
            }

            throw new NotSupportedException();
        }

        private static void ToTerms(double[,] input, int x, int y, Term[,] output)
        {
            for (int i = 0; i < output.GetLength(0); ++i)
                for (int j = 0; j < output.GetLength(1); ++j)
                    output[i, j] = input[i + x, j + y];
        }

        private static IEnumerable<Tuple<int, int>> Indices<T>(T[,] array)
        {
            for (int i = 0; i < array.GetLength(0); ++i)
                for (int j = 0; j < array.GetLength(1); ++j)
                    yield return Tuple.Create(i, j);
        }

        private static IEnumerable<Neuron> EnumerateAllNeurons(Neuron[,] hiddenLayer, Neuron outputNeuron)
        {
            var hiddenNeurons =
                from idx in Indices(hiddenLayer)
                select hiddenLayer[idx.Item1, idx.Item2];
            return hiddenNeurons.Concat(Enumerable.Repeat(outputNeuron, 1));
        }

        private static Dictionary<Variable, VariableInfo> GetVariableInfos(IEnumerable<Neuron> neurons)
        {
            var result = new Dictionary<Variable, VariableInfo>();
            foreach (var neuron in neurons)
            {
                var weightVariables =
                    from idx in Indices(neuron.WeightVariables)
                    let variable = neuron.WeightVariables[idx.Item1, idx.Item2]
                    let info = new VariableInfo { Row = idx.Item1, Col = idx.Item2, IsBias = false, Neuron = neuron }
                    select Tuple.Create(variable, info);
                
                foreach (var pair in weightVariables)
                    result[pair.Item1] = pair.Item2;
                result[neuron.BiasVariable] = new VariableInfo { IsBias = true, Neuron = neuron };       
            }
            return result;
        }
    }

    class Neuron
    {
        public double[,] Weights; // used for operation
        public double Bias;
        public Variable[,] WeightVariables; // used for training
        public Variable BiasVariable;

        public Neuron(int width, int height)
        {
            Weights = new double[width, height];
            WeightVariables = new Variable[width, height];
            for (int i = 0; i < width; ++i)
                for (int j = 0; j < height; ++j)
                    WeightVariables[i, j] = new Variable();
        }

        public double Eval(double[,] input)
        {
            // calculate weighed and biased sum
            double sum = Bias;
            for (int i = 0; i < input.GetLength(0); ++i)
                for (int j = 0; j < input.GetLength(1); ++j)
                    sum += input[i, j] * Weights[i, j];

            // apply activation function f(x) = (1 + e^-x)^(-1)
            return 1 / (1 + Math.Exp(-sum));
        }

        public Term Eval(Term[,] input)
        {
            // calculate weighes sum terms
            var sumTerms =
                from i in Enumerable.Range(0, input.GetLength(0))
                from j in Enumerable.Range(0, input.GetLength(1))
                select input[i, j] * WeightVariables[i, j];

            // add the bias
            sumTerms = sumTerms.Concat(Enumerable.Repeat(BiasVariable, 1));

            // get the term for the weighes and biased sum
            var sum = TermBuilder.Sum(sumTerms);

            // get the term for the activation function f(x) = (1 + e^-x)^(-1)
            return TermBuilder.Power(1 + TermBuilder.Exp(-sum), -1);
        }
    }

    class VariableInfo
    {
        public int Row { get; set; }
        public int Col { get; set; }
        public Neuron Neuron { get; set; }
        public bool IsBias { get; set; }
    }
}
