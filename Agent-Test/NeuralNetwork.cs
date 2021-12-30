using System;
using AgentHelper;
using Enums;

namespace NeuralNetworks
{
    public static class NeuralNetwork
    {
        private static Layer _inputLayer = new Layer();
        private static Layer _outputLayer = new Layer();
        public static string[] Train()
        {
            return default;
        }
        public static string[] Evaluate()
        {


            return default;
        }
        public static int Predict(float[] inputData)
        {
            try
            {
                var inputLayer = Vector.Multiply(inputData, _inputLayer.Weights);

                var outputLayer = Vector.Multiply(inputLayer, _outputLayer.Weights);

                var probabilities = Activation.Softmax(outputLayer);

                return Vector.ArgumentMaximum(probabilities);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static float[] GetWeights(LayerEnum layerName)
        {
            if (layerName == LayerEnum.Input)
                return _inputLayer.Weights;
            else
                return _outputLayer.Weights;
        }
        public static void SetWeights(float[] weights, LayerEnum layerName)
        {
            if (layerName == LayerEnum.Input)
                _inputLayer.Weights = weights;
            else
                _outputLayer.Weights = weights;
        }
        public static void InitializeWeights(int numberOfNodes, LayerEnum layerName)
        {
            if (layerName == LayerEnum.Input)
                _inputLayer.InitializeWeights(numberOfNodes);
            else
                _outputLayer.InitializeWeights(numberOfNodes);
        }
        public static bool IsLayerTrained(LayerEnum layerName)
        {
            return layerName == LayerEnum.Input ? _inputLayer.IsTrained : _outputLayer.IsTrained;
        }
    }
    public class Layer
    {
        public bool IsTrained;
        public float[] Weights;
        public void InitializeWeights(int numberOfNodes, int minimumWeight = -1, int maximumWeight = 1)
        {
            Random rnd = new Random(0);
            Weights = new float[numberOfNodes];

            for (int i = 0; i < Weights.Length; ++i)
                Weights[i] = (maximumWeight - minimumWeight) * (float)rnd.NextDouble() + minimumWeight;
        }
    }
    public static class Vector
    {
        public static float[] Multiply(float[] vector1, float[] vector2)
        {
            int i = 0;

            try
            {
                var results = new float[vector2.Length];

                for (; i < results.Length; i++)
                {
                    results[i] = Sum(Multiply(vector1, vector2[i]));
                }

                return results;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error:\n\tVector->V2V Multiply| index: {i}, array1 size: {vector1.Length}, array2 size: {vector2.Length}.\n\tMessage: {ex.Message}.");
            }
        }
        public static float[] Multiply(float[] vector, float value)
        {
            try
            {
                var results = new float[vector.Length];

                for (int i = 0; i < vector.Length; i++)
                {
                    results[i] = vector[i] * value;
                }

                return results;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static float[] Subtract(float[] vector1, float[] vector2)
        {
            try
            {
                if (vector1.Length != vector2.Length)
                    return null;

                var results = new float[vector1.Length];

                for (int i = 0; i < vector1.Length; i++)
                {
                    results[i] = vector1[i] - vector2[i];
                }

                return results;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static float[] Subtract(float[] vector, float value)
        {
            try
            {
                var results = new float[vector.Length];

                for (int i = 0; i < vector.Length; i++)
                {
                    results[i] = vector[i] - value;
                }

                return results;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static float Sum(float[] vector)
        {
            int i = 0;

            try
            {
                float sum = 0;

                for (; i < vector.Length; i++)
                {
                    sum += vector[i];
                }

                return sum;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error:\n\tVector->Sum| index: {i}, array size: {vector.Length}.\n\tMessage: {ex.Message}.");
            }
        }
        public static float Minimum(float[] vector)
        {
            try
            {
                float minimum = vector[0];

                for (int i = 1; i < vector.Length; i++)
                {
                    if (vector[i] < minimum)
                        minimum = vector[i];
                }

                return minimum;
            }
            catch (Exception)
            {
                return 0f;
            }
        }
        public static float Maximum(float[] vector)
        {
            try
            {
                float maximum = vector[0];

                for (int i = 1; i < vector.Length; i++)
                {
                    if (vector[i] > maximum)
                        maximum = vector[i];
                }

                return maximum;
            }
            catch (Exception)
            {
                return 0f;
            }
        }
        public static int ArgumentMaximum(float[] vector)
        {
            try
            {
                int maximum = 0;

                for (int i = 1; i < vector.Length; i++)
                {
                    if (vector[i] > maximum)
                        maximum = i;
                }

                return maximum;
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static float[] ToVector(float input1, float input2)
        {
            try
            {
                return new float[] { input1, input2 };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
    public static class Activation
    {
        public static float ReLu(float input_value)
        {
            try
            {
                if (input_value < 0f)
                    return 0f;
                else
                    return input_value;
            }
            catch (Exception)
            {
                return 0f;
            }
        }
        public static float[] Softmax(float[] hoSums)
        {
            float max = hoSums[0];

            for (int i = 0; i < hoSums.Length; ++i)
                if (hoSums[i] > max)
                    max = hoSums[i];

            float scale = 0f;

            for (int i = 0; i < hoSums.Length; ++i)
                scale += (float)Math.Exp(hoSums[i] - max);

            float[] result = new float[hoSums.Length];

            for (int i = 0; i < hoSums.Length; ++i)
                result[i] = (float)Math.Exp(hoSums[i] - max) / scale;

            return result;
        }
    }
}
