# Project Description
A library that provides fast, accurate and automatic differentiation (computes derivative /  gradient) of mathematical functions.

AutoDiff provides a simple and intuitive API for computing function gradients/derivatives along with a fast state-of-the-art algorithm for performing the computation. Such computations are mainly useful in numeric optimization scenarios.

The Library is available via NuGet:

```powershell
Install-Package AutoDiff
```

# Code example
```c#
using AutoDiff;

class Program
{
    public static void Main(string[]() args)
    {
            // define variables
            var x = new Variable();
            var y = new Variable();
            var z = new Variable();
    
            // define our function
            var func = (x + y) * TermBuilder.Exp(z + x * y);
    
            // prepare arrays needed for evaluation/differentiation
            Variable[] vars = { x, y, z };
            double[] values = {1, 2, -3 };
    
            // evaluate func at (1, 2, -3)
            double value = func.Evaluate(vars, values);
    
            // calculate the gradient at (1, 2, -3)
            double[] gradient = func.Differentiate(vars, values);
    
            // print results
            Console.WriteLine("The value at (1, 2, -3) is " + value);
            Console.WriteLine("The gradient at (1, 2, -3) is ({0}, {1}, {2})", gradient[0](0), gradient[1](1), gradient[2](2));
    }
}
```



# Documentation
The [Documentation](docs/Readme.md) contains some basic tutorials, we have an [article](http://www.codeproject.com/KB/library/Automatic_Differentiation.aspx) on CodeProject, and finally source code contains some code examples in addition to the code of the library itself.

# Motivation
There are many open and commercial .NET libraries that have numeric optimization as one of their features (for example, [Microsoft Solver Foundation](http://msdn.microsoft.com/en-us/devlabs/hh145003.aspx),  [AlgLib](http://www.alglib.net),[Extreme Optimization](http://www.extremeoptimization.com/), [CenterSpace NMath](http://www.centerspace.net/)) . Most of them require the user to be able to evaluate the function and the function's gradient. This library tries to save the work in manually developing the function's gradient and coding it.
Once the developer defines his/her function, the AutoDiff library can automatically evaluate and differentiate this function at any point. This allows +easy development and prototyping+ of applications based on numeric optimization.

# Features
* Fast! See [0.5 vs 0.3 benchmark](docs/0.5-vs-0.3-benchmark.md) and [0.3 benchmark](doss/0.3-benchmark).
* Composition of functions using arithmetic operators, Exp, Log, Power and user-defined unary and binary functions.
* Function gradient evaluation at specified points
* Function value evaluation at specified points
* Uses [Code Contracts](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.contracts.contract) for specifying valid parameters and return values
* Computes gradients using Reverse-Mode AD algorithm in **linear time**!
  * Yes, it's faster than numeric approximation for multivariate functions
  * You get both high accuracy and speed!

# **Using in research papers**

If you like the library and it helps you publish a research paper, please cite the paper I originally wrote the library for [geosemantic.bib](docs/Home_geosemantic.bib)

# Used by

* **Andreas Witsch,  Hendrik Skubch, Stefan Niemczyk, Kurt Geihs** [Using incomplete satisfiability modulo theories to determine robotic tasks](http://dx.doi.org/10.1109/IROS.2013.6697046) _Intelligent Robots and Systems (IROS), 2013 IEEE/RSJ International Conference_
* **Alex Shtof, Alexander Agathos, Yotam Gingold, Ariel Shamir, Daniel Cohen-Or** [Geosemantic Snapping for Sketch-Based Modeling](http://onlinelibrary.wiley.com/doi/10.1111/cgf.12044/abstract) _Eurographics 2013 proceedings_  ([code repository](https://bitbucket.org/alexshtf/sketchmodeller))
* **Michael Kommenda, Gabriel Kronberger, Stephan Winkler, Michael Affenzeller, Stefan Wagner** [Effects of constant optimization by nonlinear least squares minimization in symbolic regression](http://dl.acm.org/citation.cfm?id=2482691) _Proceeding of the fifteenth annual conference companion on Genetic and evolutionary computation conference companion_
* **Hendrik Skubch**, [Solving non-linear arithmetic constraints in soft realtime environments](http://dl.acm.org/citation.cfm?id=2245293) _Proceedings of the 27th Annual ACM Symposium on Applied Computing_
* [AlicaEngine](http://ros.org/wiki/AlicaEngine) - A cooperative planning engine for robotics. You can see it in action in this [video](http://www.youtube.com/watch?v=HhIrhU19PG4)
* [HeuristicsLab](http://dev.heuristiclab.com) - a framework for heuristic and evolutionary algorithms that is developed by members of the [Heuristic and Evolutionary Algorithms Laboratory (HEAL)](http://heal.heuristiclab.com/)
