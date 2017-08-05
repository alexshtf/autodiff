**Getting started**
Let us start by downloading the library. Please go to [Releases](http://autodiff.codeplex.com/releases/) and download the latest release of the library. Unzip the files and put them in your favorite folder. Then open Visual Studio 2010, start a new console project, and add the reference to AutoDiff.dll in the folder where you unzipped the downloaded files. Open the {"program.cs"} file and add {{using AutoDiff;}} to the list of using statements. Your code should look like this:
{code:c#}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoDiff;

namespace MyFirstAutodiffProgram
{
    class Program
    {
        static void Main(string[]() args)
        {
        }
    }
}
{code:c#}


**Defining a function**
The main purpose of this library is to allow its user to compute gradients. When we create a program that solved a problem using gradients we want to be able to quickly prototype and play with our functions without the need to manually compute the gradient with pen and paper. So let's see how simple it is. In the following example we will compute the gradient of the function:
:{{f(x,y) = (x+y) * log(exp(x) + exp(y))}}
So let us first define our function in code. Add the following lines of code to your {{Main}} method:
{code:c#}
Variable x = new Variable();
Variable y = new Variable();
Term func = (x + y) * TermBuilder.Log(TermBuilder.Exp(x) + TermBuilder.Exp(y));
{code:c#}
The first two lines of code created two objects {{x}} and {{y}} that represent our function's variables. The third line of code created an object that represents our function. The AutoDiff library uses the type {{Term}} to represent analytic terms (or expressions) that define our functions. Everything is a {{Term}}, even variables ({{x}} and {{y}} in this example). The class {{TermBuilder}}, as its name says, is used to construct new terms out of existing ones.

Now we can easily compute the value and the gradient at any given point. Let us compute the value and the gradient of this function at **x** = 2, **y** = -1. Add the following lines of code to the {{Main}} method:
{code:c#}
double value = func.Evaluate(new Variable[]()() { x, y }, new double[]()() { 2, -1 });
double[]()()() grad = func.Differentiate(new Variable[]()()() { x, y }, new double[]()()() { 2, -1 });
{code:c#}
As you can probably guess, the {{Evaluate}} method computes the value of our term by assigning values to variables and the function {{Differentate}} computes the gradient. The first parameter to both functions is an array with all the function's variables. The second parameter is the assignment of values to those variables. It is important to note that _order does matter_ - the first variable in the variables array is assigned the first value in the values array, and so on. In addition, order matters for the gradient too. In our example, {{grad[0](0)}} is the partial derivative with respect to {{x}} because we provided {{x}} as the first element of the variables array and {{grad[1](1)}} is the partial derivative with respect to {{y}} because we put {{y}} to be the second element of the variables array.

**Summary**
This tutorial demonstrates the most basic usage of the AutoDiff library - defining functions and computing their values and gradients. Here are the key points:
* Objects of type {{Term}} are used to define functions analytically
* Evaluate/Differentiate methods can be used to compute the value/gradient of a function given assignment of values to variables
* The order of variables given to Evaluate/Differentiate matters