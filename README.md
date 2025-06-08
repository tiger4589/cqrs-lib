# Let's write our own (minimal) CQRS Library!

## Why are we doing this?

I don't think that it's still shocking news by now that [AutoMapper and MediatR are going commercial](https://www.jimmybogard.com/automapper-and-mediatr-going-commercial/). 

A lot of developers prefers to do their own mappings instead of using Auto Mapper, 
so we won't be focusing on this.
While MediatR library isn't simply equal to CQRS, it does however facilitate the implementation of CQRS pattern. So let's spend our energy here!

## First tings first, what is CQRS?

CQRS, known as Command Query Responsibility Segregation (hard, I know), is a pattern that separates the concerns of handling data (commands) from reading the data (queries) in a system.
This separation allows for independent optimization of read and write paths, potentially improving scalability, performance, and maintainability.

### Very simple diagram:

![CQRS-Single-DB](https://github.com/user-attachments/assets/e9c5a753-0bb2-4db3-887c-4dae064bd30b)

The above diagram shows the simple approach for CQRS and how it resides in your API. If you need to create, update or delete your data, you'll be using your command model inside the API, however, if you're retrieving data from your application, you're most likely going to be using the query model inside the API.
This diagram demonstrates how you could start by using CQRS, however, to actually separate the concerns, the following diagram illustrates the full approach of using it, with two databases, one for writing and one for reading with eventual consistency between the write and read databases:

![CQRS-Two-DB](https://github.com/user-attachments/assets/f056ff24-55bd-4b4a-b018-6d2e65952a1a)

This was a quick introduction and explanation for CQRS, maybe we could dive more in depth in a later blog post, but our goal for today is to write our own library, and again, a minimal one, that we could use in our API to implement CQRS. Let's start then!

## Our Library!

### Commands

First we need to be able to define a class as a command, so let's go ahead and write our `ICommand` interface:

```csharp
public interface ICommand;
```

Second, we'll need to define a class as a command handler, that our system would know which command it will treat, so let's go ahead and write an `ICommandHandler` interface:

```csharp
public interface ICommandHandler<in TCommand> where TCommand : ICommand
{
    Task ExecuteAsync(TCommand command);
}
```

### Queries

We're going to do the same thing for the queries, with a small change. We also need to create for our queries an interface that will define the result class of our calls. So, let's go ahead and implement three interfaces: `IQuery`, `IQueryResult`, and `IQueryHandler`.

```csharp
public interface IQuery<TResult> where TResult : IQueryResult;
```

```csharp
public interface IQueryResult;
```

```csharp
public interface IQueryHandler<in TQuery, TQueryResult> where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
{
    Task<TQueryResult> RetrieveAsync(TQuery  query);
}
```

### Dispatcher

Now that we have defined our interfaces that can be used to create commands and queries, we need a way to be able to call their handlers without having to explicitly injecting them in each of our services.

We're going to achieve this by implementing a dispatcher class that basically have two overloaded methods, one to call a command, and one to call a query and retrieve the data. So let's go ahead and do that:

```csharp
public interface IDispatcher
{
    Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
    Task<TQueryResult> DispatchAsync<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult;
}
```

```csharp
internal sealed class Dispatcher(IServiceProvider serviceProvider) : IDispatcher
{
    public Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler = GetHandler<TCommand>();
        return handler.ExecuteAsync(command);
    }

    public Task<TQueryResult> DispatchAsync<TQuery, TQueryResult>(TQuery query) where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
    {
        var handler = GetHandler<TQuery, TQueryResult>();
        return handler.RetrieveAsync(query);
    }

    private ICommandHandler<TCommand> GetHandler<TCommand>()
        where TCommand : ICommand
    {
        var handler = serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
        return handler;
    }

    private IQueryHandler<TQuery, TQueryResult> GetHandler<TQuery, TQueryResult>()
        where TQuery : IQuery<TQueryResult> where TQueryResult : IQueryResult
    {
        var handler = serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();
        return handler;
    }
}
```

As you can see, the dispatcher main function will be to retrieve from our injected services the specified handlers, and either execute a command, or execute a query and retrieve the data.

### Dependency Injection

Finally, since we talked about injected services, the remaining question would be: _How am I going to inject all those handlers? Should I do it for each one alone?_ The asnwer is no. The library will take care of this with the help of [Scrutor library](https://github.com/khellang/Scrutor).

Let's go ahead and see how:

```csharp
public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCQRSLibrary(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddTransient<IDispatcher, Dispatcher>();

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        services.Scan(scan => scan
            .FromAssemblies(assemblies)
            .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime());

        return services;
    }
}
```

First, we'll inject the `IDispatcher` that will be used in our API Controllers. 
Then we ask `Scrutor` to scan the passed assemblies and find all classes that implements our handlers and add them to the IoC Container with the Transient Lifetime.

## How to use all of this?

Now that we've finalized our minimal library, let's see it in action!
We're going to take an example of a `UserController` where we're going to simulate Adding, Deleting, and getting a user.

### Application layer, where our logic resides.

First of all, let's implement our different commands and queries.

First command will be used to add a new user:

```csharp
public sealed record AddUserCommand : ICommand;

public class AddUserCommandHandler : ICommandHandler<AddUserCommand>
{
    public Task ExecuteAsync(AddUserCommand command)
    {
        Console.WriteLine("Added User");
        return Task.CompletedTask;
    }
}
```

First query will be used to retrieve a user:

```csharp
public sealed record GetUserQuery(Guid Id) : IQuery<GetUserQueryResult>;

public class GetUserQueryResult : IQueryResult;

public class GetUserQueryHandler : IQueryHandler<GetUserQuery, GetUserQueryResult>
{
    public Task<GetUserQueryResult> RetrieveAsync(GetUserQuery query)
    {
        Console.WriteLine("Here's your User");
        return Task.FromResult(new GetUserQueryResult());
    }
}
```

We'll stick to that for now, and we won't implement the actual creation and retrieval of data, since this is out of the scope of our post, but here's a look of how you could separate your code into features using the CQRS:

![image](https://github.com/user-attachments/assets/73f2929c-0b75-4431-84a8-d6ca04c42558)

### API layer, final straw.

Now that our library is ready, and our application layer is ready, let's see how to setup our API to inject all those handlers, and how to use the library in our controller.

First things first, let's inject everything using the library extension:

```csharp
builder.Services.AddCQRSLibrary(Assembly.GetAssembly(typeof(GetUserQuery))!);
```

Since all of our code resides in one simple project, we can simply call our extension method with one assembly, using the `Assembly.GetAssembly` method.

Finally, our `UserController` can now inject the `IDispatcher` and start calling different commands and queries:

```
[Route("api/[controller]")]
[ApiController]
public class UserController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(Guid id)
    {
        GetUserQuery userQuery = new(id);
        var user = await dispatcher.DispatchAsync<GetUserQuery, GetUserQueryResult>(userQuery);
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AddUserCommand command)
    {
        await dispatcher.DispatchAsync(command);
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserCommand command)
    {
        await dispatcher.DispatchAsync(command);
        return Ok();
    }
}
```

## Final Thoughts

This minimal library doesn't cover everything that the `MediatR` library offers, that's for sure. However, it's a small library that you could use as a starting point to build your own bigger library.

One of the parts that we didn't tackle at all, is Event Sourcing. CQRS is often combined with event sourcing, where commands sends events that can be used to update the read model asynchronously. 

All the code above could be found [here](https://github.com/tiger4589/cqrs-lib).
