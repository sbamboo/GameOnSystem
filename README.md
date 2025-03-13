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

Default local database file (pre-compiled in SecretConfig.cs):
```
Filename: gameon_v2.db
```

Default server address (pre-compiled in SecretConfig.cs):
```
Database: gameon_v2
Adress:   localhost
User:     root
Password: ""
```

DB Feature flags (Fields under options table): *al options are string in db, type is written here as \<dbtype\>:\<parsedtype\>*

Type        | Values         | Field                           | Fallback (InCode) |Description
------------|----------------|---------------------------------|-------------------|--------------------------------------------------------------------
string:bool | "true"/"false" | ff_grade_shows_username         | true              | Show name who placed grades in userview
string:bool | "true"/"false" | ff_hover_for_playbutton         | false             | If true the playbutton is only visible when hovering the gamebanner
string:bool | "true"/"false" | ff_grade_comment_after_deadline | false             | If true users can still comment on grades after the deadline

