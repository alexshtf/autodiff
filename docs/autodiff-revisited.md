# Motivation

Although the main objective of the library is to save the developer the tedious work of computing gradients, one of its secondary objectives is performance. The library allows achieving high performance in scenarios where the user needs to compute the gradient or the value of the same function many times at different points. A good example for such scenarios is optimization. An optimization algorithm needs to repeatedly compute the gradient and value of a function at many candidate points, until convergence. 

# The basics
The following code demonstrate term compilation:

```c#
Varaible x = new Variable();
Variable y = new Variable();
Term term = x + y * TermBuilder.Exp(x + y);
ICompiledTerm compiledTerm = term.Compile(x, y);
```

The first three lines define a function. Up until here there is nothing different from what you saw in the previous tutorial. The fourth line performs the compilation of `term`. The `Compile` extension method receives an array of variables that are used to define our function. Like you saw in the previous tutorial, the order of variables matters, as we will see later.

# Computing gradients
Now that we have our super-ninja-fast compiled term we can start computing gradients and values. Computing a value is simple. Use the `ICompiledTerm.Evaluate` method. Here is an example:
```c#
// compute the value at x = 2, y = -3
double value = compiledTerm.Evaluate(2, -3);
```
AutoDiff knows that 2 is the value for `x` and -3 is the value for `y` because of the order of variables given to `Compile`. Gradient computation is similarily done by calling the `ICompiledTerm.Differentiate` method. There is one difference - this method returns a tuple. The first item is the gradient and the second item is the value. The function value is a by-product of the gradient computation algorithm, so we get it for free. Here is a code example:
```c#
// compute value + gradient at x = 3, y = -5
Tuple<double[](), double> diff = compiledTerm.Differentiate(3, -5);
double[]() gradient = diff.Item1;
double value = diff.Item2;
```
# Behold the power
As stated before, there is a good reason to use the slightly more complicated compiled terms instead of simply invoking evaluation and differentiation methods on our terms. The reason is performance. You can see a real-world usage in the next two tutorials. However here is a small example that demonstrates the gains:
```c#
Random random = new Random();
for(int i = 0; i < 10000; ++i)
{
    double a = random.NextDouble();
    double b = random.NextDouble();
    Tuple<double[], double> diff = compiledTerm.Differentiate(a, b);
}
```
If you do the same work by invoking Evaluate/Differentiate on the original (not compiled) term, you will see the huge difference in performance. You can compare it to the running-time of the following code and see the difference yourself:
```c#
Random random = new Random();
Variable[] variables = {x, y};
double[] values = new double[2];
for(int i = 0; i < 10000; ++i)
{
    values[0] = random.NextDouble();
    values[1] = random.NextDouble();
    double[] gradient = term.Differentiate(variables, values);
}
```

# Summary
* AutoDiff allows compiling a term to create a more efficient internal representation that allows high performance
* It is beneficial to compile terms when the same function needs to be repeatedly evaluated or differentiated
* The `Compile` extension method compiles terms. It is given the term's variables, and the order does matter.
* The methods `ICompileTerm.Evaluate` and `ICompiledTerm.Differentiate` are used to evaluate and differentiate our compiled term at different points. 