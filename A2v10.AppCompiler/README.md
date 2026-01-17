# About
A2v10.AppCompiler is a code generator for A2v10 projects.


## Use CLR

Required packages:
```xml
<ItemGroup>
	<PackageReference Include="A2v10.Module.Infrastructure" Version="10.1.8620" />
	<PackageReference Include="A2v10.App.Infrastructure" Version="10.1.8620" />
</ItemGroup>
```

Use application CLR:

```csharp
services.UseApplicationClr(opts =>
{
	MainApp.StartupClr.Register(opts);
});
```