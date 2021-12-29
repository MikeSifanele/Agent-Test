using System;
using AgentHelper;
using Enums;

namespace NeuralNetworks
{
    public static class NeuralNetwork
    {
        private static float[] _inputWeights;
        private static float[] _outputWeights;
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
                var inputLayer = Vector.Multiply(inputData, _inputWeights);

                var outputLayer = Vector.Multiply(inputLayer, _outputWeights);

                var probabilities = Activation.Softmax(outputLayer);

                return Vector.ArgumentMaximum(probabilities);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static float[] GetWeights(LayersEnum layerName)
        {
            if (layerName == LayersEnum.Input)
                return _inputWeights;
            else
                return _outputWeights;
        }
        public static void SetWeights(float[] weights, LayersEnum layerName)
        {
            if (layerName == LayersEnum.Input)
                _inputWeights = weights;
            else
                _outputWeights = weights;
        }
    }
    public static class Vector
    {
        public static float[] Multiply(float[] vector1, float[] vector2)
        {
            try
            {
                if (vector1.Length != vector2.Length)
                    return null;

                var results = new float[vector1.Length];

                for (int i = 0; i < vector1.Length; i++)
                {
                    results[i] = vector1[i] * vector2[i];
                }

                return results;
            }
            catch (Exception)
            {
                return null;
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
            try
            {
                float sum = 0;

                for (int i = 0; i < vector.Length; i++)
                {
                    sum += vector[i];
                }

                return sum;
            }
            catch (Exception)
            {
                return 0f;
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
