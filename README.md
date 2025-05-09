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

Default shown mode buttons (pre-compiled in SecretConfig.cs):
```
Local: true
External: true
```

DB Feature flags (Fields under options table): *al options are string in db, type is written here as \<dbtype\>:\<parsedtype\>*

Type        | Values         | Field                           | Fallback (InCode) |Description
------------|----------------|---------------------------------|-------------------|--------------------------------------------------------------------
string:bool | "true"/"false" | ff_grade_shows_username         | true              | Show name who placed grades in userview
string:bool | "true"/"false" | ff_hover_for_playbutton         | false             | If true the playbutton is only visible when hovering the gamebanner
string:bool | "true"/"false" | ff_grade_comment_after_deadline | false             | If true users can still comment on grades after the deadline


DB Options (Fields under options table):

`group_grade_calculation` (`opt_group_grade_calculation`) decides how the average grade for a group is calculated, between the following modes:
  - `average`                       : Takes the average of all grades and gives with decimals *(double)*
  - `avgs-of-category-avgs`         : Takes the average of all grades in each category then averages the averages and gives with decimals *(double)*
  - `average-rounded`               : Same as `average` but rounds result to nearest integer *(int, Math.round)*
  - `avgs-of-category-avgs-rounded` : 


Additional notes:
- The app supports only one active edition at a time, if multiple the one defined first in the database will be shown.