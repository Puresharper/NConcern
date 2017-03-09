# NConcern .NET AOP Framework
NConcern is a .NET runtime AOP (Aspect-Oriented Programming) lightweight framework written in C# that reduces tangling caused by cross-cutting concerns. Its role is to introduce Aspect-Oriented Programming paradigm with a minimum cost to maximize quality and productivity.


## Features
NConcern AOP Framework is based on code injection at runtime.

- non-intrusive : no need to change the source code or the publishing process.
- friendly : delegates, expressions (linq) or CIL (ILGenerator) can be used to define an aspect
- no configuration : additional configuration files are not required
- no proxy : decoration by inheritance and factory pattern are not required
- low learning curve : get started under 20 minutes
- no installer : single .net library (.dll) to reference
- suited for unit testing : weaving is controlled at runtime
- low performance overhead : injection mechanic is built to be efficient
- limitless : everything except generic methods is supported (coming with next release)
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

- Disable compilation and JIT inlining by placing Debuggable attribute in AssemblyInfo.cs
```
[assembly: System.Diagnostics.Debuggable(true, true)]
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
_For real there is no overhead when Linq Expressions or ILGenerator are used. Basic advice introduce a light overhead caused by boxing and arguments array creation. However, MethodInfo is not prepared if capture is not required in lambda expression._

- **Why I have to use DebuggableAttribute?** 
_Interception is based on method swaping and cannot be applied when JIT or compiler optimize a call by inlining. The DebuggableAttribute is an acceptable way to disable inlining without being to much intrusive. You are free to do it by another way (MethodImplAttribute for example) but keep in mind that only non virtual methods can be inlined._

- **Can I add multiple aspect for same target? If yes how can I control priority?** 
_Yes you can. Priority is defined by the order of weaving. It can be reorganized by calling Aspect.Release(...)/Aspect.Weave(...) and you can check the whole aspects mapping by calling Aspect.Lookup(...)._

- **Is an attribute required to identify a mehod to weave?** 
_No you can identify a method by the way you want. There is a Aspect.Weave(...) overload that take a Func<MethodInfo, bool> to select methods._

- **Can I intercept constructor? If yes, how do I implement it?**
_Constructor interception is supported and is treated like another method with declaring type as first argument and void for return type._

## More
- https://www.nuget.org/packages/NConcern/
- https://aspectize.codeplex.com
