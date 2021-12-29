using System;
using AgentHelper;
using NeuralNetworks;

namespace EvolutionaryOptimization
{
    public class Individual : IComparable<Individual>
    {
        public float[] Chromosome;
        public float Fitness;
        private float _maximumChromosome;
        private float _mutateRate;
        private float _precision;
        static Random _rnd = new Random(0);
        public Individual(int numberOfChromosomes, float minumumChromosome, float maximumChromosome, float mutateRate, float precision)
        {
            _maximumChromosome = maximumChromosome;
            _mutateRate = mutateRate;
            _precision = precision;
            Chromosome = new float[numberOfChromosomes];

            for (int i = 0; i < Chromosome.Length; ++i)
                Chromosome[i] = (maximumChromosome - minumumChromosome) * (float)_rnd.NextDouble() + minumumChromosome;

            Fitness = Problem.Fitness(Chromosome);
        }
        public void Mutate()
        {
            float hi = _precision * _maximumChromosome;
            float lo = -hi;

            for (int i = 0; i < Chromosome.Length; ++i)
            {
                if (_rnd.NextDouble() < _mutateRate)
                    Chromosome[i] += (hi - lo) * (float)_rnd.NextDouble() + lo;
            }
        }
        public int CompareTo(Individual other)
        {
            if (Fitness > other.Fitness) return -1;
            else if (Fitness < other.Fitness) return 1;
            else return 0;
        }
    }
    public class Evolver
    {
        private int _populationSize;
        private Individual[] _population;
        private int _numberOfChromosomes;
        private float _minimumChromosome;
        private float _maximumChromosome;
        private float _mutateRate;
        private float _precision;
        private float _tau;
        private int[] _indices;
        private int _maximumGenerations;
        private static Random _rnd = null;
        public Evolver(int populationSize, int numberOfChromosomes, float minimumChromosome, float maximumChromosome, float mutateRate, float precision, float tau, int maximumGenerations)
        {
            _populationSize = populationSize;
            _population = new Individual[populationSize];

            for (int i = 0; i < _population.Length; ++i)
                _population[i] = new Individual(numberOfChromosomes, minimumChromosome, maximumChromosome, mutateRate, precision);

            _numberOfChromosomes = numberOfChromosomes;
            _minimumChromosome = minimumChromosome;
            _maximumChromosome = maximumChromosome;
            _mutateRate = mutateRate;
            _precision = precision;
            _tau = tau;
            _indices = new int[populationSize];

            for (int i = 0; i < _indices.Length; ++i)
                _indices[i] = i;

            _maximumGenerations = maximumGenerations;
            _rnd = new Random(0);
        }
        public float[] Evolve()
        {
            float bestFitness = _population[0].Fitness;
            float[] bestChomosome = new float[_numberOfChromosomes];

            _population[0].Chromosome.CopyTo(bestChomosome, 0);

            int generation = 0;

            while (generation < _maximumGenerations)
            {
                Individual[] parents = Select(2);
                Individual[] children = Reproduce(parents[0], parents[1]);

                Accept(children[0], children[1]);
                Immigrate();

                for (int i = _populationSize - 3; i < _populationSize; ++i)
                {
                    if (_population[i].Fitness < bestFitness)
                    {
                        bestFitness = _population[i].Fitness;
                        _population[i].Chromosome.CopyTo(bestChomosome, 0);
                    }
                }
                ++generation;
            }
            return bestChomosome;
        }
        private Individual[] Select(int n)
        {
            int tournamentSize = (int)(_tau * _populationSize);

            if (tournamentSize < n) 
                tournamentSize = n;

            Individual[] candidates = new Individual[tournamentSize];

            ShuffleIndexes();

            for (int i = 0; i < tournamentSize; ++i)
                candidates[i] = _population[_indices[i]];

            Array.Sort(candidates);

            Individual[] results = new Individual[n];

            for (int i = 0; i < n; ++i)
                results[i] = candidates[i];

            return results;
        }
        private void ShuffleIndexes()
        {
            for (int i = 0; i < _indices.Length; ++i)
            {
                int r = _rnd.Next(i, _indices.Length);
                int tmp = _indices[r];
                _indices[r] = _indices[i];
                _indices[i] = tmp;
            }
        }
        private Individual[] Reproduce(Individual parent1, Individual parent2)
        {
            int cross = _rnd.Next(0, _numberOfChromosomes - 1);

            Individual child1 = new Individual(_numberOfChromosomes, _minimumChromosome, _maximumChromosome, _mutateRate, _precision);
            Individual child2 = new Individual(_numberOfChromosomes, _minimumChromosome, _maximumChromosome, _mutateRate, _precision);

            for (int i = 0; i <= cross; ++i)
                child1.Chromosome[i] = parent1.Chromosome[i];
            for (int i = cross + 1; i < _numberOfChromosomes; ++i)
                child2.Chromosome[i] = parent1.Chromosome[i];
            for (int i = 0; i <= cross; ++i)
                child2.Chromosome[i] = parent2.Chromosome[i];
            for (int i = cross + 1; i < _numberOfChromosomes; ++i)
                child1.Chromosome[i] = parent2.Chromosome[i];

            child1.Mutate(); child2.Mutate();

            child1.Fitness = Problem.Fitness(child1.Chromosome);
            child2.Fitness = Problem.Fitness(child2.Chromosome);

            Individual[] result = new Individual[] { child1 , child2 };

            return result;
        }
        private void Accept(Individual child1, Individual child2)
        {
            Array.Sort(_population);

            _population[_populationSize - 1] = child1;
            _population[_populationSize - 2] = child2;
        }
        private void Immigrate()
        {
            Individual immigrant = new Individual(_numberOfChromosomes, _minimumChromosome, _maximumChromosome, _mutateRate, _precision);

            _population[_populationSize - 3] = immigrant;
        }
    }
    public class Problem
    {
        public static double Fitness(double[] chromosomes)
        {
            int reward = 0;

            while (!MLTrader.Instance.IsLastStep)
            {
                _ = MLTrader.Instance.GetObservation();

                reward += MLTrader.Instance.GetReward(action: 0);
            }

            return reward;
        }
    }
}