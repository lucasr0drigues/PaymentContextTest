using Flunt.Notifications;
using Flunt.Validations;
using PaymentContext.Domain.Commands;
using PaymentContext.Domain.Entities;
using PaymentContext.Domain.Enums;
using PaymentContext.Domain.Repositories;
using PaymentContext.Domain.Services;
using PaymentContext.Domain.ValueObjects;
using PaymentContext.Shared.Commands;
using PaymentContext.Shared.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentContext.Domain.Handlers
{
    public class SubscriptionHandler : Notifiable, IHandler<CreateBoletoSubscriptionCommand>, IHandler<CreatePayPalSubscriptionCommand>
    {
        private readonly IStudentRepository _repository;
        private readonly IEmailService _emailService;

        public SubscriptionHandler(IStudentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        public ICommandResult Handle(CreateBoletoSubscriptionCommand command)
        {
            // fail fast validations
            command.Validate();
            if (command.Invalid)
            {
                AddNotifications(command);
                return new CommandResult(false, "Não foi possivel realizada sua assinatura");
            }

            // verificar se documento já está cadastrado
            if (_repository.DocumentExists(command.Document))
            {
                AddNotification("Document", "Este CPF já está em uso");
            }

            // verificar se email já está cadastrado
            if (_repository.EmailExists(command.Email))
            {
                AddNotification("Email", "Este Email já está em uso");
            }

            // gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            // gerar as entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            var payment = new BoletoPayment(command.BarCode,command.BoletoNumber,command.PaidDate,command.ExpireDate,command.Total,command.TotalPaid
                ,command.Payer,new Document(command.PayerDocument,command.PayerDocumentType),address,email);

            // relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // agrupar as validações
            AddNotifications(name,document,address,student,subscription,payment);

            // checar as notificações
            if (Invalid)
            {
                return new CommandResult(false, "Não foi possivel realizar sua assinatura");
            }

            // salvar as informações
            _repository.CreateSubscription(student);

            // enviar email de boas vindas
            _emailService.SendEmail(student.Name.ToString(), student.Email.Address, "Bem vindo ao curso", "Sua assinatura foi criada");

            // retornar informações
            return new CommandResult(true, "Assinatura realizada com sucesso");
        }

        public ICommandResult Handle(CreatePayPalSubscriptionCommand command)
        {

            // verificar se documento já está cadastrado
            if (_repository.DocumentExists(command.Document))
            {
                AddNotification("Document", "Este CPF já está em uso");
            }

            // verificar se email já está cadastrado
            if (_repository.EmailExists(command.Email))
            {
                AddNotification("Email", "Este Email já está em uso");
            }

            // gerar os VOs
            var name = new Name(command.FirstName, command.LastName);
            var document = new Document(command.Document, EDocumentType.CPF);
            var email = new Email(command.Email);
            var address = new Address(command.Street, command.Number, command.Neighborhood, command.City, command.State, command.Country, command.ZipCode);

            // gerar as entidades
            var student = new Student(name, document, email);
            var subscription = new Subscription(DateTime.Now.AddMonths(1));
            // só muda a implementação do pagamento 
            var payment = new PayPalPayment(command.TransactionCode, command.PaidDate, command.ExpireDate, command.Total, command.TotalPaid
                , command.Payer, new Document(command.PayerDocument, command.PayerDocumentType), address, email);

            // relacionamentos
            subscription.AddPayment(payment);
            student.AddSubscription(subscription);

            // agrupar as validações
            AddNotifications(name, document, address, student, subscription, payment);

            // checar as notificações
            if (Invalid)
            {
                return new CommandResult(false, "Não foi possivel realizar sua assinatura");
            }

            // salvar as informações
            _repository.CreateSubscription(student);

            // enviar email de boas vindas
            _emailService.SendEmail(student.Name.ToString(), student.Email.Address, "Bem vindo ao curso", "Sua assinatura foi criada");

            // retornar informações
            return new CommandResult(true, "Assinatura realizada com sucesso");
        }
    }
}
