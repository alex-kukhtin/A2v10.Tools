# A2v10.Sql.MSBuild

[![NuGet](https://img.shields.io/nuget/v/A2v10.Sql.MSBuild.svg)](https://www.nuget.org/packages/A2v10.Sql.MSBuild)

MSBuild task for A2v10 applications: on every build, merges the SQL files listed
in `sql.json` into ready-to-deploy scripts.

## Installation

```xml
<PackageReference Include="A2v10.Sql.MSBuild" Version="10.1.8221" PrivateAssets="all" />
```

The package is needed only at build time. `PrivateAssets="all"` keeps it out of
the dependencies of your own package.

## How it works

After `Build`, the task looks for `sql.json` in the project folder.
No `sql.json` — nothing happens.

Each entry in `sql.json` produces one output file:

```json
[
	{
		"outputFile": "_sqlscripts/main.sql",
		"inputFiles": [
			"@sql/a2v10_platform.sql",
			"_sql/_schemas.sql",
			"_sql/_tables.sql",
			"/**/logic.sql"
		]
	},
	{
		"outputFile": "_sqlscripts/admin.sql",
		"inputFiles": [
			"admin/**/*.sql"
		]
	}
]
```

All paths are relative to the project folder.

## Input files

| Pattern                 | Matches                                                      |
|-------------------------|--------------------------------------------------------------|
| `_sql/_tables.sql`      | exactly this file                                            |
| `catalog/**/logic.sql`  | every `logic.sql` in `catalog` and all its subfolders        |
| `/**/logic.sql`         | every `logic.sql` in the whole project                       |
| `admin/**/*.sql`        | every `.sql` file under `admin` (`*` and `?` in the name)    |

Files are written in the order they are listed in `inputFiles`.

> **The order of files matched by `**` is not defined.**
> Write your SQL so that it does not depend on it.

A missing explicit file fails the build.
A missing folder in a `**` pattern simply matches nothing.

## Output

- Rewritten on every build; missing folders are created.
- UTF-8 with BOM.
- LF line endings — CRLF in the input files is converted to LF.
- Starts with a header comment, each input file is followed by a line break:

```
/* _sqlscripts/main.sql */

<contents of _sql/_schemas.sql>
<contents of catalog/agent/logic.sql>
...
```

The build log shows what was merged:

```
Build sql started..
Processing C:\Projects\MyApp\sql.json
Writing _sqlscripts/main.sql
	_sql/_schemas.sql
	catalog/agent/logic.sql
	document/invoice/logic.sql
Build sql completed
```

## License

MIT
