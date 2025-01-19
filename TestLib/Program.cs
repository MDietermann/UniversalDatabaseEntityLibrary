using UniversalDatabaseEntity;
using UniversalDatabaseEntity.DbHandling;

namespace TestLib;

internal class Program {
    private static void Main(string[] args) {
        UdeFactory factory = new UdeFactory(
            server: "localhost",
            userId: "dev",
            password: "1234",
            databaseName: "dev_userlib",
            port: "3306",
            databaseType: DbTypeEnum.MySql
        );
        

        TestGetEntity(factory: factory);
        TestGetEntitiesNoParams(factory: factory);
        TestGetEntitiesWithParams(factory: factory);
    }
    
    private static void TestGetEntity(UdeFactory factory) {
        Console.WriteLine("Single Entity:");
        List<DynamicProperty<object>> parameters = [
            new DynamicProperty<object>(key: "id", value: 1)
        ];
        SearchParameters searchParameters = new SearchParameters(tableName: "employee", parameters: parameters);
        UniversalEntity? universalEntity = factory.GetEntity(searchParameters: searchParameters);
        Console.WriteLine(value: universalEntity == null ? "No entity found." : universalEntity.ToString());
        Console.WriteLine("------\n");
    }
    
    private static void TestGetEntitiesNoParams(UdeFactory factory) {
        Console.WriteLine("All Entities (no params):");
        SearchParameters searchParameters = new SearchParameters(tableName: "department", parameters: null);
        List<UniversalEntity>? universalEntities = factory.GetEntities(searchParameters: searchParameters);
        if ( universalEntities != null )
            foreach ( UniversalEntity entity in universalEntities )
                Console.WriteLine(value: entity.ToString());
        else
            Console.WriteLine(value: "No entities found.");
        Console.WriteLine("------\n");
    }

    private static void TestGetEntitiesWithParams(UdeFactory factory) {
        Console.WriteLine("All Entities (salary_id = 2):");
        List<DynamicProperty<object>> parameters = [
            new DynamicProperty<object>(key: "salary_id", value: 2)
        ];
        SearchParameters searchParameters = new SearchParameters(tableName: "employee", parameters: parameters);
        List<UniversalEntity>? universalEntities = factory.GetEntities(searchParameters: searchParameters);
        if ( universalEntities != null )
            foreach ( UniversalEntity entity in universalEntities )
                Console.WriteLine(value: entity.ToString());
        else
            Console.WriteLine(value: "No entities found.");
        Console.WriteLine("------\n");
    }
}
