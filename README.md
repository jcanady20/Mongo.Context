# Mongo.Context

Mongo Context provides a familiar experience for developers that have worked with Entity Framework.


## Project Overview
- **Mongo.Context** is a C# library that provides an Entity Framework-like experience for MongoDB, enabling familiar patterns for .NET developers.
- The architecture centers around `MongoContext`, which acts as the main entry point for database operations, similar to `DbContext` in EF.
- Collections are accessed via generic `MongoSet<TEntity>` properties, supporting LINQ queries and expression-based access.
- Class mapping and index configuration are handled via `MongoClassMap` and `MongoBuilder`.

## Key Components
- `MongoContext`: Main context class. Handles connection, database selection, and registration of entity mappings and indexes.
- `MongoSet<TEntity>`: Wrapper for MongoDB collections, supports LINQ and direct queries.
- `MongoBuilder`/`IMongoBuilder`: Used for registering class maps and initializing sets/indexes.
- `MongoClassMap`: Used to configure collection names, property mappings, and indexes for entities.
- Example context: See `Mongo.Context.Example/Context.cs` for custom context and mapping patterns.

## Developer Workflows
- **Build**: Standard .NET build (`dotnet build Mongo.Context.sln`).
- **Test**: No explicit test project found; add tests in `Mongo.Context.AppHost/_Tests.cs` or similar.
- **Debug**: Use standard .NET debugging tools. Entry point for examples is likely in `Mongo.Context.Example/Context.cs`.

## Patterns & Conventions
- **Entity Registration**: Override `OnRegisterClasses` in your context to register class maps and indexes. Use `mongoBuilder.FromAssembly` to auto-register maps from an assembly.
- **Manual Mapping**: Use `mongoBuilder.Entry<TEntity>()` to manually configure collection names, property mappings, and indexes.
- **Indexing**: Add indexes via `MongoClassMap.AddIndex` or set the `Indexes` property.
- **Pluralization**: Collection names are pluralized using internal helpers (see `Internal/NamePluralization.cs`).
- **Extensions**: Utility methods for collections are in `Extensions/IEnumerableExtensions.cs`.

## Integration Points
- **MongoDB Driver**: Uses `MongoDB.Driver` and related packages for database operations.
- **Mapping**: Custom class maps (e.g., `CustomerMap`) should inherit from `MongoClassMap<TEntity>` and configure ID serialization/generation as needed.

## Example Usage
```csharp
public class Context : MongoContext {
    public MongoSet<Customer> Customers { get; set; }
    protected override void OnRegisterClasses(MongoBuilder mongoBuilder) {
        mongoBuilder.FromAssembly(typeof(Context).Assembly);
        mongoBuilder.Entry<Customer>().SetCollectionName("SuperCustomers");
        mongoBuilder.Entry<Customer>().AddIndex(new MongoIndex { Keys = new[] { "Name" } });
    }
}
```

## Important Files & Directories
- `Mongo.Context/`: Core library source
- `Mongo.Context.Example/`: Example context, entities, and maps
- `Mongo.Context/Extensions/`: Utility extensions
- `Mongo.Context/Internal/`: Internal helpers for set finding, pluralization, etc.
- `Mongo.Context/Mapping/`: Class mapping and index configuration

---

**Feedback Requested:**
- Are there any custom build, test, or deployment steps not covered?
- Are there project-specific patterns or conventions missing from these instructions?
- Is there any undocumented cross-component communication or integration?

Please provide feedback or clarifications to improve these instructions for future AI agents.