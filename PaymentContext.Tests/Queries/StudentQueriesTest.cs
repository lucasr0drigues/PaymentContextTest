using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Queries;
using PaymentContext.Domain.ValueObjects;
using System.Collections.Generic;
using System.Linq;

namespace PaymentContext.Tests.Entities;

[TestClass]
public class StudentQueriesTest
{
    // Red, Green, Refactor

    private IList<Student> _students;

    public StudentQueriesTest()
    {
        for(var i = 0; i < 10; i++)
        {
            _students.Add(new Student(new Name("Aluno",i.ToString()),new Document("1234567890" + i.ToString(),EDocumentType.CPF),new Email(i.ToString() + "@test.com")));
        }
    }

    [TestMethod]
    public void ShouldReturnNullWhenDocumentNotExists()
    {
        var exp = StudentQueries.GetStudentInfo("12345678901");
        var studn = _students.AsQueryable().Where(exp).FirstOrDefault();

        Assert.AreEqual(null, studn);
    }

    [TestMethod]
    public void ShouldReturnStudentWhenDocumentExists()
    {
        var exp = StudentQueries.GetStudentInfo("12345678901");
        var studn = _students.AsQueryable().Where(exp).FirstOrDefault();

        Assert.AreNotEqual(null, studn);
    }

}