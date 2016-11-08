using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Interview
{
    [TestFixture]
    public class Tests
    {

        #region Test Utility
        public class Object1 : IStoreable
        {
            public IComparable Id { get; set; }
            public string Item1 { get; set; }
            public int Item2 { get; set; }
            public bool Item3 { get; set; }
        }
        //Test Methond Get an Item Object
        public Object1 GetObject1(string item1, int item2, bool item3)
        {
            Object1 newObject = new Object1
            {
                Id = Guid.NewGuid(),
                Item1 = item1,
                Item2 = item2,
                Item3 = item3
            };
            return newObject;
        }

        //Test Methond Get an List of Objects
        public List<Object1> GetListObjects()
        {
            List<Object1> resultList = new List<Object1>();

            resultList.Add(GetObject1("Test1", 4, true));
            resultList.Add(GetObject1("Test2", 5, false));
            resultList.Add(GetObject1("Test3", 15, true));

            return resultList;
        }
        #endregion

        #region Tests
        //Add  Object
        [Test]
        public void AddObject()
        {
            // Arrange
            InMemoryRepository<Object1> controller = new InMemoryRepository<Object1>();

            // Act
            Object1 test = GetObject1("Test1", 4, true);
            controller.Save(test);
            controller.Commmit();
            Dictionary<Guid,Object1> results = controller.dbRepository;

            // Assert
            CollectionAssert.Contains(results.Values, test);
        }

        //IEnumerable All
        [Test]
        public void GetAll()
        {
            // Arrange
            InMemoryRepository<Object1> controller = new InMemoryRepository<Object1>();
            List<Object1> objectList = GetListObjects();

            // Act
            foreach (Object1 item in objectList)
            {
                controller.Save(item);
            }
            controller.Commmit();
            IEnumerable<Object1> results = controller.All();
            IEnumerable<Object1> resultTest = controller.dbRepository.Values;

            // Assert
            CollectionAssert.AreEqual(results, resultTest);
        }

        //Find Item
        [Test]
        public void FindById()
        {
            // Arrange
            InMemoryRepository<Object1> controller = new InMemoryRepository<Object1>();
            List<Object1> objectList = GetListObjects();

            // Act
            foreach (Object1 item in objectList)
            {
                controller.Save(item);
            }
            Object1 test = objectList[1];
            controller.Commmit();
            Object1 results = controller.FindById(test.Id);

            // Assert
            Assert.AreEqual(results, test);
        }

        //Delete Item
        [Test]
        public void Delete()
        {
            // Arrange
            InMemoryRepository<Object1> controller = new InMemoryRepository<Object1>();
            Object1 test = GetObject1("Test1", 4, true);

            // Act
            controller.Save(test);
            IEnumerable<Object1> resultTest = controller.dbRepository.Values;
            controller.Commmit();
            CollectionAssert.Contains(resultTest, test);
            controller.Delete(test.Id);
            controller.Commmit();

            resultTest = controller.dbRepository.Values;

            // Assert
            CollectionAssert.DoesNotContain(resultTest, test);
        }

        //Update test
        [Test]
        public void Update()
        {
            InMemoryRepository<Object1> controller = new InMemoryRepository<Object1>();
            Object1 test = GetObject1("Test1", 4, true);

            // Act
            controller.Save(test);
            controller.Commmit();
            test.Item2 = 5;
            controller.Update(test);
            controller.Commmit();
            Object1 result = controller.FindById(test.Id);
            
            // Assert
            Assert.AreEqual(result.Item2, test.Item2);
        }

        //RollBack test
        [Test]
        public void Rollback()
        {
            InMemoryRepository<Object1> controller = new InMemoryRepository<Object1>();
            List<Object1> objectList = GetListObjects();


            // Act
            foreach (Object1 item in objectList)
            {
                controller.Save(item);
            }
            controller.Delete(objectList[2].Id);
            objectList[2].Item2 = 2;
            controller.Update(objectList[2]);
            controller.RollBack();
            controller.Commmit();
            IEnumerable<Object1> check = controller.All();
            // Act

            // Assert
            CollectionAssert.IsEmpty(check);
        }


        //Mixed Test
        [Test]
        public void CompletedTest()
        {
            // Arrange
            InMemoryRepository<Object1> controller = new InMemoryRepository<Object1>();
            List<Object1> objectList = GetListObjects();
            IEnumerable<Object1> repoList = controller.dbRepository.Values;

            // Save Act
            foreach (Object1 item in objectList)
            {
                controller.Save(item);
            }

            // Delete Act
            controller.Delete(objectList[2].Id);
            objectList[0].Item2 = 25;
            controller.Update(objectList[0]);
            controller.Commmit();

            //Embty Assert
            CollectionAssert.Contains(repoList, objectList[0]);
            CollectionAssert.Contains(repoList, objectList[1]);
            CollectionAssert.DoesNotContain(repoList, objectList[2]);
        }
        #endregion Tests
    };
}
