//using System.Data.Entity.Infrastructure;
//using System.Data.Entity.Migrations;

//namespace IoT.Management.Operations.Sql.Migrations
//{
//    public static class Migration
//    {
//        private static bool _isLatest = false;

//        public static void ToLatest(string connectionName)
//        {
//            if (_isLatest)
//                return;

//            var configuration = new Configuration();
//            configuration.TargetDatabase = new DbConnectionInfo(connectionName);
//            var migrator = new DbMigrator(configuration);

//            migrator.Update();

//            _isLatest = true;
//        }
//    }
//}
