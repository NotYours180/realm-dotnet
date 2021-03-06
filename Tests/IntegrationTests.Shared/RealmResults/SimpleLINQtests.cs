﻿////////////////////////////////////////////////////////////////////////////
//
// Copyright 2016 Realm Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
////////////////////////////////////////////////////////////////////////////
 
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NUnit.Framework;
using Realms;
using IntegrationTests.Shared;

namespace IntegrationTests
{
    [TestFixture, Preserve(AllMembers = true)]
    class SimpleLINQtests : PeopleTestsBase
    {
        // see comment on base method why this isn't decorated with [SetUp]
        public override void Setup()
        {
            base.Setup();
            MakeThreePeople();
        }

        [Test]
        public void CreateList()
        {
            var s0 = _realm.All<Person>().Where(p => p.Score == 42.42f).ToList();
            Assert.That(s0.Count(), Is.EqualTo(1));
            Assert.That(s0 [0].Score, Is.EqualTo(42.42f));


            var s1 = _realm.All<Person>().Where(p => p.Longitude < -70.0 && p.Longitude > -90.0).ToList();
            Assert.That(s1 [0].Email, Is.EqualTo("john@doe.com"));

            var s2 = _realm.All<Person>().Where(p => p.Longitude < 0).ToList();
            Assert.That(s2.Count(), Is.EqualTo(2));
            Assert.That(s2 [0].Email, Is.EqualTo("john@doe.com"));
            Assert.That(s2 [1].Email, Is.EqualTo("peter@jameson.net"));

            var s3 = _realm.All<Person>().Where(p => p.Email != "");
            Assert.That(s3.Count(), Is.EqualTo(3));
        }


        [Test]
        public void CountWithNot()
        {
            var countSimpleNot = _realm.All<Person>().Where(p => !p.IsInteresting).Count();
            Assert.That(countSimpleNot, Is.EqualTo(1));

            var countSimpleNot2 = _realm.All<Person>().Count(p => !p.IsInteresting);
            Assert.That(countSimpleNot2, Is.EqualTo(1));

            var countNotEqual = _realm.All<Person>().Where(p => !(p.Score == 42.42f)).Count();
            Assert.That(countNotEqual, Is.EqualTo(2));

            var countNotComplex = _realm.All<Person>().Where(p => !(p.Longitude < -70.0 && p.Longitude > -90.0)).Count();
            Assert.That(countNotComplex, Is.EqualTo(2));
        }


        [Test]
        public void CountFoundItems()
        {
            var r0 = _realm.All<Person>().Where(p => p.Score == 42.42f);
            var c0 = r0.Count();  // defer so can check in debugger if RealmResults.Count() evaluated correctly
            Assert.That(c0, Is.EqualTo(1));

            var c1 = _realm.All<Person>().Where(p => p.Latitude <= 50).Count();
            Assert.That(c1, Is.EqualTo(2));

            var c2 = _realm.All<Person>().Where(p => p.IsInteresting).Count();
            Assert.That(c2, Is.EqualTo(2));

            var c3 = _realm.All<Person>().Where(p => p.FirstName == "John").Count();
            Assert.That(c3, Is.EqualTo(2));

            var c4 = _realm.All<Person>().Count(p => p.FirstName == "John");
            Assert.That(c4, Is.EqualTo(2));
        }


        // added to pick up a nasty side-effect from casting
        [Test]
        public void CountFoundWithCasting()
        {
            var r0 = _realm.All<Person>().Where(p => p.Score == 42.42f);
            var r1 = r0 as RealmResults<Person>;  // this is its runtime type but r0's Compile Time type is IQueryable<Person>
            var c0 = r1.Count();  // invokes RealmResults<T>.Count() shortcut method
            Assert.That(c0, Is.EqualTo(1));
        }


        [Test]
        public void CountFails()
        {
            var c0 = _realm.All<Person>().Where(p => p.Score == 3.14159f).Count();
            Assert.That(c0, Is.EqualTo(0));

            var c1 = _realm.All<Person>().Where(p => p.Latitude > 88).Count();
            Assert.That(c1, Is.EqualTo(0));

            var c3 = _realm.All<Person>().Where(p => p.FirstName == "Samantha").Count();
            Assert.That(c3, Is.EqualTo(0));
        }


        // Extension method rather than SQL-style LINQ
        // Also tests the Count on results, ElementOf, First and Single methods
        [Test]
        public void SearchComparingFloat()
        {
            var s0 = _realm.All<Person>().Where(p => p.Score == 42.42f);
            var s0l = s0.ToList();
            Assert.That(s0.Count(), Is.EqualTo(1));
            Assert.That(s0l [0].Score, Is.EqualTo(42.42f));

            var s1 = _realm.All<Person>().Where(p => p.Score != 100.0f).ToList();
            Assert.That(s1.Count, Is.EqualTo(2));
            Assert.That(s1 [0].Score, Is.EqualTo(-0.9907f));
            Assert.That(s1 [1].Score, Is.EqualTo(42.42f));

            var s2 = _realm.All<Person>().Where(p => p.Score < 0).ToList();
            Assert.That(s2.Count, Is.EqualTo(1));
            Assert.That(s2 [0].Score, Is.EqualTo(-0.9907f));

            var s3 = _realm.All<Person>().Where(p => p.Score <= 42.42f).ToList();
            Assert.That(s3.Count, Is.EqualTo(2));
            Assert.That(s3 [0].Score, Is.EqualTo(-0.9907f));
            Assert.That(s3 [1].Score, Is.EqualTo(42.42f));

            var s4 = _realm.All<Person>().Where(p => p.Score > 99.0f).ToList();
            Assert.That(s4.Count, Is.EqualTo(1));
            Assert.That(s4 [0].Score, Is.EqualTo(100.0f));

            var s5 = _realm.All<Person>().Where(p => p.Score >= 100).ToList();
            Assert.That(s5.Count, Is.EqualTo(1));
            Assert.That(s5 [0].Score, Is.EqualTo(100.0f));
        }

        [Test]
        public void SearchComparingDouble()
        {
            var s0 = _realm.All<Person>().Where(p => p.Latitude == 40.7637286);
            Assert.That(s0.Count, Is.EqualTo(1));
            Assert.That(s0.ToList() [0].Latitude, Is.EqualTo(40.7637286));

            var s1 = _realm.All<Person>().Where(p => p.Latitude != 40.7637286).ToList();
            Assert.That(s1.Count, Is.EqualTo(2));
            Assert.That(s1 [0].Latitude, Is.EqualTo(51.508530));
            Assert.That(s1 [1].Latitude, Is.EqualTo(37.7798657));

            var s2 = _realm.All<Person>().Where(p => p.Latitude < 40).ToList();
            Assert.That(s2.Count, Is.EqualTo(1));
            Assert.That(s2 [0].Latitude, Is.EqualTo(37.7798657));

            var s3 = _realm.All<Person>().Where(p => p.Latitude <= 40.7637286).ToList();
            Assert.That(s3.Count, Is.EqualTo(2));
            Assert.That(s3 [0].Latitude, Is.EqualTo(40.7637286));
            Assert.That(s3 [1].Latitude, Is.EqualTo(37.7798657));

            var s4 = _realm.All<Person>().Where(p => p.Latitude > 50).ToList();
            Assert.That(s4.Count, Is.EqualTo(1));
            Assert.That(s4 [0].Latitude, Is.EqualTo(51.508530));

            var s5 = _realm.All<Person>().Where(p => p.Latitude >= 51.508530).ToList();
            Assert.That(s5.Count, Is.EqualTo(1));
            Assert.That(s5 [0].Latitude, Is.EqualTo(51.508530));
        }

        [Test]
        public void SearchComparingLong()
        {
            var equality = _realm.All<Person>().Where(p => p.Salary == 60000).ToArray();
            Assert.That(equality.Length, Is.EqualTo(1));
            Assert.That(equality [0].FullName, Is.EqualTo("John Doe"));

            var lessThan = _realm.All<Person>().Where(p => p.Salary < 50000).ToArray();
            Assert.That(lessThan.Length, Is.EqualTo(1));
            Assert.That(lessThan [0].FullName, Is.EqualTo("John Smith"));

            var lessOrEqualThan = _realm.All<Person>().Where(p => p.Salary <= 60000).ToArray();
            Assert.That(lessOrEqualThan.Length, Is.EqualTo(2));
            Assert.That(lessOrEqualThan.All(p => p.FirstName == "John"), Is.True);

            var greaterThan = _realm.All<Person>().Where(p => p.Salary > 80000).ToArray();
            Assert.That(greaterThan.Length, Is.EqualTo(1));
            Assert.That(greaterThan [0].FullName, Is.EqualTo("Peter Jameson"));

            var greaterOrEqualThan = _realm.All<Person>().Where(p => p.Salary >= 60000).ToArray();
            Assert.That(greaterOrEqualThan.Length, Is.EqualTo(2));
            Assert.That(greaterOrEqualThan.Any(p => p.FullName == "John Doe") && greaterOrEqualThan.Any(p => p.FullName == "Peter Jameson"), Is.True);

            var between = _realm.All<Person>().Where(p => p.Salary > 30000 && p.Salary < 87000).ToArray();
            Assert.That(between.Length, Is.EqualTo(1));
            Assert.That(between [0].FullName, Is.EqualTo("John Doe"));
        }

        [Test]
        public void SearchComparingString()
        {
            var equality = _realm.All<Person>().Where(p => p.LastName == "Smith").ToArray();
            Assert.That(equality.Length, Is.EqualTo(1));
            Assert.That(equality [0].FullName, Is.EqualTo("John Smith"));

            var contains = _realm.All<Person>().Where(p => p.FirstName.Contains("et")).ToArray();
            Assert.That(contains.Length, Is.EqualTo(1));
            Assert.That(contains [0].FullName, Is.EqualTo("Peter Jameson"));

            var startsWith = _realm.All<Person>().Where(p => p.Email.StartsWith("john@")).ToArray();
            Assert.That(startsWith.Length, Is.EqualTo(2));
            Assert.That(startsWith.All(p => p.FirstName == "John"), Is.True);

            var endsWith = _realm.All<Person>().Where(p => p.Email.EndsWith(".net")).ToArray();
            Assert.That(endsWith.Length, Is.EqualTo(1));
            Assert.That(endsWith [0].FullName, Is.EqualTo("Peter Jameson"));

            var @null = _realm.All<Person>().Where(p => p.OptionalAddress == null).ToArray();
            Assert.That(@null [0].FullName, Is.EqualTo("Peter Jameson"));

            var empty = _realm.All<Person>().Where(p => p.OptionalAddress == "").ToArray();
            Assert.That(empty [0].FullName, Is.EqualTo("John Doe"));

            var null_or_empty = _realm.All<Person>().Where(p => string.IsNullOrEmpty(p.OptionalAddress));
            Assert.That(null_or_empty.Count(), Is.EqualTo(2));
        }

        [Test]
        public void SearchComparingDateTimeOffset()
        {
            var d1960 = new DateTimeOffset(1960, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var d1970 = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);
            var bdayJohnDoe = new DateTimeOffset(1963, 4, 14, 0, 0, 0, TimeSpan.Zero);
            var bdayPeterJameson = new DateTimeOffset(1989, 2, 25, 0, 0, 0, TimeSpan.Zero);

            var equality = _realm.All<Person>().Where(p => p.Birthday == bdayPeterJameson).ToArray();
            Assert.That(equality.Length, Is.EqualTo(1));
            Assert.That(equality [0].FullName, Is.EqualTo("Peter Jameson"));

            var lessThan = _realm.All<Person>().Where(p => p.Birthday < d1960).ToArray();
            Assert.That(lessThan.Length, Is.EqualTo(1));
            Assert.That(lessThan [0].FullName, Is.EqualTo("John Smith"));

            var lessOrEqualThan = _realm.All<Person>().Where(p => p.Birthday <= bdayJohnDoe).ToArray();
            Assert.That(lessOrEqualThan.Length, Is.EqualTo(2));
            Assert.That(lessOrEqualThan.All(p => p.FirstName == "John"), Is.True);

            var greaterThan = _realm.All<Person>().Where(p => p.Birthday > d1970).ToArray();
            Assert.That(greaterThan.Length, Is.EqualTo(1));
            Assert.That(greaterThan [0].FullName, Is.EqualTo("Peter Jameson"));

            var greaterOrEqualThan = _realm.All<Person>().Where(p => p.Birthday >= bdayJohnDoe).ToArray();
            Assert.That(greaterOrEqualThan.Length, Is.EqualTo(2));
            Assert.That(greaterOrEqualThan.Any(p => p.FullName == "John Doe") && greaterOrEqualThan.Any(p => p.FullName == "Peter Jameson"), Is.True);

            var between = _realm.All<Person>().Where(p => p.Birthday > d1960 && p.Birthday < d1970).ToArray();
            Assert.That(between.Length, Is.EqualTo(1));
            Assert.That(between [0].FullName, Is.EqualTo("John Doe"));
        }

        [Test]
        public void SearchComparingNullable()
        {
            var @null = _realm.All<Person>().Where(p => p.IsAmbivalent == null);
            Assert.That(@null.Single().FullName, Is.EqualTo("Peter Jameson"));

            var not_null = _realm.All<Person>().Where(p => p.IsAmbivalent != null);
            Assert.That(not_null.Count(), Is.EqualTo(2));

            var @true = _realm.All<Person>().Where(p => p.IsAmbivalent == true);
            Assert.That(@true.Single().FullName, Is.EqualTo("John Smith"));

            var @false = _realm.All<Person>().Where(p => p.IsAmbivalent == false);
            Assert.That(@false.Single().FullName, Is.EqualTo("John Doe"));
        }

        [Test]
        public void SearchComparingByteArrays()
        {
            var DEADBEEF = new byte [] { 0xde, 0xad, 0xbe, 0xef };
            var CAFEBABE = new byte [] { 0xca, 0xfe, 0xba, 0xbe };
            var EMPTY = new byte [0];

            var equality = _realm.All<Person>().Where(p => p.PublicCertificateBytes == CAFEBABE);
            Assert.That(equality.Single().PublicCertificateBytes, Is.EqualTo(CAFEBABE));

            var unequality = _realm.All<Person>().Where(p => p.PublicCertificateBytes != DEADBEEF);
            Assert.That(unequality.Count(), Is.EqualTo(2));

            var empty = _realm.All<Person>().Where(p => p.PublicCertificateBytes == EMPTY);
            Assert.That(empty, Is.Empty);

            // we should support this as well - see #570
            //var @null = _realm.All<Person>().Where(p => p.PublicCertificateBytes == null);
            //Assert.That(@null.Count(), Is.EqualTo(1));
        }

        [Test]
        public void SearchComparingChar()
        {
            _realm.Write( () => {
                var c1 = _realm.CreateObject<PrimaryKeyCharObject>();
                c1.CharProperty = 'A';
                var c2 = _realm.CreateObject<PrimaryKeyCharObject>();
                c2.CharProperty = 'B';
                var c3 = _realm.CreateObject<PrimaryKeyCharObject>();
                c3.CharProperty = 'c';
                var c4 = _realm.CreateObject<PrimaryKeyCharObject>();
                c4.CharProperty = 'a';
            });
            var equality = _realm.All<PrimaryKeyCharObject>().Where(p => p.CharProperty == 'A').ToArray();
            Assert.That(equality.Select(p => p.CharProperty), Is.EquivalentTo(
                new []{'A'}));
            

            var inequality =_realm.All<PrimaryKeyCharObject>().Where(p => p.CharProperty != 'c').ToArray();
            Assert.That(inequality.Select(p => p.CharProperty), Is.EquivalentTo(
                new []{'A', 'B', 'a'}));

            var lessThan =_realm.All<PrimaryKeyCharObject>().Where(p => p.CharProperty < 'c').ToArray();
            Assert.That(lessThan.Select(p => p.CharProperty), Is.EquivalentTo(
                new []{'A', 'B', 'a'}));

            var lessOrEqualThan =_realm.All<PrimaryKeyCharObject>().Where(p => p.CharProperty <= 'c').ToArray();
            Assert.That(lessOrEqualThan.Select(p => p.CharProperty), Is.EquivalentTo(
                new []{'A', 'B', 'a', 'c'}));

            var greaterThan =_realm.All<PrimaryKeyCharObject>().Where(p => p.CharProperty > 'a').ToArray();
            Assert.That(greaterThan.Select(p => p.CharProperty), Is.EquivalentTo(
                new []{'c'}));

            var greaterOrEqualThan =_realm.All<PrimaryKeyCharObject>().Where(p => p.CharProperty >= 'B').ToArray();
            Assert.That(greaterOrEqualThan.Select(p => p.CharProperty), Is.EquivalentTo(
                new []{'B', 'a', 'c'}));

            var between =_realm.All<PrimaryKeyCharObject>().Where(p => p.CharProperty > 'A' && p.CharProperty < 'a').ToArray();
            Assert.That(between.Select(p => p.CharProperty), Is.EquivalentTo(
                new []{'B'}));
            
            var missing = _realm.All<PrimaryKeyCharObject>().Where(p => p.CharProperty == 'X').ToArray();
            Assert.That(missing.Length, Is.EqualTo(0));
        }


        [Test]
        public void AnySucceeds()
        {
            Assert.That(_realm.All<Person>().Where(p => p.Latitude > 50).Any());
            Assert.That(_realm.All<Person>().Where(p => p.Score > 0).Any());
            Assert.That(_realm.All<Person>().Where(p => p.IsInteresting == false).Any());
            Assert.That(_realm.All<Person>().Where(p => p.FirstName == "John").Any());
        }


        [Test]
        public void AnyFails()
        {
            Assert.False(_realm.All<Person>().Where(p => p.Latitude > 100).Any());
            Assert.False(_realm.All<Person>().Where(p => p.Score > 50000).Any());
            Assert.False(_realm.All<Person>().Where(p => p.FirstName == "Samantha").Any());
        }


        [Test]
        public void SingleFailsToFind()
        {
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Single(p => p.Latitude > 100));
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Where(p => p.Latitude > 100).Single());
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Single(p => p.Score > 50000));
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Single(p => p.FirstName == "Samantha"));
        }


        [Test]
        public void SingleFindsTooMany()
        {
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Where(p => p.Latitude == 50).Single());
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Single(p => p.Score != 100.0f));
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Single(p => p.FirstName == "John"));
        }


        [Test]
        public void SingleWorks()
        {
            var s0 = _realm.All<Person>().Single(p => p.Longitude < -70.0 && p.Longitude > -90.0);
            Assert.That(s0.Email, Is.EqualTo("john@doe.com"));

            var s1 = _realm.All<Person>().Where(p => p.Score == 100.0f).Single();
            Assert.That(s1.Email, Is.EqualTo("john@doe.com"));

            var s2 = _realm.All<Person>().Single(p => p.FirstName == "Peter");
            Assert.That(s2.FirstName, Is.EqualTo("Peter"));
        }


        [Test]
        public void SingleOrDefaultWorks()
        {
            var s0 = _realm.All<Person>().SingleOrDefault(p => p.Longitude < -70.0 && p.Longitude > -90.0);
            Assert.That(s0.Email, Is.EqualTo("john@doe.com"));

            var s1 = _realm.All<Person>().Where(p => p.Score == 100.0f).SingleOrDefault();
            Assert.That(s1.Email, Is.EqualTo("john@doe.com"));

            var s2 = _realm.All<Person>().SingleOrDefault(p => p.FirstName == "Peter");
            Assert.That(s2.FirstName, Is.EqualTo("Peter"));
        }


        [Test]
        public void SingleOrDefaultReturnsDefault()
        {
            var expectedDef = _realm.All<Person>().SingleOrDefault(p => p.FirstName == "Zaphod");
            Assert.That(expectedDef, Is.Null);

            expectedDef = _realm.All<Person>().Where(p => p.FirstName == "Just Some Guy").SingleOrDefault();
            Assert.That(expectedDef, Is.Null);
        }


        [Test]
        public void FirstFailsToFind()
        {
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().First(p => p.Latitude > 100));
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Where(p => p.Latitude > 100).First());
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().First(p => p.FirstName == "Samantha"));
        }


        [Test]
        public void FirstWorks()
        {
            var s0 = _realm.All<Person>().First(p => p.Longitude < -70.0 && p.Longitude > -90.0);
            Assert.That(s0.Email, Is.EqualTo("john@doe.com"));

            var s1 = _realm.All<Person>().Where(p => p.Score == 100.0f).First();
            Assert.That(s1.Email, Is.EqualTo("john@doe.com"));

            var s2 = _realm.All<Person>().First(p => p.FirstName == "John");
            Assert.That(s2.FirstName, Is.EqualTo("John"));
        }


        [Test]
        public void FirstOrDefaultWorks()
        {
            var s0 = _realm.All<Person>().FirstOrDefault(p => p.Longitude < -70.0 && p.Longitude > -90.0);
            Assert.That(s0.Email, Is.EqualTo("john@doe.com"));

            var s1 = _realm.All<Person>().Where(p => p.Score == 100.0f).FirstOrDefault();
            Assert.That(s1.Email, Is.EqualTo("john@doe.com"));

            var s2 = _realm.All<Person>().FirstOrDefault(p => p.FirstName == "John");
            Assert.That(s2.FirstName, Is.EqualTo("John"));
        }


        [Test]
        public void FirstOrDefaultReturnsDefault()
        {
            var expectedDef = _realm.All<Person>().FirstOrDefault(p => p.FirstName == "Zaphod");
            Assert.That(expectedDef, Is.Null);

            expectedDef = _realm.All<Person>().Where(p => p.FirstName == "Just Some Guy").FirstOrDefault();
            Assert.That(expectedDef, Is.Null);
        }


        [Test]
        public void LastFailsToFind()
        {
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Last(p => p.Latitude > 100));
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Where(p => p.Latitude > 100).Last());
            Assert.Throws<InvalidOperationException>(() => _realm.All<Person>().Last(p => p.LastName == "Samantha"));
        }


        [Test]
        public void LastWorks()
        {
            var s0 = _realm.All<Person>().Last(p => p.Longitude < -70.0 && p.Longitude > -90.0);
            Assert.That(s0.Email, Is.EqualTo("john@doe.com"));  // Last same as First when one match

            var s1 = _realm.All<Person>().Where(p => p.Score == 100.0f).Last();
            Assert.That(s1.Email, Is.EqualTo("john@doe.com"));

            var s2 = _realm.All<Person>().Last(p => p.FirstName == "John");
            Assert.That(s2.FirstName, Is.EqualTo("John"));  // order not guaranteed in two items but know they match this
        }


        [Test]
        public void LastOrDefaultWorks()
        {
            var s0 = _realm.All<Person>().LastOrDefault(p => p.Longitude < -70.0 && p.Longitude > -90.0);
            Assert.That(s0.Email, Is.EqualTo("john@doe.com"));  // Last same as First when one match

            var s1 = _realm.All<Person>().Where(p => p.Score == 100.0f).LastOrDefault();
            Assert.That(s1.Email, Is.EqualTo("john@doe.com"));

            var s2 = _realm.All<Person>().LastOrDefault(p => p.FirstName == "John");
            Assert.That(s2.FirstName, Is.EqualTo("John"));  // order not guaranteed in two items but know they match this
        }


        [Test]
        public void LastOrDefaultReturnsDefault()
        {
            var expectedDef = _realm.All<Person>().LastOrDefault(p => p.FirstName == "Zaphod");
            Assert.That(expectedDef, Is.Null);

            expectedDef = _realm.All<Person>().Where(p => p.FirstName == "Just Some Guy").LastOrDefault();
            Assert.That(expectedDef, Is.Null);
        }


        [Test]
        public void ElementAtOutOfRange()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => _realm.All<Person>().Where(p => p.Latitude > 140).ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _realm.All<Person>().Where(p => p.FirstName == "Samantha").ElementAt(0));
            Assert.Throws<ArgumentOutOfRangeException>(() => _realm.All<Person>().Where(p => p.FirstName == "Peter").ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => _realm.All<Person>().ElementAt(10));
            Assert.Throws<ArgumentOutOfRangeException>(() => _realm.All<Person>().ElementAt(-1));
        }


        [Test]
        public void ElementAtInRange()
        {
            var s0 = _realm.All<Person>().Where(p => p.Longitude < -70.0 && p.Longitude > -90.0).ElementAt(0);
            Assert.That(s0.Email, Is.EqualTo("john@doe.com")); 

            var s1 = _realm.All<Person>().Where(p => p.Score == 100.0f).ElementAt(0);
            Assert.That(s1.Email, Is.EqualTo("john@doe.com"));

            var s2 = _realm.All<Person>().Where(p => p.FirstName == "John").ElementAt(1);
            Assert.That(s2.FirstName, Is.EqualTo("John")); 

            var s3 = _realm.All<Person>().ElementAt(2);
            Assert.That(s3.FirstName, Is.EqualTo("Peter")); 
        }


        [Test]
        public void ElementAtOrDefaultInRange()
        {
            var s0 = _realm.All<Person>().Where(p => p.Longitude < -70.0 && p.Longitude > -90.0).ElementAtOrDefault(0);
            Assert.That(s0.Email, Is.EqualTo("john@doe.com")); 

            var s1 = _realm.All<Person>().Where(p => p.Score == 100.0f).ElementAtOrDefault(0);
            Assert.That(s1.Email, Is.EqualTo("john@doe.com"));

            var s2 = _realm.All<Person>().Where(p => p.FirstName == "John").ElementAtOrDefault(1);
            Assert.That(s2.FirstName, Is.EqualTo("John")); 

            var s3 = _realm.All<Person>().ElementAtOrDefault(2);
            Assert.That(s3.FirstName, Is.EqualTo("Peter")); 
        }


        [Test]
        public void ElementAtOrDefaultReturnsDefault()
        {
            var expectedDef = _realm.All<Person>().Where(p => p.FirstName == "Just Some Guy").ElementAtOrDefault(0);
            Assert.That(expectedDef, Is.Null);
        }


        // note that DefaultIfEmpty returns a collection of one item
        [Test, Explicit("Currently broken and hard to implement")]
        public void DefaultIfEmptyReturnsDefault()
        {
            /*
             * This is comprehensively broken and awkward to fix in our current architecture.
             * Looking at the test code below, the Count is invoked on a RealmResults and directly 
             * invokes its query handle, which of course has zero elements.
             * One posible approach is to toggle the RealmResults into a special state where
             * it acts as a generator for a single null pointer.* 
             */
            var expectCollectionOfOne = _realm.All<Person>().Where(p => p.FirstName == "Just Some Guy").DefaultIfEmpty();
            Assert.That(expectCollectionOfOne.Count(), Is.EqualTo(1));
            var expectedDef = expectCollectionOfOne.Single();
            Assert.That(expectedDef, Is.Null);
        }


        [Test]
        public void ChainedSearch()
        {
            var moderateScorers = _realm.All<Person>().Where(p => p.Score >= 20.0f && p.Score <= 100.0f);
            var johnScorer = moderateScorers.Where(p => p.FirstName == "John").First();
            Assert.That(johnScorer, Is.Not.Null);
            Assert.That(johnScorer.Score, Is.EqualTo(100.0f));
            Assert.That(johnScorer.FullName, Is.EqualTo("John Doe"));
        }

    } // SimpleLINQtests
}
