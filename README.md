# NConcern .NET AOP Framework
NConcern is a .NET runtime AOP (Aspect-Oriented Programming) lightweight framework that reduces tangling caused by cross-cutting concerns. Its role is to introduce Aspect-Oriented Programming paradigm with a minimum cost to maximize quality and productivity.


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
