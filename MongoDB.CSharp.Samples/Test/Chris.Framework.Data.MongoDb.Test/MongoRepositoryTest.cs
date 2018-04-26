using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Chris.Framework.Data.MongoDb.Test
{
    [TestClass]
    public class MongoRepositoryTest : MongoRepository<Person>
    {

        public MongoRepositoryTest() : base("mongodb://127.0.0.1:27017/UnitTest")
        {

        }

        #region Query UnitTest
        [TestMethod]
        public void QueryAllUnitTest()
        {
            InitData();

            var data = Query().ToList();

            Console.WriteLine($"Data count:{data.Count}");

            DeleteAll();
        }
        [TestMethod]
        public void QueryIncludeFilterUnitTest()
        {
            InitData();

            var data = Query(i => i.Age > 20).ToList();

            Console.WriteLine($"Data count:{data.Count}");

            DeleteAll();
        }
        #endregion

        #region Utils UnitTest
        [TestMethod]
        public void AnyUnitTest()
        {
            InitData();

            var result = Any(i => i.Gender == false);

            Console.WriteLine($"Result:{result}");

            DeleteAll();
        }

        [TestMethod]
        public void CountUnitTest()
        {
            InitData();

            var result = Count();

            Console.WriteLine($"Data count is :{result}");

            DeleteAll();
        }

        [TestMethod]
        public async Task CountAsyncUnitTest()
        {
            InitData();

            var result = await CountAsync();

            Console.WriteLine($"Data count is :{result}");

            DeleteAll();
        }

        [TestMethod]
        public void CountIncludeFilterUnitTest()
        {
            InitData();

            var result = Count(i => i.Age > 20);

            Console.WriteLine($"Data count is :{result}");

            DeleteAll();
        }

        [TestMethod]
        public async Task CountIncludeFilterAsyncUnitTest()
        {
            InitData();

            var result = await CountAsync(i => i.Age > 20);

            Console.WriteLine($"Data count is :{result}");

            DeleteAll();
        }
        #endregion

        #region Get UnitTest

        [TestMethod]
        public void GetUnitTest()
        {
            var entity = new Person
            {
                Name = "Chris.Sun",
                Gender = true,
                Age = 26
            };

            Insert(entity);

            var result = Get(entity.Id).ToJson();

            Console.WriteLine(result);

            Delete(entity);
        }

        [TestMethod]
        public async Task GetAsyncUnitTest()
        {
            var entity = new Person
            {
                Name = "Chris.Sun",
                Gender = true,
                Age = 26
            };

            Insert(entity);

            var result = await GetAsync(entity.Id);

            Console.WriteLine(result.ToJson());

            Delete(entity);
        }
        #endregion

        #region Insert UnitTest

        [TestMethod]
        public void InsertUnitTest()
        {
            var entity = new Person
            {
                Name = "Chris.Sun",
                Gender = true,
                Age = 26
            };

            Insert(entity);

            entity.CreatedOn = DateTime.Now;

            var data = Get(entity.Id);

            Console.WriteLine(data.ToJson());

            Console.WriteLine(data.CreatedOn.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss"));

            Delete(entity);
        }

        [TestMethod]
        public async Task InsertAsyncUnitTest()
        {
            var entity = new Person
            {
                Name = "Chris.Sun",
                Gender = true,
                Age = 26
            };

            await InsertAsync(entity);

            entity.CreatedOn = DateTime.Now;

            var data = await GetAsync(entity.Id);

            Console.WriteLine(data.ToJson());

            Console.WriteLine(data.CreatedOn.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss"));

            await DeleteAsync(entity);
        }

        [TestMethod]
        public void InsertManyUnitTest()
        {
            var names = new[] { "Apple", "Beck", "Chris", "Durent", "English", "Jame", "Park", "Rose", "Nacy", "Wall" };

            var dataList = new List<Person>();

            for (var i = 0; i < 10; i++)
            {
                var entity = new Person
                {

                    Name = names[new Random().Next(0, 5)],
                    Gender = i % 2 == 0,
                    Age = i + 15
                };

                dataList.Add(entity);
            }

            Insert(dataList);

            Console.WriteLine($"Data count:{Count()}");

            DeleteAll();
        }

        [TestMethod]
        public async Task InsertManyAsyncUnitTest()
        {
            var names = new[] { "Apple", "Beck", "Chris", "Durent", "English", "Jame", "Park", "Rose", "Nacy", "Wall" };

            var dataList = new List<Person>();

            for (var i = 0; i < 10; i++)
            {
                var entity = new Person
                {

                    Name = names[new Random().Next(0, 5)],
                    Gender = i % 2 == 0,
                    Age = i + 15
                };

                dataList.Add(entity);
            }

            await InsertAsync(dataList);

            Console.WriteLine($"Data count:{Count()}");

            await DeleteAllAsync();
        }

        #endregion

        #region Delete UnitTest

        [TestMethod]
        public void DeleteUnitTest()
        {
            var entity = new Person
            {
                Name = "Chris.Sun",
                Gender = true,
                Age = 26
            };

            Insert(entity);

            var insertResult = Get(entity.Id);

            Console.WriteLine($"InsertResult：{insertResult?.ToJson()}");

            var deleteResult = Delete(entity);

            Console.WriteLine($"DeleteResult：{deleteResult}");

        }

        [TestMethod]
        public async Task DeleteAsyncUnitTest()
        {
            var entity = new Person
            {
                Name = "Chris.Sun",
                Gender = true,
                Age = 26
            };

            await InsertAsync(entity);

            var insertResult = await GetAsync(entity.Id);

            Console.WriteLine($"InsertResult：{insertResult?.ToJson()}");

            var deleteResult = await DeleteAsync(entity);

            Console.WriteLine($"DeleteResult：{deleteResult}");

        }

        [TestMethod]
        public void DeleteByIdUnitTest()
        {
            var entity = new Person
            {
                Name = "Chris.Sun",
                Gender = true,
                Age = 26
            };

            Insert(entity);

            var insertResult = Get(entity.Id);

            Console.WriteLine($"InsertResult：{insertResult?.ToJson()}");

            Delete(entity.Id);

            var deleteResult = Get(entity.Id);

            Console.WriteLine($"DeleteResult：{deleteResult?.ToJson()}");

        }

        [TestMethod]
        public async Task DeleteByIdAsyncUnitTest()
        {
            var entity = new Person
            {
                Name = "Chris.Sun",
                Gender = true,
                Age = 26
            };

            await InsertAsync(entity);

            var insertResult = await GetAsync(entity.Id);

            Console.WriteLine($"InsertResult：{insertResult?.ToJson()}");

            var deleteResult = await DeleteAsync(entity.Id);

            Console.WriteLine($"DeleteResult：{deleteResult}");

        }

        [TestMethod]
        public void DeleteIncludeFilterUnitTest()
        {
            InitData();

            Console.WriteLine($"Data count: {Count()}");

            var result = Delete(i => i.Age > 20);

            if (result)
            {
                Console.WriteLine("Delete is success.");

                Console.WriteLine($"Data count: {Count()}");
            }
            else
            {
                Console.WriteLine("Delete is fail.");

                Console.WriteLine($"Data count: {Count()}");
            }

            DeleteAll();
        }

        [TestMethod]
        public async Task DeleteIncludeFilterAsyncUnitTest()
        {
            InitData();

            Console.WriteLine($"Data count: {Count()}");

            var result = await DeleteAsync(i => i.Age > 20);

            if (result)
            {
                Console.WriteLine("Delete is success.");

                Console.WriteLine($"Data count: {Count()}");
            }
            else
            {
                Console.WriteLine("Delete is fail.");

                Console.WriteLine($"Data count: {Count()}");
            }

            await DeleteAllAsync();
        }

        [TestMethod]
        public void DeleteAllUnitTest()
        {
            InitData();

            Console.WriteLine($"Data count: {Count()}");

            var result = DeleteAll();

            if (result)
            {
                Console.WriteLine("Delete is success.");

                Console.WriteLine($"Now data count: {Count()}");
            }
            else
            {
                Console.WriteLine("Delete is fail.");

                Console.WriteLine($"Now data count: {Count()}");
            }

        }

        [TestMethod]
        public async Task DeleteAllAsyncUnitTest()
        {
            InitData();

            Console.WriteLine($"Data count: {Count()}");

            var result = await DeleteAllAsync();

            if (result)
            {
                Console.WriteLine("Delete is success.");

                Console.WriteLine($"Now data count: {Count()}");
            }
            else
            {
                Console.WriteLine("Delete is fail.");

                Console.WriteLine($"Now data count: {Count()}");
            }

        }
        #endregion

        #region Last UnitTest

        [TestMethod]
        public void LastUnitTest()
        {
            InitData();

            var result = Last();

            Console.WriteLine($"Last data:{result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void LastIncludeFilterUnitTest()
        {
            InitData();

            var result = Last(i => i.Gender);

            Console.WriteLine($"Last data:{result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void LastIncludeFilterAndOrderUnitTest()
        {
            InitData();

            var result = Last(i => i.Gender, i => i.Age);

            Console.WriteLine($"Last data:{result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void LastIncludeFilterAndOrderDescUnitTest()
        {
            InitData();

            var result = Last(i => i.Gender, i => i.Age, true);

            Console.WriteLine($"Last data:{result.ToJson()}");

            DeleteAll();
        }
        #endregion

        #region First UnitTest

        [TestMethod]
        public void FirstUnitTest()
        {
            InitData();

            var result = First();

            Console.WriteLine($"first data:{result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void FirstIncludeFilterUnitTest()
        {
            InitData();

            var result = First(i => !i.Gender);

            Console.WriteLine($"first data:{result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void FirstIncludeFilterAndOrderUnitTest()
        {
            InitData();

            var result = First(i => !i.Gender, o => o.Age);

            Console.WriteLine($"first data:{result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void FirstIncludeFilterAndOrderDescUnitTest()
        {
            InitData();

            var result = First(i => !i.Gender, o => o.Age, true);

            Console.WriteLine($"first data:{result.ToJson()}");

            DeleteAll();
        }

        #endregion

        #region Find & FindAll UnitTest

        [TestMethod]
        public void FindIncludeFilterUnitTest()
        {
            InitData();

            var result = Find(i => i.Gender).Count();

            Console.WriteLine($"When gender is true , the data count is {result}");

            DeleteAll();
        }

        [TestMethod]
        public void FindIncludeFilterPagedUnitTest()
        {
            InitData();

            var result = Find(i => i.Gender, 2, 2);

            Console.WriteLine($"When gender is true ,  json data is {result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void FindIncludeFilterAndOrderPagedUnitTest()
        {
            InitData();

            var result = Find(i => i.Gender, i => i.Age, 1, 2);

            Console.WriteLine($"When gender is true ,  json data is {result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void FindIncludeFilterAndOrderAscPagedUnitTest()
        {
            InitData();

            var result = Find(i => i.Gender, i => i.Age, 1, 2, false);

            Console.WriteLine($"When gender is true ,  json data is {result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void FindAllUnitTest()
        {
            InitData();

            var result = FindAll().Count();

            Console.WriteLine($"Total data count is {result}");

            DeleteAll();
        }

        [TestMethod]
        public void FindAllPagedUnitTest()
        {
            InitData();

            var result = FindAll(2, 2);

            Console.WriteLine($" Json data is {result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void FindAllIncludeOrderPagedUnitTest()
        {
            InitData();

            var result = FindAll(i => i.Age, 2, 2);

            Console.WriteLine($" Json data is {result.ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public void FindAllIncludeOrderAscPagedUnitTest()
        {
            InitData();

            var result = FindAll(i => i.Age, 2, 2, false);

            Console.WriteLine($" Json data is {result.ToJson()}");

            DeleteAll();
        }
        #endregion

        #region  Replace UnitTest

        [TestMethod]
        public void ReplaceUnitTest()
        {
            var data = new Person
            {
                Name = "Chris",
                Age = 26,
                Gender = true
            };

            Insert(data);

            Console.WriteLine($"The old Data is {Get(data.Id).ToJson()}");

            data.Age = 18;
            data.Gender = false;
            data.ModifiedOn = DateTime.Now;
            Thread.Sleep(100);

            var result = Replace(data);

            Console.WriteLine(result ? $"The new Data is {Get(data.Id).ToJson()}" : "replace fail .");

            DeleteAll();
        }

        [TestMethod]
        public async Task ReplaceAsyncUnitTest()
        {
            var data = new Person
            {
                Name = "Chris",
                Age = 26,
                Gender = true
            };

            await InsertAsync(data);

            Console.WriteLine($"The old Data is {Get(data.Id).ToJson()}");

            data.Age = 18;
            data.Gender = false;
            data.ModifiedOn = DateTime.Now;

            var result = await ReplaceAsync(data);

            Console.WriteLine(result ? $"The new Data is {Get(data.Id).ToJson()}" : "replace fail .");

            await DeleteAllAsync();
        }

        [TestMethod]
        public void ReplaceManyUnitTest()
        {
            var names = new[] { "Apple", "Beck", "Chris", "Durent", "English", "Jame", "Park", "Rose", "Nacy", "Wall" };

            var dataList = new List<Person>();

            for (var i = 0; i < 10; i++)
            {
                var entity = new Person
                {

                    Name = names[new Random().Next(0, 5)],
                    Gender = i % 2 == 0,
                    Age = i + 15
                };

                dataList.Add(entity);
            }

            Insert(dataList);

            Console.WriteLine($"The old  Data is {FindAll().ToList().ToJson()}");

            foreach (var person in dataList)
            {
                person.Age += 10;
                person.Gender = !person.Gender;
                person.ModifiedOn = DateTime.Now;
            }

            Replace(dataList);

            Console.WriteLine($"The new Data is {FindAll().ToList().ToJson()}");

            DeleteAll();
        }


        #endregion

        #region Update UnitTest
        [TestMethod]
        public void UpdateByIdUnitTest()
        {
            var data = new Person
            {
                Name = "Chris",
                Age = 26,
                Gender = true
            };

            Insert(data);

            var update = Updater.Set(i => i.Name, "Chris.Sun")
                 .Set(i => i.Age, 18);

            Update(data.Id, update);

            Console.WriteLine($"now data is {Get(data.Id).ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public async Task UpdateByIdAsyncUnitTest()
        {
            var data = new Person
            {
                Name = "Chris",
                Age = 26,
                Gender = true
            };

            await InsertAsync(data);

            var update = Updater.Set(i => i.Name, "Chris.Sun")
                .Set(i => i.Age, 18);

            await UpdateAsync(data.Id, update);

            Console.WriteLine($"now data is {Get(data.Id).ToJson()}");

            await DeleteAllAsync();
        }

        [TestMethod]
        public void UpdateByEntityUnitTest()
        {
            var data = new Person
            {
                Name = "Chris",
                Age = 26,
                Gender = true
            };

            Insert(data);

            var update = Updater.Set(i => i.Name, "Chris.Sun")
                .Set(i => i.Age, 18);

            Update(data, update);

            Console.WriteLine($"now data is {Get(data.Id).ToJson()}");

            DeleteAll();
        }
        [TestMethod]
        public async Task UpdateByEntityAsyncUnitTest()
        {
            var data = new Person
            {
                Name = "Chris",
                Age = 26,
                Gender = true
            };

            await InsertAsync(data);

            var update = Updater.Set(i => i.Name, "Chris.Sun")
                .Set(i => i.Age, 18);

            await UpdateAsync(data, update);

            Console.WriteLine($"now data is {Get(data.Id).ToJson()}");

            await DeleteAllAsync();
        }
        [TestMethod]
        public void UpdateIncludeUpdateDefinitionByFilterUnitTest()
        {
            InitData();

            var filterList = new List<FilterDefinition<Person>>
            {
                Filter.Eq(i => i.Gender, true),
                Filter.Gte(i => i.Age, 20)
            };

            var filters = Filter.And(filterList);


            var update = Updater.Set(i => i.Name, "Chris.Sun");

            Update(filters, update);

            Console.WriteLine($"now data is {Find(i => i.Age >= 20 && i.Gender).ToList().ToJson()}");

            DeleteAll();
        }
        [TestMethod]
        public async Task UpdateIncludeUpdateDefinitionByFilterAsyncUnitTest()
        {
            InitData();

            var filterList = new List<FilterDefinition<Person>>
            {
                Filter.Eq(i => i.Gender, true),
                Filter.Gte(i => i.Age, 20)
            };

            var filters = Filter.And(filterList);


            var update = Updater.Set(i => i.Name, "Chris.Sun");

            var result = await UpdateAsync(filters, update);

            Console.WriteLine($"result :{result}, now data is {Find(i => i.Age >= 20 && i.Gender).ToList().ToJson()}");

            await DeleteAllAsync();
        }
        [TestMethod]
        public void UpdateIncludeUpdateDefinitionByLambdaFilterUnitTest()
        {
            InitData();

            var update = Updater.Set(i => i.Name, "Chris.Sun");

            Update(i => i.Age >= 20 && i.Gender, update);

            Console.WriteLine($"now data is {Find(i => i.Age >= 20 && i.Gender).ToList().ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public async Task UpdateIncludeUpdateDefinitionByLambdaFilterAsyncUnitTest()
        {
            InitData();

            var update = Updater.Set(i => i.Name, "Chris.Sun");

            await UpdateAsync(i => i.Age >= 20 && i.Gender, update);

            Console.WriteLine($"now data is {Find(i => i.Age >= 20 && i.Gender).ToList().ToJson()}");

            await DeleteAllAsync();
        }
        [TestMethod]
        public void UpdateIncludeEntityByLambdaDataUnitTest()
        {
            var data = new Person
            {
                Name = "Chris",
                Age = 26,
                Gender = true
            };

            Insert(data);

            Update(data, i => i.Age, 18);

            Console.WriteLine($"now data is {Get(data.Id).ToJson()}");

            DeleteAll();
        }
        [TestMethod]
        public async Task UpdateIncludeEntityByLambdaDataAsyncUnitTest()
        {
            var data = new Person
            {
                Name = "Chris",
                Age = 26,
                Gender = true
            };

            await InsertAsync(data);

            await UpdateAsync(data, i => i.Age, 18);

            Console.WriteLine($"now data is {Get(data.Id).ToJson()}");

            await DeleteAllAsync();
        }

        [TestMethod]
        public void UpdateIncludeLambdaDataByFilterUnitTest()
        {
            InitData();

            var filterList = new List<FilterDefinition<Person>>
            {
                Filter.Eq(i => i.Gender, true),
                Filter.Gte(i => i.Age, 20)
            };
            var filters = Filter.And(filterList);

            Update(filters, i => i.Name, "Chris.Sun");

            Console.WriteLine($"update data are:{Find(i => i.Gender && i.Age >= 20).ToList().ToJson()}");

            DeleteAll();
        }

        [TestMethod]
        public async Task UpdateIncludeLambdaDataByFilterAsyncUnitTest()
        {
            InitData();

            var filterList = new List<FilterDefinition<Person>>
            {
                Filter.Eq(i => i.Gender, true),
                Filter.Gte(i => i.Age, 20)
            };
            var filters = Filter.And(filterList);

            await UpdateAsync(filters, i => i.Name, "Chris.Sun");

            Console.WriteLine($"update data are:{Find(i => i.Gender && i.Age >= 20).ToList().ToJson()}");

            await DeleteAllAsync();
        }
        #endregion


        private void InitData()
        {
            var names = new[] { "Apple", "Beck", "Chris", "Durent", "English", "Jame", "Park", "Rose", "Nacy", "Wall" };
            for (var i = 0; i < 10; i++)
            {
                var entity = new Person
                {

                    Name = names[i],
                    Gender = i % 2 == 0,
                    Age = i + 15
                };

                Insert(entity);
            }
        }

    }


    public class Person : Entity
    {
        public string Name { get; set; }
        public bool Gender { get; set; }

        public int Age { get; set; }

    }
}
