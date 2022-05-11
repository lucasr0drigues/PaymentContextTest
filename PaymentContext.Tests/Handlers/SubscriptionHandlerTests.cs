using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Handlers;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Tests.Mocks;
using System;

namespace PaymentContext.Tests.Entities;

[TestClass]
public class SubscriptionHandlerTests
{

    [TestMethod]
    public void ShouldReturnErrorWhenDocumentExists()
    {
        var handler = new SubscriptionHandler(new FakeStudentRepository(), new FakeEmailService());
        var command = new CreateBoletoSubscriptionCommand();
        
        command.FirstName = "TestFirstName";
        command.LastName = "TestLastName";
        command.Document = "99999999999";
        command.Email = "test2@email.com";

        command.BarCode = "123456789900";
        command.BoletoNumber = "23423423432";

        command.PaymentNumber = "12323545";
        command.PaidDate = DateTime.Now;
        command.ExpireDate = DateTime.Now.AddMonths(1);
        command.Total = 60;
        command.TotalPaid = 60;
        command.Payer = "TestPayer";
        command.PayerDocument = "123456789900";
        command.PayerDocumentType = EDocumentType.CPF;
        command.PayerEmail = "test3@email.com";

        command.Street = "TestStreet";
        command.Number = "TestNumber";
        command.Neighborhood = "TestNeighborhood";
        command.City = "TestCity";
        command.State = "TestState";
        command.Country = "TestCountry";
        command.ZipCode = "TestZipCode";

        handler.Handle(command);
        Assert.AreEqual(false, handler.Valid);


    }

}