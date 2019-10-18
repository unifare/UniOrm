using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace UniOrm.Adaption
{
    public class EFDbContext : DbContext
    {
        private readonly static object lockobj = new object();
        private readonly IMemoryCache _cache;
        public ModelBuilder ModelBuilder;
        private static string DynamicCacheKey = "DynamicModel";
        protected DbContextOptions<EFDbContext> Options;
        public string DefaultDbPrefixName { get; set; }
        public EFDbContext(DbContextOptions<EFDbContext> options, IMemoryCache cache, List<Type> typesTobeRegisted = null) : base(options)
        {
            Options = options;
            TypesUnRegisted = typesTobeRegisted;
            _cache = cache;
        }
        protected override void OnModelCreating(ModelBuilder _modelBuilder)
        {

            if (ModelBuilder == null)
            {
                foreach (var s in TypesUnRegisted)
                {
                    _modelBuilder.Model.AddEntityType(s);
                }
                ModelBuilder = _modelBuilder;
                foreach (var entity in ModelBuilder.Model.GetEntityTypes())
                {
                    var currentTableName = ModelBuilder.Entity(entity.Name).Metadata.Relational().TableName;
                    ModelBuilder.Entity(entity.Name).ToTable( DefaultDbPrefixName+ currentTableName );

                    //var properties = entity.GetProperties();
                    //foreach (var property in properties)
                    //    builder.Entity(entity.Name).Property(property.Name).HasColumnName(property.Name.ToLower());
                }
            }

            //AddTypeByObject(entity);
            //AddToREgistedType(modelBuilder);
            base.OnModelCreating(ModelBuilder);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 
            optionsBuilder.UseMemoryCache(_cache);
            // optionsBuilder.con
            //optionsBuilder.Options.ContextType.

            //if (m_modelBuilder == null)
            //{
            //    return;
            //}
            //IMutableModel model = _cache.GetOrCreate(DynamicCacheKey, entry =>
            //{
            //    //var modelBuilder = new ModelBuilder(_builder.CreateConventionSet());

            //    m_modelBuilder.Model.AddEntityType("").SqlServer().TableName = "";

            //    _cache.Set(DynamicCacheKey, m_modelBuilder.Model);
            //    return m_modelBuilder.Model;
            //});

            //optionsBuilder.UseModel(model);
            base.OnConfiguring(optionsBuilder);

        }

        public List<Type> TypesRegisted = new List<Type>();
        public List<Type> TypesUnRegisted = new List<Type>();
        public override DbSet<TEntity> Set<TEntity>()
        {
            var t = typeof(TEntity);

            //var unregistedOldtype = TypesUnRegisted.FirstOrDefault(p => p.FullName == t.Name);
            //var registedoldtype = TypesRegisted.FirstOrDefault(p => p.FullName == t.Name);
            //if (registedoldtype == null && unregistedOldtype == null)
            //{
            //    TypesUnRegisted.Add(typeof(TEntity));
            //}
            //AddToRegistedType(ModelBuilder, t);
            return base.Set<TEntity>();
        }

        public override EntityEntry Add(object entity)
        {
            //AddTypeByObject(entity);

            return base.Add(entity);
        }
        public override EntityEntry Update(object entity)
        {
            //AddTypeByObject(entity);
            return base.Update(entity);
        }

        private void AddTypeByObject(object entity)
        {
            if (entity != null)
            {
                var typeobj = entity.GetType();
                AddToRegistedType(ModelBuilder, typeobj);
            }
        }

        public override EntityEntry Remove(object entity)
        {
            ///AddTypeByObject(entity);
            return base.Remove(entity);
        }



        private void AddToRegistedType(ModelBuilder modelBuilder, Type type)
        {
            lock (lockobj)
            {

                if (modelBuilder == null)
                {
                    return;
                }

                modelBuilder.Model.GetOrAddEntityType(type);
            }
        }
    }
}
