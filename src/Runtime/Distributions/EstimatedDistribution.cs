﻿using Microsoft.ML.Probabilistic.Math;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Microsoft.ML.Probabilistic.Distributions
{
    /// <summary>
    /// Represent an approximate distribution for which we can estimate the error of CDF.
    /// </summary>
    /// <remarks>
    /// Default implementation of this class considers the estimation as exact.
    /// </remarks>
    public class EstimatedDistribution : IEstimatedDistribution
    {
        private readonly ITruncatableDistribution<double> distribution;

        public EstimatedDistribution(CanGetProbLessThan<double> distribution)
        {
            if (distribution is EstimatedDistribution estimatedDistribution)
            {
                // Avoids an extra layer of wrapping.
                this.distribution = estimatedDistribution.distribution;
            }
            else
            {
                this.distribution = new TruncatableDistribution<double>(distribution);
            }
        }

        /// <summary>
        /// Returns an instance of <see cref="EstimatedDistribution"/> representing the given <paramref name="distribution"/>
        /// as if it estimates the real distribution exactly.
        /// </summary>
        /// <param name="distribution"></param>
        /// <returns></returns>
        public static EstimatedDistribution NoError(CanGetProbLessThan<double> distribution)
            => new EstimatedDistribution(distribution);

        /// <inheritdoc/>
        public virtual double GetProbBetweenError(double approximateProb)
        {
            return 0.0; // the estimation is exact by default
        }

        /// <inheritdoc/>
        public virtual Interval GetExpectation(double maximumError, Interval left, Interval right, bool preservesPoint, Func<Interval, Interval> function, CancellationToken cancellationToken)
        {
            return Interval.GetExpectation(maximumError, cancellationToken, left, right, this, preservesPoint, function);
        }

        /// <inheritdoc/>
        public double GetProbBetween(double lowerBound, double upperBound) => distribution.GetProbBetween(lowerBound, upperBound);

        /// <inheritdoc/>
        public double GetProbLessThan(double x) => distribution.GetProbLessThan(x);

        /// <inheritdoc/>
        public double GetQuantile(double probability) => distribution.GetQuantile(probability);

        /// <inheritdoc/>
        public ITruncatableDistribution<double> Truncate(double lowerBound, double upperBound) => distribution.Truncate(lowerBound, upperBound);

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.distribution.ToString();
        }
    }
}
