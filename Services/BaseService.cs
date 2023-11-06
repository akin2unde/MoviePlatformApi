using MongoDB.Driver;
using System.Linq.Expressions;
using MongoDB.Bson;
using MoviePlatformApi.Models;
using MoviePlatformApi.Configuration;
using System.Text;
using File = MoviePlatformApi.Models.File;

namespace MoviePlatformApi.Services
{
    public class BaseService<T> where T : BaseModel
    {
        private char Separator { get { return Path.DirectorySeparatorChar; } }
        const int limit = 20;
        const int skip = 0;
        public string Token { get; set; }
        private readonly IMongoCollection<T> _entities;
        public IMongoCollection<T> DBEntity { get { return _entities; } }

        public Setting Setting { get; set; }

        public MongoClient Client { get; set; }

        public IMongoDatabase Database { get; set; }
        public HttpContext Context { get; set; }

        public User ActiveUser { get; set; }

        public bool IsAdmin { get; set; } = false;
        public readonly ILogger logger;

        public BaseService(Setting setting, IHttpContextAccessor Context, User activeUser = null, ILogger logger = null)
        {
            Setting = setting;
            Client = new MongoClient(Setting.ConnectionString);
            Database = Client.GetDatabase(Setting.DatabaseName);
            _entities = Database.GetCollection<T>(typeof(T).Name);
            CreateIndexes();
            this.Context = Context != null ? Context.HttpContext : null;
            this.ActiveUser = activeUser;
            this.logger = logger;
        }
        private void CreateIndexes()
        {
            _entities.Indexes.CreateOne(new CreateIndexModel<T>(Builders<T>.IndexKeys.Text("$**")));
            var properties = typeof(T).GetProperties().Where(prop => prop.IsDefined(typeof(UniqueAttribute), false)).ToList();
            properties.OrderBy(s => (s.GetCustomAttributes(typeof(UniqueAttribute), false) as UniqueAttribute[])[0].Position);
            var index = new BsonDocument();
            foreach (var item in properties)
            {
                index.Add(new BsonElement(item.Name, 1));
            }
            if (index.ElementCount > 0)
            {
                var indexModel = new CreateIndexModel<T>(index, new CreateIndexOptions { Unique = true });
                _entities.Indexes.CreateOne(indexModel);
            }

        }
        //count with linq
        public long Count(Expression<Func<T, bool>> predicate, int skip, int limit)
        {
            var res = (from a in DBEntity.AsQueryable().Where(predicate) select a).Count();
            return res;
        }
        public long Count(Expression<Func<T, bool>> predicate)
        {
            var res = _entities.CountDocuments(predicate);
            return res;
        }
        public long Count(FilterDefinition<T> predicate)
        {
            var res = _entities.CountDocuments(predicate);
            return res;
        }
        protected long Count()
        {
            var res = _entities.CountDocuments(c => true);
            return res;
        }
        public PagedResult<T> Get(int skip = skip, int limit = limit)
        {
            var result = _entities.Find(entity => true).Skip(skip).Limit(limit).SortByDescending(s => s.CreatedAt).ToList();
            result.ForEach(f =>
            {
                f.State = ObjectState.Unchanged;
            });
            var pr = new PagedResult<T>()
            {
                LastItem = result.Count > 0 ? result.Last().Id.ToString() : "",
                Result = result,
                Skip = skip + result.Count,
                PageSize = limit,
                Total = Count()
            };
            return pr;
        }
        public PagedResult<T> Get(Expression<Func<T, bool>> predicate, int skip = skip, int limit = limit)
        {
            var finalFilter = predicate;
            var result = _entities.Find(finalFilter, new FindOptions { Collation = new Collation("en", strength: CollationStrength.Primary) }).Skip(skip).Limit(limit).SortByDescending(s => s.CreatedAt).ToList();
            result.ForEach(f =>
            {
                f.State = ObjectState.Unchanged;
            });
            var pr = new PagedResult<T>()
            {
                LastItem = result.Count > 0 ? result.Last().Id.ToString() : "",
                Result = result,
                Skip = skip + result.Count,
                PageSize = limit,
                Total = finalFilter != null ? Count(finalFilter) : Count()
            };
            return pr;
        }
        public T Get(string id)
        {
            return _entities.Find<T>(entity => entity.Id == id).FirstOrDefault();
        }

        public T Create(T entity)
        {
            _entities.InsertOne(entity);
            return entity;
        }
        public List<T> Create(List<T> entity)
        {
            entity.ForEach(entity =>
            {
                entity.CreatedAt = DateTime.Now;
            });
            _entities.InsertMany(entity);
            return entity;
        }
        public List<T> Save(List<T> entity)
        {
            entity.ForEach(entity =>
            {
                entity.UpdatedAt = DateTime.Now;
            });
            var newObjs = entity.Where(ent => ent.State == ObjectState.New).ToList();
            var updateObjs = entity.Where(ent => ent.State == ObjectState.Changed).ToList();
            var delObjs = entity.Where(ent => ent.State == ObjectState.Removed).ToList();
            var auditTrails = new List<AuditTrail>();
            if (newObjs.ToList().Count > 0)
            {
                var res = Create(newObjs);
                if (ActiveUser != null && typeof(T) != typeof(AuditTrail))
                {
                    auditTrails.AddRange(res.Select(s => new AuditTrail
                    {
                        MailAddress = ActiveUser.MailAddress,
                        Entity = typeof(T).Name,
                        EntityId = s.Id,
                        CreatedAt = DateTime.Now,
                        Name = $"New {typeof(T).Name} created by {ActiveUser.MailAddress}"
                    }).ToList());
                }
            }
            if (updateObjs.ToList().Count > 0)
            {
                Update(updateObjs);
                if (ActiveUser != null && typeof(T) != typeof(AuditTrail))
                {
                    auditTrails.AddRange(updateObjs.Select(s => new AuditTrail
                    {
                        MailAddress = ActiveUser.MailAddress,
                        Entity = typeof(T).Name,
                        EntityId = s.Id,
                        CreatedAt = DateTime.Now,
                        Name = $"{typeof(T).Name} updated by {ActiveUser.MailAddress}"
                    }).ToList());
                }
            }
            if (delObjs.ToList().Count > 0)
            {
                Remove(delObjs.Select(s => s.Id).ToList());
                if (ActiveUser != null && typeof(T) != typeof(AuditTrail))
                {
                    auditTrails.AddRange(delObjs.Select(s => new AuditTrail
                    {
                        MailAddress = ActiveUser.MailAddress,
                        Entity = typeof(T).Name,
                        EntityId = s.Id,
                        CreatedAt = DateTime.Now,
                        Name = $"{typeof(T).Name} deleted by {ActiveUser.MailAddress}"
                    }).ToList());
                }
            }
            if (auditTrails.Count > 0)
            {
                var bs = new BaseService<AuditTrail>(Setting, null, ActiveUser) { Context = Context };
                bs.Save(auditTrails);
            }
            entity.ForEach(entity =>
            {
                entity.State = ObjectState.Unchanged;
            });
            return entity;
        }
        public void Update(T entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _entities.ReplaceOne(book => book.Id == entity.Id, entity);
        }

        public void Update(List<T> entities)
        {
            entities.ForEach(entity =>
            {
                _entities.ReplaceOne(e => e.Id == entity.Id, entity);
            });
        }
        public PagedResult<T> Search(string param, int skip = skip, int limit = limit)
        {
            if (limit == 0)
            {
                limit = 20;
            }
            var finalFilter = Builders<T>.Filter.Text($"\"{param}\"");
            var result = _entities.Aggregate().Match(finalFilter).Skip(skip).Limit(limit).ToList();
            if (result == null)
            {
                result = new List<T>();
            }
            result.ForEach(f =>
            {
                f.State = ObjectState.Unchanged;
            });
            var pr = new PagedResult<T>()
            {
                LastItem = result != null && result.Count > 0 ? result.Last().Id.ToString() : "",
                Result = result,
                Skip = skip + result.Count,
                PageSize = limit,
                Total = finalFilter != null ? Count(finalFilter) : Count()
            };
            return pr;
        }
        public void Remove(string id)
        {
            var split = id.Split(",");
            var filter = Builders<T>.Filter.In(u => u.Id, split);
            _entities.DeleteMany(filter);
            var auditTrails = new List<AuditTrail>();
            auditTrails.AddRange(split.Select(s => new AuditTrail
            {
                MailAddress = ActiveUser.MailAddress,
                Entity = typeof(T).Name,
                EntityId = s,
                CreatedAt = DateTime.Now,
                Name = $"{typeof(T).Name} deleted by {ActiveUser.MailAddress}"
            }).ToList());
            var bs = new BaseService<AuditTrail>(Setting, null, ActiveUser) { Context = Context };
            bs.Save(auditTrails);
        }
        public void RemoveAll() =>
            _entities.DeleteManyAsync(Builders<T>.Filter.Empty);

        public void Remove(T entity) =>
            _entities.DeleteOne(book => book.Id == entity.Id);

        public void Remove(List<string> ids)
        {
            var idsFilter = Builders<T>.Filter.In(d => d.Id, ids);
            _entities.DeleteMany(idsFilter);

        }
        private string GetPath(char separator, string[] comps)
        {
            StringBuilder path = new StringBuilder();
            foreach (var item in comps)
            {
                path.Append(GetValidName(item) + separator);
            }
            return path.ToString();
        }
        public string BaseAppPath
        {
            get
            {
                return $"{Directory.GetCurrentDirectory()}{Separator}wwwroot{Separator}";
            }
        }
        internal Models.File SaveFile(File file, string root, string[] path)
        {
            string location = $"{BaseAppPath}media{Separator}{GetPath(Separator, path)}";
            System.IO.Directory.CreateDirectory(location);
            var name = GetValidName(file.Name);
            location += name;
            int lastPeriod = file.Name.LastIndexOf('.');
            string ext = file.Name.Substring(lastPeriod + 1);
            int indexOfSemiColon = file.Data.IndexOf(";", StringComparison.OrdinalIgnoreCase);

            string dataLabel = file.Data.Substring(0, indexOfSemiColon);

            string contentType = dataLabel.Split(':').Last();

            var startIndex = file.Data.IndexOf("base64,", StringComparison.OrdinalIgnoreCase) + 7;

            var base64Str = file.Data.Substring(startIndex);
            System.IO.File.WriteAllBytes(location, Convert.FromBase64String(base64Str));
            file.Url = $"{root}/media/{GetPath('/', path)}{GetValidName(name)}";
            file.Extension = ext;
            file.Data = "";
            file.State = ObjectState.Unchanged;
            return file;
        }
        string GetValidName(string name)
        {
            return name.Replace(" ", "_").Replace("/", "").Replace(":", "");
        }

    }
}