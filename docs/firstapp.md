# Getting started

Let us start by creating a new project, and adding a NuGet package reference to AutoDiff. Then, modify your `program.cs` file to have the following content:

```c#
using System;
using AutoDiff;

namespace MyFirstAutodiffProgram
{
    class Program
    {
        static void Main(string[] args)
        {
        }
    }
}
```


# Defining a mathematical function
The main purpose of this library is to allow its user to compute gradients of mathematical functions, so let's see how simple it is. In the following example we will compute the gradient of the function:

> f(x,y) = (x+y) * log(exp(x) + exp(y))

We begin by defining our function in code. Add the following lines of code to the `Main` method:

```c#
Variable x = new Variable();
Variable y = new Variable();
Term func = (x + y) * TermBuilder.Log(TermBuilder.Exp(x) + TermBuilder.Exp(y));
```

The first two lines of code create two objects `x` and `y` that represent the function's variables. The third line of code creates an object that represents our function. 

The AutoDiff library uses the `Term` type to represent analytic terms (or expressions) that define our functions. Everything is a `Term`, even variables (`x` and `y` in this example). The class `TermBuilder`, as its name says, is used to construct new terms out of existing ones.

Now we can easily compute the value and the gradient at any given point. Let us compute the value and the gradient of this function at **x** = 2, **y** = -1. Add the following lines of code to the `Main` method:

```c#
var point = new double[] { 2, -1};
var variables = new Variable[] { x, y };
double value = func.Evaluate(variables, point);
double[] grad = func.Differentiate(variables, point);
```

As you can probably guess, the `Evaluate` method computes the value of the term `func` by assigning values to variables, and the `Differentiate` function computes the gradient. The first parameter to both functions is an array with all the function's variables. The second parameter is the assignment of values to those variables. 

It is important to note that _order does matter_ - the first variable in the variables array is assigned the first value in the values array, and so on. In addition, order matters for the gradient too. In our example, ``grad[0]`` is the partial derivative with respect to ``x`` because we provided ``x`` as the first element of the variables array, and similarly `grad[1]` is the partial derivative with respect to ``y``.

**Summary**
This tutorial demonstrates the most basic usage of the AutoDiff library - defining functions and computing their values and gradients. Here are the key points:
* Objects of type `Term` are used to define functions analytically
* The `Evaluate`/`Differentiate` methods can be used to compute the value/gradient of a function given assignment of values to variables
* The order of variables given to the `Evaluate and` `Differentiate` methods matters