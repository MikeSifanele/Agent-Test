using System;
using AgentHelper;

namespace EvolutionaryOptimization
{
    public class Individual : IComparable<Individual>
    {
        public double[] Chromosome;
        public double Fitness;
        private int _numberOfChromosomes;
        private double _minumumChromosome;
        private double _maximumChromosome;
        private double _mutateRate;
        private double _precision;
        static Random _rnd = new Random(0);
        public Individual(int numberOfChromosomes, double minumumChromosome, double maximumChromosome, double mutateRate, double precision)
        {
            _numberOfChromosomes = numberOfChromosomes;
            _minumumChromosome = minumumChromosome;
            _maximumChromosome = maximumChromosome;
            _mutateRate = mutateRate;
            _precision = precision;
            Chromosome = new double[numberOfChromosomes];

            for (int i = 0; i < Chromosome.Length; ++i)
                Chromosome[i] = (maximumChromosome - minumumChromosome) * _rnd.NextDouble() + minumumChromosome;

            Fitness = Problem.Fitness(Chromosome);
        }
        public void Mutate()
        {
            double hi = _precision * _maximumChromosome;
            double lo = -hi;

            for (int i = 0; i < Chromosome.Length; ++i)
            {
                if (_rnd.NextDouble() < _mutateRate)
                    Chromosome[i] += (hi - lo) * _rnd.NextDouble() + lo;
            }
        }
        public int CompareTo(Individual other)
        {
            if (Fitness < other.Fitness) return -1;
            else if (Fitness > other.Fitness) return 1;
            else return 0;
        }
    }
    public class Evolver
    {
        private int _populationSize;
        private Individual[] _population;
        private int _numberOfChromosomes;
        private double _minumumChromosome;
        private double _maximumChromosome;
        private double _mutateRate;
        private double _precision;
        private double _tau;
        private int[] _indices;
        private int _maxumumGenerations;
        private static Random _rnd = null;
        public Evolver(int populationSize, int numberOfChromosomes, double minumumChromosome, double maximumChromosome, double mutateRate, double precision, double tau, int maxumumGenerations)
        {
            _populationSize = populationSize;
            _population = new Individual[populationSize];

            for (int i = 0; i < _population.Length; ++i)
                _population[i] = new Individual(numberOfChromosomes, minumumChromosome, maximumChromosome, mutateRate, precision);

            _numberOfChromosomes = numberOfChromosomes;
            _minumumChromosome = minumumChromosome;
            _maximumChromosome = maximumChromosome;
            _mutateRate = mutateRate;
            _precision = precision;
            _tau = tau;
            _indices = new int[populationSize];

            for (int i = 0; i < _indices.Length; ++i)
                _indices[i] = i;

            _maxumumGenerations = maxumumGenerations;
            _rnd = new Random(0);
        }
        public double[] Evolve()
        {
            double bestFitness = _population[0].Fitness;
            double[] bestChomosome = new double[_numberOfChromosomes];

            _population[0].Chromosome.CopyTo(bestChomosome, 0);

            int generation = 0;

            while (generation < _maxumumGenerations)
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

            Individual child1 = new Individual(_numberOfChromosomes, _minumumChromosome, _maximumChromosome, _mutateRate, _precision);
            Individual child2 = new Individual(_numberOfChromosomes, _minumumChromosome, _maximumChromosome, _mutateRate, _precision);

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
            Individual immigrant = new Individual(_numberOfChromosomes, _minumumChromosome, _maximumChromosome, _mutateRate, _precision);

            _population[_populationSize - 3] = immigrant;
        }
    }
    public class Problem
    {
        public static double Fitness(double[] chromosome)
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