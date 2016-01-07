# EntityFramework.Metadata #
[![Nuget](https://img.shields.io/nuget/dt/EntityFramework.Metadata.svg)](https://www.nuget.org/packages/EntityFramework.Metadata/)

Get table metadata for your Entity Framework entities.  Need to know the table name of your entity programatically?  Schema name?  Column name from a property?  EntityFramework.Metadata has that and more.

    static void Main()
    {
        var context = new MyDbContext();
        var personData = context.Db<Person>();

        Console.WriteLine(personData.TableName);
		// output: People

		var nameColumn = personData.Prop("Name");

		Console.WriteLine(nameColumn.ColumnName);
		// output: MyName
    }

	[Table("People")]
	public class Person
	{
		public int PersonId { get; set; }
		[Column("MyName")]
		public string Name { get; set; }
	}

Forked from [EntityFramework.MappingAPI](https://efmappingapi.codeplex.com/) by Markko Legonkov.  Fixed bugs, added some unit tests, simplified the code and project structure, and removed support for EF5 and below.

### License ###
This project is licensed under the MIT license.  Portions of the code are subject to the BSD Simplified license - see LICENSE for more info.
