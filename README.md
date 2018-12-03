# **Important : Issue has been disabled because NConcern has been redesigned, improved and made reliable through the [Puresharp](https://github.com/Virtuoze/Puresharp) project**


[![NuGet](https://img.shields.io/nuget/v/nconcern.svg)](https://www.nuget.org/packages/NConcern)
# NConcern .NET AOP Framework
NConcern is a .NET runtime AOP (Aspect-Oriented Programming) lightweight framework written in C# that reduces tangling caused by cross-cutting concerns. Its role is to introduce Aspect-Oriented Programming paradigm with a minimum cost to maximize quality and productivity.


## Features
NConcern AOP Framework is based on code injection at runtime.

- non-intrusive : no need to adapt source code.
- friendly : delegates, expressions (linq) or CIL (ILGenerator) can be used to define an aspect
- no configuration : additional configuration files are not required
- no proxy : decoration by inheritance and factory pattern are not required
- low learning curve : get started under 20 minutes
- no installer : a nuget to install for aspects definition and another to make assembly injectable.
- suited for unit testing : weaving is controlled at runtime
- low performance overhead : injection mechanic is built to be efficient
- limitless : all kind of methods (constructors included) are supported
- runtime lifecycle : aspect can be updated/created/removed at runtime


## Aspect
An aspect represents a set of features related to a specific concern. The role of an aspect is to provide advices for method. It can be added and removed at runtime.


## Advice
An advice is the code added by an aspect to complete a method. It can be implemented using delegate, expression or CIL generation.

- Before : runs before method execution
- After : runs after method execution regardless of its outcome
- After.Returning : runs after method execution only if it completes sucessfully
- After.Throwing : runs after method execution only if it exits by throwing an exception
- Around : runs around method execution


## Example

- Use case :
```
I want to trace my service calls with "Console.WriteLine".
My services are OperationContract typed (WCF service)
```

- Calculator : WCF service
```
[ServiceContract]
public class Calculator
{
    [OperationContract]
    public int Add(int a, int b)
    {
       return a + b;
    }
}
```

- Install [CNeptune Urbanization .NET](https://www.nuget.org/packages/CNeptune/) nuget package on target assembly (where WCF is defined) to make it injectable
```
PM> Install-Package CNeptune
```

- Tracer : simple tracer to log into Console
```
static public class Tracer
{
    static public void Trace(MethodBase method, object[] arguments)
    {
        Console.WriteLine("{0}({1})", method.Name, string.Join(", ", arguments));
    }
}
```

- Install [NConcern .NET AOP Framework](https://www.nuget.org/packages/NConcern/) nuget package on assembly where Aspects will be defined
```
PM> Install-Package NConcern
```

- Logging (Aspect) : define how "Tracer" can be injected into a method
```
public class Logging : IAspect
{
    public IEnumerable<IAdvice> Advise(MethodBase method)
    {
        yield return Advice.Basic.Before((instance, arguments) => 
        {
            Tracer.Trace(method, arguments);
        });
    }
}
```

- Enable logging for services using implicit joinpoint
```
Aspect.Weave<Logging>(typeof(OperationContractAttribute));
```

- Disable logging for services using implicit joinpoint
```
Aspect.Release<Logging>(typeof(OperationContractAttribute));
```


## FAQ

- **How this AOP Framework is different from the others?** 
_Most of time developping cross-cutting source code required reflection and boxing to be done. NConcern offer a way to define it using Linq Expressions or ILGenerator because cross-cutting source code have to manage not statically known datas. No need factory and no need base class is the second exclusive feature that make the difference because interception is not based on method overriding, MarshalByRef nor ContextBoundObject._

- **How fast is "low performance overhead"?** 
_There is no perceptible overhead when Linq Expressions or ILGenerator are used. Basic advice introduce a light overhead caused by boxing and arguments array creation. However, MethodInfo is not prepared if capture is not required in lambda expression._

- **Why I have to use CNeptune?** 
_Interception is based on CNeptune. Indeed CNeptune add a build action to rewrite CIL to make assembly "Architect Friendly" by injecting transparents and hidden features to to grant full execution control at runtime.

- **Can I add multiple aspect for same target? If yes how can I control priority?** 
_Yes you can. Priority is defined by the order of weaving. It can be reorganized by calling Aspect.Release(...)/Aspect.Weave(...) and you can check the whole aspects mapping by calling Aspect.Lookup(...)._

- **Is an attribute required to identify a mehod to weave?** 
_No you can identify a method by the way you want. There is a Aspect.Weave(...) overload that take a Func<MethodInfo, bool> to select methods._

- **Can I intercept constructor? If yes, how do I implement it?**
_Constructor interception is supported and is treated like another method with declaring type as first argument and void for return type._
