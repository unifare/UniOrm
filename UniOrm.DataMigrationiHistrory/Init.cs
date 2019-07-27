using FluentMigrator;
using System;

namespace UniOrm.DataMigrationiHistrory
{
    [Migration(10000000000000)]
    public class Init : Migration
    {
        public override void Up()
        {

            Create.Table("SystemACon")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("AppName").AsString(200)
                .WithColumn("AppDiscription").AsString(300)
                .WithColumn("AppConfigs").AsString().Nullable()
                .WithColumn("VersionNum").AsString(100).Nullable()
                .WithColumn("UpdateUrl").AsString(600).Nullable()
                .WithColumn("UpdateParamter").AsString(600).Nullable()
                .WithColumn("Author").AsString(100).Nullable()
                .WithColumn("LastRunTime").AsDateTime().Nullable()
                 .WithColumn("CreateTime").AsDateTime().Nullable()
                ;
            Create.Table("SystemRegitionInfo")
              .WithColumn("Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("AuthorizeWay").AsString(200)
              .WithColumn("VersionNum").AsString(100).Nullable()
              .WithColumn("MachineCode").AsString(100).Nullable()
              .WithColumn("RigesterCode").AsString(2000).Nullable()
              .WithColumn("RigesterUrl").AsString(1000).Nullable()
              .WithColumn("RigesterFilePath").AsString(1000).Nullable()
              .WithColumn("ClientName").AsString(1000).Nullable()
              .WithColumn("TryNumber").AsInt32().Nullable().WithDefaultValue(0)
              .WithColumn("TryEndTime").AsDateTime().Nullable()
              .WithColumn("LastRunTime").AsDateTime().Nullable()
               .WithColumn("CreateTime").AsDateTime().Nullable()
              ;

            Create.Table("FieldInfoDefinition")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("BelongTypeId").AsInt64().Nullable()
                .WithColumn("Name").AsString(200)
                .WithColumn("IsPublic").AsBoolean().WithDefaultValue(true)
                .WithColumn("IsPrivate").AsBoolean().WithDefaultValue(false)
                .WithColumn("IsSpecialName").AsBoolean().WithDefaultValue(false)
                .WithColumn("FieldType").AsString(400).Nullable()
             .WithColumn("MemberType").AsInt16().Nullable()
             .WithColumn("VersionNum").AsString(100).Nullable()
             .WithColumn("AddTime").AsDateTime().Nullable();
            ;

            Create.Table("ProperityDefinition")
               .WithColumn("Id").AsInt64().PrimaryKey().Identity()
               .WithColumn("BelongTypeId").AsInt64().Nullable()
               .WithColumn("Name").AsString(400)
               .WithColumn("IsPublic").AsBoolean().WithDefaultValue(true)
               .WithColumn("IsPrivate").AsBoolean().WithDefaultValue(false)
               .WithColumn("IsSpecialName").AsBoolean().WithDefaultValue(false)
               .WithColumn("CanWrite").AsBoolean().WithDefaultValue(true)
               .WithColumn("CanRead").AsBoolean().WithDefaultValue(true)
               .WithColumn("ProperityType").AsString(400).Nullable()
               .WithColumn("MemberType").AsInt16().Nullable()
               .WithColumn("VersionNum").AsString(100).Nullable()
               .WithColumn("AddTime").AsDateTime().Nullable();
            ;

            Create.Table("MethodDefinition")
               .WithColumn("Id").AsInt64().PrimaryKey().Identity()
               .WithColumn("BelongTypeId").AsInt64().Nullable()
               .WithColumn("Name").AsString(400)
                     .WithColumn("FullName").AsString(400)
                     .WithColumn("IsStatic").AsBoolean().Nullable().WithDefaultValue(true)
                   .WithColumn("IsConstructor").AsBoolean().WithDefaultValue(false).Nullable()
               .WithColumn("IsPublic").AsBoolean().WithDefaultValue(true)
               .WithColumn("IsPrivate").AsBoolean().WithDefaultValue(false).Nullable()
               .WithColumn("ReturnType").AsString(400).WithDefaultValue(string.Empty).Nullable()
               .WithColumn("ParameterInfo").AsString(400).Nullable()
               .WithColumn("MemberType").AsInt16().Nullable()
               .WithColumn("VersionNum").AsString(100).Nullable()
               .WithColumn("AddTime").AsDateTime().Nullable();
            ;
            Create.Table("TypeDefinition")
               .WithColumn("Id").AsInt64().PrimaryKey().Identity()
               .WithColumn("AliName").AsString(400)
               .WithColumn("ClassName").AsString(400)
               .WithColumn("GenericParameterAttributes").AsInt16().Nullable()
               .WithColumn("IsAbstract").AsBoolean().WithDefaultValue(true)
               .WithColumn("IsVisible").AsBoolean().WithDefaultValue(true)
               .WithColumn("IsNotPublic").AsBoolean().WithDefaultValue(true)
               .WithColumn("Attributes").AsInt16().Nullable()
               .WithColumn("IsNestedFamORAssem").AsBoolean().WithDefaultValue(true)
               .WithColumn("IsAutoLayout").AsBoolean().WithDefaultValue(true)
               .WithColumn("GUID").AsString(80).WithDefaultValue(Guid.NewGuid().ToString("N"))
               .WithColumn("IsValueType").AsBoolean().WithDefaultValue(false)
               .WithColumn("IsInterface").AsBoolean().WithDefaultValue(false)
               .WithColumn("IsGenericType").AsBoolean().WithDefaultValue(true)
               .WithColumn("IsArray").AsBoolean().WithDefaultValue(false)
               .WithColumn("IsSpecialName").AsBoolean().WithDefaultValue(false)
               .WithColumn("IsEnum").AsBoolean().WithDefaultValue(false)
               .WithColumn("IsClass").AsBoolean().WithDefaultValue(false)
               .WithColumn("IsSealed").AsBoolean().WithDefaultValue(false)
               .WithColumn("IsPublic").AsBoolean().WithDefaultValue(true)
               .WithColumn("IsPrivate").AsBoolean().WithDefaultValue(false)
               .WithColumn("Namespace").AsString(400).WithDefaultValue(false).Nullable()
               .WithColumn("AssemblyQualifiedName").AsString(400).Nullable()
               .WithColumn("FullName").AsString(700).Nullable()
                .WithColumn("VersionNum").AsString(100).Nullable()
               .WithColumn("AddTime").AsDateTime().Nullable();
            ;
            Create.Table("ClassACon")
               .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("VersionNum").AsString(100).Nullable()
                 .WithColumn("Name").AsString(700).Nullable()
                .WithColumn("Description").AsString(700).Nullable()
                .WithColumn("IsVirtual").AsBoolean().Nullable().WithDefaultValue(false)
                .WithColumn("Guid").AsString(80).WithDefaultValue(Guid.NewGuid().ToString("N"))
               .WithColumn("TypeDefinitionId").AsInt64().Nullable()
               .WithColumn("AddTime").AsDateTime().Nullable();
            ;

            Create.Table("AssemblyACon")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("NameSpace").AsString(600).Nullable()
                 .WithColumn("Name").AsString(700).Nullable()
                 .WithColumn("Description").AsString(700).Nullable()
                .WithColumn("DllPath").AsString(1200).Nullable()
                  .WithColumn("VersionNum").AsString(100).Nullable()
                .WithColumn("Guid").AsString(80).WithDefaultValue(Guid.NewGuid().ToString("N"))
                .WithColumn("IsVirtual").AsBoolean().Nullable().WithDefaultValue(false)
                .WithColumn("AddTime").AsDateTime().Nullable();
            ;


            Create.Table("AConFlowStep")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                 .WithColumn("Guid").AsString(70).Nullable()
                 .WithColumn("AComposityId").AsString(80)
                  .WithColumn("IsUsingParentConnstring").AsBoolean().Nullable()
                  .WithColumn("IsBuildIn").AsBoolean().Nullable()
                 .WithColumn("IsUsingCache").AsBoolean().WithDefaultValue(true) 
                .WithColumn("DBType").AsInt16().Nullable()
                .WithColumn("Connectionstring").AsString(700).Nullable()
                 .WithColumn("StepOrder").AsInt32()
                .WithColumn("Name").AsString(700).Nullable()
                .WithColumn("Description").AsString(700).Nullable()
                .WithColumn("VersionNum").AsString(100).Nullable()
               .WithColumn("FlowStepType").AsInt32().Nullable()
                .WithColumn("ProxyCode").AsString(1000).Nullable()
                 .WithColumn("OutPutText").AsString().Nullable()
                .WithColumn("ReferenceDlls").AsString(300).Nullable()
                .WithColumn("StoreValueProposal").AsInt32().Nullable()
            .WithColumn("ExcuteType").AsInt32() 
               .WithColumn("GetValueProposal").AsInt32().Nullable() 
                .WithColumn("TypeLib").AsString(100).Nullable()
                 .WithColumn("TypeLibform").AsString(100).Nullable()
                  .WithColumn("TypeFullName").AsString(400).Nullable()
                   .WithColumn("DllDirName").AsString(100).Nullable()
                     .WithColumn("MethodName").AsString(100).Nullable()
                   .WithColumn("StorePoolKey").AsString(100).Nullable()
                    .WithColumn("ArgNames").AsString(100).Nullable()
                    .WithColumn("InParamter1").AsString(100).Nullable()
                   .WithColumn("InParamter2").AsString(100).Nullable()
                    .WithColumn("InParamter3").AsString(100).Nullable()
                   .WithColumn("InParamter4").AsString(100).Nullable()
                    .WithColumn("InParamter5").AsString(100).Nullable()
                   .WithColumn("InParamter6").AsString(100).Nullable()
                    .WithColumn("InParamter7").AsString(100).Nullable()
                   .WithColumn("InParamter8").AsString(100).Nullable()
                .WithColumn("InParamter9").AsString(100).Nullable()
               .WithColumn("InParamter10").AsString(100).Nullable()
                .WithColumn("InstanceName").AsString(100).Nullable()
                 .WithColumn("NextRuntimeID").AsString(100).Nullable()
               .WithColumn("AddTime").AsDateTime().Nullable();
            ;

            
        Create.Table("ComposeTemplate")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Name").AsString(700).Nullable()
            .WithColumn("StepIds").AsString(700).Nullable()
            .WithColumn("Isenable").AsBoolean( ).Nullable() 
            .WithColumn("AddTime").AsDateTime().Nullable();
            ;
           

        Create.Table("AConStateModule")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Name").AsString(700).Nullable()
            .WithColumn("Description").AsString(700).Nullable()
            .WithColumn("VersionNum").AsString(100).Nullable()
            .WithColumn("AddTime").AsDateTime().Nullable();
            ;


            Create.Table("ComposeEntity")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                 .WithColumn("Guid").AsString(70).Nullable()
                .WithColumn("Name").AsString(700).Nullable() 
                .WithColumn("RunMode").AsInt32()
                  .WithColumn("IsBuildIn").AsBoolean().Nullable()
                .WithColumn("IsUsingParentConnstring").AsBoolean().Nullable()
                .WithColumn("AppType").AsString(70).Nullable()
                .WithColumn("Connectionstring").AsString(700).Nullable()
               .WithColumn("TrigeMethod").AsString(100).Nullable()
                .WithColumn("TrigeType").AsString(100).Nullable()
                .WithColumn("Description").AsString(700).Nullable() 
                    .WithColumn("Templateid").AsString(700).Nullable()
                .WithColumn("RegisterGuid").AsString(80).WithDefaultValue(Guid.NewGuid().ToString("N"))
                .WithColumn("CalStackMD5").AsString(100).Nullable()
                .WithColumn("IsStatic").AsBoolean().Nullable().WithDefaultValue(true)
                .WithColumn("TransferParamterLib").AsString(300).Nullable()
                .WithColumn("TransferParamterPlantform").AsString(300).Nullable()
                .WithColumn("TransferParamterType").AsString(300).Nullable()
                .WithColumn("TransferParamterMethod").AsString(300).Nullable()
                .WithColumn("TransferParamterInArguments").AsString(300).Nullable()
                .WithColumn("RequsetEnviromentId").AsInt64().Nullable()
                .WithColumn("VersionNum").AsString(100).Nullable()
                .WithColumn("AddTime").AsDateTime().Nullable();
            ;

            Create.Table("StepParamter")
               .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString(700).Nullable()
                 .WithColumn("ModuleId").AsInt64().Nullable()
                .WithColumn("FlowID").AsInt64().Nullable()
                .WithColumn("VersionNum").AsString(100).Nullable()
                 .WithColumn("GetValueWayID").AsInt64().Nullable()
               .WithColumn("AddTime").AsDateTime().Nullable();
            ;


            Create.Table("ResouceInfo")
             .WithColumn("Id").AsInt64().PrimaryKey().Identity()
              .WithColumn("FlowId").AsInt64().Nullable()
              .WithColumn("ModuleId").AsInt64().Nullable()
             .WithColumn("ComposeEntityId").AsInt64().Nullable()
             .WithColumn("KeyName").AsString(100)
            .WithColumn("Value").AsString(8000)
          .WithColumn("SerialObject").AsString(8000)
               .WithColumn("VersionNum").AsString(100).Nullable()
             .WithColumn("AddTime").AsDateTime().Nullable();
            ;

           

            Create.Table("TrigerRuleInfo")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("VersionNum").AsString(100).Nullable()
                .WithColumn("RuleName").AsString(100).Nullable()
                 .WithColumn("HttpMethod").AsString(20).Nullable()
                .WithColumn("ComposityId").AsString(70).Nullable()
                .WithColumn("Rule").AsString(100).Nullable()
                .WithColumn("IsEnable").AsBoolean().Nullable().WithDefaultValue(true)
                .WithColumn("AddTime").AsDateTime().Nullable();
            ;
         
            var s = new
            {
                Guid = "D506C9B6-AC0C-421A-BD6F-BD94A92DA418",
                AddTime = DateTime.Now,
                RunMode = 0,
                IsBuildIn=true,
                IsUsingParentConnstring = true,
                Connectionstring = "sys_default",
                AppType = "aspnetcore",
                Description = "defaultweb",
                TrigeType = "urlreg",
                Name = "defaultweb",

            };

            Insert.IntoTable("ComposeEntity").Row(s);
            var ste = new
            {
                Guid = "3E3F7118-8282-4476-A903-7400336B9A9F",
                AddTime = DateTime.Now,
                IsBuildIn = true,
                FlowStepType = 0,
                ExcuteType=0,
                GetValueProposal = 5,
                IsUsingParentConnstring = true,
                AComposityId = s.Guid,
                Description = "defaultweb_Setp1",
                StepOrder = 0,
                InParamter1 = "select * from TrigerRuleInfo",
                StorePoolKey = "allrules",
                //StoreValueType = typeof(List<ComposeEntity>).FullName
            };
            Insert.IntoTable("AConFlowStep").Row(ste);
         
            var ste2 = new
            {
                Guid = "38E13CBA-4456-43B7-B3B3-A7611F738C16",
                AddTime = DateTime.Now,
                IsBuildIn = true,
                FlowStepType = 4,
                ExcuteType = 0,
                GetValueProposal = 4,
                IsUsingParentConnstring = true,
                AComposityId = s.Guid,
                IsUsingCache=false,
                Description = "defaultweb_Setp2",
                StepOrder = 1,
                TypeLib = "BasicPlugin.dll",
                MethodName = "GetRequestUrl",
                TypeFullName = "BasicPlugin.HttpUtility",
                StorePoolKey = "url",
                ArgNames = "__httpcontext",
            };
            Insert.IntoTable("AConFlowStep").Row(ste2);
            var ste33 = new
            {
                Guid = "1d9F",
                AddTime = DateTime.Now,
                IsBuildIn = true,
                FlowStepType = 2,
                ExcuteType = 0,
                GetValueProposal = 5,
                IsUsingParentConnstring = true,
                AComposityId = s.Guid,
                StepOrder =2,
                ProxyCode = "BasicPlugin.QueryGeneratorcs.test( \"3 \",\"4\")",
                TypeLib = "BasicPlugin.dll",
                StorePoolKey = "",
                //StoreValueType = typeof(List<ComposeEntity>).FullName
            };
            Insert.IntoTable("AConFlowStep").Row(ste33);
            var ste3 = new
            {
                Guid = "BBF6B4BC-AEDA-4607-83ED-406E8BB67351",
                AddTime = DateTime.Now,
                IsBuildIn = true,
                FlowStepType = 4,
                ExcuteType = 0,
                GetValueProposal = 4, 
                IsUsingCache = false,
                IsUsingParentConnstring = true,
                AComposityId = s.Guid,
                Description = "defaultweb_Setp2",
                StepOrder = 3,
                TypeLib = "BasicPlugin.dll",
                MethodName = "GetIsTriger",
                TypeFullName = "BasicPlugin.HttpUtility",
                ArgNames = "allrules,url",
                StorePoolKey = "_NextRunTimeKey",
            };
            Insert.IntoTable("AConFlowStep").Row(ste3);


            Insert.IntoTable("TrigerRuleInfo").Row(new { RuleName = "default", Rule = @"\/test", ComposityId = "1111" });
            Insert.IntoTable("TrigerRuleInfo").Row(new { RuleName = "default", Rule = @"\/_fact", ComposityId = "_fact" });
            var s_fact = new
            {
                Guid = "_fact",
                AddTime = DateTime.Now,
                IsBuildIn=false,
                IsUsingParentConnstring = true,
                Connectionstring = "sys_default",
                RunMode = 0,
                AppType = "aspnetcore",
                Description = "defaultweb_fact",
                TrigeType = "urlreg", 
                Name = "defaultweb_fact",

            };
            Insert.IntoTable("ComposeEntity").Row(s_fact);
            var ste_fact = new
            {
                Guid = "_factS1",
                AddTime = DateTime.Now,
                IsBuildIn = true,
                FlowStepType = 2,
                ExcuteType = 0,
                GetValueProposal = 5,
                IsUsingParentConnstring = true,
                AComposityId = s_fact.Guid,
                StepOrder = 2,
                ProxyCode = "\"hello\"",
                TypeLib = "BasicPlugin.dll",
                StorePoolKey = "restext",
                //StoreValueType = typeof(List<ComposeEntity>).FullName
            };
            Insert.IntoTable("AConFlowStep").Row(ste_fact);
            var ste_fact1 = new
            {
                Guid = "_factS2",
                AddTime = DateTime.Now,
                IsBuildIn = true,
                FlowStepType = 4,
                ExcuteType = 0,
                GetValueProposal = 4,
                IsUsingParentConnstring = true,

                AComposityId = s_fact.Guid,
                Description = "defaultweb_facSetp2",
                StepOrder = 3,
                TypeLib = "BasicPlugin.dll",
                MethodName = "ResponseText",
                TypeFullName = "BasicPlugin.HttpUtility",
                ArgNames = "__httpcontext,restext",
                StorePoolKey = "",
            };
            Insert.IntoTable("AConFlowStep").Row(ste_fact1);
            //var aGateWay_fact = new
            //{
            //    Guid = "9CBCEE00-2AB2-40D3-8406-4A1B923432cc",
            //    AddTime = DateTime.Now,
            //    AComposityId = s_fact.Guid,
            //    AConFlowStepId = ste_fact.Guid,
            //    InParamter1 = "select * from TrigerRuleInfo",
            //    StorePoolKeys = "url",
            //    PoolKey1 = "__httpcontext",
            //    //StorePoolsKeys = "allrules",
            //    //StoreValueType = typeof(ComposeEntity).FullName
            //};
            //Insert.IntoTable("AConGetWay").Row(aGateWay);

        }

        public override void Down()
        {
            Delete.Table("SystemACon");
            Delete.Table("SystemRegitionInfo");
            Delete.Table("FieldInfoDefinition");
            Delete.Table("ProperityDefinition");
            Delete.Table("MethodDefinition");
            Delete.Table("TypeDefinition");
            Delete.Table("ClassACon");
            Delete.Table("AssemblyACon");
            Delete.Table("AConStateModule");
            Delete.Table("AuthorizeInfo");
            Delete.Table("AConStateFlow");
            Delete.Table("AConFlowStep");
            Delete.Table("GetWay");
            Delete.Table("StepParamter");
            Delete.Table("TrigerRuleInfo");
            Delete.Table("ComposeEntity");
            Delete.Table("SystemRegistionInfo");
        }
    }
}
