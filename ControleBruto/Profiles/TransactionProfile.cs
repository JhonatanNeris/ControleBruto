using AutoMapper;
using ControleBruto.Data.Dtos.Transaction;
using ControleBruto.Models;

namespace ControleBruto.Profiles
{
    public class TransactionProfile : Profile
    {
        public TransactionProfile()
        {
            CreateMap<CreateTransactionDto, Transaction>();
            CreateMap<UpdateTransactionDto, Transaction>();
            CreateMap<Transaction, ReadTransactionDto>();

        }
    }
}
