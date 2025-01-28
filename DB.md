Make a system which should have the following methods:
  GetGeneral() -> Name, Theme
  SetGeneral(key, value)

  GetGroup(Name) -> Members # if not exists, returns empty
  SetGroup(Name, Members)   # if not exist, creates
  
  GetGame(Name) -> GroupID, URL, Grades, Description, BannerURL # if not exists, returns empty
  SetGame(Name, GroupID, URL, Grades, Description, BannerURL)   # if not exist, creates. Al options except Name are optional

  GetUser(Name) -> IsAdmin, Category, Email # if not exists, returns empty
  SetUser(Name, IsAdmin, Category, Email)   # if not exist, creates

Using this we would store it onto a .db file in some stucture like:
'''
[General] # fields under the root object
  Name
  Theme

[Group]
  Name
  Members

[Game]
  Name
  GroupID
  URL
  Grades{
    <category>: <num>,
    "Result": <num>
  }
  Description(Optional)
  BannerURL(Optional)

[User]
  Name
  IsAdmin
  Category
  Email
'''

I want to be able to use this class as <Instance>.<Method>(<...Input...>)
and if i use a method that requires no writing it opens the file and reads before closing,
else it opens the file, reads, writes and closes the file.

The db should use `Microsoft.EntityFrameworkCore`, `Microsoft.EntityFrameworkCore.Sqlite`, `Microsoft.EntityFrameworkCore.Tools`

Then create a model

the class should be named `AppDbContext.cs`

Example: (NOT MY PROJECT/STRUCTURE)
'''
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
}
'''

'''
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public DbSet<Person> People { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=people.db");
    }
}
'''