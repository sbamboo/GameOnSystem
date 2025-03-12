Project setup commands:
```
dotnet add package Microsoft.EntityFrameworkCore -v 8.0.2
dotnet add package Microsoft.EntityFrameworkCore.Sqlite -v 8.0.2
dotnet add package Microsoft.EntityFrameworkCore.Tools -v 8.0.2

dotnet add package Pomelo.EntityFrameworkCore.MySql --version 8.0.2
```

Default login:
```
Username: admin
Password: admin
```

Default server address (pre-compiled in SecretConfig.cs):
```
Adress:   localhost
User:     root
Password: ""
```

DB Feature flags (Fields under options table):

Type   | Values         | Field                   | Description
-------|----------------|-------------------------|--------------------------------------------------------------------
string | "true"/"false" | ff_grade_shows_username | Show name who placed grades in userview
string | "true"/"false" | ff_hover_for_playbutton | If true the playbutton is only visible when hovering the gamebanner

